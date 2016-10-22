using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SyndraL33T
{
    public class SpellData
    {
        private float _range;
        public Func<Obj_AI_Base, double> Damage;
        public Func<float> RangeFunc;

        public SpellData(EloBuddy.SpellSlot slot,
            TargetSelector.DamageType damageType,
            int type,
            string spellName,
            float delay,
            float radius,
            float range,
            int missileSpeed,
            Func<float> rangeFunc = null,
            Func<Obj_AI_Base, double> damage = null)
        {
            SpellName = spellName;
            Delay = delay;
            Radius = radius;
            Range = range;
            MissileSpeed = missileSpeed;
            Instance = new Spell(slot, range, damageType);

            switch (type)
            {
                case 1:
                    Instance.SetSkillshot(delay, radius, missileSpeed, false, SkillshotType.SkillshotLine);
                    break;
                case 2:
                    Instance.SetTargetted(delay, missileSpeed);
                    break;
                case 3:
                    Instance.SetSkillshot(delay, radius, missileSpeed, false, SkillshotType.SkillshotCircle);
                    break;
                case 4:
                    Instance.SetSkillshot(delay, radius, missileSpeed, false, SkillshotType.SkillshotCone);
                    break;
            }

            RangeFunc = rangeFunc;
            Damage = damage;
        }

        public string SpellName { get; private set; }
        public float Delay { get; private set; }

        public float Range
        {
            get
            {
                if (RangeFunc != null && Math.Abs(Instance.Range - RangeFunc()) > float.Epsilon)
                {
                    Instance.Range = RangeFunc();
                }
                return RangeFunc != null ? RangeFunc() : _range;
            }
            private set { _range = value; }
        }

        public float Radius { get; private set; }
        public int MissileSpeed { get; private set; }
        public Spell Instance { get; private set; }
        public int LastCastAttemptTick { get; set; }

        public int Level
        {
            get { return Instance.Level; }
        }

        public bool IsReady()
        {
            return Instance.IsReady();
        }
    }
}