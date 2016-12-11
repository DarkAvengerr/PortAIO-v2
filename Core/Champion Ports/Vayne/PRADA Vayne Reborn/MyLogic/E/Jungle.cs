using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using PRADA_Vayne.MyUtils;

using EloBuddy; 
using LeagueSharp.Common; 
namespace PRADA_Vayne.MyLogic.E
{
    public static partial class Events
    {
        private static string[] _jungleMobs = new[]
        {
            "SRU_Blue", "SRU_Red", "SRU_Krug", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "TT_Spiderboss", "TTNGolem",
            "TTNWraith", "TTNWolf"
        };
        public static void JungleUsage(EventArgs args)
        {
            if (Program.LaneClearMenu.Item("EJungleMobs").GetValue<bool>() && Program.E.IsReady())
            {
                var target = Program.Orbwalker.GetTarget();
                if (target != null && target is Obj_AI_Minion)
                {
                    var minion = (Obj_AI_Minion) target;
                    if (_jungleMobs.Contains(minion.BaseSkinName))
                    {
                        for (var i = 40; i < 425; i += 141)
                        {
                            var flags = NavMesh.GetCollisionFlags(
                                minion.ServerPosition.To2D()
                                    .Extend(
                                        Heroes.Player.ServerPosition.To2D(),
                                        -i)
                                    .To3D());
                            if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                            {
                                Program.E.Cast(minion);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
