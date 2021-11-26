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

    public class MoveRandomly {
        public static List<MoveRandomly> Instances = new List<MoveRandomly>();

        public static void PauseAnimations() {
            foreach (var moveRandAnimation in Instances) {
                moveRandAnimation.Pause();
            }
        }

        public static void StartAnimations() {
            foreach (var moveRandAnimation in Instances) {
                moveRandAnimation.Play();
            }
        }

        private const int maxFramerate = 60;
        private Interval interval;
        private Resolution max;
        private FrameworkElement[] elements;
        private List<Storyboard> storyboards = new List<Storyboard>();
        private IEasingFunction ease;
        private bool paused = false, disposing = false;

        public MoveRandomly(Resolution resolution, FrameworkElement[] elements, Interval interval, IEasingFunction ease) {
            this.elements = elements; max = resolution; this.interval = interval; this.ease = ease;
            Instances.Add(this);
        }
        private Resolution randomResolutionNotGreaterThanMax() {
            return new Resolution {
                MinHeight = 0,
                MinWidth = 0,
                MaxHeight = Utilities.Rand.Random.Next((int)max.MinHeight, (int)max.MaxHeight),
                MaxWidth = Utilities.Rand.RandomDouble((int)max.MinWidth, (int)max.MaxWidth)
            };
        }


        private Storyboard randomThicknessAnimation(FrameworkElement element) {
            var random_res = randomResolutionNotGreaterThanMax();
            var ta = new ThicknessAnimation {
                BeginTime = TimeSpan.Zero,
                To = new Thickness(random_res.MaxWidth, random_res.MaxHeight, element.Margin.Right, element.Margin.Bottom),
                Duration = new Duration(TimeSpan.FromMilliseconds(
                    /* 1 second to 2 second*/ Utilities.Rand.Random.Next(interval.Min, interval.Max))),
                EasingFunction = ease
            };

            Timeline.SetDesiredFrameRate(ta, maxFramerate);

            Storyboard storyboard = new Storyboard();
            Storyboard.SetTarget(ta, element);

            Storyboard.SetTargetProperty(ta, new PropertyPath("(FrameworkElement.Margin)"));
            storyboard.Children.Add(ta);
            return storyboard;
        }

        public void Start() {
            // add storyboard to each blob
            foreach (var element in elements) {
                // generate random resolution first
                addAnimationToElement(element);
            }
        }

        private void addAnimationToElement(FrameworkElement element) {
            var ta = randomThicknessAnimation(element);
            storyboards.Add(ta);
            ta.Completed += (s, e) => {

                if (!disposing) addAnimationToElement(element);
            };

            ta.Begin();
            if (paused) ta.Pause();

            // Storyboard.SetTargetProperty(translateYAnimation, "TranslateY");
            // element.BeginAnimation(FrameworkElement.MarginProperty, ta);
        }

        public void Stop() {
            disposing = true;
            foreach (var storyboard in storyboards) {
                storyboard.Stop();
                // generate random resolution first
                // element.BeginAnimation(FrameworkElement.MarginProperty, null);
            }
        }
        public void Pause() {
            paused = true;
            foreach (var storyboard in storyboards) {
                storyboard.Pause();
                // generate random resolution first
                // element.BeginAnimation(FrameworkElement.MarginProperty, null);
            }
        }
        public void Play() {
            paused = false;
            foreach (var storyboard in storyboards) {
                storyboard.Resume();
                // generate random resolution first
                // element.BeginAnimation(FrameworkElement.MarginProperty, null);
            }
        }
    }

}
