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

        private Stopwatch uiRenderTime = Stopwatch.StartNew();
        public MainWindow() {
            InitializeComponent();

            this.WindowStyle =
                WindowStyle.SingleBorderWindow;

        }

        private void Clicked(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
            e.Handled = true;
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
            SStage.Pipe.Connected +=
            Pipe_Connected;

            SStage.CrabGame.StatusChanged += CrabGame_StatusChanged;
            _ = SStage.CrabGame.StartWatching();

            var update_status = await SStage.UpdateChecker.GetLink();
            if (update_status.NewVersion) {
                ShowUpdatePage(update_status.Data.Description, update_status.Data.Link);
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

            Debug.WriteLine($"Took {uiRenderTime.ElapsedMilliseconds}ms to render {this.GetType().Name}.");
            uiRenderTime.Stop();

            e.Handled = true;
        }

        private void DialogsClosing(object sender, EventArgs e) {
            Dialogs.Children.Remove(sender as UIElement);
        }

        public void ShowUpdatePage(string desc, string link) {
            Welcome.Visibility = Visibility.Collapsed;
            var update_page = new UI.Scenes.New_Update(desc, link);
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
                SStage.FieldsAndHacks = new Stage.Communication_and_Pipes.JSONWatcher(SStage.Pipe, SStage.CHEWY_JSON_PATH);
                SStage.FieldsAndHacks.OnSettingsLoaded += FieldsAndHacks_OnSettingsLoaded;
                await SStage.FieldsAndHacks.InitializeStreams();
            }

            await SStage.FieldsAndHacks.ReadAndSetHacks();

            Application.Current.Dispatcher.Invoke(new Action(() => {
                // remove skies
                Utilities.UI.RunAnimation(this, "RemoveSky");
                BlobsAnimation.Stop();
                Utilities.UI.RunAnimation(this, "AquariumHiding");

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
                    var isAlreadyCopied = Utilities.Identity.CheckIfCopiedToCrabGame(crabGameDir);
                    if (!isAlreadyCopied) Utilities.Identity.CopyToCrabGameFolder(crabGameDir);
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
                    var waiting_page = new UI.Scenes.Wait_for_Crab_Game_Page();
                    SetActiveControl(waiting_page);
                    SStage.Pipe.Stop();

                    Utilities.UI.RunAnimation(this, "RemoveSkyReverse");
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
                            new UI.Move_Randomly(
                                new UI.Resolution { MaxHeight = this.Height, MaxWidth = this.Width, MinWidth = 0, MinHeight = 0 },
                                blobs.ToArray(),
                                new UI.Interval { Min = 2000, Max = 4000 },
                                new QuinticEase { EasingMode = EasingMode.EaseInOut });

                    BlobsAnimation.Start();

                    Utilities.UI.RunAnimation(this, "AquariumHidingReverse");
                    Utilities.UI.RunAnimation(this, "UnhideCarpet");


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

        private UI.Move_Randomly BlobsAnimation;

        private List<UI.Move_Randomly> SkyAnimation = new List<UI.Move_Randomly>();
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

        private void HandleMouseDown(object sender, MouseButtonEventArgs e) {
            e.Handled = true;
        }

        private void ShowSettings(object sender, MouseButtonEventArgs e) {
            if (Dialogs.Children.Count <= 0 /* make sure theres no other dialogs */) {
                var settings = new UI.Scenes.Settings(SStage.FieldsAndHacks, SStage.Pipe, this);
                settings.Closing += DialogsClosing;

                Dialogs.Children.Add(settings);
            }
            e.Handled = true;
        }
    }
}
