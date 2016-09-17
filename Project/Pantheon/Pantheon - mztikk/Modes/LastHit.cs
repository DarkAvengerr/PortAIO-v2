using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikkPantheon.Config;

    internal static class LastHit
    {
        #region Methods

        internal static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < Config.GetSliderValue("lasthit.mana") || Events.IsAutoAttacking
                || !Config.IsChecked("lasthit.q") || !Spells.Q.IsReady())
            {
                return;
            }

            var minions =
                MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range)
                    .Where(x => x.Health < Spells.Q.GetDamage(x))
                    .ToList();
            if (!minions.Any())
            {
                return;
            }

            Obj_AI_Base target;
            if (Config.IsChecked("lasthit.q.oor"))
            {
                target =
                    minions.FirstOrDefault(
                        x => x.Distance(ObjectManager.Player) > Orbwalking.GetRealAutoAttackRange(x) + 25);
            }
            else
            {
                target = minions.FirstOrDefault();
            }

            if (target == null || !target.IsValidTarget(Spells.Q.Range))
            {
                return;
            }

            Spells.Q.Cast(target);
        }

        #endregion
    }
}