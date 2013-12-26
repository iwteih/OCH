using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOCH;
using OCHEntity;
using OCHUtil;
using Community.CsharpSqlite.SQLiteClient;
using System.IO;
using System.Diagnostics;
using log4net;

namespace MessageStoreImpl
{
    public class SQLite : IMessageStore
    {
        private string DATABASE_NAME = string.Empty;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static object locker = new object();

        public SQLite(string database = null)
        {
            if (string.IsNullOrEmpty(database))
            {
                database = "OCH.db";
            }

            string appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            DATABASE_NAME = string.Format("{0}", Path.Combine(appStartPath, database));

            if (!File.Exists(DATABASE_NAME))
            {
                throw new FileNotFoundException("Cannot find sqlite database: {0}", DATABASE_NAME);
            }

            DATABASE_NAME = string.Format("Version=3,uri=file:{0}", DATABASE_NAME);
        }

        private int pageSize = 100;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        public void SaveMessage(DateTime beginTime, string messageBody, string[] contracts)
        {
            messageBody = MessageFormatter.FormatSendTimeStamp(messageBody);

            string compress = SevenZip.Compress(messageBody);

            bool isCompressed = compress.Length < messageBody.Length;

            foreach (var c in contracts)
            {
                Contact contract = SaveContract(c, beginTime);
                SaveContractCoversationDailyDate(contract, beginTime);
                SaveMessage(contract, isCompressed ? compress : messageBody, beginTime, isCompressed);
            }
        }

        private Contact GetContractByName(string ContactName)
        {
            Contact c = null;

            try
            {
                lock (locker)
                {
                    using (SqliteConnection connection = new SqliteConnection(DATABASE_NAME))
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = string.Format("select * from Contact where ContactName = '{0}'", ContactName.ToLower());

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    c = new Contact();

                                    while (reader.Read())
                                    {
                                        c.Id = int.Parse(reader["Id"].ToString());
                                        c.ContactName = reader["ContactName"].ToString();
                                        c.FriendlyName = reader["FriendlyName"] == null ? string.Empty : reader["FriendlyName"].ToString();
                                        c.LastConversationTime = DateTime.Parse(reader["LastConversationTime"].ToString());
                                    }
                                }

                                reader.Close();
                                reader.Dispose();
                            }

                            command.Dispose();
                        }

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch (SqliteException exp)
            {
                logger.Error(exp);
            }

            return c;
        }

