using EloBuddy; 
using LeagueSharp.Common; 
namespace myAurelionSol.Manager.Events.Games.Mode
{
    using myCommon;
    using Spells;
    using LeagueSharp.Common;

    internal class JungleClear : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                if (Q.IsReady() && Menu.GetBool("JungleClearQ"))
                {
                    var QMob = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (QMob != null)
                    {
                        foreach (var mob in QMob)
                        {
                            if (mob.IsValidTarget(Q.Range))
                                Q.Cast(mob);
                        }
                    }
                }

                if (SpellManager.HavePassive && Menu.GetBool("JungleClearW"))
                {
                    var WMob = MinionManager.GetMinions(Me.Position, W.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (WMob != null)
                    {
                        foreach (var mob in WMob)
                        {
                            if (mob.IsValidTarget(W.Range) && !mob.IsValidTarget(420) && !SpellManager.IsWActive)
                            {
                                W.Cast();
                            }
                            else if (SpellManager.IsWActive && mob.IsValidTarget(420))
                            {
                                W.Cast();
                            }
                        }
                    }
                }
            }
        }
    }
}
