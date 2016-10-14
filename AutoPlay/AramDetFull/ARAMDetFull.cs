using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ARAMDetFull.Champions;
using DetuksSharp;
using LeagueSharp;
using DetuksSharp;
using LeagueSharp.Common;
using KeyBindType = LeagueSharp.Common.KeyBindType;

using EloBuddy;
namespace ARAMDetFull
{
    class ARAMDetFull
    {
        /* TODO:
         * ##- Tower range higher dives a lot
         * ##- before level 6/7 play safer dont go so close stay behind other players or 800/900 units away from closest enemy champ
         * ##- Target selector based on invincible enemies
         * ##- IF invincible or revive go full in
         * ##- if attacking enemy and it is left 3 or less aa to kill then follow to kill (check movespeed dif)
         *  - bush invis manager player death
         *  - fixx gankplank plays like retard
         *  - this weeks customs
         *  - WPF put to allways take mark
         * ##- nami auto level
         *  - Some skills make aggresivity for time and how much to put in balance ignore minsions on/off
         * ## - LeeSin
         * ## - Nocturn
         *  - Gnar
         *  -Katarina error
         *  - Gangplank error
         *  ##- healing relics
         *  -Make velkoz
         */
        public static TextWriter defaultOut;
        public delegate void OnGameEndHandler(bool win);

        public ARAMDetFull()
        {
            Console.WriteLine("Aram det full started!");
            onLoad();
        }

        public static int gameStart = 0;

        public static Menu Config;

        public static int now
        {
            get { return (int)DateTime.Now.TimeOfDay.TotalMilliseconds; }
        }

        public static List<int> allyIds;
        public static List<int> enemyIds;

        private static void onLoad()
        {
            r = new Random();
            allyIds = new List<int>();
            enemyIds = new List<int>();

            gameStart = now;

            Chat.Print("ARAm - Sharp by DeTuKs");
            try
            {
                defaultOut = System.Console.Out;

                Config = new Menu("ARAM", "aramBot", true);

                var orbwalkerMenu = new Menu("Orbwalker", "my_Orbwalker");
                DeathWalker.AddToMenu(orbwalkerMenu);
                Config.AddSubMenu(orbwalkerMenu);


                Config.AddToMainMenu();

                Game.OnUpdate += OnGameUpdate;
                Game.OnNotify += Game_OnNotify;

                var gameEndNotified = false;

                OnGameEnd += ARAMDetFull_OnGameEnd;

                Game.OnTick += delegate
                {
                    if (gameEndNotified)
                    {
                        return;
                    }
                    var nexus = ObjectManager.Get<Obj_HQ>();
                    if (nexus == null)
                    {
                        return;
                    }
                    if (nexus.Any(n => n.IsDead || n.Health.Equals(0)))
                    {
                        var win = ObjectManager.Get<Obj_HQ>().Any(n => n.Health.Equals(0));
                        OnGameEnd?.Invoke(win);
                        gameEndNotified = true;
                    }
                };

                ARAMSimulator.setupARMASimulator();

                foreach(AIHeroClient ally in EloBuddy.SDK.EntityManager.Heroes.Allies)
                {
                    allyIds.Add(ally.NetworkId);
                }

                foreach (AIHeroClient enemy in EloBuddy.SDK.EntityManager.Heroes.Enemies)
                {
                    enemyIds.Add(enemy.NetworkId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static Random r { get; private set; }

        public static int teamKills;
        public static int enemyKills;

        private static void Game_OnNotify(GameNotifyEventArgs args)
        {
            if (args.EventId == GameEventId.OnChampionKill)
            {
                if(allyIds.Contains((int)args.NetworkId))
                {
                    teamKills++;
                }
                else
                {
                    enemyKills++;
                }
            }

            if (args.EventId == GameEventId.OnChampionPentaKill && allyIds.Contains((int)args.NetworkId))
            {
                Chat.Say("ez");
            }

            if (args.EventId == GameEventId.OnSurrenderVote)
                EloBuddy.SDK.Core.DelayAction(() => Chat.Say("/ff"), r.Next(2000, 5000));
        }

        private static void ARAMDetFull_OnGameEnd(bool win)
        {
            var rnd = new Random().Next(15000, 30000) + Game.Ping;
            EloBuddy.SDK.Core.DelayAction(() => { Game.QuitGame(); }, rnd);
        }

        public static event OnGameEndHandler OnGameEnd;
        private static int lastTick = now;

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                lastTick = now;
                ARAMSimulator.updateArmaPlay();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
