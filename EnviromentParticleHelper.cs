using System;
using System.Collections.Generic;
using ModCommon.Util;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Logger = Modding.Logger;
using System.Collections;
using GlobalEnums;

namespace SFCore
{
    public class EnviromentParticleHelper
    {
        private static PlayerData pd;
        private static bool initialized = false;
        private static Dictionary<int, AudioClip> customWalkAudio = new Dictionary<int, AudioClip>();
        private static Dictionary<int, AudioClip> customRunAudio = new Dictionary<int, AudioClip>();
        private static Dictionary<int, GameObject> customDashEffects = new Dictionary<int, GameObject>();
        private static Dictionary<int, GameObject> customHardLandEffects = new Dictionary<int, GameObject>();
        private static Dictionary<int, GameObject> customJumpEffects = new Dictionary<int, GameObject>();
        private static Dictionary<int, GameObject> customSoftLandEffects = new Dictionary<int, GameObject>();

        private static Dictionary<int, GameObject> customRunEffects = new Dictionary<int, GameObject>();
        private static Dictionary<int, string> customRunEffectsPrefabs = new Dictionary<int, string>();

        public static void Init()
        {
            if (!initialized)
            {
                initialized = true;
                pd = PlayerData.instance;

                On.HeroController.checkEnvironment += OnHeroControllercheckEnvironment;
                On.DashEffect.OnEnable += OnDashEffectOnEnable;
                On.HardLandEffect.OnEnable += OnHardLandEffectOnEnable;
                On.JumpEffects.OnEnable += OnJumpEffectsOnEnable;
                On.SoftLandEffect.OnEnable += OnSoftLandEffectOnEnable;
                On.EventRegister.Awake += OnEventRegisterAwake;
                //GameManager.instance.StartCoroutine(asyncAddRunEffects());
            }
        }

