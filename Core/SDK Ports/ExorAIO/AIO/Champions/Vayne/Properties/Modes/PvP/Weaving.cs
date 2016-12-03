
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Vayne
{
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
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        public static void Weaving(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient) || Invulnerable.Check((AIHeroClient)args.Target))
            {
                return;
            }

            /// <summary>
            ///     The Q Weaving Logic.
            /// </summary>
            if (Vars.Q.IsReady() && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
            {
                if (Vars.Menu["miscellaneous"]["wstacks"].GetValue<MenuBool>().Value
                    && ((AIHeroClient)args.Target).GetBuffCount("vaynesilvereddebuff") != 1)
                {
                    return;
                }

                if (!Vars.Menu["miscellaneous"]["alwaysq"].GetValue<MenuBool>().Value)
                {
                    var posAfterQ = GameObjects.Player.ServerPosition.Extend(Game.CursorPos, 300f);
                    if (GameObjects.Player.Distance(Game.CursorPos) > GameObjects.Player.GetRealAutoAttackRange()
                        && posAfterQ.CountEnemyHeroesInRange(1000f) < 3
                        && ((AIHeroClient)args.Target).Distance(posAfterQ) < GameObjects.Player.GetRealAutoAttackRange())
                    {
                        Vars.Q.Cast(Game.CursorPos);
                    }
                    return;
                }

                Vars.Q.Cast(Game.CursorPos);
            }
        }

        #endregion
    }
}