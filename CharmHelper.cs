using System;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Logger = Modding.Logger;
using SFCore.Utils;
using Object = UnityEngine.Object;

namespace SFCore
{
    /// <summary>
    ///     Charm helper class for easily adding custom charms.
    ///     The mod using this needs to handle the following:
    ///     - "CHARM_NAME_{ID}" Language string(s)
    ///     - "CHARM_DESC_{ID}" Language string(s)
    ///     - "gotCharm_{ID}" PlayerData bool(s)
    ///     - "newCharm_{ID}" PlayerData bool(s)
    ///     - "equippedCharm_{ID}" PlayerData bool(s)
    ///     - "charmCost_{ID}" PlayerData int(s)
    /// </summary>
    public static class CharmHelper
    {
        /// <summary>
        ///     List of sprites to use for the charms.
        /// </summary>
        private static readonly List<Sprite> CustomSprites = new List<Sprite>();

        /// <summary>
        ///     A detour for a private method that has no body.
        /// </summary>
        private static MonoMod.RuntimeDetour.Detour BuildEquippedCharms_Start_hook;

        /// <summary>
        ///     Constructs the mod and hooks important functions.
        /// </summary>
        static CharmHelper()
        {
            On.PlayerData.CalculateNotchesUsed += OnPlayerDataCalculateNotchesUsed;
            // i hate this, can't have shit in detroid
            //On.BuildEquippedCharms.Start += OnBuildEquippedCharmsStart;
            // workaround, since above isn't working
            BuildEquippedCharms_Start_hook = new MonoMod.RuntimeDetour.Detour(typeof(BuildEquippedCharms).GetMethod("Start", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic), typeof(CharmHelper).GetMethod(nameof(OnBuildEquippedCharmsStart_single), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic));
            BuildEquippedCharms_Start_hook.Apply();
            On.GameManager.Start += OnGameManagerStart;
            ModHooks.GetPlayerVariableHook += ModHooksOnGetPlayerVariableHook;
        }

        private static object ModHooksOnGetPlayerVariableHook(Type type, string name, object value)
        {
            if (type == typeof(List<int>) && name == "equippedCharms")
            {
                List<int> charms = value as List<int>;
                List<int> actualCharms = new();
                foreach (var charmId in charms)
                {
                    if (charmId <= 40)
                    {
                        actualCharms.Add(charmId);
                        continue;
                    }
                    if (PlayerData.instance.GetBool($"gotCharm_{charmId}"))
                    {
                        // only when charm gotten, charm in equipped list
                        actualCharms.Add(charmId);
                    }
                }
                for (int charmId = 41; charmId <= 40 + SFCoreMod.GlobalSettings.MaxCustomCharms; charmId++)
                {
                    if (!PlayerData.instance.GetBool($"gotCharm_{charmId}"))
                    {
                        // when charm not gotten, charm not equipped, simple as that
                        PlayerData.instance.SetBool($"equippedCharm_{charmId}", false);
                    }
                }
                return actualCharms;
            }
            return value;
        }

        /// <summary>
        ///     Used for static initialization.
        /// </summary>
        public static void unusedInit() { }

        /// <summary>
        ///     Adds a list of sprites as charms.
        /// </summary>
        public static List<int> AddSprites(params Sprite[] charmSprites)
        {
            List<int> ret = new List<int>();
            foreach (var spr in charmSprites)
            {
                SetInactive(spr);
                CustomSprites.Add(spr);
                ret.Add(40 + CustomSprites.Count);
            }
            return ret;
        }

        /// <summary>
        ///     On hook to indicate that the custom charms need to be readded.
        /// </summary>
        private static void OnGameManagerStart(On.GameManager.orig_Start orig, GameManager self)
        {
            SFCoreMod.GlobalSettings.MaxCustomCharms = Mathf.Max(SFCoreMod.GlobalSettings.MaxCustomCharms, CustomSprites.Count);
            orig(self);

            ClearModdedCharms();
        }

