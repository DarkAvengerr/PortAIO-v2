using EloBuddy.SDK.Events;
using EloBuddy.SDK.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortAIO
{
    class Program
    {
        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(System.EventArgs args)
        {
            PortAIO.Init.Initialize();
        }
    }
}
