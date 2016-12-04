using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.Core.Condemn_Logic
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal sealed class CondemnTypes
    {
        public bool Reformed(Obj_AI_Base target, float range, Spell spell)
        {
            return ReformedPosition(target, range, spell) != Vector3.Zero;
        }

        private static Vector3 ReformedPosition(Obj_AI_Base target, float range, Spell spell)
        {
            var prediction = spell.GetPrediction(target).UnitPosition;

            var finalPosition = prediction.Extend(ObjectManager.Player.ServerPosition, -range);

            if (NavMesh.GetCollisionFlags(finalPosition) == CollisionFlags.Wall
                || NavMesh.GetCollisionFlags(finalPosition) == CollisionFlags.Building
                || finalPosition.IsWall())
            {
                return finalPosition;
            }

            for (float i = 0; i < range; i += (int)target.BoundingRadius)
            {
                var newprediction = prediction.Extend(ObjectManager.Player.ServerPosition, -i);

                if (NavMesh.GetCollisionFlags(newprediction) == CollisionFlags.Wall
                || NavMesh.GetCollisionFlags(newprediction) == CollisionFlags.Building
                || newprediction.IsWall())
                {
                    return newprediction;
                }
            }

            return Vector3.Zero;
        }

        /// <summary>
        /// From Marksman
        /// xQx/legacy(?)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="spell"></param>
        /// <returns></returns>
        public bool Marksman(Obj_AI_Base target, Spell spell)
        {
            for (var i = 1; i < 8; i++)
            {
                var targetBehind = target.Position
                                   + Vector3.Normalize(target.ServerPosition - ObjectManager.Player.Position) * i * 50;

                if (targetBehind.IsWall() && target.IsValidTarget(spell.Range))
                {
                    spell.CastOnUnit(target);
                    return true;
                }
            }
            return false;
        }

        public bool SharpShooter(Obj_AI_Base hero, Spell spell, float pushDistance)
        {
            var prediction = spell.GetPrediction(hero);

            for (var i = 15; i < pushDistance; i += 100)
            {
                if (i > pushDistance) return false;

                var posCF =
                    NavMesh.GetCollisionFlags(
                        prediction.UnitPosition.To2D().Extend(ObjectManager.Player.Position.To2D(), -i).To3D());

                if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
