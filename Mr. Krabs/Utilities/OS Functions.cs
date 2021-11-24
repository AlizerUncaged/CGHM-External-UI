using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Utilities {
    public static class OSFunctions {
        public static void RestartAsAdmin() {
            //Create a new process
            Process target = new Process();

            target.StartInfo.FileName =
                FileSystem.CurrentFilename;

            //Required for UAC to work
            target.StartInfo.UseShellExecute = true;
            target.StartInfo.Verb = "runas";

            try {
                target.Start();
                Process.GetCurrentProcess().Kill();
            } catch { }

        }

        public static void Exit() {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
