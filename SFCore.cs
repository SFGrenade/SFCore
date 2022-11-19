using System;
using System.Collections.Generic;
using System.Reflection;
using Modding;
using SFCore.Generics;
using SFCore.Utils;

namespace SFCore
{
    /// <summary>
    ///     Used for static initialization.
    /// </summary>
    public class SFCoreMod : FullSettingsMod<SFCoreSaveSettings, SFCoreGlobalSettings>
    {
        internal static SFCoreMod instance = null;
        static SFCoreMod()
        {
            // to load all components
            //SFCore.AchievementHelper.unusedInit();
            //SFCore.CharmHelper.unusedInit();
            //SFCore.EnviromentParticleHelper.unusedInit();
            //SFCore.ItemHelper.unusedInit();
            SFCore.MenuStyleHelper.unusedInit();
            SFCore.TitleLogoHelper.unusedInit();
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        public SFCoreMod() : base("SFCore")
        {
            instance = this;
        }

        /// <summary>
        ///     Displays the version.
        /// </summary>
        public override string GetVersion() => Util.GetVersion(Assembly.GetExecutingAssembly());

        /// <summary>
        ///     Main menu is loaded.
        /// </summary>
        public override void Initialize()
        {
        }
    }

    /// <summary>
    ///     Global settings for SFCore
    /// </summary>
    public class SFCoreGlobalSettings
    {
        /// <summary>
        ///     Important to clear and reapply custom charms.
        /// </summary>
        public int MaxCustomCharms = 0;
        /// <summary>
        ///     Important to reapply the wanted menu theme.
        /// </summary>
        public string SelectedMenuTheme = "UI_MENU_STYLE_CLASSIC";
    }

    /// <summary>
    ///     Save specific settings for SFCore
    /// </summary>
    public class SFCoreSaveSettings
    {
        /// <summary>
        ///     Important to clear and reapply custom charms.
        /// </summary>
        public List<int> EquippedCustomCharms = new ();
    }
}
