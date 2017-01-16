using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using SharpDX;
using Color = System.Drawing.Color;
using ClipperLib;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace BadaoActionsLimiter
{
    public static class CameraControling
    {
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);
        public static void BadaoActivate()
        {

        }
        public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!Program.Config.Item("CameraControl").GetValue<bool>())
                return;
            if (args.StartPosition.Distance(ObjectManager.Player.Position) <= 20)
                return;

            Vector3 start = args.StartPosition;

            if (ObjectManager.Player.ChampionName == "Yasuo" && args.Slot == SpellSlot.W)
                start = ObjectManager.Player.Position.Extend(args.StartPosition, 250);

            if (!start.IsOnScreen())
            {
                var pos = Camera.ScreenPosition;
                Vector3 NewCameraPos = new Vector3(start.X + _random.Next(400), start.Y + _random.Next(400),Camera.ScreenPosition.To3D().Z);
                Camera.ScreenPosition = NewCameraPos.To2D();
                LeagueSharp.Common.Utility.DelayAction.Add(300, () => Camera.ScreenPosition = pos);
            }
        }
    }
}
