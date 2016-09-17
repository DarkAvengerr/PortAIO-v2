using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;

    internal class Draw
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Tristana.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Tristana.Q;
            }
        }

        private static Spell W
        {
            get
            {
                return SkyLv_Tristana.W;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Tristana.E;
            }
        }

        private static Spell R
        {
            get
            {
                return SkyLv_Tristana.R;
            }
        }
        #endregion

        static Draw()
        {
            //Menu
            SkyLv_Tristana.Menu.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, Color.Orange)));
            SkyLv_Tristana.Menu.SubMenu("Drawings").AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(true, Color.Green)));
            SkyLv_Tristana.Menu.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E range").SetValue(new Circle(true, Color.Blue)));
            SkyLv_Tristana.Menu.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(true, Color.Gold)));
            SkyLv_Tristana.Menu.SubMenu("Drawings").AddItem(new MenuItem("Tristana.DrawingsInsec", "Draw Insec").SetValue(new Circle(true, Color.OrangeRed)));
            SkyLv_Tristana.Menu.SubMenu("Drawings").AddItem(new MenuItem("Tristana.DrawingsREndPosition", "Draw R End Position").SetValue(new Circle(false, Color.GreenYellow)));
            SkyLv_Tristana.Menu.SubMenu("Drawings").AddItem(new MenuItem("DrawOrbwalkTarget", "Draw Orbwalk target").SetValue(new Circle(true, Color.Pink)));
            SkyLv_Tristana.Menu.SubMenu("Drawings").AddItem(new MenuItem("SpellDraw.Radius", "Spell Draw Radius").SetValue(new Slider(10, 1, 20)));
            SkyLv_Tristana.Menu.SubMenu("Drawings").AddItem(new MenuItem("Insec.Radius", "Insec Draw Radius").SetValue(new Slider(10, 1, 20)));
            SkyLv_Tristana.Menu.SubMenu("Drawings").AddItem(new MenuItem("REndPosition.Radius", "R End Position Draw Radius").SetValue(new Slider(10, 1, 20)));
            SkyLv_Tristana.Menu.SubMenu("Drawings").AddItem(new MenuItem("OrbwalkDraw.Radius", "Orbwalk Draw Radius").SetValue(new Slider(10, 1, 20)));

            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SkyLv_Tristana.SpellList)
            {
                var menuItem = SkyLv_Tristana.Menu.Item(spell.Slot + "Range").GetValue<Circle>();

                if (menuItem.Active && (spell.Slot != SpellSlot.R || R.Level > 0))
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color, SkyLv_Tristana.Menu.Item("SpellDraw.Radius").GetValue<Slider>().Value);
            }

            if (SkyLv_Tristana.Menu.Item("DrawOrbwalkTarget").GetValue<Circle>().Active)
            {
                var orbT = SkyLv_Tristana.Orbwalker.GetTarget();
                if (orbT.IsValidTarget())
                    Render.Circle.DrawCircle(orbT.Position, 100, SkyLv_Tristana.Menu.Item("DrawOrbwalkTarget").GetValue<Circle>().Color, SkyLv_Tristana.Menu.Item("OrbwalkDraw.Radius").GetValue<Slider>().Value);
            }

            if (SkyLv_Tristana.Menu.Item("Tristana.DrawingsInsec").GetValue<Circle>().Active)
            {
                var target = CustomLib.GetTarget;
                if (target != null)
                {
                    Drawing.DrawLine(Drawing.WorldToScreen(target.Position), Drawing.WorldToScreen(CustomLib.GetPushPosition(target)), SkyLv_Tristana.Menu.Item("Insec.Radius").GetValue<Slider>().Value, SkyLv_Tristana.Menu.Item("Tristana.DrawingsInsec").GetValue<Circle>().Color);
                    Render.Circle.DrawCircle(target.Position, target.BoundingRadius * 1.35f, SkyLv_Tristana.Menu.Item("Tristana.DrawingsInsec").GetValue<Circle>().Color, SkyLv_Tristana.Menu.Item("Insec.Radius").GetValue<Slider>().Value);
                    Render.Circle.DrawCircle(CustomLib.GetBehindPosition(target),target.BoundingRadius * 1.35f, SkyLv_Tristana.Menu.Item("Tristana.DrawingsInsec").GetValue<Circle>().Color, SkyLv_Tristana.Menu.Item("Insec.Radius").GetValue<Slider>().Value);
                }
            }

            if (SkyLv_Tristana.Menu.Item("Tristana.DrawingsREndPosition").GetValue<Circle>().Active)
            {
                var target = TargetSelector.GetSelectedTarget();
                if (target != null && R.IsReady())
                {
                    Render.Circle.DrawCircle(Player.Position.Extend(target.Position, Player.Distance(target) + CustomLib.RPushDistance()), target.BoundingRadius * 1.35f, SkyLv_Tristana.Menu.Item("Tristana.DrawingsREndPosition").GetValue<Circle>().Color, SkyLv_Tristana.Menu.Item("REndPosition.Radius").GetValue<Slider>().Value);
                }
            }
        }
    }
}
