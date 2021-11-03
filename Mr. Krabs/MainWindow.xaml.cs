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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mr.Krabs {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
            this.WindowStyle = WindowStyle.SingleBorderWindow;
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

        private void Rendered(object sender, RoutedEventArgs e) {
            AnimateAquarium();
        }

        // animate blurred blobs
        private void AnimateAquarium() {
            var blobs = Aquarium.Children.OfType<Ellipse>();

            UI.Move_Randomly moveAnimation = 
                new UI.Move_Randomly(new UI.Resolution {MaxHeight = this.Height, MaxWidth = this.Width}, blobs.ToArray());

            moveAnimation.Start();
        }
    }
}
