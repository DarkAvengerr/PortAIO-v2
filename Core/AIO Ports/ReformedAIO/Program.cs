using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO
{
    #region Using Directives

    using System.Collections.Generic;

    using Champions.Annie;
    using Champions.Ashe;
    using Champions.Brand;
    using Champions.Caitlyn;
    using Champions.Diana;
    using Champions.Ezreal;
    using Champions.Gnar;
    using Champions.Gragas;
    using Champions.Lucian;
    using Champions.Lux;
    using Champions.Olaf;
    using Champions.Thresh;
    using Champions.Vayne;
    using Champions.Xerath;
    using Champions.Yasuo;
    using Champions.Ziggs;

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
                new GnarLoader(),
                new GragasLoader(),
                new EzrealLoader(),
                new LucianLoader(),
                new LuxLoader(),
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