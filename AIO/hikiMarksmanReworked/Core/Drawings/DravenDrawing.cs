using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hikiMarksmanRework.Core.Menus;
using hikiMarksmanRework.Core.Spells;
using hikiMarksmanRework.Core.Utilitys;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Core.Drawings
{
    class DravenDrawing
    {
        private static readonly AIHeroClient Player = ObjectManager.Player;
        public static void Init()
        {
            if (DravenMenu.Config.Item("DCR").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(Game.CursorPos, 600, DravenMenu.Config.Item("DCR").GetValue<Circle>().Color);
            }

            if (DravenMenu.Config.Item("DE").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(Player.Position, DravenSpells.E.Range, DravenMenu.Config.Item("DE").GetValue<Circle>().Color);
            }

            if (DravenMenu.Config.Item("DR").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(Player.Position, DravenSpells.R.Range, DravenMenu.Config.Item("DR").GetValue<Circle>().Color);
            }

            if (DravenMenu.Config.Item("DAR").GetValue<Circle>().Active)
            {
                foreach (var axe in DravenAxeHelper.AxeSpots.Where(x => x.AxeObj.IsVisible && x.AxeObj.Position.LSDistance(ObjectManager.Player.Position) < 1000))
                {
                    Drawing.DrawText(Drawing.WorldToScreen(axe.AxeObj.Position).X - 40, Drawing.WorldToScreen(axe.AxeObj.Position).Y, Color.Gold, (((float)(axe.EndTick - Environment.TickCount))) + " ms");
                    Render.Circle.DrawCircle(axe.AxeObj.Position, 120, DravenAxeHelper.InCatchRadius(axe) ? DravenMenu.Config.Item("DAR").GetValue<Circle>().Color : Color.Gold);
                }
            }
        }
    }
}
