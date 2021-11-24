﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mr.Krabs.Stage.Update_System {
    public struct UpdateResult {
        public bool Success, NewVersion;
        public Update Data;
    }
    public class UpdateChecker {

        private const string updateEndpoint = "http://95.111.251.138/cghm/update.json";

        /// <summary>
        /// Returns null if there's no new update.
        /// </summary>
        public async Task<UpdateResult> GetLink() {

            UpdateResult result = new UpdateResult {
                Success = false,
                NewVersion = false
            };

            string data_from_server = await Utilities.QuickTCP.QuickReadURL(updateEndpoint);

            if (data_from_server == null) return result;

            result.Success = true;

            result.Data = JsonConvert.DeserializeObject<Update>(data_from_server);
            string newVersionFloatString = $"{result.Data.MajorVersion}.{result.Data.MinorVersion}";
            string oldVersionFloatString = $"{Utilities.Identity.MajorVersion}.{Utilities.Identity.MinorVersion}";
            float oldVersion = float.Parse(oldVersionFloatString);
            float newVersion = float.Parse(newVersionFloatString);
            result.NewVersion = newVersion > oldVersion;

            return result;
        }
    }
}
