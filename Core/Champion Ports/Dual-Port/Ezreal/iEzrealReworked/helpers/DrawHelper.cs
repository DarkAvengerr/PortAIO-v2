using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iEzrealReworked.helpers
{
    class DrawHelper
    {
        public static void DrawSpellsRanges(Dictionary<SpellSlot, Spell> spells)
        {
            foreach (var spell in spells.Where(s => iEzrealReworked.Menu.Item("com.iezreal.drawing.draw" + MenuHelper.GetStringFromSpellSlot(s.Key)).GetValue<Circle>().Active))
            {
                var value = iEzrealReworked.Menu.Item("com.iezreal.drawing.draw" + MenuHelper.GetStringFromSpellSlot(spell.Key)).GetValue<Circle>();
                if (spell.Value.Range < 4000f)
                {
                    Render.Circle.DrawCircle(
                    ObjectManager.Player.Position, spell.Value.Range,
                    spell.Value.IsReady() ? value.Color : Color.DarkRed);
                }
            }
        }
    }
}
