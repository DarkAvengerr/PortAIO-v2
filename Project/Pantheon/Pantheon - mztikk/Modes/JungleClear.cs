using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikkPantheon.Config;

    internal static class JungleClear
    {
        #region Methods

        internal static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < Config.GetSliderValue("jungleclear.mana") || Events.IsAutoAttacking)
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.Position, 
                Spells.Q.Range, 
                MinionTypes.All, 
                MinionTeam.Neutral);
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

            if (Config.IsChecked("jungleclear.q") && Spells.Q.IsReady() && target.IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.Cast(target);
            }

            if (Config.IsChecked("jungleclear.e") && Spells.E.IsReady() && target.IsValidTarget(Spells.E.Range))
            {
                Spells.E.Cast(target);
            }

            if (Config.IsChecked("jungleclear.w") && Spells.W.IsReady() && target.IsValidTarget(Spells.W.Range))
            {
                Spells.W.Cast(target);
            }
        }

        #endregion
    }
}