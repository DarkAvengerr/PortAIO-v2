using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ashe.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class RCombo : ChildBase
    {
        #region Fields

        private RLogic rLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[R]";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Interrupter2.OnInterruptableTarget -= Interrupt;
            AntiGapcloser.OnEnemyGapcloser -= Gapcloser;
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Interrupter2.OnInterruptableTarget += Interrupt;
            AntiGapcloser.OnEnemyGapcloser += Gapcloser;
            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(
                new MenuItem("RDistance", "Max Distance").SetValue(new Slider(1100, 0, 1500))
                    .SetTooltip("Too Much And You Might Not Get The Kill"));

            Menu.AddItem(new MenuItem("RMana", "Mana %").SetValue(new Slider(10, 0, 100)));

            //Menu.AddItem(
            //    new MenuItem("SemiR", "Semi-Auto R Key").SetValue(new KeyBind('A', KeyBindType.Press))
            //        .SetTooltip("Select Your Target First"));

            Menu.AddItem(new MenuItem("RKillable", "Only When Killable").SetValue(true));

            Menu.AddItem(new MenuItem("RSafety", "Safety Check").SetValue(true));

            Menu.AddItem(new MenuItem("Interrupt", "Interrupt").SetValue(true));

            Menu.AddItem(new MenuItem("Gapclose", "Gapcloser").SetValue(true));

            rLogic = new RLogic();
        }

        private void CrystalArrow()
        {
            var target = TargetSelector.GetTarget(
                Menu.Item("RDistance").GetValue<Slider>().Value,
                TargetSelector.DamageType.Physical,
                false);

            if (target == null 
                || !target.IsValid
                || target.IsInvulnerable 
                || target.IsDashing() 
                || (Menu.Item("RSafety").GetValue<bool>() && !rLogic.SafeR(target))
                || (Menu.Item("RKillable").GetValue<bool>() && !rLogic.Killable(target))) return;

            Variable.Spells[SpellSlot.R].CastIfHitchanceEquals(target, HitChance.High);
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("Gapclose").GetValue<bool>() || !Variable.Spells[SpellSlot.R].IsReady()) return;

            var target = gapcloser.Sender;

            if (target == null || !target.IsEnemy || !target.IsValidTarget(500)) return;

            Variable.Spells[SpellSlot.R].Cast(gapcloser.End);
        }

        private void Interrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Menu.Item("Interrupt").GetValue<bool>()
                || !sender.IsEnemy 
                || !Variable.Spells[SpellSlot.R].IsReady() 
                || args.DangerLevel < Interrupter2.DangerLevel.High) return;

            if (sender.IsValidTarget(1200)) Variable.Spells[SpellSlot.R].CastIfHitchanceEquals(sender, HitChance.VeryHigh);
        }

        private void OnUpdate(EventArgs args)
        {
           // SemiR();

            if (Menu.Item("RMana").GetValue<Slider>().Value > Variable.Player.ManaPercent || !Variable.Spells[SpellSlot.R].IsReady())
            {
                return;
            }

            CrystalArrow();
        }

        //private void SemiR()
        //{
        //    if (!Menu.Item("SemiR").GetValue<KeyBind>().Active) return;

        //    var target = TargetSelector.GetSelectedTarget();
        //    if (target == null) return;

        //    var pred = Variable.Spells[SpellSlot.R].GetPrediction(target);
        //    if (pred.Hitchance >= HitChance.High)
        //    {
        //        Variable.Spells[SpellSlot.R].Cast(pred.CastPosition);
        //    }
        //}

        #endregion
    }
}