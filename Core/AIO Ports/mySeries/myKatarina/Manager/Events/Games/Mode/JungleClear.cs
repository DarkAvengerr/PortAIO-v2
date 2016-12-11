using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Events.Games.Mode
{
    using System.Linq;
    using LeagueSharp.Common;
    using myCommon;
    using Spells;

    internal class JungleClear : Logic
    {
        internal static void Init()
        {
            if (SpellManager.isCastingUlt || !ManaManager.SpellFarm)
            {
                return;
            }

            var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (mobs.Any())
            {
                var mob = mobs.FirstOrDefault();

                if (mob != null && mob.IsValidTarget(Q.Range))
                {
                    if (Menu.GetBool("JungleClearQ") && Q.IsReady() && mob.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(mob, true);
                    }

                    if (Menu.GetBool("JungleClearW") && W.IsReady() && mob.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }

                    if (Menu.GetBool("JungleClearE") && E.IsReady())
                    {
                        if (Daggers.Any(
                            x =>
                                mobs.Any(a => a.Distance(x.Position) <= SpellManager.PassiveRange) &&
                                x.Position.DistanceToPlayer() <= E.Range))
                        {
                            foreach (
                                var obj in
                                Daggers.Where(x => x.Position.Distance(mob.Position) <= SpellManager.PassiveRange)
                                    .OrderByDescending(x => x.Position.Distance(mob.Position)))
                            {
                                if (obj.Dagger != null && obj.Dagger.IsValid && obj.Position.DistanceToPlayer() <= E.Range)
                                {
                                    E.Cast(obj.Position, true);
                                }
                            }
                        }
                        else if (mob.DistanceToPlayer() <= E.Range + 130)
                        {
                            var pos = Me.Position.Extend(mob.Position, mob.DistanceToPlayer() + 130);

                            E.Cast(pos, true);
                        }
                        else if (mob.IsValidTarget(E.Range))
                        {
                            E.Cast(mob, true);
                        }
                    }
                }
            }
        }
    }
}
