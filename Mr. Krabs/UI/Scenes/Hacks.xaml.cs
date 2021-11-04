using Mr.Krabs.Stage.Communication_and_Pipes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Mr.Krabs.UI.Scenes {
    /// <summary>
    /// Interaction logic for Hacks.xaml
    /// </summary>
    public partial class Hacks : UserControl {
        private Pipe_Wrapper _pipe;
        private Read_Chewy_JSON _json;
        public Hacks(Pipe_Wrapper pipe, Read_Chewy_JSON jsonwatcher) {
            InitializeComponent();
            _pipe = pipe;
            _json = jsonwatcher;
            _json.ChangedCheckBoxes += Jsonwatcher_ChangedCheckBoxes;
        }

        private void _add_datas(Stage.Communication_and_Pipes.Hacks k, PropertyInfo[] e) {
            foreach (var h in e) {
                if (h.PropertyType == typeof(bool)) {
                    var name = h.Name;

                    var value = h.GetValue(k);
                    var sval = value.ToString().ToLower();
                    // MessageBox.Show(sval);
                    Toggle(name, bool.Parse(sval));
                }
            }
        }
        private void Jsonwatcher_ChangedCheckBoxes(object sender, (Stage.Communication_and_Pipes.Hacks k, PropertyInfo[]) e) {
            _add_datas(e.k, e.Item2);
        }

        public void AddHack(PropertyInfo field) {
            if (field.PropertyType == typeof(bool)) {
                var k = (Stage.Communication_and_Pipes.HackInfo[])field.GetCustomAttributes(typeof(Stage.Communication_and_Pipes.HackInfo), false);
                var json = (JsonPropertyAttribute[])field.GetCustomAttributes(typeof(JsonPropertyAttribute), false);

                AddHack(field, k.FirstOrDefault(), json.FirstOrDefault());
            }
        }
        public void Toggle(string name, bool toggle) {
          
            Application.Current.Dispatcher.Invoke(new Action(() => {
                foreach (var child in Hecks.Children) {
                    if (child is CheckBox cb) {

                        if (cb.Name == name)
                            cb.IsChecked = toggle;
                    }
                }
            }));
         
        }

        public void AddHack(PropertyInfo field, Stage.Communication_and_Pipes.HackInfo info, JsonPropertyAttribute jsonInfo) {
            switch (info.ControlType) {
                case Stage.Communication_and_Pipes.HackInfo.HackType.Toggle:
                    string hack_name = jsonInfo.PropertyName;
                    // create checkbox
                    var cb = new CheckBox {
                        Name = field.Name,
                        Content = info.Name,
                        Margin = new Thickness(0, 0, 0, 20),
                        Background = new SolidColorBrush(Colors.Transparent)
                    };

                    cb.Click += (s, e) => {

                        Dictionary<string, bool> nameAndVal = new Dictionary<string, bool>() { { hack_name, (bool)cb.IsChecked } };
                        string jsoned = JsonConvert.SerializeObject(nameAndVal);

                        Task.Factory.StartNew(async () => {
                            await _pipe.Send(jsoned);
                        });

                        e.Handled = true;
                    };

                    cb.Checked += (s, e) => {
                        // enable
                    };
                    cb.Unchecked += (s, e) => {
                        // enable

                    };
                    Hecks.Children.Add(cb);
                    break;
            }
        }

        private void Rendered(object sender, RoutedEventArgs e) {
            // remove placeholder lol
            Hecks.Children.Remove(PlaceHolder);
        }

        private void RUnloaded(object sender, RoutedEventArgs e) {
            _json.ChangedCheckBoxes -= Jsonwatcher_ChangedCheckBoxes;
        }
    }
}
