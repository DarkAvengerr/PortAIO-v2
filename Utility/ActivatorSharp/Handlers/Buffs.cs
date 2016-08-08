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

using EloBuddy; namespace Activator.Handlers
{
    public static class Buffs
    {
        public static void StartOnUpdate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffAdd;
        }

        static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            #region Buffs

            foreach (var ally in Activator.Allies())
            {
                if (sender.LSIsValidTarget(1000) && !sender.IsZombie && sender.NetworkId == ally.Player.NetworkId)
                {
                    if (args.Buff.Name == "rengarralertsound")
                    {
                        ally.HitTypes.Add(HitType.Stealth);
                        LeagueSharp.Common.Utility.DelayAction.Add(200, () => ally.HitTypes.Remove(HitType.Stealth));
                    }
                }
            }

            #endregion
        }

       static void Game_OnUpdate(EventArgs args)
       {
            foreach (var hero in Activator.Allies())
            {
                var aura = Auradata.CachedAuras.Find(au => hero.Player.LSHasBuff(au.Name));
                if (aura == null)
                {
                    if (hero.DotTicks > 0)
                    {
                        hero.IncomeDamage -= 1;
                        hero.DotTicks -= 1;
                    }

                    if (hero.IncomeDamage < 0)
                        hero.IncomeDamage = 0;

                    continue;
                }

                if (aura.Cleanse)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(aura.CleanseTimer,
                        () =>
                        {
                            // double check after delay incase we no longer have the buff
                            if (hero.Player.LSHasBuff(aura.Name) && hero.Player.LSIsValidTarget(float.MaxValue, false))
                            {
                                hero.ForceQSS = true;
                                LeagueSharp.Common.Utility.DelayAction.Add(100, () => hero.ForceQSS = false);
                            }
                        });
                }

                if (aura.Evade)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(aura.EvadeTimer,
                        () =>
                        {                           
                            // double check after delay incase we no longer have the buff
                            if (hero.Player.LSHasBuff(aura.Name) && hero.Player.LSIsValidTarget(float.MaxValue, false))
                            {
                                if (!hero.Player.IsZombie && !hero.Immunity)
                                {
                                    if (!hero.HitTypes.Contains(HitType.Ultimate))
                                    {
                                        LeagueSharp.Common.Utility.DelayAction.Add(500, () => hero.HitTypes.Remove(HitType.Ultimate));
                                        hero.HitTypes.Add(HitType.Ultimate);
                                    }

                                    if (Utils.GameTimeTickCount - aura.TickLimiter >= 100)
                                    {
                                        hero.DotTicks += 1;
                                        hero.IncomeDamage += 1; // todo: get actuall damage
                                        aura.TickLimiter = Utils.GameTimeTickCount;
                                    }
                                }
                            }
                        });
                }

                if (aura.DoT)
                {
                    if (Utils.GameTimeTickCount - aura.TickLimiter >= aura.Interval * 1000)
                    {
                        if (hero.Player.LSIsValidTarget(float.MaxValue, false))
                        {
                            if (!hero.Player.IsZombie && !hero.Immunity)
                            {
                                if (aura.Name == "velkozresearchstack" &&
                                    !hero.Player.HasBuffOfType(BuffType.Slow))
                                    continue;

                                hero.DotTicks += 1;
                                hero.IncomeDamage += 1; // todo: get actuall damage
                                aura.TickLimiter = Utils.GameTimeTickCount;
                            }
                        }
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
