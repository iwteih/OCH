using System;

using CommunicatorAPI;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnMyPhoneChange event
    /// </summary>
    public class MyPhoneChangeEventArgs : EventArgs
    {
        public MPHONE_TYPE PhoneType { get; set; }
        public string bstrNumber { get; set; }
    }
}
