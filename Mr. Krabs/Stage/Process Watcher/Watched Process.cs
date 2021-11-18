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

    public class Watched_Process {

        public Process_Finder ProcessWatcher;
        public Process BaseProcess = null;
        /// <returns>ProcessWatcher property.</returns>
        public async Task<Process_Finder> StartWatching() {
            await Task.Delay(1000);
            // process watcher for process running and whatnot
            ProcessWatcher = new Process_Finder();
            // ProcessWatcher.ProcessFound += ProcessWatcher_ProcessFound;
            // not async
            var procs = await ProcessWatcher.Start();
            BaseProcess = _get_valid_crabgame_process(procs);

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

        private Process _get_valid_crabgame_process(IEnumerable<Process> e) {
            foreach (var process in e) {

                var isadmin =
                    Static_Utilities.IsProcessOwnerAdmin(process);

                if (isadmin)
                    StatusChanged?.Invoke(this, CrabGameStatus.IsAdmin);

                // return if we're not admin
                if (isadmin && !Static_Utilities.AmIAdmin()) 
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
