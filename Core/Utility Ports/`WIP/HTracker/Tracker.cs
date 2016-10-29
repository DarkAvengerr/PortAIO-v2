using System;
using System.Linq;
using System.Xml.Schema;
using LeagueSharp;
using LeagueSharp.SDK.Core;
using LeagueSharp.SDK.UI;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Menu = LeagueSharp.SDK.UI.Menu;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HTrackerSDK
{
    class Tracker
    {
        public static Menu Menu;
        public static void OnLoad()
        {
            Menu = new Menu("HTracker", "HTracker", true);
            {
                var spellMenu = Menu.Add(new Menu("spell.track", "Spell Tracker"));
                {
                    spellMenu.Add(new MenuBool("track.ally.skill", "Track Ally Spells", false));
                    spellMenu.Add(new MenuBool("track.my.skill", "Track My Spells", false));
                    spellMenu.Add(new MenuBool("track.enemy.skill", "Track Enemy Spells", true));
                }
                Menu.Attach();
            }
            Drawing.OnDraw += OnDraw;
           
        }
        private static void OnDraw(EventArgs args)
        {
            if (Menu["spell.track"]["track.my.skill"])
            {
                SpellTracker.PlayerTracker();
            }
            if (Menu["spell.track"]["track.ally.skill"])
            {
                SpellTracker.AllyTracker();
            }
            if (Menu["spell.track"]["track.enemy.skill"])
            {
                SpellTracker.EnemyTracker();
            }
        }
    }
}
