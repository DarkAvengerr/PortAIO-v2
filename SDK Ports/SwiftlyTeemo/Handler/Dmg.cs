#region

using LeagueSharp;
using LeagueSharp.SDK;
using Swiftly_Teemo.Menu;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Swiftly_Teemo.Handler
{
    internal class Dmg : Core
    {
        public static int IgniteDmg = 50 + 20 * GameObjects.Player.Level;
        public static float ComboDmg(Obj_AI_Base enemy)
        {
            if (enemy == null) return 0;

            float dmg = 0;

            if (MenuConfig.KillStealSummoner)
            {
                if (Spells.Ignite.IsReady())
                {
                    dmg = dmg + IgniteDmg;
                }
            }

            dmg += (float)Player.GetAutoAttackDamage(enemy);

            if (Spells.E.IsReady()) dmg += Spells.E.GetDamage(enemy);

            if (Spells.R.IsReady()) dmg += Spells.R.GetDamage(enemy);

            if (Spells.Q.IsReady()) dmg += Spells.Q.GetDamage(enemy);

            return dmg;
        }
        public static bool IsLethal(Obj_AI_Base unit)
        {
            return ComboDmg(unit) / 1.65 >= unit.Health;
        }
    }
}
