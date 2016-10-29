using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ezreal.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ECombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell eSpell;

        private readonly DashSmart dashSmart;

        public ECombo(ESpell eSpell, DashSmart dashSmart)
        {
            this.eSpell = eSpell;
            this.dashSmart = dashSmart;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(eSpell.Spell.Range + 700, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || !CheckGuardians() || Target.HealthPercent > Menu.Item("Health").GetValue<Slider>().Value)
            {
                return;
            }

            var prediction = eSpell.Spell.GetPrediction(Target);

            if (ObjectManager.Player.Position.Extend(prediction.CastPosition, eSpell.Spell.Range + 700).CountEnemiesInRange(1000) >= Menu.Item("EnemiesCheck").GetValue<Slider>().Value)
            {
                return;
            }

            var minion = MinionManager.GetMinions(ObjectManager.Player.Position, eSpell.Spell.Range).FirstOrDefault();

            if (minion != null && prediction.CastPosition.Distance(minion.Position) < 300)
            {
                return;
            } 

            switch (Menu.Item("Mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    eSpell.Spell.Cast(Game.CursorPos);
                    break;
                case 1:
                    eSpell.Spell.Cast(dashSmart.Kite(Target.Position.To2D(), eSpell.Spell.Range).To3D());
                    break;
                case 2:
                    eSpell.Spell.Cast(dashSmart.ToSafePosition(Target, Target.Position, eSpell.Spell.Range));
                    break;
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("Mode", "E To: ").SetValue(new StringList(new []{"Cursor", "Side", "Automatic"})));

            Menu.AddItem(new MenuItem("EnemiesCheck", "Don't E Into X Enemies").SetValue(new Slider(2, 1, 5)));

            Menu.AddItem(new MenuItem("Health", "Min Target Health %").SetValue(new Slider(40, 0, 100)));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
