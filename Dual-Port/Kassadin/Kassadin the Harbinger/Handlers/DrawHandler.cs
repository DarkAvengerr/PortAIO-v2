using System;
using LeagueSharp;
using LeagueSharp.Common;
using Kassadin_the_Harbinger.Utility;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kassadin_the_Harbinger.Handlers
{
    internal class DrawHandler
    {
        public static void OnDraw(EventArgs args)
        {
            var drawOff = Kassadin.Menu.Item("kassadin.drawing.drawingsOff").GetValue<bool>();
            var drawQ = Kassadin.Menu.Item("kassadin.drawing.drawQ").GetValue<Circle>();
            var drawE = Kassadin.Menu.Item("kassadin.drawing.drawE").GetValue<Circle>();
            var drawR = Kassadin.Menu.Item("kassadin.drawing.drawR").GetValue<Circle>();
            var drawDamage = Kassadin.Menu.Item("kassadin.drawing.drawDamage").GetValue<Circle>();

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
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, SkillsHandler.Spells[SpellSlot.Q].Range, Color.DarkOrange);
                }
            }

            if (drawE.Active)
            {
                if (SkillsHandler.Spells[SpellSlot.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, SkillsHandler.Spells[SpellSlot.E].Range, Color.DarkOrange);
                }
            }

            if (drawR.Active)
            {
                if (SkillsHandler.Spells[SpellSlot.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, SkillsHandler.Spells[SpellSlot.R].Range, Color.DarkOrange);
                }
            }
        }
    }
}
