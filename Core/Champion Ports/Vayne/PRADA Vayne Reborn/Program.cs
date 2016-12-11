using System;
using LeagueSharp.Common;
using PRADA_Vayne.MyUtils;

using EloBuddy; 
using LeagueSharp.Common; 
namespace PRADA_Vayne
{
    public class Program
    {
        #region Fields and Objects
        
        public static MyOrbwalker.Orbwalker Orbwalker;

        #region Menu

        public static Menu MainMenu;
        public static Menu ComboMenu;
        public static Menu LaneClearMenu;
        public static Menu EscapeMenu;
        public static Menu DrawingsMenu;
        public static Menu SkinhackMenu;
        public static Menu OrbwalkerMenu;

        #endregion Menu

        #region Spells

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        #endregion Spells

        #endregion

        public static void Main()
        {
            MyInitializer.PRADALoader.Init();
        }
    }
}
