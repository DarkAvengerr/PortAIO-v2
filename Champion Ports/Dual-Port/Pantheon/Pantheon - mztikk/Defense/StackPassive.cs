using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon.Defense
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikkPantheon.Config;

    internal static class StackPassive
    {
        #region Public Methods and Operators

        public static void OnEnemyAa(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy || !sender.IsChampion() || !args.SData.IsAutoAttack() || args.Target == null
                || !args.Target.IsMe || ObjectManager.Player.HasBuff("PantheonPassiveShield")
                || !Config.IsChecked("misc.stackpassive.q"))
            {
                return;
            }

            var buff = ObjectManager.Player.GetBuff("PantheonPassiveCounter");
            if (buff?.Count != 3) return;
            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Physical)
                         ?? MinionManager.GetMinions(
                             ObjectManager.Player.Position, 
                             Spells.Q.Range, 
                             MinionTypes.All, 
                             MinionTeam.NotAlly).OrderBy(x => x.Distance(ObjectManager.Player)).FirstOrDefault();

            if (target == null || target.IsInvulnerable || !Spells.Q.IsReady() || !target.IsValidTarget(Spells.Q.Range))
            {
                return;
            }

            Spells.Q.Cast(target);
        }

        #endregion

        #region Methods

        internal static void Execute()
        {
        }

        #endregion
    }
}