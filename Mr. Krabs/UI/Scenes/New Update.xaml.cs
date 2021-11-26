using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Mr.Krabs.UI.Scenes {
    /// <summary>
    /// Interaction logic for New_Update.xaml
    /// </summary>
    public partial class NewUpdate : UserControl {
        private string link, description;
        public NewUpdate(string description, string link) {
            this.link = link; this.description = description;
            InitializeComponent();
            Description.Text = this.description;
        }

        public void StopAnimations() {
            foreach (var j in SkyAnimation) {
                j.Stop();
            }
        }
        private void RUnloaded(object sender, RoutedEventArgs e) {
            StopAnimations();
        }

        private List<UI.MoveRandomly> SkyAnimation = new List<UI.MoveRandomly>();

        private void Update(object sender, MouseButtonEventArgs e) {
            Process.Start(link);
            e.Handled = true;
        }

        private void Rendered(object sender, RoutedEventArgs e) {

            var comets = SkullEmoji.Children.OfType<Path>().ToArray();
            const int MaxMovement = 50;
            // sky

            foreach (var comet in comets) {

                var maxTop = comet.Margin.Top - MaxMovement;
                var maxLeft = comet.Margin.Left - MaxMovement;
                var maxBottom = comet.Margin.Top + MaxMovement;
                var maxRight = comet.Margin.Left + MaxMovement;

                UI.MoveRandomly skyMoveEllipses =
                    new UI.MoveRandomly(
                        new UI.Resolution { MaxHeight = maxBottom, MaxWidth = maxRight, MinWidth = maxLeft, MinHeight = maxTop },
                        new FrameworkElement[] { comet },
                        new UI.Interval { Min = 1000, Max = 1500 },
                        new SineEase { EasingMode = EasingMode.EaseInOut }
                        );

                SkyAnimation.Add(skyMoveEllipses);
                skyMoveEllipses.Start();
            }
            e.Handled = true;
        }
    }
}
