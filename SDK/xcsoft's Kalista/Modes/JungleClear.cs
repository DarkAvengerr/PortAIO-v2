using System.Linq;

using LeagueSharp.SDK;
using LeagueSharp.SDK.Utils;

using Settings = xcKalista.Config.Modes.JungleClear;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcKalista.Modes
{
    internal sealed class JungleClear : ModeBase
    {
        internal override bool ShouldBeExecuted()
        {
            return Config.Keys.JungleClearActive;
        }

        internal override void Execute()
        {
            if (E.IsReady() && GameObjects.Player.ManaPercent > Settings.MinMana)
            {
                if (Settings.UseESmall && !Config.Auto.AutoE.KillSmallJungle && GameObjects.JungleSmall.Any(x => x.IsKillableWithE(true)))
                {
                    if (E.Cast())
                        return;
                }

                if (Settings.UseEBig && !Config.Auto.AutoE.KillBigJungle && GameObjects.JungleLarge.Any(x => x.IsKillableWithE(true)))
                {
                    if (E.Cast())
                        return;
                }

                if (Settings.UseELegendary && !Config.Auto.AutoE.KillLegendaryJungle && GameObjects.JungleLegendary.Any(x => x.IsKillableWithE(true)))
                {
                    if (E.Cast())
                        return;
                }
            }

            if (!Variables.Orbwalker.CanMove)
            {
                return;
            }

            //if (Settings.UseQ && Q.IsReady() && GameObjects.Player.ManaPercent > Settings.MinMana)
            //{
            //    Q.Cast(GameObjects.Jungle.OrderByDescending(x => x.MaxHealth).FirstOrDefault(x => x.IsValidTarget(800)));
            //}
        }
    }
}
