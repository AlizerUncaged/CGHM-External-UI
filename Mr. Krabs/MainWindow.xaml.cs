using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Mr.Krabs.Utilities.UI_Extensions;

namespace Mr.Krabs {
    /// <summary>
    /// UI
    /// </summary>
    public partial class MainWindow : Window {

        //////////////// stage
        /* stage */
        public Stage.Stage SStage;
        //////////////// stage

        private Stopwatch uiRenderTime = Stopwatch.StartNew();
        public MainWindow() {
            InitializeComponent();
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        // mousedown
        private void Clicked(object sender, MouseButtonEventArgs e) {
            e.Handled = true;
        }

        private const double confidence = 10;
        private Point oldCursorPosition;
        private void PreviewMove(object sender, MouseEventArgs e) {

            if (e.LeftButton == MouseButtonState.Pressed) {
                var point = e.GetPosition(this);
                if (oldCursorPosition.X == 0 && oldCursorPosition.Y == 0) {
                    oldCursorPosition = point;
                }

                double xChange = Math.Abs(point.X - oldCursorPosition.X),
                yChange = Math.Abs(point.Y - oldCursorPosition.Y);

                if (xChange > confidence || yChange > confidence) {
                    UI.MoveRandomly.PauseAnimations();
                    Debug.WriteLine("Animations paused.");

                    this.DragMove();

                    UI.MoveRandomly.StartAnimations();
                    Debug.WriteLine("Animations restarted.");
                }

            } else if (e.LeftButton == MouseButtonState.Released) {
                oldCursorPosition = new Point(0, 0);
            }

        }

        #region Window State and Windows Functions
        private void CloseButtonMouseDown(object sender, MouseButtonEventArgs e) {
            Utilities.OSFunctions.Exit();
            e.Handled = true;
        }

        private void MinimizeButtonMouseDown(object sender, MouseButtonEventArgs e) {
            this.WindowState = WindowState.Minimized;
            e.Handled = true;
        }
        #endregion

        #region Animations 'n Shit
        private async void Rendered(object sender, RoutedEventArgs e) {
            this.MakeTransparent();
            Debug.WriteLine($"Took {uiRenderTime.ElapsedMilliseconds}ms to render {this.GetType().Name}.");
            uiRenderTime.Stop();

            AnimateAquarium();
            Version.Text =
                $"v{Utilities.Identity.MajorVersion}.{Utilities.Identity.MinorVersion} " +
                $"{(Utilities.Identity.AmIAdmin() ? "(Admin)" : "")}";
#if (DEBUG)
            Version.Text += " BETA";
#endif
            // start watching
            SStage = new Stage.Stage();
            // start the pipes
            SStage.Pipe.Connected += Pipe_Connected;

            SStage.CrabGame.StatusChanged += CrabGame_StatusChanged;
            _ = SStage.CrabGame.StartWatching();

            var updateStatus = await SStage.UpdateChecker.GetLink();
            if (updateStatus.NewVersion) {
                ShowUpdatePage(updateStatus.Data.Description, updateStatus.Data.Link);
            }

            var required_dll = await Utilities.Identity.CheckRequiredDlls();
            if (!required_dll.AllFound) {
                var message =
                    new UI.Scenes.MessageA(
                        $"{System.IO.Path.GetFileName(required_dll.Name)} not found!",
                        required_dll.Desc,
                        required_dll.Link,
                        true);
                message.Closing += DialogsClosing;

                if (Dialogs.Children.Count <= 0 /* make sure theres no other dialogs */) {
                    Dialogs.Children.Add(message);
                }
            }

            e.Handled = true;
        }

        private void DialogsClosing(object sender, EventArgs e) {
            Dialogs.Children.Remove(sender as UIElement);
        }

        public void ShowUpdatePage(string desc, string link) {
            Welcome.Visibility = Visibility.Collapsed;
            var update_page = new UI.Scenes.NewUpdate(desc, link);
            DokcuPanelu.Children.Add(update_page);

        }
        /////////////// DLL INJECTED!
        private async void Pipe_Connected(object sender, EventArgs e) {
            // the dll is injected!
            foreach (var j in SkyAnimation) {
                j.Stop();
            }

            // dll is there, so that means json file is written right? yes.
            if (SStage.FieldsAndHacks == null) {
                SStage.FieldsAndHacks = new Stage.Communication_and_Pipes.JSONWatcher(SStage.Pipe, SStage.chewyJsonPath);
                SStage.FieldsAndHacks.OnSettingsLoaded += FieldsAndHacks_OnSettingsLoaded;
                await SStage.FieldsAndHacks.InitializeStreams();
            }

            await SStage.FieldsAndHacks.ReadAndSetHacks();

            Application.Current.Dispatcher.Invoke(new Action(() => {
                // remove skies
                this.RunAnimation("RemoveSky");
                BlobsAnimation.Stop();
                this.RunAnimation("AquariumHiding");

                // hacks ! Window !
                var hacks = new UI.Scenes.Hacks(SStage.Pipe, SStage.FieldsAndHacks);
                // add those controls
                var toggles = SStage.FieldsAndHacks.GetFields();
                // MessageBox.Show($"Toggles: {toggles.Count()}");
                foreach (var toggle in toggles) {
                    hacks.AddHack(toggle);
                }

                SStage.FieldsAndHacks.StartWatchers();

                SetActiveControl(hacks);
            }));
        }

        private void FieldsAndHacks_OnSettingsLoaded(object sender, (string, object) e) {
            switch (e.Item1) {
                case "run_external_on_start.active":
                    var crabGameDir = SStage.CrabGame.GetCrabGameDirectory();
                    Utilities.Identity.CopyToLocalAppData();
                    break;
            }
        }

        public void SetActiveControl(UserControl control) {
            Welcome.Children.RemoveAt(0);
            Welcome.Children.Add(control);
        }

        private void CrabGame_StatusChanged(object sender, Stage.Process_Watcher.CrabGameStatus e) {
            Debug.WriteLine($"Process Status: {e}");
            Application.Current.Dispatcher.Invoke(new Action(() => {
                /* Not running! */
                if (e == Krabs.Stage.Process_Watcher.CrabGameStatus.Offline) {
                    var waiting_page = new UI.Scenes.WaitForCrabGamePage();
                    SetActiveControl(waiting_page);
                    SStage.Pipe.Stop();

                    this.RunAnimation("RemoveSkyReverse");
                    foreach (var j in SkyAnimation) {
                        j.Start();
                    }
                }
                /* Game Ran */
                // this is also while injecting
                else if (e == Krabs.Stage.Process_Watcher.CrabGameStatus.FoundRunning) {
                    // generate injecting control
                    var injecting_page = new UI.Scenes.Injecting();
                    SetActiveControl(injecting_page);

                    // blurred blobs
                    var blobs = Aquarium.Children.OfType<Ellipse>().ToArray();

                    if (BlobsAnimation == null)
                        BlobsAnimation =
                            new UI.MoveRandomly(
                                new UI.Resolution { MaxHeight = this.Height, MaxWidth = this.Width, MinWidth = 0, MinHeight = 0 },
                                blobs.ToArray(),
                                new UI.Interval { Min = 2000, Max = 4000 },
                                new QuinticEase { EasingMode = EasingMode.EaseInOut });

                    BlobsAnimation.Start();

                    this.RunAnimation("AquariumHidingReverse");
                    this.RunAnimation("UnhideCarpet");


                } else if (e == Stage.Process_Watcher.CrabGameStatus.IsAdmin) {
                    // check if we're admin, if not restart as admin
                    if (!Utilities.Identity.AmIAdmin()) {
                        if (Welcome.Children.Count > 0)
                            Welcome.Children.RemoveAt(0);

                        Dialogs.Children.Add(new UI.Scenes.Admin());
                    }
                } else if (e == Stage.Process_Watcher.CrabGameStatus.DllNotFound) {

                } else if (e == Stage.Process_Watcher.CrabGameStatus.DllFound) {
                    SStage.Pipe.Start();
                }
            }));
        }

        private UI.MoveRandomly BlobsAnimation;

        private List<UI.MoveRandomly> SkyAnimation = new List<UI.MoveRandomly>();
        // animate blurred blobs
        private void AnimateAquarium() {
            const int MaxMovement = 10;
            // sky
            var comets = Space.Children.OfType<FrameworkElement>().ToArray();

            foreach (var comet in comets) {

                var maxTop = comet.Margin.Top - MaxMovement;
                var maxLeft = comet.Margin.Left - MaxMovement;
                var maxBottom = comet.Margin.Top + MaxMovement;
                var maxRight = comet.Margin.Left + MaxMovement;

                UI.MoveRandomly skyMoveEllipses =
                    new UI.MoveRandomly(
                        new UI.Resolution { MaxHeight = maxBottom, MaxWidth = maxRight, MinWidth = maxLeft, MinHeight = maxTop },
                        new FrameworkElement[] { comet },
                        new UI.Interval { Min = 1000, Max = 2500 },
                        new SineEase { EasingMode = EasingMode.EaseInOut }
                        );

                SkyAnimation.Add(skyMoveEllipses);
                skyMoveEllipses.Start();
            }

        }
        #endregion

        private void HandleMouseDown(object sender, MouseButtonEventArgs e) {
            e.Handled = true;
        }
        public bool ShowAsDialog(UI.Scenes.IDialog dialog) {
            if (Dialogs.Children.Count <= 0 /* make sure theres no other dialogs */) {
                var userControl = dialog as UserControl;
                // var settings = new UI.Scenes.Settings(SStage.FieldsAndHacks, SStage.Pipe, this);
                dialog.Closing += DialogsClosing;

                Dialogs.Children.Add(userControl);
                return true;
            }
            return false;
        }
        private void ShowSettings(object sender, MouseButtonEventArgs e) {
            var settings = new UI.Scenes.Settings(SStage.FieldsAndHacks, SStage.Pipe, this);
            ShowAsDialog(settings);
            e.Handled = true;
        }

        private void OpenInformation(object sender, MouseButtonEventArgs e) {
            var infoPage = new UI.Scenes.AnnoyingDonateFlyover();
            ShowAsDialog(infoPage);
            e.Handled = true;
        }
    }
}
