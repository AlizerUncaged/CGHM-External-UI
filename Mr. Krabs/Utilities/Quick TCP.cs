using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Utilities {
    public static class QuickTCP {
        public static async Task<string> QuickReadURL(string url) {
            return await Task.Run(() => {
                string s = null;
                try {
                    using (WebClient client = new WebClient()) {
                        s = client.DownloadString(url);
                        Debug.WriteLine($"TCP in: {s}");
                    }
                } catch { }
                return s;
            });
        }

    }
}
