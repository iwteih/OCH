using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnSignin event
    /// </summary>
    public class SigninEventArgs : EventArgs
    {
        public int hr { get; set; }
    }
}
