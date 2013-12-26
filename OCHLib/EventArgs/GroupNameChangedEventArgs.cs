using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnGroupNameChanged event
    /// </summary>
    public class GroupNameChangedEventArgs : EventArgs
    {
        public int hr { get; set; }
        public object pMGroup { get; set; }
    }
}
