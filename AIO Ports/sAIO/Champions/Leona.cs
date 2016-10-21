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
    class Leona : Helper
    {
        static int lastE = 0;
        public Leona()
        {
            Leona_OnGameLoad();
        }
        static void Leona_OnGameLoad()
        {
            Q = new Spell(SpellSlot.Q, player.AttackRange + 20);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 1200f);
            E.SetSkillshot(0.25f, 120f, 2000f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            menu.AddSubMenu(new Menu("Combo", "Combo"));
            CreateMenuBool("Combo", "Combo.Q", "Use Q", true);
            CreateMenuBool("Combo", "Combo.W", "Use W", true);
            CreateMenuBool("Combo", "Combo.E", "Use E", true);
            CreateMenuBool("Combo", "Combo.R", "Use R", true);
            CreateMenuSlider("Combo", "Combo.RDelay", "R Delay", 0, 2000, 3000);

            menu.AddSubMenu(new Menu("Harass", "Harass"));
            CreateMenuBool("Harass", "Harass.Q", "Use Q", true);
            CreateMenuBool("Harass", "Harass.W", "Use W", true);
            CreateMenuBool("Harass", "Harass.E", "Use E", true);
            CreateMenuSlider("Harass", "Harass.MinManaPercent", "Min Mana Percent To Use", 0, 15, 100);


            menu.AddSubMenu(new Menu("Interrupter", "Interrupter"));
            CreateMenuBool("Interrupter", "Interrupter.E", "Use E", true);
            CreateMenuBool("Interrupter", "Interrupter.Q", "Use Q", true);

            menu.AddSubMenu(new Menu("GC", "GC"));
            CreateMenuBool("GC", "GC.Q", "Use Q", true);

            menu.AddSubMenu(new Menu("Drawing", "Draw"));
            CreateMenuBool("Draw", "Draw.Q", "Draw Q ", true);
            CreateMenuBool("Draw", "Draw.E", "Draw E", true);
            CreateMenuBool("Draw", "Draw.R", "Draw R", true);

            menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == E.Instance.SData.Name)
                    lastE = Environment.TickCount;
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

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }

            if (W.IsReady() && gapcloser.Sender.IsValidTarget(Q.Range))
            {
                Q.Cast();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
            }
        }

        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsInRange(sender) && GetValueMenuBool("Interrupter.E") && E.IsReady())
            {
                E.Cast(sender);
            }

            if (Q.IsInRange(sender) && GetValueMenuBool("Interrupter.Q") && Q.IsReady())
            {
                Q.Cast();   
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
            }
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
        static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if(target == null || !target.IsValidTarget())
                return;

            if (Q.IsReady() && GetValueMenuBool("Combo.Q"))
                Q.Cast();

            if(E.IsReady() && E.IsInRange(target) && GetValueMenuBool("Combo.E"))
            {
                var eHitChance = E.GetPrediction(target);

                if (eHitChance.Hitchance == HitChance.High)
                {
                    E.Cast(target);                    
                }
                   
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }

            if (W.IsReady() && GetValueMenuBool("Combo.W"))
                W.Cast();

            if (R.IsReady() && R.IsInRange(target) && GetValueMenuBool("Combo.R") && Environment.TickCount - lastE >= GetValueMenuSlider("Combo.RDelay"))
                R.Cast(target);
        }
        static void Harass()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValidTarget())
                return;

            if (Q.IsReady() && GetValueMenuBool("Harass.Q"))
                Q.Cast();

            if (E.IsReady() && E.IsInRange(target) && GetValueMenuBool("Harass.E"))
            {
                var eHitChance = E.GetPrediction(target);

                if (eHitChance.Hitchance == HitChance.High)
                {
                    E.Cast(target);
                }

                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }

            if (W.IsReady() && GetValueMenuBool("Harass.W"))
                W.Cast();
        }
    }

}
