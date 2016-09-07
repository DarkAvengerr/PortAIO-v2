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
 namespace xcAshe
{
    internal static class Program
    {
        private const string ChampName = "Ashe";

        public static void Main()
        {
            Bootstrap.Init();
            Load_OnLoad();
        }

        private static void Load_OnLoad()
        {
            if (!ObjectManager.Player.ChampionName.Equals(ChampName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();

            Drawing.OnDraw += Drawing_OnDraw;
            Variables.Orbwalker.OnAction += Orbwalker_OnAction;
            Events.OnGapCloser += Events_OnGapCloser;
            Events.OnInterruptableTarget += Events_OnInterruptableTarget;
            //AttackableUnit.OnLeaveVisiblityClient += AttackableUnit_OnLeaveVisiblityClient;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            LogManager.GetCurrentClassLogger().Info($"{ChampName} Loaded successfully!");

            Notifications.Add(new Notification($"{ChampName} Loaded!", $"{ChampName} was loaded!", "Good luck, have fun!")
            {
                HeaderTextColor = Color.LightBlue,
                BodyTextColor = Color.White,
                Icon = NotificationIconType.Check,
                IconFlash = true
            });
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Drawings.DrawWRange)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, SpellManager.W.Range, System.Drawing.Color.DeepSkyBlue);
            }

            if (Config.Drawings.DrawRRange)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, SpellManager.R.Range, System.Drawing.Color.DeepSkyBlue);
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

        private static void Orbwalker_AfterAttack(OrbwalkingActionArgs e)
        {//평타 후 로직
            var targetAsHero = e.Target as AIHeroClient;
            var targetAsMinion = e.Target as Obj_AI_Minion;

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    if (targetAsHero == null)
                    {
                        //타겟이 챔피언이 아닐경우 브레이크
                        break;
                    }

                    if (Config.Modes.Combo.UseQ && SpellManager.Q.IsReady() && targetAsHero.InAutoAttackRange())
                    {
                        SpellManager.Q.Cast();
                    }
                    break;
                case OrbwalkingMode.Hybrid:
                    if (targetAsHero == null)
                    {
                        //타겟이 챔피언이 아닐경우 브레이크
                        break;
                    }

                    if (Config.Modes.Harass.UseQ && SpellManager.Q.IsReady() && GameObjects.Player.ManaPercent > Config.Modes.Harass.MinMana && targetAsHero.InAutoAttackRange())
                    {
                        SpellManager.Q.Cast();
                    }
                    break;
                case OrbwalkingMode.LaneClear:
                    if (targetAsMinion == null)
                    {
                        //타겟이 미니언이 아닐경우 브레이크
                        break;
                    }

                    //때린 오브젝트가 정글 몹인지 아닌지 판단
                    if (targetAsMinion.GetJungleType().HasFlag(JungleType.Unknown))
                    {//정글몹이 아니면 - laneclear 
                        if (Config.Modes.LaneClear.UseQ && SpellManager.Q.IsReady() && GameObjects.Player.ManaPercent > Config.Modes.LaneClear.MinMana)
                        {
                            SpellManager.Q.Cast();
                        }

                        if (Config.Modes.LaneClear.UseW && SpellManager.W.IsReady() && GameObjects.Player.ManaPercent > Config.Modes.LaneClear.MinMana)
                        {
                            SpellManager.W.Cast(targetAsMinion);
                        }
                    }
                    else
                    {//정글몹이면 - jungleclear

                        //간혹 V 키누르다 의도치 않게 정글몹에게 스킬을 사용하는 경우를 없애기위해.
                        //정글몹 콤보는 항상 평타 이후부터 시작하는걸로

                        if (Config.Modes.JungleClear.UseQ && SpellManager.Q.IsReady() && GameObjects.Player.ManaPercent > Config.Modes.JungleClear.MinMana)
                        {
                            SpellManager.Q.Cast();
                        }

                        if (Config.Modes.JungleClear.UseW && SpellManager.W.IsReady() && GameObjects.Player.ManaPercent > Config.Modes.JungleClear.MinMana)
                        {
                            SpellManager.W.Cast(targetAsMinion);
                        }
                    }
                    break;
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
                Extensions.CastR(e.Sender);
            }
        }

        private static void Events_OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs e)
        {
            if (!Config.Misc.AutoInterrupt.Enabled)
            {
                return;
            }

            if (Config.Misc.AutoInterrupt.UseR && SpellManager.R.IsReady() && e.Sender.IsValidTarget(SpellManager.R.Range))
            {
                Extensions.CastR(e.Sender);
            }
        }

        private static void AttackableUnit_OnLeaveVisiblityClient(GameObject sender, EventArgs args)
        {//클라이언트 시야에서 사라질때
            var senderHero = sender as AIHeroClient;
            if (senderHero != null      //sender가 챔피언이면
                && senderHero.IsEnemy)  //sender가 적이면
            {
                if (Config.Auto.AutoE.UseEBush
                    && SpellManager.E.IsReady()
                    && sender.Distance(GameObjects.Player) <= 1125
                    && NavMesh.IsWallOfGrass(sender.Position, 1))
                {
                    SpellManager.E.Cast(sender.Position);
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //if (sender.IsMe && args.Slot == SpellSlot.Q)
            //{
            //    //평큐평
            //    Variables.Orbwalker.ResetSwingTimer();
            //}

            //적 점멸 쓴곳에 E 날리기
            if (Config.Auto.AutoE.UseEFlash
                && SpellManager.E.IsReady()
                && sender.IsEnemy
                && args.SData.Name.Equals("summonerflash", StringComparison.OrdinalIgnoreCase)
                && sender.DistanceToPlayer() <= 1125)
            {
                var flashPosition = sender.Distance(args.End) > 450 ? sender.ServerPosition.Extend(args.End, 450) : args.End;
                //시야 확인 API 추가 예정
                SpellManager.E.Cast(flashPosition);
            }
        }
    }
}
