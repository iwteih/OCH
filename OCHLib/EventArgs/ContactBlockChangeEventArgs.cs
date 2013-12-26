using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnContactBlockChange event
    /// </summary>
    public class ContactBlockChangeEventArgs : EventArgs
    {
        public int hr { get; set; }
        public object pContact { get; set; }
        public bool pBoolBlock { get; set; }
    }
}
