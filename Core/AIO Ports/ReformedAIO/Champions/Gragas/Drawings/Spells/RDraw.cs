using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.Drawings.Spells
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class RDrawing : ChildBase
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        public RDrawing(RSpell spell)
        {
            this.spell = spell;
        }

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            Render.Circle.DrawCircle(ObjectManager.Player.Position,
                                     spell.Spell.Range,
                                     spell.Spell.IsReady()
                                     ? Color.Cyan
                                     : Color.DarkSlateGray,
                                     Menu.Item("Example.Draw.R.Width").GetValue<Slider>().Value,
                                     Menu.Item("Example.Draw.R.Z").GetValue<bool>());
        }


        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Example.Draw.R.Z", "Draw Z").SetValue(false));

            Menu.AddItem(new MenuItem("Example.Draw.R.Width", "Thickness").SetValue(new Slider(3, 1, 5)));
        }
    }
}
