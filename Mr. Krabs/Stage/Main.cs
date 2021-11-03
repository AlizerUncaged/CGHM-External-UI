using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mr.Krabs.Stage {

    /// <summary>
    /// Expose to UI.
    /// </summary>
    public class Stage  {

        public Process_Watcher.Watched_Process CrabGame;

        public Stage() {
            CrabGame = new Process_Watcher.Watched_Process();
        }
    }
}
