using System.Collections.Generic;
using System.Linq;

using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;

using SharpDX;

using Collision = LeagueSharp.SDK.Collision;
using Settings = xcKalista.Config.Modes.Combo;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcKalista.Modes
{
    internal sealed class Combo : ModeBase
    {
        internal override bool ShouldBeExecuted()
        {
            return Config.Keys.ComboActive;
        }

        internal override void Execute()
        {
            if (Settings.UseE && !Config.Auto.AutoE.KillEnemyHeros && E.IsReady() && GameObjects.EnemyHeroes.Any(x => x.IsKillableWithE(true)))
            {
                E.Cast();
            }

            if (Settings.AttackMinion && Variables.Orbwalker.CanAttack && !GameObjects.EnemyHeroes.Any(x => x.InAutoAttackRange()))
            {
                var units = new List<Obj_AI_Minion>();
                units.AddRange(GameObjects.EnemyMinions);
                units.AddRange(GameObjects.Jungle);
                var bestUnit = units.Where(x => x.IsValidTarget() && x.InAutoAttackRange()).OrderByDescending(x => x.Distance(GameObjects.Player)).FirstOrDefault();
                if (bestUnit != null)
                {
                    Variables.Orbwalker.Attack(bestUnit);
                }
            }

            if (!Variables.Orbwalker.CanMove)
            {
                return;
            }

            if (Q.IsReady() && Settings.KeepManaForE ? GameObjects.Player.Mana - Q.Instance.SData.Mana >= 30 : true)
            {
                var caststate = CastStates.NotCasted;

                if (Settings.UseQ)
                {
                    caststate = Q.CastOnBestTarget();
                }

                if (Settings.UseQPierce && caststate == CastStates.NotCasted)
                {
                    var target = Variables.TargetSelector.GetTarget(Q);
                    if (target != null)
                    {
                        var prediction = Movement.GetPrediction(new PredictionInput
                        {
                            Unit = target, Delay = Q.Delay, Radius = Q.Width, Speed = Q.Speed, Range = Q.Range
                        });
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            var collision = Collision.GetCollision(new List<Vector3>
                            {
                                prediction.UnitPosition
                            }, SpellManager.QCollisionInput);
                            collision.Remove(GameObjects.Player);
                            collision.Remove(target);
                            if (collision.All(x => x.IsKillableWithQ()))
                            {
                                Q.Cast(prediction.CastPosition);
                            }
                        }
                    }
                }
            }
        }
    }
}
