using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events.Games.Mode
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
                    var ebuffMinions = minions.Where(x => x.HasBuff("ryzee"));

                    if (ebuffMinions.Any())
                    {
                        foreach (
                            var min in ebuffMinions.Where(x => MinionManager.GetMinions(x.Position, 300).Count >= 2))
                        {
                            if (Menu.GetBool("LaneClearQ") && Q.IsReady())
                            {
                                Q.Cast(min, true);
                            }

                            if (Menu.GetBool("LaneClearW") && W.IsReady() && min.Health < W.GetDamage(min))
                            {
                                W.CastOnUnit(min, true);
                            }

                            if (Menu.GetBool("LaneClearE") && E.IsReady())
                            {
                                E.CastOnUnit(min, true);
                            }
                        }
                    }
                    else
                    {
                        if (Menu.GetBool("LaneClearQ") && Q.IsReady())
                        {
                            var qMin = minions.FirstOrDefault(x => x.Health < Q.GetDamage(x));

                            if (qMin != null)
                            {
                                Q.Cast(qMin, true);
                            }
                        }

                        if (Menu.GetBool("LaneClearW") && W.IsReady())
                        {
                            var wMin = minions.FirstOrDefault(x => x.IsValidTarget(W.Range) && x.Health < W.GetDamage(x));

                            if (wMin != null)
                            {
                                W.CastOnUnit(wMin, true);
                            }
                        }

                        if (Menu.GetBool("LaneClearE") && E.IsReady())
                        {
                            foreach (
                                var eMin in
                                minions.Where(x => MinionManager.GetMinions(x.Position, 300).Count >= 2)
                                    .OrderByDescending(x => MinionManager.GetMinions(x.Position, 300).Count))
                            {
                                if (eMin.IsValidTarget(E.Range))
                                {
                                    E.CastOnUnit(eMin, true);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
