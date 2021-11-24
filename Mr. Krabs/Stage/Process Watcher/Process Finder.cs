using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Stage.Process_Watcher {
    public class ProcessFinder {

        private string windowName;

        private IEnumerable<Process> foundProcesses = null;

        public ProcessFinder(string window_name = "Crab Game") {
            windowName = window_name;
        }

        public event EventHandler<IEnumerable<Process>> ProcessFound;

        public async Task<IEnumerable<Process>> Start() {
            do {

                var proces = await Process_Utilities.FindProcessViaWindowName(windowName);

                if (proces.Count() > 0) {
                    foundProcesses = proces;
                    // found process
                    ProcessFound?.Invoke(this, foundProcesses);
                } else await Task.Delay(1000);

            } while (foundProcesses == null);

            return foundProcesses;
        }

    }
}
