using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using UnityEngine;
using SFCore.Utils;
using Logger = Modding.Logger;
using UObject = UnityEngine.Object;

namespace SFCore
{
    /// <summary>
    ///     Enum to determine type of the inventory page.
    /// </summary>
    public enum InventoryPageType
    {
        /// <inheritdoc/>
        Empty,

        /// <inheritdoc/>
        Inventory,

        /// <inheritdoc/>
        Charms,

        /// <inheritdoc/>
        Journal,

        /// <inheritdoc/>
        Map
    }

    /// <summary>
    ///     Helper class for easily adding new inventory pages.
    ///     The mod using this needs to handle the following:
    ///     - 0 to 3 PlayerData bools per item
    ///     - 0 to 1 PlayerData int per item
    ///     - 1 to 3 name language strings per item
    ///     - 1 to 4 description language strings per item
    /// </summary>
    public static class InventoryHelper
    {
        private struct CustomPaneData
        {
            public InventoryPageType Type;
            public string Name;
            public string ConvKey;
            public string EventName;
            public string IsAvailablePdBool;

            public CustomPaneData()
            {
                Type = InventoryPageType.Empty;
                Name = "";
                ConvKey = "";
                EventName = "";
                IsAvailablePdBool = "";
            }

            public void Deconstruct(out InventoryPageType type, out string name, out string convKey, out string eventName, out string isAvailablePdBool)
            {
                (type, name, convKey, eventName, isAvailablePdBool) = (Type, Name, ConvKey, EventName, IsAvailablePdBool);
            }
        }

        private static Dictionary<string, CustomPaneData> _customPaneData = new();
        private static Dictionary<string, Action<GameObject>> _customPaneDataCallbacks = new();

        static InventoryHelper()
        {
            On.GameCameras.Start += GameCamerasOnStart;
        }

        /// <summary>
        ///     Used for static initialization.
        /// </summary>
        public static void unusedInit()
        {
        }

        public static void AddInventoryPage(InventoryPageType type, string name, string convKey, string eventName, string isAvailablePdBool, Action<GameObject> callback)
        {
            _customPaneData[eventName] = new CustomPaneData
            {
                Type = type,
                Name = name,
                ConvKey = convKey,
                EventName = eventName,
                IsAvailablePdBool = isAvailablePdBool
            };
            _customPaneDataCallbacks[eventName] = callback;
        }
        
        private static void GameCamerasOnStart(On.GameCameras.orig_Start orig, GameCameras self)
        {
            orig(self);
            // do the custom pages
            foreach (var cpd in _customPaneData.Values)
            {
                var (_, _, _, eventName, _) = cpd;
                Action<GameObject> callback = _customPaneDataCallbacks[eventName];
                GameObject ret = DoCopyPane(self, cpd);
                callback(ret);
            }
        }

        private static GameObject DoCopyPane(GameCameras cameras, CustomPaneData cpd)
        {
            var (type, name, convKey, eventName, isAvailablePdBool) = cpd;
            string prefabGoName;
            switch (type)
            {
                case InventoryPageType.Empty:
                    prefabGoName = "Inv";
                    break;
                case InventoryPageType.Inventory:
                    prefabGoName = "Inv";
                    break;
                case InventoryPageType.Charms:
                    prefabGoName = "Charms";
                    break;
                case InventoryPageType.Journal:
                    prefabGoName = "Journal";
                    break;
                case InventoryPageType.Map:
                    prefabGoName = "Map";
                    break;
                default:
                    return null;
            }

            GameObject inventoryGo = cameras.gameObject.Find("Inventory");
            var inventoryFsm = inventoryGo.LocateMyFSM("Inventory Control");
            var inventoryFsmVars = inventoryFsm.FsmVariables;
            if (inventoryFsm.GetState("Closed").Fsm == null)
            {
                inventoryFsm.Preprocess();
            }
            
            var newPaneGo = GameObject.Instantiate(inventoryGo.FindGameObjectInChildren(prefabGoName), inventoryGo.transform);
            newPaneGo.SetActive(false);
            newPaneGo.name = name;
            var newPaneFod = new FsmOwnerDefault()
            {
                GameObject = newPaneGo,
                OwnerOption = OwnerDefaultOption.SpecifyGameObject
            };

            string fsmVarNamePane = $"{name} Pane";
            string fsmStateOpenName = $"Open {name}";
            string fsmStateNext2Name = $"Next {name} 2";
            string fsmStateNext3Name = $"Next {name} 3";
            string fsmStateNextName = $"Next {name}";
            
            inventoryFsm.AddGameObjectVariable(fsmVarNamePane);
            
            int totalPanes = inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).compareTo.Last().Value + 1;
            
            #region Inventory Control - Init

            inventoryFsm.InsertAction("Init", new FindChild()
            {
                gameObject = inventoryFsm.GetAction<FindChild>("Init", 3).gameObject,
                childName = name,
                storeResult = inventoryFsmVars.FindFsmGameObject(fsmVarNamePane)
            }, 12);

            #endregion

            #region Inventory Control - Check Current Pane

