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
    class DrMundo : Champion
    {

        public DrMundo()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Banshees_Veil,ItemId.Sunfire_Cape,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Mercurys_Treads),
                            new ConditionalItem(ItemId.Frozen_Mallet),
                            new ConditionalItem(ItemId.Spirit_Visage,ItemId.Randuins_Omen,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                            new ConditionalItem(ItemId.Maw_of_Malmortius,ItemId.Thornmail,ItemCondition.ENEMY_AP),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Giants_Belt,ItemId.Boots_of_Speed
                        }
            };
            DeathWalker.BeforeAttack += beforeAttack;
        }
        

        private void beforeAttack(DeathWalker.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe && args.Target is AIHeroClient && E.IsReady())
            {
                E.Cast();
                Aggresivity.addAgresiveMove(new AgresiveMove(105, 3500, true));
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null || target.HealthPercent > 65)
                return;
            W.CastOnUnit(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.Cast();
            Aggresivity.addAgresiveMove(new AgresiveMove(105, 5000, true));
        }

        public override void useR(Obj_AI_Base target)
        {
            if (target == null || !R.IsReady())
                return;
            if (player.HealthPercent < 60)
            {
                R.Cast();
                Aggresivity.addAgresiveMove(new AgresiveMove(65, 12000, true));
            }
        }

        public bool isBurning()
        {
            return ObjectManager.Player.HasBuff("BurningAgony");
        }

        public override void useSpells()
        {
            if (player.IsChannelingImportantSpell())
                return;
            if (W.IsReady())
            {
                if (isBurning() && player.CountEnemiesInRange(450) == 0)
                    W.Cast();
                if (!isBurning() && player.CountEnemiesInRange(450) > 0)
                    W.Cast();
            }

            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }


        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 325);
            E = new Spell(SpellSlot.E, 380);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.275f, 60, 1850, true, SkillshotType.SkillshotLine);

        }
        
    }
}
