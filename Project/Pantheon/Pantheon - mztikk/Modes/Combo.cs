using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon.Modes
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikkPantheon.Config;

    internal static class Combo
    {
        #region Public Methods and Operators

        public static void OnPostAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Mainframe.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || !Config.IsChecked("combo.e")
                || !Config.IsChecked("combo.w") || !Spells.E.IsReady() || Spells.Q.IsReady() || Spells.W.IsReady())
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
            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Physical);
            if (target == null || target.IsInvulnerable)
            {
                return;
            }

            if (Config.IsChecked("combo.q") && Spells.Q.IsReady() && target.IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.Cast(target);
            }

            if (Config.IsChecked("combo.w") && Spells.W.IsReady() && target.IsValidTarget(Spells.W.Range))
            {
                if (Config.IsChecked("combo.q"))
                {
                    if (!Spells.Q.IsReady())
                    {
                        Spells.W.Cast(target);
                    }
                }
                else
                {
                    Spells.W.Cast(target);
                }
            }

            if (Config.IsChecked("combo.e") && !Config.IsChecked("combo.w") && target.IsMovementImpaired()
                && Spells.E.IsReady() && target.IsValidTarget(Spells.E.Range))
            {
                var ePred = Spells.E.GetPrediction(target, true);
                if (ePred.Hitchance >= HitChance.VeryHigh)
                {
                    Spells.E.Cast(ePred.CastPosition);
                }
            }

            if (Config.IsChecked("combo.tiamat") && Orbwalking.InAutoAttackRange(target) && !Events.IsAutoAttacking)
            {
                if (Spells.Tiamat.IsOwned() && Spells.Tiamat.IsReady())
                {
                    Spells.Tiamat.Cast();
                }

                if (Spells.RavHydra.IsOwned() && Spells.RavHydra.IsReady())
                {
                    Spells.RavHydra.Cast();
                }

                if (Spells.TitHydra.IsOwned() && Spells.TitHydra.IsReady())
                {
                    Spells.TitHydra.Cast();
                }
            }
        }

        #endregion
    }
}