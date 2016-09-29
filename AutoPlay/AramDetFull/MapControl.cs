using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using DetuksSharp;
using LeagueSharp.Common;
using SharpDX;
using Geometry = LeagueSharp.Common.Geometry;
using SkillshotType = LeagueSharp.Common.SkillshotType;
using Spell = LeagueSharp.Common.Spell;
using ARAMDetFull.SpellsSDK;
using DetuksSharp;
using DetuksSharp.Prediction;
using LeagueSharp.Data.DataTypes;
using LeagueSharp.Data.Enumerations;
using SpellDatabase = LeagueSharp.SDK.SpellDatabase;

using EloBuddy;
namespace ARAMDetFull
{
    class MapControl
    {

        public static SpellSlot[] spellSlots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };

        internal class ChampControl
        {
            public AIHeroClient hero = null;

            public float reach = 0;

            public float dangerReach = 0;

            public int activeDangers = 0;

            protected List<SpellDatabaseEntry> champSpells = new List<SpellDatabaseEntry>();

            public AttackableUnit getFocusTarget()
            {
                HealthDeath.DamageMaker dm = null;
                HealthDeath.activeDamageMakers.TryGetValue(hero.NetworkId, out dm);
                return dm?.target;
            }

            public ChampControl(AIHeroClient champ)
            {
                hero = champ;
                foreach (var spell in
                SpellDatabase.Spells.Where(
                    s =>
                        s.ChampionName.Equals(champ.ChampionName)))
                {
                    champSpells.Add(spell);
                }

                getReach();
            }

            public float getReach()
            {
                dangerReach = 0;
                reach = hero.AttackRange;
                activeDangers = 0;
                var takeInCOunt = new List<SpellTags> { SpellTags.Dash, SpellTags.Blink, SpellTags.Teleport, SpellTags.Damage, SpellTags.CrowdControl };
                foreach (var cSpell in champSpells)
                {
                    if (cSpell.SpellTags == null || !(cSpell.SpellTags.Any(takeInCOunt.Contains)))
                        continue;
                    var spell = hero.Spellbook.GetSpell(cSpell.Slot);
                    if ((spell.CooldownExpires - Game.Time) > 3.5f || spell.State == SpellState.NotLearned)
                        continue;
                    var range = (spell.SData.CastRange < 1000) ? spell.SData.CastRange : 1000;
                    if (spell.SData.CastRange > range)
                        reach = range;
                }
                var extra = (hero.IsEnemy) ? (100 - ObjectManager.Player.HealthPercent) * 1.5f : 0;
                reach += 150 + extra;
                return reach;
            }

            public int getccCount()
            {
                return champSpells.Count(sShot => sShot.IsDangerous);
            }
        }

        internal class MyControl : ChampControl
        {

            private Dictionary<SpellDatabaseEntry, Spell> spells = new Dictionary<SpellDatabaseEntry, Spell>();

            public static Spellbook sBook = ObjectManager.Player.Spellbook;
            public static SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
            public static SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
            public static SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
            public static SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);

