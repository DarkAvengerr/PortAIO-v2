using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.Drawings
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class WDraw : ChildBase
    {
        public override string Name { get; set; } = "[W]";

        public void OnDraw(EventArgs args)
        {
            if (Vars.Player.IsDead) return;

            if (Menu.Item("WReady").GetValue<bool>() && !Spells.Spell[SpellSlot.W].IsReady())
            {
                return;
            }

            Render.Circle.DrawCircle(
                 Vars.Player.Position,
                Spells.Spell[SpellSlot.W].Range,
                Spells.Spell[SpellSlot.W].IsReady()
                ? Color.LightSlateGray
                : Color.DarkSlateGray
                , Vars.Player.GetSpell(SpellSlot.W).Ammo); // hehe xd
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(new MenuItem("WReady", "Only If Ready").SetValue(false));
        }
    }
}
