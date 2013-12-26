using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnContactListAdd event
    /// </summary>
    public class ContactListAddEventArgs : EventArgs
    {
        public int hr { get; set; }
        public object pMContact { get; set; }
    }
}
