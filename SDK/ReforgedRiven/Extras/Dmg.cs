#region

using LeagueSharp;
using LeagueSharp.SDK;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Reforged_Riven.Extras
{
    internal class Dmg : Core
    {
        public static int IgniteDmg = 50 + 20 * GameObjects.Player.Level;

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            if (enemy == null) return 0;

            float damage = 0;

            if (Spells.W.IsReady())
            {
                damage += Spells.W.GetDamage(enemy);
            }

            if (Spells.Q.IsReady())
            {
                var qcount = 4 - Qstack;
                damage += Spells.Q.GetDamage(enemy) * qcount + (float)Player.GetAutoAttackDamage(enemy) * (qcount + 1);
            }
          
            if (Spells.R.IsReady())
            {
               damage += Spells.R.GetDamage(enemy);
            }

            return damage;
        }

        public static float RDmg(AIHeroClient target)
        {
            float dmg = 0;

            if (target == null || !Spells.R.IsReady()) return 0;

            dmg += Spells.R.GetDamage(target);

            return dmg;
        }
    }
}
