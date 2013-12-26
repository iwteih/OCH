using CommunicatorAPI;
using CommunicatorPrivate;
using IOCH;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;

namespace OCHLib
{
    public class OCAutomation
    { 
        #region Declarations

        static OCAutomation _instance = null;    // Singleton instance

        private static MessengerClass _communicator;                    // CommunicatorAPI object
        private static IMessengerAdvanced _communicatorAdv;             // IMessengerAdvanced object
        private static MessengerPrivClass _communicatorPriv;            // CommunicatorPrivate object
        private static IMessengerContactResolution _resolver;           // Use to resolve Communicator contacts

        private bool _connected;                                 // Is Communicator connected
        private static int _communicatorUpAndRunning = 1;               // Is Communicator up and running
        private static ResourceManager _statusStrings;                  // Resource Manager for presence status strings
        
        #endregion

        #region Constructors

        OCAutomation() {}

        static OCAutomation()
        {
            _statusStrings = new ResourceManager(
                "OCHLib.Resources", Assembly.GetExecutingAssembly());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Represents a singleton instance of the MOCAutomation class
        /// </summary>
        public static OCAutomation GetInstance()
        {
            _instance = new OCAutomation();

            _instance.Connect();

            return _instance;
        }

        /// <summary>
        /// Property indicating the status of the connection to Office Communicator
        /// </summary>
        public bool Connected
        {
            get { return _connected; }
        }

        #endregion

        #region Exposed Communicator Objects

        // Expose Communicator objects are properties to allow the user to access 
        //  the underlying functionality in whatever way is convenient for them.
        // Making the objects accessible this way ensures that they have 
        //  been initialized properly.

        /// <summary>
        /// Provides access to underlying MessengerClass object
        /// </summary>
        public MessengerClass Root_MessengerClass
        {
            get
            {
                return _communicator;
            }
        }

        /// <summary>
        /// Provides access to underlying IMessengerAdvanced object
        /// </summary>
        public IMessengerAdvanced Root_IMessengerAdvanced
        {
            get
            {
                return _communicatorAdv;
            }
        }

        /// <summary>
        /// Provides access to underlying MessengerPrivClass object
        /// </summary>
        public MessengerPrivClass Root_MessengerPrivClass
        {
            get
            {
                return _communicatorPriv;
            }
        }

        /// <summary>
        /// Provides access to underlying IMessengerContactResolution object
        /// </summary>
        public IMessengerContactResolution Root_IMessengerContactResolution
        {
            get
            {
                return _resolver;
            }
        }

        #endregion

        #region Event Handlers

        public event EventHandler<EventArgs> AppShutdown;
        public event EventHandler<ContactAddedToGroupEventArgs> ContactAddedToGroup;
        public event EventHandler<ContactBlockChangeEventArgs> ContactBlockChange;
        public event EventHandler<ContactFriendlyNameChangeEventArgs> ContactFriendlyNameChange;
        public event EventHandler<ContactListAddEventArgs> ContactListAdd;
        public event EventHandler<ContactListRemoveEventArgs> ContactListRemove;
        public event EventHandler<ContactPagerChangeEventArgs> ContactPagerChange;
        public event EventHandler<ContactPhoneChangeEventArgs> ContactPhoneChange;
        public event EventHandler<ContactPropertyChangeEventArgs> ContactPropertyChange;
        public event EventHandler<ContactRemovedFromGroupEventArgs> ContactRemovedFromGroup;
        public event EventHandler<ContactResolvedEventArgs> ContactResolved;
        public event EventHandler<ContactStatusChangeEventArgs> ContactStatusChange;
        public event EventHandler<GroupAddedEventArgs> GroupAdded;
        public event EventHandler<GroupNameChangedEventArgs> GroupNameChanged;
        public event EventHandler<GroupRemovedEventArgs> GroupRemoved;
        public event EventHandler<IMWindowContactAddedEventArgs> IMWindowContactAdded;
        public event EventHandler<IMWindowContactRemovedEventArgs> IMWindowContactRemoved;
        public event EventHandler<IMWindowCreatedEventArgs> IMWindowCreated;
        public event EventHandler<IMWindowDestroyedEventArgs> IMWindowDestroyed;
        public event EventHandler<MyFriendlyNameChangeEventArgs> MyFriendlyNameChange;
        public event EventHandler<MyPhoneChangeEventArgs> MyPhoneChange;
        public event EventHandler<MyPropertyChangeEventArgs> MyPropertyChange;
        public event EventHandler<MyStatusChangeEventArgs> MyStatusChange;
        public event EventHandler<SigninEventArgs> Signin;
        public event EventHandler<EventArgs> Signout;
        public event EventHandler<UnreadEmailChangeEventArgs> UnreadEmailChange;
        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        #endregion

        #region Internal

        /// <summary>
        /// Check if Office Communicator is up and running, and attempt to sign in to the running instance. 
        /// Wire up all events exposed by the Communicator automation API.
        /// </summary>
        private void Connect()
        {
            if (_communicator != null || _connected)
                return;

            try
            {
                // Check if Office Communicator is up and running
                //
                _communicatorUpAndRunning = 0;
                _communicatorUpAndRunning = Convert.ToInt32(Microsoft.Win32.Registry.CurrentUser
                    .OpenSubKey("Software").OpenSubKey("IM Providers").OpenSubKey("Communicator").GetValue("UpAndRunning", 1));

                if (_communicatorUpAndRunning != 2)
                    return;

                // Create Communicator objects
                //
                _communicator = new CommunicatorAPI.MessengerClass();
                _communicatorPriv = new CommunicatorPrivate.MessengerPrivClass();
                _communicatorAdv = _communicator as IMessengerAdvanced;
                _resolver = _communicator as IMessengerContactResolution;

                // Wire up Communicator events
                //
                _communicator.OnAppShutdown += new DMessengerEvents_OnAppShutdownEventHandler(communicator_OnAppShutdown);
                _communicator.OnContactAddedToGroup += new DMessengerEvents_OnContactAddedToGroupEventHandler(communicator_OnContactAddedToGroup);
                _communicator.OnContactBlockChange += new DMessengerEvents_OnContactBlockChangeEventHandler(communicator_OnContactBlockChange);
                _communicator.OnContactFriendlyNameChange += new DMessengerEvents_OnContactFriendlyNameChangeEventHandler(communicator_OnContactFriendlyNameChange);
                _communicator.OnContactListAdd += new DMessengerEvents_OnContactListAddEventHandler(communicator_OnContactListAdd);
                _communicator.OnContactListRemove += new DMessengerEvents_OnContactListRemoveEventHandler(communicator_OnContactListRemove);
                _communicator.OnContactPagerChange += new DMessengerEvents_OnContactPagerChangeEventHandler(communicator_OnContactPagerChange);
                _communicator.OnContactPhoneChange += new DMessengerEvents_OnContactPhoneChangeEventHandler(communicator_OnContactPhoneChange);
                _communicator.OnContactPropertyChange += new DMessengerEvents_OnContactPropertyChangeEventHandler(communicator_OnContactPropertyChange);
                _communicator.OnContactRemovedFromGroup += new DMessengerEvents_OnContactRemovedFromGroupEventHandler(communicator_OnContactRemovedFromGroup);
                _communicator.OnContactResolved += new DMessengerEvents_OnContactResolvedEventHandler(communicator_OnContactResolved);
                _communicator.OnContactStatusChange += new DMessengerEvents_OnContactStatusChangeEventHandler(communicator_OnContactStatusChange);
                _communicator.OnGroupAdded += new DMessengerEvents_OnGroupAddedEventHandler(communicator_OnGroupAdded);
                _communicator.OnGroupNameChanged += new DMessengerEvents_OnGroupNameChangedEventHandler(communicator_OnGroupNameChanged);
                _communicator.OnGroupRemoved += new DMessengerEvents_OnGroupRemovedEventHandler(communicator_OnGroupRemoved);
                _communicator.OnIMWindowContactAdded += new DMessengerEvents_OnIMWindowContactAddedEventHandler(communicator_OnIMWindowContactAdded);
                _communicator.OnIMWindowContactRemoved += new DMessengerEvents_OnIMWindowContactRemovedEventHandler(communicator_OnIMWindowContactRemoved);
                _communicator.OnIMWindowCreated += new DMessengerEvents_OnIMWindowCreatedEventHandler(communicator_OnIMWindowCreated);
                _communicator.OnIMWindowDestroyed += new DMessengerEvents_OnIMWindowDestroyedEventHandler(communicator_OnIMWindowDestroyed);
                _communicator.OnMyFriendlyNameChange += new DMessengerEvents_OnMyFriendlyNameChangeEventHandler(communicator_OnMyFriendlyNameChange);
                _communicator.OnMyPhoneChange += new DMessengerEvents_OnMyPhoneChangeEventHandler(communicator_OnMyPhoneChange);
                _communicator.OnMyPropertyChange += new DMessengerEvents_OnMyPropertyChangeEventHandler(communicator_OnMyPropertyChange);
                _communicator.OnMyStatusChange += new DMessengerEvents_OnMyStatusChangeEventHandler(communicator_OnMyStatusChange);
                _communicator.OnSignin += new DMessengerEvents_OnSigninEventHandler(communicator_OnSignin);
                _communicator.OnSignout += new DMessengerEvents_OnSignoutEventHandler(communicator_OnSignout);
                _communicator.OnUnreadEmailChange += new DMessengerEvents_OnUnreadEmailChangeEventHandler(communicator_OnUnreadEmailChange);

                // Check if the user is already signed in to Office Communicator
                //
                if (_communicator.MyStatus != MISTATUS.MISTATUS_OFFLINE)
                {
                    _connected = true;
                }
                else
                {
                    try
                    {
                        _communicator.AutoSignin();
                        _communicatorAdv.AutoSignin();
                    }
                    catch { }
                }

                // Raise a ConnectionStatusChanged event 
                //
                OnConnectionStateChanged(
                    new ConnectionStateChangedEventArgs()
                        {
                            Connected = true
                        });
            }
            catch (Exception)
            {
                _connected = false;
            }
        }

        /// <summary>
        ///     This calls Communicator to find a contact based on the Sip Uri.
        /// </summary>
        /// <returns>
        ///     A CommunicatorAPI.IMessengerContact which is null if the contact was not found.
        /// </returns>
        private static IMessengerContactAdvanced FindContact(string sipUri)
        {
            IMessengerContactAdvanced cont = null;
            // Check the local contact list first
            try
            {
                cont = (IMessengerContactAdvanced)_communicator.GetContact(sipUri, string.Empty);
            }
            catch
            {
                cont = null;
            }

            // If not in local contact, try the SIP Provider for Off Contact list contact
            if (cont == null || cont.Status == MISTATUS.MISTATUS_UNKNOWN)
            {
                try
                {
                    cont = (IMessengerContactAdvanced)_communicator.GetContact(sipUri, _communicator.MyServiceId);
                    return cont;
                }
                catch
                {
                    cont = null;
                }
            }

            return cont;
        }

        /// <summary>
        /// Set connection status to false when Communicator signs out or shuts down.
        /// Raise a ConnectionStateChanged event.
        /// </summary>
        private void LostConnection()
        {
            if (_connected == true)
            {
                _connected = false;

                OnConnectionStateChanged(
                    new ConnectionStateChangedEventArgs()
                        {
                            Connected = false
                        });
            }           
        }

        /// <summary>
        /// Dispose of a COM object
        /// </summary>
        private static void ReleaseComObject(object comObject)
        {
            if (comObject != null)
                Marshal.ReleaseComObject(comObject);
        }

        /// <summary>
        ///     This converts the status code used by Communicator to a string representation.
        /// </summary>
        private string StatusToText(string sipUri)
        {
            if (GetIsBlocked(sipUri))
                return _statusStrings.GetString("MISTATUS_BLOCKED");

            MISTATUS status = GetPresenceStatus(sipUri);
            
            switch (status)
            {
                case MISTATUS.MISTATUS_UNKNOWN:
                    return _statusStrings.GetString("MISTATUS_UNKNOWN");

                case MISTATUS.MISTATUS_ONLINE:
                    return _statusStrings.GetString("MISTATUS_ONLINE");

                case MISTATUS.MISTATUS_OFFLINE:
                    return _statusStrings.GetString("MISTATUS_OFFLINE");

                case MISTATUS.MISTATUS_AWAY:
                    return _statusStrings.GetString("MISTATUS_AWAY");

                case MISTATUS.MISTATUS_OUT_OF_OFFICE:
                    return _statusStrings.GetString("MISTATUS_OUT_OF_OFFICE");

                case MISTATUS.MISTATUS_OUT_TO_LUNCH:
                    return _statusStrings.GetString("MISTATUS_OUT_TO_LUNCH");

                case MISTATUS.MISTATUS_BE_RIGHT_BACK:
                    return _statusStrings.GetString("MISTATUS_BE_RIGHT_BACK");

                case MISTATUS.MISTATUS_BUSY:
                    return _statusStrings.GetString("MISTATUS_BUSY");

                case MISTATUS.MISTATUS_ON_THE_PHONE:
                    return _statusStrings.GetString("MISTATUS_ON_THE_PHONE");

                case MISTATUS.MISTATUS_IN_A_CONFERENCE:
                    return _statusStrings.GetString("MISTATUS_IN_A_CONFERENCE");

                case MISTATUS.MISTATUS_IN_A_MEETING:
                    return _statusStrings.GetString("MISTATUS_IN_A_MEETING");

                case MISTATUS.MISTATUS_IDLE:
                    return _statusStrings.GetString("MISTATUS_IDLE");

                case MISTATUS.MISTATUS_DO_NOT_DISTURB:
                    return _statusStrings.GetString("MISTATUS_DO_NOT_DISTURB");

                case MISTATUS.MISTATUS_INVISIBLE:
                    return _statusStrings.GetString("MISTATUS_INVISIBLE");

                case MISTATUS.MISTATUS_ALLOW_URGENT_INTERRUPTIONS:
                    return _statusStrings.GetString("MISTATUS_ALLOW_URGENT_INTERRUPTIONS");

                case MISTATUS.MISTATUS_MAY_BE_AVAILABLE:
                    return _statusStrings.GetString("MISTATUS_MAY_BE_AVAILABLE");

                case MISTATUS.MISTATUS_CUSTOM:
                    string customStatus = GetCustomStatusString(sipUri);
                    if (customStatus != string.Empty)
                    {
                        return customStatus;
                    }
                    else
                    {
                        return _statusStrings.GetString("MISTATUS_CUSTOM");
                    }

                default:
                    return _statusStrings.GetString("MISTATUS_UNKNOWN");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Start a Communicator Call with the contact with the given Sip Uri
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public void CallComputer(string sipUri)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null && !contact.IsSelf)
                    {
                        _communicator.StartVoice(contact);
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }
        }

        /// <summary>
        /// Call the contact's phone number of the specified type
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        /// <param name="phoneType">Phone number type</param>
        public void CallPhone(string sipUri, MPHONE_TYPE phoneType)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        _communicator.Phone(contact,
                            phoneType,
                            contact.get_PhoneNumber(phoneType));
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }
        }

