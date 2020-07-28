using System;
using System.Collections.Generic;
using ModCommon.Util;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Logger = Modding.Logger;

namespace SFCore
{
    public class CharmHelper
    {
        /* 
         * CharmHelper
         * v 1.1.1.0
         */

        public List<int> charmIDs { get; private set; }
        public int customCharms;
        public Sprite[] customSprites;

        /*
         * Needed on user-part:
         * 
         * "CHARM_NAME_{ID}" Language string(s)
         * "CHARM_DESC_{ID}" Language string(s)
         * "gotCharm_{ID}" PlayerData bool(s)
         * "newCharm_{ID}" PlayerData bool(s)
         * "equippedCharm_{ID}" PlayerData bool(s)
         * "charmCost_{ID}" PlayerData int(s)
         * 
         * Set amount of custom charms wanted in customCharms int
         * Set sprites of custom charms wanted in customSprites Sprite[]
         * IDs can be pulled from the charmIDs list
         */

        public CharmHelper()
        {
            customCharms = 1;
            customSprites = new Sprite[]
            {
                new Sprite()
            };
            charmIDs = new List<int>();

            On.CharmIconList.Start += OnCharmIconListStart;
            On.PlayerData.CalculateNotchesUsed += OnPlayerDataCalculateNotchesUsed;
            On.BuildEquippedCharms.BuildCharmList += OnBuildEquippedCharmsBuildCharmList;
        }

        private void OnBuildEquippedCharmsBuildCharmList(On.BuildEquippedCharms.orig_BuildCharmList orig, BuildEquippedCharms self)
        {
            Log("!OnBECBCL");

            #region Edit BuildEquippedCharms
            int numCharms = self.gameObjectList.Count;
            for (int i = 0; i < customCharms; i++)
            {
                try
                {
                    var t = self.gameObjectList[numCharms + i];
                }
                catch
                {
                    var equippedCharmPrefab = GameObject.Instantiate(self.gameObjectList[0]);
                    SetInactive(equippedCharmPrefab);
                    equippedCharmPrefab.name = "Charm " + (numCharms + i + 1).ToString();

                    var ci = equippedCharmPrefab.GetComponent<CharmItem>();

                    var cd = equippedCharmPrefab.GetComponent<CharmDisplay>();
                    cd.id = (numCharms + i + 1);

                    self.gameObjectList.Add(equippedCharmPrefab);
                }
            }
            #endregion
            orig(self);

            Log("~OnBECBCL");
        }

        private void OnPlayerDataCalculateNotchesUsed(On.PlayerData.orig_CalculateNotchesUsed orig, PlayerData self)
        {
            Log("!OnPDCNU");

            orig(self);
            int num = 0;
            foreach (int i in this.charmIDs)
            {
                if (self.GetBool("equippedCharm_" + i))
                {
                    num += self.GetInt("charmCost_" + i);
                }
            }
            self.SetInt("charmSlotsFilled", self.GetInt("charmSlotsFilled") + num);

            Log("~OnPDCNU");
        }