        /// <summary>
        ///     Adds custom charms to the charm board.
        /// </summary>
        private static void ClearModdedCharms()
        {
            #region CharmIconList Start

            var invGo = GameCameras.instance.hudCamera.gameObject.Find("Inventory");
            var charmsGo = invGo.Find("Charms");
            var charmsFsm = charmsGo.LocateMyFSM("UI Charms");

            var tmpCollectedCharmsGo = charmsGo.Find("Collected Charms");

            CharmIconList cil = CharmIconList.Instance;
            List<Sprite> tmpSpriteList = new List<Sprite>(cil.spriteList);
            tmpSpriteList.RemoveRange(41, tmpSpriteList.Count - 41);
            cil.spriteList = tmpSpriteList.ToArray();
            
            int rows = 4;

            #region Down State Editing

            charmsFsm.GetAction<IntCompare>("Down", 0).integer2.Value = (rows - 1) * 10;

            #endregion
            #region Left State Editing

            List<FsmInt> leftCharms = new List<FsmInt>();
            List<FsmEvent> switchEventsLeft = new List<FsmEvent>();
            for (int i = 1; i <= (rows * 10); i += 10)
            {
                leftCharms.Add(i);
                switchEventsLeft.Add(FsmEvent.FindEvent("TO LEFT"));
            }
            charmsFsm.GetAction<IntSwitch>("Left", 0).compareTo = leftCharms.ToArray();
            charmsFsm.GetAction<IntSwitch>("Left", 0).sendEvent = switchEventsLeft.ToArray();

            #endregion
            #region Right State Editing

            List<FsmInt> rightCharms = new List<FsmInt>();
            List<FsmEvent> switchEventsRight = new List<FsmEvent>();
            for (int i = 10; i <= (rows * 10); i += 10)
            {
                rightCharms.Add(i);
                switchEventsRight.Add(FsmEvent.FindEvent("TO RIGHT"));
            }
            charmsFsm.GetAction<IntSwitch>("Right", 0).compareTo = rightCharms.ToArray();
            charmsFsm.GetAction<IntSwitch>("Right", 0).sendEvent = switchEventsRight.ToArray();

            #endregion
            #region Up State Editing

            // Nothing to do

            #endregion
            #region Update Cursor State Editing

            charmsFsm.GetAction<IntClamp>("Update Cursor", 1).maxValue = rows * 10;

            #endregion

            #region Edit Backboards

            var backboardsGo = charmsGo.Find("Backboards");
            for (int i = tmpCollectedCharmsGo.transform.childCount; i >= 41; i--)
            {
                if (backboardsGo.Find($"BB {i}") != null)
                {
                    Object.DestroyImmediate(backboardsGo.Find($"BB {i}"));
                }
            }

            #endregion
            #region Edit Collected Charms

            var collectedCharmsGo = charmsGo.Find("Collected Charms");
            for (int i = tmpCollectedCharmsGo.transform.childCount; i >= 41; i--)
            {
                if (collectedCharmsGo.Find(i.ToString()) != null)
                {
                    Object.DestroyImmediate(collectedCharmsGo.Find(i.ToString()));
                }
            }

            #endregion
            
            // Vanilla has 4 rows
            float rowDeltaMultiplicator = (float) Mathf.Ceil((40f + CustomSprites.Count) / 10f) / 4f;
            GameObject backBoardTile, collectedCharmTile;
            Vector3 bbOldPos, ccOldPos;
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    backBoardTile = backboardsGo.Find("BB " + ((10 * i) + j));
                    if (backBoardTile != null)
                    {
                        bbOldPos = backBoardTile.transform.localPosition;
                        float y = ((bbOldPos.y - (-8.37f)) * rowDeltaMultiplicator) + (-8.37f);
                    }
                    collectedCharmTile = collectedCharmsGo.Find(((10 * i) + j).ToString());
                    if (collectedCharmTile != null)
                    {
                        ccOldPos = collectedCharmTile.transform.localPosition;
                        float y = ((ccOldPos.y - (-8.37f)) * rowDeltaMultiplicator) + (-8.37f);
                    }
                }
            }

