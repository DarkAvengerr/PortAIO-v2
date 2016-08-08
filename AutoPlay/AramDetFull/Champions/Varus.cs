using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DetuksSharp;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class Varus : Champion
    {
        public Varus()
        {
            DeathWalker.BeforeAttack += beforeAttack;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.The_Bloodthirster),
                            new ConditionalItem(ItemId.Berserkers_Greaves),
                            new ConditionalItem(ItemId.Infinity_Edge),
                            new ConditionalItem(ItemId.Phantom_Dancer),
                            new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Vampiric_Scepter,ItemId.Boots_of_Speed
                        }
            };
        }

        private void beforeAttack(DeathWalker.BeforeAttackEventArgs args)
        {
            args.Process = !Q.IsCharging;
        }


        public override void useQ(Obj_AI_Base target)
        {
            if (Q.LSIsReady())
            {
                if (Q.IsCharging)
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }

                if (!Q.IsCharging)
                {
                    Q.StartCharging();
                    return;
                }
            }
        }

        public override void useW(Obj_AI_Base target)
        {
          //  if (!W.LSIsReady())
          //      return;
          //  W.Cast(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.LSIsReady() || Q.IsCharging)
                return;
            E.Cast(target, true);

        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.LSIsReady() || Q.IsCharging)
                return;
            R.Cast(target);
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 1600);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R, 1300);

            Q.SetSkillshot(0.3f, 80f, 1300f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 235f, 1500f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 100f, 1950f, false, SkillshotType.SkillshotLine);
            Q.SetCharged("VarusQ", "VarusQ", 250, 1600, 1.2f);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }


    }
}
