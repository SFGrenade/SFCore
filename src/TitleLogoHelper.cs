using UnityEngine;
using System.Collections.Generic;
using UObject = UnityEngine.Object;

namespace SFCore;

/// <summary>
/// Title logo helper class for easily adding custom title logos.
/// The mod using this needs to handle the following:
/// - save title logo IDs
/// </summary>
public static class TitleLogoHelper
{
    private static bool _instanceChanged = false;
    private static MenuStyleTitle _lastInstance = null;
    private static List<Sprite> _customLogos = new List<Sprite>();

    static TitleLogoHelper()
    {
        On.MenuStyleTitle.SetTitle += OnMenuStyleTitleSetTitle;
    }
    /// <summary>
    /// Used for static initialization.
    /// </summary>
    public static void unusedInit() { }

    /// <summary>
    /// Adds the custom title logo.
    /// </summary>
    /// <returns>ID of the custom title logo</returns>
    public static int AddLogo(Sprite logo)
    {
        _customLogos.Add(logo);
        UObject.DontDestroyOnLoad(logo);
        return _customLogos.Count;
    }

    private static void OnMenuStyleTitleSetTitle(On.MenuStyleTitle.orig_SetTitle orig, MenuStyleTitle self, int index)
    {
        if (self == null)
        {
            orig(self, index);
            return;
        }

        if (!ReferenceEquals(_lastInstance, self))
        {
            _lastInstance = self;
            _instanceChanged = false;
        }

        if (!_instanceChanged)
        {
            if (self.TitleSprites == null)
            {
                orig(self, index);
                return;
            }

            RuntimePlatform[] allPlatforms = new[]
            {
                RuntimePlatform.OSXPlayer,
                RuntimePlatform.WindowsPlayer,
                RuntimePlatform.IPhonePlayer,
                RuntimePlatform.Android,
                RuntimePlatform.LinuxPlayer,
                RuntimePlatform.WebGLPlayer,
                RuntimePlatform.WSAPlayerX86,
                RuntimePlatform.WSAPlayerX64,
                RuntimePlatform.WSAPlayerARM,
                RuntimePlatform.PS4,
                RuntimePlatform.XboxOne,
                RuntimePlatform.tvOS,
                RuntimePlatform.Switch
            };
            //for (int i = 0; i < self.TitleSprites.Length; i++)
            //{
            //    self.TitleSprites[i].PlatformWhitelist = allPlatforms;
            //}

            List<MenuStyleTitle.TitleSpriteCollection> tmpList = new List<MenuStyleTitle.TitleSpriteCollection>(self.TitleSprites);
            foreach (var s in _customLogos)
            {
                var tmpTsc = new MenuStyleTitle.TitleSpriteCollection();

                tmpTsc.PlatformWhitelist = allPlatforms;
                tmpTsc.Default = s;
                tmpTsc.Chinese = null;
                tmpTsc.Russian = null;
                tmpTsc.Italian = null;
                tmpTsc.Japanese = null;
                tmpTsc.Spanish = null;
                tmpTsc.Korean = null;
                tmpTsc.French = null;
                tmpTsc.BrazilianPT = null;

                tmpList.Add(tmpTsc);
            }
            self.TitleSprites = tmpList.ToArray();

            _instanceChanged = true;
        }

        orig(self, index);
    }

    private static void LogFine(string message) => InternalLogger.LogFine(message, "[SFCore]:[TitleLogoHelper]");
    private static void LogFine(object message) => LogFine($"{message}");
    private static void LogDebug(string message) => InternalLogger.LogDebug(message, "[SFCore]:[TitleLogoHelper]");
    private static void LogDebug(object message) => LogDebug($"{message}");
    private static void Log(string message) => InternalLogger.Log(message, "[SFCore]:[TitleLogoHelper]");
    private static void Log(object message) => Log($"{message}");
    private static void LogWarn(string message) => InternalLogger.LogWarn(message, "[SFCore]:[TitleLogoHelper]");
    private static void LogWarn(object message) => LogWarn($"{message}");
    private static void LogError(string message) => InternalLogger.LogError(message, "[SFCore]:[TitleLogoHelper]");
    private static void LogError(object message) => LogError($"{message}");
}
