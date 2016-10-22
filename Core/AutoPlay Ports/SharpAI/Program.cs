using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAI.Enums;
using SharpAI.SummonersRift;
using SharpAI.SummonersRift.Data;
using SharpAI.Utility;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.UI;
using LeagueSharp.SDK.Utils;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy;
using LeagueSharp.SDK;
namespace SharpAI
{
    class Program
    {
        public delegate void OnGameEndHandler(bool win);
        public static event OnGameEndHandler OnGameEnd;

        public static System.Random r { get; private set; }

        public static void Main()
        {
            if (Game.MapId != GameMapId.SummonersRift || Game.MapId == GameMapId.HowlingAbyss)
            {
                return;
            }

            Bootstrap.Init();
            r = new System.Random();

            SessionBasedData.LoadTick = ObjectManager.Get<Obj_AI_Minion>().Any(m => m.CharData.BaseSkinName.Contains("Minion"))
                ? Environment.TickCount - 190000
                : Environment.TickCount;

            Hotfixes.Load();

            Logging.Log("LOADED " + SessionBasedData.LoadTick);

            Game.OnUpdate += (updateArgs) =>
            {
                if (Environment.TickCount - SessionBasedData.LoadTick > 15000)
                {
                    Tree.Seed();
                    Tree.Water();
                }
                else
                {
                    Logging.Log("WAITING FOR GAME START");
                }
            };

            var gameEndNotified = false;

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

            OnGameEnd += endArgs =>
            {
                Task.Run(
                    async () =>
                    {
                        await Task.Delay(10000);
                        Game.QuitGame();
                    });
            };
        }
    }
}
