using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Manager.Events.Games.Mode
{
    using System.Collections.Generic;
    using System.Linq;
    using Spells;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Orbwalking = Orbwalking;
    using static Common.Common;

    internal class EQFlash : Logic
    {
        public static void Init()
        {
            Orbwalking.MoveTo(Game.CursorPos);

            if (!SpellManager.HaveQ3)
            {
                if (Q.IsReady())
                {
                    var minion =
                        MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                            .FirstOrDefault();

                    if (minion != null && minion.DistanceToPlayer() <= Q.Range)
                    {
                        Q.Cast(minion);
                    }
                }
            }
            else
            {
                if (Me.IsDashing() && Flash != SpellSlot.Unknown && Flash.IsReady())
                {
                    var bestPos =
                        FlashPoints().Where(x => HeroManager.Enemies.Count(a => a.IsValidTarget(600f, true, x)) > 0)
                            .OrderByDescending(x => HeroManager.Enemies.Count(i => i.Distance(x, true) <= 220*220))
                            .FirstOrDefault();

                    if (bestPos.IsValid() && bestPos.CountEnemiesInRange(220) > 0 && Q3.Cast(bestPos, true))
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(10+(Game.Ping/2-5),
                                           () => Me.Spellbook.CastSpell(Flash, bestPos));
                    }
                }

                if (E.IsReady())
                {
                    var allTargets = new List<Obj_AI_Base>();

                    allTargets.AddRange(MinionManager.GetMinions(Me.Position, E.Range + 65, MinionTypes.All,
                        MinionTeam.NotAlly));
                    allTargets.AddRange(HeroManager.Enemies.Where(x => !x.IsDead && x.DistanceToPlayer() <= E.Range + 65));

                    if (allTargets.Any())
                    {
                        var eTarget =
                            allTargets.Where(x => x.IsValidTarget(E.Range + 50) && SpellManager.CanCastE(x))
                                .OrderByDescending(
                                    x =>
                                        HeroManager.Enemies.Count(
                                            t => t.IsValidTarget(600f, true, PosAfterE(x))))
                                .FirstOrDefault();

                        if (eTarget != null)
                        {
                            E.CastOnUnit(eTarget, true);
                        }
                    }
                }
            }
        }
    }
}
