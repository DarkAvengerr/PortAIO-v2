using EloBuddy; namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    using SPrediction;

    using Color = System.Drawing.Color;

    #endregion

    internal class RCombo : ChildBase
    {
        #region Fields

        private QLogic qLogic;

        private RLogic rLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[R] Explosive Cask";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw -= this.OnDraw;
            //  Obj_AI_Base.OnProcessSpellCast -= OnProcessSpellCast;
            Events.OnUpdate -= this.OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw += this.OnDraw;
            //   Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Events.OnUpdate += this.OnUpdate;
        }

        protected override void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.qLogic = new QLogic();
            this.rLogic = new RLogic();
            base.OnInitialize(sender, featureBaseEventArgs);
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "InsecTo", "Insec To").SetValue(
                    new StringList(new[] { "Ally / Turret", " Player", " Cursor" })));

            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "AllyRange", "Range To Find Allies").SetValue(new Slider(1500, 0, 2400)));

            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "TurretRange", "Range To Find Turret").SetValue(new Slider(1300, 0, 1600)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "RRange", "R Range ").SetValue(new Slider(950, 0, 1050)));

            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "RRangePred", "Range Behind Target").SetValue(new Slider(150, 0, 185)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "RMana", "Mana %").SetValue(new Slider(45, 0, 100)));

            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "QRQ", "Use Q?").SetValue(true).SetTooltip("Will do QRQ insec (BETA)"));

            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "QRQDistance", "Max Distance For QRQ Combo").SetValue(
                    new Slider(725, 0, 800)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "RDraw", "Draw R Prediction").SetValue(false));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "Enabled", "Enabled").SetValue(false));
        }

        // Need to fix this to make better QRQ Combo
        //private void OnProcessSpellCast(GameObject sender, GameObjectProcessSpellCastEventArgs args)
        //{
        //    if (!Menu.Item(Menu.Name + "QRQ").GetValue<bool>() || !Variable.Spells[SpellSlot.Q].LSIsReady() ||
        //        Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo) return;

        //    var target = args.Target as AIHeroClient;
        //    // args.SData.Name == "Gragas_Base_Q_Ally.troy"

        //    if (target == null || !target.LSIsValidTarget(1150) || !sender.IsMe) return;

        //    var pred = LeagueSharp.Common.Prediction.GetPrediction(target, Variable.Spells[SpellSlot.R].Delay
        //        + Variable.Player.Position.LSDistance(args.End) / Variable.Spells[SpellSlot.R].Speed).CastPosition;

        //    Variable.Spells[SpellSlot.Q].Cast(args.End.LSExtend(pred, Variable.Spells[SpellSlot.R].Width));
        //}

        private void ExplosiveCask()
        {
            var target = TargetSelector.GetSelectedTarget();

            if (target == null || !target.LSIsValidTarget() || target.LSIsDashing()) return;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);

            if (this.Menu.Item(this.Menu.Name + "QRQ").GetValue<bool>() && Variable.Spells[SpellSlot.Q].LSIsReady()
                && this.Menu.Item(this.Menu.Name + "QRQDistance").GetValue<Slider>().Value
                >= target.LSDistance(Variable.Player))
            {
                Variable.Spells[SpellSlot.Q].Cast(this.InsecQ(target));
            }

            Variable.Spells[SpellSlot.R].Cast(this.InsecTo(target));
        }

        // Hotfix..!
        private Vector3 InsecQ(AIHeroClient target)
        {
            var rPred = this.rLogic.RPred(target)
                .LSExtend(
                    Variable.Player.Position,
                    Variable.Spells[SpellSlot.R].Width
                    - this.Menu.Item(this.Menu.Name + "RRangePred").GetValue<Slider>().Value);

            return rPred;
        }

        private Vector3 InsecTo(AIHeroClient target)
        {
            var mePos = Variable.Player.Position;
            // Doing this we can extend to our own position if we can't access anything else (Tower, Ally)

            switch (this.Menu.Item(this.Menu.Name + "InsecTo").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    var ally =
                        HeroManager.Allies.Where(
                            x =>
                            x.LSIsValidTarget(
                                this.Menu.Item(this.Menu.Name + "AllyRange").GetValue<Slider>().Value,
                                false,
                                target.ServerPosition) && x.LSDistance(target) > 325 && !x.IsMe && x.IsAlly)
                            .MaxOrDefault(
                                x =>
                                x.LSCountAlliesInRange(
                                    this.Menu.Item(this.Menu.Name + "AllyRange").GetValue<Slider>().Value));

                    if (ally != null)
                    {
                        mePos = ally.ServerPosition;
                    }

                    var turret =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .Where(
                                x =>
                                x.IsAlly && x.LSDistance(target) > 325
                                && x.LSDistance(target)
                                < this.Menu.Item(this.Menu.Name + "TurretRange").GetValue<Slider>().Value && !x.IsEnemy)
                            .OrderBy(x => x.LSDistance(Variable.Player.Position))
                            .FirstOrDefault();

                    if (turret != null)
                    {
                        mePos = turret.ServerPosition;
                    }

                    break;
                case 1:
                    mePos = mePos; // Kappa just because i can
                    break;
                case 2:
                    mePos = Game.CursorPos;
                    break;
            }

            var pos =
                Variable.Spells[SpellSlot.R].GetVectorSPrediction(target, 980)
                    .CastTargetPosition.LSExtend(
                        mePos.LSTo2D(),
                        -this.Menu.Item(this.Menu.Name + "RRangePred").GetValue<Slider>().Value);

            return pos.To3D();
        }

        private void OnDraw(EventArgs args)
        {
            if (Variable.Player.IsDead || !this.Menu.Item(this.Menu.Name + "RDraw").GetValue<bool>()) return;

            var target = TargetSelector.GetSelectedTarget();

            if (target == null || !target.IsValid) return;

            Render.Circle.DrawCircle(this.InsecTo(target), 100, Color.Cyan);

            if (this.Menu.Item(this.Menu.Name + "QRQ").GetValue<bool>())
            {
                Render.Circle.DrawCircle(this.InsecQ(target), 60, Color.Cyan);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !Variable.Spells[SpellSlot.R].LSIsReady()
                || this.Menu.Item(this.Menu.Name + "RMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            this.ExplosiveCask();
        }

        #endregion
    }
}