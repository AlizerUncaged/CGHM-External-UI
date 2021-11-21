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

namespace Mr.Krabs.UI.Scenes {
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl, IDialog {

        public static string[] settingsFields = new string[] {
            "run_external_on_start.active"
        };

        private Stage.Communication_and_Pipes.Pipe_Wrapper _pipe;
        private Stage.Communication_and_Pipes.JSONWatcher _json;
        private Window _parent;
        public Settings(Stage.Communication_and_Pipes.JSONWatcher json, Stage.Communication_and_Pipes.Pipe_Wrapper pipe, Window parent) {
            _json = json;
            _pipe = pipe;
            _parent = parent;
            InitializeComponent();
        }

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
                            MinHeight = -comet.Height
                        },
                        new FrameworkElement[] { comet },
                        new UI.Interval { Min = 2000, Max = 3000 },
                        new SineEase { EasingMode = EasingMode.EaseInOut }
                        );

                SkyAnimation.Add(skyMoveEllipses);
                skyMoveEllipses.Start();
            }

            Utilities.UI.RunAnimation(this, "Represent");
            if (_parent != null) {
                AlwaysOnTopCB.IsChecked = _parent.Topmost;
            }

            if (_json != null && _json.Hacks != null)
                foreach (var i in settingsFields) {

                    if (!_json.Hacks.ContainsKey(i)) continue;

                    var parsedHackInfo = Stage.Communication_and_Pipes.HackInfo.GetHackTypeFromName(i, _json.Hacks[i]);

                    var cb = ControlFactory.CreateGenericCheckBoxClassA(parsedHackInfo.VariableName, parsedHackInfo.Name);

                    cb.Loaded += (s, _e) => {
                        cb.IsChecked = (bool)_json.Hacks[i];
                    };

                    cb.Click += (s, _e) => {

                        Dictionary<object, object> nameAndVal = new Dictionary<object, object>() { { parsedHackInfo.RawName, (bool)cb.IsChecked } };
                        string jsoned = JsonConvert.SerializeObject(nameAndVal);

                        Task.Factory.StartNew(async () => {
                            await _pipe.Send(jsoned);
                        });

                        e.Handled = true;
                    };

                    Parent.Children.Add(cb);
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

        private void DiscordJoin(object sender, MouseButtonEventArgs e) {
            Process.Start("https://discord.gg/n6AW3q4ewQ");
            e.Handled = true;
        }
    }
}
