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
        /// <summary>
        ///     A normal item with a single PD bool to indicate whether or not the item was acquired.
        /// </summary>
        Normal,
        /// <summary>
        ///     An item with 2 PD bools to indicate whether or not one item or another was acquired, not both.
        /// </summary>
        OneTwo,
        /// <summary>
        ///     An item with 2 PD bools to indicate whether or not one, another or both items were acquired.
        /// </summary>
        OneTwoBoth,
        /// <summary>
        ///     An item with a single PD int to indicate whether or not and how many of an item were acquired.
        /// </summary>
        Counted,
        /// <summary>
        ///     An item to mimic the delicate flower.
        ///     It has 2 sprites, 2 names and 4 descriptions.
        ///     The item counts as acquired when playerdataBool1 is true and either of playerdataBool2 and playerdataBool3 (playerdataInt) is false.
        ///     sprite1 is used when playerdataBool3 (playerdataInt) is false. Otherwise sprite2 is used.
        ///     nameConvo1 is used when playerdataBool3 (playerdataInt) is false. Otherwise nameConvo2 is used.
        ///     descConvo1 is used when both playerdataBool3 (playerdataInt) and playerdataBool2 are false.
        ///     descConvo2 is used when playerdataBool3 (playerdataInt) is true and playerdataBool2 is false.
        ///     descConvo3 (nameConvoBoth) is used when playerdataBool3 (playerdataInt) is false and playerdataBool2 is true.
        ///     descConvo4 (descConvoBoth) is used when both playerdataBool3 (playerdataInt) and playerdataBool2 are true.
        /// </summary>
        Flower
    }

    /// <summary>
    ///     Item helper class for easily adding custom items.
    ///     The mod using this needs to handle the following:
    ///     - 0 to 3 PlayerData bools per item
    ///     - 0 to 1 PlayerData int per item
    ///     - 1 to 3 name language strings per item
    ///     - 1 to 4 description language strings per item
    /// </summary>
    public static class ItemHelper
    {
        /// <summary>
        ///     Data of one item.
        /// </summary>
        public struct Item
        {
            /// <summary>
            ///     Type of the item.
            /// </summary>
            public ItemType type;

            /// <summary>
            ///     Unique name for FSM purposes.
            /// </summary>
            public string uniqueName;

            /// <summary>
            ///     Main sprite.
            /// </summary>
            public Sprite sprite1;

            /// <summary>
            ///     Alternative sprite.
            /// </summary>
            public Sprite sprite2;

            /// <summary>
            ///     Another alternative sprite.
            /// </summary>
            public Sprite spriteBoth;

            /// <summary>
            ///     PlayerData bool.
            /// </summary>
            public string playerdataBool1;

            /// <summary>
            ///     PlayerData bool.
            /// </summary>
            public string playerdataBool2;

            /// <summary>
            ///     PlayerData int, sometimes used for a bool.
            /// </summary>
            public string playerdataInt;

            /// <summary>
            ///     Main name language key.
            /// </summary>
            public string nameConvo1;

            /// <summary>
            ///     Alternative name language key.
            /// </summary>
            public string nameConvo2;

            /// <summary>
            ///     Another alternative name language key, sometimes used for a description.
            /// </summary>
            public string nameConvoBoth;

            /// <summary>
            ///     Main description language key.
            /// </summary>
            public string descConvo1;

            /// <summary>
            ///     Alternative description language key.
            /// </summary>
            public string descConvo2;

            /// <summary>
            ///     Another alternative description language key.
            /// </summary>
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
            InventoryHelper.AddInventoryPage(InventoryPageType.Journal, "ItemList", "PANE_EQUIPMENT", "EQUIPMENT", "hasCustomInventoryItem", CreateEquipmentPane);
        }

        /// <summary>
        ///     Used for static initialization.
        /// </summary>
        public static void unusedInit()
        {
        }

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

            _defaultSprites["Dash"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Dash Cloak").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["ShadowDash"] = (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Dash", 16).sprite.Value);
            _defaultSprites["Walljump"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Mantis Claw").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["Super Dash"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Super Dash").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["Double Jump"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Double Jump").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["Acid Armour"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Acid Armour").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["Lantern"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Lantern").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["Map"] = (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Map", 1).sprite.Value);
            _defaultSprites["Quill"] = (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Quill", 1).sprite.Value);
            _defaultSprites["MapQuill"] = (Sprite) UObject.Instantiate(equipmentFsm.GetAction<SetSpriteRendererSprite>("Map and Quill", 1).sprite.Value);
            _defaultSprites["Kings Brand"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Kings Brand").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["Tram Pass"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Tram Pass").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["City Key"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("City Key").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["Store Key"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Store Key").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["White Key"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("White Key").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["Love Key"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Love Key").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["Flower"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Xun Flower").GetComponent<InvItemDisplay>().inactiveSprite);
            _defaultSprites["FlowerBroken"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Xun Flower").GetComponent<InvItemDisplay>().activeSprite);
            _defaultSprites["Simple Key"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Simple Key").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["Ore"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Ore").GetComponent<SpriteRenderer>().sprite);
            _defaultSprites["Rancid Egg"] = (Sprite) UObject.Instantiate(equipmentGo.FindGameObjectInChildren("Rancid Egg").GetComponent<SpriteRenderer>().sprite);

            #endregion

            #region Make new inventory

            AddDefaultOneTwoBothItem("Dash", _defaultSprites["Dash"], _defaultSprites["ShadowDash"], _defaultSprites["ShadowDash"], "hasDash", "hasShadowDash", "INV_NAME_DASH", "INV_NAME_SHADOWDASH", "INV_NAME_SHADOWDASH", "INV_DESC_DASH", "INV_DESC_SHADOWDASH", "INV_DESC_SHADOWDASH");
            AddDefaultNormalItem("Walljump", _defaultSprites["Walljump"], "hasWalljump", "INV_NAME_WALLJUMP", "INV_DESC_WALLJUMP");
            AddDefaultNormalItem("Super Dash", _defaultSprites["Super Dash"], "hasSuperDash", "INV_NAME_SUPERDASH", "INV_DESC_SUPERDASH");
            AddDefaultNormalItem("Double Jump", _defaultSprites["Double Jump"], "hasDoubleJump", "INV_NAME_DOUBLEJUMP", "INV_DESC_DOUBLEJUMP");
            AddDefaultNormalItem("Acid Armour", _defaultSprites["Acid Armour"], "hasAcidArmour", "INV_NAME_ACIDARMOUR", "INV_DESC_ACIDARMOUR");
            AddDefaultNormalItem("Lantern", _defaultSprites["Lantern"], "hasLantern", "INV_NAME_LANTERN", "INV_DESC_LANTERN");
            AddDefaultOneTwoBothItem("Map Quill", _defaultSprites["Map"], _defaultSprites["Quill"], _defaultSprites["MapQuill"], "hasMap", "hasQuill", "INV_NAME_MAP", "INV_NAME_QUILL", "INV_NAME_MAPQUILL", "INV_DESC_MAP", "INV_DESC_QUILL", "INV_DESC_MAPQUILL");
            AddDefaultNormalItem("Kings Brand", _defaultSprites["Kings Brand"], "hasKingsBrand", "INV_NAME_KINGSBRAND", "INV_DESC_KINGSBRAND");
            AddDefaultNormalItem("Tram Pass", _defaultSprites["Tram Pass"], "hasTramPass", "INV_NAME_TRAM_PASS", "INV_DESC_TRAM_PASS");
            AddDefaultNormalItem("City Key", _defaultSprites["City Key"], "hasCityKey", "INV_NAME_CITYKEY", "INV_DESC_CITYKEY");
            AddDefaultNormalItem("Store Key", _defaultSprites["Store Key"], "hasSlyKey", "INV_NAME_STOREKEY", "INV_DESC_STOREKEY");
            AddDefaultNormalItem("White Key", _defaultSprites["White Key"], "hasWhiteKey", "INV_NAME_WHITEKEY", "INV_DESC_WHITEKEY");
            AddDefaultNormalItem("Love Key", _defaultSprites["Love Key"], "hasLoveKey", "INV_NAME_LOVEKEY", "INV_DESC_LOVEKEY");
            AddDefaultFlowerItem("Xun Flower", _defaultSprites["Flower"], _defaultSprites["FlowerBroken"], "hasXunFlower", "extraFlowerAppear", "xunFlowerBroken", "INV_NAME_FLOWER", "INV_NAME_FLOWER_BROKEN", "INV_DESC_FLOWER", "INV_DESC_FLOWER_BROKEN", "INV_DESC_FLOWER_QG", "INV_DESC_FLOWER_BROKEN_QG");
            AddDefaultCountedItem("Simple Key", _defaultSprites["Simple Key"], "simpleKeys", "INV_NAME_SIMPLEKEY", "INV_DESC_SIMPLEKEY");
            AddDefaultCountedItem("Ore", _defaultSprites["Ore"], "ore", "INV_NAME_ORE", "INV_DESC_ORE");
            AddDefaultCountedItem("Rancid Egg", _defaultSprites["Rancid Egg"], "rancidEggs", "INV_NAME_RANCIDEGG", "INV_DESC_RANCIDEGG");

            #endregion

            _initialized = true;
        }

        private static void CreateEquipmentPane(GameObject newPaneGo)
        {
            GameObject inventoryGo = newPaneGo.transform.parent.gameObject;

            GameObject equipmentGo = inventoryGo.transform.Find("Inv").Find("Equipment").gameObject;
            var equipmentFsm = equipmentGo.LocateMyFSM("Build Equipment List");
            equipmentFsm.ChangeTransition("Init", "FINISHED", "Pause");
            InitDefaultItems(equipmentGo);
            
            var inventoryFsm = inventoryGo.LocateMyFSM("Inventory Control");
            var inventoryFsmVars = inventoryFsm.FsmVariables;

            if (inventoryFsm.GetState("Closed").Fsm == null)
            {
                inventoryFsm.Preprocess();
            }

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

            var newListFsm = newListGo.LocateMyFSM("Item List Control Custom");
            var newListFsmVars = newListFsm.FsmVariables;

            var uiJournalFsm = newPaneGo.LocateMyFSM("UI Journal Custom");

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

            #region Enemy List - Item List Control

            #region Init

            newListFsm.GetAction<CallMethodProper>("Init", 15).behaviour = "CustomItemList";
            newListFsm.GetAction<CallMethodProper>("Init", 18).behaviour = "CustomItemList";

            newListFsm.RemoveAction("Init", 28);
            newListFsm.RemoveAction("Init", 27);

            newListFsm.RemoveAction("Init", 13);

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

        /// <summary>
        ///     Adds a item of type OneTwoBoth. (e.g. Map, Quill and Map and Quill)
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

        /// <summary>
        ///     Adds a flower item. (e.g. the Delicate Flower)
        ///     You have it, it can be broken, but it won't be displayed if you have it from another source and it's broken.
        ///     (bool1 AND !(bool2 AND bool3)) and it will be displayed
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
        
        private static void LogFine(string message) => InternalLogger.LogFine(message, "[SFCore]:[ItemHelper]");
        private static void LogFine(object message) => LogFine($"{message}");
        private static void LogDebug(string message) => InternalLogger.LogDebug(message, "[SFCore]:[ItemHelper]");
        private static void LogDebug(object message) => LogDebug($"{message}");
        private static void Log(string message) => InternalLogger.Log(message, "[SFCore]:[ItemHelper]");
        private static void Log(object message) => Log($"{message}");
        private static void LogWarn(string message) => InternalLogger.LogWarn(message, "[SFCore]:[ItemHelper]");
        private static void LogWarn(object message) => LogWarn($"{message}");
        private static void LogError(string message) => InternalLogger.LogError(message, "[SFCore]:[ItemHelper]");
        private static void LogError(object message) => LogError($"{message}");
    }
}
