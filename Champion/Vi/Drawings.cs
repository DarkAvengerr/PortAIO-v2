using EloBuddy; namespace ElVi
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Drawings
    {
        #region Public Methods and Operators

        public static void Drawing_OnDraw(EventArgs args)
        {
            var drawOff = ElViMenu._menu.Item("ElVi.Draw.off").GetValue<bool>();
            var drawQ = ElViMenu._menu.Item("ElVi.Draw.Q").GetValue<Circle>();
            var drawE = ElViMenu._menu.Item("ElVi.Draw.E").GetValue<Circle>();
            var drawR = ElViMenu._menu.Item("ElVi.Draw.R").GetValue<Circle>();

            if (drawOff)
            {
                return;
            }

            if (drawQ.Active)
            {
                if (Vi.Spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Vi.Spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawE.Active)
            {
                if (Vi.Spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Vi.Spells[Spells.E].Range, Color.White);
                }
            }

            if (drawR.Active)
            {
                if (Vi.Spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Vi.Spells[Spells.R].Range, Color.White);
                }
            }

            var target = TargetSelector.GetTarget(Vi.Spells[Spells.Q].Range, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                Render.Circle.DrawCircle(target.Position, 50, Color.Yellow);
            }
        }

        #endregion
    }
}
