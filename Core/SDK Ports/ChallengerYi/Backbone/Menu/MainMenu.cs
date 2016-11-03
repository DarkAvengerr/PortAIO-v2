using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ChallengerYi.Backbone.Menu
{
    internal class MainMenu
    {
        internal static LeagueSharp.SDK.UI.Menu Menu;

        internal static void Create(string menuPreffix, string displayName = "leaguesharp")
        {
            var menuName = menuPreffix + "mm";
            var menuDisplayName = displayName != "leaguesharp"
                ? displayName
                : ObjectManager.Player.ChampionName + " To The Challenger";

            Menu = new LeagueSharp.SDK.UI.Menu(menuName, menuDisplayName, true);

            new Spell1Menu(menuPreffix);
            new Spell2Menu(menuPreffix);
            new Spell3Menu(menuPreffix);
            new Spell4Menu(menuPreffix);

            Menu.Attach();
        }
    }
}
