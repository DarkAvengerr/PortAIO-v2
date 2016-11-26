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
 namespace BadaoKingdom.BadaoChampion.BadaoKatarina
{
    using static BadaoMainVariables;
    using static BadaoKatarinaVariables;
    using static BadaoKatarinaHelper;
    public static class BadaoKatarina
    {
        public static void BadaoActivate()
        {
            BadaoKatarinaConfig.BadaoActivate();
            BadaoKatarinaAuto.BadaoActivate();
            BadaoKatarinaCombo.BadaoActivate();
            BadaoKatarinaHarass.BadaoAcitvate();
            BadaoKatarinaFlee.BadaoActivate();

            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            //if (WDaggers.Any(x => Player.Distance(x.Dagger.Position) <= 150 && Environment.TickCount - Game.Ping - x.CreationTime >= 1150))
            //{
            //    var furthest = GetEVinasun().MaxOrDefault(x => x.Position.Distance(Player.Position));
            //    if (furthest != null)
            //    {
            //        E.Cast(furthest.Position);
            //    }
            //}
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender is MissileClient && (sender as MissileClient).SpellCaster.IsMe)
            {
                var missile = sender as MissileClient;
                if (missile.SData.Name.ToLower().Contains("katarinarmis"))
                {
                    RMis.RemoveAll(x => x.NetworkId == missile.NetworkId);
                }
                if (missile.SData.Name.ToLower().Contains("katarinawdaggerarc"))
                {
                    WMis.RemoveAll(x => x.NetworkId == missile.NetworkId);
                }
            }
            if (sender.Name.ToLower().Contains("katarina_base_e_beam.troy"))
            {
                MyBeam.RemoveAll(x => x.NetworkId == sender.NetworkId);
            }
            if (sender.Name.ToLower().Contains("katarina_base_w_indicator"))
            {
                Daggers.RemoveAll(x => x.Dagger.NetworkId == sender.NetworkId);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            //foreach (var x in Daggers)
            //{
            //    Render.Circle.DrawCircle(x.Dagger.Position, 150, Color.Red);
            //}
            //foreach (var x in PickableDaggers)
            //{
            //    Render.Circle.DrawCircle(x.Dagger.Position, 100, Color.Yellow);
            //}
            //foreach (var x in WDaggers)
            //{
            //    Render.Circle.DrawCircle(x.Dagger.Position, 200, Color.Pink);
            //}
            //Drawing.DrawLine(new Vector2(200,100), new Vector2(1200,100),75, Color.Green);
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            //if (sender.Name.ToLower().Contains("katarina"))
            //{
            //    Chat.Print(sender.Name);
            //}
            if (sender is MissileClient && (sender as MissileClient).SpellCaster.IsMe)
            {
                var missile = sender as MissileClient;
                //Chat.Print(missile.SData.Name);
                if (missile.SData.Name.ToLower().Contains("katarinarmis"))
                {
                    RMis.Add(missile);
                    LastRMis = Environment.TickCount + 168;
                }
                if (missile.SData.Name.ToLower().Contains("katarinawdaggerarc"))
                {
                    WMis.Add(missile);
                }
            }
            if (sender.Name.ToLower().Contains("katarina_base_e_beam.troy"))
            {
                MyBeam.Add(sender);
            }
            if (sender.Name.ToLower().Contains("katarina_base_w_indicator"))
            {
                Daggers.Add(new KatarinaDagger { Dagger = sender, CreationTime = Environment.TickCount });
            }
        }
    }
}