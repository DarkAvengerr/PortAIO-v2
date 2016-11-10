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
    class Skills
    {
        private Spell Q, W, E, R, passive;
        private SpellSlot ignite;

        public Skills()
        {
            Q = new Spell(SpellSlot.Q, 800); // circle
            W = new Spell(SpellSlot.W, 850);  // circle
            E = new Spell(SpellSlot.E, 1100); // line
            R = new Spell(SpellSlot.R, 700); // circle
            passive = new Spell(SpellSlot.Q, 1470);
            Q.SetSkillshot(Q.Instance.SData.SpellCastTime, Q.Instance.SData.LineWidth, Q.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(W.Instance.SData.SpellCastTime, W.Instance.SData.LineWidth, W.Instance.SData.MissileSpeed,false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(E.Instance.SData.SpellCastTime, E.Instance.SData.LineWidth, E.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(R.Instance.SData.SpellCastTime, R.Instance.SData.LineWidth, R.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotCircle);
            passive.SetSkillshot(passive.Instance.SData.SpellCastTime, passive.Instance.SData.LineWidth, passive.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotLine);
            ignite = ObjectManager.Player.GetSpellSlot("SummonerDot");
        }

       public Spell getQ()
        {
            return Q;
        }
       public Spell getpassive()
       {
           return passive;
       }
       public Spell getW()
       {
           return W;
       }
       public Spell getE()
       {
           return E;
       }
       public Spell getR()
       {
           return R;
       }

       public HitChance hitchanceCheck(int i)
       {
           switch (i)
           {
               case 1:
                   return HitChance.Low;
               case 2:
                   return HitChance.Medium;
               case 3:
                   return HitChance.High;
               case 4:
                   return HitChance.VeryHigh;
           }
           return HitChance.Low;
       }

       public bool qCast(Obj_AI_Base target, int hitChance)
       {
           if (target == null) return false;
           HitChance hit = hitchanceCheck(hitChance);  
           if (Q.IsReady() && Q.IsInRange(target))
           {
               Q.CastIfHitchanceEquals(target, hit);
               return true;
           }
           return false;

       }

       public bool passiveCast(int hitChance)
       {
           HitChance hit = hitchanceCheck(hitChance);  
           if (!passive.IsReady())
               return false;
           var target = TargetSelector.GetTarget(passive.Range, TargetSelector.DamageType.Magical);
           if (!target.IsValidTarget(E.Range))
               return false;
           passive.CastIfHitchanceEquals(target, hit);
           return true;
       }

       public bool wCast(Obj_AI_Base target, int hitChance)
       {
           
           if (target == null) return false;
           HitChance hit = hitchanceCheck(hitChance);
           if (W.IsReady() && W.IsInRange(target))
           {
               W.CastIfHitchanceEquals(target, hit);
               return true;
               /*
               if (Player.Spellbook.GetSpell(SpellSlot.W).Ammo.ToString() == "2")
               {
                   Chat.Print("Dentro de 2 wCast");
                   W.CastIfHitchanceEquals(target, HitChance.High);
                   LeagueSharp.Common.Utility.DelayAction.Add(400, () => W.CastIfHitchanceEquals(target, HitChance.High));
                   return true;
               }
               else if (Player.Spellbook.GetSpell(SpellSlot.W).Ammo.ToString() == "1")
               {
                   Chat.Print("Dentro de 1 wCast");
                   W.CastIfHitchanceEquals(target, HitChance.High);
                   return true;
               }
             * */
           }
           return false;
       }

       public bool eCast(Obj_AI_Base target, int hitChance)
       {
             if (target == null) return false;
             if (E.IsReady() && E.IsInRange(target))
           {
               HitChance hit = hitchanceCheck(hitChance);              
               E.CastIfHitchanceEquals(target, hit);
               return true;
           }
           return false;
        }

       public bool rCast(Obj_AI_Base target, int hitChance)
       {
           if (target == null) return false;
           if (R.IsReady() && R.IsInRange(target))
           {
               R.CastIfHitchanceEquals(target, hitchanceCheck(hitChance));
               return true;
           }
           return false;
       }

       public bool rCastHit(Obj_AI_Base target , int min)
       {
           if (target == null) return false;
           if (R.IsReady() && R.IsInRange(target))
           {
               R.CastIfWillHit(target, min);
           }
           return false;
       }

       public bool igniteCast(Obj_AI_Base target)
        {
            if (ignite.IsReady() && target.Health - ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) <= 0)
            {
                ObjectManager.Player.Spellbook.CastSpell(ignite, target);
                return true;
            }
            return false;
        }
    }
}
