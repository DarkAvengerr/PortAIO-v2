using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.Drawings.Spells
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class WDrawing : ChildBase
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell spell;

        public WDrawing(WSpell spell)
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
                                     ? Color.OrangeRed
                                     : Color.DarkSlateGray,
                                     Menu.Item("LeeSin.Draw.W.Width").GetValue<Slider>().Value,
                                     Menu.Item("LeeSin.Draw.W.Z").GetValue<bool>());
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

            Menu.AddItem(new MenuItem("LeeSin.Draw.W.Z", "Draw Z").SetValue(false));

            Menu.AddItem(new MenuItem("LeeSin.Draw.W.Width", "Thickness").SetValue(new Slider(3, 1, 5)));
        }
    }
}
