using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheGaren
{
    // ReSharper disable once InconsistentNaming
    static class AAHelper
    {
        public static bool JustFinishedAutoattack;
        public static bool WillAutoattackSoon;
        private static float _lastAaTime;
        private static bool _windingUp;
        private static readonly AIHeroClient Player;

        static AAHelper()
        {
            Player = ObjectManager.Player;
            Game.OnUpdate += Update;
        }

        private static void Update(EventArgs args)
        {
            JustFinishedAutoattack = _windingUp && !Player.Spellbook.IsAutoAttacking;
            if (JustFinishedAutoattack)
                _lastAaTime = Game.Time;
            WillAutoattackSoon = (_windingUp = Player.Spellbook.IsAutoAttacking) || Game.Time - _lastAaTime > Player.AttackDelay / 2f;
        }
    }
}
