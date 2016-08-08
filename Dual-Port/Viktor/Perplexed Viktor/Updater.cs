using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PerplexedViktor
{
     class Updater
    {
        static System.Version Version = Assembly.GetExecutingAssembly().GetName().Version;
        public static bool Outdated()
        {
            return GetLatestVersion() != Version.ToString();
        }

        public static string GetLatestVersion()
        {
            return new BetterWebClient(null).DownloadString("https://raw.githubusercontent.com/Perplexity/LeagueSharp/master/PerplexedViktor/PerplexedViktor/version/version.txt");
        }
    }
}
