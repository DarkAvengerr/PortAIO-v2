using System;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using System.Linq;
using SharpDX;
using Color = System.Drawing.Color;
using SPrediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace kSkarner
{
    internal class kSkarner
    {
        private static Menu config;
        private static Orbwalking.Orbwalker orbwalker;
        private static Spell Q, W, E, R;
        public static AIHeroClient player = ObjectManager.Player;


        public static void LoadkSkarner()
        {
            Q = new Spell(SpellSlot.Q, 350f);
            W = new Spell(SpellSlot.W); // self shield OP
            E = new Spell(SpellSlot.E, 1000f);
            E.SetSkillshot(E.Instance.SData.SpellCastTime, E.Instance.SData.LineWidth, E.Speed, false,
                SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 350f);
            Menu();
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Chat.Print("kSkarner Loaded#");

        }

        private static void Menu()
        {
            config = new Menu("kSkarner", "kSkarner", true);
            //ORB
            orbwalker = new Orbwalking.Orbwalker(config.SubMenu("Orbwalking"));
            //TS
            var ts = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(ts);
            config.AddSubMenu(ts);
            // [S]Prediction
            SPrediction.Prediction.Initialize(config);



            //COMBO
            var combo = new Menu("Combo", "Combo");
            combo.AddItem(new MenuItem("cuseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("cuseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("cuseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("cuseR", "Use R").SetValue(true));
            config.AddSubMenu(combo);


            // HARASS
            var harass = new Menu("Harass", "Harass");
            harass.AddItem(new MenuItem("useQH", "Use Q")).SetValue(true);
            harass.AddItem(new MenuItem("useEH", "Use E")).SetValue(true);
            harass.AddItem(new MenuItem("minEmanaH", "Minimun % Mana")).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(harass);


            // LaneClear Settings
            var laneclear = new Menu("LaneClear", "LaneClear");
            laneclear.AddItem(new MenuItem("useQC", "Use Q")).SetValue(true);
            laneclear.AddItem(new MenuItem("useEC", "Use E")).SetValue(true);
            laneclear.AddItem(new MenuItem("minEmanaC", "Minimun % Mana")).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(laneclear);


            // Misc
            var misc = new Menu("Misc", "Misc");
            //misc.AddItem(new MenuItem("SmartWUsage", "Smart W Usage")).SetValue(true);
            misc.AddItem(new MenuItem("useCPred", "Use [S]pacebar Prediction")).SetValue(true);
            misc.AddItem(new MenuItem("KSQ", "KS with Q")).SetValue(true);
            misc.AddItem(new MenuItem("KSE", "KS with E")).SetValue(true);
            config.AddSubMenu(misc);

            // Drawing
            var draw = new Menu("Drawing", "Drawing");
            draw.AddItem(new MenuItem("DrawQ", "Draw Q")).SetValue(true);
            draw.AddItem(new MenuItem("DrawE", "Draw E")).SetValue(true);
            config.AddSubMenu(draw);
            config.AddToMainMenu();
        }

        
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (SkarnerR)
            {
                orbwalker.SetAttack(false);
                orbwalker.SetMovement(false);
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
            else
            {
                orbwalker.SetAttack(true);
                orbwalker.SetMovement(true);
            }
            KS();
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                default:
                    break;
            }

        }


        private static void KS()
        {
            if (SkarnerR)
            {
                return;
            }
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            if (config.Item("KSQ").GetValue<bool>() && Q.IsReady() && target.Health < Q.GetDamage(target) && player.Distance(target) < Q.Range &&
                player.CountEnemiesInRange(Q.Range) == 1 && !target.IsDead && target.IsValidTarget())
            {
                Q.Cast();
                return;
            }
            if (config.Item("KSE").GetValue<bool>() && E.IsReady() && target.Health < Q.GetDamage(target) && player.Distance(target) < E.Range &&
                player.CountEnemiesInRange(E.Range) == 1 && !target.IsDead && target.IsValidTarget())
            {
                if (config.Item("useCPred").GetValue<bool>())
                    tryE(target);
                else
                {
                    E.Cast(target);
                }
            }
        }
        private static void Clear()
        {
                var mymana = (player.Mana >= (player.MaxMana*config.Item("minEmanaC").GetValue<Slider>().Value)/100);
                var testQ = MinionManager.GetMinions(Q.Range);
                var testQ2 = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                if (mymana && config.Item("useQC").GetValue<bool>() && Q.IsReady() && (testQ.Count > 0 || testQ2.Count > 0))
                {
                    Q.Cast();
                }
                if (mymana && config.Item("useEC").GetValue<bool>() && E.IsReady() && (testQ.Count > 0 || testQ2.Count > 0))
                {
                    MinionManager.FarmLocation bestE =
                        E.GetLineFarmLocation(MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly));
                    E.Cast(bestE.Position);
                }
        }
        private static void Combo()
        {
            Obj_AI_Base target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            if (SkarnerR)
            {
                return;
            }
            if (config.Item("cuseQ").GetValue<bool>() && Q.CanCast(target))
            {
                Q.Cast(target);
            }
            if (config.Item("cuseW").GetValue<bool>() || player.Distance(target) <= 600)
            {
                W.Cast();
            }
            if (config.Item("cuseE").GetValue<bool>() && E.CanCast(target))
            {
                if (config.Item("useCPred").GetValue<bool>())
                    tryE(target);
                else
                {
                    E.Cast(target);
                }
            }
            if (config.Item("cuseR").GetValue<bool>() && R.CanCast(target))
            {
                if ((player.CountEnemiesInRange(1500) == 1) && !target.HasBuffOfType(BuffType.Stun) && !target.HasBuffOfType(BuffType.Snare) && player.CountAlliesInRange(1000) > 1)
                {
                    R.Cast(target);
                }
                if (player.CountEnemiesInRange(1500) <= player.CountAlliesInRange(1000) && player.HealthPercent > 40)
                {
                    R.Cast(target);
                }
            }

        }

        private static void Harass()
        {
            Obj_AI_Base target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            var mymana = (player.Mana >= (player.MaxMana * config.Item("minEmanaC").GetValue<Slider>().Value) / 100);
            if (mymana && config.Item("useQH").GetValue<bool>() && Q.CanCast(target))
                {
                    Q.Cast(target);
                }
                if (mymana && config.Item("useEH").GetValue<bool>() && E.CanCast(target))
                {
                    if (config.Item("useCPred").GetValue<bool>())
                        tryE(target);
                    else
                    {
                        E.Cast(target);
                    }
                }
            }

        private static void tryE(Obj_AI_Base target)
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.IsValidTarget(E.Range)))
            {

                HitChance hCance = HitChance.High;
                E.SPredictionCast(enemy, hCance);
            }
        }
        // Thanks to Soresu //
        private static bool SkarnerR
        {
            get { return player.Buffs.Any(buff => buff.Name == "skarnerimpalevo"); }
        }

        private static void Game_OnDraw(EventArgs args)
        {
            if (config.Item("DrawQ").GetValue<bool>())
            {
                Drawing.DrawCircle(player.Position, Q.Range, Color.Red);
            }
            if (config.Item("DrawE").GetValue<bool>())
            {
                Drawing.DrawCircle(player.Position, E.Range, Color.Blue);
            }
        }
    }
}