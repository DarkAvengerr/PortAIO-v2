using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Rethought_Irelia
{
    #region Using Directives

    using RethoughtLib.Bootstraps.Implementations;

    using Rethought_Irelia.IreliaV1;

    #endregion

    internal class Program
    {
        #region Methods

        public static void Main()
        {
            var bootstrap = new LeagueSharpMultiBootstrap();

            bootstrap.AddModule(new Loader());
            bootstrap.AddString("Irelia_" + bootstrap.Modules.Count);

            bootstrap.Run();
        }

        #endregion
    }
}