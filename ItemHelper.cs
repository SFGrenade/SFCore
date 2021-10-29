using SFCore.Utils;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Logger = Modding.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Modding;
using TMPro;
using UObject = UnityEngine.Object;
using SFCore.MonoBehaviours;

namespace SFCore
{
    /// <summary>
    ///     Enum to determine type of item.
    /// </summary>
    public enum ItemType
    {
        Normal,
        OneTwo,
        OneTwoBoth,
        Counted,
        Flower
    }

    /// <summary>
    ///     Item helper class for easily adding custom items.
    ///     The mod using this needs to handle the following:
    ///     - up to 3 PlayerData bools per item
    ///     - up to 1 PlayerData int per item
    ///     - up to 3 name language strings per item
    ///     - up to 4 description language strings per item
    /// </summary>
    public static class ItemHelper
    {
        public struct Item
        {
            public ItemType type;
            public string uniqueName;
            public Sprite sprite1;
            public Sprite sprite2;
            public Sprite spriteBoth;
            public string playerdataBool1;
            public string playerdataBool2;
            public string playerdataInt;
            public string nameConvo1;
            public string nameConvo2;
            public string nameConvoBoth;
            public string descConvo1;
            public string descConvo2;
            public string descConvoBoth;
        }

        private static Dictionary<string, Sprite> _defaultSprites = new Dictionary<string, Sprite>();

        private static List<Item> _defaultItemList = new List<Item>();
        private static List<Item> _customItemList = new List<Item>();

        private static LanguageStrings _langStrings;

        private static bool _initialized = false;

        static ItemHelper()
        {
            _defaultItemList = new List<Item>();
            _customItemList = new List<Item>();

            _langStrings = new LanguageStrings(Assembly.GetExecutingAssembly(), "SFCore.Resources.Language.json", Encoding.UTF8);

            ModHooks.LanguageGetHook += LanguageGetHook;
            ModHooks.GetPlayerBoolHook += GetPlayerBoolHook;
            ModHooks.GetPlayerIntHook += GetPlayerIntHook;
            On.GameCameras.Start += GameCamerasOnStart;
        }
        public static void unusedInit() { }

        private static bool GetPlayerBoolHook(string originalset, bool orig)
        {
            if (originalset.Equals("hasCustomInventoryItem"))
            {
                return (CustomItemList.Instance != null) && (CustomItemList.Instance.hasAtLeastOneItem());
            }
            return orig;
        }

        private static int GetPlayerIntHook(string originalset, int orig)
        {
            if (originalset.Equals("0Return"))
            {
                return 0;
            }
            else if (originalset.Equals("customItemListGotAmount"))
            {
                return CustomItemList.Instance.GotItemAmount();
            }
            else if (originalset.Equals("customItemListTotalAmount"))
            {
                return CustomItemList.Instance.TotalItemAmount();
            }
            return orig;
        }

        private static void GameCamerasOnStart(On.GameCameras.orig_Start orig, GameCameras self)
        {
            orig(self);

            #region Display no equipments

            var equipmentGo = self.gameObject.FindGameObjectInChildren("Equipment");
            var equipmentFsm = equipmentGo.LocateMyFSM("Build Equipment List");
            if (equipmentFsm.GetState("Init").Fsm == null)
            {
                equipmentFsm.Preprocess();
            }
            equipmentFsm.ChangeTransition("Init", "FINISHED", "Pause");

            #endregion

            InitDefaultItems(equipmentGo);

            var successful = CopyJournalPane(self.gameObject.Find("Inventory"));

            if (successful)
            {
                self.gameObject.Find("Inventory").LocateMyFSM("Inventory Control").SetState("Init");
            }
            else
            {
                Log("Couldn't finish changing inventory");
            }
        }

        private static string LanguageGetHook(string key, string sheet, string orig)
        {
            if (_langStrings.ContainsKey(key, sheet))
            {
                return _langStrings.Get(key, sheet);
            }
            return orig;
        }

