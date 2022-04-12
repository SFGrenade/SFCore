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

        /// <summary>
        ///     Adds an inventory page with a specified original.
        /// </summary>
        /// <param name="type">The original to copy. For InventoryPageType.Empty see https://discord.com/channels/879125729936298015/880548951521103962/963340255661031464</param>
        /// <param name="name">The name of the new page</param>
        /// <param name="convKey">The language key for the new page</param>
        /// <param name="eventName">The event name to use to get to the new page. Needs to be unique</param>
        /// <param name="isAvailablePdBool">The playerdata bool name that indicates wether or not this page is available</param>
        /// <param name="callback">A callback so the mod can alter the given inventory page after it has been copied and made accessible</param>
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

            if (type == InventoryPageType.Empty)
            {
                var emptyInventoryFsm = newPaneGo.LocateMyFSM("UI Inventory");

                #region State Init

                for (int i = 0; i < 16; i++)
                {
                    emptyInventoryFsm.RemoveAction("Init", 5);
                }
                for (int i = 0; i < 4; i++)
                {
                    emptyInventoryFsm.RemoveAction("Init", 6);
                }
                for (int i = 0; i < 3; i++)
                {
                    emptyInventoryFsm.RemoveAction("Init", 8);
                }

                #endregion
                
                for (int i = 0; i < 4; i++)
                {
                    emptyInventoryFsm.RemoveAction("Inactive", 1);
                }
                emptyInventoryFsm.RemoveAction("Activate", 2);

                emptyInventoryFsm.GetAction<GetPosition>("Init Heart Piece", 0).gameObject = new FsmOwnerDefault()
                {
                    GameObject = emptyInventoryFsm.GetGameObjectVariable("Arrow L"),
                    OwnerOption = OwnerDefaultOption.SpecifyGameObject
                };
                for (int i = 0; i < 4; i++)
                {
                    emptyInventoryFsm.RemoveAction("L Arrow", 3);
                }
                for (int i = 0; i < 4; i++)
                {
                    emptyInventoryFsm.RemoveAction("R Arrow", 3);
                }
                emptyInventoryFsm.ChangeTransition("Inactive", "ACTIVATE", "Activate");
                emptyInventoryFsm.ChangeTransition("Init Heart Piece", "FINISHED", "L Arrow");
                emptyInventoryFsm.ChangeTransition("L Arrow", "UI RIGHT", "R Arrow");
                emptyInventoryFsm.ChangeTransition("R Arrow", "UI LEFT", "L Arrow");
                List<string> removingStates = new ()
                {
                    "Completion Rate",
                    "Any Other Panes?",
                    "Set Dream Nail Convo",
                    "DN Normal",
                    "DN Upgraded",
                    "Art Backboard",
                    "Set Heart Piece Convo",
                    "HP None",
                    "HP Some",
                    "HP All",
                    "Set Vessel Convo",
                    "Vessel None",
                    "Vessel 0",
                    "Vessel 1",
                    "Vessel 2",
                    "Vessel All",
                    "Set Nail Convo",
                    "Nail 1",
                    "Nail 2",
                    "Nail 3",
                    "Nail 4",
                    "Nail 5",
                    "Godfinder",
                    "Godfinder?",
                    "Godfinder? 2",
                    "Soul Orb",
                    "Dream Nail",
                    "Dream Gate",
                    "Quake",
                    "Fireball",
                    "Focus",
                    "Nail",
                    "Scream",
                    "Cyclone",
                    "Dash Slash",
                    "Uppercut",
                    "Geo",
                    "To Equip",
                    "To Equip 2",
                    "Bottom Choice",
                    "Bot to Inv",
                    "R to bot?",
                    "From R Bot Choice",
                    "B Choice 1",
                    "B Choice 2",
                    "B Choice 3",
                    "B Choice 4",
                    "Trinket 1",
                    "Trinket 2",
                    "Trinket 3",
                    "Trinket 4",
                    "Equip Item 1",
                    "Equip Item 2",
                    "Equip Item 3",
                    "Equip Item 4",
                    "Equip Item 5",
                    "Equip Item 6",
                    "Equip Item 7",
                    "Equip Item 8",
                    "Equip Item 9",
                    "Equip Item 10",
                    "Equip Item 11",
                    "Equip Item 12",
                    "Equip Item 13",
                    "Equip Item 14",
                    "Equip Item 15",
                    "Equip Item 16",
                    "Choice 1",
                    "Choice 2",
                    "Choice 3",
                    "Choice 4",
                    "Choice 5",
                    "Choice 6",
                    "Choice 7",
                    "Choice 8",
                    "Choice 9",
                    "Choice 10",
                    "Choice 11",
                    "Choice 12",
                    "Choice 13",
                    "Choice 14",
                    "Choice 15",
                    "Choice 16",
                    "Choice 17",
                    "Choice 18",
                    "Choice 19",
                    "Choice 20",
                    "Choice 21",
                    "Choice 22",
                    "Choice 23",
                    "Choice 24",
                    "Choice 25",
                    "Choice 26",
                    "Choice 27",
                    "Choice 28",
                    "Choice 29",
                    "Choice 30",
                    "Choice 31",
                    "Choice 32",
                    "Choice 33",
                    "Choice 34",
                    "Choice 35",
                    "Choice 36",
                    "Choice 37",
                    "Other Panes?",
                    "Other Panes? R",
                    "Heart Pieces"
                };
                for (int i = 0; i < removingStates.Count; i++)
                {
                    emptyInventoryFsm.RemoveState(removingStates[i]);
                }
                var emptyInventoryFsmVar = emptyInventoryFsm.FsmVariables;
                emptyInventoryFsmVar.FloatVariables = Array.Empty<FsmFloat>();
                emptyInventoryFsmVar.IntVariables = Array.Empty<FsmInt>();
                var boolList = new List<FsmBool>();
                foreach (var origBool in emptyInventoryFsmVar.BoolVariables)
                {
                    if (origBool.Name == "Repeating")
                    {
                        boolList.Add(origBool);
                    }
                }
                emptyInventoryFsmVar.BoolVariables = boolList.ToArray();
                emptyInventoryFsmVar.StringVariables = Array.Empty<FsmString>();
                var goList = new List<FsmGameObject>();
                var goKeepList = new List<string>
                {
                    "Arrow L",
                    "Arrow R",
                    "Border",
                    "Cursor",
                    "Cursor Back",
                    "Cursor Glow",
                    "Parent",
                    "Self"
                };
                foreach (var origGo in emptyInventoryFsmVar.GameObjectVariables)
                {
                    if (goKeepList.Contains(origGo.Name))
                    {
                        goList.Add(origGo);
                    }
                }
                emptyInventoryFsmVar.GameObjectVariables = goList.ToArray();
                List<string> removingGameObjects = new ()
                {
                    "Inv_Items",
                    "trinket_backboard",
                    "Divider L",
                    "Divider R",
                    "Text Name",
                    "Text Desc",
                    "Equipment",
                    "Text Completion",
                    "Text Desc Low",
                    "Item Control",
                    "Godfinder Icons"
                };
                for (int i = 0; i < removingGameObjects.Count; i++)
                {
                    UObject.DestroyImmediate(newPaneGo.Find(removingGameObjects[i]));
                }
                UObject.DestroyImmediate(newPaneGo.LocateMyFSM("Button Control"));
                UObject.DestroyImmediate(newPaneGo.LocateMyFSM("Update Text"));
                emptyInventoryFsm.FsmName = "Empty UI";
            }

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