        private bool NewContract(string ContactName, DateTime date)
        {
            int result = 0;

            try
            {
                lock (locker)
                {
                    using (SqliteConnection connection = new SqliteConnection(DATABASE_NAME))
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = string.Format(@"insert into Contact (ContactName, LastConversationTime)
select '{0}', '{1}'
where not exists(select 1 from Contact where ContactName ='{0}')", ContactName.ToLower(), DateTime2String(date));

                            result = command.ExecuteNonQuery();

                            command.Dispose();
                        }

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch (SqliteException exp)
            {
                logger.Error(exp);
                result = -1;
            }

            return result != 0;
        }

        private bool UpdateContract(string ContactName, DateTime date)
        {
            int result = 0;

            try
            {
                lock (locker)
                {
                    using (SqliteConnection connection = new SqliteConnection(DATABASE_NAME))
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = string.Format(@"update Contact set LastConversationTime = '{1}' , IsDeleted = 'false' where ContactName = '{0}'", ContactName.ToLower(), DateTime2String(date));

                            result = command.ExecuteNonQuery();

                            command.Dispose();
                        }

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch (SqliteException exp)
            {
                logger.Error(exp);
                result = -1;
            }

            return result != 0;
        }

        private Contact SaveContract(string ContactName, DateTime date)
        {
            bool isNewContract = NewContract(ContactName, date);

            if (!isNewContract)
            {
                bool updated = UpdateContract(ContactName, date);
            }

            Contact contract = GetContractByName(ContactName);

            return contract;
        }

        private void SaveContractCoversationDailyDate(Contact contract, DateTime date)
        {
            if (contract != null)
            {
                try
                {
                    lock (locker)
                    {
                        using (SqliteConnection connection = new SqliteConnection(DATABASE_NAME))
                        {
                            connection.Open();

                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = string.Format(@"insert into ContactMessageDate(ContactId, MessageDate)  select '{0}', '{1}'
where not exists(select 1 from ContactMessageDate where ContactId ='{0}' and MessageDate = '{1}')", contract.Id, DateTime2String(date.Date));

                                int result = command.ExecuteNonQuery();

                                command.Dispose();
                            }

                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
                catch (SqliteException exp)
                {
                    logger.Error(exp);
                }
            }
        }

        private bool NewMessage(Contact contract,
            string messageBody,
            DateTime dtBeginTime,
            bool isCompressed)
        {
            int result = 0;
            try
            {
                lock (locker)
                {
                    using (SqliteConnection connection = new SqliteConnection(DATABASE_NAME))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = string.Format(@"
		insert into Message (ContactId, MessageTime, MessageText, IsCompressed)
		select '{0}', '{1}', '{2}', '{3}'
        where not exists(select 1 from Message where ContactId ='{0}' and MessageTime = '{1}')"
                                , contract.Id, DateTime2String(dtBeginTime), messageBody, isCompressed);

                            result = command.ExecuteNonQuery();

                            command.Dispose();
                        }

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch (SqliteException exp)
            {
                logger.Error(exp);
                result = -1;
            }

            return result != 0;
        }

        private bool UpdateMessage(Contact contract,
            string messageBody,
            DateTime dtBeginTime,
            bool isCompressed)
        {
            int result = 0;

            try
            {
                lock (locker)
                {
                    using (SqliteConnection connection = new SqliteConnection(DATABASE_NAME))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = string.Format(@"
		update Message 
		   set MessageText = '{2}',
		       IsCompressed = '{3}',
               IsDeleted = 'false'
		 where ContactId = '{0}'
		   and MessageTime = '{1}'"
                                , contract.Id, DateTime2String(dtBeginTime), messageBody, isCompressed);

                            result = command.ExecuteNonQuery();

                            command.Dispose();
                        }

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch (SqliteException exp)
            {
                logger.Error(exp);
                result = -1;
            }

            return result != 0;
        }

        private void SaveMessage(
            Contact contract,
            string messageBody,
            DateTime dtBeginTime,
            bool isCompressed)
        {
            bool isNewMessage = NewMessage(contract, messageBody, dtBeginTime, isCompressed);

            if (!isNewMessage)
            {
                bool updated = UpdateMessage(contract, messageBody, dtBeginTime, isCompressed);
            }
        }

        public List<Contact> GetContractList()
        {
            List<Contact> list = new List<Contact>();

            try
            {
                lock (locker)
                {
                    using (SqliteConnection connection = new SqliteConnection(DATABASE_NAME))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = @"SELECT Id, ContactName, FriendlyName, LastConversationTime
	FROM Contact
    where IsDeleted = 'false'
	order by ContactName";
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        Contact c = new Contact();
                                        c.Id = int.Parse(reader["Id"].ToString());
                                        c.ContactName = reader["ContactName"].ToString();
                                        c.FriendlyName = reader["FriendlyName"] == null ? string.Empty : reader["FriendlyName"].ToString();
                                        c.LastConversationTime = DateTime.Parse(reader["LastConversationTime"].ToString());

                                        list.Add(c);
                                    }
                                }

                                reader.Close();
                                reader.Dispose();
                            }

                            command.Dispose();
                        }

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch (SqliteException exp)
            {
                logger.Error(exp);
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

        private List<DateTime> GetCoversationDateListLast(Contact contract)
        {
            List<DateTime> list = new List<DateTime>();

            try
            {
                lock (locker)
                {
                    using (var connection =
                                new SqliteConnection(DATABASE_NAME))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = string.Format(@"
                            select MessageDate 
                            from (
	                            select * 
		                          from ContactMessageDate
		                         where ContactId = '{0}' and ContainsMessage ='true') 
                                 order by strftime('%s',MessageDate) desc
                            limit '{1}'",
                                contract.Id,
                                pageSize);

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        DateTime messageDate = DateTime.Parse(reader["MessageDate"].ToString());
                                        list.Insert(0, messageDate);
                                    }
                                }

                                reader.Close();
                                reader.Dispose();
                            }

                            command.Dispose();
                        }

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch (SqliteException exp)
            {
                logger.Error(exp);
            }

            return list;
        }

        private List<DateTime> GetCoversationDateListFirst(Contact contract)
        {
            List<DateTime> list = new List<DateTime>();

            try
            {
                lock (locker)
                {
                    using (var connection =
                                new SqliteConnection(DATABASE_NAME))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = string.Format(@"
                            select MessageDate 
                            from (
	                            select * 
		                          from ContactMessageDate
		                         where ContactId = '{0}' and ContainsMessage ='true') 
                                 order by strftime('%s',MessageDate) asc
                            limit '{1}'",
                                contract.Id,
                                pageSize);

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        DateTime messageDate = DateTime.Parse(reader["MessageDate"].ToString());
                                        list.Add(messageDate);
                                    }
                                }

                                reader.Close();
                                reader.Dispose();
                            }

                            command.Dispose();
                        }

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch (SqliteException exp)
            {
                logger.Error(exp);
            }

            return list;
        }


        private List<DateTime> GetCoversationDateListForward(Contact contract, DateTime searchDate)
        {
            List<DateTime> list = new List<DateTime>();

            try
            {
                lock (locker)
                {
                    using (var connection =
                                new SqliteConnection(DATABASE_NAME))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = string.Format(@"
                            select MessageDate 
                            from (
	                            select * 
		                          from ContactMessageDate
		                         where ContactId = '{0}' and ContainsMessage ='true'
                                   and MessageDate > '{1}'
                                 order by strftime('%s', MessageDate) asc) 
                            limit '{2}'",
                                contract.Id,
                                DateTime2String(searchDate),
                                pageSize);

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        DateTime messageDate = DateTime.Parse(reader["MessageDate"].ToString());
                                        list.Add(messageDate);
                                    }
                                }

                                reader.Close();
                                reader.Dispose();
                            }

                            command.Dispose();
                        }

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch (SqliteException exp)
            {
                logger.Error(exp);
            }

            return list;
        }

        private List<DateTime> GetCoversationDateListBackward(Contact contract, DateTime searchDate)
        {
            List<DateTime> list = new List<DateTime>();

            try
            {
                lock (locker)
                {
                    using (var connection =
                                new SqliteConnection(DATABASE_NAME))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = string.Format(@"
                            select MessageDate 
                            from (
	                            select * 
		                          from ContactMessageDate
		                         where ContactId = '{0}' and ContainsMessage ='true'
                                   and MessageDate < '{1}'
                                 order by strftime('%s',MessageDate) desc) 
                            limit '{2}'",
                                contract.Id,
                                DateTime2String(searchDate),
                                pageSize);

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        DateTime messageDate = DateTime.Parse(reader["MessageDate"].ToString());
                                        list.Insert(0, messageDate);
                                    }
                                }

                                reader.Close();
                                reader.Dispose();
                            }

                            command.Dispose();
                        }

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch (SqliteException exp)
            {
                logger.Error(exp);
            }

            return list;
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

            if (direction == SearchDirection.None
                        || direction == SearchDirection.Last)
            {
                list = GetCoversationDateListLast(contract);
            }
            else if (direction == SearchDirection.First)
            {
                list = GetCoversationDateListFirst(contract);
            }
            else if (direction == SearchDirection.Forward)
            {
                list = GetCoversationDateListForward(contract, searchDate);
            }
            else if (direction == SearchDirection.Backward)
            {
                list = GetCoversationDateListBackward(contract, searchDate);
            }

            return list;
        }

