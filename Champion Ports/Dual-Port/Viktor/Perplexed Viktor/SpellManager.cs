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
 namespace PerplexedViktor
{
    class SpellManager
    {
        private static AIHeroClient Player = ObjectManager.Player;

        private static Spell _Q, _W, _E, _E2, _R;

        public static Spell Q { get { return _Q; } }
        public static Spell W { get { return _W; } }
        public static Spell E { get { return _E; } }
        public static Spell E2 { get { return _E2; } }
        public static Spell R { get { return _R; } }
        public static SpellSlot IgniteSlot = Player.GetSpellSlot("SummonerDot");

        public static float UltCastedTime;

        public static void Initialize()
        {
            _Q = new Spell(SpellSlot.Q, 700);
            _Q.SetTargetted(0.25f, 2000);

            _W = new Spell(SpellSlot.W, 700);
            _W.SetSkillshot(0.25f, 300, float.MaxValue, false, SkillshotType.SkillshotCircle);

            _E = new Spell(SpellSlot.E, 700);
            _E.SetSkillshot(0.0f, 90, 1200, false, SkillshotType.SkillshotLine);

            _E2 = new Spell(SpellSlot.E, 550);

            _R = new Spell(SpellSlot.R, 700);
            _R.SetSkillshot(0.25f, 250, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public static bool UltHasBeenCasted
        {
            get { return R.Instance.Name != "ViktorChaosStorm"; }
        }

        public static bool HasQBuff
        {
            get { return Player.HasBuff("viktorpowertransferreturn"); }
        }

        public static void CastSpell(Spell spell, Obj_AI_Base target, HitChance hitChance)
        {
            if (target.IsValidTarget(spell.Range) && spell.GetPrediction(target).Hitchance >= hitChance)
                spell.Cast(target);
        }

        internal static void CastSpell(Spell spell, Vector3 pos)
        {
            spell.Cast(pos);
        }

        internal static void CastSpell(Spell spell, Vector3 from, Vector3 to)
        {
            spell.Cast(from, to);
        }

        internal static void CastSpell(Spell spell, Obj_AI_Base target)
        {
            spell.Cast(target);
        }
    }
}
