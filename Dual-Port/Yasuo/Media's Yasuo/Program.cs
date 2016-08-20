using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia
{
    class Program
    {
        /// <summary>
        ///     Creates a new instance of the BootstrapContainer Class
        /// </summary>
        /// <param name="args"></param>
        public static void Main()
        {
            var bootstrap = new BootstrapContainer();

            bootstrap.Initialize();
        }           
    }
}