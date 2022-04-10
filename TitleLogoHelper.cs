using UnityEngine;
using Logger = Modding.Logger;
using System.Collections.Generic;
using UObject = UnityEngine.Object;

namespace SFCore
{
    /// <summary>
    ///     Title logo helper class for easily adding custom title logos.
    ///     The mod using this needs to handle the following:
    ///     - save title logo IDs
    /// </summary>
    public static class TitleLogoHelper
    {
        private static bool _instanceChanged = false;
        private static List<Sprite> _customLogos = new List<Sprite>();

        static TitleLogoHelper()
        {
            On.MenuStyleTitle.ctor += OnMenuStyleTitleConstructor;
            On.MenuStyleTitle.SetTitle += OnMenuStyleTitleSetTitle;
        }
        /// <summary>
        ///     Used for static initialization.
        /// </summary>
        public static void unusedInit() { }

        /// <inheritdoc />
        /// <summary>
        ///     Adds the custom title logo.
        /// </summary>
        /// <returns>ID of the custom title logo</returns>
        public static int AddLogo(Sprite logo)
        {
            _customLogos.Add(logo);
            UObject.DontDestroyOnLoad(logo);
            return _customLogos.Count;
        }

        private static void OnMenuStyleTitleConstructor(On.MenuStyleTitle.orig_ctor orig, MenuStyleTitle self)
        {
            orig(self);
            _instanceChanged = false;
        }

        private static void OnMenuStyleTitleSetTitle(On.MenuStyleTitle.orig_SetTitle orig, MenuStyleTitle self, int index)
        {
            if (!_instanceChanged)
            {
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

        private static void Log(string message)
        {
            Logger.LogDebug($"[SFCore]:[TitleLogoHelper] - {message}");
            Debug.Log($"[SFCore]:[TitleLogoHelper] - {message}");
        }
        private static void Log(object message)
        {
            Log($"{message}");
        }
    }
}
