using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using sAIO.Core;

using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace sAIO.Champions
{
    class BlitzcrankA : Helper
    {
       
      
        public BlitzcrankA()
        {
            Blitzcrank_OnGameLoad();
        }

        private static void Blitzcrank_OnGameLoad()
        {
            if (player.ChampionName != "Blitzcrank")
                return;

            Q = new Spell(SpellSlot.Q, 925f);
            Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 150f);
            R = new Spell(SpellSlot.R, 550f);

            menu.AddSubMenu(new Menu("Combo", "Combo"));
            CreateMenuBool("Combo", "Combo.Q", "Use Q", true);
            CreateMenuBool("Combo", "Combo.W", "Use W", true);
            CreateMenuBool("Combo", "Combo.E", "Use E", true);
            CreateMenuBool("Combo", "Combo.R", "Use R", true);
            //CreateMenuSlider("Combo", "Combo.RDelay", "R Delay", 0, 1000, 2000);

            menu.AddSubMenu(new Menu("Harass", "Harass"));
            CreateMenuBool("Harass", "Harass.Q", "Use Q", true);
            CreateMenuBool("Harass", "Harass.W", "Use W", true);
            CreateMenuBool("Harass", "Harass.E", "Use E", true);
            CreateMenuSlider("Harass", "Harass.MinManaPercent", "Min Mana Percent To Use", 0, 15, 100);

            menu.AddSubMenu(new Menu("Interrupter", "Interrupter"));
            CreateMenuBool("Interrupter", "Interrupter.E", "Use E", true);
            CreateMenuBool("Interrupter", "Interrupter.Q", "Use Q", true);

            menu.AddSubMenu(new Menu("GC", "GC"));
            CreateMenuBool("GC", "GC.E", "Use E", true);

            menu.AddSubMenu(new Menu("Drawing", "Draw"));
            CreateMenuBool("Draw", "Draw.Q", "Draw Q ", true);
            CreateMenuBool("Draw", "Draw.E", "Draw E", true);
            CreateMenuBool("Draw", "Draw.R", "Draw R", true);

            menu.AddToMainMenu();

            //AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
                return;

            if(E.IsReady() && GetValueMenuBool("GC.E") && E.IsInRange(gapcloser.Sender))
            {
                E.CastOnUnit(player);
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
            }
        }

        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if(Q.IsReady() && GetValueMenuBool("Interrupter.Q") && Q.IsInRange(sender) && args.DangerLevel != Interrupter2.DangerLevel.High)
            {
                var qHitChance = Q.GetPrediction(sender);

                if (qHitChance.Hitchance == HitChance.High && qHitChance.CollisionObjects.Count < 1)
                    Q.Cast(qHitChance.CastPosition);
            }

            if (E.IsReady() && GetValueMenuBool("GC.E") && E.IsInRange(sender))
            {
                E.CastOnUnit(player);
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (GetValueMenuBool("Draw.Q"))
                Drawing.DrawCircle(player.Position, Q.Range, Color.Blue);

            if (GetValueMenuBool("Draw.E"))
                Drawing.DrawCircle(player.Position, E.Range, Color.Green);

            if (GetValueMenuBool("Draw.R"))
                Drawing.DrawCircle(player.Position, R.Range, Color.Red);

        }

        static void Game_OnUpdate(EventArgs args)
        {
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo: Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed: Harass();
                    break;
            }

        }

        private static void Combo()
        {


            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if (W.IsReady() && GetValueMenuBool("Combo.W") && player.Distance(target.Position) < Q.Range)
                W.CastOnUnit(player);

            if(Q.IsReady() && GetValueMenuBool("Combo.Q") && Q.IsInRange(target))
            {
                var qPrediction = Q.GetPrediction(target);

                if (qPrediction.Hitchance == HitChance.High && qPrediction.CollisionObjects.Count < 1)
                    Q.Cast(qPrediction.CastPosition);
            }

            if(E.IsReady() && GetValueMenuBool("Combo.E") && E.IsInRange(target))
            {
                E.CastOnUnit(player);
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }

            if (R.IsReady() && GetValueMenuBool("Combo.R") && R.IsInRange(target) && !E.IsReady() && target.HasBuffOfType(BuffType.Knockup))
                R.Cast();
        }
        private static void Harass()
        {
            var minMana = GetValueMenuSlider("Harass.MinManaPercent");

            if (player.ManaPercent < minMana)
                return;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if (W.IsReady() && GetValueMenuBool("Harass.W") && player.Distance(target.Position) < Q.Range)
                W.CastOnUnit(player);

            if (Q.IsReady() && GetValueMenuBool("Harass.Q") && Q.IsInRange(target))
            {
                var qPrediction = Q.GetPrediction(target);

                if (qPrediction.Hitchance == HitChance.High && qPrediction.CollisionObjects.Count < 1)
                    Q.Cast(qPrediction.CastPosition);
            }

            if (E.IsReady() && GetValueMenuBool("Harass.E") && E.IsInRange(target))
            {
                E.CastOnUnit(player);
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }          
        }
        //static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        //{
            //throw new NotImplementedException();
        //}
    }
}
