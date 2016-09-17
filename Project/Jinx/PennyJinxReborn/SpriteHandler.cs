using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PennyJinxReborn
{
    #region
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    #endregion

    /// <summary>
    /// Handles the R sprite drawing.
    /// </summary>
    internal class SpriteHandler
    {
        /// <summary>
        /// The R sprite instance.
        /// </summary>
        private static Render.Sprite sprite;

        /// <summary>
        /// Gets the current target we will draw the sprite on
        /// </summary>
        private static AIHeroClient RTarget
        {
            get
            {
                var heroList =
                    HeroManager.Enemies.FindAll(
                        h =>
                            h.Health + 20 < PJR.GetSpellsDictionary()[SpellSlot.R].GetDamage(h) &&
                            PJR.GetSpellsDictionary()[SpellSlot.R].CanCast(h)).ToList().OrderBy(h => h.Health);
                return heroList.Any() ? heroList.First() : null;
            }
        }

        /// <summary>
        /// Gets the R target position on the screen.
        /// </summary>
        private static Vector2 RTargetPosition
        {
            get
            {
                return RTarget != null ? 
                    new Vector2(
                        Drawing.WorldToScreen(RTarget.Position).X - RTarget.BoundingRadius * 2 +
                        RTarget.BoundingRadius / 1.5f,
                        Drawing.WorldToScreen(RTarget.Position).Y - RTarget.BoundingRadius * 2) : new Vector2();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the sprite should be drawn or not.
        /// </summary>
        private static bool DrawCondition
        {
              get { return RTarget != null && Render.OnScreen(Drawing.WorldToScreen(RTarget.Position)) && PJR.GetSpellsDictionary()[SpellSlot.R].IsReady() && PJR.GetMenu().Item("dz191." + PJR.GetMenuName() + ".drawings.rsprite").GetValue<bool>(); }
        }

        /// <summary>
        /// Initializes the sprite reference. To be called when the assembly is loaded.
        /// </summary>
        internal static void InitalizeSprite()
        {
        }
    }
}
