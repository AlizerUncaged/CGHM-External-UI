using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Stage.Process_Watcher {

    // with watched process
    // this should only be one instance throughout the entire app
    // UI reacts to StatusChanged, and all memory shit goes here

    public class WatchedProcess {

        public ProcessFinder ProcessWatcher;
        public Process BaseProcess = null;
        public string GetCrabGameDirectory() {
            return 
                Path.GetDirectoryName(
                BaseProcess.MainModule.FileName);
        }
        /// <returns>ProcessWatcher property.</returns>
        public async Task<ProcessFinder> StartWatching() {
            // await Task.Delay(1000);
            // process watcher for process running and whatnot
            ProcessWatcher = new ProcessFinder();
            // ProcessWatcher.ProcessFound += ProcessWatcher_ProcessFound;
            // not async
            var procs = await ProcessWatcher.Start();
            BaseProcess = getValidCrabGameProcess(procs);

            BaseProcess.EnableRaisingEvents = true;
            BaseProcess.Exited += BaseProcess_Exited;

            // now check if dll is injected
            var exe_path = BaseProcess.MainModule.FileName;
            var injected = Process_Utilities.VerifyDllInjected(exe_path);
            StatusChanged?.Invoke(this, injected ? CrabGameStatus.DllFound : CrabGameStatus.DllNotFound);

            if (!injected) {
                // not injected yet
                // now inject
                _ = await Process_Utilities.AddDllToFolder(exe_path);
                BaseProcess = await Process_Utilities.RestartProcess(BaseProcess);
                StatusChanged?.Invoke(this, CrabGameStatus.DllFound);
                // restart crab game
            }

            return ProcessWatcher;
        }

        // process exited
        private void BaseProcess_Exited(object sender, EventArgs e) {
            StatusChanged?.Invoke(this, CrabGameStatus.Offline);

            // find again
            _ = StartWatching();
        }

        private Process getValidCrabGameProcess(IEnumerable<Process> e) {
            foreach (var process in e) {

                var isRanAsAdmin =
                    Utilities.Identity.IsProcessOwnerAdmin(process);

                if (isRanAsAdmin)
                    StatusChanged?.Invoke(this, CrabGameStatus.IsAdmin);

                // check if the process was ran as admin and we're not admin
                // if we're not ran as admin but crabgame is ran as admin return
                if (isRanAsAdmin && !Utilities.Identity.AmIAdmin()) 
                    return null;

                // now check if the file path is correct if GameAssembly is there
                var appdir = 
                    Path.GetDirectoryName(process.MainModule.FileName);

                var userassemblypath = $@"{appdir}\GameAssembly.dll";

                if (File.Exists(userassemblypath)) {
                    StatusChanged?.Invoke(this, CrabGameStatus.FoundRunning);
                    return process;
                }

            }
            return null;
        }


        public event EventHandler<CrabGameStatus> StatusChanged;
    }
}
