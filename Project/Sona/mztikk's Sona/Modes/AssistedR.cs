using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikkSona.Extensions;

    using Config = mztikkSona.Config;

    internal static class AssistedR
    {
        #region Methods

        internal static void Execute()
        {
            if (!Spells.R.CanCast()) return;
            AIHeroClient rTarget = null;
            switch (Config.GetStringListValue("assistedRTargetting"))
            {
                case 0:
                    rTarget =
                        HeroManager.Enemies.Where(
                            x => !x.IsDead && x.Distance(ObjectManager.Player) <= Spells.R.Range)
                            .OrderBy(x => x.Distance(Game.CursorPos))
                            .FirstOrDefault();
                    break;
                case 1:
                    rTarget = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Magical);
                    break;
            }

            if (rTarget == null)
            {
                return;
            }

            Spells.R.CastOnUnit(rTarget);
        }

        #endregion
    }
}