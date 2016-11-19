using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell wSpell;

        public WCombo(WSpell wSpell)
        {
            this.wSpell = wSpell;
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians())
            {
                return;
            }

            var target = TargetSelector.GetTarget(wSpell.Spell.Range, TargetSelector.DamageType.Physical);

            if (target == null
                || ObjectManager.Player.Distance(target) <= ObjectManager.Player.AttackRange + 75
                || Menu.Item("WMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var wPred = wSpell.Spell.GetPrediction(target);

            if (wPred.Hitchance >= HitChance.Medium)
            {
                wSpell.Spell.Cast(wPred.CastPosition);
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe
                || !Orbwalking.IsAutoAttack(args.SData.Name)
                || !CheckGuardians()
                || Menu.Item("WMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)));

            foreach (var target in heroes as AIHeroClient[] ?? heroes.ToArray())
            {
                if (Menu.Item("WPred").GetValue<bool>())
                {
                    wSpell.Spell.Cast(target.Position);
                }
                else
                {
                    var wPred = wSpell.Spell.GetPrediction(target);

                    if (wPred.Hitchance > HitChance.Medium)
                    {
                        wSpell.Spell.Cast(wPred.CastPosition);
                    }
                }
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("WPred", "Disable Prediction").SetValue(true));
            Menu.AddItem(new MenuItem("WMana", "Min Mana %").SetValue(new Slider(20, 0, 100)));

        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
            Obj_AI_Base.OnSpellCast -= OnSpellCast;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }
    }
}
