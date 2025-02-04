﻿using System.ComponentModel;
using System.Security.Principal;

namespace Bloxstrap
{
    static class Utilities
    {
        /// <summary>
        /// Is process running as administrator
        /// https://stackoverflow.com/a/11660205
        /// </summary>
        public static bool IsAdministrator =>
           new WindowsPrincipal(WindowsIdentity.GetCurrent())
               .IsInRole(WindowsBuiltInRole.Administrator);

        public static void ShellExecute(string website)
        {
            try
            {
                Process.Start(new ProcessStartInfo 
                { 
                    FileName = website, 
                    UseShellExecute = true 
                });
            }
            catch (Win32Exception ex)
            {
                // lmfao

                if (ex.NativeErrorCode != (int)ErrorCode.CO_E_APPNOTFOUND)
                    throw;

                Process.Start(new ProcessStartInfo
                {
                    FileName = "rundll32.exe",
                    Arguments = $"shell32,OpenAs_RunDLL {website}"
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="versionStr1"></param>
        /// <param name="versionStr2"></param>
        /// <returns>
        /// Result of System.Version.CompareTo <br />
        /// -1: version1 &lt; version2 <br />
        ///  0: version1 == version2 <br />
        ///  1: version1 &gt; version2
        /// </returns>
        public static int CompareVersions(string versionStr1, string versionStr2)
        {
            var version1 = new Version(versionStr1.Replace("v", ""));
            var version2 = new Version(versionStr2.Replace("v", ""));

            return version1.CompareTo(version2);
        }

        public static string GetRobloxVersion(bool studio)
        {
            string versionGuid = studio ? App.State.Prop.StudioVersionGuid : App.State.Prop.PlayerVersionGuid;
            string fileName = studio ? "RobloxStudioBeta.exe" : "eurotrucks2.exe";

            string playerLocation = Path.Combine(Paths.Versions, versionGuid, fileName);

            if (!File.Exists(playerLocation))
                return "";

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(playerLocation);

            if (versionInfo.ProductVersion is null)
                return "";

            return versionInfo.ProductVersion.Replace(", ", ".");
        }

        public static Process[] GetProcessesSafe()
        {
            const string LOG_IDENT = "Utilities::GetProcessesSafe";

            try
            {
                return Process.GetProcesses();
            }
            catch (ArithmeticException ex) // thanks microsoft
            {
                App.Logger.WriteLine(LOG_IDENT, $"Unable to fetch processes!");
                App.Logger.WriteException(LOG_IDENT, ex);
                return Array.Empty<Process>(); // can we retry?
            }
        }
    }
}
