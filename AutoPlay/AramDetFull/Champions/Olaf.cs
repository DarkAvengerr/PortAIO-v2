using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DetuksSharp;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;

using EloBuddy; namespace ARAMDetFull.Champions
{
    internal class OlafA : Champion
    {
        public OlafA()
        {
            GameObject.OnCreate += onCreate;
            GameObject.OnDelete += onDelete;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.The_Black_Cleaver),
                            new ConditionalItem(ItemId.Mercurys_Treads,ItemId.Ninja_Tabi,ItemCondition.ENEMY_AP),
                            new ConditionalItem((ItemId)3053),
                            new ConditionalItem((ItemId)3748),
                            new ConditionalItem(ItemId.Maw_of_Malmortius,ItemId.Frozen_Mallet,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Spirit_Visage),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Phage
                        }
            };
        }
        

        public GameObject olafAxe = null;

        private void onDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "olaf_axe_totem_team_id_green.troy" && sender.IsAlly)
            {
                olafAxe = null;
            }
        }

        private void onCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "olaf_axe_totem_team_id_green.troy" && sender.IsAlly)
            {
                olafAxe = sender;
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.CastIfHitchanceEquals(target,HitChance.VeryHigh);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast();
            Aggresivity.addAgresiveMove(new AgresiveMove(55, 6000));
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.CastOnUnit(target);
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (player.HealthPercent < 65)
            {
                R.Cast();
                Aggresivity.addAgresiveMove(new AgresiveMove(85,8000));
            }
        }

        public override void useSpells()
        {
            gatherAze();

            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        public void gatherAze()
        {
            DeathWalker.CustomOrbwalkMode = false;
            if (olafAxe == null)
                return;
            if (!safeGap(olafAxe.Position.To2D()))
                return;
            DeathWalker.CustomOrbwalkMode = true;
            DeathWalker.deathWalkTarget(olafAxe.Position,DeathWalker.getBestTarget());

        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 325);
            E = new Spell(SpellSlot.E, 325);
            R = new Spell(SpellSlot.R,350);

            Q.SetSkillshot(0.25f, 75f, 1500f, false, SkillshotType.SkillshotLine);
        }
    }
}
