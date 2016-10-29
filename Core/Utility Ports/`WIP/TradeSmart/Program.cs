using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TradeSmart
{
    using RethoughtLib.Bootstraps.Implementations;

    using TradeSmart.Modules;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var bootstrap = new LeagueSharpMultiBootstrap();

            bootstrap.AddModule(new TradeSmartLoader());

            bootstrap.AddString("TradeSmart");

            bootstrap.Run();
        }
    }
}
