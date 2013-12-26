using IOCH;
using log4net;
using OCHEntity;
using OCHUtil;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MessageStoreImpl
{
    public class SQL : IMessageStore
    {
        private string connectionString = Properties.Settings.Default.OCHConnectionString;
        private int pageSize = 100;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        public void SaveMessage(DateTime dtBeginTime, string messageBody, string[] contracts)
        {
            messageBody = MessageFormatter.FormatSendTimeStamp(messageBody);

            string compress = SevenZip.Compress(messageBody);

            bool isCompressed = compress.Length < messageBody.Length;

            foreach (var c in contracts)
            {
                Contact contract = SaveContract(c, dtBeginTime);
                SaveContractCoversationDailyDate(contract, dtBeginTime);
                SaveMessage(contract, isCompressed ? compress : messageBody, dtBeginTime, isCompressed);
            }
        }

        private Contact SaveContract(string ContactName, DateTime date)
        {
            Contact c = new Contact();

            using (SqlConnection connection = new SqlConnection(
                 connectionString))
            {
                connection.Open();

                using(SqlCommand command = new SqlCommand(
                "SaveContract", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ContactName", ContactName);
                    command.Parameters.AddWithValue("@LastTime", date);

                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            c.Id = int.Parse(reader["Id"].ToString());
                            c.ContactName = reader["ContactName"].ToString();
                            c.FriendlyName = reader["FriendlyName"].ToString();
                            c.LastConversationTime = DateTime.Parse(reader["LastConversationTime"].ToString());
                        }
                    }
                }
            }

            return c;
        }

        private void SaveContractCoversationDailyDate(Contact contract, DateTime date)
        {
            if (contract != null)
            {
                using (SqlConnection connection = new SqlConnection(
                 connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(
                    "SaveContractCoversationDailyDate", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ContactId", contract.Id);
                        command.Parameters.AddWithValue("@MessageDate", date.Date);

                        int result = command.ExecuteNonQuery();
                    }
                }
            }
        }

        private void SaveMessage(
            Contact contract, 
            string messageBody, 
            DateTime dtBeginTime, 
            bool isCompressed)
        {
            using (SqlConnection connection = new SqlConnection(
                 connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SaveMessage", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ContactId", contract.Id);
                    command.Parameters.AddWithValue("@MessageTime", dtBeginTime);
                    command.Parameters.AddWithValue("@MessageText", messageBody);
                    command.Parameters.AddWithValue("@IsCompressed", isCompressed);

                    int result = command.ExecuteNonQuery();
                }
            }

        }

        public List<Contact> GetContractList()
        {
            List<Contact> list = new List<Contact>();

            using (SqlConnection connection =
                    new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetAllContract", connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Contact c = new Contact();
                            c.Id = int.Parse(reader["Id"].ToString());
                            c.ContactName = reader["ContactName"].ToString();
                            c.FriendlyName = reader["FriendlyName"].ToString();
                            c.LastConversationTime = DateTime.Parse(reader["LastConversationTime"].ToString());

                            list.Add(c);
                        }
                    }
                }                
            }

            return list;
        }

        public void GetQueryMessageList(DateTime dtStart, DateTime dtEnd, string keyword, List<Contact> contractLit, List<OCMessage> messageList)
        {
            List<Contact> allContracts = GetContractList();

            if (allContracts == null)
            {
                return;
            }

            foreach (Contact contract in allContracts)
            {
                var list = GetOCMessage(contract, dtStart, dtEnd);

                if (list != null && list.Count > 0)
                {
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        foreach (var message in list)
                        {
                            string plainText = HtmlUtil.ConvertFromHtml(message.MessageText);

                            if (plainText.ToUpper().IndexOf(keyword.ToUpper()) != -1)
                            {
                                messageList.Add(message);

                                if (!contractLit.Contains(contract))
                                {
                                    contractLit.Add(contract);
                                }
                            }
                        }
                    }
                    else
                    {
                        messageList.AddRange(list);
                        contractLit.Add(contract);
                    }
                }
            }
        }

        public List<DateTime> GetCoversationDateList(Contact contract, DateTime searchDate, SearchDirection direction = SearchDirection.None)
        {
            List<DateTime> list = new List<DateTime>();

            if ((searchDate == DateTime.MaxValue 
                || searchDate == DateTime.MinValue) &&
                        (direction == SearchDirection.Forward ||
                        direction == SearchDirection.Backward))
            {
                return list;
            }

            bool insertAt0 = true;

            if (direction == SearchDirection.None
                        || direction == SearchDirection.Last)
            {
                insertAt0 = true;
            }
            else if (direction == SearchDirection.First)
            {
                insertAt0 = false;
            }
            else if (direction == SearchDirection.Forward)
            {
                insertAt0 = false;
            }
            else if (direction == SearchDirection.Backward)
            {
                insertAt0 = true;
            }

            using (SqlConnection connection =
                    new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetCoversationDateList", connection))
                {
                    connection.Open();

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ContratId", contract.Id);
                    command.Parameters.AddWithValue("@SearchDate", searchDate.Date == DateTime.MinValue ? 
                        new DateTime(1753, 1, 1) : 
                        searchDate.Date);
                    command.Parameters.AddWithValue("@Pagesize", pageSize);
                    command.Parameters.AddWithValue("@Direction", (int)direction);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime messageDate = DateTime.Parse(reader["MessageDate"].ToString());

                            if (insertAt0)
                            {
                                list.Insert(0, messageDate);
                            }
                            else
                            {
                                list.Add(messageDate);
                            }
                        }
                    }
                }
            }

            return list;
        }

        public List<OCMessage> GetOCMessage(Contact contract, DateTime dtStart, DateTime dtEnd)
        {
            List<OCMessage> list = new List<OCMessage>();

            using (SqlConnection connection =
                    new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetOCMessage", connection))
                {
                    connection.Open();

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ContactId", contract.Id);
                    command.Parameters.AddWithValue("@dtStart", dtStart == DateTime.MinValue ? 
                        new DateTime(1753, 1, 1) : dtStart);

                    command.Parameters.AddWithValue("@dtEnd", dtEnd);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OCMessage message = new OCMessage();
                            message.ContactId = int.Parse(reader["ContactId"].ToString());
                            message.MessageText = reader["MessageText"].ToString();
                            message.MessageTime = DateTime.Parse(reader["MessageTime"].ToString());
                            message.IsCompressed = bool.Parse(reader["IsCompressed"].ToString());
                            
                            if (message.IsCompressed)
                            {
                                message.MessageText = SevenZip.Decompress(message.MessageText);
                            }

                            list.Add(message);
                        }
                    }
                }
            }

            return list;
        }

        public int GetTotalDailyMessageCount(Contact contract)
        {
            int count = -1;

            using (SqlConnection connection =
                    new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetTotalDailyMessageCount", connection))
                {
                    connection.Open();

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ContactId", contract.Id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            count = reader.GetInt32(0);
                        }
                    }
                }
            }

            return count;
        }
    }
}
