using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon.Modes
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikkPantheon.Config;

    internal static class Harass
    {
        #region Public Methods and Operators

        public static void OnPostAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Mainframe.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed
                || ObjectManager.Player.ManaPercent < Config.GetSliderValue("harass.mana")
                || !Config.IsChecked("harass.e") || !Config.IsChecked("harass.w") || !Spells.E.IsReady()
                || Spells.Q.IsReady() || Spells.W.IsReady())
            {
                return;
            }

            var comboTarget = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Physical);
            if (comboTarget == null || comboTarget.IsInvulnerable || !comboTarget.IsMovementImpaired()
                || comboTarget.Health < ObjectManager.Player.GetAutoAttackDamage(comboTarget))
            {
                return;
            }

            Spells.E.Cast(comboTarget);
        }

        #endregion

        #region Methods

        internal static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < Config.GetSliderValue("harass.mana") || Events.IsAutoAttacking)
            {
                return;
            }

            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Physical);
            if (target == null || target.IsInvulnerable)
            {
                return;
            }

            if (Config.IsChecked("harass.q") && Spells.Q.IsReady() && target.IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.Cast(target);
            }

            if (Config.IsChecked("harass.w") && Spells.W.IsReady() && target.IsValidTarget(Spells.W.Range))
            {
                Spells.W.Cast(target);
            }

            if (Config.IsChecked("harass.e") && !Config.IsChecked("harass.w") && target.IsMovementImpaired()
                && Spells.E.IsReady() && target.IsValidTarget(Spells.E.Range))
            {
                var ePred = Spells.E.GetPrediction(target, true);
                if (ePred.Hitchance >= HitChance.VeryHigh)
                {
                    Spells.E.Cast(ePred.CastPosition);
                }
            }
        }

        #endregion
    }
}