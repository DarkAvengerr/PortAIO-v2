using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Wukong.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = Wukong.Config;

    internal static class LaneClear
    {
        #region Public Methods and Operators

        public static void OnPostAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Mainframe.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || ObjectManager.Player.ManaPercent < Config.GetSliderValue("laneclear.mana")) return;
            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range);
            if (!minions.Any())
            {
                return;
            }

            var minionsHp = minions.Sum(x => x.Health);
            if (minionsHp < ObjectManager.Player.TotalAttackDamage * 3)
            {
                return;
            }

            if (Config.IsChecked("laneclear.q") && Spells.Q.IsReady())
            {
                Spells.Q.Cast();
            }
        }

        #endregion

        #region Methods

        internal static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < Config.GetSliderValue("laneclear.mana")
                || ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.E.Range);
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

            if (Config.IsChecked("laneclear.e") && Spells.E.IsReady())
            {
                Spells.E.Cast(target);
            }
        }

        #endregion
    }
}