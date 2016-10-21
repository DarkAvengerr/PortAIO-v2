#region

using LeagueSharp;
using LeagueSharp.SDK;
using Spirit_Karma.Core;
using Spirit_Karma.Menus;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Spirit_Karma.Event
{
    internal class Interrupt : Core.Core
    {
        public static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if(!MenuConfig.UseInterrupt || args.Sender.Distance(Player) > Spells.Q.Range || args.Sender.IsInvulnerable) return;

            if (Spells.R.IsReady() && Spells.Q.IsReady())
            {
                Spells.R.Cast();
                Spells.Q.Cast(args.Sender.ServerPosition);
            }
        }
    }
}
