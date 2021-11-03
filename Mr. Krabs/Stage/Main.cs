﻿using System;
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

        private readonly string _chewy_JSON_path = 
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\ChewyGumball\settings.json";
        public Stage() {
            CrabGame = 
                new Process_Watcher.Watched_Process();

            Pipe = 
                new Communication_and_Pipes.Pipe_Wrapper(
                Communication_and_Pipes.Communication._pipe_name);

            FieldsAndHacks = 
                new Communication_and_Pipes.Read_Chewy_JSON(_chewy_JSON_path);
        }
    }
}