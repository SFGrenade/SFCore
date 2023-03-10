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
        ///     A hook for a private method that has no body.
        /// </summary>
        private static MonoMod.RuntimeDetour.Hook BuildEquippedCharms_Start_hook;

        /// <summary>
        ///     A hook for a private method.
        /// </summary>
        private static MonoMod.RuntimeDetour.Hook GameCameras_Start_hook;

        /// <summary>
        ///     Constructs the mod and hooks important functions.
        /// </summary>
        static CharmHelper()
        {
            LogFine("!cctor");
            On.PlayerData.CalculateNotchesUsed += OnPlayerDataCalculateNotchesUsed;
            // i hate this, can't have shit in detroid
            //On.BuildEquippedCharms.Start += OnBuildEquippedCharmsStart;
            // workaround, since above isn't working
            BuildEquippedCharms_Start_hook = new MonoMod.RuntimeDetour.Hook(typeof(BuildEquippedCharms).GetMethod("Start", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic), typeof(CharmHelper).GetMethod(nameof(OnBuildEquippedCharmsStart_single), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic));
            BuildEquippedCharms_Start_hook.Apply();
            On.GameManager.Start += OnGameManagerStart;
            ModHooks.AfterSavegameLoadHook += ModHooksOnAfterSavegameLoadHook;
            ModHooks.BeforeSavegameSaveHook += ModHooksOnBeforeSavegameSaveHook;
            ModHooks.SavegameSaveHook += ModHooksOnSavegameSaveHook;

            GameCameras_Start_hook = new MonoMod.RuntimeDetour.Hook(typeof(GameCameras).GetMethod("Start", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic), typeof(CharmHelper).GetMethod(nameof(GameCamerasStart_single), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic));
            GameCameras_Start_hook.Apply();
            if (GameCameras.instance != null)
            {
                GameCamerasStart_single(cameras => { }, GameCameras.instance);
            }

            On.BuildEquippedCharms.BuildCharmList += (orig, self) =>
            {
                LogFine("!OnBuildEquippedCharmsBuildCharmList");
                // apparently BuildEquippedCharms.BuildCharmList() is called before BuildEquippedCharms.Start()
                if (self.gameObjectList.Count <= 40)
                {
                    InitBuildEquippedCharms(self);
                }
                orig(self);
                LogFine("~OnBuildEquippedCharmsBuildCharmList");
            };
            LogFine("~cctor");
        }

        private static void ModHooksOnSavegameSaveHook(int obj)
        {
            LogFine("!ModHooksOnSavegameSaveHook");
            AddCustomCharmsToPlayerData(PlayerData.instance);
            LogFine("~ModHooksOnSavegameSaveHook");
        }

        private static void ModHooksOnBeforeSavegameSaveHook(SaveGameData obj)
        {
            LogFine("!ModHooksOnBeforeSavegameSaveHook");
            RemoveCustomCharmsFromPlayerData(obj.playerData);
            LogFine("~ModHooksOnBeforeSavegameSaveHook");
        }

        private static void ModHooksOnAfterSavegameLoadHook(SaveGameData obj)
        {
            LogFine("!ModHooksOnAfterSavegameLoadHook");
            AddCustomCharmsToPlayerData(obj.playerData);
            LogFine("~ModHooksOnAfterSavegameLoadHook");
        }

        private static void RemoveCustomCharmsFromPlayerData(PlayerData pd)
        {
            LogFine("!RemoveCustomCharmsFromPlayerData");
            for (int i = pd.equippedCharms.Count - 1; i >= 0; i--)
            {
                if (pd.equippedCharms[i] > 40)
                {
                    // remove all custom charms (charmid > 40) from being saved
                    pd.equippedCharms.RemoveAt(i);
                }
            }
            LogFine("~RemoveCustomCharmsFromPlayerData");
        }

        private static void AddCustomCharmsToPlayerData(PlayerData pd)
        {
            LogFine("!AddCustomCharmsToPlayerData");
            for (int charmId = 41; charmId <= 40 + SFCoreMod.GlobalSettings.MaxCustomCharms; charmId++)
            {
                if (!pd.equippedCharms.Contains(charmId) &&
                    PlayerData.instance.GetBool($"gotCharm_{charmId}") &&
                    PlayerData.instance.GetBool($"equippedCharm_{charmId}"))
                {
                    // add custom charms after loading if they are acquired and equipped
                    pd.equippedCharms.Add(charmId);
                }
            }
            LogFine("~AddCustomCharmsToPlayerData");
        }

        /// <summary>
        ///     Used for static initialization.
        /// </summary>
        public static void unusedInit()
        {
            LogFine("!unusedInit");
            LogFine("~unusedInit");
        }

        /// <summary>
        ///     Adds a list of sprites as charms.
        /// </summary>
        public static List<int> AddSprites(params Sprite[] charmSprites)
        {
            LogFine("!AddSprites");
            LogDebug($"Adding {charmSprites.Length} charm sprites");
            List<int> ret = new List<int>();
            foreach (var spr in charmSprites)
            {
                SetInactive(spr);
                CustomSprites.Add(spr);
                ret.Add(40 + CustomSprites.Count);
            }
            SFCoreMod.GlobalSettings.MaxCustomCharms = Mathf.Max(SFCoreMod.GlobalSettings.MaxCustomCharms, CustomSprites.Count);
            LogFine("~AddSprites");
            return ret;
        }

        /// <summary>
        ///     On hook to indicate that the custom charms need to be readded.
        /// </summary>
        private static void OnGameManagerStart(On.GameManager.orig_Start orig, GameManager self)
        {
            LogFine("!OnGameManagerStart");
            SFCoreMod.GlobalSettings.MaxCustomCharms = Mathf.Max(SFCoreMod.GlobalSettings.MaxCustomCharms, CustomSprites.Count);
            orig(self);

            ClearModdedCharms();
            LogFine("~OnGameManagerStart");
        }

        /// <summary>
        ///     Adds custom charms to the charm board.
        /// </summary>
        private static void ClearModdedCharms()
        {
            LogFine("!ClearModdedCharms");
            #region CharmIconList Start

            var invGo = GameCameras.instance.hudCamera.gameObject.Find("Inventory");
            if (invGo == null) return;
            var charmsGo = invGo.Find("Charms");
            if (charmsGo == null) return;
            var charmsFsm = charmsGo.LocateMyFSM("UI Charms");
            if (charmsFsm == null) return;

            var tmpCollectedCharmsGo = charmsGo.Find("Collected Charms");

            CharmIconList cil = CharmIconList.Instance;
            if (cil == null) return;
            List<Sprite> tmpSpriteList = new List<Sprite>(cil.spriteList);
            if (tmpSpriteList.Count > 41)
            {
                tmpSpriteList.RemoveRange(41, tmpSpriteList.Count - 41);
                cil.spriteList = tmpSpriteList.ToArray();
            }

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
            float rowDeltaMultiplicator = (float)Mathf.Ceil((40f + CustomSprites.Count) / 10f) / 4f;
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
            LogFine("~ClearModdedCharms");
        }

        /// <summary>
        ///     Adds custom charms to the charm board.
        /// </summary>
        private static void Init()
        {
            LogFine("!Init");
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
                    bbT1 = backboardsGo.Find($"BB {bbPrefabNum}"); // "BB 21" - "BB 40"
                    bbT2 = backboardsGo.Find($"BB {bbPrefabNum - 20}"); // "BB 1" - "BB 20"
                    bbPrefab = Object.Instantiate(bbT1, backboardsGo.transform, true);
                    AddToCharmFadeGroup(bbPrefab, backboardsGo.transform.parent.gameObject);
                    AddToCharmFadeGroup(bbPrefab.Find("New Item Orb"), backboardsGo.transform.parent.gameObject);
                    bbPrefab.transform.localPosition = bbT1.transform.localPosition + ((bbT1.transform.localPosition - bbT2.transform.localPosition) * (((i / 10f) - (bbPrefabNum / 10f)) / 2f));
                    bbPrefab.name = $"BB {i}";
                    bbPrefab.SetActive(true);

                    var icb = bbPrefab.GetComponent<InvCharmBackboard>();
                    icb.charmNum = i;
                    icb.charmNumString = i.ToString();
                    if (i > (40 + CustomSprites.Count))
                    {
                        icb.SetAttr("blanked", false);
                        icb.gotCharmString = "";
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
            float rowMultiplicator = Mathf.Ceil(((float)numCharms / 10.0f) / ((float)rows));
            // Vanilla has 4 rows
            float rowDeltaMultiplicator = 4.0f / ((float)rows);
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
            LogFine("~Init");
        }

        /// <summary>
        ///     Adds charm cost of custom charms to notches.
        /// </summary>
        private static void OnPlayerDataCalculateNotchesUsed(On.PlayerData.orig_CalculateNotchesUsed orig, PlayerData self)
        {
            LogFine("!OnPlayerDataCalculateNotchesUsed");
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
            LogFine("~OnPlayerDataCalculateNotchesUsed");
        }

        /// <summary>
        ///     Initializes equipped charms.
        /// </summary>
        private static void InitBuildEquippedCharms(BuildEquippedCharms self)
        {
            LogFine("!InitBuildEquippedCharms");
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
            LogFine("~InitBuildEquippedCharms");
        }

        /// <summary>
        ///     On hook to initialize charms and equipped charms.
        /// </summary>
        private static void OnBuildEquippedCharmsStart_single(Action<BuildEquippedCharms> orig, BuildEquippedCharms self)
        {
            LogFine("!OnBuildEquippedCharmsStart_single");
            Init();

            orig(self);

            InitBuildEquippedCharms(self);
            LogFine("~OnBuildEquippedCharmsStart_single");
        }

        /// <summary>
        ///     On hook to add more detail cost notches.
        /// </summary>
        private static void GameCamerasStart_single(Action<GameCameras> orig, GameCameras self)
        {
            LogFine("!GameCamerasStart_single");
            orig(self);
            GameObject charmDetailCost = self.gameObject.Find("HudCamera").Find("Inventory").Find("Charms").Find("Details").Find("Cost");
            PlayMakerFSM charmDetailCostFsm = charmDetailCost.LocateMyFSM("Charm Details Cost");
            charmDetailCostFsm.Preprocess();
            for (int i = 7; i <= 20; i++)
            {
                string prefabCostName = $"Cost {i - 1}";
                string newCostName = $"Cost {i}";
                string newCostVariableName = $"C{i}";
                string newCostPositionVariableName = $"{i} X";

                GameObject charmDetailCostCopied = GameObject.Instantiate(charmDetailCost.Find(prefabCostName));
                charmDetailCostCopied.name = newCostName;
                charmDetailCostCopied.transform.localPosition = new Vector3(11.5f + (i * 0.75f), -3.54f, -5.63f);
                charmDetailCostCopied.transform.SetParent(charmDetailCost.transform, false);

                FsmGameObject newChildFsmVariable = charmDetailCostFsm.AddFsmGameObjectVariable(newCostVariableName);
                FindChild initFindChildAction = new FindChild();
                initFindChildAction.gameObject = charmDetailCostFsm.GetAction<FindChild>("Init", 0).gameObject;
                initFindChildAction.childName = newCostName;
                initFindChildAction.storeResult = newChildFsmVariable;
                charmDetailCostFsm.InsertAction("Init", initFindChildAction, i - 1);

                FsmFloat newPositionFsmVariable = charmDetailCostFsm.AddFsmFloatVariable(newCostPositionVariableName);
                newPositionFsmVariable.Value = (0.82f + 1.02f) - ((0.82f + 1.02f) * ((i - 1f) / 5f)) - 1.02f;

                IntSwitch checkIntSwitch = charmDetailCostFsm.GetAction<IntSwitch>("Check", 1);
                List<FsmInt> checkIntSwitchCompareToNewList = new List<FsmInt>(checkIntSwitch.compareTo);
                checkIntSwitchCompareToNewList.Add(i);
                checkIntSwitch.compareTo = checkIntSwitchCompareToNewList.ToArray();
                List<FsmEvent> checkIntSwitchSendEventNewList = new List<FsmEvent>(checkIntSwitch.sendEvent);
                checkIntSwitchSendEventNewList.Add(FsmEvent.GetFsmEvent($"{i}"));
                checkIntSwitch.sendEvent = checkIntSwitchSendEventNewList.ToArray();

                charmDetailCostFsm.CopyState(prefabCostName, newCostName);
                charmDetailCostFsm.AddTransition("Check", $"{i}", newCostName);
                charmDetailCostFsm.GetAction<SetPosition>(newCostName, 0).x = newPositionFsmVariable;

                FsmFloat absentFloatVariable = charmDetailCostFsm.FindFsmFloatVariable("Absent Y");
                FsmFloat presentFloatVariable = charmDetailCostFsm.FindFsmFloatVariable("Present Y");
                SetPosition costNewSetPositionPresent = new SetPosition
                {
                    gameObject = new FsmOwnerDefault()
                    {
                        OwnerOption = OwnerDefaultOption.SpecifyGameObject,
                        GameObject = newChildFsmVariable
                    },
                    vector = new FsmVector3()
                    {
                        Name = "",
                        UseVariable = true
                    },
                    x = new FsmFloat()
                    {
                        Name = "",
                        UseVariable = true
                    },
                    y = presentFloatVariable,
                    z = new FsmFloat()
                    {
                        Name = "",
                        UseVariable = true
                    },
                    space = Space.Self,
                    everyFrame = false,
                    lateUpdate = false
                };
                SetPosition costNewSetPositionAbsent = new SetPosition
                {
                    gameObject = new FsmOwnerDefault()
                    {
                        OwnerOption = OwnerDefaultOption.SpecifyGameObject,
                        GameObject = newChildFsmVariable
                    },
                    vector = new FsmVector3()
                    {
                        Name = "",
                        UseVariable = true
                    },
                    x = new FsmFloat()
                    {
                        Name = "",
                        UseVariable = true
                    },
                    y = absentFloatVariable,
                    z = new FsmFloat()
                    {
                        Name = "",
                        UseVariable = true
                    },
                    space = Space.Self,
                    everyFrame = false,
                    lateUpdate = false
                };

                for (int j = 0; j < i; j++)
                {
                    string tmpStateName = $"Cost {j}";
                    charmDetailCostFsm.AddAction(tmpStateName, costNewSetPositionAbsent);
                }
                charmDetailCostFsm.AddAction(newCostName, costNewSetPositionPresent);
            }
            LogFine("~GameCamerasStart_single");
        }

        /// <summary>
        ///     Adds charm to fade group.
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
        ///     Makes a gameobject not be destroyed.
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

        private static void LogFine(string message) => InternalLogger.LogFine(message, "[SFCore]:[CharmHelper]");
        private static void LogFine(object message) => LogFine($"{message}");
        private static void LogDebug(string message) => InternalLogger.LogDebug(message, "[SFCore]:[CharmHelper]");
        private static void LogDebug(object message) => LogDebug($"{message}");
        private static void Log(string message) => InternalLogger.Log(message, "[SFCore]:[CharmHelper]");
        private static void Log(object message) => Log($"{message}");
        private static void LogWarn(string message) => InternalLogger.LogWarn(message, "[SFCore]:[CharmHelper]");
        private static void LogWarn(object message) => LogWarn($"{message}");
        private static void LogError(string message) => InternalLogger.LogError(message, "[SFCore]:[CharmHelper]");
        private static void LogError(object message) => LogError($"{message}");
    }
}