using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;
using SharpDX;
using DetuksSharp;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class Zac : Champion
    {
        public static float zacETime;
        public Zac()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Spirit_Visage),
                    new ConditionalItem(ItemId.Mercurys_Treads, ItemId.Ninja_Tabi, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Sunfire_Cape),
                    new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                    new ConditionalItem(ItemId.Warmogs_Armor),
                    new ConditionalItem(ItemId.Abyssal_Scepter),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Giants_Belt,ItemId.Boots_of_Speed
                }
            };
            Obj_AI_Base.OnSpellCast += Game_ProcessSpell;
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name == "ZacE")
            {
                if (zacETime == 0f)
                {
                    zacETime = System.Environment.TickCount;
                    LeagueSharp.Common.Utility.DelayAction.Add(4000, () => { zacETime = 0f; });
                }
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast();
        }

        public double GetAngle(Obj_AI_Base source, Vector3 target)
        {
            if (source == null || !target.IsValid())
            {
                return 0;
            }
            return Geometry.AngleBetween(
                Geometry.Perpendicular(Geometry.To2D(source.Direction)), Geometry.To2D(target - source.Position));
            ;
        }

        public int[] eRanges = new int[] { 1150, 1300, 1450, 1600, 1750 };
        public float[] eChannelTimes = new float[] { 0.9f, 1.05f, 1.2f, 1.35f, 1.5f };
        public override void useE(Obj_AI_Base target)
        {
            if (target.Distance(player) > eRanges[E.Level - 1] || LeagueSharp.SDK.Extensions.IsUnderEnemyTurret(target))
            {
                return;
            }
            var eFlyPred = E.GetPrediction(target);
            var enemyPred = Prediction.GetPrediction(
                target, eChannelTimes[E.Level - 1] + target.Distance(player) / E.Speed / 1000);
            if (E.IsCharging)
            {
                Console.WriteLine("Jump!!!");
                if (eFlyPred.CastPosition.Distance(player.Position) < E.Range)
                {
                    E.CastIfHitchanceEquals(target, HitChance.High);
                }
                else if (eFlyPred.UnitPosition.Distance(player.Position) < E.Range && target.Distance(player) < 500f)
                {
                    E.CastIfHitchanceEquals(target, HitChance.Medium);
                }
                else if ((eFlyPred.CastPosition.Distance(player.Position) < E.Range &&
                          eRanges[E.Level - 1] - eFlyPred.CastPosition.Distance(player.Position) < 200) ||
                         (GetAngle(player, eFlyPred.CastPosition) > 35))
                    
                {
                    E.CastIfHitchanceEquals(target, HitChance.Medium);
                }
                else if (eFlyPred.CastPosition.Distance(player.Position) < E.Range && zacETime != 0 &&
                         System.Environment.TickCount - zacETime > 2500)
                {
                    E.CastIfHitchanceEquals(target, HitChance.Medium);
                }
            }
            else if (enemyPred.UnitPosition.Distance(player.Position) < eRanges[E.Level - 1] &&
                     300 < target.Distance(player.Position))
            {
                E.SetCharged("ZacE", "ZacE", 300, eRanges[E.Level - 1], eChannelTimes[E.Level - 1]);
                E.StartCharging(eFlyPred.UnitPosition);
            }
        }
        private static bool eActive
        {
            get { return player.Buffs.Any(buff => buff.Name == "ZacE"); }
        }

        public override void killSteal()
        {
            if (E.IsCharging || eActive)
            {
                DeathWalker.setAttack(false);
                DeathWalker.setMovement(false);
            }
            else
            {
                DeathWalker.setAttack(true);
                DeathWalker.setMovement(true);
            }
        }

        public override void useR(Obj_AI_Base target)
        {
            if (R.CanCast(target))
            {
                AutoUlt();
            }
            
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 550);
            W = new Spell(SpellSlot.W, 350);
            E = new Spell(SpellSlot.E, 1550);
            R = new Spell(SpellSlot.R,400);

            Q.SetSkillshot(550, 120, int.MaxValue, false, SkillshotType.SkillshotLine);

            E.SetSkillshot(0.75f, 230, 1500, false, SkillshotType.SkillshotCircle);
            E.SetCharged("ZacE", "ZacE", 295, eRanges[0], eChannelTimes[0]);

        }

        public override void farm()
        {

            if (E.IsCharging || eActive)
                return;
            base.farm();
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            if (E.IsCharging || eActive)
                return;
            tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        void AutoUlt()
        {
            var comboR = 2;

            if (comboR > 0 && R.IsReady())
            {
                int enemiesHit = 0;
                int killableHits = 0;

                foreach (AIHeroClient enemy in ObjectManager.Get<AIHeroClient>().Where(he => he.IsEnemy && he.IsValidTarget(R.Range)))
                {
                    var prediction = Prediction.GetPrediction(enemy, R.Delay);

                    if (prediction != null && prediction.UnitPosition.Distance(ObjectManager.Player.ServerPosition) <= R.Range)
                    {
                        enemiesHit++;

                        if (ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.W) >= enemy.Health)
                            killableHits++;
                    }
                }

                if (enemiesHit >= comboR ||
                    (killableHits >= 1 && ObjectManager.Player.Health/ObjectManager.Player.MaxHealth <= 0.1))
                {
                    Aggresivity.addAgresiveMove(new AgresiveMove(105,4000,true));
                    CastR();
                }
            }
        }
        
        void CastR()
        {
            if (!R.IsReady())
                return;
            R.Cast();
        }
    }
}
