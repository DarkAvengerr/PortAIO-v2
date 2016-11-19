using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Yasuo.Drawings.SpellDrawings
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class QDrawing : ChildBase
    {
        public override string Name { get; set; } = "Q";

        private readonly Q1Spell spell;

        private readonly Q3Spell q3Spell;

        public QDrawing(Q1Spell spell, Q3Spell q3Spell)
        {
            this.spell = spell;
            this.q3Spell = q3Spell;
        }

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (Menu.Item("Q3").GetValue<bool>() && q3Spell.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position,
                q3Spell.Spell.Range
               , q3Spell.Spell.IsReady()
               ? Color.Cyan
               : Color.DarkSlateGray);
            }
            else
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position,
                   spell.Spell.Range
                  , spell.Spell.IsReady()
                  ? Color.Cyan
                  : Color.DarkSlateGray);
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

            Menu.AddItem(new MenuItem("Q3", "Draw Whirlwind Range").SetValue(true));
        }
    }
}
