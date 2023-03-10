using Modding;

namespace SFCore.Generics;

/// <summary>
/// Abstract class to avoid boilerplate code.
/// This adds easy SaveSettings functionality.
/// </summary>
public abstract class SaveSettingsMod<TSave> : Mod, ILocalSettings<TSave> where TSave : new()
{
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
    public SaveSettingsMod() { }
    /// <summary>
    /// Constructor of this abstract class.
    /// </summary>
    /// <param name="name">Name of the mod, how it's going to be displayed in the main and pause menus.</param>
    public SaveSettingsMod(string name) : base(name) { }
}