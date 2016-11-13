using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;
    using ReformedAIO.Library.WallExtension;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ECombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public ECombo(ESpell spell)
        {
            this.spell = spell;
        }

        private WallExtension wall;

        private DashPosition dashPos;

        private static AIHeroClient Target => TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);

        private Obj_AI_Base Minion => MinionManager.GetMinions(ObjectManager.Player.Position,
                 spell.Spell.Range).LastOrDefault(m => m.Distance(ObjectManager.Player.Position.Extend(Target.Position, 475)) <= 400);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || !CheckGuardians()
                || (Menu.Item("Turret").GetValue<bool>() && dashPos.DashEndPosition(Target, spell.Spell.Range).UnderTurret(true))
                || (Menu.Item("Enemies").GetValue<Slider>().Value < ObjectManager.Player.CountEnemiesInRange(1000)))
            {
                return;
            }

            if(Minion != null && (ObjectManager.Player.Position.Distance(Target.Position) > ObjectManager.Player.AttackRange || Target.HasBuff("YasuoDashWrapper")))
            {
                spell.Spell.CastOnUnit(Minion);
            }
            else
            {
                spell.Spell.CastOnUnit(Target);
            }
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

            dashPos = new DashPosition();
            wall = new WallExtension();

            Menu.AddItem(new MenuItem("Enemies", "Don't E Into X Enemies").SetValue(new Slider(3, 1, 5)));

            Menu.AddItem(new MenuItem("Turret", "Turret Check").SetValue(true));
        }
    }
}