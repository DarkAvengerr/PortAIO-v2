using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksCassiopeia.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikksCassiopeia.Config;

    internal static class LastHit
    {
        #region Methods

        internal static void Execute()
        {
            if (ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            if (Config.IsChecked("useEInLH") && Spells.E.IsReady())
            {
                var minToE =
                    MinionManager.GetMinions(ObjectManager.Player.Position, Spells.E.Range)
                        .FirstOrDefault(m => m.Health < Spells.GetEDamage(m) && m.IsValidTarget(Spells.E.Range));
                if (minToE != null)
                {
                    if (!Config.IsChecked("lastEonP") || minToE.HasBuffOfType(BuffType.Poison))
                    {
                        Spells.E.Cast(minToE);
                    }
                }
            }
        }

        #endregion
    }
}