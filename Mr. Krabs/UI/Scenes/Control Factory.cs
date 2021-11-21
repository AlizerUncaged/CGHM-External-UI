using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mr.Krabs.UI.Scenes {
    // 
    public static class ControlFactory {

        private static FontFamily materialDesignsFont = 
            new FontFamily(new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Resources/Roboto/"), "./#Roboto");

        public static CheckBox CreateGenericCheckBoxClassA(
            string variableName,
            string content) {

            return new CheckBox {
                Name = variableName,
                Content = content,
                Margin = new Thickness(0, 0, 0, 20),
                Background = new SolidColorBrush(Colors.Transparent),
                Foreground = Brushes.White,
                FontFamily = materialDesignsFont,
            };
        }
    }
}
