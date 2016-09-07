using LeagueSharp;
using LeagueSharp.SDK;

using Settings = xcSivir.Config.Modes.JungleClear;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcSivir.Modes
{
    internal sealed class JungleClear : ModeBase
    {
        internal override bool ShouldBeExecuted()
        {
            return Config.Keys.JungleClearActive;
        }

        internal override void Execute()
        {
            //if (!Variables.Orbwalker.CanMove)
            //{
            //    return;
            //}
        }
    }
}
