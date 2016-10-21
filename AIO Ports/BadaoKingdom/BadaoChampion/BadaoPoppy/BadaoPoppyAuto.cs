using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoPoppy
{
    public static class BadaoPoppyAuto
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (BadaoPoppyHelper.UseEAutoInterrupt())
            {
                if (sender.BadaoIsValidTarget(BadaoMainVariables.E.Range) && sender.IsEnemy)
                    BadaoMainVariables.E.Cast(sender);
            }
            if (BadaoPoppyHelper.UseRAutoInterrupt() && ! BadaoPoppyHelper.UseEAutoInterrupt())
            {
                if (sender.BadaoIsValidTarget(500) && sender.IsEnemy)
                {
                    if (!BadaoMainVariables.R.IsCharging)
                    {
                        BadaoMainVariables.R.StartCharging();
                    }
                    else
                    {
                        BadaoMainVariables.R.Cast(sender.Position);
                    }
                }
            }
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (sender.BadaoIsValidTarget() && sender.IsEnemy && sender is AIHeroClient
                && BadaoPoppyHelper.UseWAutoAntiDash(sender as AIHeroClient))
            {
                if (Geometry.Distance(ObjectManager.Player.Position.To2D(),args.StartPos, args.EndPos,true)
                    <= BadaoMainVariables.W.Range + sender.BoundingRadius
                    || args.StartPos.Distance(ObjectManager.Player.Position.To2D()) <= BadaoMainVariables.W.Range
                    || args.EndPos.Distance(ObjectManager.Player.Position.To2D()) <= BadaoMainVariables.W.Range)
                {
                    BadaoMainVariables.W.Cast();
                }

            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            foreach (var hero in HeroManager.Enemies.Where(x => x.BadaoIsValidTarget()))
            {
                if (hero.IsDashing() && BadaoPoppyHelper.UseWAutoAntiDash(hero))
                {
                    var dashInfo = hero.GetDashInfo();
                    if (Geometry.Distance(ObjectManager.Player.Position.To2D(), dashInfo.StartPos, dashInfo.EndPos, true)
                        <= BadaoMainVariables.W.Range + hero.BoundingRadius
                        || dashInfo.StartPos.Distance(ObjectManager.Player.Position.To2D()) <= BadaoMainVariables.W.Range
                        || dashInfo.EndPos.Distance(ObjectManager.Player.Position.To2D()) <= BadaoMainVariables.W.Range)
                    {
                        if (BadaoMainVariables.W.Cast())
                            break;
                    }
                }
            }
            if (BadaoPoppyHelper.UseRAutoKS())
            {
                if (!BadaoMainVariables.R.IsCharging)
                {
                    var killableTarget = HeroManager.Enemies.FirstOrDefault(x => x.BadaoIsValidTarget(500)
                        && BadaoMainVariables.R.GetDamage(x) >= x.Health);
                    if (killableTarget != null)
                        BadaoMainVariables.R.StartCharging();
                }
                else
                {
                    var killableTarget = HeroManager.Enemies.FirstOrDefault(x => x.BadaoIsValidTarget(500)
                        && BadaoMainVariables.R.GetDamage(x) >= x.Health);
                    if (killableTarget != null)
                        BadaoMainVariables.R.Cast(killableTarget.Position);
                }
            }
            if (BadaoPoppyHelper.UseRAuto3Target())
            {
                if (!BadaoMainVariables.R.IsCharging)
                {
                    var knockup = HeroManager.Enemies.FirstOrDefault(x => x.BadaoIsValidTarget(500)
                        && x.CountEnemiesInRange(300) >= 3);
                    if (knockup != null)
                        BadaoMainVariables.R.StartCharging();
                }
                else
                {
                    var knockup = HeroManager.Enemies.FirstOrDefault(x => x.BadaoIsValidTarget(500)
                        && x.CountEnemiesInRange(300) >= 3);
                    if (knockup != null)
                        BadaoMainVariables.R.Cast(knockup.Position);
                    else
                    {
                        var target = TargetSelector.GetTarget(BadaoMainVariables.R.Range, TargetSelector.DamageType.Physical);
                        if (target.BadaoIsValidTarget())
                            BadaoMainVariables.R.Cast(target);
                    }
                }
            }
        }
    }
}
