using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Stage.Process_Watcher {

    /// with watched process
    /// this should only be one instance throughout the entire app
    /// UI reacts to StatusChanged, and all memory shit goes here

    public class Watched_Process {

        public Process_Finder ProcessWatcher;
        public Process BaseProcess = null;


        /// <returns>ProcessWatcher property.</returns>
        public async Task<Process_Finder> StartWatching() {
            await Task.Delay(1000);
            // process watcher for process running and whatnot
            ProcessWatcher = new Process_Finder();
            ProcessWatcher.ProcessFound += ProcessWatcher_ProcessFound;
            // not async
            BaseProcess = await ProcessWatcher.Start();
            BaseProcess.EnableRaisingEvents = true;
            BaseProcess.Exited += BaseProcess_Exited;

            // now check if dll is injected
            var exe_path = BaseProcess.MainModule.FileName;
            var injected = Process_Utilities.VerifyDllInjected(exe_path);
            StatusChanged?.Invoke(this, injected ? CrabGameStatus.DllFound : CrabGameStatus.DllNotFound);

            if (!injected) {
                // not injected yet
                // now inject

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

        // process ran
        private void ProcessWatcher_ProcessFound(object sender, Process e) {
            StatusChanged?.Invoke(this, CrabGameStatus.FoundRunning);
        }

        public event EventHandler<CrabGameStatus> StatusChanged;
    }
}
