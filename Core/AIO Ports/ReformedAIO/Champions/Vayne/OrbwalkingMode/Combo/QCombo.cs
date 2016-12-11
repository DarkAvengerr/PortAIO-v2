using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.OrbwalkingMode.Combo
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Vayne.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        private readonly DashSmart dashSmart;

        public QCombo(QSpell spell, DashSmart dashSmart)
        {
            this.spell = spell;
            this.dashSmart = dashSmart;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range + ObjectManager.Player.AttackRange, TargetSelector.DamageType.Physical);

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Target == null
                 || !sender.IsMe 
                 || Menu.Item("Vayne.Combo.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                 || (Menu.Item("Vayne.Combo.Q.Stack").GetValue<bool>() && !spell.WStack(Target) && ObjectManager.Player.Level > 1)
                 || !CheckGuardians())
            {
                return;
            }
           
            if (Menu.Item("Vayne.Combo.Q.Melee").GetValue<bool>() && Target.Distance(ObjectManager.Player) <= ObjectManager.Player.AttackRange / 2 - (int)Target.BoundingRadius)
            {
                spell.Spell.Cast(ObjectManager.Player.ServerPosition + (ObjectManager.Player.ServerPosition - Target.ServerPosition).Normalized() * 300);
            }

            switch (Menu.Item("Vayne.Combo.Q.Mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Spell.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, spell.Spell.Range));
                    break;
                case 1:
                    spell.Spell.Cast(dashSmart.Kite(Target.Position.Extend(Target.Direction, spell.Spell.Range).To2D(), spell.Spell.Range).To3D());
                    break;
                case 2:
                    spell.Spell.Cast(dashSmart.ToSafePosition(Target, Target.Position, spell.Spell.Range));
                    break;
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);
          
            Obj_AI_Base.OnSpellCast -= OnSpellCast;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Vayne.Combo.Q.Mode", "Mode").SetValue(new StringList(new[] { "Cursor", "Kite", "Automatic" })));

            Menu.AddItem(new MenuItem("Vayne.Combo.Q.Melee", "Anti-Melee").SetValue(true));

            Menu.AddItem(new MenuItem("Vayne.Combo.Q.Stack", "Only Q To Proc W").SetValue(false));

            Menu.AddItem(new MenuItem("Vayne.Combo.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
