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
    class Skills
    {
        private Spell Q, W, E, R;
        private SpellSlot ignite;
        public Obj_AI_Base miniondraw;
        public Skills()
        {
            Q = new Spell(SpellSlot.Q, 1080); // line
            W = new Spell(SpellSlot.W, 300);  // circle
            E = new Spell(SpellSlot.E, 350); // line
            R = new Spell(SpellSlot.R, 550); // circle

            Q.SetSkillshot(Q.Instance.SData.SpellCastTime, Q.Instance.SData.LineWidth, Q.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(W.Instance.SData.SpellCastTime, W.Instance.SData.LineWidth, W.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(E.Instance.SData.SpellCastTime, E.Instance.SData.LineWidth, E.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(W.Instance.SData.SpellCastTime, R.Instance.SData.LineWidth, R.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotCircle);
            ignite = ObjectManager.Player.GetSpellSlot("SummonerDot");
        }

        public Spell getQ()
        {
            return Q;
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
        private bool haveW
        {
            get { return ObjectManager.Player.HasBuff("AuraofDespair"); }
        }
        public bool qCast(Obj_AI_Base target, int hitChance)
        {
            if (target == null) return false;
            HitChance hit = hitchanceCheck(hitChance);
            if (Q.IsReady())
            {
                Q.CastIfHitchanceEquals(target, hit);
                return true;
            }
            return false;

        }

        public void wCast()
        {
            if (!haveW)
                {
                    W.Cast();
                }  
        }
        public void wDeCast()
        {

            if (haveW)
            {
                W.Cast();
            }

        }
        public bool wCast(Obj_AI_Base target)
        {
           
            if (target == null) return false;
                if (W.IsInRange(target))
                {
                    if (!haveW)
                    {
                        W.Cast();
                        return true;
                    }
                }
                else
                {
                    if (haveW)
                    {
                        W.Cast();
                        return true;
                    }
                }           

            return false;
        }
        public bool eCast(Obj_AI_Base target)
        {
            if (target == null) return false;
            if (E.GetPrediction(target).UnitPosition.Distance(ObjectManager.Player.ServerPosition) <= E.Range) { 
                E.CastOnUnit(ObjectManager.Player);
                
                return true;
           }
            return false;
        }
    public bool WillHitEnemys(Obj_AI_Base zone , int Range , int min)
        {
        int i =0;
    //    this.min = min;
        foreach(AIHeroClient b in ObjectManager.Get<AIHeroClient>())
        {
            if(b.IsEnemy && !b.IsDead && b.Distance(zone)< Range)
            {
                i++;
            }
        }
        this.min = i;
        if (i>= min)
            return true;
        else
            return false;
        }
    public int min;
        public bool rCast(Obj_AI_Base target, int min , bool useQMinion)
        {
            if (target == null) return false;
            Obj_AI_Base minion = ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsEnemy && Q.CanCast(x) &&
                                    Q.IsInRange(x) && WillHitEnemys(x,500,min)).FirstOrDefault<Obj_AI_Base>();


            miniondraw = minion;

            if (R.IsReady() && !R.IsInRange(target))
            {
                if (Q.CanCast(target))
                {              
                    if (Q.IsReady())
                        Q.CastIfHitchanceEquals(target, HitChance.High);
                        if(!ObjectManager.Player.IsDashing() && R.CanCast(target) && R.IsInRange(target) && WillHitEnemys(target,550,min))
                            R.Cast(target);
                        return true;
                }
                else if (Q.CanCast(minion) && useQMinion)
                {                                 
                    if (Q.IsReady())
                        Q.CastIfHitchanceEquals(minion, HitChance.High);
                    if (!ObjectManager.Player.IsDashing() && R.CanCast(target) && R.IsInRange(target) && WillHitEnemys(target, 550, min))
                          R.Cast(target);
                        return true;
                }
                else
                    return false;
            }
            else
            {
                if (WillHitEnemys(target, 550, min))
                {
                    R.Cast(target);
                    return true;
                }
            }
            return false;      
        }
      
        public bool IgniteCast(AIHeroClient target)
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
