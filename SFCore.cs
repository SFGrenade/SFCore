using System.Collections.Generic;
using System.Reflection;
using SFCore.Generics;
using SFCore.MonoBehaviours;
using SFCore.Utils;
using UnityEngine;

namespace SFCore;

/// <summary>
/// Used for static initialization.
/// </summary>
public class SFCoreMod : FullSettingsMod<SFCoreSaveSettings, SFCoreGlobalSettings>
{
    internal static SFCoreMod instance = null;
    static SFCoreMod()
    {
        // to load all components
        //SFCore.AchievementHelper.unusedInit();
        //SFCore.CharmHelper.unusedInit();
        //SFCore.EnviromentParticleHelper.unusedInit();
        //SFCore.ItemHelper.unusedInit();
        SFCore.MenuStyleHelper.unusedInit();
        SFCore.TitleLogoHelper.unusedInit();
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public SFCoreMod() : base("SFCore")
    {
        instance = this;
    }

    /// <summary>
    /// Displays the version.
    /// </summary>
    public override string GetVersion() => Util.GetVersion(Assembly.GetExecutingAssembly());

    /// <summary>
    /// Get names of objects to preload.
    /// </summary>
    /// <returns>List of (scene, name) tuples to preload.</returns>
    public override List<(string, string)> GetPreloadNames() => new() { ("Town", "_SceneManager") };

    /// <summary>
    /// Main menu is loaded.
    /// </summary>
    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        SceneManagerPatcher.LoadPrefabs(preloadedObjects["Town"]["_SceneManager"].GetComponent<SceneManager>());
    }
}

/// <summary>
/// Global settings for SFCore
/// </summary>
public class SFCoreGlobalSettings
{
    /// <summary>
    /// Important to clear and reapply custom charms.
    /// </summary>
    public int MaxCustomCharms = 0;
    /// <summary>
    /// Important to reapply the wanted menu theme.
    /// </summary>
    public string SelectedMenuTheme = "UI_MENU_STYLE_CLASSIC";
}

/// <summary>
/// Save specific settings for SFCore
/// </summary>
public class SFCoreSaveSettings
{
}