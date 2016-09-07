using LeagueSharp.SDK;

using Settings = xcSivir.Config.Modes.LaneClear;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcSivir.Modes
{
    internal sealed class LaneClear : ModeBase
    {
        internal override bool ShouldBeExecuted()
        {
            return Config.Keys.LaneClearActive;
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
