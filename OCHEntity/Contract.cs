using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCHEntity
{
    [Serializable]
    public class Contact
    {
        public long Id { get; set; }
        public string FriendlyName { get; set; }
        public string ContactName { get; set; }
        public DateTime LastConversationTime { get; set; }
    }
}
