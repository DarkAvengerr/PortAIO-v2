using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using RivenToTheChallenger.Combat;
using RivenToTheChallenger.Spells;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RivenToTheChallenger
{
    class Riven
    {
        private Config config;
        private Combo combo;

        public Riven()
        {
            Console.WriteLine("Riven Loaded");
            if (ObjectManager.Player.ChampionName != "Riven")
            {
                return;
            }
            OnLoad();
        }

        public void OnLoad()
        {
            config = new Config();
            combo = new Combo(config);
            SetupEvents();
        }
        public void SetupEvents()
        {
            Game.OnUpdate += GameOnOnUpdate;
            Drawing.OnEndScene += DrawingOnOnEndScene;
        }

        private void DrawingOnOnEndScene(EventArgs args)
        {
            //var s = ObjectManager.Player.Distance(Game.CursorPos);
            //Console.WriteLine($"{s} - {s * s}");
            Drawing.DrawCircle(ObjectManager.Player.Position, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player), System.Drawing.Color.Red);
        }

        private void GameOnOnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
        }
    }
}