            public MyControl(AIHeroClient champ) : base(champ)
            {
                try
                {
                    hero = champ;
                    foreach (var spell in champSpells)
                    {
                        var spl = new Spell(spell.Slot, spell.Range);
                        if (spell.CastType.IsSkillShot())
                        {
                            bool coll = spell.CollisionObjects.Length > 1;
                            spl.SetSkillshot(spell.Delay, spell.Radius, spell.MissileSpeed, coll, spell.SpellType.GetSkillshotType());
                        }
                        spells.Add(spell, spl);
                    }
                    getReach();
                    ARAMSimulator.farmRange = reach;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            public int bonusSpellBalance()
            {
                float manaUsed = 0;
                int bal = 0;
                foreach (var spell in spells)
                {
                    if (!spell.Value.IsReady())
                        continue;
                    manaUsed += spell.Value.ManaCost;
                    if (hero.MaxMana < 300 || hero.Mana - manaUsed >= 0)
                    {
                        bal += (spell.Value.Slot == SpellSlot.R) ? 15 : 7;
                    }
                }
                return bal;
            }

            private int lastMinionSpellUse = DeathWalker.now;
            public void useSpellsOnMinions()
            {
                try
                {
                    if (lastMinionSpellUse + 277 > DeathWalker.now)
                        return;
                    lastMinionSpellUse = DeathWalker.now;
                    if (hero.MaxMana > 300 && hero.ManaPercent < 78)
                        return;
                    foreach (var spell in spells)
                    {
                        if (spell.Value.Slot == SpellSlot.R || spell.Value.Instance.Cooldown > 10 || !spell.Value.IsReady() || spell.Value.ManaCost > hero.Mana || spell.Key.SpellTags == null || !spell.Key.SpellTags.Contains(SpellTags.Damage))
                            continue;
                        var minions = MinionManager.GetMinions((spell.Value.Range != 0) ? spell.Value.Range : 500);
                        foreach (var minion in minions)
                        {
                            if (minion.Health > spell.Value.GetDamage(minion))
                                continue;
                            var movementSpells = new List<SpellTags> { SpellTags.Dash, SpellTags.Blink, SpellTags.Teleport };
                            if (spell.Value.IsSkillshot)
                            {
                                if (!(spell.Key.SpellTags != null && spell.Key.SpellTags.Any(movementSpells.Contains)) || safeGap(minion))
                                {
                                    Console.WriteLine("Cast farm location: " + spell.Key.Slot);
                                    spell.Value.Cast(minion.Position);
                                    return;
                                }
                            }
                            else
                            {
                                float range = (spell.Value.Range != 0) ? spell.Value.Range : 500;
                                if (spell.Key.CastType.Contains(CastType.Self))
                                {
                                    var bTarg = ARAMTargetSelector.getBestTarget(range, true);
                                    if (bTarg != null)
                                    {
                                        Console.WriteLine("Cast farm self: " + spell.Key.Slot);
                                        spell.Value.Cast();
                                        return;
                                    }
                                }
                                else if (spell.Key.CastType.Contains(CastType.EnemyMinions))
                                {
                                    if (minion != null)
                                    {
                                        if (!(spell.Key.SpellTags != null && spell.Key.SpellTags.Any(movementSpells.Contains)) || safeGap(minion))
                                        {
                                            Console.WriteLine("Cast farm target: " + spell.Key.Slot);
                                            spell.Value.CastOnUnit(minion);
                                            return;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            private int lastSpellUse = DeathWalker.now;
            public void useSpells()
            {
                try
                {
                    if (lastSpellUse + 277 > DeathWalker.now)
                        return;
                    lastSpellUse = DeathWalker.now;
                    foreach (var spell in spells)
                    {
                        if (!spell.Value.IsReady() || spell.Value.ManaCost > hero.Mana)
                            continue;
                        var movementSpells = new List<SpellTags> { SpellTags.Dash, SpellTags.Blink, SpellTags.Teleport };
                        var supportSpells = new List<SpellTags> { SpellTags.Shield, SpellTags.Heal, SpellTags.DamageAmplifier,
                        SpellTags.SpellShield, SpellTags.RemoveCrowdControl, };
                        if (spell.Value.IsSkillshot)
                        {
                            if (spell.Key.SpellTags != null && spell.Key.SpellTags.Any(movementSpells.Contains))
                            {
                                if (hero.HealthPercent < 25 && hero.CountEnemiesInRange(600) > 0)
                                {
                                    Console.WriteLine("Cast esacpe location: " + spell.Key.Slot);
                                    spell.Value.Cast(hero.Position.Extend(ARAMSimulator.fromNex.Position, 1235));
                                    return;
                                }
                                else
                                {
                                    var bTarg = ARAMTargetSelector.getBestTarget(spell.Value.Range, true);
                                    if (bTarg != null && safeGap(hero.Position.Extend(bTarg.Position, spell.Key.Range).To2D()))
                                    {
                                        if (spell.Value.CastIfHitchanceEquals(bTarg, HitChance.VeryHigh))
                                        {
                                            Console.WriteLine("Cast attack location gap: " + spell.Key.Slot);
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var bTarg = ARAMTargetSelector.getBestTarget(spell.Value.Range, true);
                                if (bTarg != null)
                                {
                                    if (spell.Value.CastIfHitchanceEquals(bTarg, HitChance.VeryHigh))
                                    {
                                        Console.WriteLine("Cast attack location gap: " + spell.Key.Slot);
                                        return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            float range = (spell.Value.Range != 0) ? spell.Value.Range : 500;
                            if (spell.Key.CastType.Contains(CastType.Self) || spell.Key.CastType.Contains(CastType.Activate))
                            {
                                var bTarg = ARAMTargetSelector.getBestTarget(range, true);
                                if (bTarg != null)
                                {
                                    Console.WriteLine("Cast self: " + spell.Key.Slot);
                                    spell.Value.Cast();
                                    return;
                                }
                            }
                            else if (spell.Key.CastType.Contains(CastType.AllyChampions) && spell.Key.SpellTags != null && spell.Key.SpellTags.Any(supportSpells.Contains))
                            {
                                var bTarg = ARAMTargetSelector.getBestTargetAly(range, false);
                                if (bTarg != null)
                                {
                                    Console.WriteLine("Cast ally: " + spell.Key.Slot);
                                    spell.Value.CastOnUnit(bTarg);
                                    return;
                                }
                            }
                            else if (spell.Key.CastType.Contains(CastType.EnemyChampions))
                            {
                                var bTarg = ARAMTargetSelector.getBestTarget(range, true);
                                if (bTarg != null)
                                {
                                    if (!(spell.Key.SpellTags != null && spell.Key.SpellTags.Any(movementSpells.Contains)) || safeGap(bTarg))
                                    {
                                        Console.WriteLine("Cast enemy: " + spell.Key.Slot);
                                        spell.Value.CastOnUnit(bTarg);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public float canDoDmgTo(Obj_AI_Base target, bool ignoreRange = false)
            {
                float dmgreal = 0;
                float mana = 0;
                foreach (var spell in spells.Values)
                {
                    try
                    {

                        if (spell == null || !spell.IsReady())
                            continue;

                        float dmg = 0;
                        var checkRange = spell.Range + 250;
                        if (ignoreRange || hero.Distance(target, true) < checkRange * checkRange)
                            dmg = spell.GetDamage(target);
                        if (dmg != 0)
                            mana += hero.Spellbook.GetSpell(spell.Slot).SData.Mana;
                        if (hero.Mana > mana)
                            dmgreal += dmg;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                return dmgreal;
            }
        }

        public static int fearDistance
        {
            get
            {
                try
                {
                    int assistValue = (ARAMSimulator.getType() == ARAMSimulator.ChampType.Support
                                       || ARAMSimulator.getType() == ARAMSimulator.ChampType.Tank ||
                                       ARAMSimulator.getType() == ARAMSimulator.ChampType.TankAS)
                        ? 30
                        : 20;
                    int kdaScore = myControler.hero.ChampionsKilled * 50 + myControler.hero.Assists * assistValue - myControler.hero.Deaths * 50;
                    int timeFear = 0;
                    int healthFear = (int)(-(60 - myControler.hero.HealthPercent) * 2);
                    int score = kdaScore + timeFear + 100;
                    return (score < -550) ? -550 + healthFear : ((score > 500) ? 500 : score) + healthFear;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return 0;
                }
            }
        }

        public static List<ChampControl> enemy_champions = new List<ChampControl>();

        public static List<ChampControl> ally_champions = new List<ChampControl>();

        public static MyControl myControler;
        public static void updateReaches()
        {
            foreach (var champ in enemy_champions)
            {
                champ.getReach();
            }
            foreach (var champ in ally_champions)
            {
                champ.getReach();
            }
        }

        public static void setupMapControl()
        {
            try
            {
                ally_champions.Clear();
                enemy_champions.Clear();
                foreach (var hero in ObjectManager.Get<AIHeroClient>())
                {
                    if (hero.IsMe)
                        continue;

                    if (hero.IsAlly)
                        ally_champions.Add(new ChampControl(hero));

                    if (hero.IsEnemy)
                        enemy_champions.Add(new ChampControl(hero));
                }
                myControler = new MyControl(ObjectManager.Player);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static int fightLevel()
        {
            int count = 0;
            foreach (var enem in enemy_champions.Where(ene => !ene.hero.IsDead && ene.hero.IsVisible).OrderBy(ene => ene.hero.Distance(ObjectManager.Player, true)))
            {
                if (myControler.canDoDmgTo(enem.hero) * 0.7f > enem.hero.Health)
                    count++;

                if (ally_champions.Where(ene => !ene.hero.IsDead && !ene.hero.IsMe).Any(ally => enem.hero.Distance(ally.hero, true) < 600 * 600))
                {
                    count++;
                }
            }
            return count;
        }

        public static AIHeroClient fightIsOn()
        {
            foreach (var enem in enemy_champions.Where(ene => !ene.hero.IsDead && ene.hero.IsVisible && !ene.hero.IsZombie).OrderBy(ene => ene.hero.Distance(ObjectManager.Player, true)))
            {
                if (myControler.canDoDmgTo(enem.hero) * 0.7f > enem.hero.Health)
                    return enem.hero;

                if (ally_champions.Where(ene => !ene.hero.IsDead && !ene.hero.IsMe).Any(ally => enem.hero.Distance(ally.hero, true) < 600 * 600))
                {
                    return enem.hero;
                }
            }

            return null;
        }

        public static bool fightIsClose()
        {
            foreach (var enem in enemy_champions.Where(ene => !ene.hero.IsDead && ene.hero.IsVisible).OrderBy(ene => ene.hero.Distance(ObjectManager.Player, true)))
            {

                if (ally_champions.Where(ene => !ene.hero.IsDead && !ene.hero.IsMe).Any(ally => enem.hero.Distance(ally.hero, true) < 550 * 550))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool fightIsOn(Obj_AI_Base target)
        {
            if (target == null)
            {
                return false;
            }

            if (myControler.canDoDmgTo(target) * 0.75 > target.Health)
                return true;

            if (ally_champions.Where(ene => !ene.hero.IsDead && !ene.hero.IsMe).Any(ally => target.Distance(ally.hero, true) < 300 * 300))
            {
                return true;
            }

            return false;
        }

        public static int balanceAroundPoint(Vector2 point, float range)
        {
            int balance = 0;
            balance -= enemy_champions.Where(ene => !ene.hero.IsDead).Count(ene => ene.hero.Distance(point, true) < range * range);

            balance += ally_champions.Where(aly => !aly.hero.IsDead).Count(aly => aly.hero.Distance(point, true) < (range - 150) * (range - 150));
            return balance;
        }

        public static int balanceAroundPointAdvanced(Vector2 point, float rangePlus, int fearCompansate = 0)
        {
            int balance = (point.To3D().UnderTurret(true)) ? -80 : (point.To3D().UnderTurret(false)) ? 110 : 0;
            foreach (var ene in enemy_champions)
            {
                var eneBalance = 0;
                var reach = ene.reach + rangePlus;
                if (!ene.hero.IsDead && ene.hero.Distance(point, true) < reach * reach && !unitIsUseless(ene.hero) && !notVisibleAndMostLieklyNotThere(ene.hero))
                {
                    eneBalance -= (int)((ene.hero.HealthPercent + 20 - ene.hero.Deaths * 4 + ene.hero.ChampionsKilled * 4));
                    if (!ene.hero.IsFacing(ObjectManager.Player))
                        eneBalance = (int)(eneBalance * 0.64f);
                    var focus = ene.getFocusTarget();
                    if (focus != null && focus.IsValid && focus.IsAlly && focus is AIHeroClient)
                    {
                        eneBalance = (int)(eneBalance * (focus.IsMe ? 1.20 : 0.80));
                    }
                }
                balance += eneBalance;
            }


            foreach (var aly in ally_champions)
            {
                var reach = (aly.reach - 200 < 500) ? 500 : (aly.reach - 200);
                if (!aly.hero.IsDead && /*aly.hero.Distance(point, true) < reach * reach &&*/
                    (Geometry.Distance(aly.hero, ARAMSimulator.toNex.Position) + reach < (Geometry.Distance(point, ARAMSimulator.toNex.Position) + fearDistance + fearCompansate + (ARAMSimulator.tankBal * -5) + (ARAMSimulator.agrobalance * 3))))
                    balance += ((int)aly.hero.HealthPercent + 20 + 20 - aly.hero.Deaths * 4 + aly.hero.ChampionsKilled * 4);
            }
            var myBal = ((int)myControler.hero.HealthPercent + 20 + 20 - myControler.hero.Deaths * 4 +
                         myControler.hero.ChampionsKilled * 4) + myControler.hero.Assists / 2 + myControler.bonusSpellBalance();
            balance += (myBal < 0) ? 10 : myBal;
            return balance;
        }

        public static double unitIsUselessFor(Obj_AI_Base unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                        buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                         buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare || buff.Type == BuffType.Fear))
                    .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return (result - Game.Time);
        }

        public static bool unitIsUseless(Obj_AI_Base unit)
        {
            return unitIsUselessFor(unit) > 0.3;
        }

        public static bool notVisibleAndMostLieklyNotThere(Obj_AI_Base unit)
        {
            var distEneNex = Geometry.Distance(ARAMSimulator.toNex.Position, unit.Position);
            var distEneNexDeepest = Geometry.Distance(ARAMSimulator.toNex.Position, ARAMSimulator.deepestAlly.Position);

            return !ARAMSimulator.deepestAlly.IsDead && distEneNexDeepest + 1500 < distEneNex;
        }

        public static ChampControl getByObj(Obj_AI_Base champ)
        {
            return enemy_champions.FirstOrDefault(ene => ene.hero.NetworkId == champ.NetworkId);
        }

        public static bool safeGap(Obj_AI_Base target)
        {
            return safeGap(target.Position.To2D()) || MapControl.fightIsOn(target) || (!ARAMTargetSelector.IsInvulnerable(target) && target.Health < myControler.canDoDmgTo(target, true) / 2);
        }

        public static bool safeGap(Vector2 position)
        {
            return myControler.hero.HealthPercent < 13 || (!Sector.inTowerRange(position) &&
                   (MapControl.balanceAroundPointAdvanced(position, 500) > 0)) || position.Distance(ARAMSimulator.fromNex.Position, true) < myControler.hero.Position.Distance(ARAMSimulator.fromNex.Position, true);
        }

        public static List<int> usedRelics = new List<int>();

        public static Obj_AI_Base ClosestRelic()
        {
            try
            {
                var closesEnem = ClosestEnemyTobase();
                var hprelics = ObjectManager.Get<Obj_AI_Base>().Where(
                    r => r.IsValid && !r.IsDead && (r.Name.Contains("HealthRelic") || (r.Name.ToLower().Contains("bard") && ObjectManager.Player.ChampionName == "Bard") || (r.Name.ToLower().Contains("blobdrop") && ObjectManager.Player.ChampionName == "Zac"))
                        && !usedRelics.Contains(r.NetworkId) && (closesEnem == null || (r.Name.ToLower().Contains("blobdrop") && ObjectManager.Player.ChampionName == "Zac") || r.Distance(ARAMSimulator.fromNex.Position, true) - 500 < closesEnem.Distance(ARAMSimulator.fromNex.Position, true))).ToList().OrderBy(r => ARAMSimulator.player.Distance(r, true));
                return hprelics.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static Obj_AI_Base ClosestEnemyTobase()
        {
            return
                EloBuddy.SDK.EntityManager.Heroes.Enemies
                    .Where(h => h.IsValid && !h.IsDead && h.IsVisible && h.IsEnemy)
                    .OrderBy(h => h.Distance(ARAMSimulator.fromNex.Position, true))
                    .FirstOrDefault();
        }

        /* LOGIC!!
         * 
         * Go to Kill minions
         * If no minions go for enemy tower
         * Cut path on enemies range
         * 
         * Orbwalk all the way
         * 
         * 
         * 
         */

    }
}
