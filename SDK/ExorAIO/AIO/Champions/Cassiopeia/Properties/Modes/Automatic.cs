
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Cassiopeia
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Tear Stacking Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Bools.HasTear(GameObjects.Player) && !GameObjects.Player.IsRecalling()
                && Variables.Orbwalker.ActiveMode == OrbwalkingMode.None
                && GameObjects.Player.CountEnemyHeroesInRange(1500) == 0
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["miscellaneous"]["tear"])
                && Vars.Menu["miscellaneous"]["tear"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.Q.Cast(GameObjects.Player.ServerPosition.Extend(Game.CursorPos, Vars.Q.Range - 5f));
            }

            /// <summary>
            ///     The Automatic Logics.
            /// </summary>
            foreach (var target in
                GameObjects.EnemyHeroes.Where(
                    t => Bools.IsImmobile(t) && !Invulnerable.Check(t, DamageType.Magical, false)))
            {
                /// <summary>
                ///     The Automatic W Logic.
                /// </summary>
                if (Vars.W.IsReady() && target.IsValidTarget(Vars.W.Range)
                    && !target.IsValidTarget(500)
                    && Vars.Menu["spells"]["w"]["logical"].GetValue<MenuBool>().Value)
                {
                    Vars.W.Cast(target.ServerPosition);
                }
            }

            /// <summary>
            ///     The Semi-Automatic R Logic.
            /// </summary>
            if (Vars.R.IsReady() && Vars.Menu["spells"]["r"]["bool"].GetValue<MenuBool>().Value
                && Vars.Menu["spells"]["r"]["key"].GetValue<MenuKeyBind>().Active)
            {
                var target = GameObjects.EnemyHeroes.Where(
                    t =>
                    t.IsValidTarget(Vars.R.Range - 100f) && t.IsFacing(GameObjects.Player)
                    && !Invulnerable.Check(t, DamageType.Magical, false)
                    && Vars.Menu["spells"]["r"]["whitelist"][t.ChampionName.ToLower()]
                           .GetValue<MenuBool>().Value).OrderBy(o => o.Health).FirstOrDefault();
                if (target != null)
                {
                    Vars.R.Cast(target.ServerPosition);
                }
            }
        }

        #endregion
    }
}