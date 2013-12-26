using OCHEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IOCH
{
    public interface IMessageStore
    {
        int PageSize { get; set; }

        void SaveMessage(DateTime beginTime, string messageBody, string[] contracts);

        List<Contact> GetContractList();

        void GetQueryMessageList(
            DateTime dtStart, 
            DateTime dtEnd, 
            string keyword, 
            List<Contact> contractLit, 
            List<OCMessage> messageList);

        List<DateTime> GetCoversationDateList(
            Contact contract, 
            DateTime searchDate, 
            SearchDirection direction = SearchDirection.None);

        List<OCMessage> GetOCMessage(Contact contract, DateTime dtStart, DateTime dtEnd);

        int GetTotalDailyMessageCount(Contact contract);
    }
}
