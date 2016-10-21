using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using EloBuddy;

namespace ElTristana
{

    public class MenuInit
    {

        public static Menu Menu;

        public static void Initialize()
        {
            Menu = new Menu("ElTristana", "menu", true);

            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Tristana.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            Menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);

            Menu.AddSubMenu(targetSelector);

            #region 

            var comboMenu = new Menu("Combo", "Combo");
            {
                comboMenu.AddItem(new MenuItem("ElTristana.Combo.Q", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElTristana.Combo.E", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElTristana.Combo.R", "Use R").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElTristana.Combo.Focus.E", "Focus E target").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElTristana.Combo.Always.RE", "Use E + R finisher").SetValue(false));
                comboMenu.AddItem(new MenuItem("ElTristana.Combo.E.Mana", "Minimum mana for E")).SetValue(new Slider(25));

                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
                    comboMenu.SubMenu("Use E on")
                        .AddItem(
                            new MenuItem("ElTristana.E.On" + hero.ChampionName, hero.ChampionName)
                                .SetValue(true));
            }

            Menu.AddSubMenu(comboMenu);

            #endregion

            var suicideMenu = new Menu("W settings", "Suicide menu").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow);
            {
                suicideMenu.AddItem(new MenuItem("ElTristana.W", "Use this special feature").SetValue(false));
                suicideMenu.AddItem(new MenuItem("ElTristana.W.Jump.kill", "Only jump when killable").SetValue(false));
                suicideMenu.AddItem(new MenuItem("ElTristana.W.Jump.tower", "Check under tower").SetValue(true));
                suicideMenu.AddItem(new MenuItem("ElTristana.W.Jump", "W to enemy with 4 stacks").SetValue(true));
                suicideMenu.AddItem(new MenuItem("ElTristana.W.Enemies", "Only jump when enemies in range")).SetValue(new Slider(1, 1, 5));
                suicideMenu.AddItem(new MenuItem("ElTristana.W.Enemies.Range", "Enemies in range distance check")).SetValue(new Slider(1500, 800, 2000));
            }

            Menu.AddSubMenu(suicideMenu);


            #region 

            var harassMenu = new Menu("Harass", "Harass");
            {
                harassMenu.AddItem(new MenuItem("ElTristana.Harass.Q", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("ElTristana.Harass.E", "Use E").SetValue(true));
                harassMenu.AddItem(new MenuItem("ElTristana.Harass.QE", "Use Q only with E").SetValue(true));
                harassMenu.AddItem(new MenuItem("ElTristana.Harass.E.Mana", "Minimum mana for E")).SetValue(new Slider(25));

                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
                    harassMenu.SubMenu("Use E on")
                        .AddItem(
                            new MenuItem("ElTristana.E.On.Harass" + hero.CharData.BaseSkinName, hero.CharData.BaseSkinName)
                                .SetValue(true));
            }

            Menu.AddSubMenu(harassMenu);

            #endregion

            #region 

            var itemMenu = new Menu("Items", "Items");
            {
                itemMenu.AddItem(new MenuItem("ElTristana.Items.Youmuu", "Use Youmuu's Ghostblade").SetValue(true));
                itemMenu.AddItem(new MenuItem("ElTristana.Items.Blade", "Use Blade of the Ruined King").SetValue(true));
                itemMenu.AddItem(
                    new MenuItem("ElTristana.Items.Blade.EnemyEHP", "Enemy HP Percentage").SetValue(new Slider(80, 100, 0)));
                itemMenu.AddItem(
                    new MenuItem("ElTristana.Items.Blade.EnemyMHP", "My HP Percentage").SetValue(new Slider(80, 100, 0)));
            }

            Menu.AddSubMenu(itemMenu);

            #endregion

            #region

            var laneClearMenu = new Menu("Laneclear", "Laneclear");
            {
                laneClearMenu.AddItem(new MenuItem("ElTristana.LaneClear.Q", "Use Q").SetValue(true));
                laneClearMenu.AddItem(new MenuItem("ElTristana.LaneClear.E", "Use E").SetValue(true));
                laneClearMenu.AddItem(new MenuItem("ElTristana.LaneClear.Tower", "Use E on tower").SetValue(true));
                laneClearMenu.AddItem(new MenuItem("ElTristana.LaneClear.E.Mana", "Minimum mana for E")).SetValue(new Slider(25));
            }

            Menu.AddSubMenu(laneClearMenu);

            var jungleClearMenu = new Menu("Jungleclear", "Jungleclear");
            {
                jungleClearMenu.AddItem(new MenuItem("ElTristana.JungleClear.Q", "Use Q").SetValue(true));
                jungleClearMenu.AddItem(new MenuItem("ElTristana.JungleClear.E", "Use E").SetValue(true));
                jungleClearMenu.AddItem(new MenuItem("ElTristana.JungleClear.E.Mana", "Minimum mana for E")).SetValue(new Slider(25));
            }

            Menu.AddSubMenu(jungleClearMenu);

            var killstealMenu = new Menu("Killsteal", "Killsteal");
            {
                killstealMenu.AddItem(new MenuItem("ElTristana.killsteal.Active", "Activate killsteal").SetValue(true));
                killstealMenu.AddItem(new MenuItem("ElTristana.Killsteal.R", "Use R").SetValue(true));
            }

            Menu.AddSubMenu(killstealMenu);

            #endregion

            #region 

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddItem(new MenuItem("ElTristana.Draw.off", "Turn drawings off").SetValue(true));
                miscMenu.AddItem(new MenuItem("ElTristana.DrawStacks", "Draw E stacks").SetValue(true));
                miscMenu.AddItem(new MenuItem("ElTristana.Draw.Q", "Draw Q").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElTristana.Draw.E", "Draw E").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElTristana.Draw.R", "Draw R").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElTristana.Antigapcloser", "Antigapcloser").SetValue(false));
                miscMenu.AddItem(new MenuItem("ElTristana.Interrupter", "Interrupter").SetValue(false));

                var dmgAfterE = new MenuItem("ElTristana.DrawComboDamage", "Draw combo damage").SetValue(true);
                var drawFill =
                    new MenuItem("ElTristana.DrawColour", "Fill colour", true).SetValue(
                        new Circle(true, Color.Goldenrod));
                miscMenu.AddItem(drawFill);
                miscMenu.AddItem(dmgAfterE);

                DamageIndicator.DamageToUnit = Tristana.GetComboDamage;
                DamageIndicator.Enabled = dmgAfterE.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

                dmgAfterE.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };

                drawFill.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                    DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
            }

            Menu.AddSubMenu(miscMenu);

           

            #endregion

            #region 

            var credits = new Menu("Credits", "jQuery");
            {
                credits.AddItem(new MenuItem("ElTristana.Paypal", "if you would like to donate via paypal:"));
                credits.AddItem(new MenuItem("ElTristana.Email", "info@zavox.nl"));
            }
            Menu.AddSubMenu(credits);

            Menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            Menu.AddItem(
                new MenuItem("422442fsaafsf", ($"ElTristana by jQuery v{Tristana.ScriptVersion}")));
            Menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            Menu.AddToMainMenu();

            #endregion
        }
    }
}