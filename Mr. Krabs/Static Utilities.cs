using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Mr.Krabs {
    public static class Static_Utilities {

        public static Random Random = new Random(DateTime.UtcNow.Second + DateTime.UtcNow.Millisecond);

        public static double RandomDouble(double max, double min = 0) {
            return Random.NextDouble() * (min - max) + max;
        }

        public static void RunAnimation(this Window window, string resourceName) {
            Storyboard sb = window.FindResource(resourceName) as Storyboard;
            if (sb != null) { sb.Begin(); }
        }
    }
}
