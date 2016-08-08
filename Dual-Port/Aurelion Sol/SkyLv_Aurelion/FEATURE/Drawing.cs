using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
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
                return SkyLv_AurelionSol.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_AurelionSol.Q;
            }
        }

        private static Spell W1
        {
            get
            {
                return SkyLv_AurelionSol.W1;
            }
        }

        private static Spell W2
        {
            get
            {
                return SkyLv_AurelionSol.W2;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_AurelionSol.E;
            }
        }

        private static Spell R
        {
            get
            {
                return SkyLv_AurelionSol.R;
            }
        }
        #endregion

        static Draw()
        {
            //Menu
            SkyLv_AurelionSol.Menu.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, System.Drawing.Color.Orange)));
            SkyLv_AurelionSol.Menu.SubMenu("Drawings").AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(true, System.Drawing.Color.Green)));
            SkyLv_AurelionSol.Menu.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E range").SetValue(new Circle(true, System.Drawing.Color.Blue)));
            SkyLv_AurelionSol.Menu.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
            SkyLv_AurelionSol.Menu.SubMenu("Drawings").AddItem(new MenuItem("DrawOrbwalkTarget", "Draw Orbwalk target").SetValue(new Circle(true, System.Drawing.Color.Pink)));
            SkyLv_AurelionSol.Menu.SubMenu("Drawings").AddItem(new MenuItem("SpellDraw.Radius", "Spell Draw Radius").SetValue(new Slider(10, 1, 20)));
            SkyLv_AurelionSol.Menu.SubMenu("Drawings").AddItem(new MenuItem("OrbwalkDraw.Radius", "Orbwalk Draw Radius").SetValue(new Slider(10, 1, 20)));

            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static void Drawing_OnDraw(EventArgs args)
        {

            foreach (var spell in SkyLv_AurelionSol.SpellList)
            {
                var menuItem = SkyLv_AurelionSol.Menu.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active && spell.Slot == SpellSlot.W)
                {
                    if (CustomLib.isWInLongRangeMode())
                    {
                        Render.Circle.DrawCircle(Player.Position, W2.Range, menuItem.Color, SkyLv_AurelionSol.Menu.Item("SpellDraw.Radius").GetValue<Slider>().Value);
                    }

                    if (!CustomLib.isWInLongRangeMode())
                    {
                        Render.Circle.DrawCircle(Player.Position, W1.Range, menuItem.Color, SkyLv_AurelionSol.Menu.Item("SpellDraw.Radius").GetValue<Slider>().Value);
                    }
                }

                if (menuItem.Active && spell.Slot != SpellSlot.W && (spell.Slot != SpellSlot.R || R.Level > 0))
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color, SkyLv_AurelionSol.Menu.Item("SpellDraw.Radius").GetValue<Slider>().Value);
            }

            if (SkyLv_AurelionSol.Menu.Item("DrawOrbwalkTarget").GetValue<Circle>().Active)
            {
                var orbT = SkyLv_AurelionSol.Orbwalker.GetTarget();
                if (orbT.LSIsValidTarget())
                    Render.Circle.DrawCircle(orbT.Position, 100, SkyLv_AurelionSol.Menu.Item("DrawOrbwalkTarget").GetValue<Circle>().Color, SkyLv_AurelionSol.Menu.Item("OrbwalkDraw.Radius").GetValue<Slider>().Value);
            }

        }
    }
}
