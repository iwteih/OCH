using IOCH;
using OCHEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageStoreInESENT
{
    public class ESENT : IMessageStore
    {
        public int PageSize
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void SaveMessage(DateTime dtBeginTime, string strMessageBody, string[] contact)
        {
            throw new NotImplementedException();
        }

        public List<Contract> GetContractList()
        {
            throw new NotImplementedException();
        }

        public void GetQueryMessageList(DateTime dtStart, DateTime dtEnd, string keyword, List<Contract> contractLit, List<OCMessage> messageList)
        {
            throw new NotImplementedException();
        }

        public List<DateTime> GetCoversationDateList(Contract contract, DateTime searchDate, SearchDirection direction = SearchDirection.None)
        {
            throw new NotImplementedException();
        }

        public List<OCMessage> GetOCMessage(Contract contract, DateTime dtStart, DateTime dtEnd)
        {
            throw new NotImplementedException();
        }

        public int GetTotalDailyMessageCount(Contract contract)
        {
            throw new NotImplementedException();
        }
    }
}
