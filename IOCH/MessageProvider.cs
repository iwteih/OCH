using log4net;
using OCHEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IOCH
{
    public abstract class MessageProvider
    {
        protected IMessageStore messageStore;

        protected static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public abstract string Keyword { get; set; }

        private int pageSize = 100;
        protected int PageSize 
        {
            get { return pageSize; }
            set 
            {
                pageSize = value;
                messageStore.PageSize = pageSize;
            }
        }

        public MessageProvider(IMessageStore messageStore)
        {
            this.messageStore = messageStore;
            this.messageStore.PageSize = pageSize;
        }

        public List<Contact> ContractList = new List<Contact>();

        public abstract void LoadContracts();

        public abstract int GetTotalDailyMessageCount(Contact contract);

        public abstract List<DateTime> GetConversationDateList(
            Contact contract,
            DateTime? searchDate,
            SearchDirection direction);

        public string GetDailyConversation(Contact contract, DateTime date)
        {
            var messageList = messageStore.GetOCMessage(contract, date, date.AddDays(1));

            if (messageList != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var msg in messageList)
                {
                    sb.AppendLine(ReplaceHistory(msg.MessageText));
                }

                string html = "<html><head><meta HTTP-EQUIV=\"Content-Type\" content=\"text/html; charset=utf-8\" /> <script type=\"text/javascript\">function pointToBottom(){  window.location = \"#bottomlink\";}</script></head> <body onLoad=\"pointToBottom()\">"
                            + sb.ToString()
                            + "<a name=\"bottomlink\">&nbsp;</a></body></html>";

                return html;
            }

            return string.Empty;
        }

        private string ReplaceBetween(string s, string begin, string end, string replace)
        {
            Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
            return regex.Replace(s, replace);
        }

        private string ReplaceHistory(string history)
        {
            history = ReplaceBetween(history, "<OBJECT", "</OBJECT>", "&#8227");//"&#149;"
            history = history.Replace("<A ", "<A target=\"blank\" ");
            return history;
        }

    }
}
