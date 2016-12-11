using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace hi_im_gosu_Reborn
{
 /*   class QMode
    {
        public static void RunGosu()
        {
            if (Vayne.qmenu.Item("restrictq").GetValue<bool>())
            {
                var after = ObjectManager.Player.Position
                            + Vayne.Normalize(Game.CursorPos - ObjectManager.Player.Position) * 300;
                //Chat.Print("After: {0}", after);
                var disafter = Vector3.DistanceSquared(after, Vayne.tar.Position);
                //Chat.Print("DisAfter: {0}", disafter);
                //Chat.Print("first calc: {0}", (disafter) - (630*630));
                if ((disafter < 630 * 630) && disafter > 150 * 150)
                {
                    Vayne.Q.Cast(Game.CursorPos);

                }

                if (Vector3.DistanceSquared(Vayne.tar.Position, ObjectManager.Player.Position) > 630 * 630
                    && disafter < 630 * 630)
                {
                    Vayne.Q.Cast(Game.CursorPos);

                }
            }
            else
            {
                Vayne.Q.Cast(Game.CursorPos);

            }

        }

        public static void RunFlowers()
        {
            var tar = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);
            if (Vayne.qmenu.Item("UseQC", true).GetValue<bool>() && Vayne.Q.IsReady())
            {


                if (Vayne.CheckTarget(tar, 800f))
                {

                  //  Vayne.QLogic(tar);
                }
            }
        }     
        
    }*/
}

