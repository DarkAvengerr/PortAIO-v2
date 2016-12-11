using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.Core.Condemn_Logic
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

   internal sealed class MarksmanCondemn : ICondemnType
    {
        public Vector3 Execute(Obj_AI_Base target, float range, Spell spell)
        {
            for (var i = 1; i < 8; i++)
            {
                var targetBehind = target.Position
                                   + Vector3.Normalize(target.ServerPosition - ObjectManager.Player.Position) * i * 50;

                if (targetBehind.IsWall() && target.IsValidTarget(spell.Range))
                {
                    spell.CastOnUnit(target);
                    return targetBehind;
                }
            }
            return Vector3.Zero;
        }
    }
}
