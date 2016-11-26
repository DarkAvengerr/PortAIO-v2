using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoKatarina
{
    using static BadaoKatarinaVariables;
    using static BadaoMainVariables;
    using static BadaoKatarinaHelper;
    public static class BadaoKatarinaFlee
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (FleeKey.GetValue<KeyBind>().Active)
            {
                Orbwalking.Orbwalk(null, Game.CursorPos);
                if(E.IsReady())
                {
                    var nearest = GetEVinasun().MinOrDefault(x => x.Position.Distance(Game.CursorPos));
                    if (nearest != null && nearest.Position.Distance(Game.CursorPos) < Player.Distance(Game.CursorPos))
                    {
                        var pos = nearest.Position.To2D().Extend(Game.CursorPos.To2D(), 150);
                        E.Cast(pos);
                    }
                }
            }
            if (JumpKey.GetValue<KeyBind>().Active)
            {
                var x = Player.Position.Extend(Game.CursorPos, 100);
                var y = Player.Position.Extend(Game.CursorPos, 30);
                var z = Player.Position.Extend(Game.CursorPos, 50);
                if (!x.IsWall() && !y.IsWall()) EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, x);
                if (x.IsWall() && !y.IsWall()) EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, y);
                if (x.IsWall() && z.IsWall() && W.IsReady() && E.IsReady())
                {
                    //for (int i = -30; i < 31; i = i + 5)
                    //{
                    //    var point = BadaoChecker.BadaoRotateAround(Game.CursorPos.To2D(), Player.Position.To2D(), i);
                    //    var firstwall = BadaoMath.GetFirstWallPoint(Player.Position.To2D(), point);
                    //    var lastwall = BadaoMath.GetLastWallPoint(Player.Position.To2D(), point);
                    //    if (firstwall != null && lastwall != null && BadaoMath.IsRealWall((Vector2)firstwall, (Vector2)lastwall)
                    //        && Player.Position.To2D().Distance((Vector2)lastwall) <= 350
                    //        && 175 - Player.Distance((Vector2)firstwall) >= ((Vector2)firstwall).Distance((Vector2)lastwall) / 2)
                    //    {
                    //        var pos = Player.Position.To2D().Extend(point, 150);
                    //        W.Cast();
                    //        LeagueSharp.Common.Utility.DelayAction.Add(100 + Game.Ping, () => E.Cast(pos));
                    //        break;
                    //    }
                    //}
                    var firstwall = BadaoMath.GetFirstWallPoint(Player.Position.To2D(), Game.CursorPos.To2D());
                    var lastwall = BadaoMath.GetLastWallPoint(Player.Position.To2D(), Game.CursorPos.To2D());
                    if (firstwall != null && lastwall != null && BadaoMath.IsRealWall((Vector2)firstwall, (Vector2)lastwall)
                        && Player.Position.To2D().Distance((Vector2)lastwall) <= 400
                        && 225 - Player.Distance((Vector2)firstwall) >= ((Vector2)firstwall).Distance((Vector2)lastwall) / 2)
                    {
                        var pos = Player.Position.To2D().Extend(Game.CursorPos.To2D(), 225);
                        W.Cast();
                        LeagueSharp.Common.Utility.DelayAction.Add(100 + Game.Ping, () => E.Cast(pos));
                    }
                }
            }
        }
    }
}
