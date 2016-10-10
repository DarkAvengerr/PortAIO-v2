using _Project_Geass.Functions;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Humanizer.TickTock
{

    internal class Handler
    {
        #region Private Fields

        /// <summary>
        ///     My Tick Manager
        /// </summary>
        private static readonly Manager MyTicker=new Manager();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        ///     Checks the tear stack.
        /// </summary>
        /// <returns>
        /// </returns>
        public static bool CheckAutoEvents() => MyTicker.CheckTick($"{Names.ProjectName}.AutoEvent");

        /// <summary>
        ///     Checks the items.
        /// </summary>
        /// <returns>
        /// </returns>
        public static bool CheckItems() => MyTicker.CheckTick($"{Names.ProjectName}.UseItems");

        /// <summary>
        ///     Checks the on level.
        /// </summary>
        /// <returns>
        /// </returns>
        public static bool CheckOnLevel() => MyTicker.CheckTick($"{Names.ProjectName}.OnLevel");

        /// <summary>
        ///     Checks the orbwalker.
        /// </summary>
        public static bool CheckOrbwalker() => MyTicker.CheckTick($"{Names.ProjectName}.Orbwalker");

        /// <summary>
        ///     Checks the trinket.
        /// </summary>
        /// <returns>
        /// </returns>
        public static bool CheckTrinket() => MyTicker.CheckTick($"{Names.ProjectName}.TrinketBuy");

        /// <summary>
        ///     Loads the specified humanize.
        /// </summary>
        /// <param name="humanize">
        ///     if set to <c> true </c> [humanize].
        /// </param>
        public static void Load(bool humanize)
        {
            var offset=0;
            if (humanize)
                offset=100;

            MyTicker.AddTick($"{Names.ProjectName}.OnLevel", 50+offset, 75+offset);
            MyTicker.AddTick($"{Names.ProjectName}.UseItems", offset, 25+offset);
            MyTicker.AddTick($"{Names.ProjectName}.TrinketBuy", 50+offset, 100+offset);
            MyTicker.AddTick($"{Names.ProjectName}.Orbwalker", 10+offset, 25+offset);
            MyTicker.AddTick($"{Names.ProjectName}.AutoEvent", 10+offset, 25+offset);
        }

        /// <summary>
        ///     Uses the tear stack.
        /// </summary>
        public static void UseAutoEvent() => MyTicker.UseTick($"{Names.ProjectName}.AutoEvent");

        /// <summary>
        ///     Uses the items.
        /// </summary>
        public static void UseItems() => MyTicker.UseTick($"{Names.ProjectName}.UseItems");

        /// <summary>
        ///     Uses the on level.
        /// </summary>
        public static void UseOnLevel() => MyTicker.UseTick($"{Names.ProjectName}.OnLevel");

        /// <summary>
        ///     Uses the orbwalker.
        /// </summary>
        public static void UseOrbwalker() => MyTicker.UseTick($"{Names.ProjectName}.Orbwalker");

        /// <summary>
        ///     Uses the trinket.
        /// </summary>
        public static void UseTrinket() => MyTicker.UseTick($"{Names.ProjectName}.TrinketBuy");

        #endregion Public Methods
    }

}