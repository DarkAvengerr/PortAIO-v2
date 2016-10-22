using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Evelynn
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
                return SkyLv_Evelynn.Player;
            }
        }
        #endregion


        public static float enemyChampionInPlayerRange(float Range)
        {

            return ObjectManager.Get<AIHeroClient>().Where(target => !target.IsMe && target.Team != ObjectManager.Player.Team && target.Distance(Player) <= Range).Count();

        }
    }
}
