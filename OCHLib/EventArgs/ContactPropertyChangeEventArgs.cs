using System;

using CommunicatorAPI;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnContactPropertyChange event
    /// </summary>
    public class ContactPropertyChangeEventArgs : EventArgs
    {
        public int hr { get; set; }
        public object pContact { get; set; }
        public MCONTACTPROPERTY ePropType { get; set; }
        public object vPropVal { get; set; }
    }
}
