using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Tristana
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Reflection;
    using LeagueSharp;

    class CheckVersion
    {
        public static Version Version;

        //Update by h3h3
        internal static void Check()
        {
            Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        using (var c = new WebClient())
                        {
                            var rawVersion =
                                c.DownloadString(
                                    "https://github.com/CHA2172886/NewFlowers/blob/master/Flowers%20Series/Flowers-Tristana/Properties/AssemblyInfo.cs");

                            var match =
                                new Regex(
                                    @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]")
                                    .Match(rawVersion);

                            Version = Assembly.GetExecutingAssembly().GetName().Version;

                            if (match.Success)
                            {
                                var gitVersion =
                                    new System.Version(
                                        string.Format(
                                            "{0}.{1}.{2}.{3}",
                                            match.Groups[1],
                                            match.Groups[2],
                                            match.Groups[3],
                                            match.Groups[4]));

                                if (gitVersion != Version)
                                {
                                    Chat.Print("<font color=\"#FF0000\">The Sprites is outdate!</font> Please Update to New Version!");
                                }
                                else
                                {
                                    Chat.Print("<font color=\"#FF0000\">Welcome to Tristana World!</font> - Credit:NightMoon");
                                }
                            }
                        }
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
        }
    }
}
