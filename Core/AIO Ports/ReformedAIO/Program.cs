using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO
{
    #region Using Directives

    using System.Collections.Generic;

    using ReformedAIO.Champions.Annie;

    //  using ReformedAIO.Champions;
    using ReformedAIO.Champions.Ashe;
    using ReformedAIO.Champions.Brand;
    using ReformedAIO.Champions.Caitlyn;
    using ReformedAIO.Champions.Diana;
    using ReformedAIO.Champions.Ezreal;
    using ReformedAIO.Champions.Gnar;
    using ReformedAIO.Champions.Gragas;
    using ReformedAIO.Champions.Lucian;
    using ReformedAIO.Champions.Olaf;
    using ReformedAIO.Champions.Thresh;
    using ReformedAIO.Champions.Vayne;
    using ReformedAIO.Champions.Xerath;
    using ReformedAIO.Champions.Yasuo;
    using ReformedAIO.Champions.Ziggs;

    using RethoughtLib.Bootstraps.Abstract_Classes;

    #endregion

    internal class Program
    {
        #region Methods

        public static void Main()
        {
            var bootstrap = new Bootstrap(new List<LoadableBase>
            {
                new AnnieLoader(),
                new AsheLoader(),
                new BrandLoader(),
                new CaitlynLoader(),
                new DianaLoader(),
                new GragasLoader(),
                new GnarLoader(),
                new EzrealLoader(),
                new LucianLoader(),
                new OlafLoader(),
                new ThreshLoader(),
                new VayneLoader(),
                new XerathLoader(),
                new YasuoLoader(),
                new ZiggsLoader()
            });

            bootstrap.Run();
        }

        #endregion
    }
}