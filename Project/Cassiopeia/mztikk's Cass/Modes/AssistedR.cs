using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksCassiopeia.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikksCassiopeia.Config;

    internal static class AssistedR
    {
        #region Methods

        internal static void Execute()
        {
            if (!Config.IsKeyPressed("assistedR") || !Spells.R.IsReady())
            {
                return;
            }
            var target =
                HeroManager.Enemies.Where(
                    x =>
                    !x.IsDead && x.IsValid && x.Distance(ObjectManager.Player) < Spells.R.Range
                    && x.IsFacing(ObjectManager.Player)).OrderBy(x => x.Distance(Game.CursorPos)).FirstOrDefault();
            if (target == null)
            {
                return;
            }
            var rPred = Spells.R.GetPrediction(target, true);
            if (rPred.Hitchance >= HitChance.High)
            {
                Spells.R.Cast(rPred.CastPosition);
            }
        }

        #endregion
    }
}