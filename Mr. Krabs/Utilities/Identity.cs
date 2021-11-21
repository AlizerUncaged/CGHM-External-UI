﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Utilities {
    public static class Identity {
        public const int MajorVersion = 1;
        public const int MinorVersion = 4;

        public static bool AmIAdmin() {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent()) {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static bool CopyToCrabGameFolder(string crabGameFolder) {
            var d = new DirectoryInfo(FileSystem.CurrentFolder);
            var f = d.GetFiles();
            foreach (var g in f) {

                var file = $"{crabGameFolder}/{g.Name}";

                if (!File.Exists(file)) {
                    File.Copy(g.FullName, file, true);
                }
            }
            return true;
        }
        public static bool CheckIfCopiedToCrabGame(string crabGameFolder) {
            var d = new DirectoryInfo(FileSystem.CurrentFolder);
            var f = d.GetFiles();
            foreach (var g in f) {
                if (!File.Exists($"{crabGameFolder}/{g.Name}")) {
                    return false;
                }
            }
            return true;
        }

        public struct Required_Dll {
            public bool AllFound; public string Path; public string Name; public string Link; public string Desc;
        }
        private static readonly List<Required_Dll> _required_files = new List<Required_Dll>{
            new Required_Dll {
                AllFound = false,
                Path = @"C:\Windows\SysWOW64\D3DCompiler_43.dll",
                Link = "https://www.microsoft.com/en-us/download/confirmation.aspx?id=8109",
                Desc = "Required for rendering the internal UI."
            }
        };
        public async static Task<Required_Dll> CheckRequiredDlls() {
            return await Task.Run(() => {
                foreach (var j in _required_files) {
                    if (!File.Exists(j.Path)) return j;
                }
                return new Required_Dll { AllFound = true };
            });
        }

        
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        private const int TOKEN_ASSIGN_PRIMARY = 0x1;
        private const int TOKEN_DUPLICATE = 0x2;
        private const int TOKEN_IMPERSONATE = 0x4;
        private const int TOKEN_QUERY = 0x8;
        private const int TOKEN_QUERY_SOURCE = 0x10;
        private const int TOKEN_ADJUST_GROUPS = 0x40;
        private const int TOKEN_ADJUST_PRIVILEGES = 0x20;
        private const int TOKEN_ADJUST_SESSIONID = 0x100;
        private const int TOKEN_ADJUST_DEFAULT = 0x80;
        private const int TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_SESSIONID | TOKEN_ADJUST_DEFAULT);

        // true if admin
        public static bool IsProcessOwnerAdmin(Process proc) {
            try {
                IntPtr ph = IntPtr.Zero;

                OpenProcessToken(proc.Handle, TOKEN_ALL_ACCESS, out ph);

                WindowsIdentity iden = new WindowsIdentity(ph);

                bool result = false;

                foreach (IdentityReference role in iden.Groups) {
                    if (role.IsValidTargetType(typeof(SecurityIdentifier))) {
                        SecurityIdentifier sid = role as SecurityIdentifier;

                        if (sid.IsWellKnown(WellKnownSidType.AccountAdministratorSid) || sid.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid)) {
                            result = true;
                            break;
                        }
                    }
                }

                CloseHandle(ph);

                return result;
            } catch {
                // its admin
                return true;
            }
        }
    }
}