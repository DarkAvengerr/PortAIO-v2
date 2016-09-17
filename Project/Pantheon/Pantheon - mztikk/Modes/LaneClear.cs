using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikkPantheon.Config;

    internal static class LaneClear
    {
        #region Methods

        internal static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < Config.GetSliderValue("laneclear.mana") || Events.IsAutoAttacking)
            {
                return;
            }

            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range);
            if (!minions.Any())
            {
                return;
            }

            var target = Mainframe.Orbwalker.GetTarget() is Obj_AI_Minion
                             ? Mainframe.Orbwalker.GetTarget() as Obj_AI_Minion
                             : minions.OrderBy(x => x.Distance(ObjectManager.Player)).FirstOrDefault();
            if (target == null)
            {
                return;
            }

            if (Config.IsChecked("laneclear.q") && Spells.Q.IsReady() && target.IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.Cast(target);
            }

            if (Config.IsChecked("laneclear.e") && Spells.E.IsReady() && target.IsValidTarget(Spells.E.Range))
            {
                Spells.E.Cast(target);
            }

            if (Config.IsChecked("laneclear.w") && Spells.W.IsReady() && target.IsValidTarget(Spells.W.Range))
            {
                Spells.W.Cast(target);
            }
        }

        #endregion
    }
}