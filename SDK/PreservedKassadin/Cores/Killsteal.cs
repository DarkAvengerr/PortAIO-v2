using LeagueSharp.SDK;
using System;

namespace Preserved_Kassadin.Cores
{
    class Killsteal : Core
    {
        public static void Update(EventArgs args)
        {
            if (!SafeTarget(Target)) return;

            if (Target.Health < Spells.Q.GetDamage(Target)) Spells.Q.Cast(Target);

            if (Target.Health < Spells.W.GetDamage(Target)) Spells.W.Cast(Target);

            if (Target.Health < Spells.E.GetDamage(Target))
            {
                if (Target.Distance(Player) <= Spells.R.Range + Spells.E.Range)
                {
                    Spells.R.Cast(Target.ServerPosition);
                }
                Spells.E.Cast(Target);
            }

            if (Target.Health < Spells.R.GetDamage(Target)) Spells.R.Cast(Target);


            if (Spells.Ignite.IsReady() && Target.Health < Dmg.IgniteDmg) Player.Spellbook.CastSpell(Spells.Ignite, Target);
        }
    }
}