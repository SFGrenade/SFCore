using GlobalEnums;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SFCore.Utils;

namespace SFCore;

/// <summary>
/// Structure of data for a single achievement.
/// </summary>
public struct SCustomAchievement
{
    /// <summary>
    /// The achievement key used to award the achievement.
    /// </summary>
    public string key;
    /// <summary>
    /// The sprite used for the achievement.
    /// </summary>
    public Sprite sprite;
    /// <summary>
    /// The title language key for the achievement.
    /// </summary>
    public string titleConvo;
    /// <summary>
    /// The text language key for the achievement.
    /// </summary>
    public string textConvo;
    /// <summary>
    /// A flag to indicate whether or not to display the text when the achievement is not yet acquired.
    /// </summary>
    public bool hidden;
}

/// <summary>
/// Achievement helper class for easily adding custom achievements.
/// The mod using this needs to handle the following:
/// - titleConvo Language string(s)
/// - textConvo Language string(s)
/// </summary>
public static class AchievementHelper
{
    private static List<SCustomAchievement> _customAchievements = new List<SCustomAchievement>();

    /// <summary>
    /// Constructs the helper, hooks needed methods.
    /// </summary>
    static AchievementHelper()
    {
        LogFine("!cctor");
        On.MenuAchievementsList.PreInit += OnMenuAchievementsListPreInit;
        //On.UIManager.RefreshAchievementsList += OnUIManagerRefreshAchievementsList;
        //On.AchievementHandler.CanAwardAchievement += (orig, self, key) => { orig(self, key); return true; };
        On.DesktopPlatform.IsAchievementUnlocked += OnDesktopPlatformOnIsAchievementUnlocked;
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
    /// Adds an achievement to the private list of custom achievements.
    /// </summary>
    /// <param name="key">Achievement key, determines if an achievement is unlocked</param>
    /// <param name="sprite">Sprite of the achievement</param>
    /// <param name="titleConvo">Language key of the achievement title</param>
    /// <param name="textConvo">Language key of the achievement description</param>
    /// <param name="hidden">Determines if the achievement is hidden until unlocked</param>
    public static void AddAchievement(string key, Sprite sprite, string titleConvo, string textConvo, bool hidden)
    {
        LogFine("!AddAchievement");
        foreach (SCustomAchievement achievement in _customAchievements)
        {
            // length check first because unity's framework472 is trash and doesn't do it itself
            if ((achievement.key.Length == key.Length) && (achievement.key == key))
            {
                LogError($"Achievement with key '{key}' already exists! It's defined as ('{achievement.key}', {achievement.sprite}, '{achievement.titleConvo}', '{achievement.textConvo}', {achievement.hidden})!");
                LogFine("~AddAchievement");
                return;
            }
        }
        LogDebug($"Adding achievement '{key}'");
        _customAchievements.Add(new SCustomAchievement()
        {
            key = key,
            sprite = sprite,
            titleConvo = titleConvo,
            textConvo = textConvo,
            hidden = hidden
        });
        LogFine("~AddAchievement");
    }

    /// <summary>
    /// Adds the contents of customAchievements to the given AchievementsList
    /// </summary>
    /// <param name="list">Achievement list which the custom achievements get added to</param>
    private static void InitAchievements(AchievementsList list)
    {
        LogFine("!InitAchievements");
        List<Achievement> achievements = list.GetAttr<AchievementsList, List<Achievement>>("achievements");
        foreach (var ca in _customAchievements)
        {
            // length check first because unity's framework472 is trash and doesn't do it itself
            if (!achievements.Exists(a => ((a.key.Length == ca.key.Length) && (a.key == ca.key))))
            {
                Achievement customAch = new Achievement
                {
                    key = ca.key,
                    type = ca.hidden ? AchievementType.Hidden : AchievementType.Normal,
                    earnedIcon = ca.sprite,
                    unearnedIcon = ca.sprite,
                    localizedText = ca.textConvo,
                    localizedTitle = ca.titleConvo
                };
                achievements.Add(customAch);
            }
        }
        list.SetAttr<AchievementsList, List<Achievement>>("achievements", achievements);
        LogFine("~InitAchievements");
    }

    /// <summary>
    /// On hook that initializes achievements and achivements in the menu and unhooks itself afterwards.
    /// </summary>
    private static void OnMenuAchievementsListPreInit(On.MenuAchievementsList.orig_PreInit orig, MenuAchievementsList self)
    {
        LogFine("!OnMenuAchievementsListPreInit");
        InitAchievements(GameManager.instance.achievementHandler.AchievementsList);
        orig(self);
        LogFine("~OnMenuAchievementsListPreInit");
    }

    // /// <summary>
    // /// On hook that initializes achievements and achivements in the menu and unhooks itself afterwards.
    // /// </summary>
    // private static void OnUIManagerRefreshAchievementsList(On.UIManager.orig_RefreshAchievementsList orig, UIManager self)
    // {
    //     LogFine("!OnUIManagerRefreshAchievementsList");
    //     InitAchievements(GameManager.instance.achievementHandler.achievementsList);
    //     orig(self);
    //     LogFine("~OnUIManagerRefreshAchievementsList");
    // }

    /// <summary>
    /// On hook that checks for custom achievements and looks into EncryptedSharedData for them.
    /// </summary>
    private static bool? OnDesktopPlatformOnIsAchievementUnlocked(On.DesktopPlatform.orig_IsAchievementUnlocked orig, DesktopPlatform self, string key)
    {
        LogFine("!OnDesktopPlatformIsAchievementUnlocked");
        bool? isUnlocked = orig(self, key);
        // length check first because unity's framework472 is trash and doesn't do it itself
        if (_customAchievements.Exists(x => ((x.key.Length == key.Length) && (x.key == key))))
        {
            // just check again, to be sure
            isUnlocked = self.RoamingSharedData.GetBool(key, def: false);
        }
        LogFine("~OnDesktopPlatformIsAchievementUnlocked");
        return isUnlocked;
    }


    private static void LogFine(string message) => InternalLogger.LogFine(message, "[SFCore]:[AchievementHelper]");
    private static void LogFine(object message) => LogFine($"{message}");
    private static void LogDebug(string message) => InternalLogger.LogDebug(message, "[SFCore]:[AchievementHelper]");
    private static void LogDebug(object message) => LogDebug($"{message}");
    private static void Log(string message) => InternalLogger.Log(message, "[SFCore]:[AchievementHelper]");
    private static void Log(object message) => Log($"{message}");
    private static void LogWarn(string message) => InternalLogger.LogWarn(message, "[SFCore]:[AchievementHelper]");
    private static void LogWarn(object message) => LogWarn($"{message}");
    private static void LogError(string message) => InternalLogger.LogError(message, "[SFCore]:[AchievementHelper]");
    private static void LogError(object message) => LogError($"{message}");
}