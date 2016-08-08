using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Kalista
{
    class Helper
    {
        public static bool AsunasUndyBuff(AIHeroClient target)
        {
            if (target.ChampionName == "Tryndamere" &&
                target.Buffs.Any(b => b.Caster.NetworkId == target.NetworkId && b.LSIsValidBuff() && b.DisplayName == "Undying Rage"))
            {
                return true;
            }
            if (target.Buffs.Any(b => b.LSIsValidBuff() && b.DisplayName == "Chrono Shift"))
            {
                return true;
            }
            if (target.Buffs.Any(b => b.LSIsValidBuff() && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }
            if (target.ChampionName == "Poppy")
            {
                if (HeroManager.Allies.Any(o =>
                    !o.IsMe &&
                    o.Buffs.Any(b => b.Caster.NetworkId == target.NetworkId && b.LSIsValidBuff() && b.DisplayName == "PoppyDITarget")))
                {
                    return true;
                }
            }
            return false;
        }
        public static void PierceCombo(int collisionObject, HitChance hChance)
        {
            foreach (var enemy in HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(Program.Q.Range) && hero.IsVisible && !hero.IsDead && !hero.IsZombie))
            {
                if (Program.Q.GetPrediction(enemy).Hitchance >= hChance && Program.Q.GetPrediction(enemy).CollisionObjects.Count == 0)
                {
                    Program.Q.Cast(enemy);
                }
            }
        }
        public static void PierceJungleClear(Spell spell, HitChance hChance)
        {
            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob == null || mob.Count == 0)
            {
                return;
            }
            if (Program.Q.GetPrediction(mob[0]).Hitchance >= hChance && Program.Q.GetPrediction(mob[0]).CollisionObjects.Count == 0)
            {
                Program.Q.Cast(mob[0]);
            }

        }
        public static void RendCombo()
        {
            foreach (var enemy in HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(Program.E.Range) &&
                    !AsunasUndyBuff(hero) && !hero.HasBuffOfType(BuffType.SpellShield)))
            {
                if (enemy.Health < Calculators.ChampionTotalDamage(enemy))
                {
                    Program.E.Cast();
                }
            }
        }
        public static void RendHarass(int spearcount)
        {
            foreach (var enemy in HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(Program.E.Range)))
            {
                int enemyStack = enemy.Buffs.FirstOrDefault(x => x.DisplayName == "KalistaExpungeMarker").Count;
                if (enemyStack > 0 && enemyStack > spearcount)
                {
                    Program.E.Cast();
                }
            }  
        }
        public static void RendClear(int minionCount)
        {
            var mns = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Program.E.Range);
            var mkc = mns.Count(x => Program.E.CanCast(x) && x.Health <= Program.E.GetDamage(x));
            if (mkc >= minionCount)
            {
                Program.E.Cast();
            }
        }
        public static void RendSiegeMinions()
        {
            foreach (var lasthit in MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Program.E.Range, MinionTypes.All, MinionTeam.Enemy).Where(x => x.CharData.BaseSkinName == "MinionSiege"))
            {
                if (Calculators.MinionCalculator(lasthit) > lasthit.Health)
                {
                    Program.E.Cast();
                }
            }
        }
        public static void RendJungleClear()
        {
            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob == null || (mob != null && mob.Count == 0))
            {
                return;
            }
            if (mob[0].Health + (mob[0].HPRegenRate / 2) <= Program.E.GetDamage(mob[0]))
            {
                Program.E.Cast();
            }
        }
        public static void FatesCall(int healthpercent)
        {
            foreach (var support in ObjectManager.Get<AIHeroClient>().Where(x=> x.IsAlly && !x.IsMe && x.LSDistance(ObjectManager.Player.Position) < Program.R.Range &&
                x.HasBuff("kalistacoopstrikeally")))
            {
                if (support.Health < healthpercent)
                {
                    Program.R.Cast();
                }
            }
        }
        public static void SentinelCombo() // will be come soon
        {
            
        }
        public static void BlueOrb(int level)
        {
            if (ObjectManager.Player.Level >= level && ObjectManager.Player.LSInShop() && !(Items.HasItem(3342) || Items.HasItem(3363)))
            {
                Shop.BuyItem(ItemId.Farsight_Alteration);
            }
        }
        public static void KillStealWithPierce(HitChance hChance)
        {
            foreach (var target in HeroManager.Enemies.OrderByDescending(x => x.Health))
            {
                if (target.LSDistance(ObjectManager.Player.Position) < Program.Q.Range && Program.Q.GetPrediction(target).Hitchance >= hChance
                    && Program.Q.GetDamage(target) > target.Health)
                {
                    Program.Q.Cast(target);
                }
            }
        }
        public static void KillStealWithRend()
        {
            foreach (var target in HeroManager.Enemies.OrderByDescending(x => x.Health))
            {
                if (target.Health < Calculators.ChampionTotalDamage(target))
                {
                    Program.E.Cast();
                }
            }
        }
        public static void Bitterlogic(int hppercent)
        {
            foreach (var enemy in HeroManager.Enemies.Where(o => o.LSIsValidTarget(Program.E.Range) && !o.IsDead && !o.IsZombie))
            {
                float spearDamage = Calculators.ChampionTotalDamage(enemy);
                float killableSpearCount = enemy.Health / spearDamage;
                int totalSpear = (int)Math.Ceiling(killableSpearCount);
                if (ObjectManager.Player.Health < hppercent && Calculators.KillableSpearCount(enemy) - 1 < totalSpear)
                {
                    Program.E.Cast();
                }
            }
        }
        public static bool ImmobileDetector(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned)
            {
                return true;
            }
            else
                return false;
        }
        public static void ImmobilePierce(HitChance hChance, int collisionObject)
        {
            foreach (var enemy in HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(Program.Q.Range)))
            {
                if (ImmobileDetector(enemy) && Program.Q.GetPrediction(enemy).Hitchance >= hChance && Program.Q.GetPrediction(enemy).CollisionObjects.Count == collisionObject)
                {
                    Program.Q.Cast(enemy);
                }
            }
        }
        public static void Balista(int minrange,int maxrange, Spell spell)
        {
            if (!spell.LSIsReady())
            {
                return;
            }
            var blitz = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsAlly && x.CharData.BaseSkinName == "Blitzcrank" && x.HasBuff("kalistacoopstrikeally"));
            if (blitz != null && spell.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsEnemy && o.LSIsValidTarget(2450f)))
                {
                    if (blitz.LSDistance(enemy.Position) <= 950f &&
                        blitz.LSDistance(ObjectManager.Player.Position) >= minrange &&
                        blitz.LSDistance(ObjectManager.Player.Position) <= maxrange)
                    {
                        if (enemy.Buffs != null && enemy.LSHasBuff("rocketgrab2"))
                        {
                            if (spell.LSIsReady())
                            {
                                spell.Cast();
                            }
                        }
                    }
                }
            }
        }
        public static void SKalista(int minrange, int maxrange, Spell spell)
        {
            if (!spell.LSIsReady())
            {
                return;
            }
            var skarner = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(o => o.IsAlly && o.CharData.BaseSkinName == "Skarner" &&
                o.LSHasBuff("kalistacoopstrikeally"));
            if (skarner != null && spell.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsEnemy 
                    && o.LSIsValidTarget(1849))) // Kalista R Range + Skarner R Range - 1
                {
                    if (skarner.LSDistance(enemy.Position) <= 350 &&
                        skarner.LSDistance(ObjectManager.Player.Position) >= minrange &&
                        skarner.LSDistance(ObjectManager.Player.Position) <= maxrange)
                    {
                        if (enemy.Buffs != null && enemy.LSHasBuff("SkarnerImpale"))
                        {
                            if (spell.LSIsReady())
                            {
                                spell.Cast();
                            }
                        }
                    }
                }
            }
        }
        public static void SupportProtector(Spell spell)
        {
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe && x.HasBuff("kalistacoopstrikeally") && x.LSDistance(ObjectManager.Player.Position) < Program.R.Range &&
                x.HealthPercent <= Program.Config.Item("savePercent").GetValue<Slider>().Value))
            {
                spell.Cast();
            }
        }

    }
}
