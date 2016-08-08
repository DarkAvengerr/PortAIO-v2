using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using S_Plus_Class_Kalista.Handlers;
using EloBuddy;

namespace S_Plus_Class_Kalista.Libaries
{
    class RendCheck : Core
    {

        public static void Load()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!Champion.E.LSIsReady()) return;
            //if (!ManaHandler.UseAutoE()) return;
            if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.EventDelay")) return;

            if(RendChecks(0))
                Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.EventDelay");
        }
        private static bool RendChecks(int index)
        {
            bool useRend;

            switch (index)
            {
                case 0:
                    useRend = RendEpicMonsters();
                    break;
                case 1:
                    useRend = RendEnemies();
                    break;
                case 2:
                    useRend = RendBuffs();
                    break;
                case 3:
                    useRend = RendEpicsMinions();
                    break;
                case 4:
                    useRend = RendHarass();
                    break;
                case 5:
                    useRend = RendMinions();
                    break;
                case 6:
                    useRend = RendSmallMonsters();
                    break;
                case 7:
                    useRend = RendBeforeDeath();
                    break;
                case 8:
                    useRend = RendOnLeave();
                    break;
                default:
                    useRend = true;
                    break;
            }
            return useRend || (RendChecks(++index));
        }

        public static void CheckNonKillables(AttackableUnit minion)
        {
                if (!SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendNonKillables").GetValue<bool>()) return;
                if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.NonKillableDelay")) return;
                if (!(minion.Health <= Damage.DamageCalc.CalculateRendDamage((Obj_AI_Base)minion)) || minion.Health > 60) return;
                if (!minion.LSIsValidTarget(Champion.E.Range))return;
                Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.NonKillableDelay");
                Champion.E.Cast();
        }

        private static bool RendEpicMonsters()
        {
            if (!SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendEpics").GetValue<bool>()) return false;
            if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

            if (!MinionManager.GetMinions(Player.ServerPosition,
                Champion.E.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth)
                .Where(mob => mob.Name.Contains("Baron") || mob.Name.Contains("Dragon")).Any(mob => Damage.DamageCalc.CalculateRendDamage(mob) > mob.Health))
                return false;


            Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
            Champion.E.Cast();
            return true;
        }

        public static bool RendEnemies()
        {
            if (!SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendEnemyChampions").GetValue<bool>()) return false;
            if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

            foreach (var target in HeroManager.Enemies)
            {
                if (!target.LSIsValidTarget(Champion.E.Range)) continue;
                if (Damage.DamageCalc.CheckNoDamageBuffs(target)) continue;
                if (Damage.DamageCalc.CalculateRendDamage(target) < target.Health) continue;
                if (target.IsDead) continue;

                Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
                Champion.E.Cast();
                return true;
            }
            return false;
        }

        private static bool RendBuffs()
        {

            if (!SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendBuffs").GetValue<bool>()) return false;
            if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

            if (
                !MinionManager.GetMinions(Player.ServerPosition, Champion.E.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth)
                    .Where(
                        monster =>
                            monster.CharData.BaseSkinName.Equals("SRU_Red") ||
                            monster.CharData.BaseSkinName.Equals("SRU_Blue"))
                    .Any(monster => Damage.DamageCalc.CalculateRendDamage(monster) > monster.Health))
                return false;


            Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
            Champion.E.Cast();
            return true;
        }

        private static bool RendEpicsMinions()
        {
            var found = false;
            if (!SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendEpicMinions").GetValue<bool>()) return false;
            if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

            foreach (var epic in MinionManager.GetMinions(Player.ServerPosition, Champion.E.Range).Where(epic => epic.IsValid))
            {
                if (epic.CharData.BaseSkinName.ToLower().Contains("siege"))
                {
                    if (Damage.DamageCalc.CalculateRendDamage(epic) < epic.Health) continue;
                    found = true;
                    break;
                }
                if (!epic.CharData.BaseSkinName.ToLower().Contains("super")) continue;
                if (Damage.DamageCalc.CalculateRendDamage(epic) < epic.Health) continue;
                found = true;
                break;
            }

            if (!found) return false;
            Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
            Champion.E.Cast();
            return true;
        }

        private static bool RendHarass()
        {
            if (!SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendMinions").GetValue<bool>()) return false;
            if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

            foreach (var target in HeroManager.Enemies)
            {
                if (!target.LSIsValidTarget(Champion.E.Range)) continue;
                if (Damage.DamageCalc.CheckNoDamageBuffs(target)) continue;
                var stacks = target.GetBuffCount("kalistaexpungemarker");
                if (stacks < SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendHarrassKill.Slider.Stacks").GetValue<Slider>().Value) continue;
                var minions = MinionManager.GetMinions(Player.ServerPosition, Champion.E.Range);
                var count = minions.Count(minion => minion.Health <= Damage.DamageCalc.CalculateRendDamage(minion) && minion.IsValid);
                if (SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendHarrassKill.Slider.Killed").GetValue<Slider>().Value > count) continue;

                Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
                Champion.E.Cast();
                return true;
            }
            return false;
        }

        private static bool RendMinions()
        {
            if (!SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendMinions").GetValue<bool>()) return false;
            if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;
            var minions = MinionManager.GetMinions(Player.ServerPosition, Champion.E.Range);
            var count = minions.Count(minion => minion.Health <= Damage.DamageCalc.CalculateRendDamage(minion) && minion.IsValid);

            if (SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendMinions.Slider.Killed").GetValue<Slider>().Value >
                count)
                return false;

            Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
            Champion.E.Cast();

            return true;
        }

        private static bool RendSmallMonsters()
        {
            if (!SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendSmallMonster").GetValue<bool>()) return false;
            if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

            if (
                !MinionManager.GetMinions(Player.ServerPosition, Champion.E.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth)
                    .Any(monster => Damage.DamageCalc.CalculateRendDamage(monster) > monster.Health))
                return false;


            Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
            Champion.E.Cast();
            return true;
        }
        private static bool RendBeforeDeath()
        {
            
                if (!SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendBeforeDeath").GetValue<bool>()) return false;
                if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

                var champs = 0;
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var target in HeroManager.Enemies)
            {
                if (!target.LSIsValidTarget(Champion.E.Range)) continue;
                if (!target.HasBuff("KalistaExpungeMarker")) continue;
                if (ObjectManager.Player.HealthPercent > SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendBeforeDeath.Slider.PercentHP").GetValue<Slider>().Value) continue;
                if (target.GetBuffCount("kalistaexpungemarker") < SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendBeforeDeath.Slider.Stacks").GetValue<Slider>().Value) continue;
                champs++;

                if (champs < SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendBeforeDeath.Slider.Enemies").GetValue<Slider>().Value) continue;

                Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
                Champion.E.Cast();

                return true;
            }
            return false;
        }

        private static bool RendOnLeave()
        {

            if (!SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendOnLeave").GetValue<bool>()) return false;
            if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;


            foreach (var target in HeroManager.Enemies)
            {
                if (!target.LSIsValidTarget(Champion.E.Range)) continue;
                if (Damage.DamageCalc.CheckNoDamageBuffs(target)) continue;
                if (target.IsDead) continue;
                if (target.LSDistance(Player) < Champion.E.Range - 50) continue;
                var stacks = target.GetBuffCount("kalistaexpungemarker");
                if (stacks <= SMenu.Item(RendHandler._MenuItemBase + "Boolean.RendOnLeave.Slider.Stacks").GetValue<Slider>().Value) continue;


                Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
                Champion.E.Cast();

                return true;
            }
            return false;
        }
    }
}
