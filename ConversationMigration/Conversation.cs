using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ConversationMigration
{
    [Serializable]
    public class Conversations
    {
        [XmlArrayAttribute("Items")]
        public Conversation[] Items;
    }

    [Serializable]
    public class Conversation
    {
        public string Time { get; set; }
        [XmlArrayAttribute("Recipients")]
        public string[] Recipients { get; set; }
        public string Body { get; set; }
    }
}
