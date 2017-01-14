using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
using LeagueSharp.Common; 
namespace ElKatarinaDecentralized.Utils
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class FleeManager
    {
        internal static void JumpHandler(Vector3 position, bool jumpToAllies = true, bool jumpToMinions = true) 
        {
            if (!Misc.SpellE.SpellSlot.IsReady())
            {
                return;
            }

            if (jumpToAllies || jumpToMinions)
            {
                if (jumpToAllies)
                {
                    var closestAlly = HeroManager.Allies.Where(x => x.Distance(ObjectManager.Player) < Misc.SpellE.Range && x.Distance(position) < 200 && !x.IsMe).OrderByDescending(i => i.Distance(ObjectManager.Player))
                            .ToList()
                            .FirstOrDefault();

                    if (closestAlly != null)
                    {
                        // Cast E on champion.
                        Misc.SpellE.SpellObject.CastOnUnit(closestAlly);
                        return;
                    }
                }

                if (jumpToMinions)
                {
                    var closestMinion =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                m =>
                                    m.IsAlly && m.Distance(ObjectManager.Player) < Misc.SpellE.Range && m.Distance(position) < 200
                                    && !m.Name.ToLower().Contains("ward"))
                            .OrderByDescending(i => i.Distance(ObjectManager.Player))
                            .ToList()
                            .FirstOrDefault();

                    if (closestMinion != null)
                    {
                        // Cast E on minion.
                        Misc.SpellE.SpellObject.CastOnUnit(closestMinion);
                        return;
                    }
                }
            }

        }
    }
}
