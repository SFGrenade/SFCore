using System;
using System.Collections.Generic;
using SFCore.Utils;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Object = UnityEngine.Object;

namespace SFCore
{
    /// <summary>
    ///     Enviroment particle helper class for easily adding custom enviroment particles.
    /// </summary>
    public static class EnviromentParticleHelper
    {
        private static PlayerData _pd;
        private static Dictionary<int, AudioClip> _customWalkAudio = new Dictionary<int, AudioClip>();
        private static Dictionary<int, AudioClip> _customRunAudio = new Dictionary<int, AudioClip>();
        private static Dictionary<int, GameObject> _customDashEffects = new Dictionary<int, GameObject>();
        private static Dictionary<int, GameObject> _customHardLandEffects = new Dictionary<int, GameObject>();
        private static Dictionary<int, GameObject> _customJumpEffects = new Dictionary<int, GameObject>();
        private static Dictionary<int, GameObject> _customSoftLandEffects = new Dictionary<int, GameObject>();

        private static Dictionary<int, GameObject> _customRunEffects = new Dictionary<int, GameObject>();
        private static Dictionary<int, string> _customRunEffectsPrefabs = new Dictionary<int, string>();

        static EnviromentParticleHelper()
        {
            _pd = PlayerData.instance;

            On.HeroController.checkEnvironment += OnHeroControllercheckEnvironment;
            On.DashEffect.OnEnable += OnDashEffectOnEnable;
            On.HardLandEffect.OnEnable += OnHardLandEffectOnEnable;
            On.JumpEffects.OnEnable += OnJumpEffectsOnEnable;
            On.SoftLandEffect.OnEnable += OnSoftLandEffectOnEnable;
            On.EventRegister.Awake += OnEventRegisterAwake;
        }
        /// <summary>
        ///     Used for static initialization.
        /// </summary>
        public static void unusedInit() { }

