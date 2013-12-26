using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnMyFriendlyNameChange event
    /// </summary>
    public class MyFriendlyNameChangeEventArgs : EventArgs
    {
        public int hr { get; set; }
        public string bstrPrevFriendlyName { get; set; }
    }
}
