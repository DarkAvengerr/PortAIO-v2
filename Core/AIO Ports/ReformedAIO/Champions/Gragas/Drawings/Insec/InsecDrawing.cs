using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.Drawings.Insec
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using Color = System.Drawing.Color;

    internal sealed class InsecDrawing : OrbwalkingChild
    {
        public override string Name { get; set; } = "Insec (DEBUG)";

        private readonly QSpell qSpell;
        private readonly RSpell rSpell;

        public InsecDrawing(QSpell qSpell, RSpell rSpell)
        {
            this.qSpell = qSpell;
            this.rSpell = rSpell;
        }

        private static AIHeroClient Target => TargetSelector.GetSelectedTarget();

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead
                || Target == null
                || !Target.IsVisible
                || !rSpell.Spell.IsReady()
                || Target.Distance(ObjectManager.Player) > 1800)
            {
                return;
            }

            if (!qSpell.Spell.IsReady())
            {
                return;
            }

            var pos = rSpell.InsecPositioner(Target, true, true);

            Drawing.DrawLine(Drawing.WorldToScreen(ObjectManager.Player.Position), Drawing.WorldToScreen(Target.Position), 1, Color.White);

            Render.Circle.DrawCircle(pos, 50f, Color.Cyan);

            Render.Circle.DrawCircle(pos.Extend(Target.Position, 840f), 10f, Color.AliceBlue);

            Drawing.DrawLine(Drawing.WorldToScreen(Target.Position), Drawing.WorldToScreen(pos), 1, Color.White);
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Drawing.OnEndScene -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Drawing.OnEndScene += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Gragas.Drawing...", "Draws BEST possible prediction"));
        }
    }
}