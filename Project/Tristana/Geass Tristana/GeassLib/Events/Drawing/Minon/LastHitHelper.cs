using System;
using GeassLib.Menus;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Events.Drawing.Minon
{
    class LastHitHelper
    {
        public LastHitHelper()
        {
            Game.OnUpdate += OnMinionDraw;
        }
        public void OnMinionDraw(EventArgs args)
        {
            if (Globals.Objects.Player.IsDead) return;
            if (!Globals.Objects.GeassLibMenu.Item(Names.DrawingItemBase + ".Minion." + "Boolean.LastHitHelper").GetValue<bool>()) return;

            foreach (var minion in Functions.Objects.Minions.GetEnemyMinions(Globals.Objects.GeassLibMenu.Item(Names.DrawingItemBase + ".Minion." + "Slider.RenderDistance").GetValue<Slider>().Value))
            {
                if (Globals.Objects.Player.GetAutoAttackDamage(minion)-5 > minion.Health) // Is killable
                   Render.Circle.DrawCircle(minion.Position, minion.BoundingRadius + 100, Globals.Objects.GeassLibMenu.Item(Names.DrawingItemBase + ".Minion." + "Circle.KillableColor").GetValue<Circle>().Color, 2);

            }
        }
    }
}
