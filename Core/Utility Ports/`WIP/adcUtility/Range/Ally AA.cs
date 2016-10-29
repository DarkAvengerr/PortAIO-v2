using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace adcUtility.Range
{
    public static class Ally_AA
    {
        private static Obj_AI_Base adCarry = null;
        public static bool hikiAllyAA { get; set; }

        public static Obj_AI_Base AllyAA
        {
            get
            {
                if (adCarry != null && adCarry.IsValid)
                {
                    return adCarry;
                }
                return null;
            }
        }
        static Ally_AA()
        {
            Drawing.OnDraw += Drawing_OnDraw;
            adCarry = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsAlly);
            if (adCarry != null)
            {
                Console.Write(adCarry.CharData.BaseSkinName);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var allyRange = Program.Config.Item("ally.range").GetValue<Circle>();
            var allyRangeColor = Program.Config.Item("ally.range").GetValue<Circle>().Color;
            var rangeDistance = Program.Config.Item("ally.distance").GetValue<Slider>().Value;
            var thickness = Program.Config.Item("ally.thickness").GetValue<Slider>().Value;

            if (allyRange.Active)
            {
                foreach (var ally in HeroManager.Allies.Where(o => o.IsAlly && !o.IsDead && !o.IsZombie))
                {
                    if (ally.Distance(ObjectManager.Player.Position) < rangeDistance)
                    {
                        var allyposition = Drawing.WorldToScreen(ally.Position);
                        Render.Circle.DrawCircle(new Vector3(ally.Position.X, ally.Position.Y, ally.Position.Z), ally.AttackRange, allyRangeColor, thickness, true);
                    }
                }
            }
        }

    }
}