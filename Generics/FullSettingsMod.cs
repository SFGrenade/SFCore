using System;
using Modding;

namespace SFCore.Generics
{
    public abstract class FullSettingsMod<TSave, TGlobal> : Mod where TSave : ModSettings, new() where TGlobal : ModSettings, new()
    {
        public override ModSettings SaveSettings
        {
            get => _saveSettings;
            set => _saveSettings = (TSave) value;
        }
        public override ModSettings GlobalSettings
        {
            get => _globalSettings;
            set => _globalSettings = (TGlobal) value;
        }
        protected TSave _saveSettings = new TSave();
        protected Type _saveSettingsType = typeof(TSave);
        protected TGlobal _globalSettings = new TGlobal();

        public FullSettingsMod() {}
        public FullSettingsMod(string name) : base(name) {}
    }
}
