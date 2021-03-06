using Newtonsoft.Json;
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
using System.Windows.Threading;

using Mr.Krabs.Utilities.UI_Extensions;
namespace Mr.Krabs.UI.Scenes {
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class AnnoyingDonateFlyover : UserControl, IDialog {
        public AnnoyingDonateFlyover() {
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

        private List<UI.MoveRandomly> SkyAnimation = new List<UI.MoveRandomly>();



        public event EventHandler Closing;
        private void CloseSettings(object sender, MouseButtonEventArgs e) {
            Closing?.Invoke(this, e);
            e.Handled = true;
        }

        private void OpenTag(object sender, MouseButtonEventArgs e) {
            if (sender is FrameworkElement control) {
                if (control.Tag != null) {
                    var link = control.Tag.ToString();
                    if (!string.IsNullOrWhiteSpace(link))
                        Process.Start(link);
                }
            }
            e.Handled = true;
        }

        // gets called when main page parent is rendered
        private void Rendered(object sender, RoutedEventArgs e) {

            border.Width = 50;
            border_Copy.Width = 50;


            var comets = SkullEmoji.Children.OfType<Ellipse>().ToArray();
            // sky
            foreach (var comet in comets) {


                UI.MoveRandomly skyMoveEllipses =
                    new UI.MoveRandomly(
                        new UI.Resolution {
                            MaxHeight = SkullEmoji.ActualHeight,
                            MaxWidth = SkullEmoji.ActualWidth,
                            MinWidth = -comet.Width,
                            MinHeight = -comet.Height
                        },
                        new FrameworkElement[] { comet },
                        new UI.Interval { Min = 2000, Max = 3000 },
                        new SineEase { EasingMode = EasingMode.EaseInOut }
                        );

                SkyAnimation.Add(skyMoveEllipses);
                skyMoveEllipses.Start();
            }

        }

        // gets called when the last image is rendered
        private void LastImageRendered(object sender, RoutedEventArgs e) {
            // dispatcher to really ensure everything has been rendered
            Dispatcher.BeginInvoke(new Action(() => {

                Storyboard sb = this.FindResource("Represent") as Storyboard;
                sb.Completed += (_pe, _nis) => {
                    this.RunAnimation("CrabTopiaShow");
                };
                Timeline.SetDesiredFrameRate(sb, 60);
                if (sb != null) { sb.Begin(); }

                // get crabtopia info
                // var crabtopiaStatus = await Utilities.CrabTopia_Widget.CrabTopia.GetServerInfo();
                // if (crabtopiaStatus == null) return;
                // MembersOnline.Text = $"{crabtopiaStatus.presence_count} Online";
                // ServerInfo.Visibility = Visibility.Visible;

            }), DispatcherPriority.ContextIdle);

            e.Handled = true;
        }
    }
}
