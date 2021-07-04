using System;
using GlobalEnums;
using UnityEngine;
using Logger = Modding.Logger;
using System.Collections.Generic;

namespace SFCore
{
    /// <summary>
    ///     Structure of data for a single achievement.
    /// </summary>
    public struct s_CustomAchievement
    {
        public string key;
        public Sprite sprite;
        public string titleConvo;
        public string textConvo;
        public bool hidden;
    }

    /// <summary>
    ///     Achievement helper class for easily adding custom achievements.
    ///     The mod using this needs to handle the following:
    ///     - titleConvo Language string(s)
    ///     - textConvo Language string(s)
    /// </summary>
    public static class AchievementHelper
    {
        private static List<s_CustomAchievement> customAchievements = new List<s_CustomAchievement>();

        /// <inheritdoc />
        /// <summary>
        ///     Constructs the helper, hooks needed methods.
        /// </summary>
        static AchievementHelper()
        {
            On.UIManager.RefreshAchievementsList += OnUIManagerRefreshAchievementsList;
            On.AchievementHandler.Awake += OnAchievementHandlerAwake;
            On.AchievementHandler.CanAwardAchievement += (orig, self, key) => { orig(self, key); return true; };
        }

        /// <inheritdoc />
        /// <summary>
        ///     Adds an achievement to the private list of custom achievements.
        /// </summary>
        /// <param name="key">Achievement key, determines if an achievement is unlocked</param>
        /// <param name="sprite">Sprite of the achievement</param>
        /// <param name="titleConvo">Language key of the achievement title</param>
        /// <param name="textConvo">Language key of the achievement description</param>
        /// <param name="hidden">Determines if the achievement is hidden until unlocked</param>
        public static void AddAchievement(string key, Sprite sprite, string titleConvo, string textConvo, bool hidden)
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

        /// <inheritdoc />
        /// <summary>
        ///     Adds the contents of customAchievements to the given AchievementsList
        /// </summary>
        /// <param name="list">Achievement list which the custom achievements get added to</param>
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

        /// <inheritdoc />
        /// <summary>
        ///     On hook that initializes achievements.
        /// </summary>
        private static void OnAchievementHandlerAwake(On.AchievementHandler.orig_Awake orig, AchievementHandler self)
        {
            orig(self);
            initAchievements(self.achievementsList);
            initAchievements(self.achievementsListPrefab);
        }

        /// <inheritdoc />
        /// <summary>
        ///     On hook that initializes achievements & achivements in the menu and unhooks itself afterwards.
        /// </summary>
        private static void OnUIManagerRefreshAchievementsList(On.UIManager.orig_RefreshAchievementsList orig, UIManager self)
        {
            initAchievements(GameManager.instance.achievementHandler.achievementsList);
            initAchievements(GameManager.instance.achievementHandler.achievementsListPrefab);
            initMenuAchievements(self);
            orig(self);

            On.UIManager.RefreshAchievementsList -= OnUIManagerRefreshAchievementsList;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes menu achievements
        /// </summary>
        /// <param name="manager">UI manager</param>
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

        /// <inheritdoc />
        /// <summary>
        ///     Updates status of achievement
        /// </summary>
        /// <param name="ach">The achievement in question</param>
        /// <param name="menuAch">The menu part of the achievement</param>
        /// <param name="hiddenIcon">The hidden sprite</param>
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
