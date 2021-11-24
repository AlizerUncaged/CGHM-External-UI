using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mr.Krabs.UI.Scenes {
    /// <summary>
    /// Interaction logic for MessageA.xaml
    /// </summary>
    public partial class MessageA : UserControl, IDialog {
        private string link;
        private bool close;
        public MessageA(string title, string message, string process_start, bool close = false) {
            InitializeComponent();
            Title.Text = title;
            Desc.Text = message;
            link = process_start;
            this.close = close;
        }

        private void InvokeR(object sender, MouseButtonEventArgs e) {
            Process.Start(link);
            Closing?.Invoke(this, e);
        }

        public void StopAnimations() {
            foreach (var j in SkyAnimation) {
                j.Stop();
            }
        }
        private void RUnloaded(object sender, RoutedEventArgs e) {
            StopAnimations();
        }

        private List<UI.Move_Randomly> SkyAnimation = new List<UI.Move_Randomly>();

        public event EventHandler Closing;

        private void Rendered(object sender, RoutedEventArgs e) {

            var comets = SkullEmoji.Children.OfType<Path>().ToArray();
            const int MaxMovement = 50;
            // sky

            foreach (var comet in comets) {

                var maxTop = comet.Margin.Top - MaxMovement;
                var maxLeft = comet.Margin.Left - MaxMovement;
                var maxBottom = comet.Margin.Top + MaxMovement;
                var maxRight = comet.Margin.Left + MaxMovement;

                UI.Move_Randomly skyMoveEllipses =
                    new UI.Move_Randomly(
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
