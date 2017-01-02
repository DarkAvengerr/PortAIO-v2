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
 namespace ElEkko
{
    public class ElEkkoMenu
    {
        public static Menu _menu;

        public static void Initialize()
        {
            _menu = new Menu("ElEkko", "menu", true);
            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            ElEkko.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            _menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);

            _menu.AddSubMenu(targetSelector);

            var cMenu = new Menu("Combo", "Combo");

            cMenu.SubMenu("Q").AddItem(new MenuItem("ElEkko.Combo.Q", "Use Q").SetValue(true));
            cMenu.SubMenu("Q").AddItem(new MenuItem("ElEkko.Combo.Auto.Q", "Use Q when 2 stacks").SetValue(true));

            cMenu.SubMenu("W").AddItem(new MenuItem("ElEkko.Combo.W", "Use W").SetValue(true));
            cMenu.SubMenu("W").AddItem(new MenuItem("ElEkko.Combo.W.Stuned", "Use W on stunned targets").SetValue(true));
            cMenu.SubMenu("W").AddItem(new MenuItem("ElEkko.Combo.W.Count", "Minimum targets for W >=").SetValue(new Slider(3, 1, 5)));

            cMenu.SubMenu("E").AddItem(new MenuItem("ElEkko.Combo.E", "Use E").SetValue(true));
            cMenu.SubMenu("E").AddItem(new MenuItem("ElEkko.Combo.E.Cast", "Mode").SetValue(new StringList(new[] { "Cast to target", "Cast to mouse" })));

            cMenu.SubMenu("R").AddItem(new MenuItem("ElEkko.Combo.R", "Use R").SetValue(true));
            cMenu.SubMenu("R").AddItem(new MenuItem("ElEkko.Combo.R.Kill", "Use R when target can be killed").SetValue(true));
            cMenu.SubMenu("R").AddItem(new MenuItem("ElEkko.Combo.R.HP", "Use R when HP >=").SetValue(new Slider(25)));
            cMenu.SubMenu("R").AddItem(new MenuItem("ElEkko.Combo.R.Enemies", "Use R on enemies >=").SetValue(new Slider(3, 1, 5)));

            cMenu.SubMenu("Ignite").AddItem(new MenuItem("ElEkko.Combo.Ignite", "Use ignite").SetValue(true));


            _menu.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "Harass");

            hMenu.AddItem(new MenuItem("ElEkko.Harass.Q", "Use Q").SetValue(true));
            hMenu.SubMenu("E").AddItem(new MenuItem("ElEkko.Harass.E", "Use E").SetValue(true));
            hMenu.SubMenu("E").AddItem(new MenuItem("ElEkko.Harass.E.Cast", "Mode").SetValue(new StringList(new[] { "Cast to target", "Cast to mouse" })));
            hMenu.AddItem(new MenuItem("ElEkko.Harass.Q.Mana", "Minimum mana").SetValue(new Slider(55)));

            hMenu.SubMenu("Auto harass").AddItem(new MenuItem("ElEkko.AutoHarass.Q", "[Toggle] Auto harass", false).SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Toggle)));

            _menu.AddSubMenu(hMenu);

            var lMenu = new Menu("Clear", "Clear");
            lMenu.SubMenu("Laneclear").AddItem(new MenuItem("ElEkko.LaneClear.Q", "Use Q").SetValue(true));
            lMenu.SubMenu("Laneclear").AddItem(new MenuItem("ElEkko.LaneClear.Minions", "Use Q when minions >=").SetValue(new Slider(3, 1, 5)));
            lMenu.SubMenu("Laneclear").AddItem(new MenuItem("ElEkko.LaneClear.mana", "Minimum mana").SetValue(new Slider(55)));

            lMenu.SubMenu("Jungleclear").AddItem(new MenuItem("ElEkko.JungleClear.Q", "Use Q").SetValue(true));
            lMenu.SubMenu("Jungleclear").AddItem(new MenuItem("ElEkko.JungleClear.W", "Use W").SetValue(true));
            lMenu.SubMenu("Jungleclear").AddItem(new MenuItem("ElEkko.JungleClear.Minions", "Use Q when minions >=").SetValue(new Slider(1, 1, 5)));
            lMenu.SubMenu("Jungleclear").AddItem(new MenuItem("ElEkko.JungleClear.mana", "Minimum mana").SetValue(new Slider(55)));

            _menu.AddSubMenu(lMenu);

            var kMenu = new Menu("Killsteal", "Killsteal");
            kMenu.AddItem(new MenuItem("ElEkko.Killsteal.Active", "Use Killsteal").SetValue(true));
            kMenu.AddItem(new MenuItem("ElEkko.Killsteal.Ignite", "Use ignite").SetValue(true));
            kMenu.AddItem(new MenuItem("ElEkko.Killsteal.Q", "Use Q").SetValue(true));
            kMenu.AddItem(new MenuItem("ElEkko.Killsteal.R", "Use R").SetValue(false));
            _menu.AddSubMenu(kMenu);

            var fMenu = new Menu("Flee", "Flee");
            fMenu.AddItem(new MenuItem("ElEkko.Flee.Key", "Flee").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
            _menu.AddSubMenu(fMenu);

            var miscMenu = new Menu("Misc", "Misc");

            miscMenu.SubMenu("R misc").AddItem(new MenuItem("ElEkko.R.text", "Display how many people in R").SetValue(true));
            miscMenu.SubMenu("Drawings").AddItem(new MenuItem("ElEkko.Draw.off", "Turn drawings off").SetValue(false));
            miscMenu.SubMenu("Drawings").AddItem(new MenuItem("ElEkko.Draw.Q", "Draw Q").SetValue(new Circle()));
            miscMenu.SubMenu("Drawings").AddItem(new MenuItem("ElEkko.Draw.W", "Draw W").SetValue(new Circle()));
            miscMenu.SubMenu("Drawings").AddItem(new MenuItem("ElEkko.Draw.E", "Draw E").SetValue(new Circle()));
            miscMenu.SubMenu("Drawings").AddItem(new MenuItem("ElEkko.Draw.R", "Draw R").SetValue(new Circle()));

            var dmgAfterE = new MenuItem("ElEkko.DrawComboDamage", "Draw combo damage").SetValue(true);
            var drawFill = new MenuItem("ElEkko.DrawColour", "Fill colour", true).SetValue(new Circle(true, Color.FromArgb(204, 204, 0, 0)));
            miscMenu.SubMenu("Drawings").AddItem(drawFill);
            miscMenu.SubMenu("Drawings").AddItem(dmgAfterE);

            //DrawDamage.DamageToUnit = ElEkko.GetComboDamage;
            //DrawDamage.Enabled = dmgAfterE.GetValue<bool>();
            //DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            //DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;

            dmgAfterE.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
            {
                //DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
            };

            drawFill.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
            {
                //DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                //DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            _menu.AddSubMenu(miscMenu);

            _menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            _menu.AddItem(new MenuItem("422442fsaafsf", String.Format("Version: {0}", ElEkko.ScriptVersion)));
            _menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            _menu.AddToMainMenu();

        }
    }
}
