using System;
using LeagueSharp;
using LeagueSharp.Common;
using BlackZilean.Utility;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BlackZilean.Handlers
{
    internal class DrawHandler
    {
        public static void OnDraw(EventArgs args)
        {
            var drawOff = Zilean.Menu.Item("zilean.drawing.drawingsOff").GetValue<bool>();
            var drawQ = Zilean.Menu.Item("zilean.drawing.drawQ").GetValue<Circle>();
            var drawE = Zilean.Menu.Item("zilean.drawing.drawE").GetValue<Circle>();
            var drawR = Zilean.Menu.Item("zilean.drawing.drawR").GetValue<Circle>();
            var drawDamage = Zilean.Menu.Item("zilean.drawing.drawDamage").GetValue<Circle>();

            if (drawOff || ObjectManager.Player.IsDead)
            {
                return;
            }

            DamageIndicator.DrawingColor = drawDamage.Color;
            DamageIndicator.Enabled = drawDamage.Active;

            if (drawQ.Active)
            {
                if (SkillsHandler.Spells[SpellSlot.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, SkillsHandler.Spells[SpellSlot.Q].Range, Color.Crimson);
                }
            }

            if (drawE.Active)
            {
                if (SkillsHandler.Spells[SpellSlot.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, SkillsHandler.Spells[SpellSlot.E].Range, Color.Crimson);
                }
            }

            if (drawR.Active)
            {
                if (SkillsHandler.Spells[SpellSlot.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, SkillsHandler.Spells[SpellSlot.R].Range, Color.Crimson);
                }
            }
        }
    }
}
