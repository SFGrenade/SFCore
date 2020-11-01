using System;
using GlobalEnums;
using UnityEngine;
using Logger = Modding.Logger;
using System.Collections.Generic;

namespace SFCore
{
    public struct s_CustomAchievement
    {
        public string key;
        public Sprite sprite;
        public string titleConvo;
        public string textConvo;
        public bool hidden;
    }

    public class AchievementHelper
    {
        /* 
         * AchievementHelper
         * v 2.0.0.0
         */

        private static List<s_CustomAchievement> customAchievements = new List<s_CustomAchievement>();
        private static bool initialized = false;

        public static void Initialize()
        {
            if (!initialized)
            {
                initialized = true;
                On.UIManager.RefreshAchievementsList += OnUIManagerRefreshAchievementsList;
                On.AchievementHandler.Awake += OnAchievementHandlerAwake;
            }
        }

        public static void AddAchievement(string key, Sprite sprite, string titleConvo, string textConvo, bool hidden)
        {
            if (initialized)
            {
                customAchievements.Add(new s_CustomAchievement()
                {
                    key = key,
                    sprite = sprite,
                    titleConvo = titleConvo,
                    textConvo = textConvo,
                    hidden = hidden
                });
            }
        }

        private static void initAchievements(AchievementsList list)
        {
            foreach (var ca in customAchievements)
            {
                Achievement customAch = new Achievement
                {
                    key = ca.key,
                    type = ca.hidden ? AchievementType.Hidden : AchievementType.Normal,
                    earnedIcon = ca.sprite,
                    unearnedIcon = ca.sprite,
                    localizedText = ca.textConvo,
                    localizedTitle = ca.titleConvo
                };
                bool containsCustomAch = false;
                foreach (var ach in list.achievements)
                {
                    if (ach.key == customAch.key)
                    {
                        containsCustomAch = true;
                    }
                }
                if (!containsCustomAch)
                {
                    list.achievements.Add(customAch);
                }
            }
        }
        private static void OnAchievementHandlerAwake(On.AchievementHandler.orig_Awake orig, AchievementHandler self)
        {
            orig(self);
            initAchievements(self.achievementsList);
            initAchievements(self.achievementsListPrefab);
        }
        private static void OnUIManagerRefreshAchievementsList(On.UIManager.orig_RefreshAchievementsList orig, UIManager self)
        {
            initAchievements(GameManager.instance.achievementHandler.achievementsList);
            initAchievements(GameManager.instance.achievementHandler.achievementsListPrefab);
            initMenuAchievements(self);
            orig(self);

            On.UIManager.RefreshAchievementsList -= OnUIManagerRefreshAchievementsList;
        }
        private static void initMenuAchievements(UIManager manager)
        {
            // Stolen from the game
            GameManager gm = GameManager.instance;

            AchievementsList achievementsList = gm.achievementHandler.achievementsList;
            int count = achievementsList.achievements.Count;
            for (int j = 0; j < count; j++)
            {
                Achievement achievement2 = achievementsList.achievements[j];
                if (manager.menuAchievementsList.FindAchievement(achievement2.key) == null)
                {
                    MenuAchievement menuAchievement2 = UnityEngine.Object.Instantiate(manager.menuAchievementsList.menuAchievementPrefab);
                    menuAchievement2.transform.SetParent(manager.achievementListRect.transform, false);
                    menuAchievement2.name = achievement2.key;
                    manager.menuAchievementsList.AddMenuAchievement(menuAchievement2);
                    UpdateMenuAchievementStatus(achievement2, menuAchievement2, manager.hiddenIcon);
                }
            }
            manager.menuAchievementsList.MarkInit();
        }
        private static void UpdateMenuAchievementStatus(Achievement ach, MenuAchievement menuAch, Sprite hiddenIcon)
        {
            // Stolen from the game
            try
            {
                if (GameManager.instance.IsAchievementAwarded(ach.key))
                {
                    menuAch.icon.sprite = ach.earnedIcon;
                    menuAch.icon.color = Color.white;
                    menuAch.title.text = Language.Language.Get(ach.localizedTitle, "Achievements");
                    menuAch.text.text = Language.Language.Get(ach.localizedText, "Achievements");
                }
                else if (ach.type == AchievementType.Normal)
                {
                    menuAch.icon.sprite = ach.earnedIcon;
                    menuAch.icon.color = new Color(0.57f, 0.57f, 0.57f, 0.57f);
                    menuAch.title.text = Language.Language.Get(ach.localizedTitle, "Achievements");
                    menuAch.text.text = Language.Language.Get(ach.localizedText, "Achievements");
                }
                else
                {
                    menuAch.icon.sprite = hiddenIcon;
                    menuAch.icon.color = new Color(0.57f, 0.57f, 0.57f, 0.57f);
                    menuAch.title.text = Language.Language.Get("HIDDEN_ACHIEVEMENT_TITLE", "Achievements");
                    menuAch.text.text = Language.Language.Get("HIDDEN_ACHIEVEMENT", "Achievements");
                }
            }
            catch (Exception)
            {
                if (ach.type == AchievementType.Normal)
                {
                    menuAch.icon.sprite = ach.earnedIcon;
                    menuAch.icon.color = new Color(0.57f, 0.57f, 0.57f, 0.57f);
                    menuAch.title.text = Language.Language.Get(ach.localizedTitle, "Achievements");
                    menuAch.text.text = Language.Language.Get(ach.localizedText, "Achievements");
                }
                else
                {
                    menuAch.icon.sprite = hiddenIcon;
                    menuAch.title.text = Language.Language.Get("HIDDEN_ACHIEVEMENT_TITLE", "Achievements");
                    menuAch.text.text = Language.Language.Get("HIDDEN_ACHIEVEMENT", "Achievements");
                }
            }
        }

        private static void Log(string message)
        {
            Logger.Log($"[SFCore]:[AchievementHelper] - {message}");
        }
        private static void Log(object message)
        {
            Logger.Log($"[SFCore]:[AchievementHelper] - {message.ToString()}");
        }
    }
}
