using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksCassiopeia.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikksCassiopeia.Config;

    internal static class JungleClear
    {
        #region Methods

        internal static void Execute()
        {
            var minions =
                MinionManager.GetMinions(
                    ObjectManager.Player.Position, 
                    Spells.Q.Range + 200, 
                    MinionTypes.All, 
                    MinionTeam.Neutral).OrderByDescending(x => x.MaxHealth);
            if (!minions.Any() || ObjectManager.Player.ManaPercent < Config.GetSliderValue("manaToJC"))
            {
                return;
            }

            if (Config.IsChecked("useQInJC") && Spells.Q.IsReady())
            {
                var qFarmLoc = MinionManager.GetBestCircularFarmLocation(
                    minions.Select(x => x.Position.To2D()).ToList(), 
                    Spells.Q.Width, 
                    Spells.Q.Range);
                if (qFarmLoc.MinionsHit > 0)
                {
                    Spells.Q.Cast(qFarmLoc.Position.To3D());
                }
            }

            if (Config.IsChecked("useWInJC") && Spells.W.IsReady())
            {
                var wFarmLoc = MinionManager.GetBestCircularFarmLocation(
                    minions.Select(x => x.Position.To2D()).ToList(), 
                    Spells.W.Width, 
                    Spells.W.Range);
                if (wFarmLoc.MinionsHit >= Config.GetSliderValue("minWInLC")
                    && wFarmLoc.Position.To3D().Distance(ObjectManager.Player.Position) >= Spells.WMinRange)
                {
                    Spells.W.Cast(wFarmLoc.Position.To3D());
                }
            }

            if (Config.IsChecked("useEInJC") && Spells.E.IsReady())
            {
                var jngToE =
                    MinionManager.GetMinions(
                        ObjectManager.Player.Position, 
                        Spells.E.Range, 
                        MinionTypes.All, 
                        MinionTeam.Neutral)
                        .OrderByDescending(x => x.MaxHealth)
                        .FirstOrDefault(
                            x =>
                            x.IsValidTarget(Spells.E.Range)
                            && (!Config.IsChecked("jungEonP") || x.HasBuffOfType(BuffType.Poison)));
                if (jngToE != null)
                {
                    Spells.E.Cast(jngToE);
                }
            }
        }

        #endregion
    }
}