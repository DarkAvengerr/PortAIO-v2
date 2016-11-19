using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Combo
{
    using System;
    using System.Collections.Generic;
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

        private DashPosition dashPos;

        private AIHeroClient Target => TargetSelector.GetTarget(1250, TargetSelector.DamageType.Physical);

        private Obj_AI_Base Minion => MinionManager.GetMinions(ObjectManager.Player.Position, 1000).
            FirstOrDefault(m => dashPos.DashEndPosition(m, spell.Spell.Range).Distance(Target.Position) <= ObjectManager.Player.Distance(Target));

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || !CheckGuardians()
                || (Menu.Item("CTurret").GetValue<bool>() && dashPos.DashEndPosition(Target, spell.Spell.Range).UnderTurret(true))
                || (Menu.Item("CEnemies").GetValue<Slider>().Value < ObjectManager.Player.CountEnemiesInRange(1000)))
            {
                return;
            }
        
            if(Minion != null && ObjectManager.Player.Position.Distance(Target.Position) > 85)
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

         //   Menu.AddItem(new MenuItem("DistanceRange", "Target Radius").SetValue(new Slider(125, 0, 600)));

            Menu.AddItem(new MenuItem("CEnemies", "Don't E Into X Enemies").SetValue(new Slider(3, 0, 5)));

            Menu.AddItem(new MenuItem("CTurret", "Turret Check").SetValue(true));
        }
    }
}