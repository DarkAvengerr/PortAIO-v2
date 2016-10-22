using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoElise
{
    public static class BadaoEliseCombo
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }
            if (BadaoEliseHelper.CanUseWSpider() && !BadaoEliseSpellsManager.IsHuman)
            {
                BadaoMainVariables.W2.Cast();
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                Orbwalking.Attack = true;
                return;
            }
            if (BadaoEliseHelper.CanUseQHuman() && BadaoEliseSpellsManager.IsHuman)
                Orbwalking.Attack = false;
            else
                Orbwalking.Attack = true;
            if (BadaoEliseHelper.UseE1Combo())
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.E.Range, TargetSelector.DamageType.Magical);
                if (target.BadaoIsValidTarget())
                {
                    if (BadaoEliseSpellsManager.IsHuman)
                    {
                        BadaoMainVariables.E.Cast(target);
                    }
                }
            }
            if (BadaoEliseHelper.CanUseQHuman())
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, TargetSelector.DamageType.Magical);
                if (target.BadaoIsValidTarget() && BadaoEliseSpellsManager.IsHuman)
                {
                    BadaoMainVariables.Q.Cast(target);
                }
            }
            if (BadaoEliseHelper.CanUseWHuman())
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.W.Range, TargetSelector.DamageType.Magical);
                if (target.BadaoIsValidTarget() && BadaoEliseSpellsManager.IsHuman)
                {
                    BadaoMainVariables.W.Cast(target);
                }
            }
            if (BadaoEliseHelper.CanUseQSpider())
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.Q2.Range, TargetSelector.DamageType.Magical);
                if (target.BadaoIsValidTarget() && !BadaoEliseSpellsManager.IsHuman)
                {
                    BadaoMainVariables.Q2.Cast(target);
                }
            }
            if (BadaoEliseHelper.UseE2Combo())
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.E2.Range, TargetSelector.DamageType.Magical);
                if (target.BadaoIsValidTarget() && !BadaoEliseSpellsManager.IsHuman &&
                        target.Position.To2D().Distance(ObjectManager.Player.Position.To2D()) > BadaoMainVariables.Q2.Range)
                {
                    BadaoMainVariables.E2.Cast(target);
                }
            }
            if (BadaoEliseHelper.UseRCombo())
            {
                if (BadaoEliseSpellsManager.IsHuman)
                {
                    var targetQ = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, TargetSelector.DamageType.Magical);
                    var targetW = TargetSelector.GetTarget(BadaoMainVariables.W.Range, TargetSelector.DamageType.Magical);
                    var targetE = TargetSelector.GetTarget(BadaoMainVariables.E.Range, TargetSelector.DamageType.Magical);
                    if ((!BadaoEliseHelper.CanUseQHuman() || !targetQ.BadaoIsValidTarget()) && 
                        (!BadaoEliseHelper.CanUseWHuman() || !targetW.BadaoIsValidTarget() 
                        || BadaoMainVariables.W.GetPrediction(targetW).Hitchance < HitChance.Medium) &&
                        (!BadaoEliseHelper.UseE1Combo() || !targetE.BadaoIsValidTarget() ||
                        BadaoMainVariables.E.GetPrediction(targetW).Hitchance < HitChance.Medium))
                    {
                        var target = TargetSelector.GetTarget(BadaoMainVariables.E2.Range, TargetSelector.DamageType.Magical);
                        if (BadaoEliseHelper.UseE2Combo() || (BadaoEliseHelper.CanUseQSpider()
                            && target.Position.To2D().Distance(ObjectManager.Player.Position.To2D()) <= BadaoMainVariables.Q2.Range))
                        {
                            BadaoMainVariables.R.Cast();
                        }
                    }
                }
                if (!BadaoEliseSpellsManager.IsHuman)
                {
                    var targetQ = TargetSelector.GetTarget(BadaoMainVariables.Q2.Range, TargetSelector.DamageType.Magical);
                    var targetE = TargetSelector.GetTarget(BadaoMainVariables.E2.Range, TargetSelector.DamageType.Magical);
                    if (!(BadaoEliseHelper.CanUseQSpider() && targetQ.BadaoIsValidTarget()) &&
                        !(BadaoEliseHelper.CanUseESpider() && targetE.BadaoIsValidTarget()))
                    {
                        var targetEH = TargetSelector.GetTarget(BadaoMainVariables.E.Range, TargetSelector.DamageType.Magical);
                        var targetQH = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, TargetSelector.DamageType.Magical);
                        var targetWH = TargetSelector.GetTarget(BadaoMainVariables.W.Range, TargetSelector.DamageType.Magical);
                        if ((
                            targetEH.BadaoIsValidTarget() && 
                            BadaoMainVariables.E.GetPrediction(targetEH).Hitchance >= HitChance.Medium &&
                            BadaoEliseHelper.UseE1Combo()) ||
                            (targetQH.BadaoIsValidTarget() &&
                            BadaoMainVariables.Q.IsInRange(targetQH) &&
                            BadaoEliseHelper.CanUseQHuman()) || 
                            (targetWH.BadaoIsValidTarget() &&
                            BadaoEliseHelper.CanUseWHuman() &&
                            BadaoMainVariables.W.IsInRange(targetWH) &&
                            BadaoMainVariables.W.GetPrediction(targetWH).Hitchance >= HitChance.Medium))
                        {
                            BadaoMainVariables.R.Cast();
                        }
                    }
                }
            }
        }
    }
}
