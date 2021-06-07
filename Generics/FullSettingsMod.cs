using System;
using Modding;

namespace SFCore.Generics
{
    public abstract class FullSettingsMod<TSave, TGlobal> : Mod, ILocalSettings<TSave>, IGlobalSettings<TGlobal> where TSave : new() where TGlobal : new()
    {
        // The global settings for this mod. The settings load will only occur once
        // so a static field should be used to prevent loss of data
        public static TGlobal _globalSettings { get; protected set; }
        // Implement the GlobalSettings interface.
        // This method gets called when the mod loader loads the global settings.
        public void OnLoadGlobal(TGlobal s) => _globalSettings = s;
        // This method gets called when the mod loader needs to save the global settings.
        public TGlobal OnSaveGlobal() => _globalSettings;

        // The save data specific to a certain savefile. This setting will be loaded each time a save is opened.
        public TSave _saveSettings { get; protected set; }
        // Implement the LocalSettings interface.
        // This method gets called when a save is loaded.
        public void OnLoadLocal(TSave s) => this._saveSettings = s;
        // This method gets called when the player saves their file.
        public TSave OnSaveLocal() => this._saveSettings;

        public FullSettingsMod() {}
        public FullSettingsMod(string name) : base(name) {}
    }
}
