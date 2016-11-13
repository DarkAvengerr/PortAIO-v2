using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Harass
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;
    using ReformedAIO.Library.WallExtension;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public EHarass(ESpell spell)
        {
            this.spell = spell;
        }

        private WallExtension wall;

        private static AIHeroClient Target => TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);

        private Obj_AI_Base Minion => MinionManager.GetMinions(ObjectManager.Player.Position,
                 spell.Spell.Range).LastOrDefault(m => !m.UnderTurret(true));

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || !CheckGuardians()
                || (Menu.Item("Turret").GetValue<bool>() && Target.UnderTurret(true))
                || (Menu.Item("Enemies").GetValue<Slider>().Value >= ObjectManager.Player.CountEnemiesInRange(1000)))
            {
                return;
            }

            if (Target.Distance(ObjectManager.Player) <= spell.Spell.Range)
            {
                spell.Spell.CastOnUnit(Target);
            }

            if (Minion == null)
            {
                return;
            }

            var wallPoint = wall.FirstWallPoint(ObjectManager.Player.Position, Minion.Position);

            if (wall.IsWallDash(wallPoint, spell.Spell.Range))
            {
                return;
            }

            spell.Spell.CastOnUnit(Minion);
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            wall = new WallExtension();

            Menu.AddItem(new MenuItem("Enemies", "Don't E Into X Enemies").SetValue(new Slider(3, 0, 5)));

            Menu.AddItem(new MenuItem("Turret", "Turret Check").SetValue(true));
        }
    }
}