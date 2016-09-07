#region

using Infected_Twitch.Core;
using Infected_Twitch.Event;
using Infected_Twitch.Menus;
using LeagueSharp;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Infected_Twitch
{
    internal class LoadAssembly
    {
        public static void OnGameLoad()
        {
            Spells.Load();
            MenuConfig.Load();
            Recall.Load();

            Game.OnUpdate += Killsteal.Update;
            Game.OnUpdate += Skinchanger.Update;
            Game.OnUpdate += Modes.Update;
            Game.OnUpdate += EOnDeath.Update;
            Game.OnUpdate += Trinkets.Update;

            Drawing.OnDraw += DrawSpells.OnDraw;
            Drawing.OnEndScene += DrawDmg.OnEndScene;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Infected Twitch</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> 6.17</font></b>");
        }
    }
}
