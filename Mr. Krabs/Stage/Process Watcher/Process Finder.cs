using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Stage.Process_Watcher {
    public class Process_Finder {

        private string _window_name;

        private Process _base_proc = null;

        public Process_Finder(string window_name = "Crab Game") {
            _window_name = window_name;
        }

        public event EventHandler<Process> ProcessFound;

        public async Task<Process> Start() {
            // watch for each second find crab game via title
            do {

                var proces = await Process_Utilities.FindProcessViaWindowName(_window_name);

                if (proces.Count() > 0) {
                    // found process
                    _base_proc = proces.First();
                    ProcessFound?.Invoke(this, _base_proc);
                }

            } while (_base_proc == null);

            return _base_proc;
        }

    }
}
