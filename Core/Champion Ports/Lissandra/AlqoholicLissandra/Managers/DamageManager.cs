using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicLissandra.Managers
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using Spells;

    internal class DamageManager
    {
        internal static float GetComboDamage(Obj_AI_Base target)
        {
            var damage = (float)(ObjectManager.Player.GetAutoAttackDamage(target) * 2); // 2 Auto Attacks

            if (Spells.Q.SpellObject.IsReady())
            {
                damage += Spells.Q.SpellObject.GetDamage(target);
            }

            if (Spells.W.SpellObject.IsReady())
            {
                damage += Spells.W.SpellObject.GetDamage(target);
            }

            if (Spells.E.SpellObject.IsReady())
            {
                damage += Spells.E.SpellObject.GetDamage(target);
            }

            if (Spells.R.SpellObject.IsReady())
            {
                damage += Spells.R.SpellObject.GetDamage(target);
            }

            return damage;
        }


    }
}