using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; namespace ADCPackage.Plugins
{
    internal class Ezreal
    {
        private static Spell Q, W, E, R;

        private static AIHeroClient Player => ObjectManager.Player;

        public static void Load()
        {
            Chat.Print("[<font color='#F8F46D'>ADC Package</font>] by <font color='#79BAEC'>God</font> - <font color='#FFFFFF'>Ezreal</font> loaded (incomplete)");

            InitSpells();
            InitMenu();
        }

        private static void InitSpells()
        {

        }

        public static void Combo()
        {
        }

        public static void LaneClear()
        {
        }

        private static void InitMenu()
        {
            Menu.Config.AddSubMenu(new LeagueSharp.Common.Menu("Ezreal", "adcpackage.ezreal"));
        }
    }
}