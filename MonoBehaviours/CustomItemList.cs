using SFCore.Utils;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Logger = Modding.Logger;
using Object = UnityEngine.Object;

namespace SFCore.MonoBehaviours
{
    public class CustomItemList : MonoBehaviour
    {
        public static CustomItemList instance { get; private set; }

        public List<ItemHelper.Item> list { get; private set; } = new List<ItemHelper.Item>();
        public GameObject[] listInv { get; private set; } = new GameObject[0];
        private GameObject[] currentList = new GameObject[0];
        private PlayerData pd = PlayerData.instance;
        public float yDistance { get; private set; } = -2f;
        public int itemCount { get; private set; } = -1;
        public int firstNewItem { get; private set; }
        private bool built = false;

        public CustomItemList()
        {
            instance = this;
        }

        public bool hasAtLeastOneItem()
        {
            if (this.currentList.Length > 0)
                return this.currentList[0] != null;
            return false;
        }

        public int totalItemAmount()
        {
            return this.list.Count;
        }
        public int gotItemAmount()
        {
            return this.itemCount + 1;
        }

        public void BuildItemList()
        {
            Modding.Logger.Log($"[CustomItem] - Build item list");
            this.pd = PlayerData.instance;
            this.firstNewItem = -1;
            this.itemCount = -1;
            this.listInv = new GameObject[this.list.Count];
            var prefab = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<JournalEntryStats>()
                .First(x => x.gameObject.name.Equals("Journal Crawler")).gameObject);
            Object.Destroy(prefab.GetComponent<JournalEntryStats>());
            prefab.FindGameObjectInChildren("Name").GetComponent<SetTextMeshProGameText>().sheetName = "UI";
            for (int i = 0; i < this.list.Count; i++)
            {
                this.itemCount++;
                GameObject gameObject = MakeGameObject(this.list[i], prefab);
                gameObject.transform.SetParent(transform, false);
                gameObject.SetActive(false);
                this.listInv[this.itemCount] = gameObject;
            }

            AddToFadeGroup();

            built = true;
        }

        public void UpdateItemList()
        {
            if (!built) BuildItemList();

            Modding.Logger.Log($"[CustomItem] - Update Item List");
            this.firstNewItem = 0;
            this.itemCount = -1;
            float num = 0f;
            this.currentList = new GameObject[this.listInv.Length];
            for (int i = 0; i < this.listInv.Length; i++)
            {
                GameObject gameObject = this.listInv[i];
                MakeSpritesText(gameObject, this.list[i]);

                var tmpBool = CheckBool(this.list[i]);
                gameObject.SetActive(tmpBool);
                if (tmpBool)
                {
                    this.itemCount++;
                    this.currentList[this.itemCount] = gameObject;
                    gameObject.transform.localPosition = new Vector3(0f, num, 0f);
                    num += this.yDistance;
                }
                else
                {
                    gameObject.transform.localPosition = new Vector3(-500, -500, -500);
                }
            }
        }

