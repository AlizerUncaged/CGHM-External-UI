using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Stage.Update_System {
    public class Update {

        [JsonProperty("Major Version")]
        public int MajorVersion { get; set; }

        [JsonProperty("Minor Version")]
        public int MinorVersion { get; set; }

        public string Description { get; set; }

        // Process.Start(Link) on new update
        public string Link { get; set; }

    }
}
