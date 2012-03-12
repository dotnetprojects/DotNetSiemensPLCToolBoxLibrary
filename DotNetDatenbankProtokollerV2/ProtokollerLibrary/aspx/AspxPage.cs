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
using System.Collections.Generic;
using System.Text;
using System.Web.Hosting;
using System.Globalization;

#endregion

/***
 * This is the class that we will use to process a given request. Hosting API provided 
 * a class HttpWorkerRequest that is a abstract class that takes in a HttpListenerContext.
 * Hosting API also provides the SimpleWorkerRequest that is a simple implementation of 
 * the parent class. Overriding the SimpleWorkerRequest gives us "Out-of-box" functionality
 * and requiring us to implement methods of concern to the application. This class needs to
 * handle Post/Put requests and will need to have access to Client Certificates if accessible.
 * So, this class only implements the following required overloads.
 */
namespace DotNetSimaticDatabaseProtokollerLibrary.aspx
{
    public class AspxPage : SimpleWorkerRequest
    {
        private AspxRequestInfo m_EngineDataHolder;
    
        public AspxPage(String page, String query, AspxRequestInfo dataHolder) : base(page,query,dataHolder.ResponseStreamAsWriter)
        {
            m_EngineDataHolder = dataHolder;
        }


        //Retrieve the http verb of the incoming request
        public override string GetHttpVerbName()
        {
            return m_EngineDataHolder.HttpMethod;
        }

        //Retrieve value for one of the standard known header
        public override string GetKnownRequestHeader(int index)
        {
            switch (index)
            {
                case HeaderUserAgent:
                    return m_EngineDataHolder.UserAgent;
                default:
                    return m_EngineDataHolder.RequestHeaders[GetKnownRequestHeaderName(index)];
            }
        }

        //Return header if part of custom headers
        public override string[][] GetUnknownRequestHeaders()
        {
            string[][] unknownRequestHeaders;
            System.Collections.Specialized.NameValueCollection headers = m_EngineDataHolder.RequestHeaders;
            int count = headers.Count;
            List<string[]> headerPairs = new List<string[]>(count);
            for (int i = 0; i < count; i++)
            {
                string headerName = headers.GetKey(i);
                if (GetKnownRequestHeaderIndex(headerName) == -1)
                {
                    string headerValue = headers.Get(i);
                    headerPairs.Add(new string[] { headerName, headerValue });
                }
            }
            unknownRequestHeaders = headerPairs.ToArray();
            return unknownRequestHeaders;
        }

        //Called for POST /PUT requests
        public override int ReadEntityBody(byte[] buffer, int size)
        {
            return this.ReadEntityBody(buffer,0,size);
        }

        //Override call for POST/PUT requests
        public override int ReadEntityBody(byte[] buffer, int offset, int size)
        {
            //Some boundary conditions
            if (buffer == null)
                return -1;
            if (offset < 0 || size < 0)
                return -1;
            if (size > (offset + buffer.Length))
                return -1;
            if (offset > size)
                return -1;
            if (size > buffer.Length)
                return -1;
            if ((size - offset) > buffer.Length)
                return -1;

            return m_EngineDataHolder.RequestStream.Read(buffer, offset, size);
        }

        //Get the cert data as bytest
        public override byte[] GetClientCertificate()
        {
            if (m_EngineDataHolder.ClientCertificate == null)
                return null;

            //We have a certificate and so lets serve it
            return m_EngineDataHolder.ClientCertificate.GetRawCertData();
        }

        //Get the certificate public key bytes
        public override byte[] GetClientCertificatePublicKey()
        {
            if (m_EngineDataHolder.ClientCertificate == null)
                return null;

            //We have a certificate and so lets serve it
            return m_EngineDataHolder.ClientCertificate.GetPublicKey();
        }


        //Get the issuer of the certificate
        public override byte[] GetClientCertificateBinaryIssuer()
        {
            if (m_EngineDataHolder.ClientCertificate == null)
                return null;

            //We have a certificate and so lets serve it
            return Encoding.ASCII.GetBytes(m_EngineDataHolder.ClientCertificate.Issuer);
        }


        //Get the issue date of the cert
        public override DateTime GetClientCertificateValidFrom()
        {
            if (m_EngineDataHolder.ClientCertificate == null)
                return DateTime.Now;

            //We have a certificate and so lets serve it
            return DateTime.Parse(m_EngineDataHolder.ClientCertificate.GetEffectiveDateString(), new CultureInfo("En-us"));
        }


        //Get the expiration date of the certificate
        public override DateTime GetClientCertificateValidUntil()
        {
            if (m_EngineDataHolder.ClientCertificate == null)
                return DateTime.Now;

            //We have a certificate and so lets serve it
            return DateTime.Parse(m_EngineDataHolder.ClientCertificate.GetExpirationDateString(),new CultureInfo("En-us"));
        }

    }
}
