using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Modding;
using SFCore.Utils;

namespace SFCore
{
    public class SFCoreMod : Mod
    {
        static SFCoreMod()
        {
            // to load all components
            SFCore.AchievementHelper.unusedInit();
            SFCore.CharmHelper.unusedInit();
            SFCore.EnviromentParticleHelper.unusedInit();
            //SFCore.ItemHelper.unusedInit();
            SFCore.MenuStyleHelper.unusedInit();
            SFCore.TitleLogoHelper.unusedInit();
        }

        public SFCoreMod() : base("SFCore")
        {}

        public override string GetVersion() => Util.GetVersion(Assembly.GetExecutingAssembly());

        public override void Initialize()
        {
            if (MenuStyles.Instance.styles.Length > 10)
            {
                // custom ones are added
                MenuStyles.Instance.SetStyle(MenuStyles.Instance.styles.Length - 1, false, false);
            }
        }
    }
}
