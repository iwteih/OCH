using System;

using CommunicatorAPI;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnContactStatusChange event
    /// </summary>
    public class ContactStatusChangeEventArgs : EventArgs
    {
        public object pMContact { get; set; }
        public MISTATUS mStatus { get; set; }
    }
}
