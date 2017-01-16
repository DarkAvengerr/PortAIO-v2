using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace BadaoActionsLimiter
{
    public static class Program
    {
        public static Menu Config;
        public static void Main()
        {
            Game_OnGameLoad(new EventArgs());
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Config = new Menu("BadaoActionsLimiter", "BadaoActionsLimiter", true);
            Config.AddToMainMenu();
            Config.AddItem(new MenuItem("DrawSpell", "Draw Spell Block").SetValue(true));
            Config.AddItem(new MenuItem("DrawAttack", "Draw Attack Block").SetValue(true));
            Config.AddItem(new MenuItem("DrawMove", "Draw Movement Block").SetValue(true));
            Config.AddItem(new MenuItem("CameraControl", "Camera To Out-Screen Cast Position").SetValue(true));
            SpellBlock.BadaoActivate();
            AttackBlock.BadaoActivate();
            MovementBlock.BadaoActivate();
            CameraControling.BadaoActivate();
            Drawing.OnDraw += Drawing_OnDraw;
            Chat.Print("Badao Actions Limiter Loaded !");
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("DrawSpell").GetValue<bool>())
            {
                Drawing.DrawText(Drawing.Width - 180, 100, System.Drawing.Color.Lime, "Blocked " + SpellBlock.SpellBlockCount + " Spells");
            }
            if (Config.Item("DrawAttack").GetValue<bool>())
            {
                Drawing.DrawText(Drawing.Width - 180, 115, System.Drawing.Color.Lime, "Blocked " + AttackBlock.AttackBlockCount + " Attacks");
            }
            if (Config.Item("DrawMove").GetValue<bool>())
            {
                Drawing.DrawText(Drawing.Width - 180, 130, System.Drawing.Color.Lime, "Blocked " + MovementBlock.MovementBlockCount + " Movements");
            }
        }
    }
}
