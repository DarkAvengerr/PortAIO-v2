using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SorakaSharp.Source.Handler;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SorakaSharp.Source.Manager.Heal.Ultimate
{
    internal static class CUltimate
    {

        //Source: https://github.com/Hellsing/LeagueSharp/blob/master/Kalista/SoulBoundSaver.cs
        //Credits: Hellsing

        private static Spell R
        {
            get { return CSpell.R; }
        }

        private static readonly Dictionary<float, float> _incomingDamage = new Dictionary<float, float>();
        private static readonly Dictionary<float, float> _instantDamage = new Dictionary<float, float>();
        private static float IncomingDamage
        {
            get { return _incomingDamage.Sum(e => e.Value) + _instantDamage.Sum(e => e.Value); }
        }

        private static bool IsBlocked(this AIHeroClient unit)
        {
            return CConfig.ConfigMenu.SubMenu("Ultimate").SubMenu("DontUlt").Items.Any(entry => entry.DisplayName == unit.BaseSkinName && entry.IsActive());
        }

        internal static void Initialize()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!CConfig.ConfigMenu.Item("useUltimate").GetValue<bool>() || !R.IsReady() || !sender.IsEnemy)
                return;

            // Calculations to smart save allies
            foreach (var ally in HeroManager.Allies)
            {
                //Auto Attack
                if ((!(sender is AIHeroClient) || args.SData.IsAutoAttack()) && args.Target != null &&
                    args.Target.NetworkId == ally.NetworkId && !ally.IsBlocked())
                {
                    // Calculate arrival time and damage
                    _incomingDamage.Add(
                        ally.ServerPosition.Distance(sender.ServerPosition) / args.SData.MissileSpeed + Game.Time,
                        (float)sender.GetAutoAttackDamage(ally));
                }

                // Sender is a hero
                else if (sender is AIHeroClient)
                {
                    var attacker = (AIHeroClient)sender;
                    var slot = attacker.GetSpellSlot(args.SData.Name);

                    if (slot != SpellSlot.Unknown)
                    {
                        if (slot == attacker.GetSpellSlot("SummonerDot") && args.Target != null &&
                            args.Target.NetworkId == ally.NetworkId && !ally.IsBlocked())
                        {
                            // Ingite damage (dangerous)
                            _instantDamage.Add(Game.Time + 2,
                                (float)attacker.GetSummonerSpellDamage(ally, Damage.SummonerSpell.Ignite));
                        }
                        else if (slot.HasFlag(SpellSlot.Q | SpellSlot.W | SpellSlot.E | SpellSlot.R) &&
                                 ((args.Target != null && args.Target.NetworkId == ally.NetworkId) ||
                                  args.End.Distance(ally.ServerPosition) < Math.Pow(args.SData.LineWidth, 2)))
                        {
                            // Instant damage to target
                            _instantDamage.Add(Game.Time + 2, (float)attacker.GetSpellDamage(ally, slot));
                        }
                    }
                }
            }
        }

        internal static void SmartSave()
        {
            if (!CConfig.ConfigMenu.Item("useUltimate").GetValue<bool>() || !R.IsReady())
                return;

            //Cast Ultimate
            foreach (var ally in HeroManager.Allies)
            {
                if (ally.HealthPercentage() < 5 && ally.CountEnemiesInRange(500) <= 2 || IncomingDamage > ally.Health)
                {
                    R.Cast();
                }
            }

            // Check spell arrival
            foreach (var entry in _incomingDamage.Where(entry => entry.Key < Game.Time))
            {
                _incomingDamage.Remove(entry.Key);
            }

            // Instant damage removal
            foreach (var entry in _instantDamage.Where(entry => entry.Key < Game.Time))
            {
                _instantDamage.Remove(entry.Key);
            }
        }

        //Teamfight Ultimate
        private static double GetTeamHp
        {
            get
            {
                return HeroManager.Allies.Where(allies => allies.IsValidTarget(float.MaxValue, false)).Select(allies => allies.HealthPercentage()).FirstOrDefault();
            }
        }

        internal static void TeamfightUltimate()
        {
            if (!CConfig.ConfigMenu.Item("useTeamfightUltimate").GetValue<bool>() || !R.IsReady())
                return;

            if (GetTeamHp < CConfig.ConfigMenu.Item("percentage2").GetValue<Slider>().Value)
            {
                R.Cast();
            }
        }
    }
}
