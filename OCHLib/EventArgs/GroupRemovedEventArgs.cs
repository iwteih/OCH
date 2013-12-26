using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnGroupRemoved event
    /// </summary>
    public class GroupRemovedEventArgs : EventArgs
    {
        public int hr { get; set; }
        public object pMGroup { get; set; }
    }
}
