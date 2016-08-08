using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using S_Plus_Class_Kalista.Libaries;


namespace S_Plus_Class_Kalista.Handlers
{
    class RendHandler : Core
    {
        public const string _MenuNameBase = ".Auto Rend Menu";
        public const string _MenuItemBase = "Auto.Rend.";

        public static void Load()
        {
            SMenu.AddSubMenu(_Menu());
            RendCheck.Load();
        }

        private static Menu _Menu()
        {
            var menu = new Menu(_MenuNameBase, "rendMenu");

            var subMenu = new Menu(".Kill","autoRendMenu");
            subMenu.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendNonKillables", "Rend NonKillables").SetValue(true));
            subMenu.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendEpics", "Rend epic monsters").SetValue(true));
            subMenu.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendBuffs", "Rend large buff monsters").SetValue(true));
            subMenu.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendSmallMonster", "Rend small monsters").SetValue(true));
            subMenu.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendEpicMinions", "Rend epic minions(Seige,Super)").SetValue(true));
            subMenu.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendEnemyChampions", "Rend enemies").SetValue(true));

            var subMenu2 = new Menu(".Cases", "caseRendMenu");

            subMenu2.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendMinions", "Rend minions").SetValue(true));
            subMenu2.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendMinions.Slider.Killed", "Required minions killed").SetValue(new Slider(1, 2, 10)));

            subMenu2.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendHarrassKill", "Rend harass (kills minion + stack on enemy)").SetValue(true));
            subMenu2.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendHarrassKill.Slider.Stacks", "Required stacks on enemy").SetValue(new Slider(1, 1, 5)));
            subMenu2.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendHarrassKill.Slider.Killed", "Required minions killed").SetValue(new Slider(2, 2, 10)));

            subMenu2.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendBeforeDeath", "Rend before death").SetValue(false));
            subMenu2.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendBeforeDeath.Slider.Enemies", "Required enimies with stacks").SetValue(new Slider(1, 1, 5)));
            subMenu2.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendBeforeDeath.Slider.Stacks", "Required stacks on enimies").SetValue(new Slider(1, 1, 5)));
            subMenu2.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendBeforeDeath.Slider.PercentHP", "Required remaining HP%(Player)").SetValue(new Slider(15, 1, 40)));

            subMenu2.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendOnLeave", "Rend on leave").SetValue(false));
            subMenu2.AddItem(new MenuItem(_MenuItemBase + "Boolean.RendOnLeave.Slider.Stacks", "Required stacks").SetValue(new Slider(4, 1, 10)));

            menu.AddSubMenu(subMenu);
            menu.AddSubMenu(subMenu2);

            return menu;
        }
    }
}
