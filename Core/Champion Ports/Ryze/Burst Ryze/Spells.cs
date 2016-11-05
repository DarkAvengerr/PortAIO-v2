using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RyzeAssembly
{
    class Spells
    {
        private Spell _q, _w, _e, _r;
        public Spell Q { get { return _q; } }
        public Spell W { get { return _w; } }
        public Spell E { get { return _e; } }
        public Spell R { get { return _r; } }
        private SpellSlot ignite;
        public Spells(){
            _q = new Spell(slot:SpellSlot.Q,range:900,damageType:TargetSelector.DamageType.Magical);
            _w = new Spell(slot: SpellSlot.W, range: 600, damageType: TargetSelector.DamageType.Magical);
            _e = new Spell(slot: SpellSlot.E, range: 600, damageType: TargetSelector.DamageType.Magical);
            _r = new Spell(slot: SpellSlot.R);
            _q.SetSkillshot(delay:250,width:100,speed:1700,collision:true,type:SkillshotType.SkillshotLine);
            ignite = ObjectManager.Player.GetSpellSlot("SummonerDot");
        }
        public bool qCastPred()
        {
            //  Utils.ShowNotification("Q Cast!", System.Drawing.Color.White, 100);
           var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            if (target == null) return false;
            if (_q.IsReady())
            {
                Q.CastIfHitchanceEquals(target, HitChance.High);
                return true;
            }
            return false;

        }
        public bool qCast()
        {
            //  Utils.ShowNotification("Q Cast!", System.Drawing.Color.White, 100);
            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            if (target == null) return false;
            if (_q.IsReady())
            {
                _q.Cast(target.ServerPosition);
                return true;
            }
            return false;

        }
        public bool wCast()
        {
            //  Utils.ShowNotification("Q Cast!", System.Drawing.Color.White, 100);
            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            if (target == null) return false;
            if (_w.IsReady())
            {
                W.Cast(target);
                return true;
            }
            return false;

        }
        public bool eCast()
        {
            //  Utils.ShowNotification("Q Cast!", System.Drawing.Color.White, 100);
            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            if (target == null) return false;
            if (_e.IsReady())
            {
                E.Cast(target);
                return true;
            }
            return false;

        }
        public bool rCast()
        {
            //  Utils.ShowNotification("Q Cast!", System.Drawing.Color.White, 100);
            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            if (target == null) return false;
            if (_r.IsReady())
            {
                _r.Cast();
            }
            return false;

        }
        public bool igniteCast()
        {
            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            if (target != null)
                if (ignite.IsReady() && target.Health - ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) <= 0)
                {
                    ObjectManager.Player.Spellbook.CastSpell(ignite, target);
                    return true;
                }
            return false;
        }
    }
}
