using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DariusSharp
{
    internal class ComboHandler
    {
        //Player
        private static AIHeroClient Player = ObjectManager.Player;

        //Spells
        private static Spell Q
        {
            get { return SpellHandler.Q; }
        }
        private static Spell W
        {
            get { return SpellHandler.W; }
        }
        private static Spell E
        {
            get { return SpellHandler.E; }
        }
        private static Spell R
        {
            get { return SpellHandler.R; }
        }

        //Logic
        private static void CastR(AIHeroClient target)
        {
            if (!R.IsReady())
                return;

            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsValidTarget(R.Range)))
            {
                if (Player.GetSpellDamage(target, SpellSlot.R) + SpellHandler.AdjustDamage() > hero.Health)
                {
                    R.Cast(target);
                }

                else if (Player.GetSpellDamage(target, SpellSlot.R) + SpellHandler.AdjustDamage() < hero.Health)
                {
                    foreach (var buff in hero.Buffs.Where(buff => buff.Name == "dariushemo"))
                    {
                        if (Player.GetSpellDamage(target, SpellSlot.R, 1) * (1 + buff.Count / 5) + SpellHandler.AdjustDamage() > target.Health)
                        {
                            R.CastOnUnit(target, true);
                        }
                    }
                }
            }
        }

        //Combo
        public static void ExecuteCombo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target == null)
                return;

            //Cast E first
            if (SpellHandler.IsEnabled("comboUseE") && E.CanCast(target) && (Q.IsReady() || R.IsReady()))
            {
                E.Cast(target.ServerPosition);
            }

            //Cast Q
            if (SpellHandler.IsEnabled("comboUseQ") && Q.CanCast(target))
            {
                Q.Cast();
            }
            
            //Cast R
            if (SpellHandler.IsEnabled("comboUseR") && R.CanCast(target) && !Q.IsKillable(target))
            {
                CastR(target);
               //R.Cast(target, SpellHandler.PacketCasting()); Disabled for now
            }
        }
   
        //Harass
        public static void ExecuteHarass()
        {
            //Get target
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target == null) //Return if no target
                return;

            //Return if not enough Mana
            if (Player.ManaPercentage() < ConfigHandler.SliderLinks["harassMana"].Value.Value)
                return;

            //Cast Q if all conditions met
            if (SpellHandler.IsEnabled("comboUseQ") && Q.CanCast(target))
                Q.Cast();
        }

        //Killsteal
        public static void ExecuteAdditionals()
        {
            foreach (var ksTarget in ObjectManager.Get<AIHeroClient>().Where(ksTarget => R.CanCast(ksTarget)))
                CastR(ksTarget);
        }

        //Afterattack
        public static void ExecuteAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!SpellHandler.IsEnabled("comboUseW") || !ConfigHandler.KeyLinks["comboActive"].Value.Active || !unit.IsMe || !(target is AIHeroClient))
                return;
            
            W.Cast();
        }
    }
}
