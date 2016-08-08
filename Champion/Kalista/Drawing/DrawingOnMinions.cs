using System;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using EloBuddy;

namespace S_Plus_Class_Kalista.Drawing
{
    class DrawingOnMinions : Core
    {
        private const string _MenuNameBase = ".Minions Menu";
        private const string _MenuItemBase = ".Minions.";

        public static Menu DrawingOnMinionsMenu()
        {
            var menu = new Menu(_MenuNameBase, "minionMenu");
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.DrawOnMinions", "Draw On Minions").SetValue(false));
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.DrawOnMinions.MarkerInnerColor", "Inner Marker Color").SetValue(new Circle(true, Color.DeepSkyBlue)));
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.DrawOnMinions.MakerOuterColor", "Outer Marker Color").SetValue(new Circle(true, Color.Red)));
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.DrawOnMinions.MarkerKillableColor", "Killable Marker Color").SetValue(new Circle(true, Color.Green)));
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.DrawOnMinions.Distance", "Render Distance").SetValue(new Slider(1000, 500, 2500)));
            return menu;
        }

        public static void OnMinionDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (!SMenu.Item(_MenuItemBase + "Boolean.DrawOnMinions").GetValue<bool>()) return;

            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
            {

                if (minion.LSDistance(Player) > SMenu.Item(_MenuItemBase + "Boolean.DrawOnMinions.Distance").GetValue<Slider>().Value) continue; // Out of render range
                if (minion.IsAlly) continue; //This is not Dota2
                if (minion.IsDead) continue;//Dont poke the dead
                if (!minion.IsMinion) continue; //Differect Function

                if (Player.LSGetAutoAttackDamage(minion) > minion.Health) // Is killable
                {
                    Render.Circle.DrawCircle(minion.Position, minion.BoundingRadius + 50, SMenu.Item(_MenuItemBase + "Boolean.DrawOnMinions.MarkerKillableColor").GetValue<Circle>().Color, 2);
                }


                else // Not killable
                {
                    Render.Circle.DrawCircle(minion.Position, minion.BoundingRadius + 50, SMenu.Item(_MenuItemBase + "Boolean.DrawOnMinions.MarkerInnerColor").GetValue<Circle>().Color, 2);

                    var remainingHp = (int)100 * (minion.Health - Player.LSGetAutoAttackDamage(minion)) / minion.Health;

                    Render.Circle.DrawCircle(minion.Position, minion.BoundingRadius + (float)remainingHp + 50, SMenu.Item(_MenuItemBase + "Boolean.DrawOnMinions.MakerOuterColor").GetValue<Circle>().Color, 2);
                }
            }

        }
    }
}
