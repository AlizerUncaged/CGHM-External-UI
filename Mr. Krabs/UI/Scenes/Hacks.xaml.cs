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
        public Hacks(Pipe_Wrapper pipe) {
            InitializeComponent();
            _pipe = pipe;
        }

        public void AddHack(PropertyInfo field) {
            if (field.PropertyType == typeof(bool)) {
                var k = (Stage.Communication_and_Pipes.HackInfo[])field.GetCustomAttributes(typeof(Stage.Communication_and_Pipes.HackInfo), false);
                var json = (JsonPropertyAttribute[])field.GetCustomAttributes(typeof(JsonPropertyAttribute), false);

                AddHack(k.FirstOrDefault(), json.FirstOrDefault());
            }
        }
        public void AddHack(Stage.Communication_and_Pipes.HackInfo info, JsonPropertyAttribute jsonInfo) {
            switch (info.ControlType) {
                case Stage.Communication_and_Pipes.HackInfo.HackType.Toggle:
                    // create checkbox
                    var cb = new CheckBox {
                        Content = info.Name,
                        Margin = new Thickness(0, 0, 0, 20),
                        Background = new SolidColorBrush(Colors.Transparent)
                    };

                    string hack_name = jsonInfo.PropertyName;
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
    }
}
