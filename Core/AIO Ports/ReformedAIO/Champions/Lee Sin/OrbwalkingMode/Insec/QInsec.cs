using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Insec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using HitChance = SebbyLib.Movement.HitChance;

    internal sealed class QInsec : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QInsec(QSpell spell)
        {
            this.spell = spell;
        }

        private static AIHeroClient Target => TargetSelector.GetSelectedTarget();

        private static AIHeroClient Target2 => TargetSelector.GetTarget(1300, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Target == null || Target.Distance(ObjectManager.Player) > 1800)
            {
                return;
            }
            
            if (Target.Distance(ObjectManager.Player) < spell.Spell.Range)
            {
                Cast();
            }
           else if (Menu.Item("LeeSin.Insec.Q.Minion").GetValue<bool>())
           {
                var m = MinionManager.GetMinions(Target.Position, 1200).FirstOrDefault(x => x.Distance(Target) < 500);

                if (m == null)
                {
                    return;
                }

                if (spell.IsQ1 && m.Health > spell.GetDamage(m))
                {
                    spell.Spell.Cast(m);
                }

                if (spell.HasQ2(m))
                {
                    spell.Spell.Cast();
                }
            }
            else if (Menu.Item("LeeSin.Insec.Q.Target").GetValue<bool>() && Target2 != null && Target2.Distance(Target) < 700)
            {
                Cast2();
            }
        }

        private void Cast()
        {
            if (spell.IsQ1)
            {
                spell.SmiteCollision(Target);

                var prediction = spell.Prediction(Target);

                spell.SmiteCollision(Target);

                switch (Menu.Item("LeeSin.Insec.Q.Hitchance").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            spell.Spell.Cast(prediction.CastPosition);
                        }
                        break;
                    case 1:
                        if (prediction.Hitchance >= HitChance.VeryHigh)
                        {
                            spell.Spell.Cast(prediction.CastPosition);
                        }
                        break;
                }
            }
            else
            {
                spell.Spell.Cast();
            }
        }

        private void Cast2()
        {
            if (spell.IsQ1)
            {
                var prediction = spell.Prediction(Target2);

                spell.Spell.Cast(prediction.CastPosition);
            }
            else
            {
                spell.Spell.Cast();
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("LeeSin.Insec.Q.Target", "Q To Targets (Gapclose)").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Insec.Q.Minion", "Q To Minions").SetValue(true));
            
            Menu.AddItem(new MenuItem("LeeSin.Insec.Q.Hitchance", "Hitchance:").SetValue(new StringList(new[] { "High", "Very High" })));
        }
    }
}
