using System.Security;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    public class Credentials
    {
        public string Username { get; set; }

        public SecureString Password { get; set; }
    }
}
