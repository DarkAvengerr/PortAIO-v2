using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Jax
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;


    internal class Draw
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Jax.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Jax.Q;
            }
        }

        private static Spell W
        {
            get
            {
                return SkyLv_Jax.W;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Jax.E;
            }
        }

        private static Spell R
        {
            get
            {
                return SkyLv_Jax.R;
            }
        }
        #endregion

        static Draw()
        {
            //Menu
            SkyLv_Jax.Menu.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, System.Drawing.Color.Orange)));
            SkyLv_Jax.Menu.SubMenu("Drawings").AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(true, System.Drawing.Color.Green)));
            SkyLv_Jax.Menu.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E range").SetValue(new Circle(true, System.Drawing.Color.Blue)));
            SkyLv_Jax.Menu.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
            SkyLv_Jax.Menu.SubMenu("Drawings").AddItem(new MenuItem("DrawOrbwalkTarget", "Draw Orbwalk target").SetValue(new Circle(true, System.Drawing.Color.Pink)));
            SkyLv_Jax.Menu.SubMenu("Drawings").AddItem(new MenuItem("SpellDraw.Radius", "Spell Draw Radius").SetValue(new Slider(10, 1, 20)));
            SkyLv_Jax.Menu.SubMenu("Drawings").AddItem(new MenuItem("OrbwalkDraw.Radius", "Orbwalk Draw Radius").SetValue(new Slider(10, 1, 20)));

            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            
            foreach (var spell in SkyLv_Jax.SpellList)
            {
                var menuItem = SkyLv_Jax.Menu.Item(spell.Slot + "Range").GetValue<Circle>();

                if (menuItem.Active && (spell.Slot != SpellSlot.R || R.Level > 0))
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color, SkyLv_Jax.Menu.Item("SpellDraw.Radius").GetValue<Slider>().Value);
            }

            if (SkyLv_Jax.Menu.Item("DrawOrbwalkTarget").GetValue<Circle>().Active)
            {
                var orbT = SkyLv_Jax.Orbwalker.GetTarget();
                if (orbT.IsValidTarget())
                    Render.Circle.DrawCircle(orbT.Position, 100, SkyLv_Jax.Menu.Item("DrawOrbwalkTarget").GetValue<Circle>().Color, SkyLv_Jax.Menu.Item("OrbwalkDraw.Radius").GetValue<Slider>().Value);
            }
        }
    }
}
