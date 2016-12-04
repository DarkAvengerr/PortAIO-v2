using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.Drawings.SpellDrawings
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class EDrawing : ChildBase
    {
        public override string Name { get; set; } = "E";

        private DashPosition dashPos;

        private readonly ESpell spell;

        public EDrawing(ESpell spell)
        {
            this.spell = spell;
        }

        private Obj_AI_Base Minion
            =>
                MinionManager.GetMinions(ObjectManager.Player.Position, spell.Spell.Range)
                    .LastOrDefault(m => m.Distance(Game.CursorPos) <= 400 && !m.HasBuff("YasuoDashWrapper"));

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || Minion == null)
            {
                return;
            }

            Render.Circle.DrawCircle(
                Minion.Position,
                Minion.BoundingRadius,
                spell.Spell.IsReady() ? Color.Cyan : Color.DarkSlateGray);

            if (Menu.Item("Path").GetValue<bool>())
            {
                Render.Circle.DrawCircle(
                dashPos.DashEndPosition(Minion, 475),
                25,
                spell.Spell.IsReady() 
                ? Color.White
                : Color.DarkSlateGray);

                Drawing.DrawLine(Drawing.WorldToScreen(Minion.Position),Drawing.WorldToScreen(dashPos.DashEndPosition(Minion, 475)), 2, Color.White);
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

            dashPos = new DashPosition();

            Menu.AddItem(new MenuItem("Path", "Path").SetValue(true));
        }
    }
}