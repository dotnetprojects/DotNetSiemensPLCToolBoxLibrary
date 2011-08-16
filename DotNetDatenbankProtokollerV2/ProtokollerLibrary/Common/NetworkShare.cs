using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DotNetSimaticDatabaseProtokollerLibrary.Common
{
    internal class NetworkShare : IDisposable
    {

        #region Error Message


        private string GetSystemMessage(int errorCode)
        {
            int capacity = 512;
            int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
            StringBuilder sb = new StringBuilder(capacity);
            FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, errorCode, 0,
                          sb, sb.Capacity, IntPtr.Zero);
            int i = sb.Length;
            if (i > 0 && sb[i - 1] == 10) i--;
            if (i > 0 && sb[i - 1] == 13) i--;
            sb.Length = i;
            return sb.ToString();
        }

        [DllImport("kernel32.dll")]
        public static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr Arguments);

        #endregion

        private string _shareName;

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NETRESOURCE netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);

        private enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET = 2,
            RESOURCE_REMEMBERED = 3,
            RESOURCE_RECENT = 4,
            RESOURCE_CONTEXT = 5
        };

        private enum ResourceType
        {
            RESOURCETYPE_ANY = 0,
            RESOURCETYPE_DISK = 1,
            RESOURCETYPE_PRINT = 2,
            RESOURCETYPE_RESERVED = 8
        };

        private enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010,
            RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
        };

        private enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC = 0,
            RESOURCEDISPLAYTYPE_DOMAIN = 1,
            RESOURCEDISPLAYTYPE_SERVER = 2,
            RESOURCEDISPLAYTYPE_SHARE = 3,
            RESOURCEDISPLAYTYPE_FILE = 4,
            RESOURCEDISPLAYTYPE_GROUP = 5,
            RESOURCEDISPLAYTYPE_NETWORK = 6,
            RESOURCEDISPLAYTYPE_ROOT = 7,
            RESOURCEDISPLAYTYPE_SHAREADMIN = 8,
            RESOURCEDISPLAYTYPE_DIRECTORY = 9,
            RESOURCEDISPLAYTYPE_TREE = 10,
            RESOURCEDISPLAYTYPE_NDSCONTAINER = 11
        };

        [StructLayout(LayoutKind.Sequential)]
        private class NETRESOURCE
        {
            public ResourceScope dwScope = 0;
            public ResourceType dwType = 0;
            public ResourceDisplayType dwDisplayType = 0;
            public ResourceUsage dwUsage = 0;
            public string lpLocalName = null;
            public string lpRemoteName = null;
            public string lpComment = null;
            public string lpProvider = null;
        };


        public NetworkShare(string ShareName, string User, string Password)
        {
            _shareName = ShareName;

            NETRESOURCE myNetResource = new NETRESOURCE();
            myNetResource.dwScope = ResourceScope.RESOURCE_GLOBALNET;
            myNetResource.dwType = ResourceType.RESOURCETYPE_DISK;
            myNetResource.dwDisplayType = ResourceDisplayType.RESOURCEDISPLAYTYPE_SHARE;
            myNetResource.dwUsage = ResourceUsage.RESOURCEUSAGE_ALL;
            myNetResource.lpLocalName = null;
            myNetResource.lpRemoteName = ShareName;
            myNetResource.lpProvider = null;

            int ret = WNetAddConnection2(myNetResource, Password, User, 0);

            if (ret != 0)
                throw new Exception(GetSystemMessage(ret));
        }

        #region Implementation of IDisposable

        protected virtual void Dispose(bool disposing)
        {
            WNetCancelConnection2(_shareName, 0, true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~NetworkShare()
        {
            Dispose(false);
        }

        #endregion
    }
}
