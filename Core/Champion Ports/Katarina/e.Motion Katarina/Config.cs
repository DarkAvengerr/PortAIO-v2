using LeagueSharp.Common;
using System;
using System.Linq;

using EloBuddy; 
using LeagueSharp.Common; 
namespace e.Motion_Katarina
{
    static class Config
    {
        private static Menu config;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void InitializeMenu()
        {
            //Root-Menu
            config = new Menu("e.Motion Katarina","emotionkatarina",true);
            //Orbwalker
            Menu orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            //Combo-Menu
            Menu comboMenu = new Menu("Combo", "combo");
            Menu qMenu = new Menu("Q Options", "combo.qmenu");
            qMenu.AddItem(new MenuItem("combo.q.minion", "Use Q on Minions").SetValue(true));
            qMenu.AddItem(new MenuItem("combo.q.direct", "Use Q directly on Enemy").SetValue(true));
            qMenu.AddItem(new MenuItem("combo.q.onlyrunaway", "Use Q only when enemy does not face you").SetValue(false));
            comboMenu.AddSubMenu(qMenu);
            comboMenu.AddItem(new MenuItem("combo.q", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("combo.w", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("combo.e", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("combo.ealways", "Always use E").SetValue(false));
            comboMenu.AddItem(new MenuItem("combo.r", "Use R").SetValue(true));

            //Harass-Menu
            Menu harassMenu = new Menu("Harass", "harass");
            harassMenu.AddItem(new MenuItem("harass.q", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("harass.w", "Use W").SetValue(true));
            harassMenu.AddItem(new MenuItem("harass.e", "Use E").SetValue(false));

            //Killsteal-Menu
            Menu ksMenu = new Menu("Killsteal", "ks");
            ksMenu.AddItem(new MenuItem("ks.use", "Use Killsteal").SetValue(true));
            ksMenu.AddItem(new MenuItem("ks.r", "Stop R for Killsteal").SetValue(true));

            //Laneclear-Menü
            Menu laneclear = new Menu("Laneclear", "laneclear");
            laneclear.AddItem(new MenuItem("laneclear.q", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("laneclear.w", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("laneclear.e", "Use E").SetValue(false));

            //Jungleclear-Menü
            Menu jungleclear = new Menu("Jungleclear", "jungleclear");
            jungleclear.AddItem(new MenuItem("jungleclear.q", "Use Q").SetValue(true));
            jungleclear.AddItem(new MenuItem("jungleclear.w", "Use W").SetValue(true));
            jungleclear.AddItem(new MenuItem("jungleclear.e", "Use E").SetValue(true));

            //Lasthit-Menü
            Menu lasthit = new Menu("Lasthit", "lasthit");
            lasthit.AddItem(new MenuItem("lasthit.q", "Use Q").SetValue(true));
            lasthit.AddItem(new MenuItem("lasthit.e", "Use E").SetValue(false));

            //Drawing-Menu
            Menu drawings = new Menu("Drawings", "draw");
            drawings.AddItem(new MenuItem("drawings.hpbar", "Draw Damage on Enemy's HP Bar").SetValue(true));
            drawings.AddItem(new MenuItem("drawings.fill", "Fill Color").SetValue(new Circle(true, System.Drawing.Color.Chartreuse)));
            drawings.AddItem(new MenuItem("drawings.credits", "Credits for Damage Drawings to SupportExTraGoZ"));
            drawings.AddItem(new MenuItem("drawings.daggers", "Draw Daggers").SetValue(true));

            //Misc-Menu
            Menu miscMenu = new Menu("Miscellanious", "misc");
            miscMenu.AddItem(new MenuItem("misc.noRCancel","Prevent R Cancel").SetValue(false).SetTooltip("Will prevent accidental R Cancel within first 0.4 Seconds of R Cast"));

            //Add all Menus to Root-Menu
            config.AddSubMenu(orbwalkerMenu);
            config.AddSubMenu(comboMenu);
            config.AddSubMenu(harassMenu);
            config.AddSubMenu(ksMenu);
            config.AddSubMenu(lasthit);
            config.AddSubMenu(laneclear);
            config.AddSubMenu(jungleclear);
            config.AddSubMenu(drawings);
            config.AddSubMenu(miscMenu);
            config.AddToMainMenu();

            //Make DamageDrawings Working
            InitDamageDrawings();
        }

        public static void InitDamageDrawings()
        {
            DrawDamage.DamageToUnit = Logic.GetDamage;
            DrawDamage.Enabled = config.Item("drawings.hpbar").GetValue<bool>();
            DrawDamage.Fill = config.Item("drawings.fill").GetValue<Circle>().Active;
            DrawDamage.FillColor = config.Item("drawings.fill").GetValue<Circle>().Color;
            config.Item("drawings.hpbar").ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                };
            config.Item("drawings.fill").ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
        }

        public static bool GetBoolValue(string itemName)
        {
            //if (!config.Items.Any(i => i.Name == itemName)) {
            //    Console.WriteLine("Menu-Item " + itemName + " not found");
            //    return false;
            //}
            try {
                return config.Item(itemName).GetValue<bool>();
            }
            catch
            {
                Console.WriteLine("Menu-Item " + itemName + " does not have a Boolean Value");
                return false;
            }
        }

        public static int GetSliderValue(string itemName)
        {
            if (!config.Items.Any(i => i.Name == itemName))
            {
                Console.WriteLine("Menu-Item " + itemName + " not found");
                return -1;
            }
            try
            {
                return config.Item(itemName).GetValue<Slider>().Value;
            }
            catch
            {
                Console.WriteLine("Menu-Item " + itemName + " does not have a Boolean Value");
                return -1;
            }
        }
    }
}
