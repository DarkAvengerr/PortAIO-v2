namespace Elvarus
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
            var drawOff = ElVarusMenu.Menu.Item("ElVarus.Draw.off").GetValue<bool>();
            var drawQ = ElVarusMenu.Menu.Item("ElVarus.Draw.Q").GetValue<Circle>();
            var drawW = ElVarusMenu.Menu.Item("ElVarus.Draw.W").GetValue<Circle>();
            var drawE = ElVarusMenu.Menu.Item("ElVarus.Draw.E").GetValue<Circle>();
            var drawR = ElVarusMenu.Menu.Item("ElVarus.Draw.E").GetValue<Circle>();

            if (drawOff)
            {
                return;
            }

            if (drawQ.Active)
            {
                if (Varus.spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Varus.spells[Spells.Q].Range,
                        Varus.spells[Spells.Q].LSIsReady() ? Color.Green : Color.Red);
                }
            }

            if (drawW.Active)
            {
                if (Varus.spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Varus.spells[Spells.W].Range,
                        Varus.spells[Spells.W].LSIsReady() ? Color.Green : Color.Red);
                }
            }

            if (drawE.Active)
            {
                if (Varus.spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Varus.spells[Spells.E].Range,
                        Varus.spells[Spells.E].LSIsReady() ? Color.Green : Color.Red);
                }
            }

            if (drawR.Active)
            {
                if (Varus.spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Varus.spells[Spells.R].Range,
                        Varus.spells[Spells.R].LSIsReady() ? Color.Green : Color.Red);
                }
            }
        }

        #endregion
    }
}