using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.Events
{
    class BeforeAttack
    {
        public static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe)
            {
                var buff = ObjectManager.Player.GetBuff("vaynetumblefade");
                if (buff != null)
                    if (buff.IsValidBuff())
                        if (Program.rmenu.Item("visibleR").GetValue<bool>() && Program.Player.CountEnemiesInRange(800) >= 1)
                        {
                            if (buff.EndTime - Game.Time >
                            buff.EndTime - buff.StartTime -
                            (Program.rmenu.Item("Qtime").GetValue<Slider>().Value / 1000))
                                if (!ObjectManager.Player.Position.UnderTurret(true))
                                    args.Process = false;
                        }
                        else
                        {
                            args.Process = true;
                        }
            }
        }
    }
}
