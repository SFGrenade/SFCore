using System;
using SFCore.Utils;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Logger = Modding.Logger;
using System.Collections.Generic;
using System.Linq;
using Modding;
using TMPro;
using UnityEngine.SceneManagement;
using UObject = UnityEngine.Object;
using SFCore.MonoBehaviours;
using Language;

namespace SFCore
{
    public enum ItemType
    {
        Normal,
        OneTwo,
        OneTwoBoth,
        Counted,
        Flower
    }

    public class ItemHelper
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

        private static Dictionary<string, Sprite> defaultSprites = new Dictionary<string, Sprite>();

        private static List<Item> defaultItemList = new List<Item>();
        private static List<Item> customItemList = new List<Item>();

        private static bool initialized = false;

        static ItemHelper()
        {
            defaultItemList = new List<Item>();
            customItemList = new List<Item>();
            ModHooks.Instance.LanguageGetHook += LanguageGetHook;
            ModHooks.Instance.GetPlayerBoolHook += GetPlayerBoolHook;
            On.GameCameras.Start += GameCamerasOnStart;
        }

        private static bool GetPlayerBoolHook(string originalset)
        {
            if (originalset.Equals("hasCustomInventoryItem"))
            {
                return (CustomItemList.instance != null) && (CustomItemList.instance.hasAtLeastOneItem());
            }
            return PlayerData.instance.GetBoolInternal(originalset);
        }

        private static void GameCamerasOnStart(On.GameCameras.orig_Start orig, GameCameras self)
        {
            orig(self);

            Log("Starting to change inventory");

            #region Display no equipments

            var equipmentGo = self.gameObject.FindGameObjectInChildren("Equipment");
            var equipmentFsm = equipmentGo.LocateMyFSM("Build Equipment List");
            if (equipmentFsm.GetState("Init").Fsm == null)
            {
                Log("Warning: 'Build Equipment List' Fsm not initialized");
                equipmentFsm.Preprocess();
            }
            equipmentFsm.ChangeTransition("Init", "FINISHED", "Pause");

            #endregion

            InitDefaultItems(equipmentGo);

            var successful = CopyJournalPane(self.gameObject.Find("Inventory"));

            if (successful)
            {
                self.gameObject.Find("Inventory").LocateMyFSM("Inventory Control").SetState("Init");
                Log("Finished to change inventory");
            }
            else
            {
                Log("Couldn't finish changing inventory");
            }
        }

        private static string LanguageGetHook(string key, string sheet)
        {
			if (key.Equals("PANE_EQUIPMENT") && sheet.Equals("UI"))
			{
				LanguageCode languageCode = Language.Language.CurrentLanguage();
				switch(languageCode)
				{
					case LanguageCode.EN:
						return "Equipment";
					case LanguageCode.FR:
						return "Équipement";
					case LanguageCode.DE:
						return "Ausrüstung";
					case LanguageCode.ES:
						return "Equipo";
					case LanguageCode.KO:
						return "장비";
					case LanguageCode.ZH_CN:
						return "设备";
					case LanguageCode.IT:
						return "Attrezzature";
					case LanguageCode.PT_BR:
						return "Equipamento";
					case LanguageCode.RU:
						return "Оборудование";
					case LanguageCode.JA:
						return "装置";
				}
			}
            return Language.Language.GetInternal(key, sheet);
        }

        public static void init() { }

