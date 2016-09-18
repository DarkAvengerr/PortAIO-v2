using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.HealthPrediction.Implementations.HealthPredictionObservers
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.HealthPrediction.Abstract_Classes;

    #endregion

    internal class AutoAttackObserver : HealthPredictionObserverModule
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "Auto Attacks";

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Obj_AI_Base.OnProcessSpellCast -= this.ObjAiBaseOnOnProcessSpellCast;
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Obj_AI_Base.OnProcessSpellCast += this.ObjAiBaseOnOnProcessSpellCast;
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);
        }

        private void ObjAiBaseOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Orbwalking.IsAutoAttack(args.SData.Name))
            {
                return;
            }

            //this.HealthPredictionOutput =
            //    new HealthPredictionOutput(
            //        sender.TotalAttackDamage,
            //        sender,
            //        (Obj_AI_Base)args.Target,
            //        args.Start,
            //        args.End,
            //        Utils.GameTimeTickCount,
            //        sender.AttackDelay,
            //        args.SData.MissileSpeed,
            //        new Func<Obj_AI_Base, Obj_AI_Base, double>(sender, (Obj_AI_Base)args.Target));

            //this.OnOnObserved(this.HealthPredictionOutput);
        }

        private void AutoAttackDamageCalc(Obj_AI_Base sender, Obj_AI_Base target)
        {

        }

        #endregion
    }
}