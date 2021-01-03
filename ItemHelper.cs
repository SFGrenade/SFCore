using ModCommon;
using ModCommon.Util;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Logger = Modding.Logger;
using System.Collections.Generic;
using IL.InControl;
using UObject = UnityEngine.Object;

namespace SFCore
{
    public struct s_CustomNormalItem
    {
        public string uniqueName;
        public Sprite sprite;

        public string playerdataBool;
        public string nameConvo;
        public string descConvo;
    }

    public class ItemHelper
    {
        /* 
         * ItemHelper
         * v 1.0.0.0
         */

        public ItemHelper(s_CustomNormalItem[] customNormalItems)
        {
            foreach (var i in customNormalItems)
            {
                AddNormalItem(i.uniqueName, i.sprite, i.playerdataBool, i.nameConvo, i.descConvo);
            }
        }

        public static void AddNormalItem(string uniqueName, Sprite sprite, string playerdataBool, string nameConvo, string descConvo)
        {
            // May break without preloads, idk

            #region Important Objects, do not touch
            GameObject invGO = GameObject.Find("_GameCameras").FindGameObjectInChildren("Inv");
            GameObject equipmentGO = invGO.FindGameObjectInChildren("Equipment");
            #endregion

            // Item Display Prefab
            GameObject dashPrefab = equipmentGO.FindGameObjectInChildren("Dash Cloak");

            // Custom name for the GameObject and the FSM State
            string customStateName = uniqueName;

            // Check if the GameObject already exists
            GameObject invItemDisplay = equipmentGO.FindGameObjectInChildren(customStateName);

            if (invItemDisplay == null)
            {
                #region Customize Item Display GameObject
                invItemDisplay = GameObject.Instantiate(dashPrefab, equipmentGO.transform, true);
                invItemDisplay.name = customStateName;
                invItemDisplay.GetComponent<SpriteRenderer>().sprite = sprite;
                AddToInvFadeGroup(invItemDisplay);
                var fg = invItemDisplay.transform.parent.parent.gameObject.GetComponent<FadeGroup>();
                List<SpriteRenderer> srList = new List<SpriteRenderer>(fg.spriteRenderers);
                srList.Add(invItemDisplay.GetComponent<SpriteRenderer>());
                fg.spriteRenderers = srList.ToArray();
                invItemDisplay.GetComponent<BoxCollider2D>().size = Vector2.one;
                invItemDisplay.GetComponent<BoxCollider2D>().offset = Vector2.zero;
                #endregion

                #region Customize FSM
                PlayMakerFSM equipBuildFsm = equipmentGO.LocateMyFSM("Build Equipment List");
                equipBuildFsm.CopyState("Kings Brand", customStateName);
                equipBuildFsm.GetAction<PlayerDataBoolTest>(customStateName, 0).boolName = playerdataBool;
                equipBuildFsm.GetAction<FindChild>(customStateName, 1).childName = customStateName;
                equipBuildFsm.GetAction<SetFsmString>(customStateName, 9).setValue = nameConvo;
                equipBuildFsm.GetAction<SetFsmString>(customStateName, 10).setValue = descConvo;
                #endregion

                #region Include custom item in Build Equipment List
                string toState = "Pause";
                foreach (FsmTransition transition in equipBuildFsm.GetState("Rancid Egg").Transitions)
                {
                    if (transition.EventName == "FINISHED")
                    {
                        toState = transition.ToState;
                    }
                }

                equipBuildFsm.ChangeTransition("Rancid Egg", "FINISHED", customStateName);
                equipBuildFsm.ChangeTransition(customStateName, "FINISHED", toState);
                #endregion
            }
        }

