#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Handlers/Buffs.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Activator.Base;
using Activator.Data;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Handlers
{
    public static class Buffs
    {
        public static void StartOnUpdate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += OnEnemyBuffUpdate;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffAdd;
        }

        private static void OnEnemyBuffUpdate(EventArgs args)
        {
            foreach (var enemy in Activator.Heroes)
            {
                if (!enemy.Player.IsEnemy)
                    continue;

                var aura = Auradata.CachedAuras.Find(au => enemy.Player.HasBuff(au.Name));
                if (aura == null)
                    continue;

                Gamedata data = null;

                if (aura.Champion == null && aura.Slot == SpellSlot.Unknown)
                    data = new Gamedata { SDataName = aura.Name };

                if (aura.Champion != null && aura.Slot != SpellSlot.Unknown)
                    data = Gamedata.CachedSpells.Where(x => x.Slot == aura.Slot).Find(x => x.HeroNameMatch(aura.Champion));

                if (aura.Reverse && aura.DoT)
                {
                    if (Utils.GameTimeTickCount - aura.TickLimiter >= aura.Interval * 1000)
                    {
                        foreach (var ally in Activator.Allies())
                        {
                            if (ally.Player.Distance(enemy.Player) <= aura.Radius + 35)
                            {
                                Projections.EmulateDamage(enemy.Player, ally, data, HitType.Buff, "aura.DoT");
                            }
                        }

                        aura.TickLimiter = Utils.GameTimeTickCount;
                    }
                }

                if (aura.Reverse && aura.Evade)
                {
                    if (Utils.GameTimeTickCount - aura.TickLimiter >= 100)
                    {
                        foreach (var ally in Activator.Allies())
                        {
                            if (ally.Player.Distance(enemy.Player) <= aura.Radius + 35)
                            {
                                Projections.EmulateDamage(enemy.Player, ally, data, HitType.Buff, "aura.Evade");
                            }
                        }

                        aura.TickLimiter = Utils.GameTimeTickCount;
                    }
                }
            }
        }

        static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            #region Buffs

            foreach (var ally in Activator.Allies())
            {
                if (sender.NetworkId == ally.Player.NetworkId)
                {
                    if (args.Buff.Name == "rengarralertsound")
                    {
                        Projections.EmulateDamage(sender, ally,
                            new Gamedata { SDataName = "Stealth"}, HitType.Stealth, "handlers.OnBuffGain");
                    }
                }
            }

            #endregion
        }

       static void Game_OnUpdate(EventArgs args)
       {
            foreach (var hero in Activator.Allies())
            {
                var aura = Auradata.CachedAuras.Find(au => hero.Player.HasBuff(au.Name));
                if (aura == null)
                    continue;

                if (aura.Reverse)
                    continue;
              
                if (aura.Cleanse)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(aura.CleanseTimer,
                        () =>
                        {
                            // double check after delay incase we no longer have the buff
                            if (hero.Player.HasBuff(aura.Name) && hero.Player.IsValidTarget(float.MaxValue, false))
                            {
                                hero.ForceQSS = true;
                                LeagueSharp.Common.Utility.DelayAction.Add(100, () => hero.ForceQSS = false);
                            }
                        });
                }

                var owner = hero.Player.GetBuff(aura.Name).Caster as AIHeroClient;
                if (owner == null || !owner.IsEnemy)
                {
                    continue;
                }

                Gamedata data = null;

                if (aura.Champion == null && aura.Slot == SpellSlot.Unknown)
                    data = new Gamedata { SDataName = aura.Name };

                if (aura.Champion != null && aura.Slot != SpellSlot.Unknown)
                    data = Gamedata.CachedSpells.Where(x => x.Slot == aura.Slot).Find(x => x.HeroNameMatch(aura.Champion));

                if (aura.Evade)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(aura.EvadeTimer,
                        () =>
                        {
                            // double check after delay incase we no longer have the buff
                            if (hero.Player.HasBuff(aura.Name))
                            {
                                if (Utils.GameTimeTickCount - aura.TickLimiter >= 250)
                                {
                                    // ReSharper disable once PossibleNullReferenceException
                                    Projections.EmulateDamage(owner, hero, data, HitType.Buff, "aura.Evade");
                                    aura.TickLimiter = Utils.GameTimeTickCount;
                                }
                            }
                        });
                }

                if (aura.DoT)
                {
                    if (Utils.GameTimeTickCount - aura.TickLimiter >= aura.Interval * 1000)
                    {
                        if (aura.Name == "velkozresearchstack" && !hero.Player.HasBuffOfType(BuffType.Slow))
                        {
                            continue;
                        }

                        // ReSharper disable once PossibleNullReferenceException
                        Projections.EmulateDamage(owner, hero, data, HitType.Buff, "aura.DoT");
                        aura.TickLimiter = Utils.GameTimeTickCount;
                    }
                }            
            }
        }

        internal static IEnumerable<BuffInstance> GetAuras(AIHeroClient player, string itemname)
        {
            if (player.HasBuffOfType(BuffType.Knockback) || player.HasBuffOfType(BuffType.Knockup))
                return Enumerable.Empty<BuffInstance>();

            return player.Buffs.Where(buff => 
                !Auradata.BuffList.Any(b => buff.Name.ToLower() == b.Name && b.QssIgnore) &&
                   (buff.Type == BuffType.Snare &&
                    Activator.Origin.Item(itemname + "csnare").GetValue<bool>() ||
                    buff.Type == BuffType.Silence &&
                    Activator.Origin.Item(itemname + "csilence").GetValue<bool>() ||
                    buff.Type == BuffType.Charm &&
                    Activator.Origin.Item(itemname + "ccharm").GetValue<bool>() ||
                    buff.Type == BuffType.Taunt &&
                    Activator.Origin.Item(itemname + "ctaunt").GetValue<bool>() ||
                    buff.Type == BuffType.Stun &&
                    Activator.Origin.Item(itemname + "cstun").GetValue<bool>() ||
                    buff.Type == BuffType.Flee &&
                    Activator.Origin.Item(itemname + "cflee").GetValue<bool>() ||
                    buff.Type == BuffType.Polymorph &&
                    Activator.Origin.Item(itemname + "cpolymorph").GetValue<bool>() ||
                    buff.Type == BuffType.Blind &&
                    Activator.Origin.Item(itemname + "cblind").GetValue<bool>() ||
                    buff.Type == BuffType.Suppression &&
                    Activator.Origin.Item(itemname + "csupp").GetValue<bool>() ||
                    buff.Type == BuffType.Poison &&
                    Activator.Origin.Item(itemname + "cpoison").GetValue<bool>() ||
                    buff.Type == BuffType.Slow &&
                    Activator.Origin.Item(itemname + "cslow").GetValue<bool>() || 
                    buff.Name.ToLower() == "summonerexhaust") &&
                    Activator.Origin.Item(itemname + "cexh").GetValue<bool>());
        }

        internal static int GetCustomDamage(this AIHeroClient source, string auraname, AIHeroClient target)
        {
            if (auraname == "sheen")
            {
                return
                    (int)
                        source.CalcDamage(target, Damage.DamageType.Physical,
                            1.0 * source.FlatPhysicalDamageMod + source.BaseAttackDamage);
            }

            if (auraname == "lichbane")
            {
                return
                    (int)
                        source.CalcDamage(target, Damage.DamageType.Magical,
                            (0.75 * source.FlatPhysicalDamageMod + source.BaseAttackDamage) +
                            (0.50 * source.FlatMagicDamageMod));
            }

            return 0;
        }
    }

}