        #region Hooks
        private static void OnHeroControllercheckEnvironment(On.HeroController.orig_checkEnvironment orig, HeroController self)
        {
            orig(self);

            try
            {
                self.footStepsRunAudioSource.clip = customRunAudio[pd.GetInt(nameof(pd.environmentType))];
            }
            catch (Exception)
            {}
            try
            {
                self.footStepsWalkAudioSource.clip = customWalkAudio[pd.GetInt(nameof(pd.environmentType))];
            }
            catch (Exception)
            {}
        }
        private static void OnDashEffectOnEnable(On.DashEffect.orig_OnEnable orig, DashEffect self)
        {
            orig(self);

            foreach (KeyValuePair<int, GameObject> tmp in customDashEffects)
            {
                tmp.Value.SetActive(false);
            }

            try
            {
                GameObject dashEffectGo = customDashEffects[pd.GetInt(nameof(pd.environmentType))];
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

            foreach (KeyValuePair<int, GameObject> tmp in customHardLandEffects)
            {
                tmp.Value.SetActive(false);
            }

            try
            {
                GameObject hardLandEffectGo = customHardLandEffects[pd.GetInt(nameof(pd.environmentType))];
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

            foreach (KeyValuePair<int, GameObject> tmp in customJumpEffects)
            {
                tmp.Value.SetActive(false);
            }

            try
            {
                GameObject jumpEffectGo = customJumpEffects[pd.GetInt(nameof(pd.environmentType))];
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

            foreach (KeyValuePair<int, GameObject> tmp in customSoftLandEffects)
            {
                tmp.Value.SetActive(false);
            }

            try
            {
                GameObject softLandEffectGo = customSoftLandEffects[pd.GetInt(nameof(pd.environmentType))];
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

                FsmState state = null;
                foreach (var tmpState in fsm.FsmStates)
                {
                    if (tmpState.Name == "Check Enviro")
                    {
                        state = tmpState;
                    }
                }

                foreach (var tmpREP in customRunEffectsPrefabs)
                {
                    int enviromentType = tmpREP.Key;
                    string customEventName = $"CUSTOM_{enviromentType}";
                    FsmEvent newFsmEvent = null;
                    var HGPMF = fsm.GetAttr<PlayMakerFSM, Fsm>("fsm");
                    if (HGPMF.GetEvent(customEventName) == null)
                    {
                        newFsmEvent = new FsmEvent(customEventName);
                        List<FsmEvent> events = new List<FsmEvent>(HGPMF.Events)
                        {
                            newFsmEvent
                        };
                        HGPMF.Events = events.ToArray();
                    }
                    else
                    {
                        newFsmEvent = HGPMF.GetEvent(customEventName);
                    }

                    var intSwitchAction = state.Actions[state.Actions.Length - 1] as IntSwitch;
                    List<FsmInt> tmpFI = new List<FsmInt>(intSwitchAction.compareTo);
                    FsmInt tmpFsmInt = new FsmInt(customEventName);
                    tmpFsmInt = enviromentType;
                    tmpFI.Add(tmpFsmInt);
                    intSwitchAction.compareTo = tmpFI.ToArray();

                    List<FsmEvent> tmpFE = new List<FsmEvent>(intSwitchAction.sendEvent)
                    {
                        newFsmEvent
                    };
                    intSwitchAction.sendEvent = tmpFE.ToArray();

                    List<FsmTransition> tmpFT = new List<FsmTransition>(fsm.GetState("Check Enviro").Transitions)
                    {
                        new FsmTransition() { FsmEvent = newFsmEvent, ToState = customRunEffectsPrefabs[enviromentType] }
                    };
                    fsm.GetState("Check Enviro").Transitions = tmpFT.ToArray();
                }
                foreach (var tmpRE in customRunEffects)
                {
                    var tmpGoUnused = GameObject.Instantiate(tmpRE.Value, self.gameObject.transform);
                    tmpGoUnused.name = $"{tmpRE.Key}";
                    SetInactive(tmpGoUnused);
                    tmpGoUnused.SetActive(true);
                    tmpGoUnused.transform.SetParent(self.gameObject.transform);

                    int enviromentType = tmpRE.Key;
                    string customEventName = $"CUSTOM_{enviromentType}";
                    string customStateName = $"CUSTOMSTATE_{enviromentType}";
                    FsmEvent newFsmEvent = null;
                    var HGPMF = fsm.GetAttr<PlayMakerFSM, Fsm>("fsm");
                    if (HGPMF.GetEvent(customEventName) == null)
                    {
                        newFsmEvent = new FsmEvent(customEventName);
                        List<FsmEvent> events = new List<FsmEvent>(HGPMF.Events)
                        {
                            newFsmEvent
                        };
                        HGPMF.Events = events.ToArray();
                    }
                    else
                    {
                        newFsmEvent = HGPMF.GetEvent(customEventName);
                    }

                    #region Add more variable
                    var fmsVars = fsm.FsmVariables;
                    List<FsmGameObject> goVars = new List<FsmGameObject>(fmsVars.GameObjectVariables);
                    goVars.Add(new FsmGameObject($"Custom_{enviromentType}"));
                    fmsVars.GameObjectVariables = goVars.ToArray();
                    #endregion

                    var customState = fsm.CopyState("Grass", customStateName);
                    #region Edit FindChild Action
                    (customState.Actions[0] as FindChild).childName = $"{enviromentType}";
                    #endregion

                    var intSwitchAction = state.Actions[state.Actions.Length - 1] as IntSwitch;
                    List<FsmInt> tmpFI = new List<FsmInt>(intSwitchAction.compareTo);
                    FsmInt tmpFsmInt = new FsmInt(customEventName);
                    tmpFsmInt = enviromentType;
                    tmpFI.Add(tmpFsmInt);
                    intSwitchAction.compareTo = tmpFI.ToArray();

                    List<FsmEvent> tmpFE = new List<FsmEvent>(intSwitchAction.sendEvent)
                    {
                        newFsmEvent
                    };
                    intSwitchAction.sendEvent = tmpFE.ToArray();

                    List<FsmTransition> tmpFT = new List<FsmTransition>(fsm.GetState("Check Enviro").Transitions)
                    {
                        new FsmTransition() { FsmEvent = newFsmEvent, ToState = customStateName }
                    };
                    fsm.GetState("Check Enviro").Transitions = tmpFT.ToArray();
                }
            }
        }
        #endregion

        #region Add custom GameObjects
        public static void addWalkAudio(int enviromentType, AudioClip walkAudio)
        {
            if ((enviromentType >= 0) && (enviromentType < 8))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 7 are allowed");
            }
            var tmp = AudioClip.Instantiate<AudioClip>(walkAudio);
            SetInactive(tmp);
            customWalkAudio.Add(enviromentType, tmp);
        }
        public static void addRunAudio(int enviromentType, AudioClip runAudio)
        {
            if ((enviromentType >= 0) && (enviromentType < 8))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 7 are allowed");
            }
            var tmp = AudioClip.Instantiate<AudioClip>(runAudio);
            SetInactive(tmp);
            customRunAudio.Add(enviromentType, tmp);
        }
        public static void addDashEffects(int enviromentType, GameObject dashEffects)
        {
            if ((enviromentType >= 0) && (enviromentType < 7))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 6 are allowed");
            }
            var tmp = GameObject.Instantiate(dashEffects);
            SetInactive(tmp);
            customDashEffects.Add(enviromentType, tmp);
        }
        public static void addHardLandEffects(int enviromentType, GameObject hardLandEffects)
        {
            if ((enviromentType >= 0) && (enviromentType < 7))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 6 are allowed");
            }
            var tmp = GameObject.Instantiate(hardLandEffects);
            SetInactive(tmp);
            customHardLandEffects.Add(enviromentType, tmp);
        }
        public static void addJumpEffects(int enviromentType, GameObject jumpEffects)
        {
            if ((enviromentType >= 0) && (enviromentType < 7))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 6 are allowed");
            }
            var tmp = GameObject.Instantiate(jumpEffects);
            SetInactive(tmp);
            customJumpEffects.Add(enviromentType, tmp);
        }
        public static void addSoftLandEffects(int enviromentType, GameObject softLandEffects)
        {
            if ((enviromentType >= 0) && (enviromentType < 7))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 6 are allowed");
            }
            var tmp = GameObject.Instantiate(softLandEffects);
            SetInactive(tmp);
            customSoftLandEffects.Add(enviromentType, tmp);
        }

        public static void addRunEffects(int enviromentType, GameObject runEffects)
        {

            if ((enviromentType >= 0) && (enviromentType < 7))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 6 are allowed");
            }
            var tmp = GameObject.Instantiate(runEffects);
            SetInactive(tmp);
            tmp.SetActive(true);
            tmp.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
            customRunEffects.Add(enviromentType, tmp);
        }
        public static void addRunEffects(int enviromentType, string runEffectsPrefab)
        {

            if ((enviromentType >= 0) && (enviromentType < 7))
            {
                throw new ArgumentOutOfRangeException("Only Integers smaller than 0 and larger than 6 are allowed");
            }
            customRunEffectsPrefabs.Add(enviromentType, runEffectsPrefab);
        }
        #endregion

        private static void SetInactive(UnityEngine.Object go)
        {
            if (go != null)
            {
                UnityEngine.Object.DontDestroyOnLoad(go);
            }
        }
        private static void SetInactive(GameObject go)
        {
            if (go != null)
            {
                UnityEngine.Object.DontDestroyOnLoad(go);
                go.SetActive(false);
            }
        }

        private static void Log(string message)
        {
            Logger.Log($"[SFCore]:[EnviromentParticleHelper] - {message}");
        }
        private static void Log(object message)
        {
            Logger.Log($"[SFCore]:[EnviromentParticleHelper] - {message.ToString()}");
        }


        private static void printDebugFsm(PlayMakerFSM fsm)
        {
            foreach (var state in fsm.FsmStates)
            {
                Log("State: " + state.Name);
                foreach (var trans in state.Transitions)
                {
                    Log("\t" + trans.EventName + " -> " + trans.ToState);
                }
            }
        }
        private static void printDebug(GameObject go, string tabindex = "", int parentCount = 0)
        {
            Transform parent = go.transform.parent;
            for (int i = 0; i < parentCount; i++)
            {
                if (parent != null)
                {
                    Log(tabindex + "DEBUG parent: " + parent.gameObject.name);
                    parent = parent.parent;
                }
            }
            Log(tabindex + "DEBUG Name: " + go.name);
            foreach (var comp in go.GetComponents<Component>())
            {
                Log(tabindex + "DEBUG Component: " + comp.GetType());
            }
            for (int i = 0; i < go.transform.childCount; i++)
            {
                printDebug(go.transform.GetChild(i).gameObject, tabindex + "\t");
            }
        }

    }
}
