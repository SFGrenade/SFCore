using System;
using Modding;

namespace SFCore.Generics
{
    public abstract class SaveSettingsMod<TSave> : Mod where TSave : ModSettings, new()
    {
        public override ModSettings SaveSettings
        {
            get => _saveSettings;
            set => _saveSettings = (TSave) value;
        }
        protected TSave _saveSettings = new TSave();
        protected Type _saveSettingsType = typeof(TSave);

        public SaveSettingsMod() { }
        public SaveSettingsMod(string name) : base(name) { }
    }
}
