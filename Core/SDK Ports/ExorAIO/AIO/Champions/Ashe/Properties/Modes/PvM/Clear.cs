
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Ashe
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void BuildingClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(Variables.Orbwalker.GetTarget() is Obj_HQ) && !(Variables.Orbwalker.GetTarget() is Obj_AI_Turret)
                && !(Variables.Orbwalker.GetTarget() is Obj_BarracksDampener))
            {
                return;
            }

            /// <summary>
            ///     The Q BuildingClear Logic.
            /// </summary>
            if (Vars.Q.IsReady() && GameObjects.Player.HasBuff("asheqcastready")
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["buildings"])
                && Vars.Menu["spells"]["q"]["buildings"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.Q.Cast();
            }
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Clear(EventArgs args)
        {
            if (Bools.HasSheenBuff())
            {
                return;
            }

            /// <summary>
            ///     The Clear Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Items.HasItem(3085) && GameObjects.Player.HasBuff("AsheQCastReady"))
            {
                var target = (Obj_AI_Minion)Variables.Orbwalker.GetTarget();
                /// <summary>
                ///     The JungleClear Q Logic.
                /// </summary>
                if (Targets.JungleMinions.Contains(target)
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["jungleclear"])
                    && Vars.Menu["spells"]["q"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.Q.Cast();
                }

                /// <summary>
                ///     The LaneClear Q Logic.
                /// </summary>
                else if (Targets.Minions.Contains(target)
                         && GameObjects.Player.ManaPercent
                         > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["laneclear"])
                         && Vars.Menu["spells"]["q"]["laneclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.Q.Cast();
                }
            }

            /// <summary>
            ///     The Clear W Logic.
            /// </summary>
            if (Vars.W.IsReady())
            {
                /// <summary>
                ///     The JungleClear W Logic.
                /// </summary>
                if (Targets.JungleMinions.Any()
                    && GameObjects.Player.ManaPercent
                    > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["jungleclear"])
                    && Vars.Menu["spells"]["w"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
                {
                    Vars.W.Cast(Targets.JungleMinions[0].ServerPosition);
                }
                /// <summary>
                ///     The LaneClear W Logic.
                /// </summary>
                else if (Targets.Minions.Any()
                         && GameObjects.Player.ManaPercent
                         > ManaManager.GetNeededMana(Vars.W.Slot, Vars.Menu["spells"]["w"]["laneclear"])
                         && Vars.Menu["spells"]["w"]["laneclear"].GetValue<MenuSliderButton>().BValue)
                {
                    if (Targets.Minions.Count(m => m.Distance(Targets.Minions[0]) < 200f) >= 3)
                    {
                        Vars.W.Cast(Targets.Minions[0].ServerPosition);
                    }
                }
            }
        }

        #endregion
    }
}