        public List<OCHEntity.OCMessage> GetOCMessage(Contact contract, DateTime dtStart, DateTime dtEnd)
        {
            List<OCMessage> list = new List<OCMessage>();

            try
            {
                lock (locker)
                {
                    using (var connection =
                            new SqliteConnection(DATABASE_NAME))
                    {
                        using (var command = connection.CreateCommand())
                        {
                            connection.Open();

                            command.CommandText = string.Format(@"SELECT *
	  from Message
	 where ContactId = '{0}'
	   and MessageTime >= '{1}'
	   and MessageTime < '{2}'
     order by strftime('%s',MessageTime)",
                                    contract.Id,
                                    DateTime2String(dtStart == DateTime.MinValue ?
                                new DateTime(1970, 1, 1) : dtStart),
                                DateTime2String(dtEnd));

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
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

                                reader.Close();
                                reader.Dispose();
                            }

                            command.Dispose();
                        }

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch (SqliteException exp)
            {
                logger.Error(exp);
            }

            return list;
        }

        public int GetTotalDailyMessageCount(Contact contract)
        {
            int count = -1;

            try
            {
                lock (locker)
                {
                    using (var connection =
                            new SqliteConnection(DATABASE_NAME))
                    {
                        using (var command = connection.CreateCommand())
                        {
                            connection.Open();

                            command.CommandText = @"select count(*) as MessageCount
	  from ContactMessageDate
	 where ContactId  = @ContactId";

                            command.Parameters.Add(new SqliteParameter
                            {
                                ParameterName = "@ContactId",
                                Value = contract.Id
                            });

                            count = int.Parse(command.ExecuteScalar().ToString());

                            command.Dispose();
                        }

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch (SqliteException exp)
            {
                logger.Error(exp);
            }

            return count;
        }

        private string DateTime2String(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss.ffff");
        }

    }
}
