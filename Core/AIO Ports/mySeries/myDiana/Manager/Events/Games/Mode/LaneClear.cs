using EloBuddy; 
using LeagueSharp.Common; 
namespace myDiana.Manager.Events.Games.Mode
{
    using myCommon;
    using LeagueSharp.Common;

    internal class LaneClear : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearmana")) && ManaManager.SpellFarm)
            {
                var qminions = MinionManager.GetMinions(Me.Position, Q.Range);

                if (Menu.GetBool("LaneClearQ") && Q.IsReady() && qminions.Count > 0)
                {
                    var qfarm = Q.GetCircularFarmLocation(qminions);

                    if (qfarm.MinionsHit >= Menu.GetSlider("LaneClearQCount"))
                    {
                        Q.Cast(qfarm.Position, true);
                    }
                }

                var wminions = MinionManager.GetMinions(Me.Position, W.Range);

                if (Menu.GetBool("LaneClearW") && W.IsReady() && wminions.Count >= Menu.GetSlider("LaneClearWCount"))
                {
                    W.Cast(true);
                }
            }
        }
    }
}
