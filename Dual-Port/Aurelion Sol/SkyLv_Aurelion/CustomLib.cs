using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public static class CustomLib
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_AurelionSol.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_AurelionSol.Q;
            }
        }

        private static Spell R
        {
            get
            {
                return SkyLv_AurelionSol.R;
            }
        }
        #endregion


        public static int enemyChampionInRange(int Range)
        {

                return ObjectManager.Get<AIHeroClient>().Where(target => !target.IsMe && target.Team != ObjectManager.Player.Team && target.LSDistance(Player) <= Range).Count();
            
        }

        public static double RDamage(Obj_AI_Base target)
        {
            return Player.CalcDamage(target, Damage.DamageType.Magical,
                (float)new double[] { 200, 400, 600 }[R.Level - 1] + 0.70f * Player.TotalMagicalDamage);
        }

        public static double QDamage(Obj_AI_Base target)
        {
            return Player.CalcDamage(target, Damage.DamageType.Magical,
                (float)new double[] { 70, 110, 150, 190, 230 }[Q.Level - 1] + 0.65f * Player.TotalMagicalDamage);
        }

        public static bool isWInLongRangeMode()
        {
            if (Player.LSHasBuff("AurelionSolWActive"))
            {
                return true;
            }
            else
                return false;
        }

    }
}
