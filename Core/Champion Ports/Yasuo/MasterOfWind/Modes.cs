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
namespace MasterOfWind
{
    class Modes
    {
        public Obj_AI_Base selectedminion;
        public Obj_AI_Base selectedminions;
        private AIHeroClient target;
       private Program p;
      private  Skills skills;
        public Skills getSkills()
      {
          return skills;
      }
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
        //combos
        public void laneClear(){
            var minion = MinionManager.GetMinions(skills.getE().Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FindAll(m => !m.HasBuff("YasuoDashWrapper")).FirstOrDefault();
            var useETurret = p.getMenu().Item("E to turretL").GetValue<bool>();
            var useQ = p.getMenu().Item("QL").GetValue<bool>();
            var useE = p.getMenu().Item("EL").GetValue<bool>();
            var useEL = p.getMenu().Item("ELHH").GetValue<bool>();
            if (!useQ && useE)
            {
                if (skills.EturretCheck(minion, useETurret))
                    skills.eCast(minion,useEL);
            }

            else if (useQ && useE)
            {
                skills.qCast(minion);
                if (skills.EturretCheck(minion, useETurret))
                    skills.eCast(minion,useEL);
            }
            else if (useQ && !useE)
            {
                skills.qCast(minion);
            }
            else if (useQ && useE)
            {
                skills.qCast(minion);
                if (skills.EturretCheck(minion, useETurret))
                    skills.eCast(minion,useEL);
            }
            else
                return;
            selectedminion = minion;
        }
        public void autoQTarget(AIHeroClient target)
        {
            skills.qCast(target);
        }
        public void autoQMinion(AIHeroClient target)
        {
            if (!skills.HaveQ3)
            {
                var qMinions = getMinionsOnRange(skills.getQ());
                skills.qCast(qMinions);
            }
        }
        public void jungleClear()
        {
            var minion = MinionManager.GetMinions(skills.getE().Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FindAll(m => !m.HasBuff("YasuoDashWrapper")).FirstOrDefault();
            var useQ = p.getMenu().Item("QJ").GetValue<bool>();
            var useE = p.getMenu().Item("EJ").GetValue<bool>();
            if (useQ && !useE) skills.qCast(minion);
            else if (!useQ &&  useE) skills.eCast(minion);
            else if (!useQ &&  useE)
            {
                skills.eCast(minion);
            }

            else if (useQ &&  useE)
            {
                skills.qCast(minion);
                skills.eCast(minion);
            }
            else if (useQ &&  !useE)
            {
                skills.qCast(minion);
            }
            else if (useQ &&  useE)
            {
                skills.qCast(minion);
                skills.eCast(minion);
            }
            else
                return;
            selectedminion = minion;
        }
        public void harrash(AIHeroClient target)
        {
           //    skills.qCast(target);
            var useQ = p.getMenu().Item("QH").GetValue<bool>();
            var useE = p.getMenu().Item("EH").GetValue<bool>();
    //        int q = p.getMenu().Item("sethQ").GetValue<Slider>().Value;
            if (useQ &&  !useE)
            {
                skills.qCast(target);
            }
            else if (!useQ && useE)
            {
                skills.eCast(target);
            }
            else if (useQ &&  useE)
            {
                skills.qCast(target);
                skills.eCast(target);
            }
            else if (useQ && !useE)
            {
                skills.qCast(target);
            }
            else if (useQ &&  useE) //   Q+W+Q+E+Q+R
            {
                if(!skills.getQ().IsInRange(target))
                skills.eCast(target);
                
                skills.qCast(target);
            }
            else
            {
                return;
            }
        }
        public void flee(Obj_AI_Base target)
        {
            var useQ = p.getMenu().Item("Qflee").GetValue<bool>();
            var useQ3 = p.getMenu().Item("Q3flee").GetValue<bool>();
            //use w for flee?
            //load second q for flee?
            var eMinions = getMinionsOnRange(skills.getE());
          EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, p.getPlayer().Position.Extend(Game.CursorPos, 150));
            eFlee();
            if (useQ&&!skills.HaveQ3&&p.getPlayer().IsDashing())
            {
                skills.qCast(eMinions);
            }
            if (useQ3 && !skills.HaveQ3)
            {
                skills.qCast(target);
            }
        }
        public void eFlee()
        {
            var useETurret = p.getMenu().Item("E to turretF").GetValue<bool>();
            Obj_AI_Base minion = ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsMinion && skills.getE().IsInRange(x) && !x.HasBuff("YasuoDashWrapper")).MinOrDefault(x => x.Distance(Game.CursorPos));
            if(minion!=null)
            if (minion.Distance(Game.CursorPos) < ObjectManager.Player.Distance(Game.CursorPos)&& skills.EturretCheck(minion,useETurret))
           {
                skills.eCast(minion);
                selectedminions = minion;
           }
       }
        public void eLogic(AIHeroClient target)
        {
            var useETurret = p.getMenu().Item("E to turretC").GetValue<bool>();
            if(target!=null)
            if(!skills.getE().IsInRange(target))
            {
                Obj_AI_Base minion = ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsEnemy   && skills.getE().IsInRange(x) && !x.HasBuff("YasuoDashWrapper")).MinOrDefault(x => x.Distance(target));
                if(minion!=null)
                if (skills.EturretCheck(minion, useETurret))
                if (minion.Distance(target) < ObjectManager.Player.Distance(target) && skills.EturretCheck(minion,useETurret))
                {
                    skills.eCast(minion);
                    selectedminions = minion;
                }
            }
            else
            {
                if (skills.EturretCheck(target, useETurret))
                {
                    skills.eCast(target);
                    selectedminions = target;
                }
            }
        }
        public void gameObject_OnCreate(GameObject sender, EventArgs args2)
        {
            var useW = p.getMenu().Item("DW").GetValue<bool>();
           // var useE = p.getMenu().Item("DE").GetValue<bool>();
         //   if (!(sender is Obj_SpellMissile) || !sender.IsValid)return;
      /*      var args = (Obj)sender;
            if (!args.SpellCaster.IsValid<AIHeroClient>() || args.SpellCaster.IsAlly)
            {
                return;
            }
            if (args.Name == "missile" )
            {
                if(useW)
                {
                    if (p.getPlayer().Distance(args.Position) < 200)
                    {
                        skills.wCast(args.Position);
                    }
                }

            }*/
        }
        public float getMainComboDamage(Obj_AI_Base target)
        {
            float damage = (float)p.getPlayer().GetAutoAttackDamage(target);
            if (skills.getQ().IsReady())
            {
                damage += (float)p.getPlayer().GetSpellDamage(target, SpellSlot.Q) * 2;

            }
            if (skills.getE().IsReady())
            {
                damage += (float)p.getPlayer().GetSpellDamage(target, SpellSlot.E);
            }
            if (skills.getR().IsReady())
            {
                damage += (float)p.getPlayer().GetSpellDamage(target, SpellSlot.R);
            }
            return damage;
        }
        public void combo(AIHeroClient target)
        {
           var useQ = p.getMenu().Item("QC").GetValue<bool>();
            var useE = p.getMenu().Item("EC").GetValue<bool>();
            var useR = p.getMenu().Item("RC").GetValue<bool>();
            var numER = p.getMenu().Item("NumER").GetValue<Slider>().Value; 
            if (useR)
             skills.rCast(target,numER);
            if (useQ &&  !useE) skills.qCast(target);
            else if (!useQ && useE) eLogic(target);
            else if (!useQ &&  useE)
            {
                eLogic(target);
            }
            else if (useQ && useE)
            {
                skills.qCast(target);
                eLogic(target);
            }
            else if (useQ && !useE)
            {
                skills.qCast(target);
            }
            else if (useQ &&  useE) //   Q+W+Q+E+Q+R
            {
                skills.qCast(target);
                if(!skills.getQ().IsInRange(target))
                eLogic(target);
            }
            else
                return;
        }

         public Obj_AI_Base getMinionsOnRange(Spell spell)
        {
            return  ObjectManager.Get<Obj_AI_Base>().Where(x => spell.IsInRange(x)).MinOrDefault(x => x.Distance(Game.CursorPos)); 
        }
        public void lastHit(AIHeroClient target)
        {
            var useQ = p.getMenu().Item("QLH").GetValue<bool>();
            var useE = p.getMenu().Item("ELH").GetValue<bool>();
            var qMinions = getMinionsOnRange(skills.getQ());
            var eMinions = getMinionsOnRange(skills.getE());
            if(qMinions!=null)
                if (skills.getQ().IsKillable(qMinions) && useQ)
                {
                    skills.qCast(qMinions);
                }
             if (eMinions != null)
                if (skills.getE().IsKillable(eMinions) && useE)
                {
                    skills.eCast(qMinions);
                }
            
        }
    }
}
