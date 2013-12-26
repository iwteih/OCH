using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnIMWindowContactRemoved event
    /// </summary>
    public class IMWindowContactRemovedEventArgs : EventArgs
    {
        public object pContact { get; set; }
        public object pIMWindow { get; set; }
    }
}