        private static void InitDefaultItems(GameObject equipmentGo)
        {
            var equipmentFsm = equipmentGo.LocateMyFSM("Build Equipment List");

            if (equipmentFsm.GetState("Dash").Fsm == null || _initialized) return;

            #region Populate sprite dictionary

            if (_defaultSprites.ContainsKey("Dash"))
                _defaultSprites["Dash"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Dash Cloak")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                _defaultSprites.Add("Dash",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Dash Cloak").GetComponent<SpriteRenderer>().sprite));
            if (_defaultSprites.ContainsKey("ShadowDash"))
                _defaultSprites["ShadowDash"] =
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Dash", 16).sprite.Value);
            else
                _defaultSprites.Add("ShadowDash",
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Dash", 16).sprite.Value));
            if (_defaultSprites.ContainsKey("Walljump"))
                _defaultSprites["Walljump"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Mantis Claw")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                _defaultSprites.Add("Walljump",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Mantis Claw").GetComponent<SpriteRenderer>().sprite));
            if (_defaultSprites.ContainsKey("Super Dash"))
                _defaultSprites["Super Dash"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Super Dash")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                _defaultSprites.Add("Super Dash",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Super Dash").GetComponent<SpriteRenderer>().sprite));
            if (_defaultSprites.ContainsKey("Double Jump"))
                _defaultSprites["Double Jump"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Double Jump")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                _defaultSprites.Add("Double Jump",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Double Jump").GetComponent<SpriteRenderer>().sprite));
            if (_defaultSprites.ContainsKey("Lantern"))
                _defaultSprites["Lantern"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Lantern")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                _defaultSprites.Add("Lantern",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Lantern").GetComponent<SpriteRenderer>().sprite));
            if (_defaultSprites.ContainsKey("Map"))
                _defaultSprites["Map"] =
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Map", 1).sprite.Value);
            else
                _defaultSprites.Add("Map",
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Map", 1).sprite.Value));
            if (_defaultSprites.ContainsKey("Quill"))
                _defaultSprites["Quill"] =
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Quill", 1).sprite.Value);
            else
                _defaultSprites.Add("Quill",
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Quill", 1).sprite.Value));
            if (_defaultSprites.ContainsKey("MapQuill"))
                _defaultSprites["MapQuill"] =
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Map and Quill", 1).sprite.Value);
            else
                _defaultSprites.Add("MapQuill",
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Map and Quill", 1).sprite.Value));
            if (_defaultSprites.ContainsKey("Kings Brand"))
                _defaultSprites["Kings Brand"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Kings Brand")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                _defaultSprites.Add("Kings Brand",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Kings Brand").GetComponent<SpriteRenderer>().sprite));
            if (_defaultSprites.ContainsKey("Tram Pass"))
                _defaultSprites["Tram Pass"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Tram Pass")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                _defaultSprites.Add("Tram Pass",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Tram Pass").GetComponent<SpriteRenderer>().sprite));
            if (_defaultSprites.ContainsKey("City Key"))
                _defaultSprites["City Key"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("City Key")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                _defaultSprites.Add("City Key",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("City Key").GetComponent<SpriteRenderer>().sprite));
            if (_defaultSprites.ContainsKey("Store Key"))
                _defaultSprites["Store Key"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Store Key")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                _defaultSprites.Add("Store Key",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Store Key").GetComponent<SpriteRenderer>().sprite));
            if (_defaultSprites.ContainsKey("Love Key"))
                _defaultSprites["Love Key"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Love Key")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                _defaultSprites.Add("Love Key",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Love Key").GetComponent<SpriteRenderer>().sprite));
            if (_defaultSprites.ContainsKey("Flower"))
                _defaultSprites["Flower"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Xun Flower")
                    .GetComponent<InvItemDisplay>().inactiveSprite);
            else
                _defaultSprites.Add("Flower",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Xun Flower").GetComponent<InvItemDisplay>()
                        .inactiveSprite));
            if (_defaultSprites.ContainsKey("FlowerBroken"))
                _defaultSprites["FlowerBroken"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Xun Flower")
                    .GetComponent<InvItemDisplay>().activeSprite);
            else
                _defaultSprites.Add("FlowerBroken",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Xun Flower").GetComponent<InvItemDisplay>().activeSprite));
            if (_defaultSprites.ContainsKey("Simple Key"))
                _defaultSprites["Simple Key"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Simple Key")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                _defaultSprites.Add("Simple Key",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Simple Key").GetComponent<SpriteRenderer>().sprite));
            if (_defaultSprites.ContainsKey("Ore"))
                _defaultSprites["Ore"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Ore").GetComponent<SpriteRenderer>()
                    .sprite);
            else
                _defaultSprites.Add("Ore",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Ore").GetComponent<SpriteRenderer>().sprite));
            if (_defaultSprites.ContainsKey("Rancid Egg"))
                _defaultSprites["Rancid Egg"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Rancid Egg")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                _defaultSprites.Add("Rancid Egg",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Rancid Egg").GetComponent<SpriteRenderer>().sprite));

            #endregion

            #region Make new inventory

            AddDefaultOneTwoBothItem("Dash",
                _defaultSprites["Dash"], _defaultSprites["ShadowDash"], _defaultSprites["ShadowDash"],
                "hasDash", "hasShadowDash",
                "INV_NAME_DASH", "INV_NAME_SHADOWDASH", "INV_NAME_SHADOWDASH",
                "INV_DESC_DASH", "INV_DESC_SHADOWDASH", "INV_DESC_SHADOWDASH");
            AddDefaultNormalItem("Walljump", _defaultSprites["Walljump"], "hasWalljump", "INV_NAME_WALLJUMP",
                "INV_DESC_WALLJUMP");
            AddDefaultNormalItem("Super Dash", _defaultSprites["Super Dash"], "hasSuperDash", "INV_NAME_SUPERDASH",
                "INV_DESC_SUPERDASH");
            AddDefaultNormalItem("Double Jump", _defaultSprites["Double Jump"], "hasDoubleJump",
                "INV_NAME_DOUBLEJUMP", "INV_DESC_DOUBLEJUMP");
            AddDefaultNormalItem("Lantern", _defaultSprites["Lantern"], "hasLantern", "INV_NAME_LANTERN",
                "INV_DESC_LANTERN");
            AddDefaultOneTwoBothItem("Map Quill",
                _defaultSprites["Map"], _defaultSprites["Quill"], _defaultSprites["MapQuill"],
                "hasMap", "hasQuill",
                "INV_NAME_MAP", "INV_NAME_QUILL", "INV_NAME_MAPQUILL",
                "INV_DESC_MAP", "INV_DESC_QUILL", "INV_DESC_MAPQUILL");
            AddDefaultNormalItem("Kings Brand", _defaultSprites["Kings Brand"], "hasKingsBrand",
                "INV_NAME_KINGSBRAND", "INV_DESC_KINGSBRAND");
            AddDefaultNormalItem("Tram Pass", _defaultSprites["Tram Pass"], "hasTramPass", "INV_NAME_TRAM_PASS",
                "INV_DESC_TRAM_PASS");
            AddDefaultNormalItem("City Key", _defaultSprites["City Key"], "hasCityKey", "INV_NAME_CITYKEY",
                "INV_DESC_CITYKEY");
            AddDefaultNormalItem("Store Key", _defaultSprites["Store Key"], "hasSlyKey", "INV_NAME_STOREKEY",
                "INV_DESC_STOREKEY");
            AddDefaultNormalItem("Love Key", _defaultSprites["Love Key"], "hasLoveKey", "INV_NAME_LOVEKEY",
                "INV_DESC_LOVEKEY");
            AddDefaultFlowerItem("Xun Flower",
                _defaultSprites["Flower"], _defaultSprites["FlowerBroken"],
                "hasXunFlower", "extraFlowerAppear", "xunFlowerBroken",
                "INV_NAME_FLOWER", "INV_NAME_FLOWER_BROKEN",
                "INV_DESC_FLOWER", "INV_DESC_FLOWER_BROKEN", "INV_DESC_FLOWER_QG", "INV_DESC_FLOWER_BROKEN_QG");
            AddDefaultCountedItem("Simple Key", _defaultSprites["Simple Key"], "simpleKeys", "INV_NAME_SIMPLEKEY",
                "INV_DESC_SIMPLEKEY");
            AddDefaultCountedItem("Ore", _defaultSprites["Ore"], "ore", "INV_NAME_ORE", "INV_DESC_ORE");
            AddDefaultCountedItem("Rancid Egg", _defaultSprites["Rancid Egg"], "rancidEggs", "INV_NAME_RANCIDEGG",
                "INV_DESC_RANCIDEGG");

            #endregion

            _initialized = true;
        }

        private static bool CopyJournalPane(GameObject inventoryGo)
        {
            var inventoryFsm = inventoryGo.LocateMyFSM("Inventory Control");
            var inventoryFsmVars = inventoryFsm.FsmVariables;

            if (inventoryFsm.GetState("Closed").Fsm == null)
            {
                inventoryFsm.Preprocess();
            }

            var newPaneGo = Object.Instantiate(inventoryGo.FindGameObjectInChildren("Journal"), inventoryGo.transform);
            newPaneGo.SetActive(false);
            newPaneGo.name = "ItemList";
            var newPaneFod = new FsmOwnerDefault()
            {
                GameObject = newPaneGo,
                OwnerOption = OwnerDefaultOption.SpecifyGameObject
            };

            var newListGo = newPaneGo.FindGameObjectInChildren("Enemy List");
            var newListFod = new FsmOwnerDefault()
            {
                GameObject = newListGo,
                OwnerOption = OwnerDefaultOption.SpecifyGameObject
            };
            UObject.Destroy(newListGo.GetComponent<JournalList>());
            var cli = newListGo.AddComponent<CustomItemList>();
            foreach (var item in _defaultItemList)
            {
                cli.List.Add(item);
            }
            foreach (var item in _customItemList)
            {
                cli.List.Add(item);
            }

            //cli.BuildItemList();
            //cli.UpdateItemList();

            newPaneGo.FindGameObjectInChildren("Enemy Sprite").transform.localScale = new Vector3(3, 3, 3);
            newPaneGo.FindGameObjectInChildren("hunter_symbol").SetActive(false);

            inventoryFsm.AddGameObjectVariable("ItemList Pane");
            inventoryFsm.AddGameObjectVariable("ItemList List");

            int totalPanes = inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).compareTo.Last().Value + 1;

            var newListFsm = newListGo.LocateMyFSM("Item List Control");
            var newListFsmVars = newListFsm.FsmVariables;

            var uiJournalFsm = newPaneGo.LocateMyFSM("UI Journal");

            #region Inventory Control - Init

            inventoryFsm.InsertAction("Init", new FindChild()
            {
                gameObject = inventoryFsm.GetAction<FindChild>("Init", 3).gameObject,
                childName = "ItemList",
                storeResult = inventoryFsmVars.FindFsmGameObject("ItemList Pane")
            }, 12);

            #endregion

            #region Inventory Control - Init Enemy List

            inventoryFsm.AddAction("Init Enemy List", new FindChild()
            {
                gameObject = newPaneFod,
                childName = "Enemy List",
                storeResult = inventoryFsmVars.FindFsmGameObject("ItemList List")
            });
            inventoryFsm.AddAction("Init Enemy List", new CallMethodProper()
            {
                gameObject = newListFod,
                behaviour = "CustomItemList",
                methodName = "BuildItemList",
                parameters = new FsmVar[0],
                storeResult = new FsmVar()
            });
            inventoryFsm.AddAction("Init Enemy List", new ActivateGameObject()
            {
                gameObject = newPaneFod,
                activate = false,
                recursive = false,
                resetOnExit = false,
                everyFrame = false
            });

            #endregion

            #region Inventory Control - Refresh Enemy List

            inventoryFsm.InsertAction("Refresh Enemy List", new CallMethodProper()
            {
                gameObject = newListFod,
                behaviour = "CustomItemList",
                methodName = "UpdateItemList",
                parameters = new FsmVar[0],
                storeResult = new FsmVar()
            }, 2);

            #endregion

            #region Inventory Control - Check Current Pane

            var ccp11Ct = new List<FsmInt>(inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).compareTo)
            {
                totalPanes
            };
            inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).compareTo = ccp11Ct.ToArray();
            var ccp11Se = new List<FsmEvent>(inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).sendEvent)
            {
                FsmEvent.GetFsmEvent("EQUIPMENT")
            };
            inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).sendEvent = ccp11Se.ToArray();

            #endregion

            #region Inventory Control - Open Journal copy "Open Equipment"

            inventoryFsm.CopyState("Open Journal", "Open Equipment");
            inventoryFsm.GetAction<GetLanguageString>("Open Equipment", 0).convName = "PANE_EQUIPMENT";
            inventoryFsm.GetAction<SetIntValue>("Open Equipment", 2).intValue = totalPanes;
            inventoryFsm.GetAction<SetIntValue>("Open Equipment", 3).intValue = totalPanes;
            inventoryFsm.GetAction<SetGameObject>("Open Equipment", 4).gameObject = inventoryFsmVars.FindFsmGameObject("ItemList Pane");
            inventoryFsm.AddTransition("Check Current Pane", "EQUIPMENT", "Open Equipment");

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
                FsmEvent.FindEvent("EQUIPMENT")
            };
            inventoryFsm.GetAction<IntSwitch>("Check R Pane", 1).sendEvent = crp1Se.ToArray();

            #endregion

            #region Inventory Control - Next Journal 2 copy "Next Equipment 2"

            inventoryFsm.CopyState("Next Journal 2", "Next Equipment 2");
            inventoryFsm.GetAction<PlayerDataBoolTest>("Next Equipment 2", 0).boolName = "hasCustomInventoryItem";
            inventoryFsm.GetAction<GetLanguageString>("Next Equipment 2", 1).convName = "PANE_EQUIPMENT";
            inventoryFsm.AddTransition("Check R Pane", "EQUIPMENT", "Next Equipment 2");

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
                FsmEvent.FindEvent("EQUIPMENT")
            };
            inventoryFsm.GetAction<IntSwitch>("Check L Pane", 1).sendEvent = clp1Se.ToArray();

            #endregion

            #region Inventory Control - Next Journal 3 copy "Next Equipment 3"

            inventoryFsm.CopyState("Next Journal 3", "Next Equipment 3");
            inventoryFsm.GetAction<PlayerDataBoolTest>("Next Equipment 3", 0).boolName = "hasCustomInventoryItem";
            inventoryFsm.GetAction<GetLanguageString>("Next Equipment 3", 1).convName = "PANE_EQUIPMENT";
            inventoryFsm.AddTransition("Check L Pane", "EQUIPMENT", "Next Equipment 3");

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
                FsmEvent.FindEvent("EQUIPMENT")
            };
            inventoryFsm.GetAction<IntSwitch>("Loop Through", 3).sendEvent = cls3Se.ToArray();

            #endregion

            #region Inventory Control - Next Journal copy "Next Equipment"

            inventoryFsm.CopyState("Next Journal", "Next Equipment");
            inventoryFsm.GetAction<PlayerDataBoolTest>("Next Equipment", 0).boolName = "hasCustomInventoryItem";
            inventoryFsm.GetAction<SetGameObject>("Next Equipment", 2).gameObject = inventoryFsmVars.FindFsmGameObject("ItemList Pane");
            inventoryFsm.GetAction<GetLanguageString>("Next Equipment", 3).convName = "PANE_EQUIPMENT";
            inventoryFsm.AddTransition("Loop Through", "EQUIPMENT", "Next Equipment");

            inventoryFsm.GetAction<SetIntValue>("Under", 0).intValue = totalPanes + 1;

            #endregion

            #region Enemy List - Item List Control

            #region Init

            newListFsm.GetAction<CallMethodProper>("Init", 15).behaviour = "CustomItemList";
            newListFsm.GetAction<CallMethodProper>("Init", 18).behaviour = "CustomItemList";

            newListFsm.RemoveAction("Init", 28);
            newListFsm.RemoveAction("Init", 27);

            newListFsm.RemoveAction("Init", 13);
            //newListFsm.InsertAction("Init", new CallMethodProper()
            //{
            //    gameObject = newListFOD,
            //    behaviour = "CustomItemList",
            //    methodName = "UpdateItemList",
            //    parameters = new FsmVar[0],
            //    storeResult = new FsmVar()
            //}, 13);

            newListFsm.RemoveAction("Init", 12);
            newListFsm.RemoveAction("Init", 11);

            #endregion

            #region New Item?

            newListFsm.GetAction<CallMethodProper>("New Item?", 0).behaviour = "CustomItemList";

            #endregion

            #region Prev Item

            // Don't let it go to "Prev Item"
            newListFsm.ChangeTransition("New Item?", "CANCEL", "New Item");

            #endregion

            #region Get Details

            newListFsm.GetAction<CallMethodProper>("Get Details", 1).behaviour = "CustomItemList";
            newListFsm.GetAction<GetLanguageString>("Get Details", 2).sheetName = "UI";
            newListFsm.GetAction<CallMethodProper>("Get Details", 4).behaviour = "CustomItemList";
            newListFsm.GetAction<CallMethodProper>("Get Details", 5).behaviour = "CustomItemList";
            newListFsm.GetAction<GetLanguageString>("Get Details", 7).sheetName = "UI";

            newListFsm.RemoveAction("Get Details", 0);

            #endregion

            #region Notes?

            newListFsm.GetAction<CallMethodProper>("Notes?", 0).behaviour = "CustomItemList";

            newListFsm.RemoveAction("Notes?", 1);

            #endregion

            #region Get Notes

            newListFsm.GetAction<SetTextMeshProText>("Get Notes", 5).textString = "";

            newListFsm.RemoveAction("Get Notes", 4);
            newListFsm.RemoveAction("Get Notes", 3);

            newListFsm.ChangeTransition("Get Notes", "FINISHED", "MoveTo");

            #endregion

            #region Display Kills

            newListFsm.GetAction<BuildString>("Display Kills", 4).stringParts = new[]
            {
                "Amount collected: ",
                newListFsmVars.FindFsmString("Kills String")
            };

            newListFsm.ChangeTransition("Display Kills", "FINISHED", "MoveTo");

            #endregion

            #region Check Down

            newListFsm.InsertAction("Check Down", newListFsm.GetAction<CallMethodProper>("Init", 15), 0);

            #endregion

            #region Check Up

            newListFsm.InsertAction("Check Up", newListFsm.GetAction<CallMethodProper>("Init", 15), 0);

            #endregion

            #endregion

            #region Journal go copy - UI Journal fsm

            Object.Destroy(newPaneGo.Find("Text Encountered"));
            newPaneGo.Find("Text Completion").GetComponent<SetTextMeshProGameText>().convName = "ITEMS_COLLECTED";

            uiJournalFsm.RemoveAction("Init", 6);
            uiJournalFsm.RemoveAction("Init", 5);
            uiJournalFsm.RemoveAction("Init", 3);

            uiJournalFsm.GetAction<GetPlayerDataInt>("Completion?", 2).intName = "customItemListGotAmount";
            uiJournalFsm.GetAction<GetPlayerDataInt>("Completion?", 5).intName = "customItemListGotAmount";
            uiJournalFsm.GetAction<GetPlayerDataInt>("Completion?", 8).intName = "customItemListTotalAmount";

            uiJournalFsm.RemoveAction("Completion?", 12);
            uiJournalFsm.RemoveAction("Completion?", 7);
            uiJournalFsm.RemoveAction("Completion?", 6);
            uiJournalFsm.RemoveAction("Completion?", 5);
            uiJournalFsm.RemoveAction("Completion?", 0);

            uiJournalFsm.AddTransition("Active", "DOWN", "Inactive");
            uiJournalFsm.AddTransition("Active", "CHANGE DOWN", "Inactive");


            #endregion

            //newListFsm.MakeLog();
            //inventoryFsm.MakeLog();
            //uiJournalFsm.MakeLog();

            var fg = newPaneGo.GetComponent<FadeGroup>();
            List<SpriteRenderer> tmpSprites = new List<SpriteRenderer>()
            {
                newPaneGo.Find("Arrow D").GetComponent<SpriteRenderer>(),
                newPaneGo.Find("Arrow U").GetComponent<SpriteRenderer>(),
                newPaneGo.Find("divider").GetComponent<SpriteRenderer>(),
                newPaneGo.Find("divider (1)").GetComponent<SpriteRenderer>(),
                newPaneGo.Find("Enemy Sprite").GetComponent<SpriteRenderer>(),
                newPaneGo.Find("hunter_symbol").GetComponent<SpriteRenderer>(),
                newPaneGo.Find("selector").GetComponent<SpriteRenderer>()

            };
            foreach (var sr in newPaneGo.Find("Cursor").GetComponentsInChildren<SpriteRenderer>())
            {
                tmpSprites.Add(sr);
            }
            List<TextMeshPro> tmpTextes = new List<TextMeshPro>()
            {
                newPaneGo.Find("Text Completion").GetComponent<TextMeshPro>(),
                newPaneGo.Find("Text Completion").Find("Amount").GetComponent<TextMeshPro>(),
                newPaneGo.Find("Text Completion").Find("Text Encountered").Find("Amount").GetComponent<TextMeshPro>(),
                newPaneGo.Find("Text Completion").Find("Text Encountered").Find("Total").GetComponent<TextMeshPro>(),
                newPaneGo.Find("Text Completion").Find("Total").GetComponent<TextMeshPro>(),
                newPaneGo.Find("Text Desc").GetComponent<TextMeshPro>(),
                newPaneGo.Find("Text Name").GetComponent<TextMeshPro>(),
                newPaneGo.Find("Text Notes").GetComponent<TextMeshPro>(),
            };
            fg.spriteRenderers = tmpSprites.ToArray();
            fg.texts = tmpTextes.ToArray();

            return true;
        }

        private static string GenerateRandomString(int maxLength = 0xFF)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var randomString = new string(Enumerable.Repeat(chars, Random.Range(1, maxLength + 1)).Select(s => s[Random.Range(0, s.Length)]).ToArray());

            while (_customItemList.Select(x => x.uniqueName).Contains(randomString))
            {
                randomString = new string(Enumerable.Repeat(chars, Random.Range(1, maxLength + 1)).Select(s => s[Random.Range(0, s.Length)]).ToArray());
            }

            return randomString;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Adds a normal item. (e.g. King's Brand)
        ///     You either have it, or you don't.
        /// </summary>
        /// <param name="sprite">Sprite for the item</param>
        /// <param name="playerdataBool">Bool used to determine if the item is acquired</param>
        /// <param name="nameConvo">Language string for the name</param>
        /// <param name="descConvo">Language string for the description</param>
        public static void AddNormalItem(Sprite sprite, string playerdataBool, string nameConvo, string descConvo)
        {
            var uniqueName = GenerateRandomString();
            _customItemList.Add(new Item()
            {
                type = ItemType.Normal,
                uniqueName = uniqueName,
                sprite1 = sprite,
                playerdataBool1 = playerdataBool,
                nameConvo1 = nameConvo,
                descConvo1 = descConvo
            });
        }
        /// <inheritdoc />
        /// <summary>
        ///     Adds a item of type OneTwo. (Not used ingame)
        ///     You either have one, the other or none.
        /// </summary>
        /// <param name="sprite1">Sprite for the item 1</param>
        /// <param name="sprite2">Sprite for the item 2</param>
        /// <param name="playerdataBool1">Bool used to determine if item 1 is acquired</param>
        /// <param name="playerdataBool2">Bool used to determine if item 2 is acquired</param>
        /// <param name="nameConvo1">Language string for name 1</param>
        /// <param name="nameConvo2">Language string for name 2</param>
        /// <param name="descConvo1">Language string for description 1</param>
        /// <param name="descConvo2">Language string for description 2</param>
        public static void AddOneTwoItem(Sprite sprite1, Sprite sprite2, string playerdataBool1, string playerdataBool2, string nameConvo1, string nameConvo2, string descConvo1, string descConvo2)
        {
            var uniqueName = GenerateRandomString();
            _customItemList.Add(new Item()
            {
                type = ItemType.OneTwo,
                uniqueName = uniqueName,
                sprite1 = sprite1,
                playerdataBool1 = playerdataBool1,
                nameConvo1 = nameConvo1,
                descConvo1 = descConvo1,
                sprite2 = sprite2,
                playerdataBool2 = playerdataBool2,
                nameConvo2 = nameConvo2,
                descConvo2 = descConvo2
            });
        }
        /// <inheritdoc />
        /// <summary>
        ///     Adds a item of type OneTwoBoth. (e.g. Map, Quill and Map & Quill)
        ///     You can have one, the other, both or none.
        /// </summary>
        /// <param name="sprite1">Sprite for the item 1</param>
        /// <param name="sprite2">Sprite for the item 2</param>
        /// <param name="spriteBoth">Sprite for the item both</param>
        /// <param name="playerdataBool1">Bool used to determine if item 1 is acquired</param>
        /// <param name="playerdataBool2">Bool used to determine if item 2 is acquired</param>
        /// <param name="nameConvo1">Language string for name 1</param>
        /// <param name="nameConvo2">Language string for name 2</param>
        /// <param name="nameConvoBoth">Language string for name both</param>
        /// <param name="descConvo1">Language string for description 1</param>
        /// <param name="descConvo2">Language string for description 2</param>
        /// <param name="descConvoBoth">Language string for description both</param>
        public static void AddOneTwoBothItem(Sprite sprite1, Sprite sprite2, Sprite spriteBoth, string playerdataBool1, string playerdataBool2, string nameConvo1, string nameConvo2, string nameConvoBoth, string descConvo1, string descConvo2, string descConvoBoth)
        {
            var uniqueName = GenerateRandomString();
            _customItemList.Add(new Item()
            {
                type = ItemType.OneTwoBoth,
                uniqueName = uniqueName,
                sprite1 = sprite1,
                playerdataBool1 = playerdataBool1,
                nameConvo1 = nameConvo1,
                descConvo1 = descConvo1,
                sprite2 = sprite2,
                playerdataBool2 = playerdataBool2,
                nameConvo2 = nameConvo2,
                descConvo2 = descConvo2,
                spriteBoth = spriteBoth,
                nameConvoBoth = nameConvoBoth,
                descConvoBoth = descConvoBoth
            });
        }
        /// <inheritdoc />
        /// <summary>
        ///     Adds a counted item. (e.g. Rancid Egg)
        ///     You either have at least one, or you don't.
        /// </summary>
        /// <param name="sprite">Sprite for the item</param>
        /// <param name="playerdataInt">Int used to determine if and how much of the item is acquired</param>
        /// <param name="nameConvo">Language string for the name</param>
        /// <param name="descConvo">Language string for the description</param>
        public static void AddCountedItem(Sprite sprite, string playerdataInt, string nameConvo, string descConvo)
        {
            var uniqueName = GenerateRandomString();
            _customItemList.Add(new Item()
            {
                type = ItemType.Counted,
                uniqueName = uniqueName,
                sprite1 = sprite,
                playerdataInt = playerdataInt,
                nameConvo1 = nameConvo,
                descConvo1 = descConvo
            });
        }
        /// <inheritdoc />
        /// <summary>
        ///     Adds a flower item. (e.g. the Delicate Flower)
        ///     You have it, it can be broken, but it won't be displayed if you have it from another source and it's broken.
        ///     bool1 && !(bool2 && bool3) and it will be displayed
        /// </summary>
        /// <param name="sprite">Sprite for the 'normal' item</param>
        /// <param name="sprite2">Sprite for the 'broken' item</param>
        /// <param name="playerdataBool1">Bool used to determine if the item is acquired</param>
        /// <param name="playerdataBool2">Bool used to determine if the item is broken</param>
        /// <param name="playerdataBool3">Bool used to determine if the item is from another source (e.g. Queen's Gardens flowers)</param>
        /// <param name="nameConvo1">Language string for the 'normal' name</param>
        /// <param name="nameConvo2">Language string for the 'broken' name</param>
        /// <param name="descConvo1">Language string for the 'normal' description</param>
        /// <param name="descConvo2">Language string for the 'broken' description</param>
        /// <param name="descConvo3">Language string for the 'normal, other source' description</param>
        /// <param name="descConvo4">Language string for the 'broken, other source' description</param>
        public static void AddFlowerItem(Sprite sprite, Sprite sprite2, string playerdataBool1, string playerdataBool2, string playerdataBool3, string nameConvo1, string nameConvo2, string descConvo1, string descConvo2, string descConvo3, string descConvo4)
        {
            var uniqueName = GenerateRandomString();
            _customItemList.Add(new Item()
            {
                type = ItemType.Flower,
                uniqueName = uniqueName,
                sprite1 = sprite,
                sprite2 = sprite2,
                playerdataBool1 = playerdataBool1,
                playerdataBool2 = playerdataBool2,
                playerdataInt = playerdataBool3,
                nameConvo1 = nameConvo1,
                nameConvo2 = nameConvo2,
                descConvo1 = descConvo1,
                descConvo2 = descConvo2,
                nameConvoBoth = descConvo3,
                descConvoBoth = descConvo4
            });
        }

        private static void AddDefaultNormalItem(string uniqueName, Sprite sprite, string playerdataBool, string nameConvo, string descConvo)
        {
            _defaultItemList.Add(new Item()
            {
                type = ItemType.Normal,
                uniqueName = uniqueName,
                sprite1 = sprite,
                playerdataBool1 = playerdataBool,
                nameConvo1 = nameConvo,
                descConvo1 = descConvo
            });
        }
        private static void AddDefaultOneTwoItem(string uniqueName, Sprite sprite1, Sprite sprite2, string playerdataBool1, string playerdataBool2, string nameConvo1, string nameConvo2, string descConvo1, string descConvo2)
        {
            _defaultItemList.Add(new Item()
            {
                type = ItemType.OneTwo,
                uniqueName = uniqueName,
                sprite1 = sprite1,
                playerdataBool1 = playerdataBool1,
                nameConvo1 = nameConvo1,
                descConvo1 = descConvo1,
                sprite2 = sprite2,
                playerdataBool2 = playerdataBool2,
                nameConvo2 = nameConvo2,
                descConvo2 = descConvo2
            });
        }
        private static void AddDefaultOneTwoBothItem(string uniqueName, Sprite sprite1, Sprite sprite2, Sprite spriteBoth, string playerdataBool1, string playerdataBool2, string nameConvo1, string nameConvo2, string nameConvoBoth, string descConvo1, string descConvo2, string descConvoBoth)
        {
            _defaultItemList.Add(new Item()
            {
                type = ItemType.OneTwoBoth,
                uniqueName = uniqueName,
                sprite1 = sprite1,
                playerdataBool1 = playerdataBool1,
                nameConvo1 = nameConvo1,
                descConvo1 = descConvo1,
                sprite2 = sprite2,
                playerdataBool2 = playerdataBool2,
                nameConvo2 = nameConvo2,
                descConvo2 = descConvo2,
                spriteBoth = spriteBoth,
                nameConvoBoth = nameConvoBoth,
                descConvoBoth = descConvoBoth
            });
        }
        private static void AddDefaultCountedItem(string uniqueName, Sprite sprite, string playerdataInt, string nameConvo, string descConvo)
        {
            _defaultItemList.Add(new Item()
            {
                type = ItemType.Counted,
                uniqueName = uniqueName,
                sprite1 = sprite,
                playerdataInt = playerdataInt,
                nameConvo1 = nameConvo,
                descConvo1 = descConvo
            });
        }
        private static void AddDefaultFlowerItem(string uniqueName, Sprite sprite, Sprite sprite2, string playerdataBool1, string playerdataBool2, string playerdataBool3, string nameConvo1, string nameConvo2, string descConvo1, string descConvo2, string descConvo3, string descConvo4)
        {
            _defaultItemList.Add(new Item()
            {
                type = ItemType.Flower,
                uniqueName = uniqueName,
                sprite1 = sprite,
                sprite2 = sprite2,
                playerdataBool1 = playerdataBool1,
                playerdataBool2 = playerdataBool2,
                playerdataInt = playerdataBool3,
                nameConvo1 = nameConvo1,
                nameConvo2 = nameConvo2,
                descConvo1 = descConvo1,
                descConvo2 = descConvo2,
                nameConvoBoth = descConvo3,
                descConvoBoth = descConvo4
            });
        }

        private static void Log(string message)
        {
            Logger.Log($"[SFCore]:[ItemHelper] - {message}");
            Debug.Log($"[SFCore]:[ItemHelper] - {message}");
        }
        private static void Log(object message)
        {
            Log($"{message}");
        }
    }
}
