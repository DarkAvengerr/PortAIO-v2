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
 namespace MasterOfThorns
{
    class Modes
    {
        private AIHeroClient target;
        private Skills skills;
        private Program p;
        private int delay = 1000;
       
        public Skills getSkills()
        {
            return skills;
        }
     
        public void load(Program p)
        {
            skills = new Skills();
            this.p =p;
            target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical);
        }

        public AIHeroClient getTarget()
        {
            target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical);
            return target;
        }
  
        public bool zyraZombie()
        {
            return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name ==
                   ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Name ||
                   ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name ==
                   ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name;
        }

        public void laneClear()
        {          
            var minion = MinionManager.GetMinions(skills.getQ().Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minion == null) return;
            var useQ = p.getMenu().Item("QL").GetValue<bool>();
            var useW = p.getMenu().Item("WL").GetValue<bool>();
            var useE = p.getMenu().Item("EL").GetValue<bool>();
            int min = p.getMenu().Item("seth").GetValue<Slider>().Value;
            int w = p.getMenu().Item("sethW").GetValue<Slider>().Value;
            int q = p.getMenu().Item("sethQ").GetValue<Slider>().Value;
       //     Chat.Print("q: " + q + " w: " + w + " e: " + min);
            //    if (!useQ && !useW && !useE) return;
            if (useQ && !useW && !useE)  skills.qCast(minion,q,p);
            else if (!useQ && useW && !useE) skills.wCast(minion,w,p);
            else if (!useQ && !useW && useE) skills.eCast(minion, min,p);
            else if (!useQ && useW && useE) 
            {
                  skills.eCast(minion, min,p);
                  if (skills.getE().IsReady() && skills.getE().IsInRange(minion))
                      skills.wCast(minion,w,p);
            }
            else if (useQ && !useW && useE) 
            {
                skills.eCast(minion, min,p);
                skills.qCast(minion,q,p);
            }
            else if (useQ && useW && !useE) 
            {
                skills.qCast(minion,q,p);
                if (skills.getQ().IsReady() && skills.getQ().IsInRange(minion))
                    skills.wCast(minion,w,p);
            }
            else if (useQ && useW && useE)
            {
                skills.eCast(minion, min,p);
                if (skills.getE().IsReady() && skills.getE().IsInRange(minion))
                {
                    skills.wCast(minion,w,p);
                    LeagueSharp.Common.Utility.DelayAction.Add(delay, () => skills.qCast(minion,q,p));
                    if (skills.getQ().IsReady() && skills.getQ().IsInRange(minion))                    
                        LeagueSharp.Common.Utility.DelayAction.Add(delay, () => skills.wCast(minion,w,p));                                    
                }
            }
            else
                return;    
        }
 
        public void jungleClear()
        {
            var minion = MinionManager.GetMinions(skills.getQ().Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minion == null) return;
            var useQ = p.getMenu().Item("QJ").GetValue<bool>();
            var useW =p.getMenu().Item("WJ").GetValue<bool>();
            var useE =p.getMenu().Item("EJ").GetValue<bool>();
            int min = p.getMenu().Item("seth").GetValue<Slider>().Value;
            int q = p.getMenu().Item("sethQ").GetValue<Slider>().Value;
            int w = p.getMenu().Item("sethW").GetValue<Slider>().Value;
         //    Chat.Print("min: " + min);
        //    if (!useQ && !useW && !useE) return;
            if (useQ && !useW && !useE) skills.qCast(minion,q,p);
            else if (!useQ && useW && !useE) skills.wCast(minion,w,p);
            else if (!useQ && !useW && useE) skills.eCast(minion,min,p);           
            else if (!useQ && useW && useE) 
            {
                skills.eCast(minion, min,p);       
                if (skills.getE().IsReady() && skills.getE().IsInRange(minion))
                    skills.wCast(minion,w,p);
              
            }
            else if (useQ && !useW && useE) 
            {
                skills.eCast(minion, min,p);        
                skills.qCast(minion,q,p);             
            }
            else if (useQ && useW && !useE) 
            {
                skills.qCast(minion,q,p);
                if (skills.getQ().IsReady() && skills.getQ().IsInRange(minion)) 
                    skills.wCast(minion,w,p);                
            }
            else if (useQ && useW && useE)
            {
            //    Chat.Print("juhngle: ");
                skills.eCast(minion, min,p);
                if (skills.getE().IsReady() && skills.getE().IsInRange(minion))
                {
                    skills.wCast(minion,w,p);                   
                        LeagueSharp.Common.Utility.DelayAction.Add(delay, () => skills.qCast(minion,q,p));
                        if (skills.getQ().IsReady() && skills.getQ().IsInRange(minion))                        
                            LeagueSharp.Common.Utility.DelayAction.Add(delay, () => skills.wCast(minion,w,p));                                          
                }               
            }
            else
                return;    
 /*           
  * 
  * Funciona pero saca dos plantas iguales:
  * ----------
  *  skills.eCast(minion);
                if (skills.getE().IsReady() && skills.getE().IsInRange(minion))
                {
                    skills.wCast(minion);
                    Chat.Print("1 planta");

                   LeagueSharp.Common.Utility.DelayAction.Add(50, () => skills.qCast(minion));
                    if (skills.getQ().IsReady())
                    {
                        Chat.Print("2 planta");
                        LeagueSharp.Common.Utility.DelayAction.Add(400, () => skills.wCast(minion));
                        return true;
                    }
                    return true;
                }
                return false;
  * /*/
        }
        public void harrash(AIHeroClient target)
        {            
            var useQ = p.getMenu().Item("QH").GetValue<bool>();
            var useW = p.getMenu().Item("WH").GetValue<bool>();
            int q = p.getMenu().Item("sethQ").GetValue<Slider>().Value;
            int w = p.getMenu().Item("sethW").GetValue<Slider>().Value;
         
           // Chat.Print("haras useQ: "+useQ);
            //    if (!useQ && !useW && !useE) return;
            if (useQ && !useW)
                skills.qCast(getTarget(),q,p);
            else if (!useQ && useW)
                skills.wCast(getTarget(),w,p);
            else if (useQ && useW)
            {
                skills.qCast(getTarget(),q,p);
                if (skills.getQ().IsReady())
                    skills.wCast(getTarget(),w,p);
            }
            else
                return;     

        }

        public void flee(AIHeroClient target)
        {
            if (skills.getE().IsReady())
            {
                int min = p.getMenu().Item("seth").GetValue<Slider>().Value;
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, p.getPlayer().Position.Extend(Game.CursorPos, 150));
                skills.eCast(target, min, p);
            }
            else
                return;
        }

        public void onlyR(AIHeroClient target) // Comprobar
        {          
            if (skills.getR().IsReady()) //Añadir para cuantos campeones
            {
          //      EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, p.getPlayer().Position.Extend(Game.CursorPos, 150));
                var min = p.getMenu().Item("minEnemys").GetValue<Slider>().Value;
                skills.rCastHit(target,min);
            }
        }
        public void Eplant(int min , int w)
        {
        skills.eCast(target, min, p);
            if (this.skills.getE().IsReady())
                this.skills.wCast(target, w,p);
        }
        public void Qplant(int q , int w)
        {

        }
        public void combo(AIHeroClient target)
        {         
            var useQ = p.getMenu().Item("QC").GetValue<bool>();
            var useW = p.getMenu().Item("WC").GetValue<bool>();
            var useE = p.getMenu().Item("EC").GetValue<bool>();
            var useRkill = p.getMenu().Item("comboR").GetValue<bool>();
            var min = p.getMenu().Item("seth").GetValue<Slider>().Value;
            int q = p.getMenu().Item("sethQ").GetValue<Slider>().Value;
            int w = p.getMenu().Item("sethW").GetValue<Slider>().Value;
            int r= p.getMenu().Item("sethR").GetValue<Slider>().Value;
            int minr = p.getMenu().Item("minEnemys").GetValue<Slider>().Value;
            //    if (!useQ && !useW && !useE) return;
            if (useQ && !useW && !useE)  skills.qCast(getTarget(),q,p);            
            else if (!useQ && useW && !useE) skills.wCast(getTarget(),w,p);             
            else if (!useQ && !useW && useE) skills.eCast(getTarget(), min,p);
            else if (!useQ && useW && useE)
            {
                skills.eCast(target, min, p);
                if (skills.getE().IsReady() && skills.getE().IsInRange(getTarget()))
                    skills.wCast(getTarget(),w,p); 
            }
            else if (useQ && !useW && useE) 
            {
                skills.eCast(target, min, p);
                skills.qCast(getTarget(),q,p);
            }
            else if (useQ && useW && !useE) 
            {
                skills.qCast(getTarget(),q,p);
                if (skills.getQ().IsReady() && skills.getQ().IsInRange(getTarget()))
                    skills.wCast(getTarget(),w,p); 
            }
            else if (useQ && useW && useE)
            {
                skills.eCast(target,min, p);
            if (skills.getE().IsReady())
            skills.wCast(target,w,p);
            skills.qCast(target,q,p);
           if(skills.getQ().IsReady())
             skills.wCast(target,w,p);
            /*    skills.eCast(getTarget(), min);
                if (skills.getE().IsReady() && skills.getE().IsInRange(getTarget()))
                    skills.wCast(getTarget(), w);
                
                 
                     skills.qCast(getTarget(), q);
                    if (skills.getQ().IsReady() && skills.getQ().IsInRange(getTarget()))  
                       skills.wCast(getTarget(),w);  */
                  
                //        LeagueSharp.Common.Utility.DelayAction.Add(delay, () => skills.wCast(getTarget(),w));         
                //    skills.wCast(getTarget(),w);
                   // LeagueSharp.Common.Utility.DelayAction.Add(delay, () => skills.qCast(getTarget(),q));
                 //   if (skills.getQ().IsReady() && skills.getQ().IsInRange(getTarget()))                    
                //        LeagueSharp.Common.Utility.DelayAction.Add(delay, () => skills.wCast(getTarget(),w));         
                
            }
            if (useRkill)
            {
              if(target!=null)
                if (target.Health <= getSkills().getR().GetDamage(target))
                {
                    skills.rCast(target, r, p);
                }
            }
            getSkills().rCastHit(target, minr);
            
                return;
        }

        public void rCombo(AIHeroClient target)
        {        
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, p.getPlayer().Position.Extend(Game.CursorPos, 150)); //¿? No entiedo 
            var useQ = p.getMenu().Item("QrC").GetValue<bool>();
            var useE = p.getMenu().Item("ErC").GetValue<bool>();
            int min = p.getMenu().Item("seth").GetValue<Slider>().Value;
            int q = p.getMenu().Item("sethQ").GetValue<Slider>().Value;
            int w = p.getMenu().Item("sethW").GetValue<Slider>().Value;
            int r = p.getMenu().Item("sethR").GetValue<Slider>().Value;
            //    if (!useQ && !useW && !useE) return;
            if (useQ && !useE) 
            {
                skills.eCast(target, min, p);
                skills.rCast(getTarget(),r,p);
                skills.qCast(getTarget(),q,p);
            }
            else if (!useQ && useE)
            {
                skills.rCast(getTarget(),r,p);
                skills.eCast(target, min, p);
                if (skills.getE().IsReady() && skills.getE().IsInRange(getTarget()))
                    skills.wCast(getTarget(),w,p);                 
            }
            else if (useQ && useE)
            {

                skills.eCast(target, min, p);
                if (skills.getE().IsReady() && skills.getE().IsInRange(getTarget()))
                {
                    skills.wCast(getTarget(),w,p);
                    LeagueSharp.Common.Utility.DelayAction.Add(delay, () => skills.qCast(getTarget(),q,p));
                    if (skills.getQ().IsReady() && skills.getQ().IsInRange(getTarget()))
                        LeagueSharp.Common.Utility.DelayAction.Add(delay, () => skills.wCast(getTarget(),w,p));
                }
                skills.rCast(getTarget(),r,p);               
            }
            else
                return;   
        }
    }
}
