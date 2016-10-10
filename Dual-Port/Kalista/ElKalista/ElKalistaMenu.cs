using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElKalista
{
    public class ElKalistaMenu
    {
        public static Menu _menu;
        public static String ScriptVersion { get { return typeof(Kalista).Assembly.GetName().Version.ToString(); } }


        public static void Initialize()
        {
            _menu = new Menu("ElKalista", "menu", true);

            //ElKalista.Orbwalker
            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Kalista.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            _menu.AddSubMenu(orbwalkerMenu);

            //ElKalista.TargetSelector
            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);
            _menu.AddSubMenu(targetSelector);

            var cMenu = new Menu("Combo", "Combo");
            cMenu.AddItem(new MenuItem("ElKalista.Combo.Q", "Use Q").SetValue(true));
            cMenu.AddItem(new MenuItem("ElKalista.Combo.Q.Mana", "Minimum mana for Q")).SetValue(new Slider(20));
            cMenu.AddItem(new MenuItem("ElKalista.Combo.E", "Use E").SetValue(true));
            cMenu.AddItem(new MenuItem("ElKalista.Combo.R", "Use R").SetValue(true));
            cMenu.AddItem(new MenuItem("ElKalista.sssssssss", ""));
            cMenu.AddItem(new MenuItem("ElKalista.ComboE.Auto", "Use stacked E").SetValue(true));
            cMenu.AddItem(new MenuItem("ElKalista.ssssddsdssssss", ""));
            cMenu.AddItem(new MenuItem("ElKalista.Combo.Disable.E", "Only cast E when killable in combo").SetValue(false));
            cMenu.AddItem(new MenuItem("ElKalista.hitChance", "Hitchance Q").SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3)));
            cMenu.AddItem(new MenuItem("ElKalista.SemiR", "Semi-manual R").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            cMenu.AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));
            _menu.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "Harass");
            hMenu.AddItem(new MenuItem("ElKalista.Harass.Q", "Use Q").SetValue(true));
            hMenu.AddItem(new MenuItem("ElKalista.Harass.E", "E when target has rand and minion can be killed").SetValue(false));
            hMenu.AddItem(new MenuItem("ElKalista.minmanaharass", "Harass mana")).SetValue(new Slider(55));
            hMenu.AddItem(new MenuItem("ElKalista.hitChance", "Hitchance Q").SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3)));

            hMenu.SubMenu("AutoHarass").AddItem(new MenuItem("ElKalista.AutoHarass", "[Toggle] Auto harass", false).SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Toggle)));
            hMenu.SubMenu("AutoHarass").AddItem(new MenuItem("ElKalista.UseQAutoHarass", "Use Q").SetValue(true));
            hMenu.SubMenu("AutoHarass").AddItem(new MenuItem("ElKalista.harass.mana", "Auto harass mana")).SetValue(new Slider(55));

            _menu.AddSubMenu(hMenu);

            var lMenu = new Menu("Lane clear", "Clear");
            lMenu.AddItem(new MenuItem("useQFarm", "Use Q").SetValue(true));
            lMenu.AddItem(new MenuItem("ElKalista.Count.Minions", "Killable minions with Q >=").SetValue(new Slider(2, 1, 5)));
            lMenu.AddItem(new MenuItem("useEFarm", "Use E").SetValue(true));
            lMenu.AddItem(new MenuItem("ElKalista.Count.Minions.E", "Killable minions with E >=").SetValue(new Slider(2, 1, 5)));
            lMenu.AddItem(new MenuItem("useEFarmddsddaadsd", ""));
            lMenu.AddItem(new MenuItem("useQFarmJungle", "Use Q in jungle").SetValue(true));
            lMenu.AddItem(new MenuItem("useEFarmJungle", "Use E in jungle").SetValue(true));
            lMenu.AddItem(new MenuItem("useEFarmddssd", ""));
            lMenu.AddItem(new MenuItem("minmanaclear", "Mana needed to clear ")).SetValue(new Slider(55));

            _menu.AddSubMenu(lMenu);


            var itemMenu = new Menu("Items", "Items");
            itemMenu.AddItem(new MenuItem("ElKalista.Items.Youmuu", "Use Youmuu's Ghostblade").SetValue(true));
            itemMenu.AddItem(new MenuItem("ElKalista.Items.Cutlass", "Use Cutlass").SetValue(true));
            itemMenu.AddItem(new MenuItem("ElKalista.Items.Blade", "Use Blade of the Ruined King").SetValue(true));
            itemMenu.AddItem(new MenuItem("ElKalista.Harasssfsddass.E", ""));
            itemMenu.AddItem(new MenuItem("ElKalista.Items.Blade.EnemyEHP", "Enemy HP Percentage").SetValue(new Slider(80, 100, 0)));
            itemMenu.AddItem(new MenuItem("ElKalista.Items.Blade.EnemyMHP", "My HP Percentage").SetValue(new Slider(80, 100, 0)));
            _menu.AddSubMenu(itemMenu);


            var setMenu = new Menu("Misc", "SSS");
            setMenu.AddItem(new MenuItem("ElKalista.misc.save", "Save ally with R").SetValue(true));
            setMenu.AddItem(new MenuItem("ElKalista.misc.allyhp", "Ally HP Percentage").SetValue(new Slider(25, 100, 0)));
            setMenu.AddItem(new MenuItem("useEFarmddsddsasfsasdsdsaadsd", ""));
            setMenu.AddItem(new MenuItem("ElKalista.E.Auto", "Auto use E").SetValue(true));
            setMenu.AddItem(new MenuItem("ElKalista.E.Stacks", "Stacks for E usage >=").SetValue(new Slider(10, 1, 20)));
            setMenu.AddItem(new MenuItem("useEFafsdsgdrmddsddsasfsasdsdsaadsd", ""));
            //setMenu.AddItem(new MenuItem("ElKalista.misc.autow", "Smart W usage").SetValue(false));
            setMenu.AddItem(new MenuItem("ElKalista.misc.lasthithelper", "E lasthit assist").SetValue(false));
            setMenu.AddItem(new MenuItem("ElKalista.misc.junglesteal", "Jungle steal mode").SetValue(true));
            setMenu.AddItem(new MenuItem("ElKalista.misc.kaliscrank", "Balista").SetValue(true));

            _menu.AddSubMenu(setMenu);

            //ElKalista.Misc
            var miscMenu = new Menu("Drawings", "Misc");
            miscMenu.AddItem(new MenuItem("ElKalista.Draw.off", "Turn drawings off").SetValue(false));
            miscMenu.AddItem(new MenuItem("ElKalista.Draw.Q", "Draw Q").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElKalista.Draw.W", "Draw W").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElKalista.Draw.E", "Draw E").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElKalista.Draw.R", "Draw R").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElKalista.Draw.Text", "Draw Text").SetValue(true));

            var dmgAfterE = new MenuItem("ElKalista.DrawComboDamage", "Draw E damage").SetValue(true);
            var drawFill = new MenuItem("ElKalista.DrawColour", "Fill colour", true).SetValue(new Circle(true, Color.FromArgb(204, 204, 0, 0)));
            miscMenu.AddItem(drawFill);
            miscMenu.AddItem(dmgAfterE);

            EDamage.DamageToUnit = Damages.GetTotalDamage;
            EDamage.Enabled = dmgAfterE.GetValue<bool>();
            EDamage.Fill = drawFill.GetValue<Circle>().Active;
            EDamage.FillColor = drawFill.GetValue<Circle>().Color;

            dmgAfterE.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
            {
                EDamage.Enabled = eventArgs.GetNewValue<bool>();
            };

            drawFill.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
            {
                EDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                EDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            _menu.AddSubMenu(miscMenu);

            _menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            _menu.AddItem(new MenuItem("422442fsaafsf", (string.Format("ElKalista by jQuery v{0}", ScriptVersion))));
            _menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            _menu.AddToMainMenu();

            Console.WriteLine("Menu Loaded");
        }
    }
}
