using System;
using System.Linq;
using Jhin___The_Virtuoso.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Jhin___The_Virtuoso.Modes
{
    static class Combo
    {
        /// <summary>
        /// W Minimum Range
        /// </summary>
        private static readonly int MinRange = Menus.Config.Item("w.combo.min.distance").GetValue<Slider>().Value;

        /// <summary>
        /// W Maximum Range
        /// </summary>
        private static readonly int MaxRange = Menus.Config.Item("w.combo.max.distance").GetValue<Slider>().Value;

        /// <summary>
        /// Basit spell execute
        /// </summary>
        /// <param name="spell">Spell</param>
        public static void Execute(this Spell spell)
        {
            foreach (var enemy in HeroManager.Enemies.Where(o=> o.IsValidTarget(spell.Range)))
            {
                spell.Cast(enemy);
            }
        }

        /// <summary>
        /// Q Logic
        /// </summary>
        public static void ExecuteQ()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.Q.Range) && !x.IsReloading()))
            {
                Spells.Q.CastOnUnit(enemy);
            }
        }

        /// <summary>
        /// W Logic
        /// </summary>
        public static void ExecuteW()
        {
            if (Menus.Config.Item("w.passive.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.W.Range) && 
                    (x.IsStunnable() || x.IsEnemyImmobile())))
                {
                    Spells.W.Cast(enemy);
                }
            }
            else
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValid && x.Distance(ObjectManager.Player) < MaxRange
                    && x.Distance(ObjectManager.Player) > MinRange && 
                    Spells.W.GetPrediction(x).Hitchance >= Menus.Config.HikiChance("w.hit.chance")
                    && !x.IsDead && !x.IsZombie))
                {
                    Spells.W.Cast(enemy);
                }
            }
        }

        /// <summary>
        /// E Logic
        /// </summary>
        public static void ExecuteE()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range) && x.IsEnemyImmobile()))
            { 
                var pred = Spells.E.GetPrediction(enemy);
                if (pred.Hitchance >= Menus.Config.HikiChance("e.hit.chance"))
                {
                    Spells.E.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        /// Execute all combo
        /// </summary>
        public static void ExecuteCombo()
        {
            if (Spells.Q.IsReady() && Menus.Config.Item("q.combo").GetValue<bool>())
            {
                ExecuteQ();
            }
            if (Spells.W.IsReady() && Menus.Config.Item("w.combo").GetValue<bool>())
            {
                ExecuteW();
            }
            if (Spells.E.IsReady() && Menus.Config.Item("e.combo").GetValue<bool>())
            {
                ExecuteE();
            }
        }

    }
}
