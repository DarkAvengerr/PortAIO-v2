#region

using LeagueSharp;
using LeagueSharp.SDK;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Infected_Twitch.Core
{
    internal class Dmg : Core
    {
        public static int IgniteDmg = 50 + 20 * GameObjects.Player.Level;

        public float EDamage(Obj_AI_Base target)
        {
            if (!Spells.E.IsReady() || target == null || target.IsDead || target.IsInvulnerable) return 0;
            
            float eDmg = 0;
            
            eDmg += Spells.E.GetDamage(target);

            return eDmg;
        }

        public static int Stacks(Obj_AI_Base target)
        {
            return target.GetBuffCount("TwitchDeadlyVenom");
        }
    }
}