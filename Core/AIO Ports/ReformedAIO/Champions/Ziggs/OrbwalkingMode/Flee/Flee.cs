using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs.OrbwalkingMode.Flee
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ziggs.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;
    using ReformedAIO.Library.WallExtension;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class Flee : OrbwalkingChild
    {
        public override string Name { get; set; } = "Flee";

        private readonly WSpell spell;

        public Flee(WSpell spell)
        {
            this.spell = spell;
        }

        private WallExtension wall;

        private void OnUpdate(EventArgs args)
        {
            if (!Menu.Item("Ziggs.Flee.Keybind").GetValue<KeyBind>().Active)
            {
                return;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (Menu.Item("Ziggs.Flee.Wall").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(1000) <= 0)
            {
                var end = ObjectManager.Player.Position.Extend(Game.CursorPos, 300f);

                var point = wall.FirstWallPoint(ObjectManager.Player.Position, end);

                ObjectManager.Player.GetPath(point);

                var position = ObjectManager.Player.ServerPosition
                               + (ObjectManager.Player.ServerPosition - point).Normalized() * 200;

                if (point.Distance(ObjectManager.Player.Position) < 300f)
                {
                    spell.Spell.Cast(position);
                }
            }
            else
            {
                spell.Spell.Cast(ObjectManager.Player.Position);
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

            wall = new WallExtension();

            Menu.AddItem(new MenuItem("Ziggs.Flee.Wall", "Walljump").SetValue(true));

            Menu.AddItem(new MenuItem("Ziggs.Flee.Keybind", "Keybind: ").SetValue(new KeyBind('A', KeyBindType.Press)));
        }
    }
}