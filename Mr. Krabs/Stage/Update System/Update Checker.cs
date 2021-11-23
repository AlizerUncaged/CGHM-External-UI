using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mr.Krabs.Stage.Update_System {
    public struct Update_Result {
        public bool Success, NewVersion;
        public Update Data;
    }
    public class Update_Checker {

        private const string _update_data = "http://95.111.251.138/cghm/update.json";

        /// <summary>
        /// Returns null if there's no new update.
        /// </summary>
        public async Task<Update_Result> GetLink() {

            Update_Result result = new Update_Result {
                Success = false,
                NewVersion = false
            };

            string data_from_server = await Utilities.QuickTCP.QuickReadURL(_update_data);

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
