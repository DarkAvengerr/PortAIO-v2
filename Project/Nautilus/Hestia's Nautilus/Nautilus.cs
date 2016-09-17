using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HestiaNautilus
{
    internal class Nautilus : Spells
    {
        public Nautilus()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Nautilus")
                return;

            InitializeSpells();
            ConfigMenu.InitializeMenu();

            Game.OnUpdate += OnUpdate;
            Interrupter2.OnInterruptableTarget += OnInterruptable;
            
            Drawing.OnDraw += Drawings.OnDraw;

            Notifications.AddNotification("Nautilus by Hestia loaded!", 5000);
        }

        private void OnInterruptable(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!ConfigMenu.config.Item("useQinterrupt").GetValue<bool>())
                return;

            if (args.DangerLevel != Interrupter2.DangerLevel.High)
                return;

            if (sender.Distance(ObjectManager.Player) <= q.Range && sender.IsEnemy && sender.IsValidTarget())
            {
                var qPrediction =
                    q.GetPrediction(sender, false, 0,
                        new[]
                        {
                            CollisionableObjects.Walls, CollisionableObjects.Heroes, CollisionableObjects.YasuoWall,
                            CollisionableObjects.Minions,
                        }).Hitchance;

                if (qPrediction >= HitChance.High)
                {
                    q.Cast(sender);
                }
                else if (qPrediction == HitChance.Impossible && sender.Distance(ObjectManager.Player) <= r.Range && ConfigMenu.config.Item("useRinterrupt").GetValue<bool>())
                {
                    r.CastOnUnit(sender);
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
                    ExecuteHarass();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
            }
            KillSteal();
        }

        private void ExecuteCombo()
        {
            var target = TargetSelector.GetTarget(q.Range, TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid)
                return;

            var castQ = ConfigMenu.config.Item("useQ").GetValue<bool>() && q.IsReady();
            var castW = ConfigMenu.config.Item("useW").GetValue<bool>() && w.IsReady();
            var castE = ConfigMenu.config.Item("useE").GetValue<bool>() && e.IsReady();
            var castR = ConfigMenu.config.Item("useR").GetValue<bool>() && r.IsReady();

            var wHealth = ConfigMenu.config.Item("WHealthCombo").GetValue<Slider>().Value;

            if (castQ && target.IsValidTarget(q.Range))
            {
                q.CastIfHitchanceEquals(target, CommonUtilities.GetHitChance("qHitchance"));
            }

            if (wHealth == 0 && castW && target.IsValidTarget(e.Range))
            {
                w.Cast();
            }
            else
            {
                if (ObjectManager.Player.HealthPercent <= wHealth && castW && target.IsValidTarget(e.Range))
                {
                    w.Cast();
                }
            }


            if (castE && target.IsValidTarget(e.Range))
            {
                e.Cast();
            }

            if (castR && ConfigMenu.config.Item(target.CharData.BaseSkinName) != null && ConfigMenu.config.Item(target.CharData.BaseSkinName).GetValue<bool>() == false)
            {
                r.CastOnUnit(target);
            }
        }

        private void ExecuteHarass()
        {
            var target = TargetSelector.GetTarget(e.Range, TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid)
                return;

            var castW = ConfigMenu.config.Item("useWh").GetValue<bool>() && w.IsReady();
            var castE = ConfigMenu.config.Item("useEh").GetValue<bool>() && e.IsReady();

            var eMana = ConfigMenu.config.Item("useEhMana").GetValue<Slider>().Value;
            var wMana = ConfigMenu.config.Item("useWhMana").GetValue<Slider>().Value;

            if (castW && target.IsValidTarget(e.Range) && ObjectManager.Player.ManaPercent > wMana)
            {
                w.Cast();
            }

            if (castE && target.IsValidTarget(e.Range) && ObjectManager.Player.ManaPercent > eMana)
            {
                e.Cast();
            }
        }

        private void LaneClear()
        {
            var castE = ConfigMenu.config.Item("useElc").GetValue<bool>() && e.IsReady();
            var castW = ConfigMenu.config.Item("useWlc").GetValue<bool>() && w.IsReady();

            var eMinions = ConfigMenu.config.Item("useElcMinions").GetValue<Slider>().Value;
            var wMinions = ConfigMenu.config.Item("useWlcMinions").GetValue<Slider>().Value;

            var eMana = ConfigMenu.config.Item("useElcMana").GetValue<Slider>().Value;
            var wMana = ConfigMenu.config.Item("useWlcMana").GetValue<Slider>().Value;

            if (!Orbwalking.CanMove(40))
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, e.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (castE && minions.Count > eMinions && ObjectManager.Player.ManaPercent >= eMana)
            {
                e.Cast();
            }

            if (castW && minions.Count > wMinions && ObjectManager.Player.ManaPercent >= wMana)
            {
                w.Cast();
            }
        }

        private void JungleClear()
        {
            var castE = ConfigMenu.config.Item("useEj").GetValue<bool>() && e.IsReady();
            var castW = ConfigMenu.config.Item("useWj").GetValue<bool>() && w.IsReady();

            var eMana = ConfigMenu.config.Item("useEjMana").GetValue<Slider>().Value;
            var wMana = ConfigMenu.config.Item("useWjMana").GetValue<Slider>().Value;

            if (!Orbwalking.CanMove(40))
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, e.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (castE && minions.Count > 0 && ObjectManager.Player.ManaPercent >= eMana)
            {
                e.Cast();
            }

            if (castW && minions.Count > 0 && ObjectManager.Player.ManaPercent >= wMana)
            {
                w.Cast();
            }
        }

        private void KillSteal()
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

            if (ConfigMenu.config.Item("useEks").GetValue<bool>() && e.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(q.Range) && !enemy.HasBuffOfType(BuffType.Invulnerability)).Where(target => target.Health < ObjectManager.Player.GetSpellDamage(target, SpellSlot.E)))
                {
                    e.Cast(target);
                }
            }

            if (ConfigMenu.config.Item("useRks").GetValue<bool>() && r.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(r.Range) && !enemy.HasBuffOfType(BuffType.Invulnerability)).Where(target => target.Health < ObjectManager.Player.GetSpellDamage(target, SpellSlot.R)))
                {
                    r.CastOnUnit(target);
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
    }
}
