using System;

using CommunicatorAPI;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnMyPropertyChange event
    /// </summary>
    public class MyPropertyChangeEventArgs : EventArgs
    {
        public int hr { get; set; }
        public MCONTACTPROPERTY ePropType { get; set; }
        public object vPropVal { get; set; }
    }
}
