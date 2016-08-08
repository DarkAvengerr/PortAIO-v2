using EloBuddy; namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Misaya
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class MisayaCombo : ChildBase
    {
        #region Fields

        private LogicAll logic;

        private CrescentStrikeLogic qLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "Misaya";

        #endregion

        #region Public Methods and Operators

        public void OnUpdate(EventArgs args)
        {
            if (!this.Menu.Item(this.Menu.Name + "Keybind").GetValue<KeyBind>().Active) return;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (Variables.Spells[SpellSlot.R].LSIsReady() || Variables.Spells[SpellSlot.Q].LSIsReady())
            {
                this.PaleCascade();
            }

            if (Variables.Spells[SpellSlot.E].LSIsReady())
            {
                this.MoonFall();
            }

            if (Variables.Spells[SpellSlot.W].LSIsReady())
            {
                this.LunarRush();
            }
        }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate -= this.OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate += this.OnUpdate;
        }

        protected override void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.logic = new LogicAll();
            this.qLogic = new CrescentStrikeLogic();
            base.OnInitialize(sender, featureBaseEventArgs);
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "Keybind", "Keybind").SetValue(new KeyBind('Z', KeyBindType.Press)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "Range", "Range ").SetValue(new Slider(825, 0, 825)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "UseW", "Use W").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "UseE", "Use E").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "ERange", "E Range").SetValue(new Slider(330, 0, 350)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "EKillable", "Only E If Killable").SetValue(true));
        }

        private void LunarRush()
        {
            var target = TargetSelector.GetTarget(
                Variables.Player.AttackRange + Variables.Player.BoundingRadius,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            Variables.Spells[SpellSlot.W].Cast();
        }

        private void MoonFall()
        {
            var target = TargetSelector.GetTarget(
                this.Menu.Item(this.Menu.Name + "ERange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            if (this.Menu.Item(this.Menu.Name + "EKillable").GetValue<bool>()
                && this.logic.ComboDmg(target) * 1.3 < target.Health) return;

            Variables.Spells[SpellSlot.E].Cast();
        }

        private void PaleCascade()
        {
            var target = TargetSelector.GetTarget(
                this.Menu.Item(this.Menu.Name + "Range").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            if (Variables.Spells[SpellSlot.Q].LSIsReady() && Variables.Spells[SpellSlot.R].LSIsReady()
                && target.LSDistance(Variables.Player) >= 500)
            {
                Variables.Spells[SpellSlot.R].Cast(target);
            }

            Variables.Spells[SpellSlot.Q].Cast(this.qLogic.QPred(target));
        }

        #endregion
    }
}