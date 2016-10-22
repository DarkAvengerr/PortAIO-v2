using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoDraven
{
    class LuomRiu
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static RiuNo1 RiuNo1 { get { return Program.RiuNo1; } }
        private static Orbwalking.Orbwalker Orbwalker { get { return Program.Orbwalker; } }
        private static List<Riu> Riu { get { return Program.Riu; } }
        public static void lumriu()
        {
            if (Orbwalker._orbwalkingPoint.Distance(Player.Position) <= 50)
            {
                Orbwalker.SetMovement(false);
            }
            else
            {
                Orbwalker.SetMovement(true);
            }
            float y = 300;
            var Qbuff = Player.Buffs.Find(b => b.Name.ToLower() == "dravenspinning");
            if (Program.RiuNo1 != null)
            {
                if (Player.Distance(Game.CursorPos) + 150 >= RiuNo1.Position.Distance(Game.CursorPos))
                {
                    if (Player.Distance(RiuNo1.Position) >= 120 && Player.Distance(RiuNo1.Position) + 50 > Player.MoveSpeed * (1250 - (Utils.GameTimeTickCount - RiuNo1.CreationTime)) / 1000)
                    {
                        foreach (var riu in Riu) { if (riu.NetworkId == RiuNo1.NetworkId) { Riu.Remove(riu); } }
                        Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                        Orbwalker.SetAttack(true);
                    }
                    else
                    {
                        Orbwalker.SetOrbwalkingPoint(RiuNo1.Position);
                        if (Qbuff == null)
                        {
                            if (Player.Distance(RiuNo1.Position) + 100 < Player.MoveSpeed * (0 - Utils.GameTimeTickCount + RiuNo1.CreationTime + 1250) / 1000)
                            { Orbwalker.SetAttack(true); }
                            else { Orbwalker.SetAttack(false); }
                        }
                        else
                        {
                            float a = Player.Distance(Game.CursorPos);
                            float b = RiuNo1.Position.Distance(Game.CursorPos);
                            float c = Player.Distance(RiuNo1.Position);
                            float B = (a * a + c * c - b * b) / (2 * a * c);
                            double d = Math.Acos(B) * (180 / Math.PI);
                            if (d <= 45 && Qbuff != null)
                            {
                                if (Player.Distance(RiuNo1.Position) + 100 < Player.MoveSpeed * (0 - Utils.GameTimeTickCount + RiuNo1.CreationTime + 1250) / 1000)
                                { Orbwalker.SetAttack(true); }
                                else { Orbwalker.SetAttack(false); }
                            }
                            else { Orbwalker.SetAttack(false); }
                        }
                    }
                }
                else
                {
                    foreach (var riu in Riu) { if (riu.NetworkId == RiuNo1.NetworkId) { Riu.Remove(riu); } }
                    Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                    Orbwalker.SetAttack(true);
                }
            }
            else { Orbwalker.SetOrbwalkingPoint(Game.CursorPos); Orbwalker.SetAttack(true); }
        }
        public static void LuomRiuTest()
        {
            float y = 300;
            var Qbuff = Player.Buffs.Find(b => b.Name.ToLower()=="dravenspinning");
            if (Program.RiuNo1 != null && Player.Distance(Program.RiuNo1.Position) > 70)
            {
                var target = Orbwalker.GetTarget();
                if (target != null)
                {
                    if (Player.Distance(Game.CursorPos) + 100 >= RiuNo1.Position.Distance(Game.CursorPos) + 100)
                    {
                        if (Player.Distance(RiuNo1.Position) >= 120 && Player.Distance(RiuNo1.Position) + 50 > Player.MoveSpeed * (1250 - (Utils.GameTimeTickCount - RiuNo1.CreationTime))/1000)
                        {
                            foreach (var riu in Riu) { if (riu.NetworkId == RiuNo1.NetworkId) {Riu.Remove(riu); } }
                            Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                            Orbwalker.SetMovement(true);
                            Orbwalker.SetAttack(true);
                        }
                        else
                        {
                            Orbwalker.SetOrbwalkingPoint(RiuNo1.Position);
                            if (Qbuff == null)
                            {
                                if (Player.Distance(RiuNo1.Position) + 100 < Player.MoveSpeed * (0 - Utils.GameTimeTickCount + RiuNo1.CreationTime + 1250) / 1000)
                                { Orbwalker.SetAttack(true); Orbwalker.SetMovement(true); }
                                else { Orbwalker.SetAttack(false); Orbwalker.SetMovement(true); }
                            }
                            else
                            {
                                float a = Player.Distance(Game.CursorPos);
                                float b = RiuNo1.Position.Distance(Game.CursorPos);
                                float c = Player.Distance(RiuNo1.Position);
                                float B = (a * a + c * c - b * b) / (2 * a * c);
                                double d = Math.Acos(B) * (180 / Math.PI);
                                if (d <= 45 && Qbuff != null)
                                {
                                    if (Player.Distance(RiuNo1.Position) + 100 < Player.MoveSpeed * (0 - Utils.GameTimeTickCount + RiuNo1.CreationTime + 1250) / 1000)
                                    { Orbwalker.SetAttack(true); Orbwalker.SetMovement(true); }
                                    else { Orbwalker.SetAttack(false); Orbwalker.SetMovement(true); }
                                }
                                else { Orbwalker.SetAttack(false); Orbwalker.SetMovement(true); }
                            }
                        }
                    }
                    else
                    {
                        foreach (var riu in Riu) { if (riu.NetworkId == RiuNo1.NetworkId) { Riu.Remove(riu); } }
                        Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                        Orbwalker.SetMovement(true);
                        Orbwalker.SetAttack(true);
                    }
                }

            }
            else if (Program.RiuNo1 != null && Player.Distance(Program.RiuNo1.Position) <= 70 && Orbwalking.CanMove(40))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.HoldPosition,Player);
                Orbwalker.SetOrbwalkingPoint(Program.RiuNo1.Position);
                Orbwalker.SetMovement(false);
                if (Qbuff == null) { Program.Orbwalker.SetAttack(true); }
                else if (Qbuff != null && Utils.GameTimeTickCount - RiuNo1.CreationTime < 1000) { Orbwalker.SetAttack(true); }
                else { Orbwalker.SetAttack(false); }
            }
            else { Orbwalker.SetOrbwalkingPoint(Game.CursorPos); Orbwalker.SetAttack(true); Orbwalker.SetMovement(true); }
        }
    }
}
