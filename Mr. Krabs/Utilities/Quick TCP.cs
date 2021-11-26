using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Utilities {
    public static class QuickTCP {
        // returns null if no internet
        public static async Task<string> QuickReadURL(string url) {
            string s = null;
            try {
                using (WebClient client = new WebClient()) {
                    s = await client.DownloadStringTaskAsync(url);
                    Debug.WriteLine($"TCP in: {s}");
                }
            } catch (Exception ex) { Debug.WriteLine($"Reading TCP Error: {ex}"); }
            return s;
        }

    }
}
