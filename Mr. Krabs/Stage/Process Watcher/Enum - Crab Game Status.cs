using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Stage.Process_Watcher {
    public enum CrabGameStatus {
        /// <summary>
        /// Process not run yet, or closed.
        /// </summary>
        Offline, 

        /// <summary>
        /// Process is running or opened.
        /// </summary>
        FoundRunning,
        IsAdmin,
        DllNotFound,
        DllFound
    }
}
