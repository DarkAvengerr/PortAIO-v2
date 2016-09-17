using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkXinZhao.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikkXinZhao.Extensions;

    using Config = mztikkXinZhao.Config;

    internal static class LaneClear
    {
        #region Public Methods and Operators

        public static void LaneOnPostAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Mainframe.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || ObjectManager.Player.ManaPercent < Config.GetSliderValue("lcMana"))
            {
                return;
            }

            var minz =
                MinionManager.GetMinions(
                    ObjectManager.Player.Position, 
                    Spells.E.Range, 
                    MinionTypes.All, 
                    MinionTeam.Enemy).OrderByDescending(m => m.MaxHealth);
            var minion = minz.FirstOrDefault();
            if (minion == null)
            {
                return;
            }

            var combinedHealth =
                minz.Where(m => m.Distance(ObjectManager.Player.Position) <= 350).Sum(targut => targut.Health);
            if (Spells.W.CanCast() && combinedHealth >= ObjectManager.Player.GetAutoAttackDamage(minion) * 4
                && Config.IsChecked("useWLC"))
            {
                Spells.W.Cast();
            }

            if (Spells.Q.CanCast() && combinedHealth >= ObjectManager.Player.GetAutoAttackDamage(minion) * 4
                && Config.IsChecked("useQLC"))
            {
                Spells.Q.Cast();
            }
        }

        #endregion

        #region Methods

        internal static void Execute()
        {
            var minz =
                MinionManager.GetMinions(
                    ObjectManager.Player.Position, 
                    Spells.E.Range, 
                    MinionTypes.All, 
                    MinionTeam.Enemy).OrderByDescending(m => m.MaxHealth);
            var minion = minz.FirstOrDefault();
            if (minion == null)
            {
                return;
            }

            foreach (var mina in minz)
            {
                var minAoE = MinionManager.GetMinions(mina.Position, 100);
                if (minAoE.Count() >= Config.GetSliderValue("lcEtargets") && Config.IsChecked("useELC")
                    && Spells.E.CanCast(mina))
                {
                    Spells.E.Cast(mina);
                }
            }
        }

        #endregion
    }
}