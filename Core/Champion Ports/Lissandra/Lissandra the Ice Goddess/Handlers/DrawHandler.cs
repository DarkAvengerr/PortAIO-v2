using System;
using LeagueSharp;
using LeagueSharp.Common;
using Lissandra_the_Ice_Goddess.Utility;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Lissandra_the_Ice_Goddess.Handlers
{
    internal class DrawHandler
    {
        public static void OnDraw(EventArgs args)
        {
            var drawOff = Lissandra.Menu.Item("lissandra.drawing.drawingsOff").GetValue<bool>();
            var drawQ = Lissandra.Menu.Item("lissandra.drawing.drawQ").GetValue<Circle>();
            var drawW = Lissandra.Menu.Item("lissandra.drawing.drawW").GetValue<Circle>();
            var drawE = Lissandra.Menu.Item("lissandra.drawing.drawE").GetValue<Circle>();
            var drawR = Lissandra.Menu.Item("lissandra.drawing.drawR").GetValue<Circle>();
            var drawDamage = Lissandra.Menu.Item("lissandra.drawing.drawDamage").GetValue<Circle>();

            if (drawOff || ObjectManager.Player.IsDead)
            {
                return;
            }

            //DamageIndicator.DrawingColor = //DrawDamage.Color;
            //DamageIndicator.Enabled = //DrawDamage.Active;

            if (drawQ.Active)
            {
                if (SkillsHandler.Spells[SpellSlot.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, SkillsHandler.Spells[SpellSlot.Q].Range, Color.Aqua);
                }
            }

            if (drawW.Active)
            {
                if (SkillsHandler.Spells[SpellSlot.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, SkillsHandler.Spells[SpellSlot.W].Range, Color.Aqua);
                }
            }

            if (drawE.Active)
            {
                if (SkillsHandler.Spells[SpellSlot.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, SkillsHandler.Spells[SpellSlot.E].Range, Color.Aqua);
                }
            }

            if (drawR.Active)
            {
                if (SkillsHandler.Spells[SpellSlot.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, SkillsHandler.Spells[SpellSlot.R].Range, Color.Aqua);
                }
            }
        }
    }
}
