using SFCore.Utils;
using System.Collections.Generic;
using System.Linq;
using TMProOld;
using UnityEngine;
using Logger = Modding.Logger;

namespace SFCore.MonoBehaviours;

/// <summary>
/// Custom Item List for ItemHelper
/// </summary>
public class CustomItemList : MonoBehaviour
{
    /// <summary>
    /// Instance
    /// </summary>
    public static CustomItemList Instance { get; private set; }

    /// <summary>
    /// Data List
    /// </summary>
    public List<ItemHelper.Item> List { get; private set; } = new List<ItemHelper.Item>();
    /// <summary>
    /// GameObject List
    /// </summary>
    public GameObject[] ListInv { get; private set; } = new GameObject[0];
    private GameObject[] _currentList = new GameObject[0];
    private PlayerData _pd = PlayerData.instance;
    /// <summary>
    /// Y Distance between each GameObject
    /// </summary>
    public float YDistance { get; private set; } = -2f;
    /// <summary>
    /// Amount of Items
    /// </summary>
    public int ItemCount { get; private set; } = -1;
    /// <summary>
    /// Index of new item
    /// </summary>
    public int FirstNewItem { get; private set; }
    private bool _built = false;

    /// <summary>
    /// Constructor
    /// </summary>
    public CustomItemList()
    {
        Instance = this;
    }

    /// <summary>
    /// True if at least one item is gotten.
    /// </summary>
    public bool hasAtLeastOneItem()
    {
        if (_currentList.Length > 0)
            return _currentList[0] != null;
        return false;
    }

    /// <summary>
    /// Total item amount
    /// </summary>
    public int TotalItemAmount() => List.Count;

    /// <summary>
    /// Amount of gotten items
    /// </summary>
    public int GotItemAmount() => ItemCount + 1;

    /// <summary>
    /// Build the inv list
    /// </summary>
    public void BuildItemList()
    {
        _pd = PlayerData.instance;
        FirstNewItem = -1;
        ItemCount = -1;
        ListInv = new GameObject[List.Count];
        var prefab = Instantiate(Resources.FindObjectsOfTypeAll<JournalEntryStats>()
            .First(x => x.gameObject.name.Equals("Journal Crawler")).gameObject);
        Destroy(prefab.GetComponent<JournalEntryStats>());
        prefab.FindGameObjectInChildren("Name").GetComponent<SetTextMeshProGameText>().sheetName = "UI";
        for (int i = 0; i < List.Count; i++)
        {
            ItemCount++;
            GameObject gameObject = MakeGameObject(List[i], prefab);
            gameObject.transform.SetParent(transform, false);
            gameObject.SetActive(false);
            ListInv[ItemCount] = gameObject;
        }

        AddToFadeGroup();

        _built = true;
    }

    /// <summary>
    /// Update the inv list
    /// </summary>
    public void UpdateItemList()
    {
        if (!_built) BuildItemList();

        FirstNewItem = 0;
        ItemCount = -1;
        float num = 0f;
        _currentList = new GameObject[ListInv.Length];
        for (int i = 0; i < ListInv.Length; i++)
        {
            GameObject gameObject = ListInv[i];
            MakeSpritesText(gameObject, List[i]);

            var tmpBool = CheckBool(List[i]);
            gameObject.SetActive(tmpBool);
            if (tmpBool)
            {
                ItemCount++;
                _currentList[ItemCount] = gameObject;
                gameObject.transform.localPosition = new Vector3(0f, num, 0f);
                num += YDistance;
            }
            else
            {
                gameObject.transform.localPosition = new Vector3(-500, -500, -500);
            }
        }
    }

    private GameObject MakeGameObject(ItemHelper.Item item, GameObject prefab)
    {
        var ret = Instantiate(prefab);
        ret.name = item.uniqueName;
        ret.transform.localPosition = Vector3.zero;

        MakeSpritesText(ret, item);

        ret.SetActive(true);
        ret.Find("Portrait").SetActive(true);
        ret.Find("Name").SetActive(true);
        return ret;
    }

    private void MakeSpritesText(GameObject go, ItemHelper.Item item)
    {
        var portrait = go.FindGameObjectInChildren("Portrait");
        var portraitSr = portrait.GetComponent<SpriteRenderer>();
        var name = go.FindGameObjectInChildren("Name");
        var nameText = name.GetComponent<SetTextMeshProGameText>();

        switch (item.type)
        {
            case ItemType.Normal:
                portraitSr.sprite = item.sprite1;
                nameText.convName = item.nameConvo1;
                break;
            case ItemType.OneTwo:
                if (_pd.GetBool(item.playerdataBool1))
                {
                    portraitSr.sprite = item.sprite1;
                    nameText.convName = item.nameConvo1;
                }
                else
                {
                    portraitSr.sprite = item.sprite2;
                    nameText.convName = item.nameConvo2;
                }
                break;
            case ItemType.OneTwoBoth:
                if (_pd.GetBool(item.playerdataBool1) && !_pd.GetBool(item.playerdataBool2))
                {
                    portraitSr.sprite = item.sprite1;
                    nameText.convName = item.nameConvo1;
                }
                else if (!_pd.GetBool(item.playerdataBool1) && _pd.GetBool(item.playerdataBool2))
                {
                    portraitSr.sprite = item.sprite2;
                    nameText.convName = item.nameConvo2;
                }
                else
                {
                    portraitSr.sprite = item.spriteBoth;
                    nameText.convName = item.nameConvoBoth;
                }
                break;
            case ItemType.Counted:
                portraitSr.sprite = item.sprite1;
                nameText.convName = item.nameConvo1;
                break;
            case ItemType.Flower:
                if (_pd.GetBool(item.playerdataInt)) // xunFlowerBroken
                {
                    portraitSr.sprite = item.sprite2;
                    nameText.convName = item.nameConvo2;
                }
                else
                {
                    portraitSr.sprite = item.sprite1;
                    nameText.convName = item.nameConvo1;
                }
                break;
        }
    }