        public static void AddOneTwoItem(string uniqueName, Sprite sprite1, Sprite sprite2, string playerdataBool1, string playerdataBool2, string nameConvo1, string nameConvo2, string descConvo1, string descConvo2)
        {
            GameObject invGO = GameObjectExtensions.FindGameObjectInChildren(GameObject.Find("_GameCameras"), "Inv");
            GameObject equipmentGO = GameObjectExtensions.FindGameObjectInChildren(invGO, "Equipment");
            GameObject dashPrefab = GameObjectExtensions.FindGameObjectInChildren(equipmentGO, "Dash Cloak");

            string customStateName = uniqueName;
            string customCheckStateName = $"{uniqueName} Check";
            string customState1Name = $"{uniqueName} 1";
            string customState2Name = $"{uniqueName} 2";

            GameObject invItemDisplay = equipmentGO.FindGameObjectInChildren(customStateName);

            if (invItemDisplay == null)
            {
                invItemDisplay = GameObject.Instantiate(dashPrefab, equipmentGO.transform, true);
                invItemDisplay.name = customStateName;
                invItemDisplay.GetComponent<SpriteRenderer>().sprite = sprite1;
                AddToInvFadeGroup(invItemDisplay);
                invItemDisplay.GetComponent<BoxCollider2D>().size = Vector2.one;
                invItemDisplay.GetComponent<BoxCollider2D>().offset = Vector2.zero;

                if (invItemDisplay != null)
                    UObject.DontDestroyOnLoad(invItemDisplay);
                if (sprite1 != null)
                    UObject.DontDestroyOnLoad(sprite1);
                if (sprite2 != null)
                    UObject.DontDestroyOnLoad(sprite2);

                PlayMakerFSM equipBuildFsm = equipmentGO.LocateMyFSM("Build Equipment List");

                #region Check State
                equipBuildFsm.CopyState("Map Quill Check", customCheckStateName);
                equipBuildFsm.GetAction<GetPlayerDataBool>(customCheckStateName, 0).boolName = playerdataBool1;
                equipBuildFsm.GetAction<GetPlayerDataBool>(customCheckStateName, 1).boolName = playerdataBool2;
                #endregion
                #region Single States
                equipBuildFsm.CopyState("Map", customState1Name);
                equipBuildFsm.GetAction<FindChild>(customState1Name, 0).childName = customStateName;
                equipBuildFsm.GetAction<SetSpriteRendererSprite>(customState1Name, 1).sprite = sprite1;
                equipBuildFsm.GetAction<SetFsmString>(customState1Name, 9).setValue = nameConvo1;
                equipBuildFsm.GetAction<SetFsmString>(customState1Name, 10).setValue = descConvo1;

                equipBuildFsm.CopyState("Quill", customState2Name);
                equipBuildFsm.GetAction<FindChild>(customState2Name, 0).childName = customStateName;
                equipBuildFsm.GetAction<SetSpriteRendererSprite>(customState2Name, 1).sprite = sprite2;
                equipBuildFsm.GetAction<SetFsmString>(customState2Name, 9).setValue = nameConvo2;
                equipBuildFsm.GetAction<SetFsmString>(customState2Name, 10).setValue = descConvo2;
                #endregion

                string toState = "Pause";
                foreach (FsmTransition transition in equipBuildFsm.GetState("Rancid Egg").Transitions)
                {
                    if (transition.EventName == "FINISHED")
                    {
                        toState = transition.ToState;
                    }
                }

                equipBuildFsm.ChangeTransition("Rancid Egg", "FINISHED", customCheckStateName);
                equipBuildFsm.ChangeTransition(customCheckStateName, "NONE", toState);
                equipBuildFsm.ChangeTransition(customCheckStateName, "MAP", customState1Name);
                equipBuildFsm.ChangeTransition(customCheckStateName, "QUILL", customState2Name);
                equipBuildFsm.ChangeTransition(customCheckStateName, "MAP AND QUILL", toState);
                equipBuildFsm.ChangeTransition(customState1Name, "FINISHED", toState);
                equipBuildFsm.ChangeTransition(customState2Name, "FINISHED", toState);
            }
        }
        
