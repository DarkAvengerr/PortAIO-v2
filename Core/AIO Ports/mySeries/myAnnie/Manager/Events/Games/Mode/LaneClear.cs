using EloBuddy; 
using LeagueSharp.Common; 
namespace myAnnie.Manager.Events.Games.Mode
{
    using myCommon;
    using System.Linq;
    using LeagueSharp.Common;

    internal class LaneClear : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
            {
                var minions = MinionManager.GetMinions(Me.Position, Q.Range);

                if (minions.Any())
                {
                    if (Menu.GetBool("LaneClearQ") && Q.IsReady())
                    {
                        if (Menu.GetBool("LaneClearQLH"))
                        {
                            var qMin = minions.FirstOrDefault(x => x.Health < Q.GetDamage(x));

                            if (qMin != null)
                            {
                                Q.CastOnUnit(qMin, true);
                            }
                        }
                        else
                        {
                            Q.CastOnUnit(minions.FirstOrDefault(), true);
                        }
                    }

                    if (Menu.GetBool("LaneClearW") && W.IsReady())
                    {
                        var wFarm =
                            MinionManager.GetBestCircularFarmLocation(minions.Select(x => x.Position.To2D()).ToList(),
                                W.Width, W.Range);

                        if (wFarm.MinionsHit >= Menu.GetSlider("LaneClearWCount"))
                        {
                            W.Cast(wFarm.Position, true);
                        }
                    }
                }
            }
        }
    }
}