        /// <summary>
        /// Dial a given phone number
        /// </summary>
        /// <param name="phoneNumber">The phone number to dial</param>
        public void CallPhone(string phoneNumber)
        {
            try
            {
                if (!String.IsNullOrEmpty(phoneNumber))
                {
                    var numberToDial = String.Format("tel:{0}", phoneNumber);
                    object[] telUris = { numberToDial };

                    _communicatorAdv.StartConversation(
                        CONVERSATION_TYPE.CONVERSATION_TYPE_PHONE,
                        telUris,
                        null,
                        null,
                        null,
                        numberToDial);
                }
            }
            catch { }
            // We don't have control of how the user formats the phone number. 
            // Eat the exception and let Communicator handle it.
        }

        /// <summary>
        /// Start a Video Call with the contact with the given Sip Uri
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        /// <param name="phoneType">Phone number type</param>
        public void CallVideo(string sipUri)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        _communicatorAdv.StartConversation(
                            CONVERSATION_TYPE.CONVERSATION_TYPE_VIDEO,
                            new object[] { sipUri },
                            null, null, null, null);
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }
        }

        /// <summary>
        /// Get the availability of the user with the given Sip Uri
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public int GetAvailability(string sipUri)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        Object[] cProperties = contact.PresenceProperties as Object[];
                        return (int)cProperties[(int)PRESENCE_PROPERTY.PRESENCE_PROP_AVAILABILITY];
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }

            return 0;
        }

        /// Gets all contacts in the user's contact list
        /// </summary>
        /// <returns>
        ///     Generic IEnumerable of contacts in the form IMessengerContact
        /// </returns>
        public IEnumerable<IMessengerContact> GetMyContacts()
        {
            IMessengerContacts contacts = null;

            try
            {
                contacts = (IMessengerContacts)_communicator.MyContacts;
                return contacts.Cast<IMessengerContact>();
            }
            catch { }
            finally
            {
                ReleaseComObject(contacts);
            }

            return null;
        }

        /// <summary>
        /// Get the custom status text of the user with the given Sip Uri
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public string GetCustomStatusString(string sipUri)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        Object[] cProperties = contact.PresenceProperties as Object[];
                        string status = (string)cProperties[(int)PRESENCE_PROPERTY.PRESENCE_PROP_CUSTOM_STATUS_STRING];
                        if ((status == null) || (status.Length == 0))
                            return String.Empty;
                        else
                            return status;
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }

            return default(String);
        }

        /// <summary>
        /// Get the friendly name of the user with the given Sip Uri
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public string GetFriendlyName(string sipUri)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        return contact.FriendlyName;
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }

            return default(String);
        }

        /// <summary>
        /// Get the blocked status of the user with the given Sip Uri
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public bool GetIsBlocked(string sipUri)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        return contact.Blocked;
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }

            return false;
        }

        /// <summary>
        /// Check if the user with the give Sip Uri is the signed in user's contact list
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public bool GetIsContact(string sipUri)
        {
            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    return (_communicator.GetContact(sipUri, null) != null);
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Check if the user with the give Sip Uri is the signed in user
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public bool GetIsSelf(string sipUri)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        return contact.IsSelf;
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }

            return false;
        }

        /// <summary>
        /// Get the tagged status of the user with the given Sip Uri
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public bool GetIsTagged(string sipUri)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        return contact.IsTagged;
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }

            return false;
        }

        /// <summary>
        /// Get the out-of-office note of the user with the given Sip Uri
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public string GetOofNote(string sipUri)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        Object[] cProperties = contact.PresenceProperties as Object[];
                        string oof = (string)cProperties[(int)PRESENCE_PROPERTY.PRESENCE_PROP_IS_OOF];
                        if ((oof == null) || (oof.Length == 0))
                            return String.Empty;
                        else
                            return oof;
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }

            return default(String);
        }

        /// <summary>
        /// Get the contact's phone number of the specified type
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        /// <param name="phoneType">Phone number type</param>
        public string GetPhoneNumber(string sipUri, CommunicatorAPI.MPHONE_TYPE phoneType)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        return contact.get_PhoneNumber(phoneType);
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }

            return default(String);
        }

        /// <summary>
        /// Get the presence status of the user with the given Sip Uri
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public MISTATUS GetPresenceStatus(string sipUri)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        Object[] cProperties = contact.PresenceProperties as Object[];
                        return (MISTATUS)cProperties[(int)PRESENCE_PROPERTY.PRESENCE_PROP_MSTATE];
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }

            return MISTATUS.MISTATUS_UNKNOWN;
        }

        /// <summary>
        /// Get the presence note of the user with the given Sip Uri
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public string GetPresenceNote(string sipUri)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        Object[] cProperties = contact.PresenceProperties as Object[];
                        string note = (string)cProperties[(int)PRESENCE_PROPERTY.PRESENCE_PROP_PRESENCE_NOTE];
                        if ((note == null) || (note.Length == 0))
                            return String.Empty;
                        else
                            return note;
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }

            return default(String);
        }

        /// <summary>
        /// Get the Sip Uri of the signed in user
        /// </summary>
        public string GetSignedInUser()
        {
            try
            {
                if (_connected)
                {
                    return _communicator.MySigninName;
                }
            }
            catch { }

            return default(String);
        }

        /// <summary>
        /// Get the friendly text status of the contact specified by the Sip Uri
        /// </summary>
        public string GetTextStatus(string sipUri)
        {
            return StatusToText(sipUri);
        }

        /// <summary>
        /// Convert an MISTATUS value to its friendly text representation
        /// </summary>
        public string GetTextStatus(MISTATUS status)
        {
            string textStatus = _statusStrings.GetString(status.ToString());
            if (String.IsNullOrEmpty(textStatus))
                return _statusStrings.GetString("MISTATUS_UNKNOWN");
            else
                return textStatus;
        }

        /// <summary>
        /// Get the tooltip text of the user with the given Sip Uri
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public string GetToolTip(string sipUri)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        Object[] cProperties = contact.PresenceProperties as Object[];
                        string tip = (string)cProperties[(int)PRESENCE_PROPERTY.PRESENCE_PROP_TOOL_TIP];
                        if ((tip == null) || (tip.Length == 0))
                            return String.Empty;
                        else
                            return tip;
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }

            return default(String);
        }

        /// <summary>
        /// Send an instant message to the contact with the given Sip Uri
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public void InstantMessage(string sipUri)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        _communicator.InstantMessage(contact);
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }

        }

        /// <summary>
        ///     Search for a contact.
        /// </summary>
        /// <param name="addressType" type="CommunicatorAPI.ADDRESS_TYPE">
        ///     <para>
        ///         Address type to use for resolution. One of SMTP address or display name.
        ///     </para>
        /// </param>
        /// <param name="resolutionType" type="CommunicatorAPI.CONTACT_RESOLUTION_TYPE">
        ///     <para>
        ///         Cached or asynchronous
        ///     </para>
        /// </param>
        /// <param name="contactAddress" type="string">
        ///     <para>
        ///         Address of the contact
        ///     </para>
        /// </param>
        /// <returns>
        ///     A string that is the SIP URI for the contact or null if not found.
        /// </returns>
        public string ResolveContact(ADDRESS_TYPE addressType, CONTACT_RESOLUTION_TYPE resolutionType, string contactAddress)
        {
            try
            {
                String sipUri;
                sipUri = _resolver.ResolveContact(addressType, resolutionType, contactAddress);
                if (sipUri == null)
                    return default(String);
                else
                    return sipUri;
            }
            catch
            {
                return default(String);
            }

        }

        /// <summary>
        /// Add the contact specified by the given Sip Uri to the user's contact list
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public void SetIsContact(string sipUri)
        {
            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    _communicatorPriv.AddContact(sipUri, null);
                }
            }
            catch { }
        }

        /// <summary>
        /// Set the tagged status of the user with the given Sip Uri
        /// </summary>
        /// <param name="sipUri">The Sip Uri of the contact</param>
        public bool SetIsTagged(string sipUri, bool tagged)
        {
            IMessengerContactAdvanced contact = null;

            try
            {
                if (!String.IsNullOrEmpty(sipUri) && _connected)
                {
                    contact = FindContact(sipUri);

                    if (contact != null)
                    {
                        contact.IsTagged = tagged;
                        return true;
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(contact);
            }

            return false;
        }

        /// <summary>
        /// Set the status of the signed in user.
        /// </summary>
        /// <param name="status">The value of MISTATUS to set the status to.</param>
        public void SetMyStatus(MISTATUS status)
        {
            try
            {
                IMessengerContactAdvanced myContact = (IMessengerContactAdvanced)
                    _communicator.GetContact(GetSignedInUser(), String.Empty);
                object[] mPP = new object[8]; // There are 8 default  presence properties ...
                mPP[(int)PRESENCE_PROPERTY.PRESENCE_PROP_MSTATE] = status;
                myContact.PresenceProperties = (object)mPP;

                ReleaseComObject(myContact);
            }
            catch { }
        }

        #endregion

        #region Communicator Events

        static void communicator_OnUnreadEmailChange(MUAFOLDER mFolder, int cUnreadEmail, ref bool pBoolfEnableDefault)
        {
            if (_instance.UnreadEmailChange != null)
                _instance.UnreadEmailChange(_instance,
                    new UnreadEmailChangeEventArgs() 
                        { 
                            mFolder = mFolder,
                            cUnreadEmail = cUnreadEmail,
                            pBoolfEnableDefault = pBoolfEnableDefault
                        });

        }

        void communicator_OnSignout()
        {
            LostConnection();

            if (_instance.Signout != null)
                _instance.Signout(_instance, EventArgs.Empty);
        }

        void communicator_OnSignin(int hr)
        {
            if (hr >= 0)
            {
                _connected = true;

                OnConnectionStateChanged(
                    new ConnectionStateChangedEventArgs()
                        {
                            Connected = true
                        });

                if (_instance.Signin != null)
                    _instance.Signin(_instance,
                        new SigninEventArgs()
                            {
                                hr = hr
                            });
            }

        }

        static void communicator_OnMyStatusChange(int hr, MISTATUS mMyStatus)
        {
            if (_instance.MyStatusChange != null)
                _instance.MyStatusChange(_instance,
                    new MyStatusChangeEventArgs()
                        {
                            hr = hr,
                            mMyStatus = mMyStatus
                        });
        }

        static void communicator_OnMyPropertyChange(int hr, MCONTACTPROPERTY ePropType, object vPropVal)
        {
            if (_instance.MyPropertyChange != null)
                _instance.MyPropertyChange(_instance,
                    new MyPropertyChangeEventArgs()
                        {
                            hr = hr,
                            ePropType = ePropType,
                            vPropVal = vPropVal
                        });
        }

        static void communicator_OnMyPhoneChange(MPHONE_TYPE PhoneType, string bstrNumber)
        {
            if (_instance.MyPhoneChange != null)
                _instance.MyPhoneChange(_instance,
                    new MyPhoneChangeEventArgs()
                        {
                            PhoneType = PhoneType,
                            bstrNumber = bstrNumber
                        });
        }

        static void communicator_OnMyFriendlyNameChange(int hr, string bstrPrevFriendlyName)
        {
            if (_instance.MyFriendlyNameChange != null)
                _instance.MyFriendlyNameChange(_instance,
                    new MyFriendlyNameChangeEventArgs()
                        {
                            hr = hr,
                            bstrPrevFriendlyName = bstrPrevFriendlyName
                        });
        }

        static void communicator_OnIMWindowDestroyed(object pIMWindow)
        {
            if (_instance.IMWindowDestroyed != null)
                _instance.IMWindowDestroyed(_instance,
                    new IMWindowDestroyedEventArgs()
                        {
                            pIMWindow = pIMWindow
                        });
        }

        static void communicator_OnIMWindowCreated(object pIMWindow)
        {
            if (_instance.IMWindowCreated != null)
                _instance.IMWindowCreated(_instance,
                    new IMWindowCreatedEventArgs()
                        {
                            pIMWindow = pIMWindow
                        });
        }

        static void communicator_OnIMWindowContactRemoved(object pContact, object pIMWindow)
        {
            if (_instance.IMWindowContactRemoved != null)
                _instance.IMWindowContactRemoved(_instance,
                    new IMWindowContactRemovedEventArgs()
                        {
                            pContact = pContact,
                            pIMWindow = pIMWindow
                        });
        }

        static void communicator_OnIMWindowContactAdded(object pContact, object pIMWindow)
        {
            if (_instance.IMWindowContactAdded != null)
                _instance.IMWindowContactAdded(_instance,
                    new IMWindowContactAddedEventArgs()
                    {
                        pContact = pContact,
                        pIMWindow = pIMWindow
                    });
        }

        static void communicator_OnGroupRemoved(int hr, object pMGroup)
        {
            if (_instance.GroupRemoved != null)
                _instance.GroupRemoved(_instance,
                    new GroupRemovedEventArgs()
                        {
                            hr = hr,
                            pMGroup = pMGroup
                        });
        }

        static void communicator_OnGroupNameChanged(int hr, object pMGroup)
        {
            if (_instance.GroupNameChanged != null)
                _instance.GroupNameChanged(_instance,
                    new GroupNameChangedEventArgs()
                    {
                        hr = hr,
                        pMGroup = pMGroup
                    });
        }

        static void communicator_OnGroupAdded(int hr, object pMGroup)
        {
            if (_instance.GroupAdded != null)
                _instance.GroupAdded(_instance,
                    new GroupAddedEventArgs()
                    {
                        hr = hr,
                        pMGroup = pMGroup
                    });
        }

        static void communicator_OnContactStatusChange(object pMContact, MISTATUS mStatus)
        {
            if (_instance.ContactStatusChange != null)
                _instance.ContactStatusChange(_instance,
                    new ContactStatusChangeEventArgs()
                        {
                            pMContact = pMContact,
                            mStatus = mStatus
                        });
        }

        static void communicator_OnContactResolved(int hr, ADDRESS_TYPE AddressType, string bstrAddress, string bstrIMAddress)
        {
            if (_instance.ContactResolved != null)
                _instance.ContactResolved(_instance,
                    new ContactResolvedEventArgs()
                        {
                            hr = hr,
                            AddressType = AddressType,
                            bstrAddress = bstrAddress,
                            bstrIMAddress = bstrIMAddress
                        });
        }

        static void communicator_OnContactRemovedFromGroup(int hr, object pMGroup, object pMContact)
        {
            if (_instance.ContactRemovedFromGroup != null)
                _instance.ContactRemovedFromGroup(_instance,
                    new ContactRemovedFromGroupEventArgs()
                        {
                            hr = hr,
                            pMGroup = pMGroup,
                            pMContact = pMContact
                        });
        }

        static void communicator_OnContactPropertyChange(int hr, object pContact, MCONTACTPROPERTY ePropType, object vPropVal)
        {
            if (_instance.ContactPropertyChange != null)
                _instance.ContactPropertyChange(_instance,
                    new ContactPropertyChangeEventArgs()
                        {
                            hr = hr,
                            pContact = pContact,
                            ePropType = ePropType,
                            vPropVal = vPropVal
                        });
        }

        static void communicator_OnContactPhoneChange(int hr, object pContact, MPHONE_TYPE PhoneType, string bstrNumber)
        {
            if (_instance.ContactPhoneChange != null)
                _instance.ContactPhoneChange(_instance,
                    new ContactPhoneChangeEventArgs()
                        {
                            hr = hr,
                            pContact = pContact,
                            PhoneType = PhoneType,
                            bstrNumber = bstrNumber
                        });
        }

        static void communicator_OnContactPagerChange(int hr, object pContact, bool pBoolPage)
        {
            if (_instance.ContactPagerChange != null)
                _instance.ContactPagerChange(_instance,
                    new ContactPagerChangeEventArgs()
                        {
                            hr = hr,
                            pContact = pContact,
                            pBoolPage = pBoolPage
                        });
        }

        static void communicator_OnContactListRemove(int hr, object pMContact)
        {
            if (_instance.ContactListRemove != null)
                _instance.ContactListRemove(_instance,
                    new ContactListRemoveEventArgs()
                        {
                            hr = hr,
                            pMContact = pMContact
                        });
        }

        static void communicator_OnContactListAdd(int hr, object pMContact)
        {
            if (_instance.ContactListAdd != null)
                _instance.ContactListAdd(_instance,
                    new ContactListAddEventArgs()
                        {
                            hr = hr,
                            pMContact = pMContact
                        });
        }

        static void communicator_OnContactFriendlyNameChange(int hr, object pMContact, string bstrPrevFriendlyName)
        {
            if (_instance.ContactFriendlyNameChange != null)
                _instance.ContactFriendlyNameChange(_instance,
                    new ContactFriendlyNameChangeEventArgs()
                        {
                            hr = hr,
                            pMContact = pMContact,
                            bstrPrevFriendlyName = bstrPrevFriendlyName
                        });
        }

        static void communicator_OnContactBlockChange(int hr, object pContact, bool pBoolBlock)
        {
            if (_instance.ContactBlockChange != null)
                _instance.ContactBlockChange(_instance,
                    new ContactBlockChangeEventArgs()
                        {
                            hr = hr,
                            pContact = pContact,
                            pBoolBlock = pBoolBlock
                        });
        }

        static void communicator_OnContactAddedToGroup(int hr, object pMGroup, object pMContact)
        {
            if (_instance.ContactAddedToGroup != null)
                _instance.ContactAddedToGroup(_instance,
                    new ContactAddedToGroupEventArgs()
                        {
                            hr = hr,
                            pMGroup = pMGroup,
                            pMContact = pMContact
                        });
        }

        private void DetachCommunicator()
        {
            LostConnection();

            // Clean up Communicator events
            //
            _communicator.OnAppShutdown -= new DMessengerEvents_OnAppShutdownEventHandler(communicator_OnAppShutdown);
            _communicator.OnContactAddedToGroup -= new DMessengerEvents_OnContactAddedToGroupEventHandler(communicator_OnContactAddedToGroup);
            _communicator.OnContactBlockChange -= new DMessengerEvents_OnContactBlockChangeEventHandler(communicator_OnContactBlockChange);
            _communicator.OnContactFriendlyNameChange -= new DMessengerEvents_OnContactFriendlyNameChangeEventHandler(communicator_OnContactFriendlyNameChange);
            _communicator.OnContactListAdd -= new DMessengerEvents_OnContactListAddEventHandler(communicator_OnContactListAdd);
            _communicator.OnContactListRemove -= new DMessengerEvents_OnContactListRemoveEventHandler(communicator_OnContactListRemove);
            _communicator.OnContactPagerChange -= new DMessengerEvents_OnContactPagerChangeEventHandler(communicator_OnContactPagerChange);
            _communicator.OnContactPhoneChange -= new DMessengerEvents_OnContactPhoneChangeEventHandler(communicator_OnContactPhoneChange);
            _communicator.OnContactPropertyChange -= new DMessengerEvents_OnContactPropertyChangeEventHandler(communicator_OnContactPropertyChange);
            _communicator.OnContactRemovedFromGroup -= new DMessengerEvents_OnContactRemovedFromGroupEventHandler(communicator_OnContactRemovedFromGroup);
            _communicator.OnContactResolved -= new DMessengerEvents_OnContactResolvedEventHandler(communicator_OnContactResolved);
            _communicator.OnContactStatusChange -= new DMessengerEvents_OnContactStatusChangeEventHandler(communicator_OnContactStatusChange);
            _communicator.OnGroupAdded -= new DMessengerEvents_OnGroupAddedEventHandler(communicator_OnGroupAdded);
            _communicator.OnGroupNameChanged -= new DMessengerEvents_OnGroupNameChangedEventHandler(communicator_OnGroupNameChanged);
            _communicator.OnGroupRemoved -= new DMessengerEvents_OnGroupRemovedEventHandler(communicator_OnGroupRemoved);
            _communicator.OnIMWindowContactAdded -= new DMessengerEvents_OnIMWindowContactAddedEventHandler(communicator_OnIMWindowContactAdded);
            _communicator.OnIMWindowContactRemoved -= new DMessengerEvents_OnIMWindowContactRemovedEventHandler(communicator_OnIMWindowContactRemoved);
            _communicator.OnIMWindowCreated -= new DMessengerEvents_OnIMWindowCreatedEventHandler(communicator_OnIMWindowCreated);
            _communicator.OnIMWindowDestroyed -= new DMessengerEvents_OnIMWindowDestroyedEventHandler(communicator_OnIMWindowDestroyed);
            _communicator.OnMyFriendlyNameChange -= new DMessengerEvents_OnMyFriendlyNameChangeEventHandler(communicator_OnMyFriendlyNameChange);
            _communicator.OnMyPhoneChange -= new DMessengerEvents_OnMyPhoneChangeEventHandler(communicator_OnMyPhoneChange);
            _communicator.OnMyPropertyChange -= new DMessengerEvents_OnMyPropertyChangeEventHandler(communicator_OnMyPropertyChange);
            _communicator.OnMyStatusChange -= new DMessengerEvents_OnMyStatusChangeEventHandler(communicator_OnMyStatusChange);
            _communicator.OnSignin -= new DMessengerEvents_OnSigninEventHandler(communicator_OnSignin);
            _communicator.OnSignout -= new DMessengerEvents_OnSignoutEventHandler(communicator_OnSignout);
            _communicator.OnUnreadEmailChange -= new DMessengerEvents_OnUnreadEmailChangeEventHandler(communicator_OnUnreadEmailChange);

            // Clean up Communicator objects
            //
            ReleaseComObject(_communicator);
            ReleaseComObject(_communicatorPriv);
            ReleaseComObject(_communicatorAdv);
            ReleaseComObject(_resolver);

            _communicator = null;
            _communicatorPriv = null;
            _communicatorAdv = null;
            _resolver = null;
        }

        public void ShutdownApp()
        {
            if ((_communicator != null) && _connected)
            {
                DetachCommunicator();
            }
        }

        private void communicator_OnAppShutdown()
        {
            try
            {
                DetachCommunicator();
            }
            catch (Exception e)
            {
                throw e;
            }

            if (_instance.AppShutdown != null)
                _instance.AppShutdown(_instance, EventArgs.Empty);
        }

        #endregion

        #region Other Events

        /// <summary>
        /// Event raised by MOCAutomation when there is a c`hange in Communicator's connection status.
        /// Exposed as public-static because it needs to be wired up before the instance of 
        ///  MOCAutomation is created.
        /// </summary>
        public void OnConnectionStateChanged(ConnectionStateChangedEventArgs e)
        {
            if (ConnectionStateChanged != null)
            {
                ConnectionStateChanged(_instance, e);
            }
        }
        
        #endregion
    }
}