        private static void InitDefaultItems(GameObject equipmentGo)
        {
            var equipmentFsm = equipmentGo.LocateMyFSM("Build Equipment List");

            if (equipmentFsm.GetState("Dash").Fsm == null || initialized) return;

            Log("Reconstructing Items");

            #region Populate sprite dictionary

            if (defaultSprites.ContainsKey("Dash"))
                defaultSprites["Dash"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Dash Cloak")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                defaultSprites.Add("Dash",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Dash Cloak").GetComponent<SpriteRenderer>().sprite));
            if (defaultSprites.ContainsKey("ShadowDash"))
                defaultSprites["ShadowDash"] =
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Dash", 16).sprite.Value);
            else
                defaultSprites.Add("ShadowDash",
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Dash", 16).sprite.Value));
            if (defaultSprites.ContainsKey("Walljump"))
                defaultSprites["Walljump"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Mantis Claw")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                defaultSprites.Add("Walljump",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Mantis Claw").GetComponent<SpriteRenderer>().sprite));
            if (defaultSprites.ContainsKey("Super Dash"))
                defaultSprites["Super Dash"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Super Dash")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                defaultSprites.Add("Super Dash",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Super Dash").GetComponent<SpriteRenderer>().sprite));
            if (defaultSprites.ContainsKey("Double Jump"))
                defaultSprites["Double Jump"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Double Jump")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                defaultSprites.Add("Double Jump",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Double Jump").GetComponent<SpriteRenderer>().sprite));
            if (defaultSprites.ContainsKey("Lantern"))
                defaultSprites["Lantern"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Lantern")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                defaultSprites.Add("Lantern",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Lantern").GetComponent<SpriteRenderer>().sprite));
            if (defaultSprites.ContainsKey("Map"))
                defaultSprites["Map"] =
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Map", 1).sprite.Value);
            else
                defaultSprites.Add("Map",
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Map", 1).sprite.Value));
            if (defaultSprites.ContainsKey("Quill"))
                defaultSprites["Quill"] =
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Quill", 1).sprite.Value);
            else
                defaultSprites.Add("Quill",
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Quill", 1).sprite.Value));
            if (defaultSprites.ContainsKey("MapQuill"))
                defaultSprites["MapQuill"] =
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Map and Quill", 1).sprite.Value);
            else
                defaultSprites.Add("MapQuill",
                    (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Map and Quill", 1).sprite.Value));
            if (defaultSprites.ContainsKey("Kings Brand"))
                defaultSprites["Kings Brand"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Kings Brand")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                defaultSprites.Add("Kings Brand",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Kings Brand").GetComponent<SpriteRenderer>().sprite));
            if (defaultSprites.ContainsKey("Tram Pass"))
                defaultSprites["Tram Pass"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Tram Pass")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                defaultSprites.Add("Tram Pass",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Tram Pass").GetComponent<SpriteRenderer>().sprite));
            if (defaultSprites.ContainsKey("City Key"))
                defaultSprites["City Key"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("City Key")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                defaultSprites.Add("City Key",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("City Key").GetComponent<SpriteRenderer>().sprite));
            if (defaultSprites.ContainsKey("Store Key"))
                defaultSprites["Store Key"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Store Key")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                defaultSprites.Add("Store Key",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Store Key").GetComponent<SpriteRenderer>().sprite));
            if (defaultSprites.ContainsKey("Love Key"))
                defaultSprites["Love Key"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Love Key")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                defaultSprites.Add("Love Key",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Love Key").GetComponent<SpriteRenderer>().sprite));
            if (defaultSprites.ContainsKey("Flower"))
                defaultSprites["Flower"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Xun Flower")
                    .GetComponent<InvItemDisplay>().inactiveSprite);
            else
                defaultSprites.Add("Flower",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Xun Flower").GetComponent<InvItemDisplay>()
                        .inactiveSprite));
            if (defaultSprites.ContainsKey("FlowerBroken"))
                defaultSprites["FlowerBroken"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Xun Flower")
                    .GetComponent<InvItemDisplay>().activeSprite);
            else
                defaultSprites.Add("FlowerBroken",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Xun Flower").GetComponent<InvItemDisplay>().activeSprite));
            if (defaultSprites.ContainsKey("Simple Key"))
                defaultSprites["Simple Key"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Xun Flower")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                defaultSprites.Add("Simple Key",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Xun Flower").GetComponent<SpriteRenderer>().sprite));
            if (defaultSprites.ContainsKey("Ore"))
                defaultSprites["Ore"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Ore").GetComponent<SpriteRenderer>()
                    .sprite);
            else
                defaultSprites.Add("Ore",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Ore").GetComponent<SpriteRenderer>().sprite));
            if (defaultSprites.ContainsKey("Rancid Egg"))
                defaultSprites["Rancid Egg"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Rancid Egg")
                    .GetComponent<SpriteRenderer>().sprite);
            else
                defaultSprites.Add("Rancid Egg",
                    (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Rancid Egg").GetComponent<SpriteRenderer>().sprite));

            #endregion

            #region Make new inventory

            AddDefaultOneTwoBothItem("Dash",
                defaultSprites["Dash"], defaultSprites["ShadowDash"], defaultSprites["ShadowDash"],
                "hasDash", "hasShadowDash",
                "INV_NAME_DASH", "INV_NAME_SHADOWDASH", "INV_NAME_SHADOWDASH",
                "INV_DESC_DASH", "INV_DESC_SHADOWDASH", "INV_DESC_SHADOWDASH");
            AddDefaultNormalItem("Walljump", defaultSprites["Walljump"], "hasWalljump", "INV_NAME_WALLJUMP",
                "INV_DESC_WALLJUMP");
            AddDefaultNormalItem("Super Dash", defaultSprites["Super Dash"], "hasSuperDash", "INV_NAME_SUPERDASH",
                "INV_DESC_SUPERDASH");
            AddDefaultNormalItem("Double Jump", defaultSprites["Double Jump"], "hasDoubleJump",
                "INV_NAME_DOUBLEJUMP", "INV_DESC_DOUBLEJUMP");
            AddDefaultNormalItem("Lantern", defaultSprites["Lantern"], "hasLantern", "INV_NAME_LANTERN",
                "INV_DESC_LANTERN");
            AddDefaultOneTwoBothItem("Map Quill",
                defaultSprites["Map"], defaultSprites["Quill"], defaultSprites["MapQuill"],
                "hasMap", "hasQuill",
                "INV_NAME_MAP", "INV_NAME_QUILL", "INV_NAME_MAPQUILL",
                "INV_DESC_MAP", "INV_DESC_QUILL", "INV_DESC_MAPQUILL");
            AddDefaultNormalItem("Kings Brand", defaultSprites["Kings Brand"], "hasKingsBrand",
                "INV_NAME_KINGSBRAND", "INV_DESC_KINGSBRAND");
            AddDefaultNormalItem("Tram Pass", defaultSprites["Tram Pass"], "hasTramPass", "INV_NAME_TRAM_PASS",
                "INV_DESC_TRAM_PASS");
            AddDefaultNormalItem("City Key", defaultSprites["City Key"], "hasCityKey", "INV_NAME_CITYKEY",
                "INV_DESC_CITYKEY");
            AddDefaultNormalItem("Store Key", defaultSprites["Store Key"], "hasSlyKey", "INV_NAME_STOREKEY",
                "INV_DESC_STOREKEY");
            AddDefaultNormalItem("Love Key", defaultSprites["Love Key"], "hasLoveKey", "INV_NAME_LOVEKEY",
                "INV_DESC_LOVEKEY");
            AddDefaultFlowerItem("Xun Flower",
                defaultSprites["Flower"], defaultSprites["FlowerBroken"],
                "hasXunFlower", "extraFlowerAppear", "xunFlowerBroken",
                "INV_NAME_FLOWER", "INV_NAME_FLOWER_BROKEN",
                "INV_DESC_FLOWER", "INV_DESC_FLOWER_BROKEN", "INV_DESC_FLOWER_QG", "INV_DESC_FLOWER_BROKEN_QG");
            AddDefaultCountedItem("Simple Key", defaultSprites["Simple Key"], "simpleKeys", "INV_NAME_SIMPLEKEY",
                "INV_DESC_SIMPLEKEY");
            AddDefaultCountedItem("Ore", defaultSprites["Ore"], "ore", "INV_NAME_ORE", "INV_DESC_ORE");
            AddDefaultCountedItem("Rancid Egg", defaultSprites["Rancid Egg"], "rancidEggs", "INV_NAME_RANCIDEGG",
                "INV_DESC_RANCIDEGG");

            #endregion

            foreach (var pair in defaultSprites)
            {
                Log($"Sprite '{pair.Key}': '{pair.Value}'");
            }

            initialized = true;

            Log("Items Reconstructed");
        }

        private static bool CopyJournalPane(GameObject inventoryGo)
        {
            var inventoryFsm = inventoryGo.LocateMyFSM("Inventory Control");
            var inventoryFsmVars = inventoryFsm.FsmVariables;

            if (inventoryFsm.GetState("Closed").Fsm == null)
            {
                Log("Warning: 'Inventory Control' Fsm not initialized");
                inventoryFsm.Preprocess();
            }

            var newPaneGo = GameObject.Instantiate(inventoryGo.FindGameObjectInChildren("Journal"), inventoryGo.transform);
            newPaneGo.SetActive(false);
            newPaneGo.name = "ItemList";
            var newPaneFOD = new FsmOwnerDefault()
            {
                GameObject = newPaneGo,
                OwnerOption = OwnerDefaultOption.SpecifyGameObject
            };

            var uiJournalFsm = newPaneGo.LocateMyFSM("UI Journal");
            uiJournalFsm.RemoveAction("Completion?", 0);
            uiJournalFsm.RemoveAction("Completion?", 0);
            uiJournalFsm.RemoveAction("Completion?", 0);
            uiJournalFsm.RemoveAction("Completion?", 0);
            uiJournalFsm.RemoveAction("Completion?", 0);
            uiJournalFsm.RemoveAction("Completion?", 0);
            uiJournalFsm.RemoveAction("Completion?", 0);
            uiJournalFsm.RemoveAction("Completion?", 0);
            uiJournalFsm.RemoveAction("Completion?", 0);
            uiJournalFsm.RemoveAction("Completion?", 0);
            uiJournalFsm.RemoveAction("Completion?", 0);
            uiJournalFsm.RemoveAction("Completion?", 0);
            uiJournalFsm.RemoveAction("Completion?", 0);

            var newListGo = newPaneGo.FindGameObjectInChildren("Enemy List");
            var newListFOD = new FsmOwnerDefault()
            {
                GameObject = newListGo,
                OwnerOption = OwnerDefaultOption.SpecifyGameObject
            };
            UObject.Destroy(newListGo.GetComponent<JournalList>());
            var cli = newListGo.AddComponent<CustomItemList>();
            foreach (var item in defaultItemList)
            {
                cli.list.Add(item);
            }
            foreach (var item in customItemList)
            {
                cli.list.Add(item);
            }

            cli.yDistance = -2;
            cli.BuildItemList();
            cli.UpdateItemList();
            newPaneGo.FindGameObjectInChildren("Enemy Sprite").transform.localScale = new Vector3(3, 3, 3);
            newPaneGo.FindGameObjectInChildren("hunter_symbol").SetActive(false);

            inventoryFsm.AddGameObjectVariable("ItemList Pane");
            inventoryFsm.AddGameObjectVariable("ItemList List");

            // Edit Inventory FSM to also save the new child
            inventoryFsm.InsertAction("Init", new FindChild()
            {
                gameObject = inventoryFsm.GetAction<FindChild>("Init", 3).gameObject,
                childName = "ItemList",
                storeResult = inventoryFsmVars.FindFsmGameObject("ItemList Pane")
            }, 12);

            // Edit Inventory FSM to also save the new child
            inventoryFsm.InsertAction("Init Enemy List", new FindChild()
            {
                gameObject = newPaneFOD,
                childName = "Enemy List",
                storeResult = inventoryFsmVars.FindFsmGameObject("ItemList List")
            }, 2);
            inventoryFsm.InsertAction("Init Enemy List", new CallMethodProper()
            {
                gameObject = newListFOD,
                behaviour = "CustomItemList",
                methodName = "BuildItemList",
                parameters = new FsmVar[0],
                storeResult = new FsmVar()
            }, 3);
            inventoryFsm.InsertAction("Init Enemy List", new ActivateGameObject()
            {
                gameObject = newPaneFOD,
                activate = false,
                recursive = false,
                resetOnExit = false,
                everyFrame = false
            }, 5);

            inventoryFsm.InsertAction("Refresh Enemy List", new CallMethodProper()
            {
                gameObject = newListFOD,
                behaviour = "CustomItemList",
                methodName = "UpdateItemList",
                parameters = new FsmVar[0],
                storeResult = new FsmVar()
            }, 2);

            #region new Open state

            inventoryFsm.CopyState("Open Journal", "Open Equipment");
            inventoryFsm.GetAction<GetLanguageString>("Open Equipment", 0).convName = "PANE_EQUIPMENT";
            inventoryFsm.GetAction<SetIntValue>("Open Equipment", 2).intValue = 4;
            inventoryFsm.GetAction<SetIntValue>("Open Equipment", 3).intValue = 4;
            inventoryFsm.GetAction<SetGameObject>("Open Equipment", 4).gameObject = inventoryFsmVars.FindFsmGameObject("ItemList Pane");
            inventoryFsm.AddTransition("Check Current Pane", "EQUIPMENT", "Open Equipment");

            #endregion

            #region Check Current Pane state

            var ccp11ct = new List<FsmInt>(inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).compareTo)
            {
                4
            };
            inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).compareTo = ccp11ct.ToArray();
            var ccp11se = new List<FsmEvent>(inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).sendEvent)
            {
                FsmEvent.FindEvent("EQUIPMENT")
            };
            inventoryFsm.GetAction<IntSwitch>("Check Current Pane", 11).sendEvent = ccp11se.ToArray();

            #endregion

            #region new Check R state

            inventoryFsm.CopyState("Next Journal 2", "Next Equipment 2");
            inventoryFsm.GetAction<PlayerDataBoolTest>("Next Equipment 2", 0).boolName = "hasCustomInventoryItem";
            inventoryFsm.GetAction<GetLanguageString>("Next Equipment 2", 1).convName = "PANE_EQUIPMENT";
            inventoryFsm.AddTransition("Check R Pane", "EQUIPMENT", "Next Equipment 2");

            inventoryFsm.GetAction<SetIntValue>("Under 2", 0).intValue = 5;

            #endregion

            #region Check R Pane state

            var crp1ct = new List<FsmInt>(inventoryFsm.GetAction<IntSwitch>("Check R Pane", 1).compareTo)
            {
                4
            };
            crp1ct[5] = 5;
            inventoryFsm.GetAction<IntSwitch>("Check R Pane", 1).compareTo = crp1ct.ToArray();
            var crp1se = new List<FsmEvent>(inventoryFsm.GetAction<IntSwitch>("Check R Pane", 1).sendEvent)
            {
                FsmEvent.FindEvent("EQUIPMENT")
            };
            inventoryFsm.GetAction<IntSwitch>("Check R Pane", 1).sendEvent = crp1se.ToArray();

            #endregion

            #region new Check L state

            inventoryFsm.CopyState("Next Journal 3", "Next Equipment 3");
            inventoryFsm.GetAction<PlayerDataBoolTest>("Next Equipment 3", 0).boolName = "hasCustomInventoryItem";
            inventoryFsm.GetAction<GetLanguageString>("Next Equipment 3", 1).convName = "PANE_EQUIPMENT";
            inventoryFsm.AddTransition("Check L Pane", "EQUIPMENT", "Next Equipment 3");

            inventoryFsm.GetAction<SetIntValue>("Under 3", 0).intValue = 5;

            #endregion

            #region Check L Pane state

            var clp1ct = new List<FsmInt>(inventoryFsm.GetAction<IntSwitch>("Check L Pane", 1).compareTo)
            {
                4
            };
            clp1ct[5] = 5;
            inventoryFsm.GetAction<IntSwitch>("Check L Pane", 1).compareTo = clp1ct.ToArray();
            var clp1se = new List<FsmEvent>(inventoryFsm.GetAction<IntSwitch>("Check L Pane", 1).sendEvent)
            {
                FsmEvent.FindEvent("EQUIPMENT")
            };
            inventoryFsm.GetAction<IntSwitch>("Check L Pane", 1).sendEvent = clp1se.ToArray();

            #endregion

            #region new Loop state

            inventoryFsm.CopyState("Next Journal", "Next Equipment");
            inventoryFsm.GetAction<PlayerDataBoolTest>("Next Equipment", 0).boolName = "hasCustomInventoryItem";
            inventoryFsm.GetAction<SetGameObject>("Next Equipment", 2).gameObject = inventoryFsmVars.FindFsmGameObject("ItemList Pane");
            inventoryFsm.GetAction<GetLanguageString>("Next Equipment", 3).convName = "PANE_EQUIPMENT";
            inventoryFsm.AddTransition("Loop Through", "EQUIPMENT", "Next Equipment");

            inventoryFsm.GetAction<SetIntValue>("Under", 0).intValue = 5;

            #endregion

            #region Check Loop state

            var cls3ct = new List<FsmInt>(inventoryFsm.GetAction<IntSwitch>("Loop Through", 3).compareTo)
            {
                4
            };
            cls3ct[5] = 5;
            inventoryFsm.GetAction<IntSwitch>("Loop Through", 3).compareTo = cls3ct.ToArray();
            var cls3se = new List<FsmEvent>(inventoryFsm.GetAction<IntSwitch>("Loop Through", 3).sendEvent)
            {
                FsmEvent.FindEvent("EQUIPMENT")
            };
            inventoryFsm.GetAction<IntSwitch>("Loop Through", 3).sendEvent = cls3se.ToArray();

            #endregion

            #region Enemy List - Item List Control

            var newListFsm = newListGo.LocateMyFSM("Item List Control");
            var newListFsmVars = newListFsm.FsmVariables;
            newListFsm.GetAction<CallMethodProper>("Init", 15).behaviour = "CustomItemList";
            newListFsm.GetAction<CallMethodProper>("Init", 18).behaviour = "CustomItemList";
            newListFsm.RemoveAction("Init", 13);
            newListFsm.GetAction<CallMethodProper>("New Item?", 0).behaviour = "CustomItemList";
            newListFsm.AddAction("Prev Item", new SetIntValue()
            {
                intVariable = newListFsmVars.FindFsmInt("Current Item"),
                intValue = 0,
                everyFrame = false
            });
            newListFsm.RemoveAction("Prev Item", 0);
            newListFsm.GetAction<CallMethodProper>("Get Details", 1).behaviour = "CustomItemList";
            newListFsm.GetAction<GetLanguageString>("Get Details", 2).sheetName = "UI";
            newListFsm.GetAction<CallMethodProper>("Get Details", 4).behaviour = "CustomItemList";
            newListFsm.GetAction<CallMethodProper>("Get Details", 5).behaviour = "CustomItemList";
            newListFsm.GetAction<GetLanguageString>("Get Details", 7).sheetName = "UI";
            newListFsm.RemoveAction("Get Details", 0);

            newListFsm.GetAction<CallMethodProper>("Notes?", 0).behaviour = "CustomItemList";
            newListFsm.RemoveAction("Notes?", 1);
            newListFsm.GetAction<SetTextMeshProText>("Get Notes", 5).textString = "";
            newListFsm.RemoveAction("Get Notes", 4);
            newListFsm.RemoveAction("Get Notes", 3);
            newListFsm.GetAction<BuildString>("Display Kills", 4).stringParts = new FsmString[]
            {
                "Amount collected: ",
                newListFsmVars.FindFsmString("Kills String")
            };

            newListFsm.ChangeTransition("Display Kills", "FINISHED", "MoveTo");
            newListFsm.ChangeTransition("Get Notes", "FINISHED", "MoveTo");
            newListFsm.InsertAction("Init", new CallMethodProper()
            {
                gameObject = newListFOD,
                behaviour = "CustomItemList",
                methodName = "UpdateItemList",
                parameters = new FsmVar[0],
                storeResult = new FsmVar()
            }, 13);

            #endregion

            inventoryFsm.AddMethod("Close", () =>
            {
                uiJournalFsm.SetState("Inactive");
            });

            newListFsm.MakeLog();
            inventoryFsm.MakeLog();
            uiJournalFsm.MakeLog();

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

        public static void AddNormalItem(string uniqueName, Sprite sprite, string playerdataBool, string nameConvo, string descConvo)
        {
            customItemList.Add(new Item()
            {
                type = ItemType.Normal,
                uniqueName = uniqueName,
                sprite1 = sprite,
                playerdataBool1 = playerdataBool,
                nameConvo1 = nameConvo,
                descConvo1 = descConvo
            });
        }
        public static void AddOneTwoItem(string uniqueName, Sprite sprite1, Sprite sprite2, string playerdataBool1, string playerdataBool2, string nameConvo1, string nameConvo2, string descConvo1, string descConvo2)
        {
            customItemList.Add(new Item()
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
        public static void AddOneTwoBothItem(string uniqueName, Sprite sprite1, Sprite sprite2, Sprite spriteBoth, string playerdataBool1, string playerdataBool2, string nameConvo1, string nameConvo2, string nameConvoBoth, string descConvo1, string descConvo2, string descConvoBoth)
        {
            customItemList.Add(new Item()
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
        public static void AddCountedItem(string uniqueName, Sprite sprite, string playerdataInt, string nameConvo, string descConvo)
        {
            customItemList.Add(new Item()
            {
                type = ItemType.Counted,
                uniqueName = uniqueName,
                sprite1 = sprite,
                playerdataInt = playerdataInt,
                nameConvo1 = nameConvo,
                descConvo1 = descConvo
            });
        }
        public static void AddFlowerItem(string uniqueName, Sprite sprite, Sprite sprite2, string playerdataBool1, string playerdataBool2, string playerdataBool3, string nameConvo1, string nameConvo2, string descConvo1, string descConvo2, string descConvo3, string descConvo4)
        {
            customItemList.Add(new Item()
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
            defaultItemList.Add(new Item()
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
            defaultItemList.Add(new Item()
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
            defaultItemList.Add(new Item()
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
            defaultItemList.Add(new Item()
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
            defaultItemList.Add(new Item()
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
