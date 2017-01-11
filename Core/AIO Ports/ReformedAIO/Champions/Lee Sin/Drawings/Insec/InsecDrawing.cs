    using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.Drawings.Insec
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal sealed class InsecDrawing : OrbwalkingChild
    {
        public override string Name { get; set; } = "Insec (DEBUG!!!)";

        private readonly WSpell wSpell;
        private readonly RSpell rSpell;

        public InsecDrawing(WSpell wSpell, RSpell rSpell)
        {
            this.wSpell = wSpell;
            this.rSpell = rSpell;
        }

        private static AIHeroClient Target => TargetSelector.GetSelectedTarget();

        private static AIHeroClient Target2 => HeroManager.Enemies.Where(x => x != Target && x.Distance(Target) < 1400 && x.Distance(ObjectManager.Player) < 750).OrderBy(x => x.Distance(x)).FirstOrDefault();

        private Vector3 BubbaKush()
        {
            return rSpell.MultipleInsec(Target, Target2);
        }

        private Vector3 InsecSolo()
        {
            return wSpell.InsecPositioner(
                Target,
                true, true);
        }

        private Vector3 GetInsecPosition()
        {
            return Target2 != null ? this.BubbaKush() : InsecSolo();
        }

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead
                || Target == null
                || !rSpell.Spell.IsReady()
                || Target.Distance(ObjectManager.Player) > 2000)
            {
                return;
            }

            if (!wSpell.Spell.IsReady())
            {
                return;
            }

            Render.Circle.DrawCircle(GetInsecPosition(), 65, Color.Cyan);

            Drawing.DrawLine(Drawing.WorldToScreen(Target.Position), Drawing.WorldToScreen(GetInsecPosition()), 1, Color.White);

            Render.Circle.DrawCircle(Target.Position + (Target.Position - GetInsecPosition()).Normalized() * 1200, 75, Color.White);
            Drawing.DrawLine(Drawing.WorldToScreen(Target.Position + (Target.Position - GetInsecPosition()).Normalized() * 1200), Drawing.WorldToScreen(GetInsecPosition()), 1, Color.White);
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

            Menu.AddItem(new MenuItem("LeeSin.Drawing...", "Draws BEST possible prediction"));
        }
    }
}