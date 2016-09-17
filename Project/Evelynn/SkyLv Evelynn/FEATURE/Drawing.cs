using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Evelynn
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
                return SkyLv_Evelynn.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Evelynn.Q;
            }
        }

        private static Spell W
        {
            get
            {
                return SkyLv_Evelynn.W;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Evelynn.E;
            }
        }

        private static Spell R
        {
            get
            {
                return SkyLv_Evelynn.R;
            }
        }
        #endregion

        static Draw()
        {
            //Menu
            SkyLv_Evelynn.Menu.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, System.Drawing.Color.Orange)));
            SkyLv_Evelynn.Menu.SubMenu("Drawings").AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(true, System.Drawing.Color.Green)));
            SkyLv_Evelynn.Menu.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E range").SetValue(new Circle(true, System.Drawing.Color.Blue)));
            SkyLv_Evelynn.Menu.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
            SkyLv_Evelynn.Menu.SubMenu("Drawings").AddItem(new MenuItem("VisibleRange", "Visible range").SetValue(new Circle(true, System.Drawing.Color.LightGreen)));
            SkyLv_Evelynn.Menu.SubMenu("Drawings").AddItem(new MenuItem("DrawOrbwalkTarget", "Draw Orbwalk target").SetValue(new Circle(true, System.Drawing.Color.Pink)));
            SkyLv_Evelynn.Menu.SubMenu("Drawings").AddItem(new MenuItem("SpellDraw.Radius", "Spell Draw Radius").SetValue(new Slider(10, 1, 20)));
            SkyLv_Evelynn.Menu.SubMenu("Drawings").AddItem(new MenuItem("OrbwalkDraw.Radius", "Orbwalk Draw Radius").SetValue(new Slider(10, 1, 20)));
            SkyLv_Evelynn.Menu.SubMenu("Drawings").AddItem(new MenuItem("VisibleRange.Radius", "Visible Range Draw Radius").SetValue(new Slider(10, 1, 20)));

            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            
            foreach (var spell in SkyLv_Evelynn.SpellList)
            {
                var menuItem = SkyLv_Evelynn.Menu.Item(spell.Slot + "Range").GetValue<Circle>();

                if (menuItem.Active && (spell.Slot != SpellSlot.R || R.Level > 0))
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color, SkyLv_Evelynn.Menu.Item("SpellDraw.Radius").GetValue<Slider>().Value);
            }

            if (SkyLv_Evelynn.Menu.Item("DrawOrbwalkTarget").GetValue<Circle>().Active)
            {
                var orbT = SkyLv_Evelynn.Orbwalker.GetTarget();
                if (orbT.IsValidTarget())
                    Render.Circle.DrawCircle(orbT.Position, 100, SkyLv_Evelynn.Menu.Item("DrawOrbwalkTarget").GetValue<Circle>().Color, SkyLv_Evelynn.Menu.Item("OrbwalkDraw.Radius").GetValue<Slider>().Value);
            }

            if (SkyLv_Evelynn.Menu.Item("VisibleRange").GetValue<Circle>().Active && Player.HasBuff("EvelynnStealthMarker"))
            {
                if (Player.CountEnemiesInRange(1000) > 0)
                {
                    Render.Circle.DrawCircle(Player.Position, 1000, System.Drawing.Color.OrangeRed, SkyLv_Evelynn.Menu.Item("VisibleRange.Radius").GetValue<Slider>().Value);
                }
                else
                    Render.Circle.DrawCircle(Player.Position, 1000, SkyLv_Evelynn.Menu.Item("VisibleRange").GetValue<Circle>().Color, SkyLv_Evelynn.Menu.Item("VisibleRange.Radius").GetValue<Slider>().Value);
            }

        }
    }
}
