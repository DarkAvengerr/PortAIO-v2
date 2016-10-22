using System;

using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;
using NLog;

using SharpDX;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcBlitzcrank
{
    internal static class Program
    {
        private const string ChampName = "Blitzcrank";
        private static bool DontAutoAttack;

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

            Drawing.OnDraw += Drawing_OnDraw;
            Variables.Orbwalker.OnAction += Orbwalker_OnAction;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffAdd;
            Events.OnGapCloser += Events_OnGapCloser;
            Events.OnInterruptableTarget += Events_OnInterruptableTarget;

            LogManager.GetCurrentClassLogger().Info($"{ChampName} Loaded successfully!");

            Notifications.Add(new Notification($"Blitzcrank Loaded!", $"Blitzcrank was loaded!", "Good luck, have fun!")
            {
                HeaderTextColor = Color.LightBlue,
                BodyTextColor = Color.White,
                Icon = NotificationIconType.Check,
                IconFlash = true
            });
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Drawings.DrawQRange && SpellManager.Q.Level > 0)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, SpellManager.Q.Range, System.Drawing.Color.DeepSkyBlue);
            }

            //if (Config.Drawings.DrawWRange && SpellManager.W.Level > 0)
            //{
            //    Render.Circle.DrawCircle(GameObjects.Player.Position, SpellManager.W.Range, Color.DeepSkyBlue);
            //}

            //if (Config.Drawings.DrawERange && SpellManager.E.Level > 0)
            //{
            //    Render.Circle.DrawCircle(GameObjects.Player.Position, SpellManager.E.Range, Color.DeepSkyBlue);
            //}

            if (Config.Drawings.DrawRRange && SpellManager.R.Level > 0)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, SpellManager.R.Range, System.Drawing.Color.DeepSkyBlue);
            }

            if (Variables.TickCount - Config.Misc.QRange.QMinRange_LastChangedTime < 1000)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, Config.Misc.QRange.QMinRange, System.Drawing.Color.Red);
            }

            if (Variables.TickCount - Config.Misc.QRange.QMaxRange_LastChangedTime < 1000)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, Config.Misc.QRange.QMaxRange, System.Drawing.Color.Red);
            }
        }

        private static void Orbwalker_OnAction(object sender, OrbwalkingActionArgs e)
        {
            switch (e.Type)
            {
                case OrbwalkingType.None:
                    break;
                case OrbwalkingType.Movement:
                    break;
                case OrbwalkingType.StopMovement:
                    break;
                case OrbwalkingType.BeforeAttack:
                    Orbwalker_BeforeAttack(e);
                    break;
                case OrbwalkingType.AfterAttack:
                    Orbwalker_AfterAttack(e);
                    break;
                case OrbwalkingType.OnAttack:
                    break;
                case OrbwalkingType.NonKillableMinion:
                    break;
            }
        }

        private static void Orbwalker_BeforeAttack(OrbwalkingActionArgs e)
        {
            var targetAsHero = e.Target as AIHeroClient;

            if (DontAutoAttack)
            {
                e.Process = false;
            }

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.None:
                    break;
                case OrbwalkingMode.Combo:
                    if (targetAsHero != null)
                    {
                        if (Config.Modes.Combo.UseW && SpellManager.W.IsReady() && GameObjects.Player.ManaPercent > Config.Modes.Combo.WMinManaPer)
                        {
                            SpellManager.W.Cast();
                        }
                    }
                    break;
                case OrbwalkingMode.Hybrid:
                    break;
                case OrbwalkingMode.LastHit:
                    break;
                case OrbwalkingMode.LaneClear:
                    break;
            }
        }

        private static void Orbwalker_AfterAttack(OrbwalkingActionArgs e)
        {
            var targetAsHero = e.Target as AIHeroClient;

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.None:
                    break;
                case OrbwalkingMode.Combo:
                    if (targetAsHero != null)
                    {
                        if (Config.Modes.Combo.UseE && SpellManager.E.IsReady())
                        {
                            SpellManager.E.Cast();
                        }
                    }
                    break;
                case OrbwalkingMode.Hybrid:
                    if (targetAsHero != null)
                    {
                        if (GameObjects.Player.ManaPercent > Config.Modes.Harass.MinMana)
                        {
                            if (Config.Modes.Harass.UseE && SpellManager.E.IsReady())
                            {
                                SpellManager.E.Cast();
                            }
                        }
                    }
                    break;
                case OrbwalkingMode.LastHit:
                    break;
                case OrbwalkingMode.LaneClear:
                    break;
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //if (sender.IsMe)
            //{
            //    if (args.Slot == SpellSlot.E)
            //    {
            //        Variables.Orbwalker.ResetSwingTimer();
            //    }
            //}
        }

        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Animation == "Spell1" || args.Animation == "Spell4")
                {
                    DontAutoAttack = true;
                }
                else
                {
                    DontAutoAttack = false;
                }
            }
        }

        private static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            var senderAsHero = sender as AIHeroClient;

            if (Config.Auto.AutoE.AutoE1 && senderAsHero != null && args.Buff.Name.Equals("rocketgrab2", StringComparison.OrdinalIgnoreCase) && args.Buff.Caster.IsMe)
            {
                SpellManager.E.Cast();
            }
        }

        private static void Events_OnGapCloser(object sender, Events.GapCloserEventArgs e)
        {
            if (!Config.Misc.AntiGapcloser.Enabled)
            {
                return;
            }

            if (Config.Misc.AntiGapcloser.UseR && SpellManager.R.IsReady() && e.Sender.IsValidTarget(SpellManager.R.Range))
            {
                SpellManager.R.Cast();
            }
        }

        private static void Events_OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs e)
        {
            if (!Config.Misc.AutoInterrupt.Enabled)
            {
                return;
            }

            if (Config.Misc.AutoInterrupt.UseQ && SpellManager.Q.IsReady() && e.Sender.IsValidTarget(SpellManager.Q.Range))
            {
                SpellManager.Q.Cast(e.Sender);
            }

            if (Config.Misc.AutoInterrupt.UseR && SpellManager.R.IsReady() && e.Sender.IsValidTarget(SpellManager.R.Range))
            {
                SpellManager.R.Cast();
            }
        }
    }
}
