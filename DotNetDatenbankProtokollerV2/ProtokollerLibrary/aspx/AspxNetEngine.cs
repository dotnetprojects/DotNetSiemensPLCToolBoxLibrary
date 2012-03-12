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
using System.Reflection;
using System.Web.Hosting;
using System.Net;
using System.Web;
using System.IO;
using System.Globalization;

#endregion

namespace DotNetSimaticDatabaseProtokollerLibrary.aspx
{
    public class AspxNetEngine : MarshalByRefObject, IDisposable
    {
        private String m_VirtualAlias;
        private String m_PhysicalDirectory;

        public String VirtualAlias
        {
            get
            {
                return m_VirtualAlias;
            }
        }

        public String PhysicalDirectory
        {
            get
            {
                return m_PhysicalDirectory;
            }
            private set { m_PhysicalDirectory = value; }
        }

        public AspxNetEngine ExecutingAppDomain
        {
            get
            {
                return m_ExecutingAppDomain;
            }
        }

        //Needed as IDisposable is implemented
        public void Dispose()
        {
            m_ExecutingAppDomain.Dispose();
        }

        private AspxNetEngine m_ExecutingAppDomain;

        /**
         * This is just for the ASPX Hosting API's ApplicationHost object 
         * to create an object of this class when we call ApplicationHost.CreateApplicationHost
         */
        public AspxNetEngine() { }

        /**
         * Called to configure a Physical directory as a Virtual alias.
         * To configure a directory as a AspxApplication, the exe for this project ASPXhostCS.exe
         * and ASPXHostCS.exe.config (if present) should be present in the bin directory under 
         * the physical directory being configured. The reason being, that the call to API
         * ApplicationHost.CreateApplicationhost creates a new app domain and will instantiate
         * a class specified in the typeof variable. Putting it in the bin directory enables
         * the hosting api to load the class.
         */
        internal AspxNetEngine(String virtualAlias, String physicalDir)
        {
            m_VirtualAlias = virtualAlias;
            m_PhysicalDirectory = physicalDir;

            //m_ExecutingEngine will be the actual object that the hosting API created for 
            //us and so to execute a page in the Application we will call this object to 
            //process requests
            CreateAssemblyInBin(m_PhysicalDirectory);
            m_ExecutingAppDomain = (AspxNetEngine) ApplicationHost.CreateApplicationHost(typeof (AspxNetEngine), m_VirtualAlias, m_PhysicalDirectory);
            m_ExecutingAppDomain.PhysicalDirectory = m_PhysicalDirectory;

        }

        private void CreateAssemblyInBin(string path)
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();

            if (!thisAssembly.GlobalAssemblyCache)
            {
                string copiedAssemblyPath = null;
                try
                {

                    // Create the folder if it doesn't exist, flag it as hidden
                    var binFolder = System.IO.Path.Combine(path, "bin");
                    if (!Directory.Exists(binFolder))
                    {
                        Directory.CreateDirectory(binFolder);
                        File.SetAttributes(binFolder, FileAttributes.Hidden);
                    }

                    // Delete the file if it exists, copy to bin
                    string assemblyPath = new Uri(thisAssembly.CodeBase).LocalPath;
                    copiedAssemblyPath = System.IO.Path.Combine(binFolder, System.IO.Path.GetFileName(assemblyPath));
                    if (File.Exists(copiedAssemblyPath))
                        File.Delete(copiedAssemblyPath);
                    File.Copy(assemblyPath, copiedAssemblyPath);
                }
                catch (IOException)
                {
                    if (!File.Exists(copiedAssemblyPath))
                        throw;

                    if (thisAssembly.FullName != AssemblyName.GetAssemblyName(copiedAssemblyPath).FullName)
                        throw;
                }
            }
        }

        //Utility function
        public override string ToString()
        {
            return String.Format(new CultureInfo("En-us"),"AspNetEngine: (ID : {0}), (Virtual Alias  : {1}), (Physical Directory : {2})", 1, m_VirtualAlias, m_PhysicalDirectory);
        }


        //Will be called when an ASPX page is to be served.
        public void ProcessRequest(String page, string query, AspxRequestInfo dataHolder)
        { 
            //catch the exception so we dont bring down the whole app

            try
            {
                if (string.IsNullOrEmpty(page))
                {
                    if (File.Exists(Path.Combine(m_PhysicalDirectory, "index.aspx")))
                        page = "index.aspx";
                    else if (File.Exists(Path.Combine(m_PhysicalDirectory, "default.aspx")))
                        page = "default.aspx";
                    else if (File.Exists(Path.Combine(m_PhysicalDirectory, "index.html")))
                        page = "index.html";
                    else if (File.Exists(Path.Combine(m_PhysicalDirectory, "default.html")))
                        page = "default.html";
                }

                if (page != null && (page.ToLower().EndsWith("gif") || page.ToLower().EndsWith("jpg") || page.ToLower().EndsWith("png") || page.ToLower().EndsWith("html")) && File.Exists(Path.Combine(m_PhysicalDirectory, page)))
                {
                    var _FileName = Path.Combine(m_PhysicalDirectory, page);
                    FileStream _FileStream = new FileStream(_FileName, FileMode.Open, FileAccess.Read);
                    BinaryReader _BinaryReader = new BinaryReader(_FileStream);
                    long _TotalBytes = new FileInfo(_FileName).Length;
                    var _Buffer = _BinaryReader.ReadBytes((Int32)_TotalBytes);
                    _FileStream.Close();
                    _FileStream.Dispose();
                    _BinaryReader.Close();                    
                    dataHolder.ResponseStream.Write(_Buffer, 0, (int)_TotalBytes);
                }
                else
                {
                    AspxPage swr = new AspxPage(page, query, dataHolder);
                    HttpRuntime.ProcessRequest(swr);
                }
            }
            catch (Exception e1)
            {
                //Supress the internal exception. If needed we can pass the exception back
                dataHolder.ResponseStreamAsWriter.WriteLine("500 Internal Error.");
                throw new AspxException("Internal Error", e1);
            }
            finally
            {
                //Flush the response stream so that the Browser/calling application doesnt time out
                dataHolder.ResponseStreamAsWriter.Flush();
            }
        }

        /**
         * HttpListenerContext is not Marshallable and so we have an Marshallable object
         * that will contain all required objects that we need to pass on to our 
         * implementation of SimpleWorkerRequest so we can handle PUT/POST and 
         * Client Certificate requests. If any other object from HttpListenerContext class
         * is required and that Object extends MarshallByRefObject add it to the data holder
         * object here
         */
        public static AspxRequestInfo PrepareAspxRequestInfoObject(HttpListenerContext context)
        {
            AspxRequestInfo dataHolder = new AspxRequestInfo();
            dataHolder.m_CookieCollection = context.Request.Cookies;
            dataHolder.m_Headers = context.Request.Headers;
            dataHolder.m_RequestStream = context.Request.InputStream;
            dataHolder.m_ResponseStream = context.Response.OutputStream;
            dataHolder.m_UserAgent = context.Request.UserAgent;
            dataHolder.m_HttpMethod = context.Request.HttpMethod;
            dataHolder.m_ResponseStreamAsWriter = new StreamWriter(context.Response.OutputStream);
            return dataHolder;
        }

    }
}
