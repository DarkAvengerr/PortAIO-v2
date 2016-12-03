using EloBuddy; 
using LeagueSharp.Common; 
namespace myAurelionSol.Manager.Events.Games.Mode
{
    using myCommon;
    using Spells;
    using LeagueSharp.Common;

    internal class LaneClear : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
            {
                if (Menu.GetBool("LaneClearQ") && Q.IsReady())
                {
                    var QMin = MinionManager.GetMinions(Me.Position, Q.Range);
                    var FarmLocation = Q.GetCircularFarmLocation(QMin, Q.Width);

                    if (QMin != null)
                    {
                        if (FarmLocation.MinionsHit >= 3)
                        {
                            Q.Cast(FarmLocation.Position);
                        }
                    }
                }

                if (Menu.GetBool("LaneClearW") && SpellManager.HavePassive)
                {
                    var WMin = MinionManager.GetMinions(Me.Position, W.Range);

                    if (WMin?.Count >= 2)
                    {
                        W.Cast();
                    }
                }
            }
        }
    }
}
