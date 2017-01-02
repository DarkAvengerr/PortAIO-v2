using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Xerath.Drawings.Spells
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Xerath.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using HitChance = SebbyLib.Movement.HitChance;

    internal sealed class QDrawing : ChildBase
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QDrawing(QSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Magical);

        private Color GetColor()
        {
            if (spell.SDK(Target).Hitchance >= HitChance.VeryHigh)
            {
                return Color.Green;
            }

            if (spell.SDK(Target).Hitchance >= HitChance.High)
            {
                return Color.Yellow;
            }

            return spell.SDK(Target).Hitchance <= HitChance.Medium ? Color.Red : new Color();
        }

        private void OnEndScene(EventArgs args)
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
                                     Menu.Item("Xerath.Draw.Q.Width").GetValue<Slider>().Value,
                                     Menu.Item("Xerath.Draw.Q.Z").GetValue<bool>());

            if (Target == null || Menu.Item("Xerath.Draw.Q.Prediction").GetValue<StringList>().SelectedIndex == 3 || !spell.Charging)
            {
                return;
            }

            switch (Menu.Item("Xerath.Draw.Q.Prediction").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    Drawing.DrawLine(Drawing.WorldToScreen(ObjectManager.Player.Position), Drawing.WorldToScreen(spell.SDK(Target).CastPosition), 5, GetColor());
                    break;
                case 1:
                    Drawing.DrawLine(Drawing.WorldToScreen(ObjectManager.Player.Position), Drawing.WorldToScreen(spell.OKTW(Target).CastPosition), 5, GetColor());
                    break;
                case 2:
                    Drawing.DrawLine(Drawing.WorldToScreen(ObjectManager.Player.Position), Drawing.WorldToScreen(Target.Position), 2, Color.AliceBlue);
                    break;
            }
        }


        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Drawing.OnEndScene -= OnEndScene;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);


            Drawing.OnEndScene += OnEndScene;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Xerath.Draw.Q.Prediction", "Draw Prediction").SetValue(new StringList(new [] { "SDK", "OKTW", "Target Position", "Don't Draw" }, 3)));

            Menu.AddItem(new MenuItem("Xerath.Draw.Q.Z", "Draw Z").SetValue(false));

            Menu.AddItem(new MenuItem("Xerath.Draw.Q.Width", "Thickness").SetValue(new Slider(3, 1, 5)));
        }
    }
}
