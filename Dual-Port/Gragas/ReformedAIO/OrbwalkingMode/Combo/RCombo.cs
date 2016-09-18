using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;

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
        private readonly Orbwalking.Orbwalker orbwalker;

        public RCombo(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }
        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw -= OnDraw;
            //  Obj_AI_Base.OnProcessSpellCast -= OnProcessSpellCast;
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw += OnDraw;
            //   Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
        }

        //protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        //{
        //    qLogic = new QLogic();
        //    rLogic = new RLogic();
        //    base.OnLoad(sender, featureBaseEventArgs);
        //}

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(
                new MenuItem(Menu.Name + "InsecTo", "Insec To").SetValue(
                    new StringList(new[] { "Ally / Turret", " Player", " Cursor" })));

            Menu.AddItem(
                new MenuItem(Menu.Name + "AllyRange", "Range To Find Allies").SetValue(new Slider(1500, 0, 2400)));

            Menu.AddItem(
                new MenuItem(Menu.Name + "TurretRange", "Range To Find Turret").SetValue(new Slider(1300, 0, 1600)));

            Menu.AddItem(new MenuItem(Menu.Name + "RRange", "R Range ").SetValue(new Slider(950, 0, 1050)));

            Menu.AddItem(
                new MenuItem(Menu.Name + "RRangePred", "Range Behind Target").SetValue(new Slider(150, 0, 185)));

            Menu.AddItem(new MenuItem(Menu.Name + "RMana", "Mana %").SetValue(new Slider(45, 0, 100)));

            Menu.AddItem(
                new MenuItem(Menu.Name + "QRQ", "Use Q?").SetValue(true).SetTooltip("Will do QRQ insec (BETA)"));

            Menu.AddItem(
                new MenuItem(Menu.Name + "QRQDistance", "Max Distance For QRQ Combo").SetValue(
                    new Slider(725, 0, 800)));

            Menu.AddItem(new MenuItem(Menu.Name + "RDraw", "Draw R Prediction").SetValue(false));

            Menu.AddItem(new MenuItem(Menu.Name + "Enabled", "Enabled").SetValue(false));

            qLogic = new QLogic();
            rLogic = new RLogic();
        }

        // Need to fix this to make better QRQ Combo
        //private void OnProcessSpellCast(GameObject sender, GameObjectProcessSpellCastEventArgs args)
        //{
        //    if (!Menu.Item(Menu.Name + "QRQ").GetValue<bool>() || !Variable.Spells[SpellSlot.Q].IsReady() ||
        //        Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo) return;

        //    var target = args.Target as AIHeroClient;
        //    // args.SData.Name == "Gragas_Base_Q_Ally.troy"

        //    if (target == null || !target.IsValidTarget(1150) || !sender.IsMe) return;

        //    var pred = LeagueSharp.Common.Prediction.GetPrediction(target, Variable.Spells[SpellSlot.R].Delay
        //        + Variable.Player.Position.Distance(args.End) / Variable.Spells[SpellSlot.R].Speed).CastPosition;

        //    Variable.Spells[SpellSlot.Q].Cast(args.End.Extend(pred, Variable.Spells[SpellSlot.R].Width));
        //}

        private void ExplosiveCask()
        {
            var target = TargetSelector.GetSelectedTarget();

            if (target == null || !target.IsValidTarget() || target.IsDashing()) return;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);

            if (Menu.Item(Menu.Name + "QRQ").GetValue<bool>() && Variable.Spells[SpellSlot.Q].IsReady()
                && Menu.Item(Menu.Name + "QRQDistance").GetValue<Slider>().Value
                >= target.Distance(Variable.Player))
            {
                Variable.Spells[SpellSlot.Q].Cast(InsecQ(target));
            }

            Variable.Spells[SpellSlot.R].Cast(InsecTo(target));
        }

        // Hotfix..!
        private Vector3 InsecQ(AIHeroClient target)
        {
            var rPred = rLogic.RPred(target)
                .Extend(
                    Variable.Player.Position,
                    Variable.Spells[SpellSlot.R].Width
                    - Menu.Item(Menu.Name + "RRangePred").GetValue<Slider>().Value);

            return rPred;
        }

        private Vector3 InsecTo(AIHeroClient target)
        {
            var mePos = Variable.Player.Position;
            // Doing this we can extend to our own position if we can't access anything else (Tower, Ally)

            switch (Menu.Item(Menu.Name + "InsecTo").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    var ally =
                        HeroManager.Allies.Where(
                            x =>
                            x.IsValidTarget(
                                Menu.Item(Menu.Name + "AllyRange").GetValue<Slider>().Value,
                                false,
                                target.ServerPosition) && x.Distance(target) > 325 && !x.IsMe && x.IsAlly)
                            .MaxOrDefault(
                                x =>
                                x.CountAlliesInRange(
                                    Menu.Item(Menu.Name + "AllyRange").GetValue<Slider>().Value));

                    if (ally != null)
                    {
                        mePos = ally.ServerPosition;
                    }

                    var turret =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .Where(
                                x =>
                                x.IsAlly && x.Distance(target) > 325
                                && x.Distance(target)
                                < Menu.Item(Menu.Name + "TurretRange").GetValue<Slider>().Value && !x.IsEnemy)
                            .OrderBy(x => x.Distance(Variable.Player.Position))
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
                    .CastTargetPosition.Extend(
                        mePos.To2D(),
                        -Menu.Item(Menu.Name + "RRangePred").GetValue<Slider>().Value);

            return pos.To3D();
        }

        private void OnDraw(EventArgs args)
        {
            if (Variable.Player.IsDead || !Menu.Item(Menu.Name + "RDraw").GetValue<bool>()) return;

            var target = TargetSelector.GetSelectedTarget();

            if (target == null || !target.IsValid) return;

            Render.Circle.DrawCircle(InsecTo(target), 100, Color.Cyan);

            if (Menu.Item(Menu.Name + "QRQ").GetValue<bool>())
            {
                Render.Circle.DrawCircle(InsecQ(target), 60, Color.Cyan);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !Variable.Spells[SpellSlot.R].IsReady()
                || Menu.Item(Menu.Name + "RMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            ExplosiveCask();
        }

        #endregion
    }
}