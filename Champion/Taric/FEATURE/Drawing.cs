namespace SkyLv_Taric
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class Draw
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Taric.Q;
            }
        }

        private static Spell W
        {
            get
            {
                return SkyLv_Taric.W;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Taric.E;
            }
        }

        private static Spell R
        {
            get
            {
                return SkyLv_Taric.R;
            }
        }
        #endregion

        static Draw()
        {
            //Menu
            SkyLv_Taric.Menu.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, System.Drawing.Color.Orange)));
            SkyLv_Taric.Menu.SubMenu("Drawings").AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(true, System.Drawing.Color.Green)));
            SkyLv_Taric.Menu.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E range").SetValue(new Circle(true, System.Drawing.Color.Blue)));
            SkyLv_Taric.Menu.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
            SkyLv_Taric.Menu.SubMenu("Drawings").AddItem(new MenuItem("DrawOrbwalkTarget", "Draw Orbwalk target").SetValue(new Circle(true, System.Drawing.Color.Pink)));
            SkyLv_Taric.Menu.SubMenu("Drawings").AddItem(new MenuItem("SpellDraw.Radius", "Spell Draw Radius").SetValue(new Slider(10, 1, 20)));
            SkyLv_Taric.Menu.SubMenu("Drawings").AddItem(new MenuItem("OrbwalkDraw.Radius", "Orbwalk Draw Radius").SetValue(new Slider(10, 1, 20)));

            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SkyLv_Taric.SpellList)
            {
                var menuItem = SkyLv_Taric.Menu.Item(spell.Slot + "Range").GetValue<Circle>();

                if (menuItem.Active && (spell.Slot != SpellSlot.R || R.Level > 0))
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color, SkyLv_Taric.Menu.Item("SpellDraw.Radius").GetValue<Slider>().Value);
            }

            if (SkyLv_Taric.Menu.Item("DrawOrbwalkTarget").GetValue<Circle>().Active)
            {
                var orbT = SkyLv_Taric.Orbwalker.GetTarget();
                if (orbT.IsValidTarget())
                    Render.Circle.DrawCircle(orbT.Position, 100, SkyLv_Taric.Menu.Item("DrawOrbwalkTarget").GetValue<Circle>().Color, SkyLv_Taric.Menu.Item("OrbwalkDraw.Radius").GetValue<Slider>().Value);
            }
        }
    }
}
