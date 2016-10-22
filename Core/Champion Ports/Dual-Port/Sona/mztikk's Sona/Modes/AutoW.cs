using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikkSona.Config;

    internal static class AutoW
    {
        #region Public Methods and Operators

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is AIHeroClient) && !(sender is Obj_AI_Turret)) return;
            if (!sender.IsEnemy || args.Target == null || !Spells.W.IsReady() || !Config.IsChecked("autow.incoming")) return;
            var target = args.Target as AIHeroClient;
            if (target != null && HeroManager.Allies.Any(x => x.NetworkId == target.NetworkId)
                && Config.IsChecked("autoW_" + target.ChampionName)
                && target.Distance(ObjectManager.Player) < Spells.W.Range)
            {
                Spells.W.Cast();
            }
        }

        #endregion

        #region Methods

        internal static void Execute()
        {
            if (!Config.IsChecked("autow.cancelbase") && ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (ObjectManager.Player.InShop())
            {
                return;
            }

            var woundedAlly =
                HeroManager.Allies.Where(
                    ally =>
                    !ally.IsMe && !ally.IsDead && !ally.IsZombie && Config.IsChecked("autoW_" + ally.ChampionName)
                    && ally.Distance(ObjectManager.Player) <= Spells.W.Range)
                    .OrderBy(ally => ally.Health)
                    .FirstOrDefault();
            if (woundedAlly != null && woundedAlly.HealthPercent <= Config.GetSliderValue("allyWhp"))
            {
                Spells.W.Cast();
            }

            if (ObjectManager.Player.HealthPercent <= Config.GetSliderValue("playerWhp"))
            {
                Spells.W.Cast();
            }
        }

        #endregion
    }
}