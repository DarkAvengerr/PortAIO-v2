using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.Spells
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SharpDX;

    internal class Q2Spell : SpellChild
    {
        public override string Name { get; set; } = "Piercing Light";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return !Spell.IsReady() ? 0 : Spell.GetDamage(target);
        }

        public Vector3 QPred(Obj_AI_Base target)
        {
            var pos = Spell.GetPrediction(target);

            return pos.UnitPosition;
        }

        public bool QMinionExtend(Obj_AI_Base minion)
        {
            var target = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(Spell.Range));

            if (minion == null || target == null)
            {
                return false;
            }

            var prediction = Spell.GetPrediction(target);

            if (prediction.Hitchance < HitChance.Medium)
            {
                return false;
            }

            var hit = new Geometry.Polygon.Rectangle(ObjectManager.Player.Position, ObjectManager.Player.Position.Extend(minion.Position, Spell.Range), Spell.Width);

            return !hit.IsOutside(QPred(target).To2D());
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.Q, 980);
            Spell.SetSkillshot(.5f, 50, float.MaxValue, false, SkillshotType.SkillshotLine);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
