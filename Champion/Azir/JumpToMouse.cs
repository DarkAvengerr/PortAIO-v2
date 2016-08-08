using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using System.Text.RegularExpressions;
using Color = System.Drawing.Color;
using EloBuddy;

namespace HeavenStrikeAzir
{
    public static class JumpToMouse
    {
        public static AIHeroClient Player { get{ return ObjectManager.Player; } }
        public static int LastJump;
        public static void Initialize()
        {
            Game.OnUpdate += Game_OnUpdate;
            CustomEvents.Unit.OnDash += Unit_OnDash;
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            //if (!sender.IsMe)
            //    return;
            //Chat.Print(args.Speed.ToString());
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            //Chat.Print(Player.LSDistance(Game.CursorPos).ToString());
            //Chat.Print(GameObjects.EnemyMinions.Count().ToString());
            if (!Program.eqmouse)
                return;
            if (OrbwalkCommands.CanMove())
            {
                OrbwalkCommands.MoveTo(Game.CursorPos);
            }
            if (Environment.TickCount - LastJump < 500)
                return;
            if (Program.Eisready && Program.Qisready())
            {
                var position = Game.CursorPos;
                var distance = Player.Position.LSDistance(position);
                var sold = Soldiers.soldier
                    .Where(x => Player.LSDistance(x.Position) <= 1100)
                    .OrderBy(x => x.Position.LSDistance(Game.CursorPos)).FirstOrDefault();
                var posWs = GeoAndExten.GetWsPosition(position.LSTo2D()).Where(x => x != null);
                // case 1
                if (sold != null && sold.Position.LSDistance(position) <= 875)
                {
                    var time = sold.Position.LSDistance(Player.Position) * 1000 / 1700;
                    Program._e.Cast(sold.Position);
                    LeagueSharp.Common.Utility.DelayAction.Add((int)time - 150 - Program.EQdelay, () => Program._q2.Cast(position));
                    LastJump = Environment.TickCount;
                    return;
                }
                // case 2
                var posW2 = posWs.FirstOrDefault(x => ((Vector2)x).LSDistance(position) <= 875);
                if (Program._w.LSIsReady() && posW2 != null)
                {
                    var time = ((Vector2)posW2).LSDistance(Player.Position) * 1000 / 1700;
                    Program._w.Cast(Player.Position.LSTo2D().LSExtend((Vector2)posW2,Program._w.Range));
                    LeagueSharp.Common.Utility.DelayAction.Add(0, () => Program._e.Cast((Vector2)posW2));
                    LeagueSharp.Common.Utility.DelayAction.Add((int)time + 300 - 150 - Program.EQdelay, () => Program._q2.Cast(position));
                    LastJump = Environment.TickCount;
                    return;
                }
                // case 3
                var posW3 = posWs.OrderBy(x => ((Vector2)x).LSDistance(position)).FirstOrDefault();
                if (sold != null && (posW3 == null || sold.Position.LSDistance(position) <= ((Vector2)posW3).LSDistance(position)))
                {
                    var time = sold.Position.LSDistance(Player.Position) * 1000 / 1700;
                    Program._e.Cast(sold.Position);
                    LeagueSharp.Common.Utility.DelayAction.Add((int)time - 150 - Program.EQdelay, () => Program._q2.Cast(position));
                    LastJump = Environment.TickCount;
                    return;
                }
                if (Program._w.LSIsReady() && posW3 != null)
                {
                    var time = ((Vector2)posW3).LSDistance(Player.Position) * 1000 / 1700;
                    Program._w.Cast(Player.Position.LSTo2D().LSExtend((Vector2)posW3, Program._w.Range));
                    LeagueSharp.Common.Utility.DelayAction.Add(0, () => Program._e.Cast((Vector2)posW3));
                    LeagueSharp.Common.Utility.DelayAction.Add((int)time + 300 - 150 - Program.EQdelay, () => Program._q2.Cast(position));
                    LastJump = Environment.TickCount;
                    return;
                }

            }
        }


    }
}
