using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.OrbwalkingModes.Combo
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommonEx;
    using CommonEx.Classes;
    using CommonEx.Extensions;
    using CommonEx.Menu;
    using CommonEx.Menu.Presets;
    using CommonEx.Objects;

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PathTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;
    using global::YasuoMedia.CommonEx.Utility;
    using global::YasuoMedia.Yasuo.LogicProvider;
    using global::YasuoMedia.Yasuo.Menu.MenuSets.OrbwalkingModes.Combo;

    using LeagueSharp;
    using LeagueSharp.Common;

    using HitChance = LeagueSharp.SDK.Enumerations.HitChance;
    using PredictionOutput = LeagueSharp.SDK.PredictionOutput;

    #endregion

    internal class SteelTempest : FeatureChild<Combo>
    {
        #region Fields

        /// <summary>
        ///     The blacklist
        /// </summary>
        public BlacklistMenu BlacklistMenu;
        
        /// <summary>
        ///     The PathBase
        /// </summary>
        protected YasuoPath<Point, ConnectionBase<Point>> PathBase;

        /// <summary>
        ///     The target
        /// </summary>
        protected AIHeroClient Target;

        /// <summary>
        ///     The targets
        /// </summary>
        protected List<AIHeroClient> Targets = new List<AIHeroClient>();

        /// <summary>
        ///     The minions
        /// </summary>
        protected List<Obj_AI_Base> Units = new List<Obj_AI_Base>();

        /// <summary>
        ///     The E logicprovider
        /// </summary>
        private SweepingBladeLogicProvider providerE;

        /// <summary>
        ///     The Q logicprovider
        /// </summary>
        private SteelTempestLogicProvider providerQ;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SteelTempest" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public SteelTempest(Combo parent)
            : base(parent)
        {
            this.OnLoad();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name => "(Q) Steel Tempest";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnUpdate(EventArgs args)
        {
            this.SoftReset();

            if (GlobalVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !GlobalVariables.Spells[SpellSlot.Q].IsReady())
            {
                return;
            }

            this.GetUnits();

            this.LogicSingleTarget();

            this.LogicMultiKnockup();

            this.LogicStacking();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable()
        {
            Events.OnUpdate -= this.OnUpdate;
            base.OnDisable();
        }

        /// <summary>
        ///     Called when [enable].
        /// </summary>
        protected override void OnEnable()
        {
            Events.OnUpdate += this.OnUpdate;
            base.OnEnable();
        }

        /// <summary>
        ///     Called when [initialize].
        /// </summary>
        protected override void OnInitialize()
        {
            this.providerQ = new SteelTempestLogicProvider();
            this.providerE = new SweepingBladeLogicProvider();

            base.OnInitialize();
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);

            this.BlacklistMenu = new BlacklistMenu(this.Menu, "Blacklist");

            var menuGenerator = new MenuGenerator(new SteelTempestMenu(this.Menu));

            menuGenerator.Generate();

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        /// <summary>
        ///     Executes SteelTempest on the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="aoe">if set to <c>true</c> [aoe].</param>
        private void Execute(Obj_AI_Base target, bool aoe = false)
        {
            var predictionOutput = this.providerQ.GetPrediction(target, aoe);

            if (predictionOutput.Hitchance >= HitChance.High)
            {
                Console.WriteLine(predictionOutput.CastPosition);

                GlobalVariables.CastManager.Queque.Enqueue(
                    2,
                    () => GlobalVariables.Spells[SpellSlot.Q].Cast(predictionOutput.CastPosition));
            }
        }

        /// <summary>
        ///     Gets the units.
        /// </summary>
        private void GetUnits()
        {
            this.Targets =
                HeroManager.Enemies.Where(
                    x => x.Distance(GlobalVariables.Player) <= GlobalVariables.Spells[SpellSlot.Q].Range).ToList();

            this.Target = TargetSelector.GetTarget(
                GlobalVariables.Spells[SpellSlot.Q].Range,
                TargetSelector.DamageType.Physical);
        }

        // TODO: Decomposite
        /// <summary>
        ///     Method to Multi-KnockUp.
        /// </summary>
        private void LogicMultiKnockup()
        {
            var multiknockupsettings = this.Menu.SubMenu(this.Menu.Name + "Multi-Knockup Settings");

            var preds = new List<PredictionOutput>();

            if (this.Targets.Any())
            {
                foreach (var target in
                    this.Targets.Where(x => x.IsValid && x.IsValidTarget(GlobalVariables.Spells[SpellSlot.Q].Range)))
                {
                    var pred = this.providerQ.GetPrediction(target, true);

                    preds.Add(pred);
                }
            }

            if (!preds.Any()) return;
            {
                switch (
                    multiknockupsettings.Item(multiknockupsettings.Name + "Mode").GetValue<StringList>().SelectedIndex)
                {
                    // Custom
                    case 0:

                        var mostKnockedUp =
                            preds.Where(
                                x =>
                                x.AoeTargetsHitCount
                                >= multiknockupsettings.Item(multiknockupsettings.Name + "MinHitAOECustom")
                                       .GetValue<Slider>()
                                       .Value && x.Hitchance >= HitChance.VeryHigh)
                                .MaxOrDefault(x => x.AoeTargetsHit.Count);

                        if (mostKnockedUp != null && !mostKnockedUp.CastPosition.IsZero)
                        {
                            Console.WriteLine(@"MultiKnockUp Vector: " + mostKnockedUp.CastPosition);
                            GlobalVariables.CastManager.Queque.Enqueue(
                                2,
                                () => GlobalVariables.Spells[SpellSlot.Q].Cast(mostKnockedUp.CastPosition));
                        }

                        break;


                    //// PathBase Based
                    //// TODO: ADD calculation for arriving to that point on the PathBase
                    //case 1:
                    //    var PathBase = SweepingBlade.PathBaseCopy;

                    //    if (PathBase == null)
                    //    {
                    //        return;
                    //    }

                    //    // TODO
                    //    var vectors =
                    //        PathBase.SplitIntoVectors(multiknockupsettings.Item("SegmentAmount").GetValue<Slider>().Value);

                    //    var predDic = new Dictionary<Vector3, List<PredictionOutput>>();

                    //    if (vectors.Count > 0)
                    //    {
                    //        foreach (var vector in vectors)
                    //        {
                    //            var predsVec =
                    //                this.Targets.ToList()
                    //                    .Select(unit => this.providerQ.GetPrediction(vector, unit, true, 0))
                    //                    .ToList();

                    //            predDic.Add(vector, predsVec);
                    //        }
                    //    }

                    //    var scoreDic = new Dictionary<PredictionOutput, float>();

                    //    foreach (var entry in predDic.ToList())
                    //    {
                    //        var value = entry.Value;

                    //        foreach (var pred in value.ToList())
                    //        {
                    //            if (pred.Hitchance < HitChance.High)
                    //            {
                    //                continue;
                    //            }

                    //            switch (multiknockupsettings.Item("PriorityMode").GetValue<StringList>().SelectedIndex)
                    //            {
                    //                // ChampionYasuo Priority (Target Selector)
                    //                case 0:
                    //                    scoreDic.Add(pred, pred.AoeTargetsHit.Sum(x => TargetSelector.GetPriority(x)));
                    //                    break;
                    //                // Killability
                    //                case 1:
                    //                    scoreDic.Add(pred, pred.AoeTargetsHit.Sum(x => TargetSelector.GetPriority(x)));
                    //                    break;
                    //                // Something smart that takes many things into consideration
                    //                //case 2:
                    //                //    break;
                    //            }
                    //        }
                    //    }

                    //    PredictionOutput finalPred = null;

                    //    if (scoreDic.Any())
                    //    {
                    //        finalPred = scoreDic.MaxOrDefault(x => x.Value).Key;
                    //    }

                    //    if (finalPred != null)
                    //    {
                    //        GlobalVariables.Spells[SpellSlot.Q].Cast(finalPred.CastPosition);
                    //    }
                    //    break;
                }
            }
        }

        // TODO: Prediction while dashing
        /// <summary>
        ///     Method to Q single targets.
        /// </summary>
        private void LogicSingleTarget()
        {
            if (this.Target == null || !this.Target.IsValid)
            {
                return;
            }

            if (GlobalVariables.Player.IsDashing())
            {
                var dash = new CommonEx.Objects.Dash(GlobalVariables.Player.GetDashInfo().Unit);

                if (dash.EndPosition.Distance(this.Target.ServerPosition) > GlobalVariables.Spells[SpellSlot.Q].Range)
                {
                    return;
                }

                if (GlobalVariables.Debug)
                {
                    Console.WriteLine(@"OrbwalkingModes > Combo > Steel Tempest > EQ Trigger");
                }

                GlobalVariables.CastManager.ForceAction(
                    () => GlobalVariables.Spells[SpellSlot.Q].Cast(this.Target.ServerPosition));
            }
            else
            {
                this.Execute(this.Target);
            }
        }

        // TODO: Decomposite
        /// <summary>
        ///     Method to stack Q.
        /// </summary>
        private void LogicStacking()
        {
            var stacksettingsMenu = this.Menu.SubMenu(this.Menu.Name + "Stack-Settings");

            if (stacksettingsMenu.Item(stacksettingsMenu.Name + "Mode").GetValue<StringList>().SelectedValue
                == "Disabled" || GlobalVariables.Player.HasQ3())
            {
                return;
            }

            this.Units = this.providerE.GetUnits(GlobalVariables.Player.ServerPosition);

            this.PathBase = SweepingBlade.PathBaseCopy;

            switch (stacksettingsMenu.Item(stacksettingsMenu.Name + "Mode").GetValue<StringList>().SelectedIndex)
            {
                    #region Mode: Custom

                case 0:
                    var nearesthero =
                        HeroManager.Enemies.Where(x => !x.IsDead || !x.IsZombie)
                            .MinOrDefault(x => x.Distance(GlobalVariables.Player));

                    if (nearesthero == null
                        || nearesthero.Distance(GlobalVariables.Player.ServerPosition)
                        > stacksettingsMenu.Item(stacksettingsMenu.Name + "MaxDistance").GetValue<Slider>().Value
                        || nearesthero.Distance(GlobalVariables.Player.ServerPosition)
                        < stacksettingsMenu.Item(stacksettingsMenu.Name + "MinDistance").GetValue<Slider>().Value
                        || this.providerQ.RealCooldown()
                        > stacksettingsMenu.Item(stacksettingsMenu.Name + "MaxCooldownQ").GetValue<Slider>().Value
                        || !this.Units.Any())
                    {
                        return;
                    }

                    var path = this.PathBase;

                    if (path?.Connections != null
                        && stacksettingsMenu.Item(stacksettingsMenu.Name + "CarePath").GetValue<bool>())
                    {
                        var dashConnections = path.Connections.OfType<YasuoDashConnection>();

                        var yasuoDashConnections = dashConnections as IList<YasuoDashConnection> ?? dashConnections.ToList();

                        if (!yasuoDashConnections.Any()) return;

                        foreach (var unit in this.Units.ToList())
                        {
                            if (yasuoDashConnections.Where(x => x.Unit != null).Any(x => x.Unit.Equals(unit)))
                            {
                                this.Units.Remove(unit);
                            }
                        }
                    }

                    if (true)
                    {
                        var unitsNotMoving =
                            this.Units.Where(
                                x =>
                                !x.IsMoving
                                && x.Distance(GlobalVariables.Player) <= GlobalVariables.Spells[SpellSlot.Q].Range)
                                .ToList();

                        if (unitsNotMoving.Any())
                        {
                            var unit = unitsNotMoving.MinOrDefault(x => x.Health);

                            if (unit != null)
                            {
                                if (GlobalVariables.Debug)
                                {
                                    Console.WriteLine(
                                        @"OrbwalkingModes > Combo > SteelTempest > Stacking > Case 0: not moving units");
                                }

                                this.Execute(unit);
                            }
                        }
                        else
                        {
                            var unit =
                                this.Units.Where(x => GlobalVariables.Spells[SpellSlot.Q].IsInRange(x))
                                    .MinOrDefault(x => x.Health);

                            if (unit != null)
                            {
                                if (GlobalVariables.Debug)
                                {
                                    Console.WriteLine(
                                        @"OrbwalkingModes > Combo > SteelTempest > Stacking > Case 0: moving units");
                                }

                                this.Execute(unit);
                            }
                        }
                    }

                    break;

                    #endregion

                    #region Mode: Always

                case 1:
                    if (this.Units.Any())
                    {
                        var unitsNotMoving =
                            this.Units.Where(
                                x =>
                                !x.IsMoving
                                && x.Distance(GlobalVariables.Player) <= GlobalVariables.Spells[SpellSlot.Q].Range)
                                .ToList();

                        if (unitsNotMoving.Any())
                        {
                            var unit = unitsNotMoving.MinOrDefault(x => x.Health);

                            if (unit != null)
                            {
                                if (GlobalVariables.Debug)
                                {
                                    Console.WriteLine(
                                        @"OrbwalkingModes > Combo > SteelTempest > Stacking > Case 1: not moving units");
                                }

                                this.Execute(unit);
                            }
                        }
                        else
                        {
                            var unit =
                                this.Units.Where(x => GlobalVariables.Spells[SpellSlot.Q].IsInRange(x))
                                    .MinOrDefault(x => x.Health);

                            if (unit != null)
                            {
                                if (GlobalVariables.Debug)
                                {
                                    Console.WriteLine(
                                        @"OrbwalkingModes > Combo > SteelTempest > Stacking > Case 1: moving units");
                                }

                                this.Execute(unit);
                            }
                        }
                    }
                    break;

                    #endregion

                    #region Mode: PathBase Based

                case 2:
                    if (this.PathBase != null && this.Units.Any())
                    {
                        foreach (var unit in this.Units.ToList())
                        {
                            var dashConnections = this.PathBase.Connections.OfType<YasuoDashConnection>();

                            if (dashConnections.Any(x => x.Unit != null && x.Unit.Equals(unit)))
                            {
                                this.Units.Remove(unit);
                            }
                        }

                        if (GlobalVariables.Player.CountEnemiesInRange(GlobalVariables.Spells[SpellSlot.Q].Range) == 0
                            && this.PathBase.PathCost + 500 > GlobalVariables.Spells[SpellSlot.Q].Cooldown)
                        {
                            var dashConnections = this.PathBase.Connections.OfType<YasuoDashConnection>().ToList();

                            if (dashConnections.Any(
                                    x =>
                                    x.Unit.Health
                                    > this.providerE.GetDamage(x.Unit) + this.providerQ.GetDamage(x.Unit)))
                            {
                                if (GlobalVariables.Player.IsDashing())
                                {
                                    this.Execute(dashConnections.FirstOrDefault(x => x.Unit != null)?.Unit);
                                }
                            }

                            else if (this.Units.Count > 0)
                            {
                                var unitsNotMoving =
                                    this.Units.Where(
                                        x =>
                                        !x.IsMoving
                                        && x.Distance(GlobalVariables.Player)
                                        <= GlobalVariables.Spells[SpellSlot.Q].Range).ToList();

                                if (unitsNotMoving.Any())
                                {
                                    this.Execute(unitsNotMoving.MinOrDefault(x => x.Distance(GlobalVariables.Player)));
                                }
                                else
                                {
                                    var unit = this.Units.MinOrDefault(x => x.Distance(GlobalVariables.Player));

                                    if (unit != null
                                        && unit.Distance(GlobalVariables.Player)
                                        <= GlobalVariables.Spells[SpellSlot.Q].Range)
                                    {
                                        this.Execute(unit);
                                    }
                                }
                            }
                        }
                    }

                    break;

                    #endregion
            }
        }

        /// <summary>
        ///     Resets
        /// </summary>
        private void SoftReset()
        {
            this.Units = new List<Obj_AI_Base>();
            this.PathBase = null;
        }

        #endregion
    }
}