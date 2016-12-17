using EloBuddy; 
using LeagueSharp.Common; 
namespace ElLeeSin.Components.SpellManagers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElLeeSin.Components;
    using ElLeeSin.Utilities;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using Geometry = ElLeeSin.Geometry;

    internal class JumpHandler
    {
        #region Static Fields

        public static bool InitQ;

        private static readonly List<Vector3> JunglePos = new List<Vector3>
                                                              {
                                                                  new Vector3(6271.479f, 12181.25f, 56.47668f),
                                                                  new Vector3(6971.269f, 10839.12f, 55.2f),
                                                                  new Vector3(8006.336f, 9517.511f, 52.31763f),
                                                                  new Vector3(10995.34f, 8408.401f, 61.61731f),
                                                                  new Vector3(10895.08f, 7045.215f, 51.72278f),
                                                                  new Vector3(12665.45f, 6466.962f, 51.70544f),
                                                                  new Vector3(4966.042f, 10475.51f, 71.24048f),
                                                                  new Vector3(39000.529f, 7901.832f, 51.84973f),
                                                                  new Vector3(2106.111f, 8388.643f, 51.77686f),
                                                                  new Vector3(3753.737f, 6454.71f, 52.46301f),
                                                                  new Vector3(6776.247f, 5542.872f, 55.27625f),
                                                                  new Vector3(7811.688f, 4152.602f, 53.79456f),
                                                                  new Vector3(8528.921f, 2822.875f, 50.92188f),
                                                                  new Vector3(9850.102f, 4432.272f, 71.24072f),
                                                                  new Vector3(3926f, 7918f, 51.74162f)
                                                              };

        private static bool active;

        private static Geometry.Polygon rect;

        #endregion

        #region Public Properties

        public static Obj_AI_Base BuffedEnemy => ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(unit => unit.IsEnemy && unit.HasQBuff());

        #endregion

        #region Public Methods and Operators

        public static void Load()
        {
            Drawing.OnDraw += args => Draw();
            Game.OnUpdate += args => Tick();
        }

        #endregion

        #region Methods

        private static void Draw()
        {
            if (!Misc.GetMenuItem("escapeMode")
                || !Misc.GetMenuItem("ElLeeSin.Draw.Escape"))
            {
                return;
            }

            if (active && LeeSin.spells[LeeSin.Spells.Q].IsReady()
                && Misc.GetMenuItem("ElLeeSin.Draw.Q.Width"))
            {
                rect.Draw(Color.White);
            }
            foreach (var pos in JunglePos)
            {
                if (rect != null)
                {
                    if (pos.Distance(ObjectManager.Player.Position) < 2000)
                    {
                        Render.Circle.DrawCircle(pos, 100, rect.IsOutside(pos.To2D()) ? Color.White : Color.DeepSkyBlue);
                    }
                }
                else
                {
                    if (pos.Distance(ObjectManager.Player.Position) < 2000)
                    {
                        Render.Circle.DrawCircle(pos, 100, Color.White);
                    }
                }
            }
        }

        private static void Escape()
        {
            Misc.Orbwalk(Game.CursorPos);

            if (BuffedEnemy.IsValidTarget() && BuffedEnemy.IsValid<AIHeroClient>())
            {
                InitQ = false;
                return;
            }
            if (InitQ)
            {
                foreach (var point in JunglePos)
                {
                    if ((ObjectManager.Player.Distance(point) < 100) || (LeeSin.LastQ2 + 2000 < Environment.TickCount))
                    {
                        InitQ = false;
                    }
                }
            }

            rect = new Geometry.Polygon.Rectangle(
                       ObjectManager.Player.Position.To2D(),
                       ObjectManager.Player.Position.To2D().Extend(Game.CursorPos.To2D(), 1050),
                       100);

            if (Misc.IsQOne)
            {
                if (LeeSin.spells[LeeSin.Spells.Q].IsReady())
                {
                    foreach (var pos in JunglePos)
                    {
                        if (rect.IsOutside(pos.To2D()))
                        {
                            continue;
                        }
                        InitQ = true;
                        LeeSin.spells[LeeSin.Spells.Q].Cast(pos);
                        return;
                    }
                }
            }
            else
            {
                LeeSin.spells[LeeSin.Spells.Q].Cast();
                InitQ = true;
            }
        }

        private static void Tick()
        {
            if (MyMenu.Menu.Item("ElLeeSin.Escape").GetValue<KeyBind>().Active
                && Misc.GetMenuItem("escapeMode"))
            {
                Escape();
                active = true;
            }
            else
            {
                active = false;
            }
        }

        #endregion
    }
}