
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbActivator
{
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The drawings class.
    /// </summary>
    internal class Drawings
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Loads the drawings.
        /// </summary>
        public static void Initialize()
        {
            Drawing.OnDraw += delegate
                {
                    /// <summary>
                    ///     Loads the Smite drawing.
                    /// </summary>
                    if (Vars.Smite.Slot != SpellSlot.Unknown
                        && Vars.Menu["keys"]["smite"].GetValue<MenuKeyBind>().Active)
                    {
                        if (Vars.Menu["smite"]["drawings"]["range"].GetValue<MenuBool>().Value)
                        {
                            Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.Smite.Range, Color.Orange, 1);
                        }
                    }
                };
        }

        #endregion
    }
}