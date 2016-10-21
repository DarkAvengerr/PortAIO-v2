using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona.Modes
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikkSona.Extensions;

    using Config = mztikkSona.Config;

    internal static class Flee
    {
        #region Methods

        internal static void Execute()
        {
            if (Config.IsChecked("slowAttack") && ObjectManager.Player.HasBuff("sonapassiveattack") && Spells.LastSpellSlot == SpellSlot.E)
            {
                var target = TargetSelector.GetTarget(
                    ObjectManager.Player.AttackRange, 
                    TargetSelector.DamageType.Magical);
                if (target != null)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
            }

            if (Config.IsChecked("fleeE") && Spells.E.CanCast())
            {
                Spells.E.Cast();
            }
        }

        #endregion
    }
}