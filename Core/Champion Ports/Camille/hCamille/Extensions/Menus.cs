using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace hCamille.Extensions
{
    public static class Menus
    {
        public static Menu Config { get; set; }
        public static Orbwalking.Orbwalker Orbwalker { get; set; }

        public static void Initializer()
        {
            Config = new Menu("hCamille", "hCamille", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var combomenu = new Menu("Combo Settings", "Combo Settings");
                {
                    combomenu.AddItem(new MenuItem("q.settings", "                            [Q] Settings").SetFontStyle(FontStyle.Bold,
                        SharpDX.Color.Gold));
                    combomenu.AddItem(new MenuItem("q.combo", "Use [Q] ").SetValue(true));
                    combomenu.AddItem(new MenuItem("q.mode", "[Q] Type").SetValue(new StringList(new[] { "After Attack", "In AA Range" })));

                    combomenu.AddItem(new MenuItem("W.settings", "                            [W] Settings").SetFontStyle(FontStyle.Bold,
                        SharpDX.Color.Gold));
                    combomenu.AddItem(new MenuItem("w.combo", "Use [W] ").SetValue(true));
                    combomenu.AddItem(new MenuItem("w.mode", " [W Mode]").SetValue(new StringList(new[] { "While Dashing", "Always" }, 1)));

                    combomenu.AddItem(new MenuItem("E.settings", "                            [E] Settings").SetFontStyle(FontStyle.Bold,
                        SharpDX.Color.HotPink));
                    combomenu.AddItem(new MenuItem("e.combo", "Use [E] ").SetValue(true));
                    combomenu.AddItem(new MenuItem("wall.search.range", "[E] Wall Search Range").SetValue(new Slider(1300, 1, 2500))).SetTooltip("1300 is recommenced");
                    combomenu.AddItem(new MenuItem("wall.distance.to.enemy", "[E] Max Wall Distance to Enemy").SetValue(new Slider(865, 1, 1500))).SetTooltip("865 is recommenced");
                    combomenu.AddItem(new MenuItem("enemy.search.range", "[E] Enemy Search Range").SetValue(new Slider(1365, 1365, 1900))).SetTooltip("1365 is recommenced (1365 -> E.Range + 500)");
                    combomenu.AddItem(new MenuItem("max.enemy.count", "[E] Max Enemy Count").SetValue(new Slider(5, 1, 5)));
                    Config.AddSubMenu(combomenu);
                }

                var ultimatemenu = new Menu("Ultimate Settings", "Ultimate Settings");
                {
                    ultimatemenu.AddItem(new MenuItem("r.combo", "Use [R] ").SetValue(true));
                    ultimatemenu.AddItem(new MenuItem("enemy.health.percent", "[R] Enemy Health Percentage").SetValue(new Slider(30, 1, 99)));

                    var whitelist = new Menu("Ultimate Whitelist", "Ultimate Whitelist");
                    {
                        foreach (var enemy in HeroManager.Enemies)
                        {
                            whitelist.AddItem(new MenuItem("r."+enemy.ChampionName, "Use [R]:  "+enemy.ChampionName).SetValue(Utilities.HighChamps.Contains(enemy.ChampionName)));
                        }
                        ultimatemenu.AddSubMenu(whitelist);
                    }
                    ultimatemenu.AddItem(new MenuItem("r.mode", "[R] Type").SetValue(new StringList(new[] { "Auto", "Only Selected" })));
                    Config.AddSubMenu(ultimatemenu);
                }

                var harassmenu = new Menu("Harass Settings", "Harass Settings");
                {
                    harassmenu.AddItem(new MenuItem("q.harass", "Use [Q] ").SetValue(true));
                    harassmenu.AddItem(new MenuItem("w.harass", "Use [W] ").SetValue(true));
                    harassmenu.AddItem(new MenuItem("harass.mana", "Mana Manager").SetValue(new Slider(50, 1, 99))).SetFontStyle(FontStyle.Bold, SharpDX.Color.Gold);
                    Config.AddSubMenu(harassmenu);
                }

                var clearmenu = new Menu("Wave Settings", "Wave Settings");
                {
                    clearmenu.AddItem(new MenuItem("q.clear", "Use [Q]").SetValue(true));
                    clearmenu.AddItem(new MenuItem("w.clear", "Use [W]").SetValue(true));
                    clearmenu.AddItem(new MenuItem("min.count", "[W] Min. Minion Count").SetValue(new Slider(3, 1, 5)));
                    clearmenu.AddItem(new MenuItem("clear.mana", "Mana Manager").SetValue(new Slider(50, 1, 99))).SetFontStyle(FontStyle.Bold,SharpDX.Color.Gold);
                    Config.AddSubMenu(clearmenu);
                }

                var junglemenu = new Menu("Jungle Clear Settings", "Jungle Clear Settings");
                {
                    junglemenu.AddItem(new MenuItem("q.jungle", "Use [Q]").SetValue(true));
                    junglemenu.AddItem(new MenuItem("w.jungle", "Use [W]").SetValue(true));
                    junglemenu.AddItem(new MenuItem("jungle.mana", "Mana Manager").SetValue(new Slider(50, 1, 99))).SetFontStyle(FontStyle.Bold, SharpDX.Color.Gold);
                    Config.AddSubMenu(junglemenu);
                }

                var miscmenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscmenu.AddItem(new MenuItem("e.anti", "[E] -> Antigapcloser ?").SetValue(true));
                    miscmenu.AddItem(new MenuItem("e.interrupt", "[E] -> Interrupter ?").SetValue(true));
                    Config.AddSubMenu(miscmenu);
                }

                var drawMenu = new Menu("Draw Settings", "Draw Settings");
                {
                    var skillDraw = new Menu("Skill Draws", "Skill Draws");
                    {
                        skillDraw.AddItem(new MenuItem("q.draw", "Draw E Range").SetValue(new Circle(false, Color.White)));
                        skillDraw.AddItem(new MenuItem("w.draw", "Draw W Range").SetValue(new Circle(false, Color.White)));
                        skillDraw.AddItem(new MenuItem("e.draw", "Draw E Range").SetValue(new Circle(false, Color.White)));
                        skillDraw.AddItem(new MenuItem("r.draw", "Draw R Range").SetValue(new Circle(false, Color.White)));
                        drawMenu.AddSubMenu(skillDraw);
                    }

                    Config.AddSubMenu(drawMenu);

                }
                var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo Damage").SetValue(true);
                var drawFill = new MenuItem("RushDrawEDamageFill", "Combo Damage Fill").SetValue(new Circle(true, Color.Gold));

                drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
                drawMenu.SubMenu("Damage Draws").AddItem(drawFill);

                //DamageIndicator.DamageToUnit = TotalDamage;
                //DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
                //DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                //DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

                drawDamageMenu.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    //DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

                drawFill.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    //DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                    //DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
                Config.AddItem(
                    new MenuItem("keys", "                                      Keys").SetFontStyle(
                        FontStyle.Bold, SharpDX.Color.DodgerBlue));
                Config.AddItem(
                    new MenuItem("flee", "Flee!").SetValue(new KeyBind("A".ToCharArray()[0],
                        KeyBindType.Press)));
                Config.AddItem(
                    new MenuItem("credits.x1", "                          Developed by Hikigaya").SetFontStyle(
                        FontStyle.Bold, SharpDX.Color.Gold));
                Config.AddToMainMenu();
            }
        }

        private static float TotalDamage(AIHeroClient hero)
        {
            var damage = 0d;
            if (Spells.Q.IsReady())
            {
                if (Calculation.HasProtocolOneBuff)
                {
                    damage += hero.ProtocolDamage();
                }
                if (Calculation.HasProtocolTwoBuff)
                {
                    damage += hero.ProtocolTwoDamage();
                }
            }
            if (Spells.W.IsReady())
            {
                damage += hero.TacticalDamage();
            }
            if (Spells.E.IsReady())
            {
                damage += hero.WallDiveDamage();
            }
            if (Spells.R.IsReady())
            {
                damage += hero.HextechDamage()*4;
            }
            return (float)damage;
        }
    }
}
