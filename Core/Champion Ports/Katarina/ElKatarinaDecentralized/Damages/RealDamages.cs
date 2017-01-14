using EloBuddy; 
using LeagueSharp.Common; 
namespace ElKatarinaDecentralized.Damages
{
    using ElKatarinaDecentralized.Components;
    using ElKatarinaDecentralized.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class RealDamages
    {
        /// <summary>
        ///     Gets the total damage.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>
        ///     Total calculated damage.
        /// </returns>
        internal static float GetTotalDamage(Obj_AI_Base target)
        {
            var damage = (float)ObjectManager.Player.GetAutoAttackDamage(target);

            if (Misc.SpellQ.SpellObject.IsReady())
            {
                damage += Misc.SpellQ.SpellObject.GetDamage(target);
            }

            if (Misc.SpellE.SpellObject.IsReady())
            {
                damage += Misc.SpellE.SpellObject.GetDamage(target);
            }

            if (Misc.SpellR.SpellObject.IsReady())
            {
                damage += Misc.SpellR.SpellObject.GetDamage(target) * 15; // max daggers
            }

            return damage;
        }

    }
}
