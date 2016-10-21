using System.Collections.Generic;
using System.Linq;

using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using SharpDX;

using Collision = LeagueSharp.SDK.Collision;
using Settings = xcKalista.Config.Modes.Harass;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcKalista.Modes
{
    internal sealed class Harass : ModeBase
    {
        internal override bool ShouldBeExecuted()
        {
            return Config.Keys.HarassActive;
        }

        internal override void Execute()
        {
            if (!Variables.Orbwalker.CanMove)
            {
                return;
            }

            if (Q.IsReady() && GameObjects.Player.ManaPercent > Settings.Mana)
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
