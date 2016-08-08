using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace Mundo
{
    internal class Mundo : Spells
    {

        public Mundo()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "DrMundo")
                return;

            InitializeSpells();
            ConfigMenu.InitializeMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            Drawing.OnDraw += Drawings.OnDraw;

            Notifications.AddNotification("Dr.Mundo by Hestia loaded!", 5000);
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if ((ConfigMenu.orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || ConfigMenu.orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) && unit.IsMe)
            {
                if (ConfigMenu.config.Item("useE").GetValue<bool>() && e.LSIsReady() && target is AIHeroClient && target.LSIsValidTarget(e.Range))
                {
                    e.Cast();
                }

            }

            if (ConfigMenu.orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && unit.IsMe)
            {
                if (ConfigMenu.config.Item("useEj").GetValue<bool>() && e.LSIsReady() && target is Obj_AI_Minion && target.LSIsValidTarget(e.Range))
                {
                    e.Cast();
                }

            }

            if ((ConfigMenu.orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                 ConfigMenu.orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) && unit.IsMe)
            {
                if ((ConfigMenu.config.Item("titanicC").GetValue<bool>() || ConfigMenu.config.Item("ravenousC").GetValue<bool>() ||
                     ConfigMenu.config.Item("tiamatC").GetValue<bool>()) && !e.LSIsReady() && target is AIHeroClient &&
                    target.LSIsValidTarget(e.Range) && CommonUtilities.CheckItem())
                {
                    CommonUtilities.UseItem();
                }

            }

            if (ConfigMenu.orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && unit.IsMe)
            {
                if ((ConfigMenu.config.Item("titanicF").GetValue<bool>() || ConfigMenu.config.Item("ravenousF").GetValue<bool>() ||
                     ConfigMenu.config.Item("tiamatF").GetValue<bool>()) && !e.LSIsReady() && target is Obj_AI_Minion &&
                    target.LSIsValidTarget(e.Range) && CommonUtilities.CheckItem())
                {
                    CommonUtilities.UseItem();
                }

            }


        }

        private void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            switch (ConfigMenu.orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    ExecuteCombo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    LastHit();
                    ExecuteHarass();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    BurningManager();
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    Flee();
                    break;
            }

            AutoR();
            AutoQ();
            KillSteal();
        }

        private void ExecuteCombo()
        {
            var target = TargetSelector.GetTarget(q.Range, TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid)
                return;

            var castQ = ConfigMenu.config.Item("useQ").GetValue<bool>() && q.LSIsReady();
            var castW = ConfigMenu.config.Item("useW").GetValue<bool>() && w.LSIsReady();

            var qHealth = ConfigMenu.config.Item("QHealthCombo").GetValue<Slider>().Value;
            var wHealth = ConfigMenu.config.Item("WHealthCombo").GetValue<Slider>().Value;

            if (castQ && ObjectManager.Player.HealthPercent >= qHealth && target.LSIsValidTarget(q.Range))
            {
                q.CastIfHitchanceEquals(target, CommonUtilities.GetHitChance("hitchanceQ"));
            }

            if (castW && ObjectManager.Player.HealthPercent >= wHealth && !IsBurning() && target.LSIsValidTarget(400))
            {
                w.Cast();
            }
            else if (castW && IsBurning() && !FoundEnemies(450))
            {
                w.Cast();
            }
        }

        private void ExecuteHarass()
        {
            var target = TargetSelector.GetTarget(q.Range, TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid)
                return;

            var castQ = ConfigMenu.config.Item("useQHarass").GetValue<bool>() && q.LSIsReady();

            var qHealth = ConfigMenu.config.Item("useQHarassHP").GetValue<Slider>().Value;

            if (castQ && ObjectManager.Player.HealthPercent >= qHealth && target.LSIsValidTarget(q.Range))
            {
                q.CastIfHitchanceEquals(target, CommonUtilities.GetHitChance("hitchanceQ"));
            }
        }

        private void LastHit()
        {
            var castQ = ConfigMenu.config.Item("useQlh").GetValue<bool>() && q.LSIsReady();

            var qHealth = ConfigMenu.config.Item("useQlhHP").GetValue<Slider>().Value;

            if (!Orbwalking.CanMove(40))
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, q.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (minions.Count > 0 && castQ && ObjectManager.Player.HealthPercent >= qHealth)
            {
                foreach (var minion in minions)
                {
                    if (ConfigMenu.config.Item("qRange").GetValue<bool>())
                    {
                        if (HealthPrediction.GetHealthPrediction(minion, (int) (q.Delay + (minion.LSDistance(ObjectManager.Player.Position)/q.Speed))) < ObjectManager.Player.LSGetSpellDamage(minion, SpellSlot.Q) && ObjectManager.Player.LSDistance(minion) > ObjectManager.Player.AttackRange*2)
                        {
                            q.Cast(minion);
                        }
                    }
                    else
                    {
                        if (HealthPrediction.GetHealthPrediction(minion, (int) (q.Delay + (minion.LSDistance(ObjectManager.Player.Position)/q.Speed))) < ObjectManager.Player.LSGetSpellDamage(minion, SpellSlot.Q))
                        {
                            q.Cast(minion);
                        }
                    }
                }
            }
        }

        private void LaneClear()
        {
            var castQ = ConfigMenu.config.Item("useQlc").GetValue<bool>() && q.LSIsReady();
            var castW = ConfigMenu.config.Item("useWlc").GetValue<bool>() && w.LSIsReady();

            var qHealth = ConfigMenu.config.Item("useQlcHP").GetValue<Slider>().Value;
            var wHealth = ConfigMenu.config.Item("useWlcHP").GetValue<Slider>().Value;
            var wMinions = ConfigMenu.config.Item("useWlcMinions").GetValue<Slider>().Value;

            if (!Orbwalking.CanMove(40))
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, q.Range);
            var minionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 400);

            if (minions.Count > 0)
            {
                if (castQ && ObjectManager.Player.HealthPercent >= qHealth)
                {
                    foreach (var minion in minions)
                    {
                        if (HealthPrediction.GetHealthPrediction(minion, (int) (q.Delay + (minion.LSDistance(ObjectManager.Player.Position)/q.Speed))) < ObjectManager.Player.LSGetSpellDamage(minion, SpellSlot.Q))
                        {
                            q.Cast(minion);
                        }
                    }
                }
            }

            if (minionsW.Count >= wMinions)
            {
                if (castW && ObjectManager.Player.HealthPercent >= wHealth && !IsBurning())
                {
                    w.Cast();
                }
                else if (castW && IsBurning() && minions.Count < wMinions)
                {
                    w.Cast();
                }
            }
        }

        private void JungleClear()
        {
            var castQ = ConfigMenu.config.Item("useQj").GetValue<bool>() && q.LSIsReady();
            var castW = ConfigMenu.config.Item("useWj").GetValue<bool>() && w.LSIsReady();

            var qHealth = ConfigMenu.config.Item("useQjHP").GetValue<Slider>().Value;
            var wHealth = ConfigMenu.config.Item("useWjHP").GetValue<Slider>().Value;

            if (!Orbwalking.CanMove(40))
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var minionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 400, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (minions.Count > 0)
            {
                var minion = minions[0];

                if (castQ && ObjectManager.Player.HealthPercent >= qHealth)
                {
                    q.Cast(minion);
                }
            }

            if (minionsW.Count > 0)
            {
                if (castW && ObjectManager.Player.HealthPercent >= wHealth && !IsBurning())
                {
                    w.Cast();
                }
                else if (castW && IsBurning() && minionsW.Count < 1)
                {
                    w.Cast();
                }
            }
            
        }

        private void KillSteal()
        {
            if (!ConfigMenu.config.Item("killsteal").GetValue<bool>())
                return;

            if (ConfigMenu.config.Item("useQks").GetValue<bool>() && q.LSIsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(q.Range) && !enemy.HasBuffOfType(BuffType.Invulnerability)).Where(target => target.Health < ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.Q)))
                {
                    q.CastIfHitchanceEquals(target, CommonUtilities.GetHitChance("hitchanceQ"));
                }
            }

            if (ConfigMenu.config.Item("useIks").GetValue<bool>() && ignite.Slot.LSIsReady() && ignite != null && ignite.Slot != SpellSlot.Unknown)
            {
                foreach (var target in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(ignite.SData.CastRange) && !enemy.HasBuffOfType(BuffType.Invulnerability)).Where(target => target.Health < ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)))
                {
                    ObjectManager.Player.Spellbook.CastSpell(ignite.Slot, target);
                }
            }
        }

        private void AutoQ()
        {
            var autoQ = ConfigMenu.config.Item("autoQ").GetValue<KeyBind>().Active && q.LSIsReady();

            var qHealth = ConfigMenu.config.Item("autoQhp").GetValue<Slider>().Value;

            var target = TargetSelector.GetTarget(q.Range, TargetSelector.DamageType.Magical);

            if (autoQ && ObjectManager.Player.HealthPercent >= qHealth && target.LSIsValidTarget(q.Range))
            {
                q.CastIfHitchanceEquals(target, CommonUtilities.GetHitChance("hitchanceQ"));
            }
        }

        private void AutoR()
        {
            var castR = ConfigMenu.config.Item("useR").GetValue<bool>() && r.LSIsReady();

            var rHealth = ConfigMenu.config.Item("RHealth").GetValue<Slider>().Value;
            var rEnemies = ConfigMenu.config.Item("RHealthEnemies").GetValue<bool>();

            if (rEnemies && castR && ObjectManager.Player.HealthPercent <= rHealth && !ObjectManager.Player.LSInFountain())
            {
                if (FoundEnemies(q.Range))
                {
                    r.Cast();
                }
            }
            else if (!rEnemies && castR && ObjectManager.Player.HealthPercent <= rHealth && !ObjectManager.Player.LSInFountain())
            {
                r.Cast();
            }
        }

        private void Flee()
        {
            var target = TargetSelector.GetTarget(q.Range, TargetSelector.DamageType.Magical);

            var useQ = ConfigMenu.config.Item("qFlee").GetValue<bool>() && q.LSIsReady();
            var useR = ConfigMenu.config.Item("rFlee").GetValue<bool>() && r.LSIsReady();

            if (useQ && target.LSIsValidTarget(q.Range))
            {
                q.CastIfHitchanceEquals(target, CommonUtilities.GetHitChance("hitchanceQ"));
            }

            if (useR && FoundEnemies(q.Range*2))
            {
                r.Cast();
            }
        }

        public bool IsBurning()
        {
            return ObjectManager.Player.HasBuff("BurningAgony");
        }

        public bool FoundEnemies(float range)
        {
            return HeroManager.Enemies.Any(enemy => enemy.LSIsValidTarget(range));
        }

        private void BurningManager()
        {
            if (!ConfigMenu.config.Item("handleW").GetValue<bool>())
                return;
            
            if (IsBurning() && w.LSIsReady())
            {
                w.Cast();
            }
        }
    }
}