        private GameObject MakeGameObject(ItemHelper.Item item, GameObject prefab)
        {
            var ret = GameObject.Instantiate(prefab);
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
            var portraitSR = portrait.GetComponent<SpriteRenderer>();
            var name = go.FindGameObjectInChildren("Name");
            var nameText = name.GetComponent<SetTextMeshProGameText>();

            switch (item.type)
            {
                case ItemType.Normal:
                    portraitSR.sprite = item.sprite1;
                    nameText.convName = item.nameConvo1;
                    break;
                case ItemType.OneTwo:
                    if (this.pd.GetBool(item.playerdataBool1))
                    {
                        portraitSR.sprite = item.sprite1;
                        nameText.convName = item.nameConvo1;
                    }
                    else
                    {
                        portraitSR.sprite = item.sprite2;
                        nameText.convName = item.nameConvo2;
                    }
                    break;
                case ItemType.OneTwoBoth:
                    if (this.pd.GetBool(item.playerdataBool1) && !this.pd.GetBool(item.playerdataBool2))
                    {
                        portraitSR.sprite = item.sprite1;
                        nameText.convName = item.nameConvo1;
                    }
                    else if (!this.pd.GetBool(item.playerdataBool1) && this.pd.GetBool(item.playerdataBool2))
                    {
                        portraitSR.sprite = item.sprite2;
                        nameText.convName = item.nameConvo2;
                    }
                    else
                    {
                        portraitSR.sprite = item.spriteBoth;
                        nameText.convName = item.nameConvoBoth;
                    }
                    break;
                case ItemType.Counted:
                    portraitSR.sprite = item.sprite1;
                    nameText.convName = item.nameConvo1;
                    break;
                case ItemType.Flower:
                    if (this.pd.GetBool(item.playerdataInt)) // xunFlowerBroken
                    {
                        portraitSR.sprite = item.sprite2;
                        nameText.convName = item.nameConvo2;
                    }
                    else
                    {
                        portraitSR.sprite = item.sprite1;
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
                    return this.pd.GetBool(item.playerdataBool1);
                case ItemType.OneTwo:
                    return this.pd.GetBool(item.playerdataBool1) ^ this.pd.GetBool(item.playerdataBool2);
                case ItemType.OneTwoBoth:
                    return this.pd.GetBool(item.playerdataBool1) || this.pd.GetBool(item.playerdataBool2);
                case ItemType.Counted:
                    return this.pd.GetInt(item.playerdataInt) > 0;
                case ItemType.Flower:
                    return this.pd.GetBool(item.playerdataBool1) && !(this.pd.GetBool(item.playerdataBool2) && this.pd.GetBool(item.playerdataInt));
            }
            return false;
        }

        public int GetItemCount()
        {
            Modding.Logger.Log($"[CustomItem] - Itemcount: {this.itemCount}");
            return this.itemCount;
        }

        public string GetDescConvo(int itemNum)
        {
            var item = this.list.First(x => x.uniqueName.Equals(this.currentList[itemNum].name));
            Modding.Logger.Log($"[CustomItem] - Desc: {itemNum}/{this.list.Count}");

            switch (item.type)
            {
                case ItemType.Normal:
                    return item.descConvo1;
                case ItemType.OneTwo:
                    if (this.pd.GetBool(item.playerdataBool1))
                        return item.descConvo1;
                    else
                        return item.descConvo2;
                case ItemType.OneTwoBoth:
                    if (this.pd.GetBool(item.playerdataBool1) && !this.pd.GetBool(item.playerdataBool2))
                        return item.descConvo1;
                    else if (!this.pd.GetBool(item.playerdataBool1) && this.pd.GetBool(item.playerdataBool2))
                        return item.descConvo2;
                    else
                        return item.descConvoBoth;
                case ItemType.Counted:
                    return item.descConvo1;
                case ItemType.Flower:
                    if (this.pd.GetBool(item.playerdataInt))
                    {
                        if (this.pd.GetBool(item.playerdataBool2))
                        {
                            return item.descConvoBoth;
                        }
                        return item.descConvo2;
                    }
                    else
                    {
                        if (this.pd.GetBool(item.playerdataBool2))
                        {
                            return item.nameConvoBoth;
                        }
                        return item.descConvo1;
                    }
            }
            return "";
        }

        public string GetNameConvo(int itemNum)
        {
            var item = this.list.First(x => x.uniqueName.Equals(this.currentList[itemNum].name));
            Modding.Logger.Log($"[CustomItem] - Name: {itemNum}/{this.list.Count}");

            switch (item.type)
            {
                case ItemType.Normal:
                    return item.nameConvo1;
                case ItemType.OneTwo:
                    if (this.pd.GetBool(item.playerdataBool1))
                        return item.nameConvo1;
                    else
                        return item.nameConvo2;
                case ItemType.OneTwoBoth:
                    if (this.pd.GetBool(item.playerdataBool1) && !this.pd.GetBool(item.playerdataBool2))
                        return item.nameConvo1;
                    else if (!this.pd.GetBool(item.playerdataBool1) && this.pd.GetBool(item.playerdataBool2))
                        return item.nameConvo2;
                    else
                        return item.nameConvoBoth;
                case ItemType.Counted:
                    return item.nameConvo1;
                case ItemType.Flower:
                    if (this.pd.GetBool(item.playerdataInt))
                        return item.nameConvo2;
                    else
                        return item.nameConvo1;
            }
            return "";
        }

        public Sprite GetSprite(int itemNum)
        {
            Modding.Logger.Log($"[CustomItem] - get sprite");
            return this.currentList[itemNum].GetComponentInChildren<SpriteRenderer>().sprite;
        }

        public float GetYDistance()
        {
            Modding.Logger.Log($"[CustomItem] - get y: {this.yDistance}");
            return this.yDistance;
        }

        public int GetFirstNewItem()
        {
            Modding.Logger.Log($"[CustomItem] - get new: {this.firstNewItem}");
            return this.firstNewItem;
        }

        public string GetPlayerDataKillsName(int itemNum)
        {
            var item = this.list.First(x => x.uniqueName.Equals(this.currentList[itemNum].name));
            Logger.Log($"[CustomItem] - get int name: {item.type}");

            if (item.type != ItemType.Counted) return "0Return";
            return item.playerdataInt;
        }

        private void AddToFadeGroup()
        {
            var fg = transform.parent.GetComponent<FadeGroup>();
            List<SpriteRenderer> srList = new List<SpriteRenderer>(fg.spriteRenderers);
            List<TextMeshPro> tmpList = new List<TextMeshPro>(fg.texts);

            foreach (var o in this.listInv)
            {
                foreach (var sr in o.GetComponentsInChildren<SpriteRenderer>())
                    srList.Add(sr);
                foreach (var tmp in o.GetComponentsInChildren<TextMeshPro>())
                    tmpList.Add(tmp);
            }

            fg.spriteRenderers = srList.ToArray();
            fg.texts = tmpList.ToArray();
        }
    }
}
