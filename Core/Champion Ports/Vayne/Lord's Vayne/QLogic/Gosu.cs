using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using System.Linq;
using System.Collections.Generic;
using System;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.QLogic
{
    class Gosu
    {
        public static void Run()
        {
            if ((Program.orbwalker.ActiveMode.ToString() == "Combo") && Program.qmenu.Item("UseQC").GetValue<bool>() && !Program.qmenu.Item("FastQ").GetValue<bool>()
                    || (Program.orbwalker.ActiveMode.ToString() == "Mixed" && Program.qmenu.Item("hq").GetValue<bool>()))

            {
                if (Program.qmenu.Item("restrictq").GetValue<bool>())
                {
                    var after = ObjectManager.Player.Position
                                + Normalize(Game.CursorPos - ObjectManager.Player.Position) * 300;
                    //Chat.Print("After: {0}", after);
                    var disafter = Vector3.DistanceSquared(after, Program.tar.Position);
                    //Chat.Print("DisAfter: {0}", disafter);
                    //Chat.Print("first calc: {0}", (disafter) - (630*630));
                    if ((disafter < 630 * 630) && disafter > 150 * 150)
                    {
                        Program.Q.Cast(Game.CursorPos);


                    }

                    if (Vector3.DistanceSquared(Program.tar.Position, ObjectManager.Player.Position) > 630 * 630
                        && disafter < 630 * 630)
                    {
                        Program.Q.Cast(Game.CursorPos);


                    }
                }
                else
                {
                    Program.Q.Cast(Game.CursorPos);



                }
                //Q.Cast(Game.CursorPos);
            }

            if ((Program.orbwalker.ActiveMode.ToString() == "Combo")&& Program.qmenu.Item("UseQC").GetValue<bool>() && Program.qmenu.Item("UseQC").GetValue<bool>() && Program.qmenu.Item("FastQ").GetValue<bool>()
                   || (Program.orbwalker.ActiveMode.ToString() == "Mixed" && Program.qmenu.Item("hq").GetValue<bool>()))
            {
                if (Program.qmenu.Item("restrictq").GetValue<bool>())
                {
                    var after = ObjectManager.Player.Position
                                + Normalize(Game.CursorPos - ObjectManager.Player.Position) * 300;
                    //Chat.Print("After: {0}", after);
                    var disafter = Vector3.DistanceSquared(after, Program.tar.Position);
                    //Chat.Print("DisAfter: {0}", disafter);
                    //Chat.Print("first calc: {0}", (disafter) - (630*630));
                    if ((disafter < 630 * 630) && disafter > 150 * 150)
                    {
                        Program.Q.Cast(Game.CursorPos);
                        EloBuddy.Player.DoEmote(Emote.Dance);

                    }

                    if (Vector3.DistanceSquared(Program.tar.Position, ObjectManager.Player.Position) > 630 * 630
                        && disafter < 630 * 630)
                    {
                        Program.Q.Cast(Game.CursorPos);
                        EloBuddy.Player.DoEmote(Emote.Dance);

                    }
                }
                else
                {
                    Program.Q.Cast(Game.CursorPos);
                    EloBuddy.Player.DoEmote(Emote.Dance);


                }
                //Q.Cast(Game.CursorPos);//
            }
        }


        public static Vector3 Normalize(Vector3 A)
        {
            double distance = Math.Sqrt(A.X * A.X + A.Y * A.Y);
            return new Vector3(new Vector2((float)(A.X / distance)), (float)(A.Y / distance));

        }
    }
}
 

