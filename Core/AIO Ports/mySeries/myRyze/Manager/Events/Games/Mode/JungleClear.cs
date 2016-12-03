using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events.Games.Mode
{
    using System.Linq;
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class JungleClear : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu.GetBool("JungleClearQ") && Q.IsReady() && mob.IsValidTarget(Q.Range))
                    {
                        Q.Cast(mob, true);
                    }

                    if (Menu.GetBool("JungleClearE") && E.IsReady() && mob.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(mob, true);
                    }

                    if (Menu.GetBool("JungleClearW") && W.IsReady() && mob.IsValidTarget(W.Range))
                    {
                        W.CastOnUnit(mob, true);
                    }
                }
            }
        }
    }
}
