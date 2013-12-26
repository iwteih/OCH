using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnGroupAdded event
    /// </summary>
    public class GroupAddedEventArgs : EventArgs
    {
        public int hr { get; set; }
        public object pMGroup { get; set; }
    }
}
