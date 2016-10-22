using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class Kalista : Champion
    {

        public AIHeroClient CoopStrikeAlly;
        public float CoopStrikeAllyRange = 1250f;

        public Kalista()
        {
           // DeathWalker.moveDelay = 200;
            DeathWalker.AfterAttack += onAfterAttack;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Runaans_Hurricane_Ranged_Only),
                            new ConditionalItem(ItemId.Berserkers_Greaves),
                            new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                            new ConditionalItem(ItemId.The_Bloodthirster),
                            new ConditionalItem(ItemId.Guinsoos_Rageblade),
                            new ConditionalItem(ItemId.Infinity_Edge),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Vampiric_Scepter,ItemId.Boots_of_Speed
                        }
            };
        }

        private void onAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe && ARAMSimulator.awayTo.IsValid())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, ARAMSimulator.awayTo.To3D());
            }
        }


        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return;
            if (!Sector.inTowerRange(target.Position.To2D()) &&
                (MapControl.balanceAroundPoint(target.Position.To2D(), 700) >= -1 ||
                 (MapControl.fightIsOn() != null && MapControl.fightIsOn().NetworkId == target.NetworkId)))
                Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || W.IsReady())
                return;
            E.Cast();
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            R.Cast(target);
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 5500);
            E = new Spell(SpellSlot.E, 1050);
            R = new Spell(SpellSlot.R, 1050);

            Q.SetSkillshot(0.25f, 40f, 1200f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotLine);
        }

        public override void useSpells()
        {
            if (CoopStrikeAlly == null)
            {
                if (LeagueSharp.Common.Items.HasItem(3599))
                {
                    var targ = ObjectManager.Get<AIHeroClient>()
                        .Where(h => h.IsAlly && h.IsValid)
                        .OrderBy(h => player.Distance(h, true))
                        .FirstOrDefault();
                    if (targ != null)
                        LeagueSharp.Common.Items.UseItem(3599, targ);
                }

                foreach (
                    var ally in
                        from ally in ObjectManager.Get<AIHeroClient>().Where(tx => tx.IsAlly && !tx.IsDead && !tx.IsMe)
                        where ObjectManager.Player.Distance(ally) <= CoopStrikeAllyRange
                        from buff in ally.Buffs
                        where buff.Name.Contains("kalistacoopstrikeally")
                        select ally)
                {
                    CoopStrikeAlly = ally;
                }
            }

            AIHeroClient t;

            if (Q.IsReady())
            {
                t = ARAMTargetSelector.getBestTarget(Q.Range);
                if (t != null)
                    Q.Cast(t);
            }

            if (E.IsReady())
            {
                foreach (var targ in HeroManager.Enemies.Where(o => o.IsValidTarget(E.Range) && !o.IsDead))
                {
                    if (targ.Health < player.GetSpellDamage(targ, SpellSlot.E))
                    {
                        E.Cast();
                    }
                }
                var deadMins =
                    MinionManager.GetMinions(E.Range)
                        .Count(o => o.IsValidTarget(E.Range) && !o.IsDead && E.GetDamage(o) > o.Health);

                if (deadMins > 1)
                    E.Cast();
            }

            if (!R.IsReady()) return;
            t = ARAMTargetSelector.getBestTarget(R.Range);
            if (t != null)
                R.Cast(t);
        }



        public int KalistaMarkerCount
        {
            get
            {
                var xbuffCount = 0;
                foreach (
                    var buff in from enemy in ObjectManager.Get<AIHeroClient>().Where(tx => tx.IsEnemy && !tx.IsDead)
                                where ObjectManager.Player.Distance(enemy) < E.Range
                                from buff in enemy.Buffs
                                where buff.Name.Contains("kalistaexpungemarker")
                                select buff)
                {
                    xbuffCount = buff.Count;
                }
                return xbuffCount;
            }
        }

        private float GetEDamage(Obj_AI_Base t)
        {
            if (!E.IsReady())
                return 0f;
            return (float)ObjectManager.Player.GetSpellDamage(t, SpellSlot.E);

            /* I think this calculation working good but i cant check now. after I'll do */
            var buff = t.Buffs.FirstOrDefault(xBuff => xBuff.DisplayName.ToLower() == "kalistaexpungemarker");
            if (buff.Count == 0)
                return 0f;

            double damage = ObjectManager.Player.FlatPhysicalDamageMod + ObjectManager.Player.BaseAttackDamage;
            double eDmg = damage * 0.60 + new double[] { 0, 20, 30, 40, 50, 60 }[E.Level];

            if (buff.Count == 1)
                return (float)eDmg;

            damage += buff.Count * 0.003 * damage + eDmg;
            return (float)ObjectManager.Player.CalcDamage(t, Damage.DamageType.Physical, damage);
        }
    }
}
