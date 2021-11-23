using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mr.Krabs.Stage.Communication_and_Pipes {

    public class JSONWatcher {
        private const int _json_refresh_rate = 200; // milliseconds
        private string _filepath = "";
        private Dictionary<string, object> _hacks;
        private FileStream _read_fileStream;
        private StreamReader _read_stream;
        private Pipe_Wrapper _comms;
        public JSONWatcher(Pipe_Wrapper comms, string filepath) {
            _filepath = filepath;
            _comms = comms;
        }


        public async Task InitializeStreams() {
            for (int i = 0; !File.Exists(_filepath); i++) {
                string initiator = $"{{\"mod_fly.active\": {(i % 2 == 0 ? "true" : "false")}}}";
                _ = _comms.Send(initiator);
                await Task.Delay(200);
            }

            _read_fileStream = File.Open(_filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _read_stream = new StreamReader(_read_fileStream);

        }

        // penis? cock? maybe even cum sometimes
        public async Task ReadAndSetHacks() {
            await Task.Run(async () => {
                string filestring = null;
                while (filestring == null) {
                    filestring = _read_stream.ReadToEnd();
                    _read_fileStream.Position = 0;
                    _read_stream.DiscardBufferedData();
                    if (filestring == null)
                        await Task.Delay(200);
                }

                var hacks = JsonConvert.DeserializeObject<Dictionary<string, object>>(filestring);

                foreach (var settingsOption in UI.Scenes.Settings.settingsFields) {
                    if (hacks.ContainsKey(settingsOption)) {

                        OnSettingsLoaded?.Invoke(this, (settingsOption, hacks[settingsOption]));

                        /* bool isRemoved = */ hacks.Remove(settingsOption);
                    }
                }

                _hacks = hacks;
            });
        }


        public event EventHandler<(string, object)> OnSettingsLoaded;

        // booleans
        public IEnumerable<HackInfo.HackMetadata> GetFields() {
            List<HackInfo.HackMetadata> metadatas = new List<HackInfo.HackMetadata>();
            foreach (var i in _hacks) {
                metadatas.Add(HackInfo.GetHackTypeFromName(i.Key, i.Value));
            }
            return metadatas;
        }

        public Dictionary<string, object> Hacks;
        private bool _keep_reading = false;
        public void StartWatchers() {
            Hacks = new Dictionary<string, object>();
            if (_keep_reading) return;

            _keep_reading = true;
            Task.Factory.StartNew(async () => {
                while (_keep_reading) {
                    await Task.Delay(_json_refresh_rate);

                    string read = _read_stream.ReadToEnd();

                    _read_fileStream.Position = 0;
                    _read_stream.DiscardBufferedData();

                    Dictionary<string, object> newHacks = JsonConvert.DeserializeObject<Dictionary<string, object>>(read);

                    if (newHacks == null) 
                        continue;
                    

                    List<(string, object)> newProperties = new List<(string, object)>();

                    for (int i = 0; i < newHacks.Count; i++) {
                        var newHacksElementAtIndex = newHacks.ElementAt(i);
                        if (i < Hacks.Count)
                            if (newHacksElementAtIndex.Value.Equals(Hacks.ElementAt(i).Value)) {
                                newProperties.Add((newHacksElementAtIndex.Key, newHacksElementAtIndex.Value));
                            }
                    }

                    if (newProperties.Count > 0)
                        ChangedCheckBoxes?.Invoke(this, newProperties);


                    Hacks = newHacks;

                }
            });
        }
        public void StopWatchers() {
            _keep_reading = false;
        }

        // contains new values
        public event EventHandler<List<(string, object)>> ChangedCheckBoxes;

    }
}
