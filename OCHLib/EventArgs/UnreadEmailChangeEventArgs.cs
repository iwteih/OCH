using System;

using CommunicatorAPI;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnUnreadEmailChange event
    /// </summary>
    public class UnreadEmailChangeEventArgs : EventArgs
    {
        public MUAFOLDER mFolder { get; set; }
        public int cUnreadEmail { get; set; }
        public bool pBoolfEnableDefault { get; set; }
    }
}
