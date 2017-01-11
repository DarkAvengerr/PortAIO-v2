using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Flee
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    class Wardjump : OrbwalkingChild
    {
        public override string Name { get; set; } = "Wardjump";

        private readonly WSpell spell;

        public Wardjump(WSpell spell)
        {
            this.spell = spell;
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || !spell.W1 || !Menu.Item("LeeSin.Wardjump").GetValue<KeyBind>().Active)
            {
                return;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            FleeJungle();
        }

        private void FleeJungle()
        {
            var position = ObjectManager.Player.Position + (Game.CursorPos - ObjectManager.Player.Position).Normalized() * 600;

            spell.Jump(position, 
                        false,
                        false, 
                        true);
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

            Menu.AddItem(new MenuItem("LeeSin.Wardjump", "Keybind:").SetValue(new KeyBind('G', KeyBindType.Press)));
        }
    }
}
