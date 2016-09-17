using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksCassiopeia.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikksCassiopeia.Config;

    internal static class LaneClear
    {
        #region Methods

        internal static void Execute()
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range + 500);
            var objAiMinions = minions;
            if (!objAiMinions.Any() || minions == null
                || ObjectManager.Player.ManaPercent < Config.GetSliderValue("manaToLC"))
            {
                return;
            }

            if (Config.IsChecked("useQInLC") && Spells.Q.IsReady() && (!ObjectManager.Player.Spellbook.IsAutoAttacking))
            {
                var qFarmLoc = MinionManager.GetBestCircularFarmLocation(
                    minions.Select(x => x.Position.To2D()).ToList(), 
                    Spells.Q.Width, 
                    Spells.Q.Range);
                if (qFarmLoc.MinionsHit >= Config.GetSliderValue("minQInLC"))
                {
                    Spells.Q.Cast(qFarmLoc.Position.To3D());
                }
            }

            if (Config.IsChecked("useWInLC") && Spells.W.IsReady() && (!ObjectManager.Player.Spellbook.IsAutoAttacking))
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

            if (Config.IsChecked("useEInLC") && Spells.E.IsReady())
            {
                var minToE =
                    MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range)
                        .FirstOrDefault(
                            m =>
                            m.IsValidTarget(Spells.E.Range) && Spells.GetEDamage(m) > m.Health
                            && ((!Config.IsChecked("laneEonP") || m.HasBuffOfType(BuffType.Poison))
                                || (Config.IsChecked("useManaEInLC")
                                    && ObjectManager.Player.ManaPercent <= Config.GetSliderValue("manaEInLC"))));
                if (minToE != null)
                {
                    Spells.E.Cast(minToE);
                }
            }
        }

        #endregion
    }
}