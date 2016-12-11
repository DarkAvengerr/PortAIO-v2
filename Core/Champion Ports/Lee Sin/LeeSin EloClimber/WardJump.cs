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
namespace LeeSin_EloClimber
{
    internal class WardJump
    {
        internal static void Load()
        {
            Game.OnUpdate += Update;
        }

        private static void Update(EventArgs args)
        {
            if (!MenuManager.myMenu.Item("wardjump.key").GetValue<KeyBind>().Active)
                return;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            Vector3 endPos = (Game.CursorPos);
            if (LeeSin.myHero.Distance(endPos) > 600)
                endPos = LeeSin.myHero.Position + (Game.CursorPos - LeeSin.myHero.Position).Normalized() * 600;

            LeeSin.WardJump_Position(endPos);
        }
    }
}
