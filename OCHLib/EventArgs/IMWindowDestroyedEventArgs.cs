using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnIMWindowDestroyed event
    /// </summary>
    public class IMWindowDestroyedEventArgs : EventArgs
    {
        public object pIMWindow { get; set; }
    }
}
