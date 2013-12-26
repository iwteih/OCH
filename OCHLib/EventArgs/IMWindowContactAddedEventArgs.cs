using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnIMWindowContactAdded event
    /// </summary>
    public class IMWindowContactAddedEventArgs : EventArgs
    {
        public object pContact { get; set; }
        public object pIMWindow { get; set; }
    }
}
