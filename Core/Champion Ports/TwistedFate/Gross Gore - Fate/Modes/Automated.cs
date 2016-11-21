#region Use
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX; 
#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    using Config = GrossGoreTwistedFate.Config;

    internal static class Automated
    {
        #region Prop

        private static readonly float Qangle = 28 * (float)Math.PI / 180;

        #endregion

        #region Methods

        internal static void Execute()
        {
            AutoCcq();
            AutoKillsteal();
        }

        private static int CountHits(Vector2 position, List<Vector2> points, List<int> hitBoxes)
        {
            var result = 0;

            var startPoint = ObjectManager.Player.ServerPosition.To2D();
            var originalDirection = Spells._q.Range * (position - startPoint).Normalized();
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
                        (Spells._q.Width + hitBoxes[i]) * (Spells._q.Width + hitBoxes[i]))
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
            var originalDirection = Spells._q.Range * (unitPosition - startPoint).Normalized();

            foreach (var enemy in HeroManager.Enemies)
            {
                if (enemy.IsValidTarget() && enemy.NetworkId != unit.NetworkId)
                {
                    var pos = Spells._q.GetPrediction(enemy);
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
                    var k = (2 / 3 * (unit.BoundingRadius + Spells._q.Width));
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

            Spells._q.Cast(bestPosition.To3D(), true);
        }

        private static void AutoCcq()
        {
            if (!Spells._q.IsReadyPerfectly() || ObjectManager.Player.IsDead)
            {
                return;
            }

            foreach (var enemy in HeroManager.Enemies)
            {
                if(!enemy.IsDead)
                {
                    if(enemy.IsValidTarget(Spells._q.Range * 2))
                    {
                        var pred = Spells._q.GetPrediction(enemy);

                        if(Config.IsImmobile)
                        {
                            if(ObjectManager.Player.ManaPercent >= Config.AutoqMana)
                            {
                                if(pred.Hitchance == HitChance.Immobile)
                                {
                                    CastQ(enemy, pred.UnitPosition.To2D());
                                }
                            }
                        }else if(Config.IsDashing)
                        {
                            if(ObjectManager.Player.ManaPercent >= Config.AutoqMana)
                            {
                                if(pred.Hitchance == HitChance.Dashing)
                                {
                                    CastQ(enemy, pred.UnitPosition.To2D());
                                }
                            }
                        }
                    }
                }

                if (enemy.IsValidTarget(Spells._q.Range))
                {
                    if (ObjectManager.Player.ManaPercent >= Config.AutoqMana)
                    {
                        if(Config.IsSlowed)
                        {
                            if(enemy.MoveSpeed <= 275)
                            {
                                var qPred = Spells._q.GetPrediction(enemy);

                                if (qPred.Hitchance >= Spells._q.MinHitChance)
                                {
                                    Spells._q.Cast(qPred.CastPosition);
                                }
                            }
                        }else if(enemy.IsCharmed)
                        {
                            var qPred = Spells._q.GetPrediction(enemy);

                            if (qPred.Hitchance >= Spells._q.MinHitChance)
                            {
                                Spells._q.Cast(qPred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        private static void AutoKillsteal()
        {
            if (!Spells._q.IsReadyPerfectly() || ObjectManager.Player.IsDead)
            {
                return;
            }
            
            if(Config.CanqKS)
            {
                var qMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;

                if (ObjectManager.Player.ManaPercent >= Config.AutoqMana)
                {
                    if(ObjectManager.Player.Mana >= qMana)
                    {
                        var entKs = HeroManager.Enemies.FirstOrDefault(
                                h => !h.IsDead && h.IsValidTarget(1000)
                                && h.Health < ObjectManager.Player.GetSpellDamage(h, SpellSlot.Q));

                        if(entKs != null)
                        {
                            var canWKill = HeroManager.Enemies.FirstOrDefault(
                                    h => !h.IsDead && h.IsValidTarget()
                                    && (ObjectManager.Player.Distance(h) < Orbwalking.GetAttackRange(ObjectManager.Player) + 250)
                                    && h.Health < ObjectManager.Player.GetSpellDamage(h, SpellSlot.W));

                            if (canWKill == null)
                            {
                                if(!Spells._w.IsReadyPerfectly())
                                {
                                    var wMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;

                                    if (ObjectManager.Player.Mana < wMana)
                                    {
                                        var qPred = Spells._q.GetPrediction(entKs);

                                        if (qPred.Hitchance >= HitChance.High)
                                        {
                                            Spells._q.Cast(qPred.CastPosition);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}