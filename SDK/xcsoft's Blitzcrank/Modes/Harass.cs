using LeagueSharp.SDK;

using Settings = xcBlitzcrank.Config.Modes.Harass;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcBlitzcrank.Modes
{
    internal sealed class Harass : ModeBase
    {
        internal override bool ShouldBeExecuted()
        {
            return Config.Keys.HarassActive;
        }

        internal override void Execute()
        {
            if (!Variables.Orbwalker.CanMove)
            {
                return;
            }

            if (GameObjects.Player.ManaPercent < Settings.MinMana)
            {
                return;
            }

            if (Settings.UseQ && Q.IsReady())
            {
                var target = Variables.TargetSelector.GetTarget(Q, false);
                if (target != null)
                {
                    Extensions.CastQ(target);
                }
            }
        }
    }
}