            #endregion
        }
        
        /// <summary>
        ///     Adds custom charms to the charm board.
        /// </summary>
        private static void Init()
        {
            #region CharmIconList Start

            var invGo = GameCameras.instance.hudCamera.gameObject.Find("Inventory");
            var charmsGo = invGo.Find("Charms");
            var charmsFsm = charmsGo.LocateMyFSM("UI Charms");

            var tmpCollectedCharmsGo = charmsGo.Find("Collected Charms");
            int numCharms = tmpCollectedCharmsGo.transform.childCount - 1;

            int finalCharmAmount = numCharms + CustomSprites.Count;

            CharmIconList cil = CharmIconList.Instance;
            List<Sprite> tmpSpriteList = new List<Sprite>(cil.spriteList);
            for (int i = 0; i < CustomSprites.Count; i++)
            {
                SetInactive(CustomSprites[i]);
                tmpSpriteList.Add(CustomSprites[i]);
            }
            cil.spriteList = tmpSpriteList.ToArray();

            int rows = Mathf.CeilToInt(finalCharmAmount / 10.0f);

            #region Down State Editing

            charmsFsm.GetAction<IntCompare>("Down", 0).integer2.Value = (rows - 1) * 10;

            #endregion
            #region Left State Editing

            List<FsmInt> leftCharms = new List<FsmInt>();
            List<FsmEvent> switchEventsLeft = new List<FsmEvent>();
            for (int i = 1; i <= (rows * 10); i += 10)
            {
                leftCharms.Add(i);
                switchEventsLeft.Add(FsmEvent.FindEvent("TO LEFT"));
            }
            charmsFsm.GetAction<IntSwitch>("Left", 0).compareTo = leftCharms.ToArray();
            charmsFsm.GetAction<IntSwitch>("Left", 0).sendEvent = switchEventsLeft.ToArray();

            #endregion
            #region Right State Editing

            List<FsmInt> rightCharms = new List<FsmInt>();
            List<FsmEvent> switchEventsRight = new List<FsmEvent>();
            for (int i = 10; i <= (rows * 10); i += 10)
            {
                rightCharms.Add(i);
                switchEventsRight.Add(FsmEvent.FindEvent("TO RIGHT"));
            }
            charmsFsm.GetAction<IntSwitch>("Right", 0).compareTo = rightCharms.ToArray();
            charmsFsm.GetAction<IntSwitch>("Right", 0).sendEvent = switchEventsRight.ToArray();

            #endregion
            #region Up State Editing

            // Nothing to do

            #endregion
            #region Update Cursor State Editing

            charmsFsm.GetAction<IntClamp>("Update Cursor", 1).maxValue = rows * 10;

            #endregion

            #region Edit Backboards

            var backboardsGo = charmsGo.Find("Backboards");
            GameObject bbPrefab;
            int bbPrefabNum;
            GameObject bbT1, bbT2;
            for (int i = (numCharms + 1); i <= (rows * 10); i++)
            {
                bbPrefabNum = ((i - 1) % 20) + 21;
                if (backboardsGo.Find($"BB {i}") == null)
                {
                    bbT1 = backboardsGo.Find($"BB {bbPrefabNum}");
                    bbT2 = backboardsGo.Find($"BB {bbPrefabNum - 20}");
                    bbPrefab = Object.Instantiate(bbT1, backboardsGo.transform, true);
                    AddToCharmFadeGroup(bbPrefab, backboardsGo.transform.parent.gameObject);
                    AddToCharmFadeGroup(bbPrefab.Find("New Item Orb"), backboardsGo.transform.parent.gameObject);
                    bbPrefab.transform.localPosition = bbT1.transform.localPosition + ((bbT1.transform.localPosition - bbT2.transform.localPosition) * (((i / 10) - (bbPrefabNum / 10)) / 2));
                    bbPrefab.name = $"BB {i}";
                    bbPrefab.SetActive(true);

                    var icb = bbPrefab.GetComponent<InvCharmBackboard>();
                    icb.charmNum = i;
                    icb.charmNumString = i.ToString();
                    if (i > (40 + CustomSprites.Count))
                    {
                        icb.SetAttr("blanked", false);
                        icb.gotCharmString = "openingCreditsPlayed";
                    }
                    else
                    {
                        icb.SetAttr("blanked", true);
                    }
                    icb.gotCharmString = "gotCharm_" + i;
                    icb.newCharmString = "newCharm_" + i;
                }
            }

            #endregion
            #region Edit Collected Charms

            var collectedCharmsGo = charmsGo.Find("Collected Charms");
            GameObject ccPrefab;
            int ccPrefabNum;
            GameObject ccT1;
            PlayMakerFSM charmShowFsm;
            for (int i = numCharms + 1; i <= finalCharmAmount; i++)
            {
                ccPrefabNum = ((i - 1) % 20) + 21;
                //if (backboardsGo.Find(ccPrefabNum.ToString()) == null)
                if (collectedCharmsGo.Find(ccPrefabNum.ToString()) != null && collectedCharmsGo.Find(i.ToString()) == null)
                {
                    // Create new charm gameobject
                    ccT1 = collectedCharmsGo.Find(ccPrefabNum.ToString());
                    bool tmp = ccT1.activeSelf;
                    ccT1.gameObject.SetActive(true);
                    ccPrefab = Object.Instantiate(ccT1.gameObject, collectedCharmsGo.transform, true);
                    ccPrefab.SetActive(false);
                    AddToCharmFadeGroup(ccPrefab.Find("Sprite"), collectedCharmsGo.transform.parent.gameObject);
                    ccT1.gameObject.SetActive(tmp);
                    ccPrefab.name = i.ToString();

                    charmShowFsm = ccPrefab.LocateMyFSM("charm_show_if_collected");
                    var ccFsmVars = charmShowFsm.FsmVariables;
                    ccFsmVars.GetFsmInt("ID").Value = i;
                    ccFsmVars.GetFsmString("Name").Value = $"Charm {i}";
                    ccFsmVars.GetFsmString("PD Bool Name").Value = "gotCharm_" + i;
                    ccFsmVars.GetFsmGameObject("Charm Sprite").Value = ccPrefab.Find("Sprite");
                    ccFsmVars.GetFsmGameObject("Glow").Value = ccPrefab.Find("Glow");
                    ccPrefab.SetActive(true);
                }
                else if (collectedCharmsGo.Find(i.ToString()) != null)
                {
                    // Use already existing one and just change stuff
                    ccPrefab = collectedCharmsGo.Find(i.ToString());
                    ccPrefab.SetActive(false);
                    AddToCharmFadeGroup(ccPrefab.Find("Sprite"), collectedCharmsGo.transform.parent.gameObject);
                    ccPrefab.name = i.ToString();

                    charmShowFsm = ccPrefab.LocateMyFSM("charm_show_if_collected");
                    var ccFsmVars = charmShowFsm.FsmVariables;
                    ccFsmVars.GetFsmInt("ID").Value = i;
                    ccFsmVars.GetFsmString("Name").Value = $"Charm {i}";
                    ccFsmVars.GetFsmString("PD Bool Name").Value = "gotCharm_" + i;
                    ccFsmVars.GetFsmGameObject("Charm Sprite").Value = ccPrefab.Find("Sprite");
                    ccFsmVars.GetFsmGameObject("Glow").Value = ccPrefab.Find("Glow");
                    ccPrefab.SetActive(true);
                }
            }

            #endregion

            // Shift rows
            float rowMultiplicator = Mathf.Ceil(((float) numCharms / 10.0f) / ((float) rows));
            // Vanilla has 4 rows
            float rowDeltaMultiplicator = 4.0f / ((float) rows);
            GameObject backBoardTile, collectedCharmTile;
            Vector3 bbOldPos, ccOldPos;
            for (int i = 0; i <= rows; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    backBoardTile = backboardsGo.Find("BB " + ((10 * i) + j));
                    if (backBoardTile != null)
                    {
                        bbOldPos = backBoardTile.transform.localPosition;
                        float y = ((bbOldPos.y - (-8.37f)) * rowDeltaMultiplicator) + (-8.37f);
                        backBoardTile.transform.localPosition = new Vector3(bbOldPos.x, y, bbOldPos.z);
                        if (((10 * i) + j) > finalCharmAmount)
                        {
                            backBoardTile.GetComponent<SpriteRenderer>().color = new Color(116f / 255f, 116f / 255f, 116f / 255f, 0f / 255f);
                        }
                        else
                        {
                            backBoardTile.GetComponent<SpriteRenderer>().color = new Color(116f / 255f, 116f / 255f, 116f / 255f, 255f / 255f);
                        }
                    }
                    collectedCharmTile = collectedCharmsGo.Find(((10 * i) + j).ToString());
                    if (collectedCharmTile != null)
                    {
                        ccOldPos = collectedCharmTile.transform.localPosition;
                        if (i <= 3)
                        {
                            float y = ((ccOldPos.y - (-8.34f)) * rowDeltaMultiplicator) + (-8.34f);
                            collectedCharmTile.transform.localPosition = new Vector3(ccOldPos.x, y, ccOldPos.z);
                        }
                        else
                        {
                            float xLeft = ((i % 2) == 0) ? -7.92f : -7.01f;
                            float y = (-8.37f) - (i * 1.42f * rowDeltaMultiplicator);
                            collectedCharmTile.transform.localPosition = new Vector3(xLeft + ((j - 1) * 1.5f), y, ccOldPos.z);
                        }
                    }
                }
            }

            #endregion
        }
        
        /// <summary>
        ///     Adds charm cost of custom charms to notches.
        /// </summary>
        private static void OnPlayerDataCalculateNotchesUsed(On.PlayerData.orig_CalculateNotchesUsed orig, PlayerData self)
        {
            orig(self);
            int num = 0;
            for (int i = 0; i < CustomSprites.Count; i++)
            {
                if (self.GetBool("equippedCharm_" + (41 + i)))
                {
                    num += self.GetInt("charmCost_" + (41 + i));
                }
            }
            self.SetInt("charmSlotsFilled", self.GetInt("charmSlotsFilled") + num);
        }
        
        /// <summary>
        ///     Initializes equipped charms.
        /// </summary>
        private static void InitBuildEquippedCharms(BuildEquippedCharms self)
        {
            List<GameObject> tmplist = new List<GameObject>();

            #region Populate tmplist with all existing charms + custom charms as null values
            int max = 40 + CustomSprites.Count;
            for (int i = 0; i < max; i++)
            {
                if (i < self.gameObjectList.Count)
                    tmplist.Add(self.gameObjectList[i]);
                else
                    tmplist.Add(default(GameObject));
            }
            #endregion

            #region Fill nulls in tmplist with copies of tmplist[0], but adjusted Charm IDs
            for (int i = 41; i <= 40 + CustomSprites.Count; i++)
            {
                var equippedCharmPrefab = Object.Instantiate(tmplist[0]);

                SetInactive(equippedCharmPrefab);

                equippedCharmPrefab.name = $"{i}";

                var ci = equippedCharmPrefab.GetComponent<CharmItem>();

                var cd = equippedCharmPrefab.GetComponent<CharmDisplay>();
                cd.id = i;

                tmplist[i - 1] = equippedCharmPrefab;
            }
            #endregion

            self.gameObjectList = tmplist;
        }

        /// <summary>
        ///     On hook to initialize charms and equipped charms.
        /// </summary>
        private static void OnBuildEquippedCharmsStart_single()
        {
            Init();

            InitBuildEquippedCharms(GameObject.FindObjectOfType<BuildEquippedCharms>(true));
        }

        /// <summary>
        ///     On hook to initialize charms and equipped charms.
        /// </summary>
        private static void OnBuildEquippedCharmsStart(On.BuildEquippedCharms.orig_Start orig, BuildEquippedCharms self)
        {
            Init();

            orig(self);

            InitBuildEquippedCharms(self);
        }
        
        /// <summary>
        ///     Adds charm to fade group.
        /// </summary>
        private static void AddToCharmFadeGroup(GameObject spriteGo, GameObject fgGo)
        {
            var sr = spriteGo.GetComponent<SpriteRenderer>();
            sr.sortingLayerID = 629535577;
            var fg = fgGo.GetComponent<FadeGroup>();
            var srList = new List<SpriteRenderer>(fg.spriteRenderers);
            srList.Add(sr);
            fg.spriteRenderers = srList.ToArray();
        }
        
        /// <summary>
        ///     Makes a gameobject not be destroyed.
        /// </summary>
        private static void SetInactive(Object go)
        {
            if (go != null)
            {
                Object.DontDestroyOnLoad(go);
            }
        }

        private static void Log(string message)
        {
            Logger.LogDebug($"[SFCore]:[CharmHelper] - {message}");
            Debug.Log($"[SFCore]:[CharmHelper] - {message}");
        }
        private static void Log(object message)
        {
            Log($"{message}");
        }
    }
}
