using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Stage.Process_Watcher {
    public static class Process_Utilities {
        public async static Task<IEnumerable<Process>> FindProcessViaWindowName(string window_name) {
            return await Task.Run(() => {
                List<Process> procs = new List<Process>();
                var running_procs = Process.GetProcesses();
                foreach (var proc in running_procs)
                    if (proc.MainWindowTitle.ToLower().Trim() == window_name.ToLower().Trim())
                        procs.Add(proc);
                return procs;
            });
        }
        /// <summary>
        /// Verifies if Dll is in the game's folder and the hash is the same as the latest Dll's.
        /// </summary>
        public static bool VerifyDllInjected(string exe_path) {
            string directory = Path.GetDirectoryName(exe_path);
            if (File.Exists($@"{directory}\version.dll")) {
                // exists
                return true;
            }
            // check if has is the same
            return false;
        }
    }
}
