using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WarwickII.Common;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace WarwickII.Modes
{
    internal class ModeConfig
    {
        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu MenuConfig { get; private set; }
        public static Menu MenuKeys { get; private set; }
        public static Menu MenuHarass { get; private set; }
        public static Menu MenuFarm { get; private set; }
        public static Menu MenuMisc { get; private set; }
        public static Menu MenuTools { get; private set; }
        public static void Init()
        {
            MenuConfig = new Menu(":: Warwick ::", "Warwick", true).SetFontStyle(FontStyle.Regular, Color.GreenYellow);

            MenuTools = new Menu("Tools", "Tools");
            MenuConfig.AddSubMenu(MenuTools);

            MenuTools.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(MenuTools.SubMenu("Orbwalking"));
            Orbwalker.SetAttack(true);

            Common.CommonTargetSelector.Initialize(MenuTools);
            Common.CommonAutoLevel.Initialize(MenuTools);
            Common.CommonAutoBush.Initialize(MenuTools);
            Common.CommonSkins.Initialize(MenuTools);

            //EvadeMain.Initialize();
            Common.CommonHelper.Initialize();
            Modes.ModeCombo.Initialize(MenuConfig);
            Modes.ModePerma.Initialize(MenuConfig);

            
            MenuFarm = new Menu("Farm", "Farm");
            {
                Modes.ModeLane.Initialize(MenuFarm);
                Modes.ModeJungle.Initialize(MenuFarm);

                MenuFarm.AddItem(new MenuItem("Farm.Enable", ":: Lane / Jungle Clear Active!").SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Toggle, true))).Permashow(true, ObjectManager.Player.ChampionName + " | " + "Lane/Jungle Farm", Colors.ColorPermaShow);
                MenuFarm.AddItem(new MenuItem("Farm.MinMana.Enable", ":: Min. Mana Control Active!").SetValue(new KeyBind("M".ToCharArray()[0], KeyBindType.Toggle, true)).SetFontStyle(FontStyle.Regular, Color.Aqua)).Permashow(true, ObjectManager.Player.ChampionName + " | " + "Min. Mana Control Active", Colors.ColorPermaShow);

                MenuConfig.AddSubMenu(MenuFarm);
            }

            new ModeDraw().Initialize();

            MenuConfig.AddToMainMenu();
            
            foreach (var i in MenuConfig.Children.Cast<Menu>().SelectMany(GetSubMenu))
            {
                i.DisplayName = ":: " + i.DisplayName;
            }
        }

        private static IEnumerable<Menu> GetSubMenu(Menu menu)
        {
            yield return menu;

            foreach (var childChild in menu.Children.SelectMany(GetSubMenu))
                yield return childChild;
        }

    }
}
