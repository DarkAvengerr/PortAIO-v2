using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SSHCommon;
//typedefs
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Ekko
{
    class Program
    {
        public static BaseChamp Champion;
        static void Main(string[] args)
        {
            if (Game.Mode == GameMode.Running)
            {
                Game_OnGameLoad(null);
            }

            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
			if(ObjectManager.Player.ChampionName == "Ekko")
			{
				Champion = new Ekko();
				Champion.CreateConfigMenu();
				Champion.SetSpells();
				SPrediction.Prediction.Initialize(Champion.Config);
				Notifications.AddNotification(String.Format("HikiCarry Ekko Loaded !"), 3000);
				Notifications.AddNotification(String.Format("Dont Forget Upvote on"), 4000);
				Notifications.AddNotification(String.Format("Assembly.DB"), 5000);
			}
			else
			{
				return;
			}
        }
    }
}
