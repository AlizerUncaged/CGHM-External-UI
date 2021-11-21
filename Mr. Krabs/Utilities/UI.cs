using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Mr.Krabs.Utilities {
    public static class UI {
        public static void RunAnimation(this FrameworkElement window, string resourceName) {
            Storyboard sb = window.FindResource(resourceName) as Storyboard;
            Timeline.SetDesiredFrameRate(sb, 60);
            if (sb != null) { sb.Begin(); }
        }

    }
}