            var ccp11Ct = new List<FsmInt>(inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).compareTo)
            {
                totalPanes
            };
            inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).compareTo = ccp11Ct.ToArray();
            var ccp11Se = new List<FsmEvent>(inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).sendEvent)
            {
                FsmEvent.GetFsmEvent(eventName)
            };
            inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).sendEvent = ccp11Se.ToArray();

            #endregion

            #region Inventory Control - Open Journal copy "Open Equipment"

            inventoryFsm.CopyState("Open Journal", fsmStateOpenName);
            inventoryFsm.GetAction<GetLanguageString>(fsmStateOpenName, 0).convName = convKey;
            inventoryFsm.GetAction<SetIntValue>(fsmStateOpenName, 2).intValue = totalPanes;
            inventoryFsm.GetAction<SetIntValue>(fsmStateOpenName, 3).intValue = totalPanes;
            inventoryFsm.GetAction<SetGameObject>(fsmStateOpenName, 4).gameObject = inventoryFsmVars.FindFsmGameObject(fsmVarNamePane);
            inventoryFsm.AddTransition("Check Current Pane", eventName, fsmStateOpenName);

            #endregion

            #region Inventory Control - Check R Pane

            var crp1Ct = new List<FsmInt>(inventoryFsm.GetAction<IntSwitch>("Check R Pane", 1).compareTo)
            {
                totalPanes
            };
            crp1Ct[5] = totalPanes + 1;
            inventoryFsm.GetAction<IntSwitch>("Check R Pane", 1).compareTo = crp1Ct.ToArray();
            var crp1Se = new List<FsmEvent>(inventoryFsm.GetAction<IntSwitch>("Check R Pane", 1).sendEvent)
            {
                FsmEvent.FindEvent(eventName)
            };
            inventoryFsm.GetAction<IntSwitch>("Check R Pane", 1).sendEvent = crp1Se.ToArray();

            #endregion

            #region Inventory Control - Next Journal 2 copy "Next Equipment 2"

            inventoryFsm.CopyState("Next Journal 2", fsmStateNext2Name);
            inventoryFsm.GetAction<PlayerDataBoolTest>(fsmStateNext2Name, 0).boolName = isAvailablePdBool;
            inventoryFsm.GetAction<GetLanguageString>(fsmStateNext2Name, 1).convName = convKey;
            inventoryFsm.AddTransition("Check R Pane", eventName, fsmStateNext2Name);

            inventoryFsm.GetAction<SetIntValue>("Under 2", 0).intValue = totalPanes + 1;

            #endregion

            #region Inventory Control - Check L Pane

            var clp1Ct = new List<FsmInt>(inventoryFsm.GetAction<IntSwitch>("Check L Pane", 1).compareTo)
            {
                totalPanes
            };
            clp1Ct[5] = totalPanes + 1;
            inventoryFsm.GetAction<IntSwitch>("Check L Pane", 1).compareTo = clp1Ct.ToArray();
            var clp1Se = new List<FsmEvent>(inventoryFsm.GetAction<IntSwitch>("Check L Pane", 1).sendEvent)
            {
                FsmEvent.FindEvent(eventName)
            };
            inventoryFsm.GetAction<IntSwitch>("Check L Pane", 1).sendEvent = clp1Se.ToArray();

            #endregion

            #region Inventory Control - Next Journal 3 copy "Next Equipment 3"

            inventoryFsm.CopyState("Next Journal 3", fsmStateNext3Name);
            inventoryFsm.GetAction<PlayerDataBoolTest>(fsmStateNext3Name, 0).boolName = isAvailablePdBool;
            inventoryFsm.GetAction<GetLanguageString>(fsmStateNext3Name, 1).convName = convKey;
            inventoryFsm.AddTransition("Check L Pane", eventName, fsmStateNext3Name);

            inventoryFsm.GetAction<SetIntValue>("Under 3", 0).intValue = totalPanes + 1;

            #endregion

            #region Inventory Control - Loop Through

            var cls3Ct = new List<FsmInt>(inventoryFsm.GetAction<IntSwitch>("Loop Through", 3).compareTo)
            {
                totalPanes
            };
            cls3Ct[5] = totalPanes + 1;
            inventoryFsm.GetAction<IntSwitch>("Loop Through", 3).compareTo = cls3Ct.ToArray();
            var cls3Se = new List<FsmEvent>(inventoryFsm.GetAction<IntSwitch>("Loop Through", 3).sendEvent)
            {
                FsmEvent.FindEvent(eventName)
            };
            inventoryFsm.GetAction<IntSwitch>("Loop Through", 3).sendEvent = cls3Se.ToArray();

            #endregion

            #region Inventory Control - Next Journal copy "Next Equipment"

            inventoryFsm.CopyState("Next Journal", fsmStateNextName);
            inventoryFsm.GetAction<PlayerDataBoolTest>(fsmStateNextName, 0).boolName = isAvailablePdBool;
            inventoryFsm.GetAction<SetGameObject>(fsmStateNextName, 2).gameObject = inventoryFsmVars.FindFsmGameObject(fsmVarNamePane);
            inventoryFsm.GetAction<GetLanguageString>(fsmStateNextName, 3).convName = convKey;
            inventoryFsm.AddTransition("Loop Through", eventName, fsmStateNextName);

            inventoryFsm.GetAction<SetIntValue>("Under", 0).intValue = totalPanes + 1;

            #endregion

            return newPaneGo;
        }

        private static void Log(string message)
        {
            Logger.LogDebug($"[SFCore]:[InventoryHelper] - {message}");
            Debug.Log($"[SFCore]:[InventoryHelper] - {message}");
        }

        private static void Log(object message)
        {
            Log($"{message}");
        }
    }
}
