#region

using System;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Menus;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event
{
    internal class KillSteal : Core.Core
    {
        public static void Update(EventArgs args)
        {
            var hero = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Physical);

            if (hero == null || hero.HasBuff("kindrednodeathbuff") || hero.HasBuff("Undying Rage") ||
               hero.HasBuff("JudicatorIntervention")) return;

            if (Spells.W.IsReady() && InWRange(hero))
            {
                if (hero.Health <= Spells.W.GetDamage(hero))
                {
                    Spells.W.Cast();
                }
            }

            if (Spells.R.IsReady() && Spells.R.Instance.Name == IsSecondR)
            {
                if (hero.Health < Dmg.RDmg(hero))
                {
                    var pred = Spells.R.GetPrediction(hero);

                    Spells.R.Cast(pred.CastPosition);
                }
            }

            if (!Spells.Ignite.IsReady() || !MenuConfig.Ignite) return;

            if (hero.IsValidTarget(600f) && Dmg.IgniteDamage(hero) >= hero.Health)
            {
                Player.Spellbook.CastSpell(Spells.Ignite, hero);
            }
        }
    }
}
