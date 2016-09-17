using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon.Modes
{
    using System.Linq;

    using LeagueSharp.Common;

    internal static class Killsteal
    {
        #region Methods

        internal static void Execute()
        {
            if (!Spells.Q.IsReady())
            {
                return;
            }

            var target =
                HeroManager.Enemies.Where(
                    x => !x.IsDead && x.IsValid && x.IsValidTarget(Spells.Q.Range) && x.Health < Spells.Q.GetDamage(x))
                    .OrderBy(TargetSelector.GetPriority)
                    .FirstOrDefault();
            if (target == null)
            {
                return;
            }

            Spells.Q.Cast(target);
        }

        #endregion
    }
}