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

        public Process_Watcher.Watched_Process CrabGame;
        public Communication_and_Pipes.Pipe_Wrapper Pipe;
        public Communication_and_Pipes.Read_Chewy_JSON FieldsAndHacks;
        public Update_System.Update_Checker UpdateChecker;

        private const string PIPE_NAME = 
            "e983404b";
        public readonly string CHEWY_JSON_PATH = 
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\ChewyGumball\settings.json";
        public Stage() {
            CrabGame = 
                new Process_Watcher.Watched_Process();

            Pipe = 
                new Communication_and_Pipes.Pipe_Wrapper(PIPE_NAME);

            // FieldsAndHacks = 
            //     new Communication_and_Pipes.Read_Chewy_JSON(_chewy_JSON_path);

            UpdateChecker = 
                new Update_System.Update_Checker();
        }
    }
}