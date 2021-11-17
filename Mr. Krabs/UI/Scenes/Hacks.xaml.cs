using Mr.Krabs.Stage.Communication_and_Pipes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
        private Dictionary<string, CheckBox> _cached_checkbox_and_names = new Dictionary<string, CheckBox>();
        public Hacks(Pipe_Wrapper pipe, Read_Chewy_JSON jsonwatcher) {
            InitializeComponent();
            _pipe = pipe;
            _json = jsonwatcher;
            _json.ChangedCheckBoxes += Jsonwatcher_ChangedCheckBoxes;
        }

        private void _add_datas(List<(string, object)> e) {


            foreach (var h in e) {

                var hackMetadata = Stage.Communication_and_Pipes.HackInfo.GetHackTypeFromName(h.Item1);
                var value = h.Item2;
                if (value.GetType() == typeof(bool)) {
                    var name = hackMetadata.VariableName;
                    var sval = value.ToString().ToLower();
                    Toggle(name, bool.Parse(sval));
                }
            }
        }
        private void Jsonwatcher_ChangedCheckBoxes(object sender, List<(string, object)> e) {
            _add_datas(e);
        }
        public void Toggle(string name, bool toggle) {
          
            Application.Current.Dispatcher.Invoke(new Action(() => {
                CheckBox cb;
                if (_cached_checkbox_and_names.TryGetValue(name, out cb)) {
                    cb.IsChecked = toggle;
                }
            }));
         
        }

        public void AddHack(HackInfo.HackMetadata field) {
            switch (field.HackType) {
                case Stage.Communication_and_Pipes.HackInfo.HackType.Toggle:
                    // create checkbox
                    var cb = new CheckBox {
                        Name = field.VariableName,
                        Content = field.Name,
                        Margin = new Thickness(0, 0, 0, 20),
                        Background = new SolidColorBrush(Colors.Transparent)
                    };

                    cb.Click += (s, e) => {

                        Dictionary<string, bool> nameAndVal = new Dictionary<string, bool>() { { field.VariableName, (bool)cb.IsChecked } };
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
                    _cached_checkbox_and_names.Add(field.VariableName, cb);
                    Hecks.Children.Add(cb);
                    break;
            }
        }

        private void Rendered(object sender, RoutedEventArgs e) {
            // remove placeholder lol
            Hecks.Children.Remove(PlaceHolder);
            Scrolly.MaxHeight = this.ActualHeight;
        }

        private void RUnloaded(object sender, RoutedEventArgs e) {
            _json.ChangedCheckBoxes -= Jsonwatcher_ChangedCheckBoxes;
        }
    }
}