        #region Hooks
        private static void OnHeroControllercheckEnvironment(On.HeroController.orig_checkEnvironment orig, HeroController self)
        {
            orig(self);

            try
            {
                if (AddCustomWalkAudioHook != null)
                {
                    foreach (var callback in AddCustomWalkAudioHook.GetInvocationList())
                    {
                        if (callback == null)
                            continue;
                        var (enviromentType, runAudio) = ((int enviromentType, AudioClip runAudio)) callback.DynamicInvoke(self);
                        AddRunAudio(enviromentType, runAudio);
                    }
                }
                if (_customRunAudio != null)
                    self.footStepsRunAudioSource.clip = _customRunAudio[_pd.GetInt("environmentType")];
            }
            catch (Exception)
            {}
            try
            {
                if (AddCustomWalkAudioHook != null)
                {
                    foreach (var callback in AddCustomWalkAudioHook.GetInvocationList())
                    {
                        if (callback == null)
                            continue;
                        var (enviromentType, walkAudio) = ((int enviromentType, AudioClip walkAudio)) callback.DynamicInvoke(self);
                        AddWalkAudio(enviromentType, walkAudio);
                    }
                }
                if (_customWalkAudio != null)
                    self.footStepsWalkAudioSource.clip = _customWalkAudio[_pd.GetInt("environmentType")];
            }
            catch (Exception)
            {}
        }
        private static void OnDashEffectOnEnable(On.DashEffect.orig_OnEnable orig, DashEffect self)
        {
            orig(self);

            foreach (KeyValuePair<int, GameObject> tmp in _customDashEffects)
            {
                tmp.Value.SetActive(false);
            }

            try
            {
                if (AddCustomDashEffectsHook != null)
                {
                    foreach (var callback in AddCustomDashEffectsHook.GetInvocationList())
                    {
                        if (callback == null)
                            continue;
                        var (enviromentType, dashEffects) = ((int enviromentType, GameObject dashEffects)) callback.DynamicInvoke(self);
                        AddDashEffects(enviromentType, dashEffects);
                    }
                }
                GameObject dashEffectGo = _customDashEffects[_pd.GetInt("environmentType")];
                self.heroDashPuff.SetActive(false);
                self.dashDust.SetActive(false);
                self.heroDashPuff_anim.Stop();

                dashEffectGo.transform.SetParent(self.transform);
                dashEffectGo.transform.localPosition = Vector3.zero;
                dashEffectGo.SetActive(true);
            }
            catch (Exception)
            {}
        }
        private static void OnHardLandEffectOnEnable(On.HardLandEffect.orig_OnEnable orig, HardLandEffect self)
        {
            orig(self);

            foreach (KeyValuePair<int, GameObject> tmp in _customHardLandEffects)
            {
                tmp.Value.SetActive(false);
            }

            try
            {
                if (AddCustomHardLandEffectsHook != null)
                {
                    foreach (var callback in AddCustomHardLandEffectsHook.GetInvocationList())
                    {
                        if (callback == null)
                            continue;
                        var (enviromentType, hardLandEffects) = ((int enviromentType, GameObject hardLandEffects)) callback.DynamicInvoke(self);
                        AddHardLandEffects(enviromentType, hardLandEffects);
                    }
                }
                GameObject hardLandEffectGo = _customHardLandEffects[_pd.GetInt("environmentType")];
                hardLandEffectGo.transform.SetParent(self.transform);
                hardLandEffectGo.transform.localPosition = Vector3.zero;
                hardLandEffectGo.SetActive(true);
                //Log($"hard land effect activated: \"{hardLandEffectGo}\", at: {hardLandEffectGo.transform.position}");
            }
            catch (Exception)
            {}
        }
        private static void OnJumpEffectsOnEnable(On.JumpEffects.orig_OnEnable orig, JumpEffects self)
        {
            orig(self);

            foreach (KeyValuePair<int, GameObject> tmp in _customJumpEffects)
            {
                tmp.Value.SetActive(false);
            }

            try
            {
                if (AddCustomJumpEffectsHook != null)
                {
                    foreach (var callback in AddCustomJumpEffectsHook.GetInvocationList())
                    {
                        if (callback == null)
                            continue;
                        var (enviromentType, jumpEffects) = ((int enviromentType, GameObject jumpEffects)) callback.DynamicInvoke(self);
                        AddJumpEffects(enviromentType, jumpEffects);
                    }
                }
                GameObject jumpEffectGo = _customJumpEffects[_pd.GetInt("environmentType")];
                self.dustEffects.SetActive(false);

                jumpEffectGo.transform.SetParent(self.transform);
                jumpEffectGo.transform.localPosition = Vector3.zero;
                jumpEffectGo.SetActive(true);
                //Log($"jump effect activated: \"{jumpEffectGo}\", at: {jumpEffectGo.transform.position}");
            }
            catch (Exception)
            {}
        }
        private static void OnSoftLandEffectOnEnable(On.SoftLandEffect.orig_OnEnable orig, SoftLandEffect self)
        {
            orig(self);

            foreach (KeyValuePair<int, GameObject> tmp in _customSoftLandEffects)
            {
                tmp.Value.SetActive(false);
            }

            try
            {
                if (AddCustomSoftLandEffectsHook != null)
                {
                    foreach (var callback in AddCustomSoftLandEffectsHook.GetInvocationList())
                    {
                        if (callback == null)
                            continue;
                        var (enviromentType, softLandEffects) = ((int enviromentType, GameObject softLandEffects)) callback.DynamicInvoke(self);
                        AddSoftLandEffects(enviromentType, softLandEffects);
                    }
                }
                GameObject softLandEffectGo = _customSoftLandEffects[_pd.GetInt("environmentType")];
                self.dustEffects.SetActive(false);

                softLandEffectGo.transform.SetParent(self.transform);
                softLandEffectGo.transform.localPosition = Vector3.zero;
                softLandEffectGo.SetActive(true);
                //Log($"soft land effect activated: \"{softLandEffectGo}\", at: {softLandEffectGo.transform.position}");
            }
            catch (Exception)
            {}
        }
        private static void OnEventRegisterAwake(On.EventRegister.orig_Awake orig, EventRegister self)
        {
            orig(self);

            if (self.GetAttr<EventRegister, string>("subscribedEvent") == "ENVIRO UPDATE")
            {
                var fsm = self.gameObject.LocateMyFSM("Run Effects");

                if (fsm == null)
                    return;

                FsmState state = fsm.GetState("Check Enviro");

                foreach (var tmpRep in _customRunEffectsPrefabs)
                {
                    int enviromentType = tmpRep.Key;
                    string customEventName = $"CUSTOM_{enviromentType}";
                    FsmEvent newFsmEvent = FsmEvent.GetFsmEvent(customEventName);

                    var intSwitchAction = fsm.GetAction<IntSwitch>("Check Enviro", 5);
                    FsmInt tmpFsmInt = new FsmInt(customEventName) { Value = enviromentType };

                    List<FsmInt> tmpFi = new List<FsmInt>(intSwitchAction.compareTo) { tmpFsmInt };
                    intSwitchAction.compareTo = tmpFi.ToArray();

                    List<FsmEvent> tmpFe = new List<FsmEvent>(intSwitchAction.sendEvent) { newFsmEvent };
                    intSwitchAction.sendEvent = tmpFe.ToArray();

                    fsm.AddTransition("Check Enviro", customEventName, _customRunEffectsPrefabs[enviromentType]);
                }
                if (AddCustomRunEffectsHook != null)
                {
                    foreach (var callback in AddCustomRunEffectsHook.GetInvocationList())
                    {
                        if (callback == null)
                            continue;
                        var (enviromentType, runEffects) = ((int enviromentType, GameObject runEffects)) callback.DynamicInvoke(self.gameObject);
                        AddRunEffects(enviromentType, runEffects);
                    }
                }
                foreach (var tmpRe in _customRunEffects)
                {
                    var tmpGoUnused = Object.Instantiate(tmpRe.Value, self.gameObject.transform);
                    tmpGoUnused.name = $"{tmpRe.Key}";
                    Object.DontDestroyOnLoad(tmpGoUnused);
                    tmpGoUnused.transform.SetParent(self.gameObject.transform);

                    int enviromentType = tmpRe.Key;
                    string customEventName = $"CUSTOM_{enviromentType}";
                    string customStateName = $"CUSTOMSTATE_{enviromentType}";
                    FsmEvent newFsmEvent = FsmEvent.GetFsmEvent(customEventName);

                    fsm.AddGameObjectVariable($"Custom_{enviromentType}");

                    fsm.CopyState("Grass", customStateName);

                    #region Edit FindChild Action
                    fsm.GetAction<FindChild>(customStateName, 0).childName = $"{enviromentType}";
                    #endregion

                    var intSwitchAction = fsm.GetAction<IntSwitch>("Check Enviro", 5);
                    FsmInt tmpFsmInt = new FsmInt(customEventName) { Value = enviromentType };

                    List<FsmInt> tmpFi = new List<FsmInt>(intSwitchAction.compareTo) { tmpFsmInt };
                    intSwitchAction.compareTo = tmpFi.ToArray();

                    List<FsmEvent> tmpFe = new List<FsmEvent>(intSwitchAction.sendEvent) { newFsmEvent };
                    intSwitchAction.sendEvent = tmpFe.ToArray();

                    fsm.AddTransition("Check Enviro", customEventName, customStateName);
                }
            }
        }
        #endregion

