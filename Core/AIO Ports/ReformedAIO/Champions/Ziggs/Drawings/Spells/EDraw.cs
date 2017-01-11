using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs.Drawings.Spells
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ziggs.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using Color = System.Drawing.Color;

    internal sealed class EDrawing : ChildBase
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public EDrawing(ESpell spell)
        {
            this.spell = spell;
        }

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || spell.GameobjectLists == null)
            {
                return;
            }

            foreach (var vector3 in spell.GameobjectLists)
            {
                Render.Circle.DrawCircle(vector3.Position,
                    65,
                    Color.White,
                    1);
            }
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
        }
    }
}
