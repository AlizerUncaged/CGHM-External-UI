using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Stage.Communication_and_Pipes {
   
    public class _Hacks {

        [JsonProperty("menu_color.x")]
        public double MenuColorX { get; set; }

        [JsonProperty("menu_color.y")]
        public double MenuColorY { get; set; }

        [JsonProperty("menu_color.z")]
        public double MenuColorZ { get; set; }

        [JsonProperty("menu_color.w")]
        public double MenuColorW { get; set; }

        [JsonProperty("menu_scale")]
        public double MenuScale { get; set; }

        [JsonProperty("esp_thickness")]
        public double EspThickness { get; set; }

        [JsonProperty("keybinds_show.hotkey")]
        public int KeybindsShowHotkey { get; set; }

        [JsonProperty("search_show.hotkey")]
        public int SearchShowHotkey { get; set; }

        [JsonProperty("keybinds_set.hotkey")]
        public int KeybindsSetHotkey { get; set; }

        [JsonProperty("menu_show.hotkey")]
        public int MenuShowHotkey { get; set; }

        // [HackInfo("Fly Mod" , HackInfo.HackType.Toggle)]
        [JsonProperty("mod_fly.active")]
        public bool ModFlyActive { get; set; }

        [JsonProperty("mod_fly.hotkey")]
        public int ModFlyHotkey { get; set; }

        // [HackInfo("No-Clip", HackInfo.HackType.Toggle)]
        [JsonProperty("mod_noclip.active")]
        public bool ModNoclipActive { get; set; }

        [JsonProperty("mod_noclip.hotkey")]
        public int ModNoclipHotkey { get; set; }

        // [HackInfo("Infinite Punch", HackInfo.HackType.Toggle)]
        [JsonProperty("mod_infinite_punch.active")]
        public bool ModInfinitePunchActive { get; set; }

        [JsonProperty("mod_infinite_punch.hotkey")]
        public int ModInfinitePunchHotkey { get; set; }

        // [HackInfo("Game Start", HackInfo.HackType.BoolButton)]
        [JsonProperty("mod_game_start.active")]
        public bool ModGameStartActive { get; set; }

        [JsonProperty("mod_game_start.hotkey")]
        public int ModGameStartHotkey { get; set; }

        // [HackInfo("Game Start", HackInfo.HackType.BoolButton)]
        [JsonProperty("mod_game_next.active")]
        public bool ModGameNextActive { get; set; }

        [JsonProperty("mod_game_next.hotkey")]
        public int ModGameNextHotkey { get; set; }

        [JsonProperty("esp_box.value.x")]
        public double EspBoxValueX { get; set; }

        [JsonProperty("esp_box.value.y")]
        public double EspBoxValueY { get; set; }

        [JsonProperty("esp_box.value.z")]
        public double EspBoxValueZ { get; set; }

        [JsonProperty("esp_box.value.w")]
        public double EspBoxValueW { get; set; }

        // [HackInfo("Player ESP Box", HackInfo.HackType.Toggle)]
        [JsonProperty("esp_box.active")]
        public bool EspBoxActive { get; set; }

        [JsonProperty("esp_box.hotkey")]
        public int EspBoxHotkey { get; set; }

        [JsonProperty("esp_tracer.value.x")]
        public double EspTracerValueX { get; set; }

        [JsonProperty("esp_tracer.value.y")]
        public double EspTracerValueY { get; set; }

        [JsonProperty("esp_tracer.value.z")]
        public double EspTracerValueZ { get; set; }

        [JsonProperty("esp_tracer.value.w")]
        public double EspTracerValueW { get; set; }

        // [HackInfo("Player ESP Tracer", HackInfo.HackType.Toggle)]
        [JsonProperty("esp_tracer.active")]
        public bool EspTracerActive { get; set; }

        [JsonProperty("esp_tracer.hotkey")]
        public int EspTracerHotkey { get; set; }
    }
}
