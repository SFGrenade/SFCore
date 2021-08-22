using Modding;

namespace SFCore.Generics
{
    public abstract class FullSettingsMod<TSave, TGlobal> : Mod, ILocalSettings<TSave>, IGlobalSettings<TGlobal> where TSave : new() where TGlobal : new()
    {
        // The global settings for this mod. The settings load will only occur once
        // so a static field should be used to prevent loss of data
        public static TGlobal GlobalSettings { get; protected set; }
        // Implement the GlobalSettings interface.
        // This method gets called when the mod loader loads the global settings.
        public void OnLoadGlobal(TGlobal s) => GlobalSettings = s;
        // This method gets called when the mod loader needs to save the global settings.
        public TGlobal OnSaveGlobal() => GlobalSettings;

        // The save data specific to a certain savefile. This setting will be loaded each time a save is opened.
        public TSave SaveSettings { get; protected set; }
        // Implement the LocalSettings interface.
        // This method gets called when a save is loaded.
        public void OnLoadLocal(TSave s) => SaveSettings = s;
        // This method gets called when the player saves their file.
        public TSave OnSaveLocal() => SaveSettings;

        public FullSettingsMod() {}
        public FullSettingsMod(string name) : base(name) {}
    }
}
