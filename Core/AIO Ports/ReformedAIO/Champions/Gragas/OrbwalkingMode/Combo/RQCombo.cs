using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using HitChance = SebbyLib.Prediction.HitChance;

    internal sealed class RQCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q & R";

        private readonly QSpell qSpell;

        private readonly RSpell rSpell;

        public RQCombo(QSpell qSpell, RSpell rSpell)
        {
            this.qSpell = qSpell;
            this.rSpell = rSpell;
        }

        private AIHeroClient Target => TargetSelector.GetSelectedTarget();

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Target == null || Target.Distance(ObjectManager.Player) > 800)
            {
                return;
            }

            qSpell.ExplodeHandler(Target);

            if (rSpell.Spell.IsReady())
            {
                Insec();
            }
           else if (!qSpell.HasThrown && qSpell.Spell.IsReady())
            {
                Combo();
            }
        }

        private void Combo()
        {
            switch (Menu.Item("Gragas.Combo.RQ.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (qSpell.OKTW(Target).Hitchance >= HitChance.High)
                    {
                        qSpell.Handle(qSpell.OKTW(Target).CastPosition);
                    }
                    break;
                case 1:
                    if (qSpell.OKTW(Target).Hitchance >= HitChance.VeryHigh)
                    {
                        qSpell.Handle(qSpell.OKTW(Target).CastPosition);
                    }
                    break;
            }
        }

        private void Insec()
        {
            var Position = rSpell.InsecPositioner(
                Target,
                Menu.Item("Gragas.Combo.RQ.Turret").GetValue<bool>(),
                Menu.Item("Gragas.Combo.RQ.Ally").GetValue<bool>());

            rSpell.Spell.Cast(Position);

            if (Menu.Item("Gragas.Combo.RQ.Q").GetValue<bool>() && !qSpell.HasThrown && qSpell.Spell.IsReady())
            {
                qSpell.Handle(Position.Extend(ObjectManager.Player.Position, 840));
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

            Menu.AddItem(new MenuItem("Gragas.Combo.RQ.Draw", "Draw").SetValue(false));

            Menu.AddItem(new MenuItem("Gragas.Combo.RQ.Hitchance", "Hitchance:").SetValue(new StringList(new[] { "High", "Very High" })));

            Menu.AddItem(new MenuItem("Gragas.Combo.RQ.Q", "Insec To: Q Barrel").SetValue(true));

            Menu.AddItem(new MenuItem("Gragas.Combo.RQ.Turret", "Insec To: Turret").SetValue(true));

            Menu.AddItem(new MenuItem("Gragas.Combo.RQ.Ally", "Insec To: Ally").SetValue(true));

            Menu.AddItem(new MenuItem("Gragas.Combo.RQ.Gragas", "Insec To: Gragas").SetValue(false));
        }
    }
}
