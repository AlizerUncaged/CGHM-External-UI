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
        public double MinHeight, MinWidth;
        public double MaxHeight, MaxWidth;
    }

    /// <summary>
    /// Interval in milliseconds.
    /// </summary>
    public struct Interval {
        public int Min, Max;
    }
    public class Move_Randomly {

        private Interval _interval;
        private Resolution _max;
        private FrameworkElement[] _elements;
        private IEasingFunction _ease;
        private bool _keepGoing = true;
        public Move_Randomly(Resolution resolution, FrameworkElement[] elements, Interval interval, IEasingFunction ease) {
            _elements = elements; _max = resolution; _interval = interval; _ease = ease;
        }
        private Resolution _random_resolution_not_greater_than_max() {
            return new Resolution {
                MinHeight = 0,
                MinWidth = 0,
                MaxHeight = Static_Utilities.Random.Next((int)_max.MinHeight, (int)_max.MaxHeight),
                MaxWidth = Static_Utilities.RandomDouble((int)_max.MinWidth, (int)_max.MaxWidth)
            };
        }


        private ThicknessAnimation _random_thickness(FrameworkElement element) {
            var random_res = _random_resolution_not_greater_than_max();
            var ta = new ThicknessAnimation {
                BeginTime = TimeSpan.Zero,
                To = new Thickness(random_res.MaxWidth, random_res.MaxHeight, element.Margin.Right, element.Margin.Bottom),
                Duration = new Duration(TimeSpan.FromMilliseconds(
                    /* 1 second to 2 second*/ Static_Utilities.Random.Next(_interval.Min, _interval.Max))),
                EasingFunction = _ease
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
            var ta = _random_thickness(element);
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
