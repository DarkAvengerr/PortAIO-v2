using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
using LeagueSharp.Common; 
namespace HandicapEzreal.Damages
{
    using HandicapEzreal.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;

    //soon
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
            // todo: add lichbane and gunblade damage.

            var damage = (float)ObjectManager.Player.GetAutoAttackDamage(target);

            if (Misc.SpellQ.SpellObject.IsReady())
            {
                damage += Misc.SpellQ.SpellObject.GetDamage(target);
            }

            if (Misc.SpellW.SpellObject.IsReady())
            {
                damage += Misc.SpellW.SpellObject.GetDamage(target);
            }

            return damage;
        }
    }
}
