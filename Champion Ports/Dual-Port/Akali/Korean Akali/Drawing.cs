using System;
using System.Drawing;
using KoreanCommon;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAkali
{
    internal class Drawing
    {
        private CommonChampion champion { get; set; }
        private CommonSpells spells { get; set; }

        public Drawing(CommonChampion champion)
        {
            this.champion = champion;
            spells = champion.Spells;

            EloBuddy.Drawing.OnDraw += DrawRanges;
        }

        private void DrawRanges(EventArgs args)
        {
            if (!KoreanUtils.GetParamBool(champion.MainMenu, "drawskillranges"))
            {
                return;
            }

            if (spells.R.IsReady() && spells.R.CanCast())
            {
                Render.Circle.DrawCircle(champion.Player.Position, spells.R.Range, Color.DarkGreen, 5);
            }
            else if (spells.Q.IsReady() && spells.Q.CanCast())
            {
                Render.Circle.DrawCircle(champion.Player.Position, spells.Q.Range, Color.DarkGreen, 5);
            }
            else if (spells.W.IsReady() && spells.W.CanCast())
            {
                Render.Circle.DrawCircle(champion.Player.Position, spells.W.Range, Color.DarkGreen, 5);
            }
            else if (spells.E.IsReady() && spells.E.CanCast())
            {
                Render.Circle.DrawCircle(champion.Player.Position, spells.E.Range, Color.DarkGreen, 5);
            }
        }
    }
}