using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnContactFriendlyNameChange event
    /// </summary>
    public class ContactFriendlyNameChangeEventArgs : EventArgs
    {
        public int hr  { get; set; }
        public object pMContact  { get; set; }
        public string bstrPrevFriendlyName { get; set; }
    }
}
