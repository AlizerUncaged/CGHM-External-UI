using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Mr.Krabs {

    public static class Static_Utilities {

        public const int MajorVersion = 1;
        public const int MinorVersion = 1;

        public static Random Random = new Random(DateTime.UtcNow.Second + DateTime.UtcNow.Millisecond);

        public static readonly string CurrentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static double RandomDouble(double max, double min = 0) {
            return Random.NextDouble() * (min - max) + max;
        }

        public static void RunAnimation(this Window window, string resourceName) {
            Storyboard sb = window.FindResource(resourceName) as Storyboard;
            if (sb != null) { sb.Begin(); }
        }

        public static async Task<string> QuickReadURL(string url) {
            return await Task.Run(() => {
                using (WebClient client = new WebClient()) {
                    string s = client.DownloadString(url);
                    return s;
                }
            });
        }
    }
}
