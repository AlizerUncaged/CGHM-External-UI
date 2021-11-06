using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Stage.Communication_and_Pipes {
    public class HackInfo : Attribute {
        public enum HackType {
            Toggle, TextBox, BoolButton, SettingsToggle
        }
        public readonly HackType ControlType;
        public readonly string Name;
        public HackInfo(string name, HackType t) {
            Name = name; ControlType = t;
        }
    }
}
