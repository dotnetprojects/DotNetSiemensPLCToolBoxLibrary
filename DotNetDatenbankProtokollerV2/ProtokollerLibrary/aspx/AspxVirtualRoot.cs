//---------------------------------------------------------------------
//  This file is part of the Microsoft .NET Framework SDK Code Samples.
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
// 
//This source code is intended only as a supplement to Microsoft
//Development Tools and/or on-line documentation.  See these other
//materials for detailed information regarding Microsoft code samples.
// 
//THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
//KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//PARTICULAR PURPOSE.
//---------------------------------------------------------------------

#region Using directives

using System;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Globalization;

#endregion


namespace DotNetSimaticDatabaseProtokollerLibrary.aspx
{
    public class AspxVirtualRoot : IDisposable
    {
        private HttpListener m_HttpListener;
        private int m_Port;
        private int m_SslPort;
        private String m_HostName;
        private AspxNetEngine m_AspNetEngine;
        private static CultureInfo m_CultureInfo = new CultureInfo("En-us");
        private AuthenticationSchemes m_AuthSchemes;

        private EventHandler _onAppDomainUnload;

        private static Object m_lockObject = new object();

        public HttpListener HttpListener
        {
            get
            {
                return m_HttpListener;
            }
        }

        public int Port
        {
            get
            {
                return m_Port;
            }
        }

        public int SslPort
        {
            get
            {
                return m_SslPort;
            }
        }

        public AuthenticationSchemes AuthenticationScheme
        {
            get
            {
                return m_AuthSchemes;
            }
            set
            {
                m_AuthSchemes = value;
            }
        }

        public void Dispose()
        {
            if (m_HttpListener != null && m_HttpListener.IsListening)
                m_HttpListener.Close();
            if (m_AspNetEngine != null)
                m_AspNetEngine.Dispose();
        }


        public AspxVirtualRoot(int port)
            : this(port, 0)
        {
        }

        public AspxVirtualRoot(int port, int sslPort)
        {

            m_Port = port;
            m_SslPort = sslPort;

            //Get the Hostname of the machine. by Default we would be listening on http://[Hostname]/
            m_HostName = Dns.GetHostName();

            if (String.IsNullOrEmpty(m_HostName))
            {
                //highly improbable but just in case.
                throw new AspxException("Unable to resolve machine name");
            }

            //Check if port is valid
            if (port < 0 || sslPort < 0)
            {
                throw new AspxException("Invalid port number specified for argument " +
                                         ((sslPort < 0) ? "SslPort"
                                                        : "Port"));
            }


            //Create a HttpListener to receive incoming requests
            m_HttpListener = new HttpListener();

            //Register the base Root as prefix. by default, we are gonna assume that the current directory is where we serve pages from
            m_HttpListener.Prefixes.Add(String.Format(m_CultureInfo, "http://*:{0}/", m_Port));

            if (m_SslPort != 0)
            {
                //Check if SSL port is requested, configure a prefix for SSL
                m_HttpListener.Prefixes.Add(String.Format(m_CultureInfo, "https://*:{0}/", m_Port));
            }

            //Hookup a delegate for Selecting authentication
            m_HttpListener.AuthenticationSchemeSelectorDelegate += new AuthenticationSchemeSelector(AuthenticationSchemeSelectorCallback);
            //Set default auth scheme to Anonymous
            m_AuthSchemes = AuthenticationSchemes.Anonymous;


            // start watching for app domain unloading
            _onAppDomainUnload = new EventHandler(OnAppDomainUnload);
            Thread.GetDomain().DomainUnload += _onAppDomainUnload;

            //Lets kick off the Listener so that it starts listening.
            try
            {
                StartListening();
            }
            catch (Exception e)
            {
                //Error in starting the listener
                throw new AspxException("Error in starting HttpListener", e);
            }

        }

        /**
         * Take in a virtual alias and a physical directory to be configured
         */
        public void Configure(String virtualAlias, String physicalDirectory)
        {
            m_AspNetEngine = new AspxNetEngine(virtualAlias, physicalDirectory);
        }

        //Shutdown the listener gracefully
        public void StopListener()
        {
            m_HttpListener.Stop();
        }


