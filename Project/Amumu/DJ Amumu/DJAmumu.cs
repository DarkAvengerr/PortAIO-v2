using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DJAmumu
{
    class Program
    {
        // Fields and Functions
        private static Orbwalking.Orbwalker Orbwalker;
        private static Menu Menu;
        private static Spell Q, W, E, R;
        private static SpellSlot Ignite;
        private static AIHeroClient Player;
        private static float IgniteDamage(Obj_AI_Base target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Amumu")
            {
                return;
            }
            Player = ObjectManager.Player;
            Notifications.AddNotification("DJ Amumu loaded", 10000);
            Q = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 300);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 550);
            Q.SetSkillshot(250f, 90f, 2000f, true, SkillshotType.SkillshotLine);
            Ignite = Player.GetSpellSlot("summonerdot");

            // Menu
            Menu = new Menu("DJ Amumu", "Amumu", true);
            var tsMenu = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(tsMenu);

            Menu.AddSubMenu(tsMenu);
            Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalking"));

            Menu.AddSubMenu(new Menu("Combo", "Combo"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("qc", "Use Q").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("wc", "Use W").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("ec", "Use E").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("rc", "Use R").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("enemiesr", "Min Enemies for R").SetValue(new Slider(3, 1, 5)));
            Menu.SubMenu("Combo").AddItem(new MenuItem("comboignite", "Use Ignite").SetValue(true));

            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("qh", "Use Q").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("wh", "Use W").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("eh", "Use E").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("harassmm", "Mana Manager").SetValue(new Slider(50, 1, 100)));

            Menu.AddSubMenu(new Menu("Lane Clear", "LaneClear"));
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("qlc", "Use Q").SetValue(true));
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("wlc", "Use W").SetValue(true));
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("elc", "Use E").SetValue(true));
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("lanemm", "Mana Manager").SetValue(new Slider(50, 1, 100)));

            Menu.AddSubMenu(new Menu("Jungle Clear", "JungleClear"));
            Menu.SubMenu("JungleClear").AddItem(new MenuItem("qjc", "Use Q").SetValue(true));
            Menu.SubMenu("JungleClear").AddItem(new MenuItem("wjc", "Use W").SetValue(true));
            Menu.SubMenu("JungleClear").AddItem(new MenuItem("ejc", "Use E").SetValue(true));
            Menu.SubMenu("JungleClear").AddItem(new MenuItem("junglemm", "Mana Manager").SetValue(new Slider(50, 1, 100)));

            Menu.AddSubMenu(new Menu("Last Hit", "LastHit"));
            Menu.SubMenu("LastHit").AddItem(new MenuItem("qlh", "Use Q").SetValue(true));
            Menu.SubMenu("LastHit").AddItem(new MenuItem("wlh", "Use W").SetValue(true));
            Menu.SubMenu("LastHit").AddItem(new MenuItem("elh", "Use E").SetValue(true));
            Menu.SubMenu("LastHit").AddItem(new MenuItem("lastmm", "Mana Manager").SetValue(new Slider(50, 1, 100)));
            Menu.SubMenu("LastHit").AddItem(new MenuItem("lasthitkeybinding", "Last Hit Key").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            Menu.AddSubMenu(new Menu("Kill Steal", "KillSteal"));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("qks", "Use Q").SetValue(true));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("eks", "Use E").SetValue(true));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("rks", "Use R").SetValue(true));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("ksignite", "Use Ignite").SetValue(true));

            Menu.AddSubMenu(new Menu("Misc", "Misc"));
            Menu.SubMenu("Misc").AddItem(new MenuItem("miscignite", "Ignite Mode").SetValue(new StringList(new[] { "Combo", "Kill Steal" })));

            Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("drawq", "Draw Q").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("draww", "Draw W").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("drawe", "Draw E").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("drawr", "Draw R").SetValue(true));

            Menu.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
        }
        // Methods
        static void Drawing_OnDraw(EventArgs args)
        {
            if (Menu.Item("drawq").GetValue<bool>())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.OrangeRed);
            }
            if (Menu.Item("draww").GetValue<bool>())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.OrangeRed);
            }
            if (Menu.Item("drawe").GetValue<bool>())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.OrangeRed);
            }
            if (Menu.Item("drawr").GetValue<bool>())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.OrangeRed);
            }
        }
        static void Game_OnGameUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo(target);
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass(target);
                    break;
            }
            LastHit();
            var ksTarget = ObjectManager.Get<AIHeroClient>().Where(t => t.IsValidTarget()).OrderBy(t => t.Health).FirstOrDefault();
            if (ksTarget != null)
                KillSteal(ksTarget);
        }
        static void Combo(AIHeroClient target)
        {
            if (!target.IsValidTarget())
            {
                return;
            }
            if (Menu.Item("qc").GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.GetPrediction(target).Hitchance >= HitChance.Medium)
            {
                Q.CastIfHitchanceEquals(target, HitChance.Medium);
            }
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
            {
                if (Menu.Item("wc").GetValue<bool>() && W.IsReady() && ObjectManager.Get<AIHeroClient>().Any(hero => hero.IsValidTarget(W.Range)))
                {
                    W.Cast();
                }
            }
            if (Menu.Item("ec").GetValue<bool>() && E.IsReady())
            {
                E.Cast(target);
            }
            var enemyCount = ObjectManager.Get<AIHeroClient>().Count(e => e.IsValidTarget(R.Range));
            if (Menu.Item("rc").GetValue<bool>() && enemyCount >= Menu.Item("enemiesr").GetValue<Slider>().Value && R.IsReady())
            {
                R.Cast();
            }
            if (Player.Distance(target.Position) <= 600 && IgniteDamage(target) >= target.Health && Menu.Item("comboignite").GetValue<bool>() && Menu.Item("miscignite").GetValue<StringList>().SelectedIndex == 0)
            {
                Player.Spellbook.CastSpell(Ignite, target);
            }
        }
        static void Harass(AIHeroClient target)
        {
            if (!target.IsValidTarget())
            {
                return;
            }
            if (Player.ManaPercentage() >= Menu.Item("harassmm").GetValue<Slider>().Value)
            {
                if (Menu.Item("qh").GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.GetPrediction(target).Hitchance >= HitChance.Medium)
                {
                    Q.CastIfHitchanceEquals(target, HitChance.Medium);
                }
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                {
                    if (Menu.Item("wh").GetValue<bool>() && W.IsReady() && ObjectManager.Get<AIHeroClient>().Any(hero => hero.IsValidTarget(W.Range)))
                    {
                        W.Cast();
                    }
                }
                if (Menu.Item("eh").GetValue<bool>() && E.IsReady())
                {
                    E.Cast(target);
                }
            }
        }
        static void LaneClear()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            if (Player.ManaPercentage() >= Menu.Item("lanemm").GetValue<Slider>().Value)
            {
                if (Menu.Item("qlc").GetValue<bool>() && Q.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.IsValidTarget())
                        {
                            Q.GetPrediction(minion);
                            Q.CastIfHitchanceEquals(minion, HitChance.Medium);
                        }
                    }
                }
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                {
                    if (Menu.Item("wlc").GetValue<bool>() && W.IsReady() && ObjectManager.Get<Obj_AI_Minion>().Any(minion => minion.IsValidTarget(W.Range)))
                    {
                        W.Cast();
                    }
                }
                if (Menu.Item("elc").GetValue<bool>() && E.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.IsValidTarget())
                        {
                            E.CastOnUnit(minion);
                        }
                    }
                }
            }
        }
        static void LastHit()
        {
            var keyActive = Menu.Item("lasthitkeybinding").GetValue<KeyBind>().Active;
            if (!keyActive)
                return;
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            if (Player.ManaPercentage() >= Menu.Item("lastmm").GetValue<Slider>().Value)
            {
                if (Menu.Item("qlh").GetValue<bool>() && Q.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.IsValidTarget())
                        {
                            Q.GetPrediction(minion);
                            Q.CastIfHitchanceEquals(minion, HitChance.Medium);
                        }
                    }
                }
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                {
                    if (Menu.Item("wlh").GetValue<bool>() && W.IsReady() && ObjectManager.Get<Obj_AI_Minion>().Any(minion => minion.IsValidTarget(W.Range)))
                    {
                        W.Cast();
                    }
                }
                if (Menu.Item("elh").GetValue<bool>() && E.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.IsValidTarget())
                        {
                            E.CastOnUnit(minion);
                        }
                    }
                }
            }
        }
        static void JungleClear()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (Player.ManaPercentage() >= Menu.Item("junglemm").GetValue<Slider>().Value)
            {
                if (Menu.Item("qjc").GetValue<bool>() && Q.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.IsValidTarget())
                        {
                            Q.GetPrediction(minion);
                            Q.CastIfHitchanceEquals(minion, HitChance.High);
                        }
                    }
                }
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                {
                    if (Menu.Item("wjc").GetValue<bool>() && W.IsReady() && ObjectManager.Get<Obj_AI_Minion>().Any(minion => minion.IsValidTarget(W.Range)))
                    {
                        W.Cast();
                    }
                }
                if (Menu.Item("ejc").GetValue<bool>() && E.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.IsValidTarget())
                        {
                            E.CastOnUnit(minion);
                        }
                    }
                }
            }
        }
        static void KillSteal(AIHeroClient Target)
        {
            var Champions = ObjectManager.Get<AIHeroClient>();
            if (Menu.Item("qks").GetValue<bool>() && Q.IsReady() && ObjectManager.Get<AIHeroClient>().Any(hero => hero.IsValidTarget(Q.Range)))
            {
                foreach (var champ in Champions.Where(champ => champ.Health <= ObjectManager.Player.GetSpellDamage(champ, SpellSlot.Q)))
                {
                    if (champ.IsValidTarget())
                    {
                        Q.GetPrediction(Target);
                        Q.CastIfHitchanceEquals(Target, HitChance.Medium);
                    }
                }
            }
            if (Menu.Item("eks").GetValue<bool>() && E.IsReady())
            {
                foreach (var champ in Champions.Where(champ => champ.Health <= ObjectManager.Player.GetSpellDamage(champ, SpellSlot.E)))
                {
                    if (champ.IsValidTarget())
                    {
                        E.CastOnUnit(champ);
                    }
                }
            }
            if (Menu.Item("rks").GetValue<bool>() && E.IsReady())
            {
                foreach (var champ in Champions.Where(champ => champ.Health <= ObjectManager.Player.GetSpellDamage(champ, SpellSlot.R)))
                {
                    if (champ.IsValidTarget())
                    {
                        R.CastOnUnit(champ);
                    }
                }
            }
            if (Player.Distance(Target.Position) <= 600 && IgniteDamage(Target) >= Target.Health && Menu.Item("comboignite").GetValue<bool>() && Menu.Item("ksignite").GetValue<bool>() && Menu.Item("miscignite").GetValue<StringList>().SelectedIndex == 1)
            {
                Player.Spellbook.CastSpell(Ignite, Target);
            }
        }
    }
}
