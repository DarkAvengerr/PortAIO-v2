using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkXinZhao.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikkXinZhao.Extensions;

    using Config = mztikkXinZhao.Config;

    internal static class JungleClear
    {
        #region Public Methods and Operators

        public static void JungleOnPostAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Mainframe.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || ObjectManager.Player.ManaPercent < Config.GetSliderValue("jcMana"))
            {
                return;
            }

            var jngTargetsNo = MinionManager.GetMinions(
                ObjectManager.Player.Position, 
                300, 
                MinionTypes.All, 
                MinionTeam.Neutral);
            var jngTargets = Config.IsChecked("jungleclear.smallmonsters")
                                 ? jngTargetsNo.OrderBy(x => x.MaxHealth)
                                 : jngTargetsNo.OrderByDescending(x => x.MaxHealth);
            var jngTarget = jngTargets.FirstOrDefault();
            if (jngTarget == null || target.NetworkId != jngTarget.NetworkId)
            {
                return;
            }

            var combinedHealth =
                jngTargets.Where(m => m.Distance(ObjectManager.Player.Position) <= 250).Sum(targut => targut.Health);
            if (Spells.W.CanCast() && combinedHealth >= ObjectManager.Player.GetAutoAttackDamage(jngTarget) * 4
                && Config.IsChecked("useWJC"))
            {
                Spells.W.Cast();
            }

            if (Spells.Q.CanCast() && combinedHealth >= ObjectManager.Player.GetAutoAttackDamage(jngTarget) * 4
                && Config.IsChecked("useQJC"))
            {
                Spells.Q.Cast();
            }
        }

        #endregion

        #region Methods

        internal static void Execute()
        {
            var jngTargetsNo = MinionManager.GetMinions(
                ObjectManager.Player.Position, 
                300, 
                MinionTypes.All, 
                MinionTeam.Neutral);
            var jngTargets = Config.IsChecked("jungleclear.smallmonsters")
                                 ? jngTargetsNo.OrderBy(x => x.MaxHealth)
                                 : jngTargetsNo.OrderByDescending(x => x.MaxHealth);
            var jngTarget = jngTargets.FirstOrDefault();
            if (jngTarget == null)
            {
                return;
            }

            if (Config.IsChecked("useEJC") && Spells.E.CanCast(jngTarget) && jngTarget.IsValidTarget(Spells.E.Range))
            {
                Spells.E.Cast(jngTarget);
            }
        }

        #endregion
    }
}