        private void StartListening()
        {
            m_HttpListener.Start();
            m_HttpListener.BeginGetContext(new AsyncCallback(ContextReceivedCallback), null);
        }

        /**
         * This method will be called when a client/browser makes a request. We will pass on
         * required information to the hosting API from this method
         */
        private void ContextReceivedCallback(IAsyncResult asyncResult)
        {
            //Check if listener is listening other wise return
            if (!m_HttpListener.IsListening)
                return;

            //Get the context
            HttpListenerContext listenerContext = m_HttpListener.EndGetContext(asyncResult);

            //Lets get the Listener listening for the next request
            m_HttpListener.BeginGetContext(new AsyncCallback(ContextReceivedCallback), null);

            //If No AspNetEngine was configured, just respond with 404
            if (m_AspNetEngine == null)
            {
                listenerContext.Response.StatusCode = 404;
                listenerContext.Response.Close();
                return;
            }

            //Retrieve the URL requested
            String pageRequested = listenerContext.Request.Url.LocalPath;

            //Remove the "/alias"  from the begining page request as we just need to 
            //pass the file and the query out to the Application Host
            pageRequested = AspxVirtualRoot.RemoveAliasFromRequest(pageRequested, m_AspNetEngine.VirtualAlias);

            //Prepare the DataHolder object that will be passed on for processing
            AspxRequestInfo dataHolder = AspxNetEngine.PrepareAspxRequestInfoObject(listenerContext); ;

            //Look for Client Certificate if it has any
            if (listenerContext.Request.IsSecureConnection)
            {
                dataHolder.m_ClientCertificate = listenerContext.Request.GetClientCertificate();
                Console.WriteLine("Client certificate received.");
            }

            try
            {
                //Pass the request to the Hosted application
                m_AspNetEngine.ExecutingAppDomain.ProcessRequest(pageRequested, listenerContext.Request.Url.Query.Replace("?", ""), dataHolder);
            }
            catch (AspxException aspxException)
            {
                //Error occured.Log it and move on
                Console.WriteLine(aspxException);
            }

            Console.WriteLine(listenerContext.Request.Url.LocalPath + "...   " +
                       listenerContext.Response.StatusCode + " " + listenerContext.Response.StatusDescription);

            //Finally close the response or else the client will time out
            listenerContext.Response.Close();
        }

        /*
         * Callback that will be called whenever a request is received by HttpListener. 
         * NOTE: Instead of this callback, the authentication scheme for the listener can be
         * set using the HttpListener.AuthenticationScheme property. The reason for using a 
         * callback is that in case this sample is to be extended to host multiple ASPX hosting
         * apps for the same Listener, this callback will need to have code the determines 
         * which hosting app supports which authentication scheme.
         */
        private AuthenticationSchemes AuthenticationSchemeSelectorCallback(HttpListenerRequest request)
        {
            //This sample is one Aspx hosted application for one listener. So return the variable
            return m_AuthSchemes;

            /*
             * TODO: Incase this sample needs to be extended to host multiple apps,
             * logic would be
             * Determing request is for which app by using the raw Url fo the request
             *     If app found, return AuthenticationScheme for that app
             */

        }

        private static string RemoveAliasFromRequest(string request, string alias)
        {
            String aliasToLook = "/" + alias;
            Console.WriteLine("Entered as " + request + ":" + alias);

            //Remove the /alias from the query
            if (request.StartsWith(aliasToLook))
                request = request.Substring(aliasToLook.Length + 1);

            //After removing /alias, check for URL's of type /alias/

            if (request.StartsWith("/"))
                request = request.Substring(1);

            Console.WriteLine("Exiting as " + request);
            return request;
        }


        public override string ToString()
        {
            if (m_SslPort == 0)
                return String.Format(m_CultureInfo, "http://*:{0}/", m_Port);
            else
                return String.Format(m_CultureInfo, "http://*:{0}/{1}https://*:{2}/", m_Port, Environment.NewLine, m_SslPort);

        }

        private void OnAppDomainUnload(Object unusedObject, EventArgs unusedEventArgs)
        {
            Thread.GetDomain().DomainUnload -= _onAppDomainUnload;
            Console.WriteLine("App domain unloaded", TraceLevel.Info);
        }

    }

}
