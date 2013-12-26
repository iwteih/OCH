using System;

namespace OCHLib
{
    /// <summary>
    /// EventArgs class representing Communicator's OnContactPagerChange event
    /// </summary>
    public class ContactPagerChangeEventArgs : EventArgs
    {
        public int hr { get; set; }
        public object pContact { get; set; }
        public bool pBoolPage { get; set; }
    }
}
