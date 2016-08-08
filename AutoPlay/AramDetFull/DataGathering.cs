using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LeagueSharp;using DetuksSharp;

using EloBuddy; namespace ARAMDetFull
{
    class DataGathering
    {
        public static bool on = true;

        public static void sendEndGame(bool won)
        {
            var postData = "champion=" + ObjectManager.Player.ChampionName;
            postData += "&kills=" + ObjectManager.Player.ChampionsKilled;
            postData += "&deaths=" + ObjectManager.Player.Deaths;
            postData += "&assists=" + ObjectManager.Player.Assists;
            postData += "&won=" + ((won)?"Y":"N");
            sendData("logendgame.php", postData);
        }

        public static void sendError(string data)
        {
            var postData = "champion="+ ObjectManager.Player.ChampionName;
            postData += "&stacktrace=" + ((data.Length>999)?data.Substring(0,999):data);
            sendData("logerror.php", postData);
        }

        [SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
        private static void sendData(string to, string postData)
        {
            if (!on)
                return;
            try
            {
                new Thread(() =>
                {
                    var request = (HttpWebRequest)WebRequest.Create("http://errorsharp.azurewebsites.net/" + to);
                    request.Timeout = 2000;
                    var data = Encoding.ASCII.GetBytes(postData);

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)request.GetResponse();
                }).Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
