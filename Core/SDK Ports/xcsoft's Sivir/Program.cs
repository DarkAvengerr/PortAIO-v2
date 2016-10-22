using System;
using System.Linq;
using System.Collections.Generic;

using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;
using LeagueSharp.Data.Enumerations;
using NLog;

using SharpDX;
using LeagueSharp.SDK.UI;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcSivir
{
    internal static class Program
    {
        private const string ChampName = "Sivir";

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
            Events.OnGapCloser += Events_OnGapCloser;
            //Events.OnInterruptableTarget += Events_OnInterruptableTarget;

            LogManager.GetCurrentClassLogger().Info($"{ChampName} Loaded successfully!");

            Notifications.Add(new Notification($"{ChampName} Loaded!", $"{ChampName} was loaded!", "Good luck, have fun!")
            {
                HeaderTextColor = Color.LightBlue,
                BodyTextColor = Color.White,
                Icon = NotificationIconType.Check,
                IconFlash = true
            });
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //적이 시전한 스펠이아니면 리턴
            //타겟이 없을경우 리턴
            //타겟이 내가 아닐경우 리턴
            if (!sender.IsEnemy || args.Target == null || !args.Target.IsMe)
            {
                return;
            }

            //오토 E 옵션 꺼져있으면 리턴
            if (!Config.Auto.AutoE.AutoEAgainstTargetedSpells)
            {
                return;
            }

            //E 레디아니면 리턴
            if (!SpellManager.E.IsReady())
            {
                return;
            }

            //SpellDatabase를 이용한 타게팅 스펠 감지
            var theSpell = SpellDatabase.GetByName(args.SData.Name);
            if (theSpell != null)
            {
                //스펠타입이 Targeted 이거나 TargetedMissile일경우
                //적 한테 타겟지정 시전 가능한 스펠일경우
                //옵션에서 켜놓은 스펠일경우
                if ((theSpell.SpellType.HasFlag(SpellType.Targeted) || theSpell.SpellType.HasFlag(SpellType.TargetedMissile))
                    && theSpell.CastType.Contains(CastType.EnemyChampions))
                {
                    var option = Config.Auto.AutoE.TargetedSpells.Menu.GetValue<MenuBool>($"{theSpell.ChampionName}.{theSpell.SpellName}");
                    if (option != null && option.Value)
                    {
                        if (SpellManager.E.Cast())
                            return;
                    }
                }
            }

            //추가 로직
            var senderAsHero = sender as AIHeroClient;
            if (senderAsHero != null)
            {
                //알리 Q 스턴 막기
                var option3 = Config.Auto.AutoE.TargetedSpells.Menu[$"Alistar.Pulverize"];
                if (option3 != null && option3.GetValue<MenuBool>().Value
                    && args.SData.Name.Equals("Pulverize", StringComparison.OrdinalIgnoreCase)
                    && senderAsHero.DistanceToPlayer() <= 400)
                {
                    if (SpellManager.E.Cast())
                        return;
                }

                //우디르 평타 스턴 막기
                var option1 = Config.Auto.AutoE.TargetedSpells.Menu[$"Udyr.AttackStun"];
                if (option1 != null && option1.GetValue<MenuBool>().Value
                    && args.SData.Name.Equals("UdyrBearAttack", StringComparison.OrdinalIgnoreCase)
                    && !GameObjects.Player.HasBuff("udyrbearstuncheck"))
                {
                    if (SpellManager.E.Cast())
                        return;
                }

                //브라움 패시브 스턴 막기
                var option2 = Config.Auto.AutoE.TargetedSpells.Menu[$"Braum.PassiveStun"];
                if (option2 != null && option2.GetValue<MenuBool>().Value
                    && args.SData.Name.Equals("BraumBasicAttackPassiveOverride", StringComparison.OrdinalIgnoreCase))
                {
                    if (SpellManager.E.Cast())
                        return;
                }

                //레오나 Q 평타 스턴 막기
                var option4 = Config.Auto.AutoE.TargetedSpells.Menu[$"Leona.AttackStun"];
                if (option4 != null && option4.GetValue<MenuBool>().Value
                    && args.SData.Name.Equals("LeonaShieldOfDaybreakAttack", StringComparison.OrdinalIgnoreCase))
                {
                    if (SpellManager.E.Cast())
                        return;
                }

                //레넥톤 W 평타 스턴 막기
                var option5 = Config.Auto.AutoE.TargetedSpells.Menu[$"Renekton.AttackStun"];
                if (option5 != null && option5.GetValue<MenuBool>().Value
                    && (args.SData.Name.Equals("RenektonExecute", StringComparison.OrdinalIgnoreCase) || args.SData.Name.Equals("RenektonSuperExecute", StringComparison.OrdinalIgnoreCase)))
                {
                    if (SpellManager.E.Cast())
                        return;
                }

                //가렌 Q 평타 막기
                var option6 = Config.Auto.AutoE.TargetedSpells.Menu[$"Garen.QAttack"];
                if (option6 != null && option6.GetValue<MenuBool>().Value
                    && args.SData.Name.Equals("GarenQAttack", StringComparison.OrdinalIgnoreCase))
                {
                    if (SpellManager.E.Cast())
                        return;
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Drawings.DrawQRange)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, SpellManager.Q.Range, System.Drawing.Color.DeepSkyBlue);
            }

            //if (Config.Drawings.DrawWRange)
            //{
            //    Render.Circle.DrawCircle(GameObjects.Player.Position, SpellManager.W.Range, System.Drawing.Color.DeepSkyBlue);
            //}

            //if (Config.Drawings.DrawERange)
            //{
            //    Render.Circle.DrawCircle(GameObjects.Player.Position, SpellManager.E.Range, System.Drawing.Color.DeepSkyBlue);
            //}

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
        {
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

                    if (Config.Modes.Combo.UseW && SpellManager.W.IsReady() && targetAsHero.InAutoAttackRange())
                    {
                        SpellManager.W.Cast();
                    }
                    break;
                case OrbwalkingMode.Hybrid:
                    if (targetAsHero == null)
                    {
                        //타겟이 챔피언이 아닐경우 브레이크
                        break;
                    }

                    if (Config.Modes.Combo.UseW && SpellManager.W.IsReady() && GameObjects.Player.ManaPercent > Config.Modes.Harass.MinMana && targetAsHero.InAutoAttackRange())
                    {
                        SpellManager.W.Cast();
                    }
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
                    if (targetAsMinion.GetJungleType().HasFlag(JungleType.Unknown))
                    {//정글몹이 아니면 - laneclear 
                        if (Config.Modes.LaneClear.UseQ && SpellManager.Q.IsReady() && GameObjects.Player.ManaPercent > Config.Modes.LaneClear.MinMana)
                        {
                            var farmPosition = SpellManager.Q.GetLineFarmLocation(GameObjects.EnemyMinions.Where(x => x.IsValidTarget(SpellManager.Q.Range)).ToList());
                            if (farmPosition.MinionsHit >= 3)
                            {
                                SpellManager.Q.Cast(farmPosition.Position);
                            }
                        }

                        if (Config.Modes.LaneClear.UseW && SpellManager.W.IsReady() && GameObjects.Player.ManaPercent > Config.Modes.LaneClear.MinMana)
                        {
                            SpellManager.W.Cast();
                        }
                    }
                    else
                    {//정글몹이면 - jungleclear

                        //간혹 V 키누르다 의도치 않게 정글몹에게 스킬을 사용하는 경우를 없애기위해.
                        //정글몹 콤보는 항상 평타 이후부터 시작하는걸로

                        if (Config.Modes.JungleClear.UseQ && SpellManager.Q.IsReady() && GameObjects.Player.ManaPercent > Config.Modes.JungleClear.MinMana)
                        {
                            SpellManager.Q.Cast(targetAsMinion);
                        }

                        if (Config.Modes.JungleClear.UseW && SpellManager.W.IsReady() && GameObjects.Player.ManaPercent > Config.Modes.JungleClear.MinMana)
                        {
                            SpellManager.W.Cast();
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

            if (Config.Misc.AntiGapcloser.UseR && SpellManager.R.IsReady() && e.End.DistanceToPlayer() <= 500)
            {
                SpellManager.R.Cast();
            }
        }

        //private static void Events_OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs e)
        //{
        //    if (!Config.Misc.AutoInterrupt.Enabled)
        //    {
        //        return;
        //    }

        //    if (Config.Misc.AutoInterrupt.UseQ &&
        //        SpellManager.Q.IsReady() &&
        //        e.Sender.IsValidTarget(SpellManager.Q.Range))
        //    {//타겟지정 필수
        //        SpellManager.Q.Cast();
        //    }
        //}

    }
}
