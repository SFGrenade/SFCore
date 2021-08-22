using Modding;

namespace SFCore.Generics
{
    public abstract class SaveSettingsMod<TSave> : Mod, ILocalSettings<TSave> where TSave : new()
    {
        // The save data specific to a certain savefile. This setting will be loaded each time a save is opened.
        public TSave SaveSettings { get; protected set; }
        // Implement the LocalSettings interface.
        // This method gets called when a save is loaded.
        public void OnLoadLocal(TSave s) => SaveSettings = s;
        // This method gets called when the player saves their file.
        public TSave OnSaveLocal() => SaveSettings;

        public SaveSettingsMod() { }
        public SaveSettingsMod(string name) : base(name) { }
    }
}
