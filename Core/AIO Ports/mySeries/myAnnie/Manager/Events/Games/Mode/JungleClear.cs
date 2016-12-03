using EloBuddy; 
using LeagueSharp.Common; 
namespace myAnnie.Manager.Events.Games.Mode
{
    using myCommon;
    using System.Linq;
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
                        Q.CastOnUnit(mob, true);
                    }

                    if (Menu.GetBool("JungleClearW") && W.IsReady() && mob.IsValidTarget(W.Range))
                    {
                        W.Cast(mob, true);
                    }
                }
            }
        }
    }
}
