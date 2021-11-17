using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Stage.Communication_and_Pipes {
    public class HackInfo {
        public enum HackType {
            Toggle, TextBox, BoolButton, SettingsToggle
        }

        public struct HackMetadata {
            public string Name;
            public HackType HackType;
        }
        public static HackMetadata GetHackTypeFromName(string name) {
            HackMetadata metadata = new HackMetadata();
            // parse name
            string metadataName = name.Replace("mod_", string.Empty);
            var words = metadataName.Split('_');
            var capitalized = words.Select(x => char.ToUpper(x[0]));

            metadata.Name = string.Join(" ", capitalized);
            if (name.Contains(".active")) metadata.HackType = HackType.Toggle;

            return metadata;
        }
    }
}
