using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.Core
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal sealed class Dmg
    {
        public float GetDamage(Obj_AI_Base x)
        {
            if (x == null
                || x.IsInvulnerable
                || x.HasBuffOfType(BuffType.SpellShield)
                || x.HasBuffOfType(BuffType.SpellImmunity))
            {
                return 0;
            }

            float dmg = 0;

            if (!Vars.Player.Spellbook.IsAutoAttacking)
            {
                dmg += (float)Vars.Player.GetAutoAttackDamage(x);
            }

            if (Spells.Q.IsReady())
            {
                dmg += Spells.Q.GetDamage(x);
            }

            if (Spells.W2.IsReady())
            {
                dmg += Spells.W2.GetDamage(x);
            }

            if (Spells.E.IsReady())
            {
                dmg += Spells.E.GetDamage(x);
            }

            if (Spells.R2.IsReady())
            {
                dmg += Spells.R2.GetDamage(x);
            }

            return dmg;
        }
    }
}
