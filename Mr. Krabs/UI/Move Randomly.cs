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
        private const int maxFramerate = 60;
        private Interval interval;
        private Resolution max;
        private FrameworkElement[] elements;
        private IEasingFunction ease;
        private bool keepGoing = true;

        public Move_Randomly(Resolution resolution, FrameworkElement[] elements, Interval interval, IEasingFunction ease) {
            this.elements = elements; max = resolution; this.interval = interval; this.ease = ease;
        }
        private Resolution randomResolutionNotGreaterThanMax() {
            return new Resolution {
                MinHeight = 0,
                MinWidth = 0,
                MaxHeight = Utilities.Rand.Random.Next((int)max.MinHeight, (int)max.MaxHeight),
                MaxWidth = Utilities.Rand.RandomDouble((int)max.MinWidth, (int)max.MaxWidth)
            };
        }
         

        private ThicknessAnimation randomThicknessAnimation(FrameworkElement element) {
            var random_res = randomResolutionNotGreaterThanMax();
            var ta = new ThicknessAnimation {
                BeginTime = TimeSpan.Zero,
                To = new Thickness(random_res.MaxWidth, random_res.MaxHeight, element.Margin.Right, element.Margin.Bottom),
                Duration = new Duration(TimeSpan.FromMilliseconds(
                    /* 1 second to 2 second*/ Utilities.Rand.Random.Next(interval.Min, interval.Max))),
                EasingFunction = ease
            };
            Timeline.SetDesiredFrameRate(ta, maxFramerate);
            return ta;
        }

        public void Start() {
            keepGoing = true;
            // add storyboard to each blob
            foreach (var element in elements) {
                // generate random resolution first
                addAnimationToElement(element);
            }
        }

        private void addAnimationToElement(FrameworkElement element) {
            var ta = randomThicknessAnimation(element);
            ta.Completed += (s, e) => {
                if (keepGoing) {
                    // replay
                    var random_res = randomResolutionNotGreaterThanMax();
                    addAnimationToElement(element);
                }
            };
            element.BeginAnimation(FrameworkElement.MarginProperty, ta);
        }

        public void Stop() {
            keepGoing = false;
        }
    }

}
