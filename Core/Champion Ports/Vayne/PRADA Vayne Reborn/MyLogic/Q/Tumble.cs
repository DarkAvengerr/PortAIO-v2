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
namespace PRADA_Vayne.MyLogic.Q
{
    public static class Tumble
    {
        public static Vector3 TumbleOrderPos = Vector3.Zero;

        public static void Cast(Vector3 position)
        {
            if (!Program.ComboMenu.Item("QCombo").GetValue<bool>()) return;

            TumbleOrderPos = position;
            if (position != Vector3.Zero)
            {
                Program.Q.Cast(TumbleOrderPos);
            }
            if (position == Vector3.Zero && ObjectManager.Player.Buffs.Any(b => b.Name.ToLower().Contains("vayneinquisition")))
            {
                Program.Q.Cast(Game.CursorPos);
            }
        }
    }
}
