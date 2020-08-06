using ModCommon;
using ModCommon.Util;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Logger = Modding.Logger;

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
         * v 0.0.1.0
         */

        /* 
         * This HelperClass is currently set up to add only normal items, so not things like Rancid Eggs, Simple Keys and such.
         * 
         * Example inclusion in your mod's Initialize() function:
         * 
         * itemHelper = new ItemHelper(new s_CustomNormalItem[] {
         *     new s_CustomNormalItem() {
         *         uniqueName = "Creator Mod Something",
         *         sprite = new Sprite(),
         *         playerdataBool = "",
         *         nameConvo = "",
         *         descConvo = ""
         *     }
         * });
         */

        public ItemHelper(s_CustomNormalItem[] customNormalItems)
        {
            // May break without preloads, idk

            #region Important Objects, do not touch
            GameObject invGO = GameObject.Find("_GameCameras").FindGameObjectInChildren("Inv");
            GameObject equipmentGO = invGO.FindGameObjectInChildren("Equipment");
            #endregion

            foreach (var i in customNormalItems)
            {
                // Item Display Prefab
                GameObject dashPrefab = equipmentGO.FindGameObjectInChildren("Dash Cloak");

                // Custom name for the GameObject and the FSM State
                string customStateName = i.uniqueName;

                // Check if the GameObject already exists
                GameObject invItemDisplay = equipmentGO.FindGameObjectInChildren(customStateName);

                if (invItemDisplay == null)
                {
                    #region Customize Item Display GameObject
                    invItemDisplay = UnityEngine.Object.Instantiate(dashPrefab, equipmentGO.transform, true);
                    invItemDisplay.name = customStateName;
                    invItemDisplay.GetComponent<SpriteRenderer>().sprite = i.sprite;
                    invItemDisplay.GetComponent<BoxCollider2D>().size = Vector2.one;
                    invItemDisplay.GetComponent<BoxCollider2D>().offset = Vector2.zero;
                    #endregion

                    #region Customize FSM
                    PlayMakerFSM equipBuildFsm = equipmentGO.LocateMyFSM("Build Equipment List");
                    equipBuildFsm.CopyState("Kings Brand", customStateName);
                    equipBuildFsm.GetAction<PlayerDataBoolTest>(customStateName, 0).boolName = i.playerdataBool;
                    equipBuildFsm.GetAction<FindChild>(customStateName, 1).childName = customStateName;
                    equipBuildFsm.GetAction<SetFsmString>(customStateName, 9).setValue = i.nameConvo;
                    equipBuildFsm.GetAction<SetFsmString>(customStateName, 10).setValue = i.descConvo;
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
