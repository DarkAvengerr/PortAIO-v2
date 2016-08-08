
using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using System.Linq;
using S_Plus_Class_Kalista.Handlers;
using EloBuddy;

namespace S_Plus_Class_Kalista.Libaries
{
    class SoulBoundDamage : Core
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<float, float> _incomingDamage = new Dictionary<float, float>();
        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<float, float> _instantDamage = new Dictionary<float, float>();

        public static void Load()
        {
            //Calculates all the damage
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnCast;
        }

        public static float IncomingDamage
        {
            get { return _incomingDamage.Sum(e => e.Value) + _instantDamage.Sum(e => e.Value); }
        }

        private static void OnUpdate(EventArgs args)
        {       
            if (!Champion.R.LSIsReady()) return;

            if (SoulBoundHero == null)
                  SoulBoundHero = HeroManager.Allies.Find(ally =>ally.Buffs.Any(user => user.Caster.IsMe && user.Name.Contains("kalistacoopstrikeally")));

            foreach (var entry in _incomingDamage.Where(entry => entry.Key < Game.Time))
            {
                _incomingDamage.Remove(entry.Key);
            }

            foreach (var entry in _instantDamage.Where(entry => entry.Key < Game.Time))
            {
                _instantDamage.Remove(entry.Key);
            }

        }


        private static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy) return;
            if (SoulBoundHero == null) return;
            // Calculate Damage
            if ((!(sender is AIHeroClient) || args.SData.LSIsAutoAttack()) && args.Target != null && args.Target.NetworkId == SoulBoundHero.NetworkId)
            {
                // Calculate arrival time and damage
                _incomingDamage.Add(SoulBoundHero.ServerPosition.LSDistance(sender.ServerPosition) / args.SData.MissileSpeed + Game.Time, (float)sender.LSGetAutoAttackDamage(SoulBoundHero));
            }
            // Sender is a hero
            else if (sender is AIHeroClient)
            {
                var attacker = (AIHeroClient)sender;
                var slot = attacker.LSGetSpellSlot(args.SData.Name);

                if (slot == SpellSlot.Unknown) return;

                if (slot == attacker.LSGetSpellSlot("SummonerDot") && args.Target != null && args.Target.NetworkId == SoulBoundHero.NetworkId)
                    _instantDamage.Add(Game.Time + 2, (float)attacker.GetSummonerSpellDamage(SoulBoundHero, LeagueSharp.Common.Damage.SummonerSpell.Ignite));
                    
                else if (slot.HasFlag(SpellSlot.Q | SpellSlot.W | SpellSlot.E | SpellSlot.R) &&
                         ((args.Target != null && args.Target.NetworkId == SoulBoundHero.NetworkId) ||
                          args.End.LSDistance(SoulBoundHero.ServerPosition) < Math.Pow(args.SData.LineWidth, 2)))
                    _instantDamage.Add(Game.Time + 2, (float)attacker.LSGetSpellDamage(SoulBoundHero, slot));
            }
        }

    }
}
