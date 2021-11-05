using System;
using System.Collections.Generic;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl, IDialog {
        private Window _parent;
        public Settings(Window parent) {
            _parent = parent;
            InitializeComponent();
        }
        /// <summary>
        /// Placeholder.
        /// </summary>
        public Settings() {
            InitializeComponent();
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

        private void Rendered(object sender, RoutedEventArgs e) {

            var comets = SkullEmoji.Children.OfType<Ellipse>().ToArray();
            // sky
            foreach (var comet in comets) {


                UI.Move_Randomly skyMoveEllipses =
                    new UI.Move_Randomly(
                        new UI.Resolution { 
                            MaxHeight = SkullEmoji.ActualHeight, 
                            MaxWidth = SkullEmoji.ActualWidth, 
                            MinWidth = -comet.Width, 
                            MinHeight = -comet.Height},
                        new FrameworkElement[] { comet },
                        new UI.Interval { Min = 2000, Max = 3000 },
                        new SineEase { EasingMode = EasingMode.EaseInOut }
                        );

                SkyAnimation.Add(skyMoveEllipses);
                skyMoveEllipses.Start();
            }

            Static_Utilities.RunAnimation(this, "Represent");
            if (_parent != null) {
                AlwaysOnTopCB.IsChecked = _parent.Topmost;
            }

            e.Handled = true;
        }

        private void AlwaysOnTopChanged(object sender, RoutedEventArgs e) {
            if (_parent != null) _parent.Topmost = (bool)(sender as CheckBox).IsChecked;
            e.Handled = true;
        }

        public event EventHandler Closing;
        private void CloseSettings(object sender, MouseButtonEventArgs e) {
            Closing?.Invoke(this, e);
            e.Handled = true;
        }
    }
}
