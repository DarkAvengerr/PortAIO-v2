using Arcane_Ryze.Main;
using LeagueSharp;
using LeagueSharp.SDK;
using System.Linq;
using static Arcane_Ryze.Core;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Arcane_Ryze.Modes
{
    class Jungle
    {
        private static AIHeroClient Player = ObjectManager.Player;
        public static void JungleLogic()
        {
            var mob = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && m.IsValidTarget(Spells.Q.Range)).ToList();
            if(mob == null || Player.Spellbook.IsAutoAttacking)
            {
                return;
            }
            foreach(var m in mob)
            {
                if(m.IsValidTarget() && !m.IsZombie && !m.IsDead)
                {
                    if (Spells.R.IsReady() && Spells.Q.IsReady() && Player.ManaPercent > 20 && Spells.Q.IsReady() && Spells.E.IsReady() && m.HealthPercent >= 30 && MenuConfig.JungleR) // This is my temporary logic for not casting R at small jungle camps
                    {
                        Spells.R.Cast();
                    }
                    if (Spells.Q.IsReady())
                    {
                        Spells.Q.Cast(m);
                    }
                    if (Spells.E.IsReady())
                    {
                        Spells.E.Cast(m);
                    }
                    if (Spells.W.IsReady())
                    {
                        Spells.W.Cast(m);
                    }
                }
            }
        }
    }
}
