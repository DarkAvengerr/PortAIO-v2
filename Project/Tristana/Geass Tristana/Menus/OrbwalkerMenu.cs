using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Geass_Tristana.Menus
{
    internal class OrbwalkerMenu : Events.OrbwalkerEvents, GeassLib.Interfaces.Core.Menu
    {
        private Menu ManaMenu
        {
            get
            {
                var menu = new Menu(ManaMenuNameBase, "ManaManager");
                menu.AddItem(new MenuItem(ManaMenuItemBase + "Use.ManaManager", "Use ManaManager").SetValue(true));

                var subMenuCombo = new Menu(".Combo", "comboManaMenu2");
                subMenuCombo.AddItem(new MenuItem(ManaMenuItemBase + "Combo.Slider.MinMana.Q", "Min Mana% Q").SetValue(new Slider(30)));
                subMenuCombo.AddItem(new MenuItem(ManaMenuItemBase + "Combo.Slider.MinMana.E", "Min Mana% E").SetValue(new Slider(25)));
                subMenuCombo.AddItem(new MenuItem(ManaMenuItemBase + "Combo.Slider.MinMana.R", "Min Mana% R").SetValue(new Slider(1)));

                var subMenuMixed = new Menu(".Mixed", "mixedManaMenu");
                subMenuMixed.AddItem(new MenuItem(ManaMenuItemBase + "Mixed.Slider.MinMana.Q", "Min Mana% Q").SetValue(new Slider(40)));
                subMenuMixed.AddItem(new MenuItem(ManaMenuItemBase + "Mixed.Slider.MinMana.E", "Min Mana% E").SetValue(new Slider(30)));
                //subMenuMixed.AddItem(new MenuItem(MenuNameBase + "Mixed.Slider.MinMana.R", "Min Mana% R").SetValue(new Slider()));

                var subMenuClear = new Menu(".Clear", "clearManaMenu");
                subMenuClear.AddItem(new MenuItem(ManaMenuItemBase + "Clear.Slider.MinMana.Q", "Min Mana% Q").SetValue(new Slider(25)));
                subMenuClear.AddItem(new MenuItem(ManaMenuItemBase + "Clear.Slider.MinMana.E", "Min Mana% E").SetValue(new Slider(15)));

                menu.AddSubMenu(subMenuCombo);
                menu.AddSubMenu(subMenuMixed);
                menu.AddSubMenu(subMenuClear);
                return menu;
            }
        }


        public Menu GetMenu()
        {
            SMenu.AddSubMenu(ManaMenu);

            var menu = new Menu(MenuNameBase, "Orbwalker");

            var subMenuCombo = new Menu(".Combo", "comboMenu");
            subMenuCombo.AddItem(new MenuItem(MenuNameBase + "Combo.Boolean.UseQ", "Use Q").SetValue(true));
            subMenuCombo.AddItem(new MenuItem(MenuNameBase + "Combo.Boolean.UseE", "Use E").SetValue(true));
            subMenuCombo.AddItem(new MenuItem(MenuNameBase + "Combo.Boolean." +
                                              "UseR" +
                                              "" +
                                              "", "Use R (Killable)").SetValue(true));
            subMenuCombo.AddItem(new MenuItem(MenuNameBase + "Combo.Boolean.FocusETarget", "Focus E target").SetValue(false));
            var subEComboMenu = new Menu(".Combo.EChamps", "comboEMenu");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>())
            {
                if (!enemy.IsEnemy) continue;
                subEComboMenu.AddItem(new MenuItem(MenuNameBase + "Combo.Boolean.UseE.On." + enemy.ChampionName, "Use E On " + enemy.ChampionName).SetValue(true));
            }

            var subRComboMenu = new Menu(".Combo.RChamps", "comboRMenu");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>())
            {
                if (!enemy.IsEnemy) continue;
                subRComboMenu.AddItem(new MenuItem(MenuNameBase + "Combo.Boolean.UseR.On." + enemy.ChampionName, "Use R On " + enemy.ChampionName).SetValue(true));
            }

            subMenuCombo.AddSubMenu(subEComboMenu);
            subMenuCombo.AddSubMenu(subRComboMenu);

            var subMenuMixed = new Menu(".Mixed", "mixedMenu");
            subMenuMixed.AddItem(new MenuItem(MenuNameBase + "Mixed.Boolean.UseQ", "Use Q").SetValue(true));
            subMenuMixed.AddItem(new MenuItem(MenuNameBase + "Mixed.Boolean.UseE", "Use E").SetValue(true));
            subMenuMixed.AddItem(new MenuItem(MenuNameBase + "Mixed.Slider.MaxDistance", "Max Distance (Range-Distance)").SetValue(new Slider(100, 0, 300)));
            subMenuMixed.AddItem(new MenuItem(MenuNameBase + "Mixed.Boolean.FocusETarget", "Focus E target").SetValue(true));
            var subEMixedMenu = new Menu(".Mixed.EChamps", "mixedEMenu");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>())
            {
                if (!enemy.IsEnemy) continue;
                subEMixedMenu.AddItem(new MenuItem(MenuNameBase + "Mixed.Boolean.UseE.On." + enemy.ChampionName, "Use E On " + enemy.ChampionName).SetValue(true));
            }
            subMenuMixed.AddSubMenu(subEMixedMenu);

            var subMenuClear = new Menu(".Clear", "clearMenu");

            subMenuClear.AddItem(new MenuItem(MenuNameBase + "Clear.Boolean.UseQ.Minons", "Use Q On Minons").SetValue(true));
            subMenuClear.AddItem(new MenuItem(MenuNameBase + "Clear.Boolean.UseE.Minons", "Use E On Minons").SetValue(true));
            subMenuClear.AddItem(new MenuItem(MenuNameBase + "Clear.Minons.Slider.MinMinons", "Min Minons").SetValue(new Slider(3, 1, 10)));

            subMenuClear.AddItem(new MenuItem(MenuNameBase + "Clear.Boolean.UseQ.Turret", "Use Q On Turrets").SetValue(true));
            subMenuClear.AddItem(new MenuItem(MenuNameBase + "Clear.Boolean.UseE.Turret", "Use E On Turrets").SetValue(true));

            subMenuClear.AddItem(new MenuItem(MenuNameBase + "Clear.Boolean.UseQ.Monsters", "Use Q on Jungle").SetValue(true));
            subMenuClear.AddItem(new MenuItem(MenuNameBase + "Clear.Boolean.UseE.Monsters", "Use E on Jungle").SetValue(true));

            subMenuClear.AddItem(new MenuItem(MenuNameBase + "Clear.Boolean.FocusETarget", "Focus E target").SetValue(true));
            menu.AddSubMenu(subMenuCombo);
            menu.AddSubMenu(subMenuMixed);
            menu.AddSubMenu(subMenuClear);
            return menu;
        }
        public void Load()
        {
            SMenu.AddSubMenu(GetMenu());
            var orbwalkHandler = new Events.OrbwalkerEvents();
            CommonOrbwalker = new Orbwalking.Orbwalker(SMenu.SubMenu(".CommonOrbwalker"));
            Game.OnUpdate += orbwalkHandler.OnUpdate;
        }
    }
}