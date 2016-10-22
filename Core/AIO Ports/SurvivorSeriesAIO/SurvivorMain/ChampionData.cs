using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.SurvivorMain
{
    internal class ChampionData
    {
        public AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public float AARange()
        {
            float range = 0;
            switch (Player.ChampionName)
            {
                case "Ashe":
                    range = 600;
                    break;
                case "Ryze":
                    range = 550;
                    break;
                case "Malzahar":
                    range = 500;
                    break;
                case "Brand":
                    range = 550;
                    break;
            }
            return range;
        }
    }
}