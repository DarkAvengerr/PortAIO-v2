using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CjShuJinx.Modes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    public class OnDamageEvent
    {
        public int Time;
        public float Damage;

        internal OnDamageEvent(int time, float damage)
        {
            Time = time;
            Damage = damage;
        }
    }

    internal static class ModeStealMobs
    {
        private const float InitialSpeed = 1700;
        private const float ChangerSpeedDistance = 1350;
        private const float FinalSpeed = 2200;

        public static readonly Dictionary<int, List<OnDamageEvent>> DamagesOnTime =
            new Dictionary<int, List<OnDamageEvent>>();

        public static void Initialize()
        {
            Game.OnUpdate += Game_OnTick;
            AttackableUnit.OnDamage += AttackableUnit_OnDamage;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var minion = sender as Obj_AI_Minion;
            if (minion != null && minion.IsEnemy && minion.MaxHealth >= 3500)
            {
                if (DamagesOnTime.ContainsKey(minion.NetworkId))
                {
                    DamagesOnTime.Remove(minion.NetworkId);
                }
            }
        }

        private static void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (sender.IsEnemy)
            {
                var minion = sender as Obj_AI_Minion;
                if (minion != null && minion.MaxHealth >= 3500)
                {
                    if (!DamagesOnTime.ContainsKey(minion.NetworkId))
                        DamagesOnTime[minion.NetworkId] = new List<OnDamageEvent>();
                    DamagesOnTime[minion.NetworkId].Add(new OnDamageEvent(Environment.TickCount, args.Damage));
                }
                var sourceMinion = sender as Obj_AI_Minion;
                if (sourceMinion != null && sourceMinion.MaxHealth >= 3500)
                {
                    if (!DamagesOnTime.ContainsKey(sourceMinion.NetworkId))
                    {
                        DamagesOnTime[sourceMinion.NetworkId] = new List<OnDamageEvent>();
                    }

                    DamagesOnTime[sourceMinion.NetworkId].Add(new OnDamageEvent(Environment.TickCount, 0));
                }
            }
        }

        private static double GetUltimateDamage(this Obj_AI_Base monster, float health)
        {
            var percentMod = Math.Min((int) (ObjectManager.Player.Distance(monster)/100f)*6f + 10f, 100f)/100f;
            var level = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level;
            double rawDamage = 0.8f*percentMod*
                               (200f + 50f*level + ObjectManager.Player.TotalAttackDamage +
                                Math.Min((0.25f + 0.05f*level)*(monster.MaxHealth - health), 300f));
            return ObjectManager.Player.CalcDamage(monster, Damage.DamageType.Physical, rawDamage);
        }

        private static float GetPredictedDamage(this Obj_AI_Base monster, int time)
        {
            if (!DamagesOnTime.ContainsKey(monster.NetworkId))
            {
                return 0f;
            }
            return
                DamagesOnTime[monster.NetworkId].Where(
                    onDamage => onDamage.Time > Environment.TickCount - time && onDamage.Time <= Environment.TickCount)
                    .Sum(onDamage => onDamage.Damage);
        }

        private static float GetPredictedHealth(this Obj_AI_Base monster, int time)
        {
            // Return monster.TotalShieldHealth() + monster.HPRegenRate * 2 - monster.GetPredictedDamage(time);

            return monster.AllShield + monster.HPRegenRate*2 - monster.GetPredictedDamage(time);
        }

        private static bool IsGettingAttacked(this Obj_AI_Minion monster)
        {
            return DamagesOnTime.ContainsKey(monster.NetworkId) &&
                   DamagesOnTime[monster.NetworkId].LastOrDefault(o => Environment.TickCount - o.Time < 12000) != null;
        }

        public static IEnumerable<Obj_AI_Base> BigMonsters
        {
            get
            {
                // Return EntityManager.MinionsAndMonsters.Monsters.Where(m => m.MaxHealth >= 3500 && m.IsGettingAttacked());
                return MinionManager.GetMinions(40000).Where(m => m.MaxHealth >= 3500 && m.IsHPBarRendered);
            }
        }

        internal static float GetUltimateTravelTime(this Obj_AI_Base target)
        {
            var distance = Vector3.Distance(ObjectManager.Player.ServerPosition, target.ServerPosition);
            if (distance >= ChangerSpeedDistance)
            {
                return ChangerSpeedDistance/InitialSpeed + (distance - ChangerSpeedDistance)/FinalSpeed +
                       Champion.PlayerSpells.R.Delay/1000f;
            }
            return distance/InitialSpeed + Champion.PlayerSpells.R.Delay/1000f;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (SpellSlot.R.IsReady() && ObjectManager.Player.Mana >= Champion.PlayerSpells.R.ManaCost)
            {
                foreach (var monster in BigMonsters)
                {
                    var time = (int) (1000*monster.GetUltimateTravelTime());
                    var health = monster.GetPredictedHealth(time);
                    var damage = monster.GetUltimateDamage(health);
                    var heroNear = HeroManager.Enemies.Find(h => h.Distance(monster) >= 225f + monster.BoundingRadius);

                    // DrawManager.JungleStealText.TextValue = monster.Name + " is getting attacked. Damage left: " + (int)(100 * Math.Max(health - damage, 0) / monster.MaxHealth) + "%.";
                    if (health <= damage && heroNear != null)
                    {
                        Champion.PlayerSpells.R.Cast(heroNear);
                    }
                }
            }
        }
    }
}