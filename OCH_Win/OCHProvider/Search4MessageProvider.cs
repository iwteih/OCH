using IOCH;
using OCHEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCH_Win
{
    public class Search4MessageProvider : MessageProvider
    {
        private DateTime dtStart, dtEnd;

        private List<OCMessage> OCMessageList = new List<OCMessage>();

        public Search4MessageProvider(IMessageStore messageStore, 
            DateTime dtStart, 
            DateTime dtEnd, 
            string keyword) : base(messageStore) 
        {
            this.dtStart = dtStart;
            this.dtEnd = dtEnd;
            this.Keyword = keyword;
        }

        public override string Keyword
        {
            get;
            set;
        }
        
        public override void LoadContracts()
        {
            ContractList.Clear();
            OCMessageList.Clear();
            messageStore.GetQueryMessageList(dtStart, dtEnd, Keyword, ContractList, OCMessageList);
        }

        public override int GetTotalDailyMessageCount(Contact contract)
        {
            return OCMessageList.Count(c => c.ContactId == contract.Id);
        }

        public override List<DateTime> GetConversationDateList(Contact contract, DateTime? searchDate, SearchDirection direction)
        {
            List<DateTime> list = new List<DateTime>();
            
            DateTime date = DateTime.Now;

            if (searchDate == null || searchDate == DateTime.MaxValue)
            { 
                var v = OCMessageList.Where(f => f.ContactId == contract.Id)
                    .OrderBy(o => o.MessageTime)
                    .LastOrDefault();

                if(v != null)
                {
                    date = v.MessageTime.Date;
                }
            }
            else if (searchDate != null && searchDate == DateTime.MinValue)
            { 
                date = OCMessageList.Where(f => f.ContactId == contract.Id)
                    .OrderBy(o => o.MessageTime)
                    .FirstOrDefault().MessageTime.Date;
            }
            else 
            {
                date = searchDate.Value;
            }

            var currentUserDateList = OCMessageList
                .Where(f => f.ContactId == contract.Id)
                .Select(s => s.MessageTime.Date)
                .OrderBy(o => o).ToList();
            IEnumerable<DateTime> enumerator = null;

            int index = currentUserDateList.IndexOf(date);
            if (index == -1)
            {
                logger.Warn(string.Format("Cannot find messages for date: {0}", searchDate));
                return null;
            }

            if (direction == SearchDirection.None
                || direction == SearchDirection.Last)
            {
                enumerator = currentUserDateList.Skip(currentUserDateList.Count - PageSize).Take(PageSize);
            }
            else if (direction == SearchDirection.First)
            {
                enumerator = currentUserDateList.Take(PageSize);
            }
            else if (direction == SearchDirection.Forward)
            {
                if (currentUserDateList.LastOrDefault() != date)
                {
                    enumerator = currentUserDateList.Skip(index).Take(PageSize);
                }
            }
            else if (direction == SearchDirection.Backward)
            {
                if (currentUserDateList.FirstOrDefault() != date)
                {
                    int precheck = index - PageSize + 1;
                    enumerator = currentUserDateList.Skip(precheck > 0 ? precheck : 0).Take(index < PageSize ? index + 1 : PageSize);
                }
            }

            if (enumerator != null)
            {
                foreach (var e in enumerator)
                {
                    if (!list.Contains(e.Date))
                    {
                        list.Add(e);
                    }
                }
            }

            return list;
        }

        
    }
}
