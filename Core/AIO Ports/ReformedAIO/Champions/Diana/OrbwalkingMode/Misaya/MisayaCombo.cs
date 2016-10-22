using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Misaya
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class MisayaCombo : ChildBase
    {
        #region Fields

        private LogicAll logic;
        private Orbwalking.Orbwalker orbwalker;
        private CrescentStrikeLogic qLogic;

        public MisayaCombo(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "Misaya";

        #endregion

        #region Public Methods and Operators

        public void OnUpdate(EventArgs args)
        {
            if (!Menu.Item(Menu.Name + "Keybind").GetValue<KeyBind>().Active) return;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (Variables.Spells[SpellSlot.R].IsReady() || Variables.Spells[SpellSlot.Q].IsReady())
            {
                PaleCascade();
            }

            if (Variables.Spells[SpellSlot.E].IsReady())
            {
                MoonFall();
            }

            if (Variables.Spells[SpellSlot.W].IsReady())
            {
                LunarRush();
            }
        }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }

        //protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        //{
        //    logic = new LogicAll();
        //    qLogic = new CrescentStrikeLogic();
        //    base.OnLoad(sender, featureBaseEventArgs);
        //}

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(
                new MenuItem(Menu.Name + "Keybind", "Keybind").SetValue(new KeyBind('Z', KeyBindType.Press)));

            Menu.AddItem(new MenuItem(Menu.Name + "Range", "Range ").SetValue(new Slider(825, 0, 825)));

            Menu.AddItem(new MenuItem(Menu.Name + "UseW", "Use W").SetValue(true));

            Menu.AddItem(new MenuItem(Menu.Name + "UseE", "Use E").SetValue(true));

            Menu.AddItem(new MenuItem(Menu.Name + "ERange", "E Range").SetValue(new Slider(330, 0, 350)));

            Menu.AddItem(new MenuItem(Menu.Name + "EKillable", "Only E If Killable").SetValue(true));

            logic = new LogicAll();
            qLogic = new CrescentStrikeLogic();
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
                Menu.Item(Menu.Name + "ERange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            if (Menu.Item(Menu.Name + "EKillable").GetValue<bool>()
                && logic.ComboDmg(target) * 1.3 < target.Health) return;

            Variables.Spells[SpellSlot.E].Cast();
        }

        private void PaleCascade()
        {
            var target = TargetSelector.GetTarget(
                Menu.Item(Menu.Name + "Range").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            if (Variables.Spells[SpellSlot.Q].IsReady() && Variables.Spells[SpellSlot.R].IsReady()
                && target.Distance(Variables.Player) >= 500)
            {
                Variables.Spells[SpellSlot.R].Cast(target);
            }

            Variables.Spells[SpellSlot.Q].Cast(qLogic.QPred(target));
        }

        #endregion
    }
}