        #region Add custom GameObjects
        /// <summary>
        ///     Hook to add custom walk audio.
        /// </summary>
        /// <param name="self">active HeroController</param>
        /// <returns>Tuple of the enviromentType you want to add and AudioClip of the sound you want</returns>
        public delegate (int enviromentType, AudioClip walkAudio) CustomWalkAudioHook(HeroController self);
        /// <summary>
        ///     Hook to add custom walk audio.
        /// </summary>
        /// <returns>Tuple of the enviromentType you want to add and AudioClip of the sound you want</returns>
        public static CustomWalkAudioHook AddCustomWalkAudioHook;
        /// <summary>
        ///     Hook to add custom run audio.
        /// </summary>
        /// <param name="self">active HeroController</param>
        /// <returns>Tuple of the enviromentType you want to add and AudioClip of the sound you want</returns>
        public delegate (int enviromentType, AudioClip runAudio) CustomRunAudioHook(HeroController self);
        /// <summary>
        ///     Hook to add custom run audio.
        /// </summary>
        /// <returns>Tuple of the enviromentType you want to add and AudioClip of the sound you want</returns>
        public static CustomRunAudioHook AddCustomRunAudioHook;
        /// <summary>
        ///     Hook to add custom dash effects.
        /// </summary>
        /// <param name="self">active DashEffect</param>
        /// <returns>Tuple of the enviromentType you want to add and GameObject of the dash effects you want</returns>
        public delegate (int enviromentType, GameObject dashEffects) CustomDashEffectsHook(DashEffect self);
        /// <summary>
        ///     Hook to add custom dash effects.
        /// </summary>
        /// <returns>Tuple of the enviromentType you want to add and GameObject of the dash effects you want</returns>
        public static CustomDashEffectsHook AddCustomDashEffectsHook;
        /// <summary>
        ///     Hook to add custom hard land effects.
        /// </summary>
        /// <param name="self">active HardLandEffect</param>
        /// <returns>Tuple of the enviromentType you want to add and GameObject of the hard land effects you want</returns>
        public delegate (int enviromentType, GameObject hardLandEffects) CustomHardLandEffectsHook(HardLandEffect self);
        /// <summary>
        ///     Hook to add custom hard land effects.
        /// </summary>
        /// <returns>Tuple of the enviromentType you want to add and GameObject of the hard land effects you want</returns>
        public static CustomHardLandEffectsHook AddCustomHardLandEffectsHook;
        /// <inheritdoc />
        /// <summary>
        ///     Hook to add custom jump effects.
        /// </summary>
        /// <param name="self">active JumpEffects</param>
        /// <returns>Tuple of the enviromentType you want to add and GameObject of the jump effects you want</returns>
        public delegate (int enviromentType, GameObject jumpEffects) CustomJumpEffectsHook(JumpEffects self);
        /// <inheritdoc />
        /// <summary>
        ///     Hook to add custom jump effects.
        /// </summary>
        /// <returns>Tuple of the enviromentType you want to add and GameObject of the jump effects you want</returns>
        public static CustomJumpEffectsHook AddCustomJumpEffectsHook;
        /// <summary>
        ///     Hook to add custom soft land effects.
        /// </summary>
        /// <param name="self">active SoftLandEffect</param>
        /// <returns>Tuple of the enviromentType you want to add and GameObject of the soft land effects you want</returns>
        public delegate (int enviromentType, GameObject softLandEffects) CustomSoftLandEffectsHook(SoftLandEffect self);
        /// <summary>
        ///     Hook to add custom soft land effects.
        /// </summary>
        /// <returns>Tuple of the enviromentType you want to add and GameObject of the soft land effects you want</returns>
        public static CustomSoftLandEffectsHook AddCustomSoftLandEffectsHook;
        /// <summary>
        ///     Hook to add custom run effects.
        /// </summary>
        /// <param name="runEffectsGo">active run effects GameObject</param>
        /// <returns>Tuple of the enviromentType you want to add and GameObject of the run effects you want</returns>
        public delegate (int enviromentType, GameObject runEffects) CustomRunEffectsHook(GameObject runEffectsGo);
        /// <summary>
        ///     Hook to add custom run effects.
        /// </summary>
        /// <returns>Tuple of the enviromentType you want to add and GameObject of the run effects you want</returns>
        public static CustomRunEffectsHook AddCustomRunEffectsHook;

