#region

using System;
using System.Linq;
using Infected_Twitch.Core;
using Infected_Twitch.Menus;
using LeagueSharp.SDK;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Infected_Twitch.Event
{
    internal class EOnDeath
    {
        public static void Update(EventArgs args)
        {
            if(!MenuConfig.EBeforeDeath || !Spells.E.IsReady()) return;

            var target = GameObjects.EnemyHeroes.FirstOrDefault(x => x.IsValidTarget(Spells.E.Range) && Dmg.Stacks(x) > 0 && !x.IsDead && !x.IsInvulnerable);

            if(target == null) return;

            if (GameObjects.Player.HealthPercent <= 10)
            {
                Spells.E.Cast();
            }
        }
    }
}
