using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mr.Krabs.Stage.Communication_and_Pipes {
    public class HackInfo {
        public enum HackType {
            Toggle, TextBox, BoolButton, SettingsToggle
        }

        public struct HackMetadata {
            public string VariableName;
            public string Name;
            public HackType HackType;
        }
        private static readonly Regex rgx = new Regex("[^a-zA-Z0-9 -]");
        public static HackMetadata GetHackTypeFromName(string name) {
            HackMetadata metadata = new HackMetadata();

            metadata.VariableName = rgx.Replace(name, string.Empty);
            // parse name
            string metadataName = name.Replace("mod_", string.Empty);

            const char
                variableNameSeparator = '.',
                metadataNameSeparator = '_';

            if (metadataName.Contains(variableNameSeparator)) metadataName = metadataName.Split(variableNameSeparator)[0];
            var words = metadataName.Split(metadataNameSeparator);
            var capitalized = words.Select(x => char.ToUpper(x[0]));

            metadata.Name = string.Join(" ", capitalized);
            if (name.Contains(".active")) metadata.HackType = HackType.Toggle;

            return metadata;
        }
    }
}
