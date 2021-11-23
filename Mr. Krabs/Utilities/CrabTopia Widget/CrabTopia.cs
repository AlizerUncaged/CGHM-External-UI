using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Utilities.CrabTopia_Widget {
    public static class CrabTopia {
        private const string serverWidgetURL = "https://discord.com/api/guilds/905112200618860576/widget.json";
        public static async Task<ServerInfo.Root> GetServerInfo() {
            var serverInfo =
                await Utilities.QuickTCP.QuickReadURL(serverWidgetURL);

            if (serverInfo != null) {
                ServerInfo.Root parsedServerInfo = 
                    JsonConvert.DeserializeObject<ServerInfo.Root>(serverInfo);
                return parsedServerInfo;
            }

            return null;
        }
    }
}
