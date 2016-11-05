using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace MasterOfSadness
{
    class Modes
    {
        private AIHeroClient target;
        private Program p;
        private Skills skills;
        public void load(Program p)
        {
            skills = new Skills();
            this.p = p;
            target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical);
        }
        public AIHeroClient getTarget()
        {
            target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical);
            return target;
        }
        public Skills getSkills()
        {

            return skills;
        }


        //combos   
        public void laneClear()
        {
            var minion = MinionManager.GetMinions(skills.getQ().Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault();
            var useQ = p.getMenu().Item("QL").GetValue<bool>();
            var useW = p.getMenu().Item("WL").GetValue<bool>();
            var useE = p.getMenu().Item("EL").GetValue<bool>();
            var hit = p.getMenu().Item("seth").GetValue<Slider>().Value;

            if (useQ && !useW && !useE) skills.qCast(minion, hit);
            else if (!useQ && useW && !useE) skills.wCast(minion);
            else if (!useQ && !useW && useE) skills.eCast(minion);
            else if (!useQ && useW && useE)
            {
                skills.wCast(minion);
                skills.eCast(minion);
            }

            else if (useQ && !useW && useE)
            {
                skills.qCast(minion, hit);
                skills.eCast(minion);
            }
            else if (useQ && useW && !useE)
            {
                skills.qCast(minion, hit);
                skills.wCast(minion);
            }
            else if (useQ && useW && useE)
            {
                skills.qCast(minion, hit);
                skills.wCast(minion);
                skills.eCast(minion);

            }
            else
                return;
      
        }
        public void jungleClear()
        {
    
            var minion = MinionManager.GetMinions(skills.getQ().Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            var useQ = p.getMenu().Item("QJ").GetValue<bool>();
            var useW = p.getMenu().Item("WJ").GetValue<bool>();
            var useE = p.getMenu().Item("EJ").GetValue<bool>();
            var hit = p.getMenu().Item("seth").GetValue<Slider>().Value;

            if (useQ && !useW && !useE) skills.qCast(minion, hit);
            else if (!useQ && useW && !useE) skills.wCast(minion);
            else if (!useQ && !useW && useE) skills.eCast(minion);
            else if (!useQ && useW && useE)
            {
                skills.wCast(minion);
                skills.eCast(minion);
            }

            else if (useQ && !useW && useE)
            {
                skills.qCast(minion, hit);
                skills.eCast(minion);
            }
            else if (useQ && useW && !useE)
            {
                skills.qCast(minion, hit);
                skills.wCast(minion);
            }
            else if (useQ && useW && useE)
            {
                skills.qCast(minion, hit);
                skills.wCast(minion);
                skills.eCast(minion);
            }
            else
                return;
          
        }
        public void harrash(AIHeroClient target)
        {
         
            var useQ = p.getMenu().Item("QH").GetValue<bool>();
            var useW = p.getMenu().Item("WH").GetValue<bool>();
            var useE = p.getMenu().Item("EH").GetValue<bool>();
            var hit = p.getMenu().Item("seth").GetValue<Slider>().Value;         

            if (useQ && !useW && !useE)
            {
                skills.qCast(target, hit);
            }
            else if (!useQ && useW && !useE)
                skills.wCast(target);
            else if (!useQ && !useW && useE)
                skills.eCast(target);
            else if (!useQ && useW && useE)
            {
                skills.wCast(target);
                skills.eCast(target);
            }
            else if (useQ && !useW && useE)
            {
                skills.qCast(target, hit);
                skills.eCast(target);
            }
            else if (useQ && useW && !useE)
            {
                skills.qCast(target, hit);
                skills.wCast(target);
            }
            else if (useQ && useW && useE) 
            {
                skills.wCast(target);
                skills.eCast(target);
                skills.qCast(target, hit);
            }
            else
            {
                return;
            }
        }

        public void combo(AIHeroClient target)
        {
            var useQ = p.getMenu().Item("QC").GetValue<bool>();
            var useW = p.getMenu().Item("WC").GetValue<bool>();
            var useE = p.getMenu().Item("EC").GetValue<bool>();
            var useR = p.getMenu().Item("RC").GetValue<bool>();
            var useQr = p.getMenu().Item("RqC").GetValue<bool>();
            int useRpC = p.getMenu().Item("RcC").GetValue<Slider>().Value;
            var hit = p.getMenu().Item("seth").GetValue<Slider>().Value;

            //Faltan casos con r on 

            if (useQ && !useW && !useE && !useR)
                skills.qCast(target, hit);
            else if (!useQ && useW && !useE && !useR)
                skills.wCast(target);
            else if (!useQ && !useW && useE && !useR)
                skills.eCast(target);
            else if (!useQ && useW && useE && !useR)
            {
                skills.wCast(target);
                skills.eCast(target);
            }
            else if (useQ && !useW && useE && !useR)
            {
                skills.qCast(target, hit);
                skills.eCast(target);
            }
            else if (useQ && useW && !useE && !useR)
            {
                skills.qCast(target, hit);
                skills.wCast(target);
            }
            else if (useQ && useW && useE && !useR)
            {
                skills.qCast(target, hit);
                skills.wCast(target);
                skills.eCast(target);
            }
            else if (useQ && useW && useE && useR)
            {
                skills.rCast(target, useRpC, useQr);
                if (!skills.getR().IsReady() || !skills.WillHitEnemys(ObjectManager.Player,550,useRpC))
                {
                    if (skills.getQ().IsReady())
                        skills.qCast(target, hit);
                    skills.wCast(target);
                    skills.eCast(target);
                }
            }
            else
                return;
        }     
    }
}
