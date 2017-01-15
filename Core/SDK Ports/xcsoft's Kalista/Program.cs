using System;
using System.Collections.Generic;

using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;
using NLog;

using SharpDX;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcKalista
{
    internal static class Program
    {
        private const string ChampName = "Kalista";

        public static void Main()
        {
            Bootstrap.Init();
            Load_OnLoad();
        }

        private static void Load_OnLoad()
        {
            if (ObjectManager.Player.ChampionName != ChampName)
            {
                return;
            }

            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();
            OathswornManager.Initialize();
            SoulHandler.Initialize();

            Drawing.OnDraw += Drawing_OnDraw;
            Variables.Orbwalker.OnAction += Orbwalker_OnAction;

            //LogManager.GetCurrentClassLogger().Info("Kalista Loaded successfully!");

            Notifications.Add(new Notification("Kalista Loaded!", "Kalista was loaded!", "Good luck, have fun!")
            {
                HeaderTextColor = Color.LightBlue, BodyTextColor = Color.White, Icon = NotificationIconType.Check, IconFlash = true
            });
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Drawings.DrawQRange && SpellManager.Q.Level > 0)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, SpellManager.Q.Range, System.Drawing.Color.DeepSkyBlue);
            }

            if (Config.Drawings.DrawWRange && SpellManager.W.Level > 0)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, SpellManager.W.Range, System.Drawing.Color.DeepSkyBlue);
            }

            if (Config.Drawings.DrawERange && SpellManager.E.Level > 0)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, SpellManager.E.Range, System.Drawing.Color.DeepSkyBlue);
            }

            if (Config.Drawings.DrawRRange && SpellManager.R.Level > 0)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, SpellManager.R.Range, System.Drawing.Color.DeepSkyBlue);
            }
        }

        private static void Orbwalker_OnAction(object sender, OrbwalkingActionArgs e)
        {
            switch (e.Type)
            {
                case OrbwalkingType.NonKillableMinion:
                    Orbwalker_NonKillableMinion(e);
                    break;
                case OrbwalkingType.AfterAttack:
                    Orbwalker_AfterAttack(e);
                    break;
            }
        }

        private static void Orbwalker_NonKillableMinion(OrbwalkingActionArgs e)
        {
            var target = e.Target as Obj_AI_Minion;

            if (target != null && Config.Auto.AutoE.KillUnkillableMinions && SpellManager.E.IsReady() && GameObjects.Player.ManaPercent > Config.Auto.AutoE.KillUnkillableMinionsMinMana && target.IsKillableWithE(true))
            {
                SpellManager.E.Cast();
            }
        }

        private static void Orbwalker_AfterAttack(OrbwalkingActionArgs e)
        {
            var targetAsMinion = e.Target as Obj_AI_Minion;

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.None:
                    break;
                case OrbwalkingMode.Combo:
                    break;
                case OrbwalkingMode.Hybrid:
                    break;
                case OrbwalkingMode.LastHit:
                    break;
                case OrbwalkingMode.LaneClear:
                    if (targetAsMinion == null)
                    {
                        //타겟이 미니언이 아닐경우 브레이크
                        break;
                    }

                    //때린 오브젝트가 정글 몹인지 아닌지 판단
                    switch (targetAsMinion.GetJungleType())
                    {
                        case JungleType.Unknown://정글몹이 아님
                            break;
                        case JungleType.Small://작은몹
                            break;
                        //case JungleType.Large://큰몹                            
                        //    break;
                        //case JungleType.Legendary://전설몹                            
                        //    break;
                        default://큰몹이거나 전설몹이면
                            if (Config.Modes.JungleClear.UseQ && SpellManager.Q.IsReady() && GameObjects.Player.ManaPercent > Config.Modes.JungleClear.MinMana)
                            {
                                SpellManager.Q.Cast(targetAsMinion);
                            }
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
