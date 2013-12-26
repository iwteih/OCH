using IOCH;
using log4net;
using OCHEntity;
using OCHUtil;
using STSdb.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MessageStoreImpl
{
    public class STSdb : IMessageStore
    {
        private string DATABASE_NAME = string.Empty;
        private static readonly string TABLENAME_CONTRACT = "contract";
        private static object locker = new object();
        private int pageSize = 100;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       
        public STSdb(string database)
        {
            string appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            DATABASE_NAME = Path.Combine(appStartPath, database);
        }

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

            foreach(var c in contracts)
            {
                Contact contract = SaveContract(c, dtBeginTime);
                SaveContractCoversationDailyDate(contract, dtBeginTime);
                SaveMessage(contract, isCompressed ? compress : messageBody, dtBeginTime, isCompressed);
            }
        }

        private void SaveMessage(
            Contact contract, 
            string messageBody, 
            DateTime dtBeginTime, 
            bool isCompressed)
        {
            var message = new OCMessage
            {
                ContactId = contract.Id,
                MessageText = messageBody,
                MessageTime = dtBeginTime,
                IsCompressed = isCompressed
            };

            lock (locker)
            {
                using (var repository = StorageEngine.FromFile(DATABASE_NAME))
                {
                    var table = repository.Scheme.CreateOrOpenXTable<long, OCMessage>(
                        new Locator(contract.ContactName));
                    repository.Scheme.Commit();

                    var msg = table.FirstOrDefault(f => f.Record.MessageTime == dtBeginTime);

                    if (msg == null)
                    {
                        long id = table.Count == 0 ? 0 : table.LastRow.Key + 1;
                        table[id] = message;
                    }
                    else
                    {
                        table[msg.Key] = message;
                    }

                    table.Commit();
                    table.Close();
                }
            }
        }
        
        private Contact SaveContract(string ContactName, DateTime date)
        {
            Contact contract = new Contact() { ContactName = ContactName, LastConversationTime = date };

            lock (locker)
            {
                using (var repository = StorageEngine.FromFile(DATABASE_NAME))
                {
                    var table = repository.Scheme.CreateOrOpenXTable<long, Contact>(
                        new Locator(TABLENAME_CONTRACT));
                    repository.Scheme.Commit();

                    var c = table.FirstOrDefault(f => f.Record.ContactName == ContactName);

                    if (c == null)
                    {
                        long id = table.Count == 0 ? 0 : table.LastRow.Key + 1;
                        contract.Id = id;
                        table[id] = contract;
                    }
                    else
                    {
                        contract.Id = c.Key;
                        table[c.Key].LastConversationTime = date;
                    }

                    table.Commit();
                    table.Close();
                }
            }

            return contract;
        }

        private void SaveContractCoversationDailyDate(Contact contract, DateTime date)
        {
            if (contract != null)
            {
                DateTime convDay = new DateTime(date.Year, date.Month, date.Day);

                lock (locker)
                {
                    using (var repository = StorageEngine.FromFile(DATABASE_NAME))
                    {
                        var table = repository.Scheme.CreateOrOpenXTable<long, DateTime>(
                            new Locator(string.Format("{0}_MsgDate", contract.Id)));
                        repository.Scheme.Commit();

                        var d = table.FirstOrDefault(f => f.Record.Year == date.Year
                            && f.Record.Month == date.Month
                            && f.Record.Day == date.Day);

                        if (d == null)
                        {
                            long id = table.Count == 0 ? 0 : table.LastRow.Key + 1;
                            table[id] = date.Date;
                        }

                        table.Commit();
                        table.Close();
                    }
                }
            }
        }

        public List<Contact> GetContractList()
        {
            List<Contact> list = null;

            lock (locker)
            {
                using (var repository = StorageEngine.FromFile(DATABASE_NAME))
                {
                    var table = repository.Scheme.CreateOrOpenXTable<long, Contact>(
                        new Locator(TABLENAME_CONTRACT));
                    repository.Scheme.Commit();

                    var v = table.OrderBy(f => f.Record.ContactName).Select(s => s.Record);

                    if (v != null)
                    {
                        list = v.ToList();
                    }

                    table.Close();
                }
            }

            return list;
        }

        public void GetQueryMessageList(DateTime dtStart, 
            DateTime dtEnd, 
            string keyword, 
            List<Contact> contractLit,
            List<OCMessage> messageList)
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

        public List<DateTime> GetCoversationDateList(
            Contact contract,
            DateTime searchDate,
            SearchDirection direction = SearchDirection.None)
        {
            List<DateTime> list = new List<DateTime>();

            bool insertAt0 = true;

            lock (locker)
            {
                using (var repository = StorageEngine.FromFile(DATABASE_NAME))
                {
                    var table = repository.Scheme.CreateOrOpenXTable<long, DateTime>(
                            new Locator(string.Format("{0}_MsgDate", contract.Id)));
                    repository.Scheme.Commit();

                    var v = table.Where(w => w.Record == searchDate).FirstOrDefault();

                    var first = table.FirstOrDefault();
                    var last = table.LastOrDefault();

                    //avoid the first/last' forward/backward
                    if (v == null && 
                        (direction == SearchDirection.Forward||
                        direction == SearchDirection.Backward))
                    {
                        logger.Warn(string.Format("Cannot found record of {0}", searchDate));

                        return list;
                    }

                    IEnumerable<Row<long, DateTime>> enumerator = null;

                    if (direction == SearchDirection.None
                        || direction == SearchDirection.Last)
                    {
                        enumerator = table.Backward();
                        insertAt0 = true;
                    }
                    else if (direction == SearchDirection.First)
                    {
                        enumerator = table.Forward();
                        insertAt0 = false;
                    }
                    else if (direction == SearchDirection.Forward)
                    {
                        if (v.Key != last.Key)
                        {
                            enumerator = table.Forward(v.Key);
                            insertAt0 = false;
                        }
                    }
                    else if (direction == SearchDirection.Backward)
                    {
                        if (v.Key != first.Key)
                        {
                            enumerator = table.Backward(v.Key);
                            insertAt0 = true;
                        }
                    }

                    if (enumerator != null)
                    {
                        foreach (var row in enumerator.Take(PageSize))
                        {
                            if (insertAt0)
                            {
                                list.Insert(0, row.Record);
                            }
                            else
                            {
                                list.Add(row.Record);
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

            lock (locker)
            {
                using (var repository = StorageEngine.FromFile(DATABASE_NAME))
                {
                    var table = repository.Scheme.CreateOrOpenXTable<long, OCMessage>(
                        new Locator(contract.ContactName));
                    repository.Scheme.Commit();

                    var ocMessages = table.Where(w => w.Record.MessageTime >= dtStart
                        && w.Record.MessageTime < dtEnd)
                        .Select(s => s.Record);

                    if (ocMessages != null)
                    {
                        foreach (var v in ocMessages)
                        {
                            var msg = new OCMessage()
                            {
                                ContactId = v.ContactId,
                                MessageText = v.IsCompressed ? SevenZip.Decompress(v.MessageText) : v.MessageText,
                                MessageTime = v.MessageTime
                            };
                            list.Add(msg);
                        }
                    }
                }
            }

            return list;
        }

        public int GetTotalDailyMessageCount(Contact contract)
        {
            lock (locker)
            {
                using (var repository = StorageEngine.FromFile(DATABASE_NAME))
                {
                    var table = repository.Scheme.CreateOrOpenXTable<long, DateTime>(
                            new Locator(string.Format("{0}_MsgDate", contract.Id)));
                    repository.Scheme.Commit();

                    int total = table.Count();

                    return total;
                }
            }
        }

       
    }
}