        private void OnCharmIconListStart(On.CharmIconList.orig_Start orig, CharmIconList self)
        {
            Log("!OnCILS");

            var invGo = findChild(GameCameras.instance.hudCamera.gameObject, "Inventory").gameObject;
            var charmsGo = findChild(invGo, "Charms").gameObject;
            var charmsFsm = charmsGo.LocateMyFSM("UI Charms");

            var tmpCollectedCharmsGo = findChild(charmsGo, "Collected Charms").gameObject;
            int numCharms = tmpCollectedCharmsGo.transform.childCount - 1;
            Log("numCharms " + numCharms);

            int finalCharmAmount = numCharms + customCharms;

            CharmIconList cil = CharmIconList.Instance;
            Sprite[] tmpSpriteList = new Sprite[finalCharmAmount + 1];
            cil.spriteList.CopyTo(tmpSpriteList, 0);
            for (int i = 0; i < customCharms; i++)
            {
                tmpSpriteList[numCharms + i + 1] = customSprites[i % customSprites.Length];
                SetInactive(tmpSpriteList[numCharms + i + 1]);
                charmIDs.Add(numCharms + i + 1);
            }
            cil.spriteList = tmpSpriteList;

            int rows = (int)Math.Ceiling(finalCharmAmount / 10.0);

            #region Down State Editing
            var downState = charmsFsm.GetState("Down");
            (downState.Actions[0] as IntCompare).integer2.Value = (rows - 1) * 10;
            #endregion
            #region Left State Editing
            var leftState = charmsFsm.GetState("Left");
            List<FsmInt> leftCharms = new List<FsmInt>();
            List<FsmEvent> switchEventsLeft = new List<FsmEvent>();
            for (int i = 1; i <= finalCharmAmount; i += 10)
            {
                leftCharms.Add(i);
                switchEventsLeft.Add(FsmEvent.FindEvent("TO LEFT"));
            }
            (leftState.Actions[0] as IntSwitch).compareTo = leftCharms.ToArray();
            (leftState.Actions[0] as IntSwitch).sendEvent = switchEventsLeft.ToArray();
            #endregion
            #region Right State Editing
            var rightState = charmsFsm.GetState("Right");
            List<FsmInt> rightCharms = new List<FsmInt>();
            List<FsmEvent> switchEventsRight = new List<FsmEvent>();
            for (int i = 10; i <= finalCharmAmount; i += 10)
            {
                rightCharms.Add(i);
                switchEventsRight.Add(FsmEvent.FindEvent("TO RIGHT"));
            }
            (rightState.Actions[0] as IntSwitch).compareTo = rightCharms.ToArray();
            (rightState.Actions[0] as IntSwitch).sendEvent = switchEventsRight.ToArray();
            #endregion
            #region Up State Editing
            var upState = charmsFsm.GetState("Up");
            // Nothing to do
            #endregion
            #region Update Cursor State Editing
            var updateCursorState = charmsFsm.GetState("Update Cursor");
            (updateCursorState.Actions[1] as IntClamp).maxValue = rows * 10;
            #endregion

            #region Edit Backboards
            var backboardsGo = findChild(charmsGo, "Backboards").gameObject;
            GameObject bbPrefab;
            int bbPrefabNum;
            Transform bbT1, bbT2;
            for (int i = (numCharms + 1); i <= (rows * 10); i++)
            {
                bbPrefabNum = ((i - 1) % 20) + 21;
                if (findChild(backboardsGo, "BB " + i) == null)
                {
                    bbT1 = findChild(backboardsGo, "BB " + bbPrefabNum);
                    bbT2 = findChild(backboardsGo, "BB " + (bbPrefabNum - 20));
                    bbPrefab = GameObject.Instantiate(bbT1.gameObject, backboardsGo.transform, true);
                    bbPrefab.transform.localPosition = bbT1.localPosition + ((bbT1.localPosition - bbT2.localPosition) * (((i / 10) - (bbPrefabNum / 10)) / 2));
                    bbPrefab.name = "BB " + i;
                    bbPrefab.SetActive(true);

                    var icb = bbPrefab.GetComponent<InvCharmBackboard>();
                    icb.charmNum = i;
                    icb.charmNumString = i.ToString();
                    icb.gotCharmString = "gotCharm_" + i;
                    icb.newCharmString = "newCharm_" + i;
                }
            }
            #endregion
            #region Edit Collected Charms
            var collectedCharmsGo = findChild(charmsGo, "Collected Charms").gameObject;
            GameObject ccPrefab;
            int ccPrefabNum;
            Transform ccT1;
            PlayMakerFSM charmShowFsm;
            for (int i = numCharms + 1; i <= finalCharmAmount; i++)
            {
                ccPrefabNum = ((i - 1) % 20) + 21;
                if (findChild(backboardsGo, ccPrefabNum.ToString()) == null)
                {
                    ccT1 = findChild(collectedCharmsGo, ccPrefabNum.ToString());
                    bool tmp = ccT1.gameObject.activeSelf;
                    ccT1.gameObject.SetActive(true);
                    ccPrefab = GameObject.Instantiate(ccT1.gameObject, collectedCharmsGo.transform, true);
                    ccT1.gameObject.SetActive(tmp);
                    ccPrefab.SetActive(true);
                    ccPrefab.name = i.ToString();

                    charmShowFsm = ccPrefab.LocateMyFSM("charm_show_if_collected");
                    var ccFsmVars = charmShowFsm.FsmVariables;
                    ccFsmVars.GetFsmInt("ID").Value = i;
                    ccFsmVars.GetFsmString("Name").Value = "Charm " + i.ToString();
                    ccFsmVars.GetFsmString("PD Bool Name").Value = "gotCharm_" + i;
                    ccFsmVars.GetFsmGameObject("Charm Sprite").Value = findChild(ccPrefab, "Sprite").gameObject;
                    ccFsmVars.GetFsmGameObject("Glow").Value = findChild(ccPrefab, "Glow").gameObject;
                    ccPrefab.SetActive(true);
                }
            }
            #endregion

            // Shift rows
            float rowMultiplicator = ((float)Math.Ceiling(((float)numCharms) / 10.0)) / ((float)rows);
            // Vanilla has 4 rows
            float rowDeltaMultiplicator = 4.0f / ((float)rows);
            Transform backBoardTile, collectedCharmTile;
            Vector3 bbOldPos, ccOldPos;
            for (int i = 0; i <= rows; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    backBoardTile = findChild(backboardsGo, "BB " + ((10 * i) + j));
                    if (backBoardTile != null)
                    {
                        bbOldPos = backBoardTile.localPosition;
                        float y = ((bbOldPos.y - (-8.34f)) * rowMultiplicator) + (-8.34f);
                        backBoardTile.localPosition = new Vector3(bbOldPos.x, y, bbOldPos.z);
                        if (((10 * i) + j) > finalCharmAmount)
                        {
                            backBoardTile.GetComponent<SpriteRenderer>().color = new Color(116f / 255f, 116f / 255f, 116f / 255f, 0f / 255f);
                        }
                        else
                        {
                            backBoardTile.GetComponent<SpriteRenderer>().color = new Color(116f / 255f, 116f / 255f, 116f / 255f, 255f / 255f);
                        }
                    }
                    collectedCharmTile = findChild(collectedCharmsGo, ((10 * i) + j).ToString());
                    if (collectedCharmTile != null)
                    {
                        ccOldPos = collectedCharmTile.localPosition;
                        if (i <= 3)
                        {
                            float y = ((ccOldPos.y - (-8.34f)) * rowMultiplicator) + (-8.34f);
                            collectedCharmTile.localPosition = new Vector3(ccOldPos.x, y, ccOldPos.z);
                        }
                        else
                        {
                            float x_left = ((i % 2) == 0) ? -7.92f : -7.01f;
                            float y = (-8.37f) - (i * 1.42f * rowDeltaMultiplicator);
                            collectedCharmTile.localPosition = new Vector3(x_left + ((j - 1) * 1.5f), y, ccOldPos.z);
                        }
                    }
                }
            }

            Log("~OnCILS");
            orig(self);
        }

        private Transform findChild(GameObject parent, string name)
        {
            Transform ret = null;

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                if (parent.transform.GetChild(i).name == name)
                {
                    ret = parent.transform.GetChild(i);
                }
            }

            return ret;
        }
        private static void SetInactive(UnityEngine.Object go)
        {
            if (go != null)
            {
                UnityEngine.Object.DontDestroyOnLoad(go);
            }
        }

        private void Log(string message)
        {
            Logger.Log($"[{GetType().FullName.Replace(".", "]:[")}] - {message}");
        }
        private void Log(object message)
        {
            Logger.Log($"[{GetType().FullName.Replace(".", "]:[")}] - {message.ToString()}");
        }
    }
}
