using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnContactRemovedFromGroup event
    /// </summary>
    public class ContactRemovedFromGroupEventArgs : EventArgs
    {
        public int hr { get; set; }
        public object pMGroup { get; set; }
        public object pMContact { get; set; }
    }
}