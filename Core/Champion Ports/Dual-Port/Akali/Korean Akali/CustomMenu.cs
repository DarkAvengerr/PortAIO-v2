using KoreanCommon;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAkali
{
    internal static class CustomMenu
    {
        public static void Load(CommonChampion akali)
        {
            HarasMenu(akali);
            LaneClearMenu(akali);
            ComboMenu(akali);
            OptionsMenu(akali);
        }

        private static void HarasMenu(CommonChampion akali)
        {
            var menu = akali.MainMenu;

            menu.HarasMenu.Items.Remove(KoreanUtils.GetParam(menu, "usewtoharas"));
            menu.HarasMenu.Items.Remove(KoreanUtils.GetParam(menu, "manalimittoharas"));

            menu.HarasMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(menu, "minenergytoharas"), "Min energy to haras").SetValue(
                    new Slider(60, 0, 100)));

        }

        private static void LaneClearMenu(CommonChampion akali)
        {
            var menu = akali.MainMenu;

            menu.LaneClearMenu.Items.Remove(KoreanUtils.GetParam(menu, "usewtolaneclear"));
            menu.LaneClearMenu.Items.Remove(KoreanUtils.GetParam(menu, "manalimittolaneclear"));

            menu.LaneClearMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(menu, "saveqtofarm"), "Save Q to last hit").SetValue(true));

            menu.LaneClearMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(menu, "minminionstoe"), "Use E if will hit x minions").SetValue(
                    new Slider(4, 0, 10)));

            menu.LaneClearMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(menu, "minenergytolaneclear"), "Min energy to laneclear").SetValue(
                    new Slider(40, 0, 100)));
        }

        private static void ComboMenu(CommonChampion akali)
        {
            var menu = akali.MainMenu;

            menu.ComboMenu.Items.Remove(KoreanUtils.GetParam(menu, "minenemiestor"));
            menu.ComboMenu.Items.Remove(KoreanUtils.GetParam(menu, "disableaa"));
        }

        private static void OptionsMenu(CommonChampion akali)
        {
            var menu = akali.MainMenu;

            menu.MiscMenu.AddItem(new MenuItem(KoreanUtils.ParamName(menu, "useqtofarm"), "Use Q to farm").SetValue(true));

            var useItemsMenu = menu.MiscMenu.AddSubMenu(new Menu("Use items", "useitemsoncombo"));

            useItemsMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(menu, "offensiveitemslabel"), "Offensive Items"));

            useItemsMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(menu, "usebilgewatercutlass"), "Bilgewater Cutlass").SetValue(true));

            useItemsMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(menu, "usehextechgunblade"), "Hextech Gunblade").SetValue(true));

            useItemsMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(menu, "usebotrk"), "Blade of the Ruined King").SetValue(true));
        }
    }
}