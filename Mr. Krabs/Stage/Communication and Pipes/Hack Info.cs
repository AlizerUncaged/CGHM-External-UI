using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mr.Krabs.Stage.Communication_and_Pipes {
    public class HackInfo {
        public enum HackType {
            Unknown,
            Toggle, TextBox, BoolButton, SettingsToggle
        }

        public struct HackMetadata {
            public string RawName;
            public string VariableName;
            public string Name;
            public object Value;
            public HackType HackType;
        }
        private static readonly Regex rgx = new Regex("[^a-zA-Z0-9 -]");
        private static readonly
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        private static readonly string[] removeEnds = new string[] { "active" };
        private static readonly string[] removeStarts = new string[] { "mod" };
        public static HackMetadata GetHackTypeFromName(string name, object value) {

            HackMetadata metadata = new HackMetadata();
            metadata.RawName = name;
            metadata.Value = value;
            metadata.VariableName = rgx.Replace(name, string.Empty);
            // parse name
            string metadataName = name.Replace("mod_", string.Empty);
            metadataName = rgx.Replace(name, " ");

            foreach (string i in removeStarts) {
                if (metadataName.StartsWith(i)) {
                    metadataName = metadataName.Substring(i.Length);
                }
            }
            foreach (string i in removeEnds) {
                if (metadataName.EndsWith(i)) {
                    metadataName = metadataName.Substring(0, metadataName.Length - i.Length);
                }
            }
            metadataName = metadataName.Trim();
            const char
                variableNameSeparator = '.',
                metadataNameSeparator = '_';

            // if (metadataName.Contains(variableNameSeparator)) metadataName = metadataName.Split(variableNameSeparator)[0];

            var words = metadataName
                .Split(metadataNameSeparator);

            var capitalized = words.Select(x => textInfo.ToTitleCase(x));

            metadata.Name = string.Join(" ", capitalized);

            if (name.Contains(".active")
                && value.GetType() == typeof(bool)) metadata.HackType = HackType.Toggle;
            if (name.Contains(".value")) metadata.HackType = HackType.TextBox;

            return metadata;
        }
    }
}
