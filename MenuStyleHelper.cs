using System;
using System.Collections;
using ModCommon.Util;
using UnityEngine;
using Logger = Modding.Logger;
using System.Collections.Generic;
using System.Linq;
using Modding;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UObject = UnityEngine.Object;

namespace SFCore
{
    public class MenuStyleHelper
    {
        private static bool initialized = false;

        private static List<(string, GameObject, int, string, string[], MenuStyles.MenuStyle.CameraCurves, AudioMixerSnapshot)> queue = new List<(string, GameObject, int, string, string[], MenuStyles.MenuStyle.CameraCurves, AudioMixerSnapshot)>();
        private static List<Func<MenuStyles, (string, GameObject, int, string, string[], MenuStyles.MenuStyle.CameraCurves, AudioMixerSnapshot)>> callbackQueue = new List<Func<MenuStyles, (string, GameObject, int, string, string[], MenuStyles.MenuStyle.CameraCurves, AudioMixerSnapshot)>>();

        public delegate (string languageString, GameObject styleGo, int titleIndex, string unlockKey, string[] achievementKeys, MenuStyles.MenuStyle.CameraCurves cameraCurves, AudioMixerSnapshot musicSnapshot) Hook(MenuStyles self);
        public static Hook AddMenuStyleHook;

        public static void Initialize()
        {
            if (!initialized)
            {
                initialized = true;
                On.MenuStyles.Awake += OnMenuStylesAwake;
                On.MenuStyles.SetStyle += OnMenuStylesSetStyle;
            }
        }

        public static void AddMenuStyle(string languageString, GameObject styleGo, int titleIndex = -1, string unlockKey = "", string[] achievementKeys = null, MenuStyles.MenuStyle.CameraCurves cameraCurves = null, AudioMixerSnapshot musicSnapshot = null)
        {
            queue.Add((languageString, styleGo, titleIndex, unlockKey, achievementKeys, cameraCurves, musicSnapshot));
            UObject.DontDestroyOnLoad(styleGo);
            UObject.DontDestroyOnLoad(musicSnapshot);
        }
        public static void AddMenuStyle(Func<MenuStyles, (string, GameObject, int, string, string[], MenuStyles.MenuStyle.CameraCurves, AudioMixerSnapshot)> callback)
        {
            callbackQueue.Add(callback);
        }

        private static void OnMenuStylesAwake(On.MenuStyles.orig_Awake orig, MenuStyles self)
        {
            orig(self);
            AddCustomStyles(self);
        }

        private static void AddCustomStyles(MenuStyles self)
        {
            List<MenuStyles.MenuStyle> tmpList = new List<MenuStyles.MenuStyle>(self.styles);
            var tmpMenuStyle = tmpList[0];
            foreach (var (languageString, styleGo, titleIndex, unlockKey, achievementKeys, cameraCurves, musicSnapshot) in queue)
            {
                var tmpCameraCurves = cameraCurves ?? tmpMenuStyle.cameraColorCorrection;
                var tmpMusicSnapshot = musicSnapshot ?? tmpMenuStyle.musicSnapshot;
                styleGo.transform.SetParent(self.transform);
                var tmpStyle = new MenuStyles.MenuStyle
                {
                    enabled = true,
                    displayName = languageString,
                    styleObject = styleGo,
                    cameraColorCorrection = tmpCameraCurves,
                    musicSnapshot = tmpMusicSnapshot,
                    titleIndex = titleIndex,
                    unlockKey = unlockKey,
                    achievementKeys = achievementKeys,
                    initialAudioVolumes = tmpMenuStyle.initialAudioVolumes
                };
                tmpList.Add(tmpStyle);
            }
            foreach (var callback in callbackQueue)
            {
                var (languageString, styleGo, titleIndex, unlockKey, achievementKeys, cameraCurves, musicSnapshot) =
                    callback(self);
                var tmpCameraCurves = cameraCurves ?? tmpMenuStyle.cameraColorCorrection;
                var tmpMusicSnapshot = musicSnapshot ?? tmpMenuStyle.musicSnapshot;
                styleGo.transform.SetParent(self.transform);
                var tmpStyle = new MenuStyles.MenuStyle
                {
                    enabled = true,
                    displayName = languageString,
                    styleObject = styleGo,
                    cameraColorCorrection = tmpCameraCurves,
                    musicSnapshot = tmpMusicSnapshot,
                    titleIndex = titleIndex,
                    unlockKey = unlockKey,
                    achievementKeys = achievementKeys,
                    initialAudioVolumes = tmpMenuStyle.initialAudioVolumes
                };
                tmpList.Add(tmpStyle);
            }
            foreach (var callback in AddMenuStyleHook.GetInvocationList())
            {
                if (callback == null)
                    continue;

                var (languageString, styleGo, titleIndex, unlockKey, achievementKeys, cameraCurves, musicSnapshot) =
                    ((string, GameObject, int, string, string[], MenuStyles.MenuStyle.CameraCurves,
                        AudioMixerSnapshot))callback.DynamicInvoke(self);
                var tmpCameraCurves = cameraCurves ?? tmpMenuStyle.cameraColorCorrection;
                var tmpMusicSnapshot = musicSnapshot ?? tmpMenuStyle.musicSnapshot;
                styleGo.transform.SetParent(self.transform);
                var tmpStyle = new MenuStyles.MenuStyle
                {
                    enabled = true,
                    displayName = languageString,
                    styleObject = styleGo,
                    cameraColorCorrection = tmpCameraCurves,
                    musicSnapshot = tmpMusicSnapshot,
                    titleIndex = titleIndex,
                    unlockKey = unlockKey,
                    achievementKeys = achievementKeys,
                    initialAudioVolumes = tmpMenuStyle.initialAudioVolumes
                };
                tmpList.Add(tmpStyle);
            }
            self.styles = tmpList.ToArray();

            int tmpInt = Platform.Current.EncryptedSharedData.GetInt("menuStyle", 0);
            if (tmpInt >= self.styles.Length)
                Platform.Current.EncryptedSharedData.SetInt("menuStyle", 0);
        }

        private static IEnumerator OnMenuStylesFade(On.MenuStyles.orig_Fade orig, MenuStyles self, int styleindex, int fadetype, bool fade, AudioSource[] audiosources)
        {
            var tmp = orig(self, styleindex, fadetype, fade, audiosources);
            GameCameras.instance.colorCorrectionCurves.UpdateTextures();
            return tmp;
        }

        private static void OnMenuStylesSetStyle(On.MenuStyles.orig_SetStyle orig, MenuStyles self, int index, bool fade, bool save)
        {
            if (index < 0 || index >= 10)
            {
                orig(self, index, fade, false);
            }
            else
            {
                orig(self, index, fade, save);
            }
            //GameManager.instance.StartCoroutine(UpdateTexturesWhileSwitching(self));
        }

        private static IEnumerator UpdateTexturesWhileSwitching(MenuStyles self)
        {
            while (self.GetAttr<MenuStyles, Coroutine>("fadeRoutine") != null)
            {
                GameCameras.instance.colorCorrectionCurves.UpdateTextures();
                yield return null;
            }
            for (float elapsed = 0f; elapsed < 0.251f; elapsed += Time.deltaTime)
            {
                GameCameras.instance.colorCorrectionCurves.UpdateTextures();
                yield return null;
            }
            yield break;
        }

        private static void Log(string message)
        {
            Logger.Log($"[SFCore]:[MenuStyleHelper] - {message}");
        }
        private static void Log(object message)
        {
            Logger.Log($"[SFCore]:[MenuStyleHelper] - {message.ToString()}");
        }
    }
}
