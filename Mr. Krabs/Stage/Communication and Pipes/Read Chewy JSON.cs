using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
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

        private string _filepath;
        private Hacks _hacks;
        public Read_Chewy_JSON(string filepath) {
            _filepath = filepath;
        }

        public async Task<ChewyStatus> Read() {
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

    }
}
