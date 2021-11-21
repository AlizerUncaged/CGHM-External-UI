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

        private static Thickness defaultThickness = new Thickness(0, 0, 0, 20);
        public static CheckBox CreateGenericCheckBoxClassA(
            string variableName,
            string content) {

            return new CheckBox {
                Name = variableName,
                Content = content,
                Margin = defaultThickness,
                Background = new SolidColorBrush(Colors.Transparent),
                Foreground = Brushes.White,
                FontFamily = materialDesignsFont,
            };
        }  
        public static TextBox CreateGenericTextBoxClassA(
            string variableName,
            string title, Brush borderBrush) {

            return new TextBox {

                Name = variableName,
                // Content = field.Name,
                Margin = new Thickness(0, 0, 0, 30),
                Background = new SolidColorBrush(Colors.Transparent),
                Foreground = Brushes.White,
                BorderBrush = borderBrush,
                Text = title,
                FontFamily = materialDesignsFont
            };
        }
    }
}
