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
    public override List<(string, string)> GetPreloadNames() => new() { ("Room_Mender_House", "_SceneManager") };

    /// <summary>
    /// Main menu is loaded.
    /// </summary>
    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        SceneManagerPatcher.LoadPrefabs(preloadedObjects["Room_Mender_House"]["_SceneManager"].GetComponent<SceneManager>());
    }
}
