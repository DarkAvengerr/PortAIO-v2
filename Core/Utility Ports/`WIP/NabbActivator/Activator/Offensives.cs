
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbActivator
{
    using System.Linq;

    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

    /// <summary>
    ///     The activator class.
    /// </summary>
    internal partial class Activator
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Called on orbwalker action.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="OrbwalkingActionArgs" /> instance containing the event data.</param>
        public static void Offensives(object sender, OrbwalkingActionArgs args)
        {
            if (!Vars.Menu["offensives"].GetValue<MenuBool>().Value
                || !Vars.Menu["keys"]["combo"].GetValue<MenuKeyBind>().Active)
            {
                return;
            }

            /// <summary>
            ///     The Bilgewater Cutlass Logic.
            /// </summary>
            if (Items.CanUseItem(3144) && Targets.Target.IsValidTarget(550f + GameObjects.Player.BoundingRadius))
            {
                Items.UseItem(3144, Targets.Target);
            }

            /// <summary>
            ///     The Blade of the Ruined King Logic.
            /// </summary>
            if (Items.CanUseItem(3153) && Targets.Target.IsValidTarget(550f + GameObjects.Player.BoundingRadius)
                && GameObjects.Player.HealthPercent <= 90)
            {
                Items.UseItem(3153, Targets.Target);
            }

            /// <summary>
            ///     The Entropy Logic.
            /// </summary>     
            if (Items.CanUseItem(3184))
            {
                Items.UseItem(3184);
            }

            /// <summary>
            ///     The Frost Queen's Claim Logic.
            /// </summary>
            if (Items.CanUseItem(3092))
            {
                if (
                    GameObjects.EnemyHeroes.Count(
                        t =>
                        t.IsValidTarget(4000f)
                        && t.CountEnemyHeroesInRange(1500f)
                        <= GameObjects.Player.CountAllyHeroesInRange(1500f) + t.CountAllyHeroesInRange(1500f) - 1) >= 1)
                {
                    Items.UseItem(3092);
                }
            }

            /// <summary>
            ///     The Hextech Gunblade Logic.
            /// </summary>
            if (Items.CanUseItem(3146) && Targets.Target.IsValidTarget(700f + GameObjects.Player.BoundingRadius))
            {
                Items.UseItem(3146, Targets.Target);
            }

            /// <summary>
            ///     The Youmuu's Ghostblade Logic.
            /// </summary>
            if (Items.CanUseItem(3142))
            {
                Items.UseItem(3142);
            }

            /// <summary>
            ///     The Hextech GLP-800 Logic.
            /// </summary>
            if (Items.CanUseItem(3030) && Targets.Target.IsValidTarget(800f + GameObjects.Player.BoundingRadius))
            {
                Items.UseItem(3030, Targets.Target.ServerPosition);
            }

            /// <summary>
            ///     The Hextech Protobelt Logic.
            /// </summary>
            if (Items.CanUseItem(3152))
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        t.IsValidTarget(850f + GameObjects.Player.BoundingRadius)
                        && t.CountEnemyHeroesInRange(200f) >= 3))
                {
                    Items.UseItem(3152, target.ServerPosition);
                }
            }
        }

        #endregion
    }
}