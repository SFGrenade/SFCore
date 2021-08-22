using Modding;

namespace SFCore.Generics
{
    public abstract class GlobalSettingsMod<TGlobal> : Mod, IGlobalSettings<TGlobal> where TGlobal : new()
    {
        // The global settings for this mod. The settings load will only occur once
        // so a static field should be used to prevent loss of data
        public static TGlobal GlobalSettings { get; protected set; }
        // Implement the GlobalSettings interface.
        // This method gets called when the mod loader loads the global settings.
        public void OnLoadGlobal(TGlobal s) => GlobalSettings = s;
        // This method gets called when the mod loader needs to save the global settings.
        public TGlobal OnSaveGlobal() => GlobalSettings;

        public GlobalSettingsMod() { }
        public GlobalSettingsMod(string name) : base(name) { }
    }
}
