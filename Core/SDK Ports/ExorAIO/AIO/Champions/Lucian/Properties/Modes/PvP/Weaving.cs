
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Lucian
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
        /// <param name="args">The args.</param>
        public static void Weaving(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient) || Invulnerable.Check((AIHeroClient)args.Target))
            {
                return;
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady())
            {
                if (!Game.CursorPos.IsUnderEnemyTurret()
                    || ((AIHeroClient)args.Target).Health
                    < GameObjects.Player.GetAutoAttackDamage((AIHeroClient)args.Target) * 2)
                {
                    switch (Vars.Menu["spells"]["e"]["mode"].GetValue<MenuList>().Index)
                    {
                        case 0:
                            Vars.E.Cast(
                                GameObjects.Player.ServerPosition.Extend(
                                    Game.CursorPos,
                                    GameObjects.Player.Distance(Game.CursorPos)
                                    < GameObjects.Player.GetRealAutoAttackRange()
                                        ? GameObjects.Player.BoundingRadius
                                        : 475f));
                            break;
                        case 1:
                            Vars.E.Cast(GameObjects.Player.ServerPosition.Extend(Game.CursorPos, 475f));
                            break;
                        case 2:
                            Vars.E.Cast(
                                GameObjects.Player.ServerPosition.Extend(
                                    Game.CursorPos,
                                    GameObjects.Player.BoundingRadius));
                            break;
                    }

                    return;
                }
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() && ((AIHeroClient)args.Target).IsValidTarget(Vars.Q.Range)
                && Vars.Menu["spells"]["q"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.Q.CastOnUnit((AIHeroClient)args.Target);
                return;
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() && ((AIHeroClient)args.Target).IsValidTarget(Vars.W.Range)
                && Vars.Menu["spells"]["w"]["combo"].GetValue<MenuBool>().Value)
            {
                Vars.W.Cast(Vars.W.GetPrediction((AIHeroClient)args.Target).UnitPosition);
            }
        }

        #endregion
    }
}