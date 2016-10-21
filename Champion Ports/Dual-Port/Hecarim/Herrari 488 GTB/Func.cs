using System;
using System.Collections.Generic;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Herrari_488_GTB
{
    static class Func
    {
        internal static Menu Menu { get { return MenuProvider.MenuInstance.SubMenu("Champion"); } }
        static AIHeroClient Player { get { return ObjectManager.Player; } }
        static AIHeroClient ShieldTarget;
        static Orbwalking.Orbwalker Orbwalker { get { return MenuProvider.Orbwalker; } }
        static int AttackTime;

        internal static void Load()
        {
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        internal static void Shield(this Spell spell, float EffectRadius = 0f)
        {
            var SR = Math.Max(spell.Range, Player.AttackRange);
            if (ShieldTarget != null && ShieldTarget.Distance(Player.ServerPosition) <= SR && spell.IsReady()
            && AttackTime - Utils.GameTimeTickCount + 250 >= 0)
            {
                if (spell.IsSkillshot)
                { }
                else if (false == spell.IsSkillshot)
                {
                    if (EffectRadius == 0f)
                        spell.Cast(ShieldTarget);
                    else //차후 추가예정.
                        return;
                }
                else if (ShieldTarget.IsMe)
                    spell.Cast();
            }
        }

        internal static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Player.IsDead)
                return;
            if (Player.Distance(args.End) > 1000)
                return;
            if (sender.IsMe)
                return;
            var HeroSender = sender as AIHeroClient;
            var Target = args.Target as AIHeroClient;
            if (HeroSender == null)
                return;

            if (HeroSender != null && Target != null && !Orbwalking.IsOnHit(args.SData.Name) && Target.IsAlly && !HeroSender.IsAlly && Target.Distance(Player.ServerPosition) <= 1000)
            {
                ShieldTarget = Target;
                AttackTime = Utils.GameTimeTickCount;
            }
            if (HeroSender != null && !Orbwalking.IsOnHit(args.SData.Name) && HeroManager.Allies.Where(x => x.Distance(args.End) <= 80 && x.Distance(Player.ServerPosition) <= 1000).Count() > 0)
            {
                ShieldTarget = HeroManager.Allies.Where(x => x.Distance(args.End) <= 80).FirstOrDefault();
                AttackTime = Utils.GameTimeTickCount;
            }
        }

        internal static List<Obj_AI_Base> getCollisionMinions(AIHeroClient source, SharpDX.Vector3 targetPos, float predDelay, float predWidth, float predSpeed)
        {
            var input = new PredictionInput
            {
                Unit = source,
                Radius = predWidth,
                Delay = predDelay,
                Speed = predSpeed,
            };

            input.CollisionObjects[0] = CollisionableObjects.Minions;

            return Collision.GetCollision(new List<SharpDX.Vector3> { targetPos }, input).OrderBy(obj => obj.Distance(source, false)).ToList();
        }

        internal static bool HasBuff2(this Obj_AI_Base unit,
            string buffName,
            bool dontUseDisplayName = false)
        {
            return
                unit.Buffs.Any(
                    buff =>
                        ((dontUseDisplayName &&
                          String.Equals(buff.Name, buffName, StringComparison.CurrentCultureIgnoreCase)) ||
                         (!dontUseDisplayName &&
                          String.Equals(buff.DisplayName, buffName, StringComparison.CurrentCultureIgnoreCase))) &&
                        buff.IsValidBuff());
        }

        internal static BuffInstance getBuffInstance(Obj_AI_Base target, string buffName)
        {
            return target.Buffs.Find(x => x.Name == buffName && x.IsValidBuff());
        }

        internal static BuffInstance getBuffInstance(Obj_AI_Base target, string buffName, Obj_AI_Base buffCaster)
        {
            return target.Buffs.Find(x => x.Name == buffName && x.Caster.NetworkId == buffCaster.NetworkId && x.IsValidBuff());
        }

        internal static bool isKillable(Obj_AI_Base target, float damage)
        {
            return target.Health + (target.HPRegenRate / 2) <= damage;
        }

        internal static bool isKillable(Obj_AI_Base target, Spell spell, int stage = 0)
        {
            return target.Health + (target.HPRegenRate / 2) <= spell.GetDamage(target, stage);
        }

        internal static void sendDebugMsg(string message, string tag = "Herrari 488 GTB: ")
        {
            Console.WriteLine(tag + message);

            if (Menu.Item("Misc.Notify Debug Message", true) != null &&
                Menu.Item("Misc.Notify Debug Message", true).GetValue<bool>())
                Notifications.AddNotification(tag + message, 4000);
        }

        internal static bool anyoneValidInRange(float range)
        {
            return HeroManager.Enemies.Any(x => x.IsValidTarget(range));
        }

        internal static float GetDamageCalc(Obj_AI_Base Sender, Obj_AI_Base Target, Damage.DamageType Type, double Equation = 0d)
        {
            return (float)Damage.CalcDamage(Sender, Target, Type, Equation);
        }


        internal static bool CanHit(this Spell spell, Obj_AI_Base T, float Drag = 0f)
        {
            return T.IsValidTarget(spell.Range + Drag - ((T.Distance(Player.ServerPosition) - spell.Range) / spell.Speed + spell.Delay) * T.MoveSpeed);
        }

        internal static float PredHealth(Obj_AI_Base Target, Spell spell)
        {
            return Pred.PredHealth(Target, spell);
        }

        internal static void MotionCancel()
        {
            Chat.Say("/d");
        }

        internal static void AALcJc(this Spell spell, float ExtraTargetDistance = 150f, float ALPHA = float.MaxValue, float Cost = 1f, float BombRadius = 0f) //지금으로선 새 방식으로 메뉴 만든 경우에만 사용가능. AALaneclear AAJungleclear 대체
        {// 아주 편하게 평캔 Lc, Jc를 구현할수 있습니다(그것도 분리해서!!). 그냥 AIO_Func.AALcJc(Q); 이렇게 쓰세요. 선형 스킬일 경우 세부 설정을 원할 경우 AIO_Func.AALcJc(E,ED,0f); 이런식으로 쓰세요.
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var Minions = MinionManager.GetMinions(Math.Max(spell.Range, Orbwalking.GetRealAutoAttackRange(Player)), MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                var Mobs = MinionManager.GetMinions(Math.Max(spell.Range, Orbwalking.GetRealAutoAttackRange(Player)), MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                bool HM = true;
                bool LM = true;
                bool JM = true;
                bool LHM = true;
                if (Cost == 1f)
                {
                    HM = (Menu.Item("Harass.If Mana >", true) != null ? (Player.ManaPercent > MenuProvider.Champion.Harass.IfMana) : true);
                    LM = (Menu.Item("Laneclear.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Laneclear.IfMana : true);
                    JM = (Menu.Item("Jungleclear.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Jungleclear.IfMana : true);
                    LHM = (Menu.Item("Lasthit.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Lasthit.IfMana : true);
                }
                if (Mobs.Count > 0 && Menu.Item("Jungleclear.Use " + spell.Slot.ToString(), true) != null)
                {
                    if (Menu.Item("Jungleclear.Use " + spell.Slot.ToString(), true).GetValue<bool>()// || Menu.Item("JcUse" + spell.Slot.ToString(), true).GetValue<bool>())
                    && spell.IsReady() && JM)
                    {
                        if (spell.IsSkillshot)
                        {
                            if (spell.Type == SkillshotType.SkillshotLine)
                                LCast(spell, Mobs[0], ExtraTargetDistance, ALPHA);
                            else if (spell.Type == SkillshotType.SkillshotCircle)
                                CCast(spell, Mobs[0]);
                            else if (spell.Type == SkillshotType.SkillshotCone)
                                spell.Cast(Mobs[0]);
                        }
                        else if (false == spell.IsSkillshot)
                            spell.Cast(Mobs[0]);
                        else
                            spell.Cast();
                    }
                }
                if (Minions.Count > 0 && Menu.Item("Laneclear.Use " + spell.Slot.ToString(), true) != null)
                {
                    if (Menu.Item("Laneclear.Use " + spell.Slot.ToString(), true).GetValue<bool>() //  || Menu.Item("LcUse" + spell.Slot.ToString(), true).GetValue<bool>())
                    && spell.IsReady() && LM)
                    {
                        if (spell.IsSkillshot)
                        {
                            if (spell.Type == SkillshotType.SkillshotLine)
                            {
                                if (ALPHA > 1f)
                                    LCast(spell, Minions[0], ExtraTargetDistance, ALPHA, false, BombRadius);
                                else
                                    LH(spell, ALPHA);
                            }
                            else if (spell.Type == SkillshotType.SkillshotCircle)
                                CCast(spell, Minions[0]);
                            else if (spell.Type == SkillshotType.SkillshotCone)
                                spell.Cast(Minions[0]);
                        }
                        else if (false == spell.IsSkillshot)
                            spell.Cast(Minions[0]);
                        else
                            spell.Cast();
                    }
                }
            }
        }

        internal static void AACb(this Spell spell, float ExtraTargetDistance = 150f, float ALPHA = float.MaxValue, float Cost = 1f, float BombRadius = 0f) //지금으로선 새 방식으로 메뉴 만든 경우에만 사용가능.
        { // 아주 편하게 평캔 Cb, Hrs를 구현할수 있습니다. 그냥 AIO_Func.AACb(Q); 이렇게 쓰세요. Line 스킬일 경우에만 AIO_Func.AACb(E,ED,0f) 이런식으로 쓰시면 됩니다.
            var target = TargetSelector.GetTarget(Math.Max(spell.Range, Orbwalking.GetRealAutoAttackRange(Player)), (spell.DamageType), true); //
            bool HM = true;
            bool LM = true;
            bool JM = true;
            bool LHM = true;
            if (Cost == 1f)
            {
                HM = (Menu.Item("Harass.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Harass.IfMana : true);
                LM = (Menu.Item("Laneclear.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Laneclear.IfMana : true);
                JM = (Menu.Item("Jungleclear.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Jungleclear.IfMana : true);
                LHM = (Menu.Item("Lasthit.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Lasthit.IfMana : true);
            } //굳이 switch문을 쓸 필요가 없음. 또한 false == spell.IsSkillshot을 !spell.IsSkillshot로 고치면 바로 밑의 else가 존재하지 않게 되어 액티브 스킬 사용불가하게됨.
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Menu.Item("Combo.Use " + spell.Slot.ToString(), true) != null)
            {
                if (Menu.Item("Combo.Use " + spell.Slot.ToString(), true).GetValue<bool>() // || Menu.Item("CbUse" + spell.Slot.ToString(), true).GetValue<bool>()) 구버전 지원 중단
                && spell.IsReady())
                {
                    if (spell.IsSkillshot)
                    {
                        if (spell.Type == SkillshotType.SkillshotLine) // 선형 스킬일경우
                            LCast(spell, target, ExtraTargetDistance, ALPHA);
                        else if (spell.Type == SkillshotType.SkillshotCircle) // 원형 스킬일경우
                            CCast(spell, target);
                        else if (spell.Type == SkillshotType.SkillshotCone) //원뿔 스킬
                            spell.Cast(target);
                    }
                    else if (false == spell.IsSkillshot) //스킬샷이 아닐떄
                        spell.Cast(target);
                    else //스킬샷 데이터가 null일때
                        spell.Cast();
                }
            }
            else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && Menu.Item("Harass.Use " + spell.Slot.ToString(), true) != null)
            {
                if (Menu.Item("Harass.Use " + spell.Slot.ToString(), true).GetValue<bool>()
                && spell.IsReady() && HM)
                {
                    if (spell.IsSkillshot)
                    {
                        if (spell.Type == SkillshotType.SkillshotLine) // 선형 스킬일경우
                            LCast(spell, target, ExtraTargetDistance, ALPHA);
                        else if (spell.Type == SkillshotType.SkillshotCircle) // 원형 스킬일경우
                            CCast(spell, target);
                        else if (spell.Type == SkillshotType.SkillshotCone) //원뿔 스킬
                            spell.Cast(target);
                    }
                    else if (false == spell.IsSkillshot)
                        spell.Cast(target);
                    else
                        spell.Cast();
                }
            }
        }

        internal static void LH(this Spell spell, float ALPHA = 0f) // For Last hit with skill for farming 사용법은 매우 간단. AIO_Func.LH(Q,0) or AIO_Func(Q,float.MaxValue) 이런식으로. 럭스나 베이가같이 타겟이 둘 가능할 경우엔 AIO_Func.LH(Q,1) 이런식.
        {
            var M = MinionManager.GetMinions(Math.Max(spell.Range, Orbwalking.GetRealAutoAttackRange(Player)), MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.Health).FirstOrDefault(m => isKillable(m, spell, 0) && HealthPrediction.GetHealthPrediction(m, (int)(Player.Distance(m, false) / spell.Speed), (int)(spell.Delay * 1000 + Game.Ping / 2)) > 0
            && (m != Orbwalking._lastTarget || m == Orbwalking._lastTarget && !isKillable(m, (float)Player.GetAutoAttackDamage(m, true))));
            if (spell.IsReady() && M != null)
            {
                if (spell.IsSkillshot)
                {
                    if (spell.Type == SkillshotType.SkillshotLine) // 선형 스킬일경우 위에 MinionOrderTypes.MaxHealth 없애서 기본값으로 바꿨음 막타잘치게 NotAlly
                        LCast(spell, M, 50f, ALPHA);
                    else if (spell.Type == SkillshotType.SkillshotCircle) // 원형 스킬일경우
                        CCast(spell, M);
                    else if (spell.Type == SkillshotType.SkillshotCone) //원뿔 스킬
                        spell.ConeCast(M, 50f, ALPHA);
                }
                else if (spell.IsChargedSpell)
                {
                    if (spell.IsCharging)
                        spell.Cast(M);
                    else
                        spell.StartCharging();
                }
                else if (false == spell.IsSkillshot)
                    spell.Cast(M);
                else
                    spell.Cast();
            }
        }

        internal static void SC(this Spell spell, float ExtraTargetDistance = 150f, float ALPHA = float.MaxValue, float Cost = 1f, float BombRadius = 0f, float 난사 = 0f) //
        { // 
            var target = TargetSelector.GetTarget(Math.Max(spell.Range, Orbwalking.GetRealAutoAttackRange(Player)), (spell.DamageType), true); //
            bool HM = true;
            bool LM = true;
            bool JM = false;
            bool LHM = false;
            if (Cost == 1f)
            {
                HM = (Menu.Item("Harass.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Harass.IfMana : true);
                LM = (Menu.Item("Laneclear.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Laneclear.IfMana : true);
                JM = (Menu.Item("Jungleclear.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Jungleclear.IfMana : true);
                LHM = (Menu.Item("Lasthit.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Lasthit.IfMana : true);
            }
            if (target != null)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Menu.Item("Combo.Use " + spell.Slot.ToString(), true) != null)
                {
                    if (Menu.Item("Combo.Use " + spell.Slot.ToString(), true).GetValue<bool>()
                    && spell.IsReady())
                    {
                        if (spell.IsSkillshot)
                        {
                            if (spell.Type == SkillshotType.SkillshotLine)
                                LCast(spell, target, ExtraTargetDistance, ALPHA, false, BombRadius);
                            else if (spell.Type == SkillshotType.SkillshotCircle)
                            {
                                var ctarget = TargetSelector.GetTarget(spell.Range + spell.Width / 2, spell.DamageType, true);
                                CCast(spell, ctarget);
                            }
                            else if (spell.Type == SkillshotType.SkillshotCone)
                                spell.ConeCast(target, ExtraTargetDistance, ALPHA);

                        }
                        else if (spell.IsChargedSpell)
                        {
                            if (!spell.IsCharging)
                                spell.StartCharging();
                        }
                        else
                        {
                            if (false == spell.IsSkillshot)
                                spell.Cast(target);
                            else
                                spell.AOECast(target);
                        }
                    }
                }
                else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && Menu.Item("Harass.Use " + spell.Slot.ToString(), true) != null)
                {
                    if (Menu.Item("Harass.Use " + spell.Slot.ToString(), true).GetValue<bool>()
                    && spell.IsReady() && HM)
                    {
                        if (spell.IsSkillshot)
                        {
                            if (spell.Type == SkillshotType.SkillshotLine)
                                LCast(spell, target, ExtraTargetDistance, ALPHA, false, BombRadius);
                            else if (spell.Type == SkillshotType.SkillshotCircle)
                            {
                                var ctarget = TargetSelector.GetTarget(spell.Range + spell.Width / 2, spell.DamageType, true);
                                CCast(spell, ctarget);
                            }
                            else if (spell.Type == SkillshotType.SkillshotCone)
                                spell.ConeCast(target, ExtraTargetDistance, ALPHA);
                        }
                        else if (spell.IsChargedSpell)
                        {
                            if (!spell.IsCharging)
                                spell.StartCharging();
                        }
                        else
                        {
                            if (false == spell.IsSkillshot)
                                spell.Cast(target);
                            else
                                spell.AOECast(target);
                        }
                    }
                }
                else if (Menu.Item("Harass.Auto Harass", true) != null)
                {
                    if (Menu.Item("Harass.Auto Harass", true).GetValue<bool>() && Menu.Item("Harass.Use " + spell.Slot.ToString(), true) != null)
                    {
                        if (Menu.Item("Harass.Use " + spell.Slot.ToString(), true).GetValue<bool>()
                        && spell.IsReady() && HM)
                        {
                            if (spell.IsSkillshot)
                            {
                                if (spell.Type == SkillshotType.SkillshotLine)
                                    LCast(spell, target, ExtraTargetDistance, ALPHA, false, BombRadius);
                                else if (spell.Type == SkillshotType.SkillshotCircle)
                                {
                                    var ctarget = TargetSelector.GetTarget(spell.Range + spell.Width / 2, spell.DamageType, true);
                                    CCast(spell, ctarget);
                                }
                                else if (spell.Type == SkillshotType.SkillshotCone)
                                    spell.ConeCast(target, ExtraTargetDistance, ALPHA);
                            }
                            /*else if(spell.IsChargedSpell)
                            { 오토하레스 차징 스킬은 음 일단 잠시 보류.
                                if(!spell.IsCharging)
                                spell.StartCharging();
                            }*/
                            else
                            {
                                if (false == spell.IsSkillshot)
                                    spell.Cast(target);
                                else
                                    spell.AOECast(target);
                            }
                        }
                    }
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var Minions = MinionManager.GetMinions(Math.Max(spell.Range, Orbwalking.GetRealAutoAttackRange(Player)), MinionTypes.All, MinionTeam.Enemy);
                var Mobs = MinionManager.GetMinions(Math.Max(spell.Range, Orbwalking.GetRealAutoAttackRange(Player)), MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                if (Mobs.Count > 0 && Menu.Item("Jungleclear.Use " + spell.Slot.ToString(), true) != null)
                {
                    if (Menu.Item("Jungleclear.Use " + spell.Slot.ToString(), true).GetValue<bool>()
                    && spell.IsReady() && JM)
                    {
                        if (spell.IsSkillshot)
                        {
                            if (spell.Type == SkillshotType.SkillshotLine)
                                LCast(spell, Mobs[0], ExtraTargetDistance, ALPHA, false, BombRadius);
                            else if (spell.Type == SkillshotType.SkillshotCircle)
                                CCast(spell, Mobs[0]);
                            else if (spell.Type == SkillshotType.SkillshotCone)
                                spell.ConeCast(Mobs[0], ExtraTargetDistance, ALPHA);
                        }
                        else if (spell.IsChargedSpell)
                        {
                            if (!spell.IsCharging)
                                spell.StartCharging();
                        }
                        else
                        {
                            if (false == spell.IsSkillshot)
                                spell.Cast(Mobs[0]);
                            else
                                spell.AOECast(Mobs[0]);
                        }
                    }
                }
                if (Minions.Count > 0 && Menu.Item("Laneclear.Use " + spell.Slot.ToString(), true) != null)
                {
                    if (Menu.Item("Laneclear.Use " + spell.Slot.ToString(), true).GetValue<bool>()
                    && spell.IsReady() && LM)
                    {
                        if (spell.IsSkillshot)
                        {
                            if (spell.Type == SkillshotType.SkillshotLine)
                            {
                                if (ALPHA > 1f)
                                    LCast(spell, Minions[0], ExtraTargetDistance, ALPHA, false, BombRadius);
                                else
                                    LH(spell, ALPHA);
                            }
                            else if (spell.Type == SkillshotType.SkillshotCircle)
                                CCast(spell, Minions[0]);
                            else if (spell.Type == SkillshotType.SkillshotCone)
                                spell.ConeCast(Minions[0], ExtraTargetDistance, ALPHA);
                        }
                        else if (spell.IsChargedSpell)
                        {
                            if (!spell.IsCharging && Minions.Count() > 2)
                                spell.StartCharging();
                        }
                        else
                        {
                            if (난사 == 0f)
                                LH(spell);
                            else
                            {
                                if (false == spell.IsSkillshot)
                                    spell.Cast(Minions[0]);
                                else
                                    spell.AOECast(Minions[0]);
                            }
                        }
                    }
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit && Menu.Item("Lasthit.Use " + spell.Slot.ToString(), true) != null)
            {
                if (Menu.Item("Lasthit.Use " + spell.Slot.ToString(), true).GetValue<bool>() && spell.IsReady() && LHM)
                {
                    var Mini = MinionManager.GetMinions(Math.Max(Orbwalking.GetRealAutoAttackRange(Player), spell.Range), MinionTypes.All, MinionTeam.NotAlly);
                    if (Mini.Count() > 0)
                        LH(spell, ALPHA);
                }
            }
            if (spell.IsChargedSpell && spell.IsCharging) // 따로 해놔야 스킬 써서 마나가 ifMana보다 적어졌거나 타겟이 사라져 새 타겟이 필요할때 혹은 스킬을 직접 캐스팅했을 때에도 대응 가능.
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    var TG = TargetSelector.GetTarget(spell.ChargedMaxRange, spell.DamageType); //타겟을 다시 잡게 지정해야함. 그래야 타겟이 사라져 새 타겟을 잡아야 할 때 멍때리고 움직이지도 못하는 병신같은 일 방지 가능.
                    if (TG != null)
                        spell.LCast(TG, ExtraTargetDistance, ALPHA, false, BombRadius);
                    /*else 있어도 상관은 없지만. Flee 모드 추가함.
                    {
                        Player.Spellbook.CastSpell(SpellSlot.Recall); //귀환으로 차지 취소.
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }*/
                }
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    var M = MinionManager.GetMinions(Player.ServerPosition, spell.ChargedMaxRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                    var MI = MinionManager.GetMinions(Player.ServerPosition, spell.ChargedMaxRange, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
                    var Vec = spell.GetLineFarmLocation(MI);
                    var Vec2 = spell.GetLineFarmLocation(M);
                    if (MI.Count() >= 1 && Vec.MinionsHit >= Math.Min(MI.Count(), 7) && Vec.Position.IsValid())
                        spell.Cast(Vec.Position);
                    if (M.Count() >= 1 && Vec2.MinionsHit >= Math.Min(M.Count(), 4) && Vec2.Position.IsValid())
                        spell.Cast(Vec2.Position);
                }
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Flee)
                {
                    Player.Spellbook.CastSpell(SpellSlot.Recall); //귀환으로 차지 취소.
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
            }
        }

        internal static void MouseSC(this Spell spell, float Cost = 1f) // 베인 니달리 리븐 등등.,.,
        {
            AIHeroClient target = null;
            float TRange = 500f; // spell.Range
            target = TargetSelector.GetTarget(Math.Max(Orbwalking.GetRealAutoAttackRange(Player), spell.Range) + 300f, (spell.DamageType), true);
            bool HM = true;
            bool LM = true;
            bool JM = false;
            bool LHM = false;
            if (Cost == 1f)
            {
                HM = (Menu.Item("Harass.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Harass.IfMana : true);
                LM = (Menu.Item("Laneclear.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Laneclear.IfMana : true);
                JM = (Menu.Item("Jungleclear.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Jungleclear.IfMana : true);
                LHM = (Menu.Item("Lasthit.If Mana >", true) != null ? Player.ManaPercent > MenuProvider.Champion.Lasthit.IfMana : true);
            }
            if (target != null)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Menu.Item("Combo.Use " + spell.Slot.ToString(), true) != null)
                {
                    if (Menu.Item("Combo.Use " + spell.Slot.ToString(), true).GetValue<bool>()
                    && spell.IsReady())
                        spell.Cast(Game.CursorPos);
                }
                else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && Menu.Item("Harass.Use " + spell.Slot.ToString(), true) != null)
                {
                    if (Menu.Item("Harass.Use " + spell.Slot.ToString(), true).GetValue<bool>()
                    && spell.IsReady() && HM)
                        spell.Cast(Game.CursorPos);
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var Minions = MinionManager.GetMinions(TRange, MinionTypes.All, MinionTeam.Enemy);
                var Mobs = MinionManager.GetMinions(TRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                if (Mobs.Count > 0 && Menu.Item("Jungleclear.Use " + spell.Slot.ToString(), true) != null)
                {
                    if (Menu.Item("Jungleclear.Use " + spell.Slot.ToString(), true).GetValue<bool>()
                    && spell.IsReady() && JM)
                        spell.Cast(Game.CursorPos);
                }
                if (Minions.Count > 0 && Menu.Item("Laneclear.Use " + spell.Slot.ToString(), true) != null)
                {
                    if (Menu.Item("Laneclear.Use " + spell.Slot.ToString(), true).GetValue<bool>()
                    && spell.IsReady() && LM)
                        spell.Cast(Game.CursorPos);
                }
            }
        }

        internal static void Heal(this Spell spell, float Mana = 40, float Max = 60, float Cost = 1f)
        {
            bool M = true;
            if (Cost == 1f)
                M = Player.ManaPercent > Mana;
            else
                M = true;
            foreach (var Ally in HeroManager.Allies.Where(x => x.Distance(Player.ServerPosition) <= spell.Range && x.HealthPercent < Max && (Player.ChampionName == "Soraka" ? x != Player : x != null))) //소라카는 자신을 힐 못하니까!
            {
                if (spell.IsReady() && M && Ally != null)
                    spell.Cast(Ally);
            }
        }

        internal static List<AIHeroClient> GetEnemyList()// 어짜피 원 기능은 중복되니 추가적으로 옵션을 줌.
        {
            return ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.IsValid && !x.IsDead && !x.IsInvulnerable).ToList();
        }

        internal static int EnemyCount(this float range, float min = 0, float max = 100)// 어짜피 원 기능은 중복되니 추가적으로 옵션을 줌. 특정 체력% 초과 특정 체력% 이하의 적챔프 카운트
        {
            return GetEnemyList().Where(x => x.Distance(Player.ServerPosition) <= range && x.HealthPercent > min && x.HealthPercent <= max).Count();
        }

        internal static int ECTarget(this Obj_AI_Base target, float range, float min = 0, float max = 100)// 어짜피 원 기능은 중복되니 추가적으로 옵션을 줌. 특정 체력% 초과 특정 체력% 이하의 적챔프 카운트
        {
            return GetEnemyList().Where(x => x.Distance(target.ServerPosition) <= range && x.HealthPercent > min && x.HealthPercent <= max).Count();
        }

        internal static double UnitIsImmobileUntil(Obj_AI_Base unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                        buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                         buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare))
                    .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return (result - Game.Time);
        }

        internal static bool CollisionCheck(AIHeroClient source, AIHeroClient target, float width)
        {
            var input = new PredictionInput
            {
                Radius = width,
                Unit = source,
            };

            input.CollisionObjects[0] = CollisionableObjects.Heroes;
            input.CollisionObjects[1] = CollisionableObjects.YasuoWall;

            return Collision.GetCollision(new List<SharpDX.Vector3> { target.ServerPosition }, input).Where(x => x.NetworkId != source.NetworkId && x.NetworkId != target.NetworkId).Any(); // && x.NetworkId != target.NetworkId가 없을 경우 절대로 스킬을 쓰지 않기 때문에 추가.
        }

        internal static bool CollisionCheck(SharpDX.Vector3 from, AIHeroClient target, float width)
        {
            var input = new PredictionInput
            {
                Radius = width,
                From = from
            };

            input.CollisionObjects[0] = CollisionableObjects.Heroes;

            return Collision.GetCollision(new List<SharpDX.Vector3> { target.ServerPosition }, input).Where(x => x.NetworkId != Player.NetworkId && x.NetworkId != target.NetworkId).Any(); // && x.NetworkId != target.NetworkId가 없을 경우 절대로 스킬을 쓰지 않기 때문에 추가.
        }

        internal static bool YasuoWallCheck(AIHeroClient source, AIHeroClient target, float width)
        {
            var input = new PredictionInput
            {
                Radius = width,
                Unit = source
            };

            input.CollisionObjects[0] = CollisionableObjects.YasuoWall;

            return Collision.GetCollision(new List<SharpDX.Vector3> { target.ServerPosition }, input).Any();
        }

        internal static int CountEnemyMinionsInRange(this SharpDX.Vector3 point, float range)
        {
            return ObjectManager.Get<Obj_AI_Minion>().Count(h => h.IsValidTarget(range, true, point));
        }

        internal class SelfAOE_Prediction
        {
            internal static int HitCount(float delay, float range)
            {
                byte hitcount = 0;

                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(range, false)))
                {
                    var pred = Prediction.GetPrediction(enemy, delay);

                    if (Player.ServerPosition.Distance(pred.UnitPosition) <= range)
                        hitcount++;
                }

                return hitcount;
            }

            internal static int HitCount(float delay, float range, SharpDX.Vector3 sourcePosition)
            {
                byte hitcount = 0;

                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(range, false, sourcePosition)))
                {
                    var pred = Prediction.GetPrediction(enemy, delay);

                    if (sourcePosition.Distance(pred.UnitPosition) <= range)
                        hitcount++;
                }

                return hitcount;
            }
        }

        internal static void CCast(Spell spell, Obj_AI_Base target) //for Circular spells
        {
            Pred.CCast(spell, target);
        }
        internal static void LCast(Spell spell, Obj_AI_Base target, float alpha = 0f, float colmini = float.MaxValue, bool HeroOnly = false, float BombRadius = 0f) //for Linar spells  사용예시 AIO_Func.LCast(Q,Qtarget,50,0)  
        {
            Pred.LCast(spell, target, alpha, colmini, HeroOnly, BombRadius);
        }
        internal static void RMouse(Spell spell)
        {
            Pred.RMouse(spell);
        }
        internal static void AtoB(Spell spell, Obj_AI_Base T, float Drag = 700f) //Coded By RL244 AtoB Drag 기본값 700f는 빅토르를 위한 것임.
        {
            Pred.AtoB(spell, T, Drag);
        }
        internal static void FleeToPosition(Spell spell, string W = "N") // N 정방향, R 역방향.
        {
            Pred.FleeToPosition(spell, W);
        }
        internal static bool InAARange(this Obj_AI_Base T)
        {
            return Player.Distance(T.ServerPosition) <= Orbwalking.GetRealAutoAttackRange(Player);
        }
    }
}
