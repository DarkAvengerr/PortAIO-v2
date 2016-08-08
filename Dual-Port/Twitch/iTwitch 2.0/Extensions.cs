using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iTwitch
{
    using System;
    using System.Linq;

    using LeagueSharp;

    using SharpDX;

    using Color = System.Drawing.Color;

    static class Extensions
    {
        #region Public Methods and Operators

        public static void DrawTextOnScreen(this Vector3 location, string message, Color colour)
        {
            var worldToScreen = Drawing.WorldToScreen(location);
            Drawing.DrawText(worldToScreen[0] - message.Length * 5, worldToScreen[1] - 200, colour, message);
        }

        public static bool HasUndyingBuff(this Obj_AI_Base target1)
        {
            var target = target1 as AIHeroClient;

            if (target == null) return false;

            // Tryndamere R
            if (target.ChampionName == "Tryndamere"
                && target.Buffs.Any(
                    b => b.Caster.NetworkId == target.NetworkId && b.IsValid && b.DisplayName == "Undying Rage"))
            {
                return true;
            }

            // Zilean R
            if (target.Buffs.Any(b => b.IsValid && b.DisplayName == "Chrono Shift"))
            {
                return true;
            }

            // Kayle R
            if (target.Buffs.Any(b => b.IsValid && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }

            if (target.Buffs.Any(b => b.IsValid && b.Name == "kindredrnodeathbuff"))
            {
                return true;
            }

            // TODO poppy
            return false;
        }

        public static float GetPoisonDamage(this Obj_AI_Base target)
        {
            if (target == null || !target.LSHasBuff("twitchdeadlyvenom") || target.IsInvulnerable
                || target.HasUndyingBuff() || target.LSHasBuff("KindredRNoDeathBuff")
                || target.HasBuffOfType(BuffType.SpellShield))
            {
                return 0;
            }

            double baseDamage = Twitch.Spells[SpellSlot.E].GetDamage(target);

            // Exhaust
            if (ObjectManager.Player.LSHasBuff("SummonerExhaust"))
            {
                baseDamage *= 0.6;
            }

            // Urgot P
            if (ObjectManager.Player.LSHasBuff("urgotentropypassive"))
            {
                baseDamage *= 0.85;
            }

            // Bond Of Stone
            var bondofstoneBuffCount = target.GetBuffCount("MasteryWardenOfTheDawn");
            if (bondofstoneBuffCount > 0)
            {
                baseDamage *= 1 - (0.06 * bondofstoneBuffCount);
            }

            // Phantom Dancer
            var phantomdancerBuff = ObjectManager.Player.GetBuff("itemphantomdancerdebuff");
            if (phantomdancerBuff != null && phantomdancerBuff.Caster == target)
            {
                baseDamage *= 0.88;
            }

            /*
            // Bond Of Stone
            if (targetHero.GetResolve(Resolve.BondofStone).LSIsValid())
            {
                baseDamage *= 0.96;
            }
            */

            // Alistar R
            if (target.LSHasBuff("FerociousHowl"))
            {
                baseDamage *= 0.6 - new[] { 0.1, 0.2, 0.3 }[target.Spellbook.GetSpell(SpellSlot.R).Level - 1];
            }

            // Amumu E
            if (target.LSHasBuff("Tantrum"))
            {
                baseDamage -= new[] { 2, 4, 6, 8, 10 }[target.Spellbook.GetSpell(SpellSlot.E).Level - 1];
            }

            // Braum E
            if (target.LSHasBuff("BraumShieldRaise"))
            {
                baseDamage *= 1
                              - new[] { 0.3, 0.325, 0.35, 0.375, 0.4 }[target.Spellbook.GetSpell(SpellSlot.E).Level - 1];
            }

            // Galio R
            if (target.LSHasBuff("GalioIdolOfDurand"))
            {
                baseDamage *= 0.5;
            }

            // Garen W
            if (target.LSHasBuff("GarenW"))
            {
                baseDamage *= 0.7;
            }

            // Gragas W
            if (target.LSHasBuff("GragasWSelf"))
            {
                baseDamage *= 1
                              - new[] { 0.1, 0.12, 0.14, 0.16, 0.18 }[target.Spellbook.GetSpell(SpellSlot.W).Level - 1];
            }

            /*
            // Kassadin P
            if (target.LSHasBuff("VoidStone") && damageType == DamageType.Magical)
            {
                baseDamage *= 0.85;
            }
            */

            // Katarina E
            if (target.LSHasBuff("KatarinaEReduction"))
            {
                baseDamage *= 0.85;
            }

            // Maokai R
            if (target.LSHasBuff("MaokaiDrainDefense"))
            {
                baseDamage *= 0.8;
            }

            // MasterYi W
            if (target.LSHasBuff("Meditate"))
            {
                baseDamage *= 1 - new[] { 0.5, 0.55, 0.6, 0.65, 0.7 }[target.Spellbook.GetSpell(SpellSlot.W).Level - 1];
            }

            // Urgot R
            if (target.LSHasBuff("urgotswapdef"))
            {
                baseDamage *= 1 - new[] { 0.3, 0.4, 0.5 }[target.Spellbook.GetSpell(SpellSlot.R).Level - 1];
            }

            // Yorick P
            if (target.LSHasBuff("YorickUnholySymbiosis"))
            {
                baseDamage *= 1
                              - (ObjectManager.Get<Obj_AI_Minion>()
                                     .Count(
                                         g =>
                                         g.Team == target.Team
                                         && (g.Name.Equals("Clyde") || g.Name.Equals("Inky") || g.Name.Equals("Blinky")
                                             || (g.LSHasBuff("yorickunholysymbiosis")
                                                 && g.GetBuff("yorickunholysymbiosis").Caster == target))) * 0.05);
            }

            if (!(target is Obj_AI_Minion))
                return (float)baseDamage;

            if (target.Name.Contains("Baron"))
            {
                baseDamage *= 0.5f;
            }

            return (float)baseDamage;
        }

        public static ColorBGRA ToSharpDxColor(this Color c)
        {
            return new ColorBGRA(c.R, c.G, c.B, c.A);
        }

        public static float GetPoisonStacks(this Obj_AI_Base target)
        {
            return target.GetBuffCount("TwitchDeadlyVenom");
        }

        public static float GetRealHealth(this Obj_AI_Base target)
        {
            return target.Health + 10; //+ (target.AttackShield > 0 ? target.AttackShield : 0);
        }

        public static float GetRemainingBuffTime(this Obj_AI_Base target, string buffName)
        {
            return
                target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => string.Equals(buff.Name, buffName, StringComparison.CurrentCultureIgnoreCase))
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault() - Game.Time;
        }

        public static bool IsPoisonKillable(this Obj_AI_Base target)
        {
            return GetPoisonDamage(target) >= GetRealHealth(target);
        }

        #endregion
    }
}