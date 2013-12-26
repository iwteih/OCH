using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnIMWindowCreated event
    /// </summary>
    public class IMWindowCreatedEventArgs : EventArgs
    {
        public object pIMWindow { get; set; }
    }
}
