using EloBuddy; 
using LeagueSharp.Common; 
namespace myViktor.Manager.Events.Games.Mode
{
    using Spells;
    using myCommon;
    using System.Linq;
    using LeagueSharp.Common;

    internal class JungleClear : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                {
                    var qMob =
                        MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                            MinionOrderTypes.MaxHealth).FirstOrDefault();

                    if (qMob != null && qMob.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(qMob, true);
                    }
                }

                if (Menu.GetBool("JungleClearE") && E.IsReady())
                {
                    var eMobs = MinionManager.GetMinions(Me.Position, E.Range + EWidth, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (eMobs.Any())
                    {
                        var mobs = eMobs.FirstOrDefault();

                        if (mobs != null)
                        {
                            SpellManager.FixECast(
                                mobs.DistanceToPlayer() > E.Range
                                    ? mobs.Position
                                    : Me.Position.Extend(mobs.Position, 150), mobs.Position);
                        }
                    }
                }
            }
        }
    }
}
