using IOCH;
using OCHEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCH_Win
{
    //default view
    public class DailyMessageProvider : MessageProvider
    {
        public DailyMessageProvider(IMessageStore messageStore) : base(messageStore) { }

        public override void LoadContracts()
        {
            ContractList = messageStore.GetContractList();
        }

        public override int GetTotalDailyMessageCount(Contact contract)
        {
            return messageStore.GetTotalDailyMessageCount(contract);
        }

        public override List<DateTime> GetConversationDateList(
            Contact contract, 
            DateTime? searchDate, 
            SearchDirection direction)
        {
            return messageStore.GetCoversationDateList(contract, 
                searchDate == null ? contract.LastConversationTime: searchDate.Value, 
                direction);
        }


        public override string Keyword
        {
            get
            {
                return null;
            }
            set
            {
                Console.WriteLine("try to assign value to Keyword");
            }
        }
    }
}
