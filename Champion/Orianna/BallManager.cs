using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace Orianna
{
    public static class BallManager
    {
        public static Vector3 BallPosition { get; private set; }
        private static int _sTick = Utils.GameTimeTickCount;

        static BallManager()
        {
            Game.OnUpdate += Game_OnGameUpdate;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            BallPosition = ObjectManager.Player.Position;
        }

        static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.SData.Name)
                {
                    case "OrianaIzunaCommand":
                        LeagueSharp.Common.Utility.DelayAction.Add((int)(BallPosition.LSDistance(args.End) / 1.2 - 70 - Game.Ping), () => BallPosition = args.End);
                        BallPosition = Vector3.Zero;
                        _sTick = Utils.GameTimeTickCount;
                        break;

                    case "OrianaRedactCommand":
                        BallPosition = Vector3.Zero;
                        _sTick = Utils.GameTimeTickCount;
                    break;
                }
            }
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (Utils.GameTimeTickCount - _sTick > 300 && ObjectManager.Player.HasBuff("OrianaGhostSelf"))
            {
                BallPosition = ObjectManager.Player.Position;
            }

            foreach (var ally in HeroManager.Allies)
            {
                if (ally.HasBuff("OrianaGhost"))
                {
                    BallPosition = ally.Position;
                }
            }
        }
    }
}
