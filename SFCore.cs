using System;
using System.Reflection;
using Modding;
using SFCore.Generics;
using SFCore.Utils;

namespace SFCore
{
    /// <summary>
    ///     Used for static initialization.
    /// </summary>
    public class SFCoreMod : GlobalSettingsMod<SFCoreSettings>
    {
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
        public SFCoreMod() : base("SFCore") {}

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

    public class SFCoreSettings
    {
        public int MaxCustomCharms = 0;
    }
}