        public static void AddOneTwoBothItem(string uniqueName, Sprite sprite1, Sprite sprite2, Sprite spriteBoth, string playerdataBool1, string playerdataBool2, string nameConvo1, string nameConvo2, string nameConvoBoth, string descConvo1, string descConvo2, string descConvoBoth)
        {
            GameObject invGO = GameObjectExtensions.FindGameObjectInChildren(GameObject.Find("_GameCameras"), "Inv");
            GameObject equipmentGO = GameObjectExtensions.FindGameObjectInChildren(invGO, "Equipment");
            GameObject dashPrefab = GameObjectExtensions.FindGameObjectInChildren(equipmentGO, "Dash Cloak");

            string customStateName = uniqueName;
            string customCheckStateName = $"{uniqueName} Check";
            string customState1Name = $"{uniqueName} 1";
            string customState2Name = $"{uniqueName} 2";
            string customStateBothName = $"{uniqueName} Both";

            GameObject invItemDisplay = equipmentGO.FindGameObjectInChildren(customStateName);

            if (invItemDisplay == null)
            {
                invItemDisplay = GameObject.Instantiate(dashPrefab, equipmentGO.transform, true);
                invItemDisplay.name = customStateName;
                invItemDisplay.GetComponent<SpriteRenderer>().sprite = sprite1;
                AddToInvFadeGroup(invItemDisplay);
                invItemDisplay.GetComponent<BoxCollider2D>().size = Vector2.one;
                invItemDisplay.GetComponent<BoxCollider2D>().offset = Vector2.zero;

                if (invItemDisplay != null)
                    UObject.DontDestroyOnLoad(invItemDisplay);
                if (sprite1 != null)
                    UObject.DontDestroyOnLoad(sprite1);
                if (sprite2 != null)
                    UObject.DontDestroyOnLoad(sprite2);
                if (spriteBoth != null)
                    UObject.DontDestroyOnLoad(spriteBoth);

                PlayMakerFSM equipBuildFsm = equipmentGO.LocateMyFSM("Build Equipment List");

                #region Check State
                equipBuildFsm.CopyState("Map Quill Check", customCheckStateName);
                equipBuildFsm.GetAction<GetPlayerDataBool>(customCheckStateName, 0).boolName = playerdataBool1;
                equipBuildFsm.GetAction<GetPlayerDataBool>(customCheckStateName, 1).boolName = playerdataBool2;
                #endregion
                #region Single States
                equipBuildFsm.CopyState("Map", customState1Name);
                equipBuildFsm.GetAction<FindChild>(customState1Name, 0).childName = customStateName;
                equipBuildFsm.GetAction<SetSpriteRendererSprite>(customState1Name, 1).sprite = sprite1;
                equipBuildFsm.GetAction<SetFsmString>(customState1Name, 9).setValue = nameConvo1;
                equipBuildFsm.GetAction<SetFsmString>(customState1Name, 10).setValue = descConvo1;

                equipBuildFsm.CopyState("Quill", customState2Name);
                equipBuildFsm.GetAction<FindChild>(customState2Name, 0).childName = customStateName;
                equipBuildFsm.GetAction<SetSpriteRendererSprite>(customState2Name, 1).sprite = sprite2;
                equipBuildFsm.GetAction<SetFsmString>(customState2Name, 9).setValue = nameConvo2;
                equipBuildFsm.GetAction<SetFsmString>(customState2Name, 10).setValue = descConvo2;

                equipBuildFsm.CopyState("Map and Quill", customStateBothName);
                equipBuildFsm.GetAction<FindChild>(customStateBothName, 0).childName = customStateName;
                equipBuildFsm.GetAction<SetSpriteRendererSprite>(customStateBothName, 1).sprite = spriteBoth;
                equipBuildFsm.GetAction<SetFsmString>(customStateBothName, 9).setValue = nameConvoBoth;
                equipBuildFsm.GetAction<SetFsmString>(customStateBothName, 10).setValue = descConvoBoth;
                #endregion

                string toState = "Pause";
                foreach (FsmTransition transition in equipBuildFsm.GetState("Rancid Egg").Transitions)
                {
                    if (transition.EventName == "FINISHED")
                    {
                        toState = transition.ToState;
                    }
                }

                equipBuildFsm.ChangeTransition("Rancid Egg", "FINISHED", customCheckStateName);
                equipBuildFsm.ChangeTransition(customCheckStateName, "NONE", toState);
                equipBuildFsm.ChangeTransition(customCheckStateName, "MAP", customState1Name);
                equipBuildFsm.ChangeTransition(customCheckStateName, "QUILL", customState2Name);
                equipBuildFsm.ChangeTransition(customCheckStateName, "MAP AND QUILL", customStateBothName);
                equipBuildFsm.ChangeTransition(customState1Name, "FINISHED", toState);
                equipBuildFsm.ChangeTransition(customState2Name, "FINISHED", toState);
                equipBuildFsm.ChangeTransition(customStateBothName, "FINISHED", toState);
            }
        }

