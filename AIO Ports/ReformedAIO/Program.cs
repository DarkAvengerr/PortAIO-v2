using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO
{
    #region Using Directives

    using System.Collections.Generic;

    using ReformedAIO.Champions;
    using ReformedAIO.Champions.Ashe;
    using ReformedAIO.Champions.Caitlyn;
    using ReformedAIO.Champions.Diana;
    using ReformedAIO.Champions.Gnar;
    using ReformedAIO.Champions.Gragas;
    using ReformedAIO.Champions.Lucian;

    using RethoughtLib.Bootstraps.Abstract_Classes;

    #endregion

    internal class Program
    {
        #region Methods

        public static void Main()
        {
            var bootstrap = new Bootstrap(new List<LoadableBase>
            {
                new AsheLoader(),
                new CaitlynLoader(),
                new DianaLoader(),
                new LucianLoader(),
                new GragasLoader(),
                new GnarLoader()
            });

            bootstrap.Run();
        }

        #endregion
    }
}