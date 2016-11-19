
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Udyr
{
    using System;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
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
        public static void Combo(EventArgs args)
        {
            if (Bools.HasSheenBuff() || !Targets.Target.IsValidTarget() || Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() && !Targets.Target.HasBuff("udyrbearstuncheck")
                && !Targets.Target.HasBuffOfType(BuffType.Stun)
                && Vars.Menu["spells"]["e"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.E.Cast();
            }
            if (!Targets.Target.HasBuff("udyrbearstuncheck"))
            {
                return;
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (GameObjects.Player.HasBuff("itemmagicshankcharge")
                || GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
            {
                if (Vars.R.IsReady() && GameObjects.Player.GetBuffCount("UdyrPhoenixStance") != 3
                    && Vars.Menu["spells"]["r"]["combo"].GetValue<MenuBool>().Value)
                {
                    Vars.R.Cast();
                }
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            else
            {
                if (Vars.Q.IsReady() && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
                {
                    Vars.Q.Cast();
                }
            }
        }

        #endregion
    }
}