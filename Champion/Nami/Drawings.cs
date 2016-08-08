namespace ElNamiBurrito
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class Drawings
    {
        #region Public Methods and Operators

        public static void Drawing_OnDraw(EventArgs args)
        {
            if (ElNamiMenu.Menu.Item("ElNamiReborn.Draw.off").GetValue<bool>())
            {
                return;
            }

            var drawQRange = ElNamiMenu.Menu.Item("ElNamiReborn.Draw.Q").GetValue<Circle>();
            var drawWRange = ElNamiMenu.Menu.Item("ElNamiReborn.Draw.W").GetValue<Circle>();
            var drawERange = ElNamiMenu.Menu.Item("ElNamiReborn.Draw.E").GetValue<Circle>();
            var drawRRange = ElNamiMenu.Menu.Item("ElNamiReborn.Draw.R").GetValue<Circle>();

            if (drawQRange.Active)
            {
                if (Nami.spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Nami.spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawERange.Active)
            {
                if (Nami.spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Nami.spells[Spells.E].Range, Color.White);
                }
            }

            if (drawWRange.Active)
            {
                if (Nami.spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Nami.spells[Spells.W].Range, Color.White);
                }
            }

            if (drawRRange.Active)
            {
                if (Nami.spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Nami.spells[Spells.R].Range, Color.White);
                }
            }
        }

        #endregion
    }
}