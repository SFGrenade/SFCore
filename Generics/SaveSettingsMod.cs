using Modding;

namespace SFCore.Generics
{
    public abstract class SaveSettingsMod<TSave> : Mod, ILocalSettings<TSave> where TSave : new()
    {
        // The save data specific to a certain savefile. This setting will be loaded each time a save is opened.
        public TSave _saveSettings { get; protected set; }
        // Implement the LocalSettings interface.
        // This method gets called when a save is loaded.
        public void OnLoadLocal(TSave s) => this._saveSettings = s;
        // This method gets called when the player saves their file.
        public TSave OnSaveLocal() => this._saveSettings;

        public SaveSettingsMod() { }
        public SaveSettingsMod(string name) : base(name) { }
    }
}
