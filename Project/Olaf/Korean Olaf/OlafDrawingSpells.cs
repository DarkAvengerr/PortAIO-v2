using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanOlaf
{
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    class OlafDrawingSpells
    {
        private readonly OlafMenu olafMenu;

        private readonly OlafSpells olafSpells;

        public OlafDrawingSpells(OlafMenu olafMenu, OlafSpells olafSpells)
        {
            this.olafMenu = olafMenu;
            this.olafSpells = olafSpells;

            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Drawing_OnDraw(System.EventArgs args)
        {
            if (!olafMenu.GetParamBool("koreanolaf.drawing.skillranges"))
            {
                return;
            }

            OlafSpell olafSpell = null;

            if (olafSpells.Q.UseOnCombo && olafSpells.Q.IsReady())
            {
                olafSpell = olafSpells.Q;
            }
            else if (olafSpells.E.UseOnCombo && olafSpells.E.IsReady())
            {
                olafSpell = olafSpells.E;
            }

            if (olafSpell != null)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, olafSpell.Range, Color.FromArgb(150, Color.DarkGreen), 5);
            }
        }
    }
}
