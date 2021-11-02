using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Mr.Krabs.UI {

    public struct Resolution {
        public double MaxHeight, MaxWidth;
    }
    public class Move_Randomly {

        private Resolution _max;
        private Ellipse[] _elements;
        private bool _keepGoing = true;
        public Move_Randomly(Resolution resolution, Ellipse[] elements) {
            _elements = elements; _max = resolution;
        }
        private Resolution _random_resolution_not_greater_than_max() {
            return new Resolution {
                MaxHeight = Static_Utilities.RandomDouble(_max.MaxHeight),
                MaxWidth = Static_Utilities.RandomDouble(_max.MaxWidth)
            };
        }
        private ThicknessAnimation _random_thickness() {

            var random_res = _random_resolution_not_greater_than_max();
            var ta = new ThicknessAnimation {
                BeginTime = TimeSpan.Zero,
                To = new Thickness(random_res.MaxWidth, random_res.MaxHeight, 0, 0),
                Duration = new Duration(TimeSpan.FromMilliseconds( 
                    /* 1 second to 2 second*/ Static_Utilities.Random.Next(4000, 6000))),
                EasingFunction = new QuadraticEase()
            };

            return ta;
        }

        public void Start() {
            _keepGoing = true;
            // add storyboard to each blob
            foreach (var element in _elements) {
                // generate random resolution first
                _add_animation_to_element(element);
            }
        }

        private void _add_animation_to_element(FrameworkElement element) {
            var ta = _random_thickness();
            ta.Completed += (s, e) => {
                if (_keepGoing) {
                    // replay
                    var random_res = _random_resolution_not_greater_than_max();
                    _add_animation_to_element(element);
                }
            };
            element.BeginAnimation(FrameworkElement.MarginProperty, ta);
        }

        public void Stop() {
            _keepGoing = false;
        }
    }

}
