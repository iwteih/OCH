using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnContactListRemove event
    /// </summary>
    public class ContactListRemoveEventArgs : EventArgs
    {
        public int hr { get; set; }
        public object pMContact { get; set; }
    }
}
