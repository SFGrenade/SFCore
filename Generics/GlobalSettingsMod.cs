using System;
using Modding;

namespace SFCore.Generics
{
    public abstract class GlobalSettingsMod<TGlobal> : Mod where TGlobal : ModSettings, new()
    {
        public override ModSettings GlobalSettings
        {
            get => _globalSettings;
            set => _globalSettings = (TGlobal) value;
        }
        protected TGlobal _globalSettings = new TGlobal();
        protected Type _globalSettingsType = typeof(TGlobal);

        public GlobalSettingsMod() { }
        public GlobalSettingsMod(string name) : base(name) { }
    }
}
