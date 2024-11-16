using System;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using SFCore.Utils;
using Object = UnityEngine.Object;

namespace SFCore;

/// <summary>
/// DLC icon helper class for easily adding custom dlc icons.
/// </summary>
public static class DlcIconHelper
{
    /// <summary>
    /// List of sprites to use.
    /// </summary>
    private static readonly List<Sprite> CustomSprites = new List<Sprite>();

    /// <summary>
    /// Constructs the mod and hooks important functions.
    /// </summary>
    static DlcIconHelper()
    {
        LogFine("!cctor");

        On.UIManager.Start += AddDlcIcons;

        LogFine("~cctor");
    }

    /// <summary>
    /// Used for static initialization.
    /// </summary>
    public static void unusedInit()
    {
        LogFine("!unusedInit");
        LogFine("~unusedInit");
    }

    /// <summary>
    /// Adds a sprite to the list of dlc icons.
    /// </summary>
    public static void AddDlcIcon(Sprite icon)
    {
        LogFine("!AddDlcIcon");

        CustomSprites.Add(icon);

        LogFine("~AddDlcIcon");
    }

    /// <summary>
    /// Helper method for positioning.
    /// </summary>
    private static Vector3 GetPositionOffsetForIndex(int index)
    {
        float xOffset = 0.83f;
        float yOffset = 0.83f;

        float xStart = 3.1f - (5 * xOffset);
        float xComponent = xStart + ((index % 10) * xOffset);
        float yComponent = -0.16f + (Mathf.Floor(index / 10f) * yOffset);
        return new Vector3(xComponent, yComponent);
    }

    /// <summary>
    /// Method that actually does add the icons to the dlc display.
    /// </summary>
    private static void AddDlcIcons(On.UIManager.orig_Start orig, UIManager self)
    {
        LogFine("!AddDlcIcons");
        orig(self);

        var dlc = self.transform.Find("UICanvas/MainMenuScreen/TeamCherryLogo/Hidden_Dreams_Logo").gameObject;

        for (int i = 0; i < CustomSprites.Count; i++)
        {
            var clone = Object.Instantiate(dlc, dlc.transform.parent);
            clone.name = $"CUSTOM DLC ICON {CustomSprites[i].name}";
            clone.SetActive(true);

            clone.transform.position += GetPositionOffsetForIndex(i + 5);

            var sr = clone.GetComponent<SpriteRenderer>();
            sr.sprite = CustomSprites[i];
        }
        LogFine("~AddDlcIcons");
    }

    /// <summary>
    /// Adds charm to fade group.
    /// </summary>
    private static void AddToCharmFadeGroup(GameObject spriteGo, GameObject fgGo)
    {
        LogFine("!AddToCharmFadeGroup");
        var sr = spriteGo.GetComponent<SpriteRenderer>();
        sr.sortingLayerID = 629535577;
        var fg = fgGo.GetComponent<FadeGroup>();
        var srList = new List<SpriteRenderer>(fg.spriteRenderers);
        srList.Add(sr);
        fg.spriteRenderers = srList.ToArray();
        LogFine("~AddToCharmFadeGroup");
    }

    /// <summary>
    /// Makes a gameobject not be destroyed.
    /// </summary>
    private static void SetInactive(Object go)
    {
        LogFine("!SetInactive");
        if (go != null)
        {
            Object.DontDestroyOnLoad(go);
        }

        LogFine("~SetInactive");
    }

    private static void LogFine(string message) => InternalLogger.LogFine(message, "[SFCore]:[DlcIconHelper]");
    private static void LogFine(object message) => LogFine($"{message}");
    private static void LogDebug(string message) => InternalLogger.LogDebug(message, "[SFCore]:[DlcIconHelper]");
    private static void LogDebug(object message) => LogDebug($"{message}");
    private static void Log(string message) => InternalLogger.Log(message, "[SFCore]:[DlcIconHelper]");
    private static void Log(object message) => Log($"{message}");
    private static void LogWarn(string message) => InternalLogger.LogWarn(message, "[SFCore]:[DlcIconHelper]");
    private static void LogWarn(object message) => LogWarn($"{message}");
    private static void LogError(string message) => InternalLogger.LogError(message, "[SFCore]:[DlcIconHelper]");
    private static void LogError(object message) => LogError($"{message}");
}