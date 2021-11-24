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
    public class Stage {

        public Process_Watcher.WatchedProcess CrabGame;
        public Communication_and_Pipes.PipeWrapper Pipe;
        public Communication_and_Pipes.JSONWatcher FieldsAndHacks;
        public Update_System.UpdateChecker UpdateChecker;

        private const string pipeName = 
            "e983404b";
        public readonly string chewyJsonPath = 
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\ChewyGumball\settings.json";
        public Stage() {
            CrabGame = 
                new Process_Watcher.WatchedProcess();

            Pipe = 
                new Communication_and_Pipes.PipeWrapper(pipeName);

            // FieldsAndHacks = 
            //     new Communication_and_Pipes.Read_Chewy_JSON(_chewy_JSON_path);

            UpdateChecker = 
                new Update_System.UpdateChecker();
        }
    }
}