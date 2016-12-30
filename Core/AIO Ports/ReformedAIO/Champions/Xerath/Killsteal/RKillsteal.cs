using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Xerath.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Xerath.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using SebbyLib;

    internal sealed class RKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        public RKillsteal(RSpell spell)
        {
            this.spell = spell;
        }

        private Random rand = new Random();

        private AIHeroClient Target => TargetSelector.GetTarget(spell.RealRange, TargetSelector.DamageType.Magical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || (Target.Health > spell.GetDamage(Target) && !spell.IsCasting)
                || (!spell.IsCasting && Target.Distance(ObjectManager.Player) < 800)
                || (Menu.Item("Xerath.Killsteal.R.Turret").GetValue<bool>() && ObjectManager.Player.UnderTurret(true) && !spell.IsCasting)
                || Menu.Item("Xerath.Killsteal.R.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            if (Menu.Item("Xerath.Killsteal.R.Ping").GetValue<bool>())
            {
                Ping();
            }

            if (!spell.IsCasting && !Menu.Item("Xerath.Killsteal.R.Auto").GetValue<bool>())
            {
               return;
            }

            Cast();
        }

        private void Cast()
        {
            switch (Menu.Item("Xerath.Killsteal.R.Prediction").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (spell.SDK(Target) != null)
                    {
                        spell.Spell.Cast(spell.SDK(Target).CastPosition);
                    }
                    break;
                case 1:
                    if (spell.OKTW(Target) != null)
                    {
                        spell.Spell.Cast(spell.OKTW(Target).CastPosition);
                    }
                    break;
                case 2:
                    spell.Spell.Cast(Target.Position);
                    break;
            }
        }

        private int lastPing;

        private void Ping()
        {
            if (Utils.TickCount - lastPing < 30000)
            {
                return;
            }

            lastPing = Utils.TickCount;

            LeagueSharp.Common.Utility.DelayAction.Add(rand.Next(100, 200), () => TacticalMap.ShowPing(PingCategory.OnMyWay, Target, true));

            LeagueSharp.Common.Utility.DelayAction.Add(rand.Next(200, 450), () => TacticalMap.ShowPing(PingCategory.AssistMe, Target, true));
        }

        private void OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (spell.IsCasting)
            {
                args.Process = false;
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

            EloBuddy.Player.OnIssueOrder += OnIssueOrder;

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Xerath.Killsteal.R.Ping", "Ping Locally On Killable").SetValue(true));

            Menu.AddItem(new MenuItem("Xerath.Killsteal.R.Auto", "Automatically Use R1").SetValue(false));

            Menu.AddItem(new MenuItem("Xerath.Killsteal.R.Prediction", "Prediction: ").SetValue(new StringList(new[] { "SDK", "OKTW", "Position" })));

            Menu.AddItem(new MenuItem("Xerath.Killsteal.R.Turret", "Turret Check").SetValue(true));

            Menu.AddItem(new MenuItem("Xerath.Killsteal.R.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
