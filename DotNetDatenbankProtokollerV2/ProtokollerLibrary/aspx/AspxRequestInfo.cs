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
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace DotNetSimaticDatabaseProtokollerLibrary.aspx
{
    //Class that acts as a data holder so we can pass all required objects 
    //to the ASPX hosting app domain
    public sealed class AspxRequestInfo : MarshalByRefObject
    {
        internal Stream m_RequestStream;
        internal Stream m_ResponseStream;
        internal NameValueCollection m_Headers;
        internal CookieCollection m_CookieCollection;
        internal String m_UserAgent;
        internal TextWriter m_ResponseStreamAsWriter;
        internal X509Certificate m_ClientCertificate;
        internal String m_HttpMethod;

        internal AspxRequestInfo()
        {
        }

        public String HttpMethod
        {
            get
            {
                return m_HttpMethod;
            }
        }

        public Stream RequestStream
        {
            get
            {
                return m_RequestStream;
            }
        }

        public Stream ResponseStream
        {
            get
            {
                return m_ResponseStream;
            }
        }

        public TextWriter ResponseStreamAsWriter
        {
            get
            {
                return m_ResponseStreamAsWriter;
            }
        }

        public NameValueCollection RequestHeaders
        {
            get
            {
                return m_Headers;
            }
        }

        public CookieCollection Cookies
        {
            get
            {
                return m_CookieCollection;
            }
        }

        public String UserAgent
        {
            get
            {
                return m_UserAgent;
            }
        }

        public X509Certificate ClientCertificate
        {
            get
            {
                return m_ClientCertificate;
            }
        }
    }

}
