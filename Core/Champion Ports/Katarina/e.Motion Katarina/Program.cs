using System;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace e.Motion_Katarina
{
    static class Program
    {
        public static void Main()
        {
            if(HeroManager.Player.ChampionName != "Katarina")
            {
                return;
            }
            Initialize(new EventArgs());
        }
        static void Initialize(EventArgs args)
        {
            //Various Entry Points
            Config.InitializeMenu();
            Logic.startLogic();
            DaggerManager.startTracking();
            BlockIssueOrder.InitializeBlockIssueOrder();
        }
    }
}
