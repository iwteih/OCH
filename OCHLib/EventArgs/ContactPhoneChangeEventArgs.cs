using System;

using CommunicatorAPI;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnContactPhoneChange event
    /// </summary>
    public class ContactPhoneChangeEventArgs : EventArgs
    {
        public int hr { get; set; }
        public object pContact { get; set; }
        public MPHONE_TYPE PhoneType { get; set; }
        public string bstrNumber { get; set; }
    }
}
