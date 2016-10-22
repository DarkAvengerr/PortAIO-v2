using Arcane_Ryze.Handler;
using Arcane_Ryze.Main;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using System;
using System.Linq;
using static Arcane_Ryze.Core;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Arcane_Ryze.Modes
{
    class LastHit
    {
        private static AIHeroClient Player = ObjectManager.Player;
        public static void LastHitLogic()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.IsValidTarget(Player.AttackRange)).ToList();
            if (!MenuConfig.LaneQ || minions == null)
            {
                return;
            }
            if(PassiveStack < 4)
            {
                foreach (var m in minions)
                {
                    if(m.Health < Spells.Q.GetDamage(m) && m.Health > (float) Player.GetAutoAttackDamage(m) && !Player.Spellbook.IsAutoAttacking)
                    {
                        Spells.Q.Cast(m);
                    }
                    if (m.Health < Spells.W.GetDamage(m) && m.Health > (float)Player.GetAutoAttackDamage(m) && !Player.Spellbook.IsAutoAttacking)
                    {
                        Spells.W.Cast(m);
                    }
                }
            }
        }
    }
}
