
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Orianna
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

    using SharpDX;

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
            var possiblePosition1 =
                ObjectManager.Get<Obj_AI_Minion>()
                    .FirstOrDefault(m => Math.Abs(m.Health) > 0 && m.BaseSkinName.Equals("oriannaball"));
            var possiblePosition2 =
                GameObjects.AllyHeroes.FirstOrDefault(
                    a => a.Buffs.Any(b => b.Caster.IsMe && b.Name.Equals("orianaghost")));

            Orianna.GetBallPosition = possiblePosition1?.ServerPosition ?? (GameObjects.Player.HasBuff("orianaghostself") ? GameObjects.Player.ServerPosition : possiblePosition2?.ServerPosition ?? Vector3.Zero);

            if (GameObjects.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Automatic R Logic.
            /// </summary>
            if (Vars.R.IsReady() && GameObjects.EnemyHeroes.Count(
                // ReSharper disable once PossibleInvalidOperationException
                t => t.IsValidTarget() && t.Distance((Vector2)Orianna.GetBallPosition) < Vars.R.Range - 60f)
                >= Vars.Menu["spells"]["r"]["aoe"].GetValue<MenuSliderButton>().SValue
                && Vars.Menu["spells"]["r"]["aoe"].GetValue<MenuSliderButton>().BValue)
            {
                Vars.R.Cast();
            }
        }

        #endregion
    }
}