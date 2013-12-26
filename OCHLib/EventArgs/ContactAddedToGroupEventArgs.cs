using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnContactAddedToGroup event
    /// </summary>
    public class ContactAddedToGroupEventArgs : EventArgs
    {
        public int hr { get; set; }
        public object pMGroup { get; set; }
        public object pMContact { get; set; }
    }
}
