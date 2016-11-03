using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event.Misc
{
    #region

    using System;

    using LeagueSharp.Common;

    using NechritoRiven.Core;

    #endregion

    internal class KillSteal : Core
    {
        #region Public Methods and Operators

        public static void Update(EventArgs args)
        {
            var target = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Physical);

            if (target == null || target.HasBuff(BackgroundData.InvulnerableList.ToString()))
            {
                return;
            }

            if (Spells.W.IsReady()
                && MenuConfig.KsW 
                && target.Health <= Spells.W.GetDamage(target))
            {
                BackgroundData.CastW(target);
            }

            if (Spells.R.IsReady()
                && Spells.R.Instance.Name == IsSecondR
                && MenuConfig.KsR2)
            {
                if (target.Health < Dmg.RDmg(target))
                {
                    var pred = Spells.R.GetPrediction(target);

                    Spells.R.Cast(pred.CastPosition);
                }
            }

            if (target.Health < Spells.Q.GetDamage(target) && Spells.Q.IsReady() && Qstack != 3)
            {
                Spells.Q.Cast(target.Position);
            }

            if (!Spells.Ignite.IsReady() || !MenuConfig.Ignite)
            {
                return;
            }

            if (target.IsValidTarget(600f) && Dmg.IgniteDamage(target) >= target.Health)
            {
                Player.Spellbook.CastSpell(Spells.Ignite, target);
            }
        }

        #endregion
    }
}