
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Vayne
{
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using SharpDX;

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
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
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
            if (Vars.Q.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["buildings"])
                && Vars.Menu["spells"]["q"]["buildings"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.Q.Cast(Game.CursorPos);
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        public static void Clear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            /// <summary>
            ///     The Q FarmHelper Logic.
            /// </summary>
            if (Vars.Q.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["farmhelper"])
                && Vars.Menu["spells"]["q"]["farmhelper"].GetValue<MenuSliderButton>().BValue)
            {
                if (Targets.Minions.Any()
                    && Targets.Minions.Count(
                        m =>
                        Vars.GetRealHealth(m)
                        < GameObjects.Player.GetAutoAttackDamage(m)
                        + (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q)
                        && m.Distance(GameObjects.Player.ServerPosition.Extend(Game.CursorPos, 300f))
                        < GameObjects.Player.GetRealAutoAttackRange()) > 1)
                {
                    Vars.Q.Cast(Game.CursorPos);
                }
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        public static void JungleClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(Variables.Orbwalker.GetTarget() is Obj_AI_Minion)
                || !Targets.JungleMinions.Contains(Variables.Orbwalker.GetTarget() as Obj_AI_Minion))
            {
                return;
            }

            /// <summary>
            ///     The E JungleClear Logic.
            /// </summary>
            if (Vars.E.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.E.Slot, Vars.Menu["spells"]["e"]["jungleclear"])
                && Vars.Menu["spells"]["e"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
            {
                var target = (Obj_AI_Minion)Variables.Orbwalker.GetTarget();
                if (!target.BaseSkinName.Contains("SRU_Dragon")
                    && !target.BaseSkinName.Equals("SRU_Baron")
                    && !target.BaseSkinName.Equals("SRU_Riftherald"))
                {
                    for (var i = 1; i < 10; i++)
                    {
                        var position = target.ServerPosition;
                        var prediction = Vars.E.GetPrediction(target).UnitPosition;
                        var prediction2 = Vars.E2.GetPrediction(target).UnitPosition;
                        var vector = Vector3.Normalize(target.ServerPosition - GameObjects.Player.ServerPosition);
                        if ((position + vector * (i * 42)).IsWall() && (position + vector * (i * 45)).IsWall()
                            && (prediction + vector * (i * 42)).IsWall() && (prediction + vector * (i * 45)).IsWall()
                            && (prediction2 + vector * (i * 42)).IsWall() && (prediction2 + vector * (i * 45)).IsWall())
                        {
                            Vars.E.CastOnUnit(target);
                            return;
                        }
                    }
                }
            }

            /// <summary>
            ///     The Q JungleClear Logic.
            /// </summary>
            if (Vars.Q.IsReady()
                && GameObjects.Player.ManaPercent
                > ManaManager.GetNeededMana(Vars.Q.Slot, Vars.Menu["spells"]["q"]["jungleclear"])
                && Vars.Menu["spells"]["q"]["jungleclear"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.Q.Cast(Game.CursorPos);
            }
        }

        #endregion
    }
}