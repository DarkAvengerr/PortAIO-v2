using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.Core.Condemn_Logic
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

   internal sealed class SharpshooterCondemn : ICondemnType
    {
        public Vector3 Execute(Obj_AI_Base target, float range, Spell spell)
        {
            var prediction = spell.GetPrediction(target);

            for (var i = 15; i < range; i += 100)
            {
                if (i > range) return Vector3.Zero;

                var pred = prediction.UnitPosition.To2D().Extend(ObjectManager.Player.Position.To2D(), -i);

                var posCF = pred.To3D();

                if (NavMesh.GetCollisionFlags(posCF) == CollisionFlags.Wall || NavMesh.GetCollisionFlags(posCF) == CollisionFlags.Building)
                {
                    return posCF;
                }
            }
            return Vector3.Zero;
        }
    }
}
