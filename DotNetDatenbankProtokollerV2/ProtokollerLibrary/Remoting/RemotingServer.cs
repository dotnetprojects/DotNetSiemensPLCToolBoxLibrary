using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Principal;
using System.Text;

namespace DotNetSimaticDatabaseProtokollerLibrary.Remoting
{
    public class RemotingServer
    {
        public void Start()
        {
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider {TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full};

            BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
            
            IpcChannel ipcCh = new IpcChannel(RemotingConfig.Config, clientProvider, serverProvider);
            
            ChannelServices.RegisterChannel(ipcCh, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof (ClientComms), RemotingConfig.ServerName, WellKnownObjectMode.Singleton);
        }


        public class ClientComms : MarshalByRefObject, ICallsToServer
        {
            /// <summary>
            /// Local copy of event holding a collection
            /// </summary>
            private static event NotifyCallback s_notify;

            /// <summary>
            /// Add or remove callback destinations on the client
            /// </summary>
            public event NotifyCallback Notify
            {
                add { s_notify = value; }
                remove { }
            }

            /// <summary>
            /// Call this method to send the string to the client. This call will throw an exception
            /// if the client has gone or network is down
            /// </summary>
            /// <param name="s">Some test to send to client</param>
            public static void CallNotifyEvent(string s)
            {
                if (s_notify != null)
                    s_notify(s);
            }
        }
    }
}