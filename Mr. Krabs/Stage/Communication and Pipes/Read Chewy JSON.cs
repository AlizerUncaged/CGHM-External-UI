using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mr.Krabs.Stage.Communication_and_Pipes {

    public struct ChewyStatus {

        public Hacks Hacks;

        public bool Success;
    }

    public class Read_Chewy_JSON {
        private const int _json_refresh_rate = 200; // milliseconds
        private string _filepath = "";
        private Hacks _hacks;
        private FileStream _read_fileStream;
        private StreamReader _read_stream;
        private Pipe_Wrapper _comms;
        public Read_Chewy_JSON(Pipe_Wrapper comms, string filepath) {
            _filepath = filepath;
            _comms = comms;
        }

        public async Task InitializeStreams() {

            while (!File.Exists(_filepath)) {
                _ = _comms.Send("esp_box.active.false");

                await Task.Delay(200);
            }

            _read_fileStream = File.Open(_filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _read_stream = new StreamReader(_read_fileStream);

        }

        public async Task<ChewyStatus> ReadAndSetHacks() {
            return await Task.Run(() => {
                try {

                    var filestring = File.ReadAllText(_filepath);
                    var hacks = JsonConvert.DeserializeObject<Hacks>(filestring);
                    _hacks = hacks;

                    return new ChewyStatus {
                        Hacks = hacks,
                        Success = true
                    };
                } catch {

                    return new ChewyStatus {
                        Hacks = null,
                        Success = false
                    };
                }
            });
        }

        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext) {
            errorContext.Handled = true;
        }

        private readonly BindingFlags bindingFlags = BindingFlags.Public |
                        BindingFlags.NonPublic |
                        BindingFlags.Instance |
                        BindingFlags.Static;
        // booleans
        public PropertyInfo[] GetCheckBoxes() {
            var properties = _hacks
                .GetType()
                .GetProperties(bindingFlags)
                .Where(x => x.PropertyType == typeof(bool))
                .ToArray();
            return properties;
        }
        private Hacks _old_hacks;
        private bool _keep_reading = false;
        public void StartWatchers() {
            _old_hacks = new Hacks();
            if (_keep_reading) return;

            _keep_reading = true;
            Task.Factory.StartNew(async () => {
                while (_keep_reading) {
                    await Task.Delay(_json_refresh_rate);

                    string read = _read_stream.ReadToEnd();

                    _read_fileStream.Position = 0;
                    _read_stream.DiscardBufferedData();

                    var _read_hacks =
                    JsonConvert.DeserializeObject<Hacks>(read);

                    if (_read_hacks == null) {
                        /*
                        try {
                            _read_fileStream.Close();
                        } catch { Debug.WriteLine("_read_fileStream dispose fail."); }
                        _read_fileStream = File.Open(_filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        try {
                            _read_stream.Close();
                        } catch { Debug.WriteLine("_read_stream dispose fail."); }
                        _read_stream = new StreamReader(_read_fileStream);
                        */
                        continue;
                    }

                    List<PropertyInfo> newProperties = new List<PropertyInfo>();

                    foreach (var property in _read_hacks.GetType().GetProperties()) {
                        var oldValue = property.GetValue(_old_hacks, null);
                        var newValue = property.GetValue(_read_hacks, null);

                        if (!object.Equals(oldValue, newValue)) {
                            newProperties.Add(property);
                        }

                    }

                    if (newProperties.Count > 0) {
                        ChangedCheckBoxes?.Invoke(this, (_read_hacks, newProperties.ToArray()));
                    }

                    _old_hacks = _read_hacks;

                }
            });
        }
        public void StopWatchers() {
            _keep_reading = false;
        }

        // contains new values
        public event EventHandler<(Hacks k, PropertyInfo[])> ChangedCheckBoxes;

    }
}
