using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Modules.Auto
{
    using System;

    using CommonEx;
    using CommonEx.Classes;

    using global::YasuoMedia.CommonEx.Utility;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class KillSteal : FeatureChild<Modules>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="KillSteal" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public KillSteal(Modules parent)
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
        public override string Name => "Killsteal";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Executes the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="spellslot">The spellslot.</param>
        public void Execute(Obj_AI_Base target, SpellSlot spellslot)
        {
            GlobalVariables.Spells[spellslot].Cast(target);
        }

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnUpdate(EventArgs args)
        {
            // TODO: Add Logic for every spell.
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
            base.OnInitialize();
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);
            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinHealthPercentage", "Min Own Health %").SetValue(new Slider(15, 0, 99)));

            #region E

            this.Menu.AddItem(new MenuItem(this.Name + "NoEintoEnemies", "Don't E into enemies").SetValue(true));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MaxEnemiesAroundE", "Max Enemies Around DashEnd Position").SetValue(
                    new Slider(2, 1, 5)));

            #endregion

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        #endregion
    }
}