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

namespace Mr.Krabs {
    /// <summary>
    /// UI
    /// </summary>
    public partial class MainWindow : Window {

        //////////////// stage
        /* stage */
        public Stage.Stage SStage;
        //////////////// stage

        public MainWindow() {
            InitializeComponent();

            this.WindowStyle =
                WindowStyle.SingleBorderWindow;

        }

        private void Clicked(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
            e.Handled = true;
        }

        #region Window State and Windows Functions
        private void CloseButtonMouseDown(object sender, MouseButtonEventArgs e) {
            Windows.Exit();
        }

        private void MinimizeButtonMouseDown(object sender, MouseButtonEventArgs e) {
            this.WindowState = WindowState.Minimized;
        }
        #endregion

        #region Animations 'n Shit
        private void Rendered(object sender, RoutedEventArgs e) {

            AnimateAquarium();

            // start watching
            SStage = new Stage.Stage();

            SStage.CrabGame.StatusChanged += CrabGame_StatusChanged;
            _ = SStage.CrabGame.StartWatching();

        }

        /////////////// DLL INJECTED!
        private async void Pipe_Connected(object sender, EventArgs e) {
            // the dll is injected!
            foreach (var j in SkyAnimation) {
                j.Stop();
            }
            // remove skies
            Static_Utilities.RunAnimation(this, "RemoveSky");
            // dll is there, so that means json file is written right?
            await SStage.FieldsAndHacks.Read();
            var hacks = new UI.Scenes.Hacks(SStage.Pipe, SStage.FieldsAndHacks);
            SStage.FieldsAndHacks.StartWatchers();

            // add those controls
            var toggles = SStage.FieldsAndHacks.GetCheckBoxes();
            foreach (var toggle in toggles) {
                hacks.AddHack(toggle);
            }
            SetActiveControl(hacks);
        }

        public void SetActiveControl(UserControl control) {
            Welcome.Children.RemoveAt(0);
            Welcome.Children.Add(control);
        }

        private void CrabGame_StatusChanged(object sender, Stage.Process_Watcher.CrabGameStatus e) {
            Debug.WriteLine($"Process Status: {e}");

            /* Not running! */
            if (e == Krabs.Stage.Process_Watcher.CrabGameStatus.Offline) {
                SStage.Pipe.Stop();
                Static_Utilities.RunAnimation(this, "RemoveSkyReverse");
            }
            /* Game Ran */
            else if (e == Krabs.Stage.Process_Watcher.CrabGameStatus.FoundRunning) {
                // generate injecting control
                var injecting_page = new UI.Scenes.Injecting();
                SetActiveControl(injecting_page);

                // blurred blobs
                var blobs = Aquarium.Children.OfType<Ellipse>();

                BlobsAnimation =
                    new UI.Move_Randomly(
                        new UI.Resolution { MaxHeight = this.Height, MaxWidth = this.Width, MinWidth = 0, MinHeight = 0 },
                        blobs.ToArray(),
                        new UI.Interval { Min = 2000, Max = 4000 },
                        new QuinticEase { EasingMode = EasingMode.EaseInOut });

                BlobsAnimation.Start();

                Static_Utilities.RunAnimation(this, "UnhideCarpet");

                // start the pipes
                SStage.Pipe.Connected += Pipe_Connected;
              SStage.Pipe.Start();

            } else if (e == Stage.Process_Watcher.CrabGameStatus.Injecting) {
                // the dll is not there yet
                // maybe DO NOTHING
                // Watcher is already injecting it
            } else if (e == Krabs.Stage.Process_Watcher.CrabGameStatus.Injected) {
                // Injected!
            }
        }

        private UI.Move_Randomly BlobsAnimation;

        private List<UI.Move_Randomly> SkyAnimation = new List<UI.Move_Randomly>();
        // animate blurred blobs
        private void AnimateAquarium() {

            const int MaxMovement = 10;
            // sky
            var comets = Space.Children.OfType<FrameworkElement>();
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

        }
        #endregion

    }
}
