// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="LeagueSharp - h3h3">
//   Copyright (C) 2015 h3h3
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   ProFlash, Fail Flash and Insec / Stun Protection
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using EloBuddy;
using LeagueSharp.Common;
namespace ProFlash
{
    using System;
    using System.Threading.Tasks;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class Program
    {
        #region Static Fields and Properties

        private static Menu Menu;
        private static SpellSlot FlashSlot;
        private static Vector3 WallPosition;
        private static Vector3 FlashPosition;
        private static int LastCastAttempt;

        private static bool MenuActive
        {
            get
            {
                return Menu.Item("active").GetValue<bool>();
            }
        }

        private static bool MenuHealth
        {
            get
            {
                return Menu.Item("health").GetValue<Slider>().Value < ObjectManager.Player.HealthPercent;
            }
        }

        private static bool MenuWall
        {
            get
            {
                return Menu.Item("wall").GetValue<bool>();
            }
        }

        private static bool MenuInsec
        {
            get
            {
                return Menu.Item("insec").GetValue<bool>();
            }
        }

        private static bool MenuCondemn
        {
            get
            {
                return Menu.Item("condemn").GetValue<bool>();
            }
        }

        #endregion

        #region Public Methods and Operators

        public static Vector3? GetFirstWallPoint(Vector3 from, Vector3 to, float step = 25)
        {
            var direction = (to - from).Normalized();

            for (float d = 0; d < from.Distance(to); d = d + step)
            {
                var testPoint = from + (d * direction);
                if (testPoint.IsWall())
                {
                    return from + ((d - step) * direction);
                }
            }

            return null;
        }

        #endregion

        #region Methods

        public static void Main()
        {
            FlashSlot = ObjectManager.Player.GetSpellSlot("summonerflash");
            if (FlashSlot == SpellSlot.Unknown)
            {
                Console.WriteLine("ProFlash witout Flash, nice try dude...");
                return;
            }

            Menu = new Menu("ProFlash", "ProFlash", true);
            Menu.AddItem(new MenuItem("active", "Active").SetValue(true));
            Menu.AddItem(new MenuItem("insec", "Insec Protection").SetValue(true));
            Menu.AddItem(new MenuItem("condemn", "Condemn Protection").SetValue(true));
            Menu.AddItem(new MenuItem("wall", "Fail Flash Protection").SetValue(true));
            Menu.AddToMainMenu();

            Spellbook.OnCastSpell += OnCastSpell;
            //Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!MenuActive)
            {
                return;
            }

            if (!MenuWall)
            {
                return;
            }

            if (!sender.Owner.IsValid<AIHeroClient>())
            {
                return;
            }

            if (!sender.Owner.IsMe)
            {
                return;
            }

            if (args.Slot != FlashSlot)
            {
                return;
            }

            if (ObjectManager.Player.Position.Distance(Game.CursorPos) > 850)
            {
                return;
            }

            LastCastAttempt = Utils.TickCount;
            FlashPosition = new Vector3();
            WallPosition = new Vector3();

            var firstWall = GetFirstWallPoint(ObjectManager.Player.Position, Game.CursorPos);

            if (firstWall.HasValue && Game.CursorPos.IsWall()
                && (ObjectManager.Player.Distance(firstWall.Value) > 100
                    && ObjectManager.Player.Distance(firstWall.Value) < 850))
            {
                args.Process = false;

                WallPosition = firstWall.Value;
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, firstWall.Value);

                return;
            }

            var currentPosition = Game.CursorPos;

            for (var distance = ObjectManager.Player.Distance(Game.CursorPos); distance < 850; distance += 50)
            {
                currentPosition = ObjectManager.Player.Position.Extend(Game.CursorPos, distance);

                if (!currentPosition.IsWall())
                {
                    break;
                }
            }

            if (!currentPosition.IsWall())
            {
                FlashPosition = currentPosition;
            }

            if (!FlashPosition.IsZero)
            {
                Console.WriteLine("- FLASH -");
                ObjectManager.Player.Spellbook.CastSpell(FlashSlot, FlashPosition, false);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (LastCastAttempt + 1000 > Utils.TickCount)
            {
                Render.Circle.DrawCircle(FlashPosition, 100, FlashPosition.IsWall() ? Color.Red : Color.Green);
                Render.Circle.DrawCircle(WallPosition, 50, Color.Aqua);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!MenuActive)
            {
                return;
            }

            if (!MenuInsec)
            {
                return;
            }

            if (!args.Target.IsValid<AIHeroClient>())
            {
                return;
            }

            if (!sender.IsValid<AIHeroClient>())
            {
                return;
            }

            if (!args.Target.IsMe)
            {
                return;
            }

            if (args.SData.Name != "BlindMonkRKick")
            {
                return;
            }

            if (!ObjectManager.Player.GetSpell(FlashSlot).IsReady())
            {
                return;
            }

            FlashPosition = ObjectManager.Player.Position.Extend(sender.Position, 450);
            ObjectManager.Player.Spellbook.CastSpell(FlashSlot, FlashPosition, false);
        }

        #endregion
    }
}