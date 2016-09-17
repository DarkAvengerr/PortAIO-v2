using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Events
    {
        #region Properties

        internal static bool IsAutoAttacking => Attacking;

        private static bool Attacking { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            Attacking = false;
        }

        public static void OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            Attacking = true;
        }

        public static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (!sender.IsMe || args.Buff.Name != "pantheonesound")
            {
                return;
            }

            Orbwalking.Attack = true;
            Orbwalking.Move = true;
            Mainframe.CancelEverything = false;
        }

        public static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsMe || sender.IsAlly || !Config.IsChecked("misc.interrupt.w") || !Spells.W.IsReady()
                || !sender.IsValidTarget(Spells.W.Range))
            {
                return;
            }

            Spells.W.Cast(sender);
        }

        public static void OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            Attacking = false;
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || args.Slot != SpellSlot.E)
            {
                return;
            }

            Orbwalking.Attack = false;
            Orbwalking.Move = false;
            Mainframe.CancelEverything = true;
        }

        #endregion
    }
}