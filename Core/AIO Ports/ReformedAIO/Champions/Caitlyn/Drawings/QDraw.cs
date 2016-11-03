using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.Drawings
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Logic;
    using ReformedAIO.Champions.Caitlyn.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class QDraw : ChildBase
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell qSpell;

        public QDraw(QSpell qSpell)
        {
            this.qSpell = qSpell;
        }

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            if (Menu.Item("QReady").GetValue<bool>() && !qSpell.Spell.IsReady()) return;

            Render.Circle.DrawCircle(
                 ObjectManager.Player.Position,
                qSpell.Spell.Range,
                qSpell.Spell.IsReady()
                 ? Color.Cyan
                 : Color.DarkSlateGray);
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("QReady", "Only If Ready").SetValue(false));
        }
    }
}
