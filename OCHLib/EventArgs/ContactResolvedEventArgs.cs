using System;

using CommunicatorAPI;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnContactResolved event
    /// </summary>
    public class ContactResolvedEventArgs : EventArgs
    {
        public int hr { get; set; }
        public ADDRESS_TYPE AddressType { get; set; }
        public string bstrAddress { get; set; }
        public string bstrIMAddress { get; set; }
    }
}
