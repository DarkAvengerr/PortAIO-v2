using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Data;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
namespace HelpingLSharpNasus
{
    internal class Program
    {
        private const string ChampionName = "Nasus";

        public static int Sheen = 3057, Iceborn = 3025;

        public static Orbwalking.Orbwalker Orbwalker;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static Menu Config;

        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;

        public static void Main()
        {
            Game_OnGameLoad(new EventArgs());
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Chat.Print("Lord's Nasus Loaded! Enjoy :)");

            if (Player.ChampionName != ChampionName)
                return;

            Q = new Spell(SpellSlot.Q, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 50);

            // Main Menu
            Config = new Menu("Lord's Nasus", "Nasus", true);
            Config.AddToMainMenu();

            // OrbWalker Menu
            var orbwalkMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            Config.AddSubMenu(orbwalkMenu);

            // Target Selector Menu
            var targetSelector = new Menu("Target Selector", "ts");
            TargetSelector.AddToMenu(targetSelector);
            Config.AddSubMenu(targetSelector);

            // Combo Menu
            var comboMenu = new Menu("Combo", "Combo");
            comboMenu.AddItem(new MenuItem("ComboQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("ComboW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("ComboE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("ComboR", "Use R (Misc | Check)").SetValue(true).SetTooltip("Use R as Defensive Spell when HP Percent < Slider below (Smart)"));
            comboMenu.AddItem(new MenuItem("RHP", "Use R if % HP").SetValue(new Slider(25)));
            Config.AddSubMenu(comboMenu);

            // Lane Clear Menu
            var laneClearMenu = new Menu("Lane Clear", "laneclear");
            laneClearMenu.AddItem(new MenuItem("LCQ", "Use Q to stack").SetValue(true));
            laneClearMenu.AddItem(new MenuItem("LCE", "Use E").SetValue(false));
            Config.AddSubMenu(laneClearMenu);

            // LastHit Menu
            var lastHitMenu = new Menu("LastHit", "lasthit");
            lastHitMenu.AddItem(new MenuItem("LHQ", "Use Q to stack").SetValue(true));
            lastHitMenu.AddItem(new MenuItem("manamanagerQ", "Mana Percent before using Q").SetValue(new Slider(50)));           
            Config.AddSubMenu(lastHitMenu);

            // Harass Menu
            var harassMenu = new Menu("Harass", "Harass");
            harassMenu.AddItem(new MenuItem("HQ", "Use Q to Harass").SetValue(true));
            harassMenu.AddItem(new MenuItem("HW", "Use W to Harass").SetValue(true));
            harassMenu.AddItem(new MenuItem("HE", "Use E to Harass").SetValue(true));
            Config.AddSubMenu(harassMenu);

            // Drawings Menu
            var drawMenu = new Menu("Drawings", "Drawings");
            drawMenu.AddItem(new MenuItem("DQ", "Draw Q Range").SetValue(true));
            drawMenu.AddItem(new MenuItem("DW", "Draw W Range").SetValue(true));
            drawMenu.AddItem(new MenuItem("DE", "Draw E Range").SetValue(true));
            drawMenu.AddItem(new MenuItem("LH", "LastHit Minions").SetValue(true));

            Config.AddSubMenu(drawMenu);

            // Credits to this boy ;) 
            var credsMenu = new Menu("Credits", "Credits");
            credsMenu.AddItem(new MenuItem("Credits", ".:Credits:.").SetFontStyle(FontStyle.Bold, SharpDX.Color.Chartreuse));
            credsMenu.AddItem(new MenuItem("CreditsBoy", "SupportExTraGoZ").SetFontStyle(FontStyle.Bold, SharpDX.Color.DeepPink));
            credsMenu.AddItem(new MenuItem("CreditsBoy2", "Screeder").SetFontStyle(FontStyle.Bold, SharpDX.Color.BlueViolet));
            credsMenu.AddItem(new MenuItem("CreditsBoy3", "NentoR").SetFontStyle(FontStyle.Bold, SharpDX.Color.BlueViolet));
            Config.AddSubMenu(credsMenu);

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void   Drawing_OnDraw(EventArgs args)
        {
            // Accurate Draw AutoAttack
            if (Config.Item("DQ").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Chartreuse);
            if (Config.Item("DW").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.DeepPink);
            if (Config.Item("DE").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.DeepSkyBlue);
            if (Config.Item("LH").GetValue<bool>())
            {
                var minions = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    E.Range,
                    MinionTypes.All,
                    MinionTeam.NotAlly);
                foreach (var minion in minions)
                {
                    if (minion != null)
                    {
                        if ((GetBonusDmg(minion) > minion.Health))
                        {
                            Render.Circle.DrawCircle(minion.ServerPosition, minion.BoundingRadius, Color.Green);
                        }
                    }
                }
            }
        }
        private static double GetBonusDmg(Obj_AI_Base target)
        {
            double dmgItem = 0;
            if (Items.HasItem(Sheen) && (Items.CanUseItem(Sheen) || Player.HasBuff("sheen"))
                && Player.BaseAttackDamage > dmgItem)
            {
                dmgItem = Player.GetAutoAttackDamage(target);
            }

            if (Items.HasItem(Iceborn) && (Items.CanUseItem(Iceborn) || Player.HasBuff("itemfrozenfist"))
                && Player.BaseAttackDamage * 1.25 > dmgItem)
            {
                dmgItem = Player.GetAutoAttackDamage(target) * 1.25;
            }

            return Q.GetDamage(target) + Player.GetAutoAttackDamage(target) + dmgItem;
        }

        private static void Combo()
        {
            var ComboQ = Config.Item("ComboQ").GetValue<bool>();
            var ComboW = Config.Item("ComboW").GetValue<bool>();
            var ComboE = Config.Item("ComboE").GetValue<bool>();
            var RHP = Config.Item("RHP").GetValue<Slider>().Value;

            var TargetEE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (!TargetEE.IsValidTarget() || TargetEE == null)
                return;

            if (TargetEE.IsValidTarget(Q.Range) && ComboQ && Q.IsReady())
                Q.Cast();
            Orbwalker.ForceTarget(TargetEE);

            if (TargetEE.IsValidTarget(W.Range) && ComboW && W.IsReady())
                W.CastOnUnit(TargetEE);

            if (TargetEE.IsValidTarget(E.Range) && ComboE && E.IsReady())
                E.Cast(TargetEE.Position);

        }

        private static void RCheck()
        {
            if (Config.Item("ComboR").GetValue<bool>() && R.IsReady() && Player.HealthPercent < Config.Item("RHP").GetValue<Slider>().Value && Player.CountEnemiesInRange(W.Range) > 0)
            {
                R.Cast();
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }
            RCheck();
        }

        private static void LaneClear()
        {
            var StackQ = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
            var MQ = Config.Item("LCQ").GetValue<bool>();
            var ME = Config.Item("LCE").GetValue<bool>();
            var MECast = MinionManager.GetMinions(E.Range + E.Width);
            var ELoc = E.GetCircularFarmLocation(MECast, E.Range);

            if (MQ)
            {
                foreach (var minion in StackQ)
                {

                    if (minion.Health <= Q.GetDamage(minion)
                        + Player.GetAutoAttackDamage(minion) && Q.IsReady())
                    {
                        Q.Cast();
                        Orbwalker.ForceTarget(minion);
                    }
                }
            }
            if (ME)
            {
                foreach (var minion in MECast)
                {
                    if (E.IsInRange(minion))
                    {
                        E.Cast(ELoc.Position);

                    }
                }
            }
        }

        private static void Harass()
        {
            var HarassQ = Config.Item("HQ").GetValue<bool>();
            var HarassW = Config.Item("HW").GetValue<bool>();
            var HarassE = Config.Item("HE").GetValue<bool>();

            var Qtarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            var Wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            var Etarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (HarassQ)
            {
                if (Qtarget.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    Q.Cast();
                }
            }

            if (HarassW)
            {
                if (Wtarget.IsValidTarget(W.Range) && W.IsReady())
                {
                    W.Cast();
                }

            }

            if (HarassE)
            {
                if (Etarget.IsValidTarget(E.Range) && E.IsReady())
                {
                    E.CastIfHitchanceEquals(Etarget, HitChance.VeryHigh);
                }
            }
        }
       
        private static void LastHit()
        {
            var useQLastHit = Config.Item("LHQ").GetValue<bool>();
            var manamanagerQ = Config.Item("manamanagerQ").GetValue<Slider>().Value;
            var minionQ = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);


            if (useQLastHit)
            {
                foreach (var minion in minionQ)
                {
                    if (manamanagerQ <= Player.ManaPercent && minion.Health <= Q.GetDamage(minion) + Player.GetAutoAttackDamage(minion) && Q.IsReady())
                    {
                        Q.Cast();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                    }
                }
            }
        }
    }
}
       
           