        public static void AddCountedItem(string uniqueName, Sprite sprite, string playerdataInt, string nameConvo, string descConvo)
        {
            // May break without preloads, idk

            #region Important Objects, do not touch
            GameObject invGO = GameObject.Find("_GameCameras").FindGameObjectInChildren("Inv");
            GameObject equipmentGO = invGO.FindGameObjectInChildren("Equipment");
            #endregion

            // Item Display Prefab
            GameObject eggPrefab = equipmentGO.FindGameObjectInChildren("Rancid Egg");

            // Custom name for the GameObject and the FSM State
            string customStateName = uniqueName;

            // Check if the GameObject already exists
            GameObject invItemDisplay = equipmentGO.FindGameObjectInChildren(customStateName);

            if (invItemDisplay == null)
            {
                #region Customize Item Display GameObject
                invItemDisplay = GameObject.Instantiate(eggPrefab, equipmentGO.transform, true);
                invItemDisplay.name = customStateName;
                invItemDisplay.GetComponent<SpriteRenderer>().sprite = sprite;
                AddToInvFadeGroup(invItemDisplay);
                invItemDisplay.GetComponent<BoxCollider2D>().size = Vector2.one;
                invItemDisplay.GetComponent<BoxCollider2D>().offset = Vector2.zero;
                invItemDisplay.GetComponent<DisplayItemAmount>().playerDataInt = playerdataInt;
                invItemDisplay.GetComponent<DisplayItemAmount>().textObject = invItemDisplay.GetComponentInChildren<TMPro.TextMeshPro>();
                #endregion

                #region Customize FSM
                PlayMakerFSM equipBuildFsm = equipmentGO.LocateMyFSM("Build Equipment List");
                equipBuildFsm.CopyState("Rancid Egg", customStateName);
                equipBuildFsm.GetAction<GetPlayerDataInt>(customStateName, 0).intName = playerdataInt;
                equipBuildFsm.GetAction<FindChild>(customStateName, 2).childName = customStateName;
                equipBuildFsm.GetAction<SetFsmString>(customStateName, 10).setValue = nameConvo;
                equipBuildFsm.GetAction<SetFsmString>(customStateName, 11).setValue = descConvo;
                #endregion

                #region Include custom item in Build Equipment List
                string toState = "Pause";
                foreach (FsmTransition transition in equipBuildFsm.GetState("Rancid Egg").Transitions)
                {
                    if (transition.EventName == "FINISHED")
                    {
                        toState = transition.ToState;
                    }
                }

                equipBuildFsm.ChangeTransition("Rancid Egg", "FINISHED", customStateName);
                equipBuildFsm.ChangeTransition(customStateName, "FINISHED", toState);
                #endregion
            }
        }

        private static void AddToInvFadeGroup(GameObject spriteGo)
        {
            var sr = spriteGo.GetComponent<SpriteRenderer>();
            sr.sortingLayerID = 629535577;
            var invGo = spriteGo.transform.parent.parent.gameObject;
            var fg = invGo.GetComponent<FadeGroup>();
            var srList = new List<SpriteRenderer>(fg.spriteRenderers);
            srList.Add(sr);
            fg.spriteRenderers = srList.ToArray();
        }

        private static void Log(string message)
        {
            Logger.Log($"[SFCore]:[ItemHelper] - {message}");
        }
        private static void Log(object message)
        {
            Logger.Log($"[SFCore]:[ItemHelper] - {message.ToString()}");
        }
    }
}
