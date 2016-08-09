using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class Tristana : Champion
    {

        public Tristana()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Infinity_Edge),
                            new ConditionalItem(ItemId.Berserkers_Greaves),
                            new ConditionalItem(ItemId.Runaans_Hurricane_Ranged_Only),
                            new ConditionalItem((ItemId)3094),
                            new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                            new ConditionalItem(ItemId.Guinsoos_Rageblade),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Pickaxe,ItemId.Boots_of_Speed
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return;
            Q.Cast();
            Aggresivity.addAgresiveMove(new AgresiveMove(90));
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;

            if (EnemyInRange(2, 500) || (EnemyInRange(1, 500) && player.HealthPercent<35))
                W.Cast(player.Position.To2D().Extend(ARAMSimulator.fromNex.Position.To2D(), 900));
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;
            E.Cast(target);

        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            if (R.GetDamage(target) > target.Health || (player.HealthPercent < 40 || target.Distance(player)<400))
                R.CastOnUnit(target);
        }

        public override void setUpSpells()
        {
            //Ability Information - Range - Variables.
            Q = new Spell(SpellSlot.Q,650);

            //RocketJump Settings
            W = new Spell(SpellSlot.W, 900);
            W.SetSkillshot(0.25f, 150, 1200, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 630);
            R = new Spell(SpellSlot.R, 630);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        public static bool EnemyInRange(int numOfEnemy, float range)
        {
            return LeagueSharp.Common.Utility.CountEnemysInRange(ObjectManager.Player, (int)range) >= numOfEnemy;
        }
    }
}
