using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Xerath.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Xerath.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        public RKillsteal(RSpell spell)
        {
            this.spell = spell;
        }

        private readonly Random rand = new Random();

        private AIHeroClient Target => TargetSelector.GetTarget(spell.RealRange, TargetSelector.DamageType.Magical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || (Target.Health > spell.GetDamage(Target) && !spell.IsCasting)
                || (!spell.IsCasting && Target.Distance(ObjectManager.Player) < 1000)
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

            switch (Menu.Item("Xerath.Killsteal.R.Type").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    Cast();
                    break;
                case 1:
                    break;
            }
        }

        private void Cast()
        {
            switch (Menu.Item("Xerath.Killsteal.R.Prediction").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (spell.SDK(Target).Hitchance >= SebbyLib.Movement.HitChance.High)
                    {
                        if (Menu.Item("Xerath.Killsteal.R.Mouse").GetValue<bool>()
                            && spell.SDK(Target).CastPosition.Distance(Game.CursorPos) > 600)
                        {
                            return;
                        }
                        spell.Spell.Cast(spell.SDK(Target).CastPosition);
                    }
                    break;
                case 1:
                    if (spell.OKTW(Target).Hitchance >= SebbyLib.Prediction.HitChance.High)
                    {
                        if (Menu.Item("Xerath.Killsteal.R.Mouse").GetValue<bool>()
                              && spell.OKTW(Target).CastPosition.Distance(Game.CursorPos) > 600)
                        {
                            return;
                        }
                        spell.Spell.Cast(spell.OKTW(Target).CastPosition);
                    }
                    break;
                case 2:
                    if (spell.Prediction(Target).Hitchance >= HitChance.High)
                    {
                        if (Menu.Item("Xerath.Killsteal.R.Mouse").GetValue<bool>()
                                && spell.Prediction(Target).CastPosition.Distance(Game.CursorPos) > 600)
                        {
                            return;
                        }
                        spell.Spell.Cast(spell.Prediction(Target).CastPosition);
                    }
                    break;
            }
        }

        private int lastPing;

        private void Ping() 
        {
            if (Utils.TickCount - lastPing < 25000)
            {
                return;
            }

            lastPing = Utils.TickCount;

            LeagueSharp.Common.Utility.DelayAction.Add(rand.Next(50, 100), () => TacticalMap.ShowPing(PingCategory.Danger, Target, true));

            LeagueSharp.Common.Utility.DelayAction.Add(rand.Next(200, 300), () => TacticalMap.ShowPing(PingCategory.OnMyWay, Target, true));

            LeagueSharp.Common.Utility.DelayAction.Add(rand.Next(400, 900), () => TacticalMap.ShowPing(PingCategory.AssistMe, Target, true));

            LeagueSharp.Common.Utility.DelayAction.Add(rand.Next(1000, 1300), () => TacticalMap.ShowPing(PingCategory.OnMyWay, Target, true));
        }

        private void OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (spell.IsCasting && Menu.Item("Xerath.Killsteal.R.Move").GetValue<bool>())
            {
                args.Process = false;
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            EloBuddy.Player.OnIssueOrder -= OnIssueOrder;

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

            Menu.AddItem(new MenuItem("Xerath.Killsteal.R.Move", "Disable Movement In R").SetValue(false));

            Menu.AddItem(new MenuItem("Xerath.Killsteal.R.Ping", "Ping Locally On Killable").SetValue(true));

            Menu.AddItem(new MenuItem("Xerath.Killsteal.R.Auto", "Automatically Use R1").SetValue(false));

            Menu.AddItem(new MenuItem("Xerath.Killsteal.R.Turret", "Turret Check").SetValue(true));

            Menu.AddItem(new MenuItem("Xerath.Killsteal.R.Mouse", "Only Ult Near Mouse").SetValue(false));

            Menu.AddItem(new MenuItem("Xerath.Killsteal.R.Type", "Type: ").SetValue(new StringList(new[] { "Auto", "Manual" })));

            Menu.AddItem(new MenuItem("Xerath.Killsteal.R.Prediction", "Prediction: ").SetValue(new StringList(new[] { "SDK", "OKTW", "Common" })));

            Menu.AddItem(new MenuItem("Xerath.Killsteal.R.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
