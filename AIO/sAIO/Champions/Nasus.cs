using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using sAIO.Core;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace sAIO.Champions
{
    internal class NasusA : Helper
    {
        internal NasusA()
        {
            Nasus_OnGameLoad();
        }
        private static void Nasus_OnGameLoad()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 600f);
            E = new Spell(SpellSlot.E, 650f);
            R = new Spell(SpellSlot.R);

            menu.AddSubMenu(new Menu("Combo","Combo"));
            CreateMenuBool("Combo", "Combo.Q", "Use Q", true);
            CreateMenuBool("Combo", "Combo.W", "Use W", true);
            CreateMenuBool("Combo", "Combo.E", "Use E", true);
            CreateMenuBool("Combo", "Combo.R", "Use R", true);

            menu.AddSubMenu(new Menu("Harass", "Harass"));
            CreateMenuBool("Harass", "Harass.Q", "Use Q", true);
            CreateMenuBool("Harass", "Harass.W", "Use W", true);
            CreateMenuBool("Harass", "Harass.E", "Use E", true);

            menu.AddSubMenu(new Menu("Kill Steal", "KS"));
            CreateMenuBool("KS", "KS.Q", "Use Q", true);
            CreateMenuBool("KS", "KS.E", "Use E", false);

            menu.AddSubMenu(new Menu("Gap closer", "GC"));
            CreateMenuBool("GC", "GC.W", "Use W", true);

            menu.AddSubMenu(new Menu("Draw", "Draw"));
            CreateMenuBool("Draw", "Draw.Q", "Draw Q killable minion", true);
            CreateMenuBool("Draw", "Draw.W", "Draw W", true);
            CreateMenuBool("Draw", "Draw.E", "Draw E", true);

            menu.AddSubMenu(new Menu("Auto Farm Q", "AutoFarmQ"));
            CreateMenuBool("AutoFarmQ", "AutoFarmQ.Enable", "Enable", true);

            menu.AddSubMenu(new Menu("R", "R"));
            CreateMenuBool("R", "R.AutoR", "Auto use R", false);
            CreateMenuSlider("R", "R.MinHealth", "Use R if health >= ", 0, 10, 100);

            menu.AddSubMenu(new Menu("Farm", "Farm"));
            CreateMenuBool("Farm", "Farm.Q", "Use Q", true);

            menu.AddSubMenu(new Menu("Lane Clean", "LC"));
            CreateMenuBool("LC", "LC.Q", "Use Q", true);
            CreateMenuBool("LC", "LC.E", "Use E", true);
            menu.AddToMainMenu();                    

            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

            Chat.Print("sAIO: " + player.ChampionName + " loaded");

        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly || !GetValueMenuBool("GC.W")) return;

            if (W.IsReady() && W.IsInRange(gapcloser.Sender))
                W.CastOnUnit(gapcloser.Sender);
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if(GetValueMenuBool("Draw.Q") && Q.IsReady())
            {
                foreach(var qMinion in MinionManager.GetMinions(Q.Range))
                {
                    var totalQDamage = (Q.GetDamage(qMinion) + player.GetAutoAttackDamage(qMinion)) * 0.93;

                    if (qMinion.Health < totalQDamage && Q.IsReady())
                        Drawing.DrawCircle(qMinion.ServerPosition, qMinion.BoundingRadius, System.Drawing.Color.Green);
                }                                
            }

            if (GetValueMenuBool("Draw.W"))
                Drawing.DrawCircle(player.Position, W.Range, System.Drawing.Color.Red);

            if (GetValueMenuBool("Draw.E"))
                Drawing.DrawCircle(player.Position, E.Range, System.Drawing.Color.Blue);
        }

        static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if(orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo | orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if(Q.IsReady() && (GetValueMenuBool("Combo.Q") || GetValueMenuBool("Harass.Q")))
                {
                    Q.Cast();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            switch(orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    Farm();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClean();
                    break;
            }

            if(GetValueMenuBool("AutoFarmQ.Enable"))
                AutoFarmQ();

            KillSteal();
            AutoR();
        }
        private static void Combo()
        {
            var target=TargetSelector.GetTarget(E.Range,TargetSelector.DamageType.Magical);

            if (GetValueMenuBool("Combo.R") && R.IsReady() && target.IsValidTarget(R.Range))
                R.Cast();

            if (GetValueMenuBool("Combo.E") && E.IsReady() && target.IsValidTarget(E.Range))
                E.Cast(target.Position);

            if (GetValueMenuBool("Combo.W") && W.IsReady() && target.IsValidTarget(W.Range))
                W.CastOnUnit(target);
        }
        private static void Harass()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (GetValueMenuBool("Harass.E") && E.IsReady() && target.IsValidTarget(E.Range))
                E.Cast(target.Position);

            if (GetValueMenuBool("Harass.W") && W.IsReady() && target.IsValidTarget(W.Range))
                W.CastOnUnit(target);
        }
        private static void Farm()
        {
            if(GetValueMenuBool("Farm.Q") && Q.IsReady())
            {
                var qMinion = MinionManager.GetMinions(E.Range).Where(m => orbwalker.InAutoAttackRange(m))
                    .FirstOrDefault(m => m.Health < (Q.GetDamage(m) + player.GetAutoAttackDamage(m)) * 0.93);

                if(qMinion != null)
                {
                    Q.Cast();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, qMinion);
                }
            }
        }
        private static void LaneClean()
        {           
            var minions = MinionManager.GetMinions(E.Range);

            var pos = E.GetCircularFarmLocation(minions);

            if (GetValueMenuBool("LC.E") && pos.MinionsHit > 1 && E.IsReady())
                E.Cast(pos.Position);

            if(GetValueMenuBool("LC.Q") && Q.IsReady())
            {
                var qMinion = minions.Where(m => orbwalker.InAutoAttackRange(m))
                    .FirstOrDefault(m => m.Health < (Q.GetDamage(m) + player.GetAutoAttackDamage(m)) * 0.93);
                if(qMinion != null)
                {
                    Q.Cast();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, qMinion);
                }
            }                
        }
        private static void AutoFarmQ()
        {
            if(GetValueMenuBool("AutoFarmQ.Enable"))
            {
                var qMinion = MinionManager.GetMinions(E.Range).Where(m => orbwalker.InAutoAttackRange(m))
                    .FirstOrDefault(m => m.Health < (Q.GetDamage(m) + player.GetAutoAttackDamage(m)) * 0.93);

                if (qMinion != null && Q.IsReady())
                {
                    Q.Cast();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, qMinion);
                }
            }
        }
        private static void KillSteal()
        {
            foreach(var enemyHero in HeroManager.Enemies)
            {
                var totalQDamage = (Q.GetDamage(enemyHero) + player.GetAutoAttackDamage(enemyHero)) * 0.9;
                var eDamage = E.GetDamage(enemyHero);

                if(enemyHero.Health < totalQDamage)
                {
                    Q.Cast();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, enemyHero);
                }

                if (E.IsReady() && E.IsInRange(enemyHero) && enemyHero.Health < eDamage)
                    E.Cast(enemyHero.Position);
            }
        }
        private static void AutoR()
        {
            if (R.IsReady() && GetValueMenuBool("R.AutoR"))
            {
                var minPercentHealthToR = GetValueMenuSlider("R.MinHealth");

                if ((int)player.HealthPercent <= minPercentHealthToR)
                    R.Cast();
            }
        }
    }
}
