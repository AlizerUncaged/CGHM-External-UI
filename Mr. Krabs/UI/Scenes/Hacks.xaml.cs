using Mr.Krabs.Stage.Communication_and_Pipes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Mr.Krabs.UI.Scenes {
    /// <summary>
    /// Interaction logic for Hacks.xaml
    /// </summary>
    public partial class Hacks : UserControl {
        private Pipe_Wrapper _pipe;
        private Read_Chewy_JSON _json;

        private Dictionary<string  /* element name */, FrameworkElement> _cached_checkbox_and_names =
            new Dictionary<string, FrameworkElement>();
        public Hacks(Pipe_Wrapper pipe, Read_Chewy_JSON jsonwatcher) {
            InitializeComponent();
            _pipe = pipe;
            _json = jsonwatcher;
            _json.ChangedCheckBoxes += Jsonwatcher_ChangedCheckBoxes;
        }

        private void _add_datas(List<(string, object)> e) {

            foreach (var h in e) {
                var hackMetadata = Stage.Communication_and_Pipes.HackInfo.GetHackTypeFromName(h.Item1, h.Item2);
                var value = h.Item2;
                Toggle(hackMetadata);

            }
        }
        private void Jsonwatcher_ChangedCheckBoxes(object sender, List<(string /* name */, object /* value */)> e) {
            _add_datas(e);
        }
        public void Toggle(HackInfo.HackMetadata toggle) {
            // Debug.WriteLine($"New Toggle: {toggle.Value}");

            Application.Current.Dispatcher.Invoke(new Action(() => {
                FrameworkElement element;
                if (_cached_checkbox_and_names.TryGetValue(toggle.VariableName, out element)) {
                    switch (toggle.HackType) {
                        case HackInfo.HackType.Toggle:
                            if (element is CheckBox cb) {
                                cb.IsChecked = (bool)toggle.Value;
                            }
                            break;
                        case HackInfo.HackType.TextBox:
                            if (element is TextBox tb) {
                                string value = toggle.Value.ToString();
                                if (!string.IsNullOrWhiteSpace(tb.Text))
                                    if (!tb.Text.Equals(value))
                                        tb.Text = value;
                            }
                            break;
                    }
                }
            }));

        }

        private static readonly Regex onlyNumbers = new Regex("[^0-9.-]+");
        public void AddHack(HackInfo.HackMetadata field) {
            switch (field.HackType) {
                case HackInfo.HackType.Toggle:
                    // create checkbox
                    var cb = new CheckBox {
                        Name = field.VariableName,
                        Content = field.Name,
                        Margin = new Thickness(0, 0, 0, 20),
                        Background = new SolidColorBrush(Colors.Transparent)
                    };

                    cb.Click += (s, e) => {

                        Dictionary<object, object> nameAndVal = new Dictionary<object, object>() { { field.RawName, (bool)cb.IsChecked } };
                        string jsoned = JsonConvert.SerializeObject(nameAndVal);

                        Task.Factory.StartNew(async () => {
                            await _pipe.Send(jsoned);
                        });

                        e.Handled = true;
                    };

                    _cached_checkbox_and_names.Add(field.VariableName, cb);
                    Hecks.Children.Add(cb);
                    break;
                case HackInfo.HackType.TextBox:
                    var tb = new TextBox {
                        Name = field.VariableName,
                        // Content = field.Name,
                        Margin = new Thickness(0, 0, 0, 30),
                        Background = new SolidColorBrush(Colors.Transparent),
                        Foreground = PlaceholderName.Foreground,
                        BorderBrush = PlaceholderName.BorderBrush,
                        Text = $"{field.Value}",
                        FontFamily = PlaceholderName.FontFamily
                    };

                    tb.PreviewTextInput += (s, e) => {
                        var text = e.Text;
                        var valueType = field.Value.GetType();
                        if (valueType == typeof(double) || valueType == typeof(int) || valueType == typeof(float)) {
                            e.Handled = onlyNumbers.IsMatch(text) && !string.IsNullOrWhiteSpace(text);
                        }

                    };

                    tb.TextChanged += (s, e) => {

                        if (!tb.IsFocused) return;

                        Dictionary<object, object> defaultVal = new Dictionary<object, object>() 
                        { { field.RawName, Activator.CreateInstance(field.Value.GetType()) } };

                        string jsoned = JsonConvert.SerializeObject(defaultVal);

                        if (!string.IsNullOrWhiteSpace(tb.Text)) {
                            var originalTypeParsed = Convert.ChangeType(tb.Text, field.Value.GetType());
                            defaultVal[field.RawName] = originalTypeParsed;
                            jsoned = JsonConvert.SerializeObject(defaultVal);
                        }

                        Task.Factory.StartNew(async () => {
                            await _pipe.Send(jsoned);
                        });

                        e.Handled = true;

                    };
                    MaterialDesignThemes.Wpf.HintAssist.SetHelperText(tb, field.Name);
                    MaterialDesignThemes.Wpf.HintAssist.SetBackground(tb, Brushes.Transparent);
                    MaterialDesignThemes.Wpf.HintAssist.SetFontFamily(tb, PlaceholderName.FontFamily);
                    _cached_checkbox_and_names.Add(field.VariableName, tb);
                    Hecks.Children.Add(tb);

                    break;
            }
        }

        private void Rendered(object sender, RoutedEventArgs e) {
            // remove placeholders lol
            const string placeholderTag = "Placeholder";
            for (int i = Hecks.Children.Count - 1; i >= 0; i--) {
                var control = Hecks.Children[i] as FrameworkElement;
                if (control.Tag != null) {
                    string tagName = control.Tag.ToString();
                    if (tagName == placeholderTag)
                        Hecks.Children.Remove(control);
                }
            }

            Scrolly.MaxHeight = this.ActualHeight;
        }

        private void RUnloaded(object sender, RoutedEventArgs e) {
            _json.ChangedCheckBoxes -= Jsonwatcher_ChangedCheckBoxes;
        }
    }
}
