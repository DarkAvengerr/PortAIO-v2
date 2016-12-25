using System;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Data;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace MightyNidalee
{
    class Drawings : R_Manager
    {
        public static void DrawEvent()
        {
            Drawing.OnDraw += SpellRanges;
            Drawing.OnDraw += CooldownDrawer;
            // Drawing.OnEndScene += MinimapR;

        }
        private static void CooldownDrawer(EventArgs args)
        {
            var Drawings = Config.Item("Draw.Cooldowns").GetValue<bool>();
            var pos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            if (Drawings)
            {
                if (QhumanReady && Mighty.Q.Level >= 1)
                    Drawing.DrawText(pos[0] + 105, pos[1] + 20, System.Drawing.Color.White, "Human Q: Ready", 16);
                else if (Mighty.Q.Level < 1)
                    Drawing.DrawText(pos[0] + 105, pos[1] + 20, System.Drawing.Color.White, "Human Q: Not Learned Yet", 16);
                else Drawing.DrawText(pos[0] + 105, pos[1] + 20, System.Drawing.Color.White, "Human Q: " + Qlefttime().ToString("#.#"), 16);

                if (WcougarReady && Mighty.W.Level >= 1)
                    Drawing.DrawText(pos[0] + 105, pos[1] + 35, System.Drawing.Color.White, "Cougar W: Ready", 16);
                else if (Mighty.W.Level < 1)
                    Drawing.DrawText(pos[0] + 105, pos[1] + 35, System.Drawing.Color.White, "Cougar W: Not Learned Yet", 16);
                else Drawing.DrawText(pos[0] + 105, pos[1] + 35, System.Drawing.Color.White, "Cougar W: " + Wlefttime().ToString("#.#"));

            }

        }
        
        private static void SpellRanges(EventArgs args)
        {
            if (Config.Item("disable.draws").GetValue<bool>()) return;
            if (!R_Manager.CougarForm)
            {
                if (Config.Item("Draw.Q").GetValue<Circle>().Active)
                {
                    if (Q.Level > 0)
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Q.IsReady() ?
                            Config.Item("Draw.Q").GetValue<Circle>().Color : System.Drawing.Color.Red, Config.Item("CircleThickness").GetValue<Slider>().Value);
                }

                if (Config.Item("Draw.W").GetValue<Circle>().Active)
                {
                    if (W.Level > 0)
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, W.IsReady() ?
                            Config.Item("Draw.W").GetValue<Circle>().Color : System.Drawing.Color.Red, Config.Item("CircleThickness").GetValue<Slider>().Value);
                }

                if (Config.Item("Draw.E").GetValue<Circle>().Active)
                {
                    if (E.Level > 0)
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, E.IsReady() ?
                            Config.Item("Draw.E").GetValue<Circle>().Color : System.Drawing.Color.Red, Config.Item("CircleThickness").GetValue<Slider>().Value);
                }
            }

            if (R_Manager.CougarForm)
            {

                if (Config.Item("Draw.W2").GetValue<Circle>().Active)
                {
                    if (W2.Level > 0)
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, W2.Range, W2.IsReady() ?
                            Config.Item("Draw.W2").GetValue<Circle>().Color : System.Drawing.Color.Red, Config.Item("CircleThickness").GetValue<Slider>().Value);
                }

                if (Config.Item("Draw.E2").GetValue<Circle>().Active)
                {
                    if (E2.Level > 0)
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, E2.Range, E2.IsReady() ?
                            Config.Item("Draw.E2").GetValue<Circle>().Color : System.Drawing.Color.Red, Config.Item("CircleThickness").GetValue<Slider>().Value);
                }
            }
        }
    }
}
