using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.Drawings.Damage
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Damage;
    using ReformedAIO.Champions.Lee_Sin.Core.Spells;
    using ReformedAIO.Library.Drawings;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    internal sealed class DamageDrawing : ChildBase
    {
        public override string Name { get; set; } = "Damage";

        public readonly LeeSinStatistisks Statistisks;

        private readonly QSpell spell;

        public DamageDrawing(LeeSinStatistisks statistisks, QSpell spell)
        {
            this.Statistisks = statistisks;
            this.spell = spell;
        }

        private HeroHealthBarIndicator heroHealthBarIndicator;

     
        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1200)))
            {
                heroHealthBarIndicator.Unit = enemy;

                if (Menu.Item("LeeSin.Drawing.Damage.Q").GetValue<bool>() && spell.Spell.IsReady())
                {
                    var dmg = spell.GetDamage(enemy);

                    if (spell.Smite.IsReady())
                    {
                        dmg += spell.SmiteTargetableDamage();
                    }

                    heroHealthBarIndicator.DrawDmg(dmg,
                                             enemy.Health <= dmg
                                             ? Color.DarkSlateGray
                                             : Color.Green);
                }

                heroHealthBarIndicator.DrawDmg(Statistisks.GetComboDamage(enemy),
                                               enemy.Health <= Statistisks.GetComboDamage(enemy) * .8
                                               ? Color.LawnGreen
                                               : Color.Yellow);
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

            Menu.AddItem(new MenuItem("LeeSin.Drawing.Damage.Q", "Draw Q Damage").SetValue(false));

            heroHealthBarIndicator = new HeroHealthBarIndicator();
        }
    }
}
