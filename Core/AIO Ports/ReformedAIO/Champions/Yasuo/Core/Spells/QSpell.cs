using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SharpDX;

    internal class QSpell : SpellChild
    {
        public override string Name { get; set; } = "Steel Tempest";

        public override Spell Spell { get; set; }

        private Vector2 dashEndPos;

        private const int EDelay = 450;

        private const string YasuoQ3W = "YasuoQ3W";

        public SpellState Spellstate { get; set; }

        internal enum SpellState
        {
            Standard,

            Whirlwind,

            DashQ,
        }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public bool CanEQ(Obj_AI_Base target)
        {
            return target.Position.Distance(ObjectManager.Player.Position) < 250 - target.BoundingRadius;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.Q, 475);

            Spell.SetSkillshot(.2f, 20, float.MaxValue, false, SkillshotType.SkillshotLine);

            Obj_AI_Base.OnBuffGain += OnBuffAdd;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
            CustomEvents.Unit.OnDash += OnDash;
        }

        private void SetWhirlwhind()
        {
            Spellstate = SpellState.Whirlwind;
            Spell.SetSkillshot(.35f, 90, 1200, false, SkillshotType.SkillshotLine);
            Spell.Range = 900;
        }

        private void SetStandard()
        {
            Spell.Range = 475;
            Spell.SetSkillshot(.2f, 20, float.MaxValue, false, SkillshotType.SkillshotLine);
            Spellstate = SpellState.Standard;
        }

        private void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe || args.Buff.Name != YasuoQ3W)
            {
                return;
            }

            SetWhirlwhind();
        }

        private void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (!sender.IsMe || args.Buff.Name != YasuoQ3W)
            {
                return;
            }

            SetStandard();
        }

        private void OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (!sender.IsMe) return;

            Spellstate = SpellState.DashQ;
           
            if (ObjectManager.Player.HasBuff(YasuoQ3W))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(EDelay, SetWhirlwhind);
            }
            else
            {
                LeagueSharp.Common.Utility.DelayAction.Add(EDelay, SetStandard);
            }

            dashEndPos = args.EndPos;
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
