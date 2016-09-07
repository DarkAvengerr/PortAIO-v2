using System;
using LeagueSharp;
using LeagueSharp.SDK;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Preserved_Kassadin.Cores
{
    class Dmg : Core
    {
        public static int IgniteDmg = 50 + 20 * Player.Level;

        public static float Damage(Obj_AI_Base target)
        {
            if (!SafeTarget(target)) return 0;

            float Dmg = 0;

            if (Player.Spellbook.CanUseSpell(Spells.Ignite) == SpellState.Ready) Dmg = Dmg + IgniteDmg;

            if (!Player.Spellbook.IsAutoAttacking) Dmg = Dmg + (float)Player.GetAutoAttackDamage(target);

            if (Spells.Q.IsReady()) Dmg = Dmg + Spells.Q.GetDamage(target);

            if (Spells.W.IsReady()) Dmg = Dmg + Spells.W.GetDamage(target);

            if (Spells.E.IsReady()) Dmg = Dmg + Spells.E.GetDamage(target);

            if (Spells.R.IsReady()) Dmg = Dmg + Spells.R.GetDamage(target);

            return Dmg;
        }
    }
}
