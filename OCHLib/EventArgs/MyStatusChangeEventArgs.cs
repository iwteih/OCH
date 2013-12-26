using System;

using CommunicatorAPI;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnMyStatusChange event
    /// </summary>
    public class MyStatusChangeEventArgs : EventArgs
    {
        public int hr { get; set; }
        public MISTATUS mMyStatus { get; set; }
    }
}
