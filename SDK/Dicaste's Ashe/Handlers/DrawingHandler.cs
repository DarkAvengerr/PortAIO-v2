using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DicasteAshe.Handlers
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Utils;

    using static MenuHandler;

    using static SpellHandler;

    internal static class DrawingHandler
    {
        private static AIHeroClient Player { get; } = ObjectManager.Player;

        internal static void Init()
        {
            Drawing.OnDraw += OnDraw;
        }

        private static void DrawSpellRange(Spell spell, Color color)
        {
            var spellslot = default(SpellSlot);

            if (spell == Q)
            {
                spellslot = SpellSlot.Q;
            }
            else if (spell == W)
            {
                spellslot = SpellSlot.W;
            }
            else if (spell == E)
            {
                spellslot = SpellSlot.E;
            }
            else if (spell == R)
            {
                spellslot = SpellSlot.R;
            }

            if (MainMenu["Drawings"][string.Concat("Draw", spellslot)] && spell.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, spell.Range, color);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            DrawSpellRange(W, Color.Red);
        }
    }
}