    private bool CheckBool(ItemHelper.Item item)
    {
        switch (item.type)
        {
            case ItemType.Normal:
                return _pd.GetBool(item.playerdataBool1);
            case ItemType.OneTwo:
                return _pd.GetBool(item.playerdataBool1) ^ _pd.GetBool(item.playerdataBool2);
            case ItemType.OneTwoBoth:
                return _pd.GetBool(item.playerdataBool1) || _pd.GetBool(item.playerdataBool2);
            case ItemType.Counted:
                return _pd.GetInt(item.playerdataInt) > 0;
            case ItemType.Flower:
                return _pd.GetBool(item.playerdataBool1) && !(_pd.GetBool(item.playerdataBool2) && _pd.GetBool(item.playerdataInt));
        }
        return false;
    }

    /// <summary>
    /// Gets ItemCount
    /// </summary>
    public int GetItemCount() => ItemCount;

    /// <summary>
    /// Gets description for a specific item
    /// </summary>
    public string GetDescConvo(int itemNum)
    {
        var item = List.First(x => x.uniqueName.Equals(_currentList[itemNum].name));

        switch (item.type)
        {
            case ItemType.Normal:
                return item.descConvo1;
            case ItemType.OneTwo:
                if (_pd.GetBool(item.playerdataBool1))
                    return item.descConvo1;
                else
                    return item.descConvo2;
            case ItemType.OneTwoBoth:
                if (_pd.GetBool(item.playerdataBool1) && !_pd.GetBool(item.playerdataBool2))
                    return item.descConvo1;
                else if (!_pd.GetBool(item.playerdataBool1) && _pd.GetBool(item.playerdataBool2))
                    return item.descConvo2;
                else
                    return item.descConvoBoth;
            case ItemType.Counted:
                return item.descConvo1;
            case ItemType.Flower:
                if (_pd.GetBool(item.playerdataInt))
                {
                    if (_pd.GetBool(item.playerdataBool2))
                    {
                        return item.descConvoBoth;
                    }
                    return item.descConvo2;
                }
                else
                {
                    if (_pd.GetBool(item.playerdataBool2))
                    {
                        return item.nameConvoBoth;
                    }
                    return item.descConvo1;
                }
        }
        return "";
    }

    /// <summary>
    /// Gets name for a specific item
    /// </summary>
    public string GetNameConvo(int itemNum)
    {
        var item = List.First(x => x.uniqueName.Equals(_currentList[itemNum].name));

        switch (item.type)
        {
            case ItemType.Normal:
                return item.nameConvo1;
            case ItemType.OneTwo:
                if (_pd.GetBool(item.playerdataBool1))
                    return item.nameConvo1;
                else
                    return item.nameConvo2;
            case ItemType.OneTwoBoth:
                if (_pd.GetBool(item.playerdataBool1) && !_pd.GetBool(item.playerdataBool2))
                    return item.nameConvo1;
                else if (!_pd.GetBool(item.playerdataBool1) && _pd.GetBool(item.playerdataBool2))
                    return item.nameConvo2;
                else
                    return item.nameConvoBoth;
            case ItemType.Counted:
                return item.nameConvo1;
            case ItemType.Flower:
                if (_pd.GetBool(item.playerdataInt))
                    return item.nameConvo2;
                else
                    return item.nameConvo1;
        }
        return "";
    }

    /// <summary>
    /// Gets sprite for a specific item
    /// </summary>
    public Sprite GetSprite(int itemNum) => _currentList[itemNum].GetComponentInChildren<SpriteRenderer>().sprite;

    /// <summary>
    /// Get y distance
    /// </summary>
    public float GetYDistance() => YDistance;

    /// <summary>
    /// Get index of first new item
    /// </summary>
    public int GetFirstNewItem() => FirstNewItem;

    /// <summary>
    /// Get amount of counted items
    /// </summary>
    public string GetPlayerDataKillsName(int itemNum)
    {
        var item = List.First(x => x.uniqueName.Equals(_currentList[itemNum].name));

        if (item.type != ItemType.Counted) return "0Return";
        return item.playerdataInt;
    }

    private void AddToFadeGroup()
    {
        var fg = transform.parent.GetComponent<FadeGroup>();
        List<SpriteRenderer> srList = new List<SpriteRenderer>(fg.spriteRenderers);
        List<TextMeshPro> tmpList = new List<TextMeshPro>(fg.texts);

        foreach (var o in ListInv)
        {
            foreach (var sr in o.GetComponentsInChildren<SpriteRenderer>())
                srList.Add(sr);
            foreach (var tmp in o.GetComponentsInChildren<TextMeshPro>())
                tmpList.Add(tmp);
        }

        fg.spriteRenderers = srList.ToArray();
        fg.texts = tmpList.ToArray();
    }

    private static void Log(string message)
    {
        Logger.LogDebug($"[SFCore]:[CustomItemList] - {message}");
        Debug.Log($"[SFCore]:[CustomItemList] - {message}");
    }
    private static void Log(object message)
    {
        Log($"{message}");
    }
}