        /// <summary>
        ///     Adds custom walk audio.
        /// </summary>
        /// <param name="enviromentType">Enviroment type to add the custom content to</param>
        /// <param name="walkAudio">The custom content</param>
        public static void AddWalkAudio(int enviromentType, AudioClip walkAudio)
        {
            if ((enviromentType >= 0) && (enviromentType < 8))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 7 are allowed");
            }
            if (!_customWalkAudio.ContainsKey(enviromentType))
            {
                var tmp = Object.Instantiate(walkAudio);
                SetInactive(tmp);
                _customWalkAudio.Add(enviromentType, tmp);
            }
        }
        /// <inheritdoc />
        /// <summary>
        ///     Adds custom walk audio.
        /// </summary>
        /// <param name="enviromentType">Enviroment type to add the custom content to</param>
        /// <param name="runAudio">The custom content</param>
        public static void AddRunAudio(int enviromentType, AudioClip runAudio)
        {
            if ((enviromentType >= 0) && (enviromentType < 8))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 7 are allowed");
            }
            if (!_customRunAudio.ContainsKey(enviromentType))
            {
                var tmp = Object.Instantiate(runAudio);
                SetInactive(tmp);
                _customRunAudio.Add(enviromentType, tmp);
            }
        }
        /// <inheritdoc />
        /// <summary>
        ///     Adds custom walk audio.
        /// </summary>
        /// <param name="enviromentType">Enviroment type to add the custom content to</param>
        /// <param name="dashEffects">The custom content</param>
        public static void AddDashEffects(int enviromentType, GameObject dashEffects)
        {
            if ((enviromentType >= 0) && (enviromentType < 7))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 6 are allowed");
            }
            if (!_customDashEffects.ContainsKey(enviromentType))
            {
                var tmp = Object.Instantiate(dashEffects);
                SetInactive(tmp);
                _customDashEffects.Add(enviromentType, tmp);
            }
        }
        /// <inheritdoc />
        /// <summary>
        ///     Adds custom walk audio.
        /// </summary>
        /// <param name="enviromentType">Enviroment type to add the custom content to</param>
        /// <param name="hardLandEffects">The custom content</param>
        public static void AddHardLandEffects(int enviromentType, GameObject hardLandEffects)
        {
            if ((enviromentType >= 0) && (enviromentType < 7))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 6 are allowed");
            }
            if (!_customHardLandEffects.ContainsKey(enviromentType))
            {
                var tmp = Object.Instantiate(hardLandEffects);
                SetInactive(tmp);
                _customHardLandEffects.Add(enviromentType, tmp);
            }
        }
        /// <inheritdoc />
        /// <summary>
        ///     Adds custom walk audio.
        /// </summary>
        /// <param name="enviromentType">Enviroment type to add the custom content to</param>
        /// <param name="jumpEffects">The custom content</param>
        public static void AddJumpEffects(int enviromentType, GameObject jumpEffects)
        {
            if ((enviromentType >= 0) && (enviromentType < 7))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 6 are allowed");
            }
            if (!_customJumpEffects.ContainsKey(enviromentType))
            {
                var tmp = Object.Instantiate(jumpEffects);
                SetInactive(tmp);
                _customJumpEffects.Add(enviromentType, tmp);
            }
        }
        /// <inheritdoc />
        /// <summary>
        ///     Adds custom walk audio.
        /// </summary>
        /// <param name="enviromentType">Enviroment type to add the custom content to</param>
        /// <param name="softLandEffects">The custom content</param>
        public static void AddSoftLandEffects(int enviromentType, GameObject softLandEffects)
        {
            if ((enviromentType >= 0) && (enviromentType < 7))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 6 are allowed");
            }
            if (!_customSoftLandEffects.ContainsKey(enviromentType))
            {
                var tmp = Object.Instantiate(softLandEffects);
                SetInactive(tmp);
                _customSoftLandEffects.Add(enviromentType, tmp);
            }
        }
        /// <inheritdoc />
        /// <summary>
        ///     Adds custom walk audio.
        /// </summary>
        /// <param name="enviromentType">Enviroment type to add the custom content to</param>
        /// <param name="runEffects">The custom content</param>
        public static void AddRunEffects(int enviromentType, GameObject runEffects)
        {
            if ((enviromentType >= 0) && (enviromentType < 7))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 6 are allowed");
            }
            if (!_customRunEffects.ContainsKey(enviromentType))
            {
                var tmp = Object.Instantiate(runEffects);
                SetInactive(tmp);
                tmp.SetActive(true);
                tmp.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
                _customRunEffects.Add(enviromentType, tmp);
            }
        }
        /// <inheritdoc />
        /// <summary>
        ///     Adds custom walk audio.
        /// </summary>
        /// <param name="enviromentType">Enviroment type to add the custom content to</param>
        /// <param name="runEffectsPrefab">Existing run effects you want to have in your custom enviroment</param>
        public static void AddRunEffects(int enviromentType, string runEffectsPrefab)
        {

            if ((enviromentType >= 0) && (enviromentType < 7))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 6 are allowed");
            }
            if (!_customRunEffectsPrefabs.ContainsKey(enviromentType))
            {
                _customRunEffectsPrefabs.Add(enviromentType, runEffectsPrefab);
            }
        }
        #endregion

        private static void SetInactive(Object go)
        {
            if (go != null)
            {
                Object.DontDestroyOnLoad(go);
            }
        }
        private static void SetInactive(GameObject go)
        {
            if (go != null)
            {
                Object.DontDestroyOnLoad(go);
                go.SetActive(false);
            }
        }
    }
}
