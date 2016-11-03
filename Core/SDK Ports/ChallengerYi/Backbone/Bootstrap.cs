using ChallengerYi.Backbone.Menu;
using ChallengerYi.Backbone.Utils;
using ChallengerYi.Logic;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Utils;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ChallengerYi.Backbone
{
    internal class Bootstrap
    {
        internal static void Start()
        {
            //Init SDK
            LeagueSharp.SDK.Bootstrap.Init();

            //Init the menu
            MainMenu.Create("challengeryi", "Challenger Yi");

            //Enable Orbwalker (some assemblies still disable it zzz)

            if (Variables.Orbwalker.Enabled == false)
            {
                Variables.Orbwalker.Enabled = true;
            }

            //Load champion info
            GameData.Init();

            Chat.Print("<font color='#3fbffd'>myo (imsosharp):</font> <font color='#ffffff'>Hey, what's up? Thanks for trying Challenger Yi :)</font>");
            Chat.Print("<font color='#3fbffd'>myo (imsosharp):</font> <font color='#ffffff'>R>Q>E>W, Bloodrazor's > Guinsoo's > BOTRK > 1SHOT</font>");
            Chat.Print("<font color='#3fbffd'>myo (imsosharp):</font> <font color='#ffffff'>I'm using Evade + ElUtility with this. GodJungleTracker too if updated.</font>");
            Chat.Print("<font color='#3fbffd'>myo (imsosharp):</font> <font color='#ffffff'>If you like this assembly upvote it in db :) cheers!</font>");

            //Init Logics
            new Spell1Usage();
            new Spell2Usage();
            new Spell3Usage();
            new Spell4Usage();
        }
    }
}
