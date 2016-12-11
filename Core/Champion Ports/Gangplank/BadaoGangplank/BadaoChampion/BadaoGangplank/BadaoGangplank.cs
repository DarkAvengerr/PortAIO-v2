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
namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    using static BadaoMainVariables;
    using static BadaoGangplankVariables;
    public static class BadaoGangplank
    {
        public static void BadaoActivate()
        {
            BadaoGangplankConfig.BadaoActivate();
            BadaoGangplankBarrels.BadaoActivate();
            BadaoGangplankCombo.BadaoActivate();
            BadaoGangplankHarass.BadaoActivate();
            BadaoGangplankLaneClear.BadaoActivate();
            BadaoGangplankJungleClear.BadaoActivate();
            BadaoGangplankAuto.BadaoActivate();
            //Game.OnUpdate += Game_OnUpdate;
        }
        private static int lastcount = 0;
        private static void Game_OnUpdate(EventArgs args)
        {
            //Chat.Print(E.Instance.Ammo.ToString());
            if (BadaoGangplankBarrels.QableBarrels().Any())
                Chat.Print("yes");
            if (Environment.TickCount - lastcount < 100 + Game.Ping)
                return;
            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed)
                return;
            foreach (var barrel in BadaoGangplankBarrels.QableBarrels())
            {
                if (Q.IsReady() && E.IsReady() && E.Instance.Ammo >= 2)
                {
                    Orbwalking.Attack = false;
                    Orbwalking.Move = false;
                    LeagueSharp.Common.Utility.DelayAction.Add(100 + Game.Ping + 875, () =>
                    {
                        Orbwalking.Attack = true;
                        Orbwalking.Move = true;
                    });
                    var pos1 = barrel.Bottle.Position.To2D().Extend(ObjectManager.Player.Position.To2D(), 0);
                    var pos2 = pos1.Extend( ObjectManager.Player.Position.To2D(), 0);
                    E.Cast(pos1);
                    LeagueSharp.Common.Utility.DelayAction.Add(450, () => Q.Cast(barrel.Bottle));
                    LeagueSharp.Common.Utility.DelayAction.Add(875, () => E.Cast(pos2));
                    lastcount = Environment.TickCount + 875;
                    return;
                }
            }
        }
    }
}
