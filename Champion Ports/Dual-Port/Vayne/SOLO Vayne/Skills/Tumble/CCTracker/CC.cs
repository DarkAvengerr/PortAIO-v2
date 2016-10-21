using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Skills.Tumble.CCTracker
{
    class CC
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CC"/> class.
        /// </summary>
        /// <param name="ChampName">Name of the champ.</param>
        /// <param name="Slot">The slot.</param>
        /// <param name="Range">The range.</param>
        /// <param name="RangeType">Type of the range.</param>
        /// <param name="Type">The type.</param>
        public CC(string ChampName, SpellSlot Slot, float Range, CCRange RangeType, CCType Type)
        {
            this.ChampName = ChampName;
            this.Slot = Slot;
            this.Range = Range;
            this.RangeType = RangeType;
            this.Type = Type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CC"/> class.
        /// </summary>
        /// <param name="ChampName">Name of the champ.</param>
        /// <param name="Slot">The slot.</param>
        /// <param name="Range">The range.</param>
        /// <param name="AOERadius">The aoe radius.</param>
        /// <param name="RangeType">Type of the range.</param>
        /// <param name="Type">The type.</param>
        public CC(string ChampName, SpellSlot Slot, float Range, float AOERadius, CCRange RangeType, CCType Type)
        {
            this.ChampName = ChampName;
            this.Slot = Slot;
            this.Range = Range;
            this.AOERadius = AOERadius;
            this.RangeType = RangeType;
            this.Type = Type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CC"/> class.
        /// </summary>
        /// <param name="ChampName">Name of the champ.</param>
        /// <param name="Slot">The slot.</param>
        /// <param name="Range">The range.</param>
        /// <param name="RangeType">Type of the range.</param>
        /// <param name="Type">The type.</param>
        /// <param name="d">The d.</param>
        public CC(string ChampName, SpellSlot Slot, float Range, CCRange RangeType, CCType Type, ConditionDelegate d)
        {
            this.ChampName = ChampName;
            this.Slot = Slot;
            this.Range = Range;
            this.RangeType = RangeType;
            this.Type = Type;
            this.NecessaryCondition = d;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CC"/> class.
        /// </summary>
        /// <param name="ChampName">Name of the champ.</param>
        /// <param name="Slot">The slot.</param>
        /// <param name="Range">The range.</param>
        /// <param name="AOERadius">The aoe radius.</param>
        /// <param name="RangeType">Type of the range.</param>
        /// <param name="Type">The type.</param>
        /// <param name="d">The d.</param>
        public CC(string ChampName, SpellSlot Slot, float Range, float AOERadius, CCRange RangeType, CCType Type, ConditionDelegate d)
        {
            this.ChampName = ChampName;
            this.Slot = Slot;
            this.Range = Range;
            this.AOERadius = AOERadius;
            this.RangeType = RangeType;
            this.Type = Type;
            this.NecessaryCondition = d;
        }

        /// <summary>
        /// Gets or sets the name of the champ.
        /// </summary>
        /// <value>
        /// The name of the champ.
        /// </value>
        public string ChampName { get; set; }

        /// <summary>
        /// Gets or sets the slot.
        /// </summary>
        /// <value>
        /// The slot.
        /// </value>
        public SpellSlot Slot { get; set; }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>
        /// The range.
        /// </value>
        public float Range { get; set; }

        /// <summary>
        /// Gets or sets the aoe radius.
        /// </summary>
        /// <value>
        /// The aoe radius.
        /// </value>
        public float AOERadius { get; set; }

        /// <summary>
        /// Gets or sets the type of the range.
        /// </summary>
        /// <value>
        /// The type of the range.
        /// </value>
        public CCRange RangeType { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public CCType Type { get; set; }

        /// <summary>
        /// Gets or sets the necessary condition.
        /// </summary>
        /// <value>
        /// The necessary condition.
        /// </value>
        public ConditionDelegate NecessaryCondition { get; set; }

        /// <summary>
        /// The Condition Delegate
        /// </summary>
        /// <returns></returns>
        public delegate bool ConditionDelegate();

        /// <summary>
        /// Gets the danger value.
        /// </summary>
        /// <returns></returns>
        public float GetDangerValue()
        {
            //Some Calculations here
            return 0f;
        }

    }

    enum CCRange
    {
        /// <summary>
        /// Ranged CC
        /// </summary>
        Ranged,

        /// <summary>
        /// Melee CC
        /// </summary>
        Melee
    }

    enum CCType
    {
        /// <summary>
        /// AOE CC
        /// </summary>
        AOE,

        /// <summary>
        /// Targetted CC
        /// </summary>
        Targetted,

        /// <summary>
        /// AOE CC Starting at Champion Position
        /// </summary>
        AOEFromChamp
    }
}
