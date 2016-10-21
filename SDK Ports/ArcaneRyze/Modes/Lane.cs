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
    class Lane
    {
        private static AIHeroClient Player = ObjectManager.Player;
        public static void LaneLogic()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.IsValidTarget(Player.AttackRange)).ToList();
            if (PassiveStack > 4)
            {
                return;
            }
            if (!(Player.ManaPercent >= MenuConfig.LaneMana.Value)) return;
            {
                foreach (var m in minions)
                {
                    if (!m.IsValidTarget() || m.IsZombie || m.IsDead) continue;
                    if (m.Health < Spells.Q.GetDamage(m) && !Player.Spellbook.IsAutoAttacking)
                    {
                        Spells.Q.Cast(m);
                    }
                    if (Spells.Q.IsReady() && m.Health > Spells.Q.GetDamage(m) && Player.GetAutoAttackDamage(m) > m.Health)
                    {
                        Spells.Q.Cast(m);
                    }
                    if (Spells.E.IsReady())
                    {
                        Spells.E.Cast(m);
                    }
                    if (Spells.W.IsReady() && m.Health < Spells.W.GetDamage(m))
                    {
                        Spells.W.Cast(m);
                    }
                    if (Spells.R.IsReady() && MenuConfig.LaneR)
                    {
                        Spells.R.Cast();
                    }
                }
            }
        }
    }
}
