
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Corki
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.Data.Enumerations;
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
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Vars.Menu["spells"]["q"]["logical"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t => Bools.IsImmobile(t) && !Invulnerable.Check(t) && t.IsValidTarget(Vars.Q.Range)))
                {
                    Vars.Q.Cast(target.ServerPosition);
                }
            }

            /// <summary>
            ///     The Automatic R LastHit Logics.
            /// </summary>
            if (Vars.R.IsReady() && Variables.Orbwalker.ActiveMode != OrbwalkingMode.Combo
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.R.Slot, Vars.Menu["spells"]["r"]["logical"])
                && Vars.Menu["spells"]["r"]["logical"].GetValue<MenuSliderButton>().BValue)
            {
                foreach (var minion in
                    GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Vars.R.Range) && !m.IsValidTarget(Vars.AaRange))
                    )
                {
                    if (Vars.GetRealHealth(minion)
                        < (float)
                          GameObjects.Player.GetSpellDamage(
                              minion,
                              SpellSlot.R,
                              ObjectManager.Player.HasBuff("corkimissilebarragecounterbig")
                                  ? DamageStage.Empowered
                                  : DamageStage.Default))
                    {
                        if (!Vars.R.GetPrediction(minion).CollisionObjects.Any(c => Targets.Minions.Contains(c)))
                        {
                            Vars.R.Cast(minion.ServerPosition);
                        }
                    }
                }
            }
        }

        #endregion
    }
}