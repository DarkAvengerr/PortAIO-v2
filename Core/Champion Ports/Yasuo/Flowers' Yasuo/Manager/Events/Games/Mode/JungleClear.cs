using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Manager.Events.Games.Mode
{
    using Spells;
    using System.Linq;
    using LeagueSharp.Common;

    internal class JungleClear : Logic
    {
        internal static void Init()
        {
            var mobs = MinionManager.GetMinions(Me.ServerPosition, Q3.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (mobs.Any())
            {
                var mob = mobs.FirstOrDefault(x => x.IsValidTarget(E.Range) && SpellManager.CanCastE(x));

                if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady() 
                    && mob != null && mob.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(mob, true);
                }

                if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady() && !SpellManager.HaveQ3)
                {
                    var qFarm = MinionManager.GetBestLineFarmLocation(mobs.Select(x => x.Position.To2D()).ToList(),
                        Q.Width, Q.Range);

                    if (qFarm.MinionsHit >= 1)
                    {
                        Q.Cast(qFarm.Position, true);
                    }
                }

                if (Menu.Item("JungleClearQ3", true).GetValue<bool>() && Q3.IsReady() && SpellManager.HaveQ3)
                {
                    var q3Farm = MinionManager.GetBestLineFarmLocation(mobs.Select(x => x.Position.To2D()).ToList(),
                        Q3.Width, Q3.Range);

                    if (q3Farm.MinionsHit >= 1)
                    {
                        Q3.Cast(q3Farm.Position, true);
                    }
                }
            }
        }
    }
}
