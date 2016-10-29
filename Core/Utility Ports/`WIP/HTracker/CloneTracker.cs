using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core;
using LeagueSharp.SDK.UI;


using EloBuddy; 
 namespace HTrackerSDK
{
    class CloneTracker
    {
        public static void OnLoad()
        {

            var cloneTracker = Tracker.Menu.Add(new Menu("clone.tracker", "Clone Tracker"));
            {
                cloneTracker.Add(new MenuBool("show.clone", "Clone Tracker ?", true));
            }
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (!Tracker.Menu["clone.track"]["show.clone"]) return;

            foreach (var hero in GameObjects.Get<AIHeroClient>().Where(o => o.IsVisible && o.IsEnemy && !o.IsDead && (o.ChampionName.ToLower().Contains("yorick") 
                || o.ChampionName.ToLower().Contains("leblanc") || o.ChampionName.ToLower().Contains("monkeyking") || o.ChampionName.ToLower().Contains("shaco"))
                && o.ServerPosition.IsOnScreen()))
            {
                Drawing.DrawCircle(hero.Position,100,Color.LawnGreen);
            }
        }
    }
}
