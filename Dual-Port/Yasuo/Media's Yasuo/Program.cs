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
        static void Main(string[] args)
        {
            var bootstrap = new BootstrapContainer();

            bootstrap.Initialize();
        }           
    }
}