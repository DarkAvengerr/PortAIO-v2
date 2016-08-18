using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes
{
    #region Using Directives

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Dash = Objects.Dash;

    #endregion

    public class YasuoDashConnection : ConnectionBase<Point>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Constructor for a connection
        /// </summary>
        /// <param name="start">start point</param>
        /// <param name="end">end point</param>
        /// <param name="unit"></param>
        public YasuoDashConnection(Point start, Point end, Obj_AI_Base unit)
        {
            this.Start = start;
            this.End = end;
            this.Unit = unit;

            this.Cost = new Dash(this.Start.Position, unit).EndPosition.Distance(GlobalVariables.Player.ServerPosition)
                        / GlobalVariables.Spells[SpellSlot.E].Speed;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the unit.
        /// </summary>
        /// <value>
        ///     The unit.
        /// </value>
        public Obj_AI_Base Unit { get; set; }

        #endregion
    }
}