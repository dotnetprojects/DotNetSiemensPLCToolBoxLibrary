using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Automation;

namespace TiaGitHandler
{
    public static class TiaOpennessWhitelist
    {
        /// <summary>
        /// Retrieves the path to the currently executing .exe
        /// </summary>
        private static string GetApplicationPath()
        {
            // use the executing assembly’s location
            var path = Assembly.GetExecutingAssembly().Location;

            if (!string.IsNullOrEmpty(path))
                return path;

            // use the main module’s file name of the current process
            using (var proc = Process.GetCurrentProcess())
            {
                path = proc.MainModule?.FileName;
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            throw new InvalidOperationException("Could not determine the application path.");
        }

        /// <summary>
        /// Ensures that a whitelist entry exists and is up-to-date.
        /// Creates or updates the entry only if Path, DateModified, or FileHash differ.
        /// </summary>
        /// <param name="tiaVersion">TIA Openness version, e.g. "V16.0".</param>
        /// <returns>True if the entry was created or updated; false if it was already current.</returns>
        public static bool EnsureWhitelistEntry(string tiaVersion)
        {
            // 1. determine the application path
            var applicationPath = GetApplicationPath();
            var exeName = Path.GetFileName(applicationPath);

            // 2. Compute the desired DateModified value
            var fileInfo = new FileInfo(applicationPath);
            var desiredDate = fileInfo.LastWriteTimeUtc
                .ToString("yyyy/MM/dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);

            // 3. Compute the desired FileHash value (SHA-256, Base64)
            byte[] hashBytes;
            using (var sha = SHA256.Create())
            using (var stream = File.OpenRead(applicationPath))
                hashBytes = sha.ComputeHash(stream);
            var desiredHash = Convert.ToBase64String(hashBytes);

            // 4. Construct the registry subkey path
            var parseResult = float.TryParse(tiaVersion, out var version);
            if (!parseResult)
                throw new ArgumentException($"Invalid TIA version format: {tiaVersion}");

            var subKey = version > 20.0 ? $@"SOFTWARE\Siemens\Automation\Openness\AllowList\{exeName}\Entry" : $@"SOFTWARE\Siemens\Automation\Openness\{tiaVersion}\Whitelist\{exeName}\Entry";

            // 5. Open or create the registry key
            using (var entryKey = Registry.LocalMachine.OpenSubKey(subKey, writable: true)
                                  ?? Registry.LocalMachine.CreateSubKey(subKey, writable: true))
                {
                    if (entryKey == null)
                        throw new InvalidOperationException($"Could not create or open registry key: HKLM\\{subKey}");

                    // 6. Read existing values
                    var currentPath = entryKey.GetValue("Path") as string;
                    var currentDate = entryKey.GetValue("DateModified") as string;
                    var currentHash = entryKey.GetValue("FileHash") as string;

                    // 7. Check if any value differs
                    var needsUpdate =
                        currentPath != applicationPath ||
                        currentDate != desiredDate ||
                        currentHash != desiredHash;

                    if (!needsUpdate)
                        return false;  // already up-to-date

                    // 8. Write the new values
                    entryKey.SetValue("Path", applicationPath, RegistryValueKind.String);
                    entryKey.SetValue("DateModified", desiredDate, RegistryValueKind.String);
                    entryKey.SetValue("FileHash", desiredHash, RegistryValueKind.String);

                    return true;
                }
        }
    }
}
