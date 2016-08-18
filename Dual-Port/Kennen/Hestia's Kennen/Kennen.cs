using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kennen
{
    internal class Kennen : Spells
    {
        public Kennen()
        {
            OnLoad();
        }

        private static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Kennen")
                return;

            InitializeSpells();
            ConfigMenu.InitializeMenu();

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.OnDraw;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit += CommonUtilities.GetComboDamage;

            Notifications.AddNotification("Kennen by Hestia loaded!", 5000);
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = ConfigMenu.config.Item("drawDmg").GetValue<bool>();

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
                case Orbwalking.OrbwalkingMode.CustomMode:
                    Flee();
                    break;
            }

            AutoQ();
            KillSteal();
            CastRmulti();
        }

        private static void ExecuteCombo()
        {
            var target = TargetSelector.GetTarget(q.Range, TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid)
                return;

            var castQ = ConfigMenu.config.Item("useQ").GetValue<bool>() && q.IsReady();
            var castW = ConfigMenu.config.Item("useW").GetValue<bool>() && w.IsReady();
            var modeW = ConfigMenu.config.Item("useWmodeCombo").GetValue<StringList>();
            var castR = ConfigMenu.config.Item("useR").GetValue<bool>() && r.IsReady();

            var useZhonya = ConfigMenu.config.Item("useZhonya").GetValue<bool>() && CommonUtilities.CheckZhonya();
            var zhonyaHp = ConfigMenu.config.Item("useZhonyaHp").GetValue<Slider>().Value;


            if (castQ && target.IsValidTarget(q.Range))
            {
                q.CastIfHitchanceEquals(target, CommonUtilities.GetHitChance("hitchanceQ"));
            }

            if (castW && target.IsValidTarget(w.Range))
            {
                switch (modeW.SelectedIndex)
                {
                    case 0:
                        if (HasMark(target))
                        {
                            w.Cast();
                        }
                        break;

                    case 1:
                        if (CanStun(target))
                        {
                            w.Cast();
                        }
                        break;
                }
            }

            if (castR && target.IsValidTarget(r.Range - 50))
            {
                if (target.Health > CommonUtilities.GetComboDamage(target))
                {
                    r.Cast();

                    if (HasR() && useZhonya && ObjectManager.Player.HealthPercent < zhonyaHp &&
                        ObjectManager.Player.CountEnemiesInRange(r.Range) > 0)
                    {
                        CommonUtilities.UseZhonya();
                    }
                }
            }
        }

        private static void ExecuteHarass()
        {
            var target = TargetSelector.GetTarget(q.Range, TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid)
                return;

            var castQ = ConfigMenu.config.Item("useQHarass").GetValue<bool>() && q.IsReady();
            var castW = ConfigMenu.config.Item("useWHarass").GetValue<bool>() && w.IsReady();
            var modeW = ConfigMenu.config.Item("useWmodeHarass").GetValue<StringList>();


            if (castQ && target.IsValidTarget(q.Range) && ObjectManager.Player.ManaPercent >= ConfigMenu.config.Item("useQHarassMana").GetValue<Slider>().Value)
            {
                q.CastIfHitchanceEquals(target, CommonUtilities.GetHitChance("hitchanceQ"));
            }

            if (castW && target.IsValidTarget(w.Range) && ObjectManager.Player.ManaPercent >= ConfigMenu.config.Item("useWHarassMana").GetValue<Slider>().Value)
            {
                switch (modeW.SelectedIndex)
                {
                    case 0:
                        if (HasMark(target))
                        {
                            w.Cast();
                        }
                        break;

                    case 1:
                        if (CanStun(target))
                        {
                            w.Cast();
                        }
                        break;
                }
            }
        }

        private static void LastHit()
        {
            var castQ = ConfigMenu.config.Item("useQlh").GetValue<bool>() && q.IsReady();

            if (!Orbwalking.CanMove(40))
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, q.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (minions.Count > 0 && castQ && ObjectManager.Player.ManaPercent >= ConfigMenu.config.Item("useQlhMana").GetValue<Slider>().Value)
            {
                foreach (var minion in minions)
                {
                    if (ConfigMenu.config.Item("qRange").GetValue<bool>())
                    {
                        if (HealthPrediction.GetHealthPrediction(minion, (int)(q.Delay + (minion.Distance(ObjectManager.Player.Position) / q.Speed))) < ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) && ObjectManager.Player.Distance(minion) > ObjectManager.Player.AttackRange)
                        {
                            q.Cast(minion);
                        }
                    }
                    else
                    {
                        if (HealthPrediction.GetHealthPrediction(minion, (int)(q.Delay + (minion.Distance(ObjectManager.Player.Position) / q.Speed))) < ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q))
                        {
                            q.Cast(minion);
                        }
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var castQ = ConfigMenu.config.Item("useQlc").GetValue<bool>() && q.IsReady();

            if (!Orbwalking.CanMove(40))
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, q.Range);

            if (minions.Count > 0 && castQ && ObjectManager.Player.ManaPercent >= ConfigMenu.config.Item("useQlcMana").GetValue<Slider>().Value)
            {
                foreach (var minion in minions)
                {
                    if (HealthPrediction.GetHealthPrediction(minion, (int)(q.Delay + (minion.Distance(ObjectManager.Player.Position) / q.Speed))) < ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q))
                    {
                        q.Cast(minion);
                    }
                }
            }
        }

        private static void JungleClear()
        {
            var castQ = ConfigMenu.config.Item("useQj").GetValue<bool>() && q.IsReady();
            var castW = ConfigMenu.config.Item("useWj").GetValue<bool>() && w.IsReady();

            if (!Orbwalking.CanMove(40))
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var minionsW = minions.Where(HasMark).Count();

            if (minions.Count > 0)
            {
                var minion = minions[0];

                if (castQ)
                {
                    q.Cast(minion);
                }
            }

            if (minionsW > 0)
            {
                if (castW)
                {
                    w.Cast();
                }
            }

        }

        private static void KillSteal()
        {
            if (!ConfigMenu.config.Item("killsteal").GetValue<bool>())
                return;

            if (ConfigMenu.config.Item("useQks").GetValue<bool>() && q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(q.Range) && !enemy.HasBuffOfType(BuffType.Invulnerability)).Where(target => target.Health < ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q)))
                {
                    q.CastIfHitchanceEquals(target, CommonUtilities.GetHitChance("hitchanceQ"));
                }
            }

            if (ConfigMenu.config.Item("useWks").GetValue<bool>() && q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(w.Range) && !enemy.HasBuffOfType(BuffType.Invulnerability)).Where(target => target.Health < ObjectManager.Player.GetSpellDamage(target, SpellSlot.W)))
                {
                    if (HasMark(target))
                    {
                        w.Cast();
                    }
                }
            }

            if (ConfigMenu.config.Item("useIks").GetValue<bool>() && ignite.Slot.IsReady() && ignite != null && ignite.Slot != SpellSlot.Unknown)
            {
                foreach (var target in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(ignite.SData.CastRange) && !enemy.HasBuffOfType(BuffType.Invulnerability)).Where(target => target.Health < ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)))
                {
                    ObjectManager.Player.Spellbook.CastSpell(ignite.Slot, target);
                }
            }
        }

        private static void AutoQ()
        {
            var autoQ = ConfigMenu.config.Item("autoQ").GetValue<KeyBind>().Active && q.IsReady();

            var target = TargetSelector.GetTarget(q.Range, TargetSelector.DamageType.Magical);

            if (autoQ && target.IsValidTarget(q.Range))
            {
                q.CastIfHitchanceEquals(target, CommonUtilities.GetHitChance("hitchanceQ"));
            }
        }

        private static void CastRmulti()
        {
            var castR = ConfigMenu.config.Item("useRmul").GetValue<bool>() && r.IsReady();
            var minR = ConfigMenu.config.Item("useRmulti").GetValue<Slider>().Value;
            var enemiesCount = ObjectManager.Player.CountEnemiesInRange(r.Range - 50);

            if (castR && enemiesCount >= minR)
            {
                r.Cast();
            }
        }

        private static void Flee()
        {
            var target = TargetSelector.GetTarget(q.Range, TargetSelector.DamageType.Magical);

            var useQ = ConfigMenu.config.Item("qFlee").GetValue<bool>() && q.IsReady();
            var useE = ConfigMenu.config.Item("eFlee").GetValue<bool>() && e.IsReady();

            if (useQ && target.IsValidTarget(q.Range))
            {
                q.Cast(target);
            }

            if (useE)
            {
                e.Cast();
            }
        }

        private static bool HasMark(Obj_AI_Base target)
        {
            return target.HasBuff("KennenMarkOfStorm");
        }

        private static bool HasR()
        {
            return ObjectManager.Player.HasBuff("KennenShurikenStorm");
        }

        private static bool CanStun(Obj_AI_Base target)
        {
            return target.GetBuffCount("KennenMarkOfStorm") == 2;
        }
    }
}
