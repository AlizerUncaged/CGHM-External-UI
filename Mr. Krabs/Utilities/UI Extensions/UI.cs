using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Mr.Krabs.Utilities.UI_Extensions {
    public static class UI {
        public static void RunAnimation(this ContentControl window, string resourceName) {
            Storyboard sb = window.FindResource(resourceName) as Storyboard;
            if (sb != null) {
                Timeline.SetDesiredFrameRate(sb, 60); 
                sb.Begin();
            }
        }

    }
}
