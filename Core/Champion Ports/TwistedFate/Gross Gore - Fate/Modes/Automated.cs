using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Windows.Input;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = GrossGoreTwistedFate.Config;
    using SharpDX;

    internal static class Automated
    {
        #region Prop

        private static readonly float Qangle = 28 * (float)Math.PI / 180;

        #endregion

        #region Methods

        internal static void Execute()
        {
            AutoCcq();

            var qMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;

            var wMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;

            if (Config.IsChecked("qKS") && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("qAMana") && ObjectManager.Player.Mana >= qMana && Spells.Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);

                var canWKill =
                    HeroManager.Enemies.FirstOrDefault(
                    h =>
                    !h.IsDead && h.IsValidTarget()
                    && (ObjectManager.Player.Distance(h) < Orbwalking.GetAttackRange(ObjectManager.Player) + 250)
                    && h.Health < ObjectManager.Player.GetSpellDamage(h, SpellSlot.W));

                var entKs =
                    HeroManager.Enemies.FirstOrDefault(
                        h =>
                        !h.IsDead && h.IsValidTarget(1000)
                        && h.Health < ObjectManager.Player.GetSpellDamage(h, SpellSlot.Q));

                if (entKs != null
                    && (canWKill == null
                        && ObjectManager.Player.Mana < wMana
                        && !Spells.W.IsReady()))
                {
                    var qPred = Spells.Q.GetPrediction(entKs);

                    if (qPred.Hitchance >= HitChance.High)
                    {
                        Spells.Q.Cast(qPred.CastPosition);
                    }
                }
            }
        }

        private static int CountHits(Vector2 position, List<Vector2> points, List<int> hitBoxes)
        {
            var result = 0;

            var startPoint = ObjectManager.Player.ServerPosition.To2D();
            var originalDirection = Spells.Q.Range * (position - startPoint).Normalized();
            var originalEndPoint = startPoint + originalDirection;

            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];

                for (var k = 0; k < 3; k++)
                {
                    var endPoint = new Vector2();
                    if (k == 0) endPoint = originalEndPoint;
                    if (k == 1) endPoint = startPoint + originalDirection.Rotated(Qangle);
                    if (k == 2) endPoint = startPoint + originalDirection.Rotated(-Qangle);

                    if (point.Distance(startPoint, endPoint, true, true) <
                        (Spells.Q.Width + hitBoxes[i]) * (Spells.Q.Width + hitBoxes[i]))
                    {
                        result++;
                        break;
                    }
                }
            }

            return result;
        }

        private static void CastQ(Obj_AI_Base unit, Vector2 unitPosition, int minTargets = 0)
        {
            var points = new List<Vector2>();
            var hitBoxes = new List<int>();

            var startPoint = ObjectManager.Player.ServerPosition.To2D();
            var originalDirection = Spells.Q.Range * (unitPosition - startPoint).Normalized();

            foreach (var enemy in HeroManager.Enemies)
            {
                if (enemy.IsValidTarget() && enemy.NetworkId != unit.NetworkId)
                {
                    var pos = Spells.Q.GetPrediction(enemy);
                    if (pos.Hitchance >= HitChance.Medium)
                    {
                        points.Add(pos.UnitPosition.To2D());
                        hitBoxes.Add((int)enemy.BoundingRadius);
                    }
                }
            }

            var posiblePositions = new List<Vector2>();

            for (var i = 0; i < 3; i++)
            {
                if (i == 0) posiblePositions.Add(unitPosition + originalDirection.Rotated(0));
                if (i == 1) posiblePositions.Add(startPoint + originalDirection.Rotated(Qangle));
                if (i == 2) posiblePositions.Add(startPoint + originalDirection.Rotated(-Qangle));
            }


            if (startPoint.Distance(unitPosition) < 900)
            {
                for (var i = 0; i < 3; i++)
                {
                    var pos = posiblePositions[i];
                    var direction = (pos - startPoint).Normalized().Perpendicular();
                    var k = (2 / 3 * (unit.BoundingRadius + Spells.Q.Width));
                    posiblePositions.Add(startPoint - k * direction);
                    posiblePositions.Add(startPoint + k * direction);
                }
            }

            var bestPosition = new Vector2();
            var bestHit = -1;

            foreach (var position in posiblePositions)
            {
                var hits = CountHits(position, points, hitBoxes);
                if (hits > bestHit)
                {
                    bestPosition = position;
                    bestHit = hits;
                }
            }

            if (bestHit + 1 <= minTargets)
                return;

            Spells.Q.Cast(bestPosition.To3D(), true);
        }

        private static void AutoCcq()
        {
            if (!Spells.Q.IsReady())
            {
                return;
            }

            foreach (var enemy in HeroManager.Enemies)
            {
                if (!enemy.IsDead && enemy.IsValidTarget(Spells.Q.Range * 2))
                {
                    var pred = Spells.Q.GetPrediction(enemy);

                    if ((Config.IsChecked("qImmobile") && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("qAMana") && pred.Hitchance == HitChance.Immobile) || (Config.IsChecked("qDashing") && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("qAMana") && pred.Hitchance == HitChance.Dashing))
                    {
                        CastQ(enemy, pred.UnitPosition.To2D());
                    }
                }
            }

            var qTarget = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);

            if (qTarget.IsValidTarget(Spells.Q.Range) && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("qAMana") && ((Config.IsChecked("qSlowed") && qTarget.MoveSpeed <= 275)
                || qTarget.IsCharmed))
            {
                var qPred = Spells.Q.GetPrediction(qTarget);

                if (qPred.Hitchance >= HitChance.VeryHigh)
                {
                    Spells.Q.Cast(qPred.CastPosition);
                }
            }
        }

        #endregion
    }
}