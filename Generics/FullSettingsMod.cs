using Modding;

namespace SFCore.Generics
{
    /// <summary>
    /// Abstract class to avoid boilerplate code.
    /// This adds easy SaveSettings and GlobalSettings functionality.
    /// </summary>
    public abstract class FullSettingsMod<TSave, TGlobal> : Mod, ILocalSettings<TSave>, IGlobalSettings<TGlobal> where TSave : new() where TGlobal : new()
    {
        /// <summary>
        /// The global settings for this mod. The settings load will only occur once
        /// so a static field should be used to prevent loss of data
        /// </summary>
        public static TGlobal GlobalSettings { get; protected set; } = new TGlobal();
        /// <summary>
        /// Loads the saved GlobalSettings.
        /// </summary>
        /// <param name="s">The settings that are being loaded</param>
        public void OnLoadGlobal(TGlobal s) => GlobalSettings = s;
        /// <summary>
        /// Returns the current SaveSettings to save them to the disk.
        /// </summary>
        /// <returns>The settings that are going to be saved.</returns>
        public TGlobal OnSaveGlobal() => GlobalSettings;

        /// <summary>
        /// The save data specific to a certain savefile. This setting will be loaded each time a save is opened.
        /// </summary>
        public TSave SaveSettings { get; protected set; } = new TSave();
        /// <summary>
        /// Loads the saved SaveSettings of the save.
        /// </summary>
        /// <param name="s">The settings that are being loaded</param>
        public void OnLoadLocal(TSave s) => SaveSettings = s;
        /// <summary>
        /// Returns the current SaveSettings to save them to the disk.
        /// </summary>
        /// <returns>The settings that are going to be saved.</returns>
        public TSave OnSaveLocal() => SaveSettings;

        /// <summary>
        /// Constructor of this abstract class.
        /// </summary>
        public FullSettingsMod() { }
        /// <summary>
        /// Constructor of this abstract class.
        /// </summary>
        /// <param name="name">Name of the mod, how it's going to be displayed in the main and pause menus.</param>
        public FullSettingsMod(string name) : base(name) {}
    }
}
