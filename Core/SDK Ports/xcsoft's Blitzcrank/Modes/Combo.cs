using System.Linq;

using LeagueSharp.SDK;
using LeagueSharp.SDK.UI;

using Settings = xcBlitzcrank.Config.Modes.Combo;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcBlitzcrank.Modes
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

            if (Settings.UseQ && Q.IsReady())
            {
                var ignorechamps = GameObjects.EnemyHeroes.Where(x => Settings.QIgnoreChamps.Menu.GetValue<MenuBool>(x.ChampionName).Value);
                var target = Variables.TargetSelector.GetTarget(Q, false, ignorechamps);
                if (target != null)
                {
                    Extensions.CastQ(target);
                }
            }

            if (Settings.UseR && R.IsReady())
            {
                var casted = false;

                if (GameObjects.EnemyHeroes.Any(x =>
                {
                    if (!x.IsValidTarget(R.Range))
                    {
                        return false;
                    }

                    if (x.HasBuff("rocketgrab2") /*instant silent*/|| x.IsKillablewithR() /*killsteal*/)
                    {
                        return true;
                    }

                    var immobileTime = x.IsImmobileUntil();
                    return immobileTime > 0 && immobileTime <= 0.35f; /*it will be longest silent*/
                }))
                {
                    casted = R.Cast();
                }

                if (!casted && GameObjects.Player.CountEnemyHeroesInRange(R.Range) >= 2) /*aoe damage*/
                {
                    R.Cast();
                }
            }
        }
    }
}
