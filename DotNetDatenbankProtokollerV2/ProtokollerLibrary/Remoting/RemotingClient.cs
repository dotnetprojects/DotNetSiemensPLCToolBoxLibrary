using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;

namespace DotNetSimaticDatabaseProtokollerLibrary.Remoting
{
    public class RemotingClient : NotifyCallbackSink
    {
        public void Start()
        {
            // Create the object for calling into the server
            var m_RemoteObject = (ICallsToServer) Activator.GetObject(typeof (ICallsToServer), "ipc://" + RemotingConfig.PortName + "/" + RemotingConfig.ServerName);

            //Define sink for events
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemotingClient), "ServerEvents", WellKnownObjectMode.Singleton);

            try
            {
                // Assign the callback from the server to here
                m_RemoteObject.Notify += (s) =>
                                             {
                                                 if (DataArrived != null)
                                                     DataArrived(s);
                                             };
            }
            catch (RemotingException ex)
            { }
        }

        public event NotifyCallback DataArrived;

        /// <summary>
        /// Events from the server call into here. This is not in the GUI thread.
        /// </summary>
        /// <param name="s">Pass a string for testing</param>
        protected override void OnNotifyCallback(string DataSetName)
        {
            if (DataArrived != null)
                DataArrived(DataSetName);                     
        }
    }
}
