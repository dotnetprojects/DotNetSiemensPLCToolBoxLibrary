using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace DotNetSimaticDatabaseProtokollerLibrary.Remoting
{
    /// <summary>
    /// Delegate defines the method call from the server to the client
    /// </summary>    
    public delegate void NotifyCallback(string DataSetName);

    /// <summary>
    /// Defines server interface which will be deployed on every client
    /// </summary>
    public interface ICallsToServer
    {
        // Functions wich the Client can Call on the Server....       
        //    string NotifyNewData(string DataSetName);

        /// <summary>
        /// Add or remove callback destinations on the client
        /// </summary>
        event NotifyCallback Notify;
    }

    public static class RemotingConfig
    {
        public static string PortName = "JFKProtokoller";
        public static string ServerName = "RemoteServer";

        private static IDictionary _config;
        public static IDictionary Config
        {
            get
            {
                {
                    if (_config == null)
                    {
                        SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                        NTAccount acct = (NTAccount) sid.Translate(typeof (NTAccount));

                        IDictionary prop = new Hashtable();
                        prop["portName"] = PortName;
                        prop["authorizedGroup"] = acct.Value;
                        prop["exclusiveAddressUse"] = false;
                        _config = prop;
                    }
                    return _config;
                }

            }
        }
    }

    /// <summary>
    /// This class is used by client to provide delegates to the server that will
    /// fire events back through these delegates. Overriding OnServerEvent to capture
    /// the callback from the server
    /// </summary>
    public abstract class NotifyCallbackSink : MarshalByRefObject
    {
        /// <summary>
        /// Called by the server to fire the call back to the client
        /// </summary>
        /// <param name="s">Pass a string for testing</param>
        public void FireNotifyCallback(string s)
        {
            //Console.WriteLine("Activating callback");
            OnNotifyCallback(s);
        }

        /// <summary>
        /// Client overrides this method to receive the callback events from the server
        /// </summary>
        /// <param name="s">Pass a string for testing</param>
        protected abstract void OnNotifyCallback(string s);
    }
}
