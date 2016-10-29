using System;
using System.Linq;
using System.Windows.Forms;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.UI;
using LeagueSharp.SDK.Utils;
using SharpDX;
using Menu = LeagueSharp.SDK.UI.Menu;

using EloBuddy; 
 namespace HTrackerSDK
{
    class PathTracker
    {
        public static void OnLoad()
        {
            var pathMenu = Tracker.Menu.Add(new Menu("path.tracker", "Path Tracker"));
            {
                pathMenu.Add(new MenuBool("ally.path", "Ally Path ?"));
                pathMenu.Add(new MenuBool("enemy.path", "Enemy Path ?"));
                pathMenu.Add(new MenuBool("my.path", "My Path ?"));
                pathMenu.Add(new MenuBool("eta", "Eta ?"));
            }

            Drawing.OnDraw += Drawing_OnDraw;
        }
        public static Vector3 WayPoint(AIHeroClient hero)
        {
            return hero.GetWaypoints()[hero.GetWaypoints().Count - 1].ToVector3();
        }
        public static float Eta(AIHeroClient hero)
        {
            var x1 = hero.Distance(WayPoint(hero));
            var x2 = x1 / hero.MoveSpeed;
            return x2;
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Tracker.Menu["path.tracker"]["ally.path"])
            {
                foreach (var ally in GameObjects.AllyHeroes.Where(x => !x.IsMe && ObjectManager.Player.Distance(x.Position) < 1000))
                {
                    if (Tracker.Menu["path.tracker"]["eta"])
                    {
                        Drawing.DrawText((int)Drawing.WorldToScreen(WayPoint(ally)).X + 20, (int)Drawing.WorldToScreen(WayPoint(ally)).Y + 20, System.Drawing.Color.Gold, "" + Eta(ally));
                    }
                    Drawing.DrawLine(Drawing.WorldToScreen(ally.Position), Drawing.WorldToScreen(WayPoint(ally)), 2, System.Drawing.Color.Gold);
                }
            }
            if (Tracker.Menu["path.tracker"]["enemy.path"])
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(x => ObjectManager.Player.Distance(x.Position) < 1000))
                {
                    if (Tracker.Menu["path.tracker"]["eta"])
                    {
                        Drawing.DrawText((int)Drawing.WorldToScreen(WayPoint(enemy)).X + 20, (int)Drawing.WorldToScreen(WayPoint(enemy)).Y + 20, System.Drawing.Color.Gold, "" + Eta(enemy));
                    }
                    Drawing.DrawLine(Drawing.WorldToScreen(enemy.Position), Drawing.WorldToScreen(WayPoint(enemy)), 2, System.Drawing.Color.Gold);
                }
            }
            if (Tracker.Menu["path.tracker"]["my.path"])
            {
                if (Tracker.Menu["path.tracker"]["eta"])
                {
                    Drawing.DrawText((int)Drawing.WorldToScreen(WayPoint(GameObjects.Player)).X + 20, (int)Drawing.WorldToScreen(WayPoint(GameObjects.Player)).Y + 20, System.Drawing.Color.Gold, "" + Eta(GameObjects.Player));
                }
                Drawing.DrawLine(Drawing.WorldToScreen(GameObjects.Player.Position), Drawing.WorldToScreen(WayPoint(GameObjects.Player)), 2, System.Drawing.Color.Gold);
            }
        }
    }
}
