using LeagueSharp.SDK;

using Settings = xcSivir.Config.Modes.Combo;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcSivir.Modes
{
    internal sealed class Combo : ModeBase
    {
        internal override bool ShouldBeExecuted()
        {
            return Config.Keys.ComboActive;
        }

        internal override void Execute()
        {
            if (!Variables.Orbwalker.CanMove)
            {
                return;
            }

            if(Settings.UseQ && Q.IsReady())
            {
                Q.CastOnBestTarget(0, true);
            }
        }
    }
}
