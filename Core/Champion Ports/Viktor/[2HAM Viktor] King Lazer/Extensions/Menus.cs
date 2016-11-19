using LeagueSharp.Common;
using System.Drawing;
using Color = SharpDX.Color;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Viktor.Extensions
{
    internal class Menus
    {
        public static Menu menuCfg;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void InitializeMenu()
        {
            menuCfg = new Menu("..:: [2HAM] Viktor", "2ham.Viktor", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(menuCfg.SubMenu("..:: Orbwalker Settings"));

                var QMenu = new Menu("..:: Spell Config [Q]", "spells.config.q");
                {
                    QMenu.AddItem(new MenuItem("q.combo",   "Use [Q] in Combo").SetValue(true));
                    QMenu.AddItem(new MenuItem("q.harass",  "Use [Q] in Harass").SetValue(true));
                    QMenu.AddItem(new MenuItem("q.lasthit", "Use [Q] Unkilable minion").SetValue(true));
                    QMenu.AddItem(new MenuItem("q.uber",    "Use [Q] Uber Mode").SetValue(true).SetTooltip("Use [Q] in minion if target is far away from [Q] Range"));
                    menuCfg.AddSubMenu(QMenu);
                }
                var WMenu = new Menu("..:: Spell Config [W]", "spells.config.w");
                {
                    WMenu.AddItem(new MenuItem("w.combo",           "Use [W] in Combo").SetValue(false)).SetTooltip("Recommended set [OFF]", Color.GreenYellow);
                    WMenu.AddItem(new MenuItem("w.harass",          "Use [W] in Harass").SetValue(false)).SetTooltip("Recommended set [OFF]", Color.GreenYellow);
                    WMenu.AddItem(new MenuItem("w.misc",            "Use [W] in CC/Immobile Target").SetValue(true));
                    WMenu.AddItem(new MenuItem("w.gapcloser",       "Use [W] Anti-GapCloser").SetValue(true));
                    WMenu.AddItem(new MenuItem("sel.predict.w",     "Select Prediction:").SetValue(new StringList(new[] { "Common", "OKTW Prediction" }, 1)));
                    WMenu.AddItem(new MenuItem("sel.hitchance.w",   "Hit Chance:").SetValue(new StringList(Utilities.HitchanceNameArray, 2)));
                    WMenu.AddItem(new MenuItem("tip.hitchance.w",   "Recommended set HitChance for [Medium] in Commom/OKTW Prediction"));
                    menuCfg.AddSubMenu(WMenu);
                }
                var EMenu = new Menu("..:: Spell Config [E]", "spells.config.e");
                {
                    EMenu.AddItem(new MenuItem("e.combo",           "Use [E] in Combo").SetValue(true));
                    EMenu.AddItem(new MenuItem("e.harass",          "Use [E] in Harass").SetValue(true));
                    EMenu.AddItem(new MenuItem("e.clear",           "Use [E] in Lane Clear").SetValue(true));
                    EMenu.AddItem(new MenuItem("e.clear.minhit",    "Min. minions to Hit [E]")).SetValue(new Slider(4, 1, 10));
                    EMenu.AddItem(new MenuItem("sel.hitchance.e",   "Hit Chance:").SetValue(new StringList(Utilities.HitchanceNameArray, 3)));
                    EMenu.AddItem(new MenuItem("tip.hitchance.e",   "Recommended set HitChance to [High]"));
                    menuCfg.AddSubMenu(EMenu);
                }
                var RMenu = new Menu("..:: Spell Config [R]", "spells.config.r");
                {
                    RMenu.AddItem(new MenuItem("r.combo",           "Use [R] in Combo").SetValue(true));
                    RMenu.AddItem(new MenuItem("r.interrupt",       "Use [R] Interruptable").SetValue(true));
                    RMenu.AddItem(new MenuItem("r.follow",          "Use [R] Auto Follow").SetValue(true));
                    RMenu.AddItem(new MenuItem("r.min.hit",         "Minimum Enemys to Hit [R]")).SetValue(new Slider(2, 1, 5));
                    RMenu.AddItem(new MenuItem("sel.hitchance.r",   "Hit Chance:").SetValue(new StringList(Utilities.HitchanceNameArray, 2)));
                    RMenu.AddItem(new MenuItem("tip.hitchance.r",   "Recommended set HitChance for [Very High]"));
                    menuCfg.AddSubMenu(RMenu);
                }
                var RWhiteList = new Menu("Use [R] WhiteList", "r.whitelist");
                {
                    foreach (var enemy in HeroManager.Enemies)
                    {
                        RWhiteList.AddItem(new MenuItem("r.champ.whitelist" + enemy.ChampionName, "Use [R] on:  " + enemy.ChampionName).SetValue(true).SetFontStyle(FontStyle.Bold, Color.Gold));
                    }
                    RMenu.AddSubMenu(RWhiteList);
                }
                var MiscMenu = new Menu("..:: Misc Config", "misc.config");
                {
                    MiscMenu.AddItem(new MenuItem("q.misc", "Disabe Auto Attack").SetValue(true));
                    menuCfg.AddSubMenu(MiscMenu);
                }
                var DrawMenu = new Menu("..:: Drawming Config", "draw.config");
                {
                    DrawMenu.AddItem(new MenuItem("draw.q",     "Draw [Q] Range").SetValue(false));
                    DrawMenu.AddItem(new MenuItem("draw.w",     "Draw [W] Range").SetValue(false));
                    DrawMenu.AddItem(new MenuItem("draw.e",     "Draw [E] Range").SetValue(true));
                    DrawMenu.AddItem(new MenuItem("draw.r",     "Draw [R] Range").SetValue(false));
                    menuCfg.AddSubMenu(DrawMenu);
                }

                #region Draw Damage
                var drawDamageMenu = new MenuItem("draw.combo.damage", "Draw [Combo] Damage").SetValue(true);
                var drawFill = new MenuItem("draw.combo.damage.fill", "Draw [Combo] Damage Fill").SetValue(new Circle(true, System.Drawing.Color.Gold));

                DrawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
                DrawMenu.SubMenu("Damage Draws").AddItem(drawFill);

                DrawingDamage.DamageToUnit = Champions.Viktor.CalculateDamage;
                DrawingDamage.Enabled = drawDamageMenu.GetValue<bool>();
                DrawingDamage.Fill = drawFill.GetValue<Circle>().Active;
                DrawingDamage.FillColor = drawFill.GetValue<Circle>().Color;

                drawDamageMenu.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    DrawingDamage.Enabled = eventArgs.GetNewValue<bool>();
                };

                drawFill.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    DrawingDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                    DrawingDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
                #endregion

                menuCfg.AddToMainMenu();
            }

        }
    }
}
