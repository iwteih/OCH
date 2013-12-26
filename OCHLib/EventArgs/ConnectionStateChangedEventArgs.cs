using System;

namespace OCHLib
{
    public class ConnectionStateChangedEventArgs : EventArgs
    {
        public bool Connected { get; set; }
    }
}