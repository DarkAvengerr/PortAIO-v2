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
    class Taliyah : Champion
    {
        private static Vector3 lastE;
        private static int lastETick = Environment.TickCount;
        private static bool Q5x = true;
        private static bool EWCasting = false;
        public Taliyah()
        {
            Console.WriteLine("Taliah on!!!!");
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Rod_of_Ages),
                    new ConditionalItem(ItemId.Sorcerers_Shoes),
                    new ConditionalItem(ItemId.Athenes_Unholy_Grail),
                    new ConditionalItem(ItemId.Zhonyas_Hourglass),
                    new ConditionalItem(ItemId.Rabadons_Deathcap),
                    new ConditionalItem(ItemId.Void_Staff),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Catalyst_the_Protector
                }
            };
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += OnInterruptableSpell;
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.LSIsReady())
                return;
            var pred = Q.GetPrediction(target);
            if(pred.Hitchance>HitChance.High || pred.AoeTargetsHit.Count>1)
                Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.LSIsReady())
                return;
            var pred = W.GetPrediction(target);
            if (pred.Hitchance >= HitChance.High)
                W.Cast(pred.UnitPosition);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.LSIsReady())
                return;

            if (W.LSIsReady() && W.IsInRange(target))
            {
                var pred = W.GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    lastE = ObjectManager.Player.ServerPosition;
                    E.Cast(ObjectManager.Player.ServerPosition +
                           (pred.CastPosition - ObjectManager.Player.ServerPosition).LSNormalized()*(E.Range - 200));
                    LeagueSharp.Common.Utility.DelayAction.Add(250, () =>
                    {
                        W.Cast(pred.UnitPosition);
                        EWCasting = false;
                    });
                    EWCasting = true;
                }
            }
            else
            {
                E.Cast(target);
                lastE = ObjectManager.Player.ServerPosition;
            }
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.LSIsReady())
                return;
           // if (player.LSCountEnemiesInRange(450) > 1 || player.HealthPercent < 25)
           //     R.Cast();
        }

        public override void killSteal()
        {
            if (!W.LSIsReady())
                return;
            var killable = HeroManager.Enemies.Where(h => h.Health < W.GetDamage(h) && W.IsInRange(h)).ToList();
            if (killable != null && killable.Any())
            {
                var pred = W.GetPrediction(killable.FirstOrDefault());
                if (pred.Hitchance >= HitChance.High)
                    W.Cast(pred.UnitPosition);
            }
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 900f);
            Q.SetSkillshot(0f, 60f, Q.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 900f);
            W.SetSkillshot(0.5f, 50f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 600f);
            E.SetSkillshot(0.25f, 150f, 2000f, false, SkillshotType.SkillshotLine);
        }

        public override void useSpells()
        {
            try
            {
                if (player.IsChannelingImportantSpell())
                    return;
                var tar = ARAMTargetSelector.getBestTarget(Q.Range);
                if (tar != null) useQ(tar);
                tar = ARAMTargetSelector.getBestTarget(E.Range);
                if (tar != null) useE(tar);
                tar = ARAMTargetSelector.getBestTarget(W.Range);
                if (tar != null) useW(tar);
               // tar = ARAMTargetSelector.getBestTarget(R.Range);
                //if (tar != null) useR(tar);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        private void OnInterruptableSpell(AIHeroClient unit, InterruptableSpell spell)
        {
            if (W.LSIsReady() && unit.LSIsValidTarget(W.Range))
                W.Cast(unit.ServerPosition);
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.LSIsReady() && gapcloser.Sender.LSIsValidTarget(W.Range))
                W.Cast(gapcloser.Sender.ServerPosition);
        }
        
    }
}
