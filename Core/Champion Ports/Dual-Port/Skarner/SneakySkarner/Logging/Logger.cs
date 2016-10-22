using System;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SneakySkarner.Logging
{
    public static class Logger
    {
        private static readonly string Prefix = ConsoleColor.Gray + "[AlrikAIO] " + ConsoleColor.White;
        private static readonly string DebugPrefix = ConsoleColor.DarkGray + "[Debug] " + ConsoleColor.White;
        public static void PrintInfo(string msg)
        {
            var formatMsg = msg;
            Console.WriteLine(Prefix + formatMsg);
        }

        public static void PrintError(string msg)
        {
            var formatMsg = msg;
            Console.WriteLine(Prefix + formatMsg);
        }

        public static void PrintWarning(string msg)
        {
            var formatMsg = msg;
            Console.WriteLine(Prefix + formatMsg);
        }

        public static void PrintDebug(string msg)
        {
            var formatMsg = msg;
            Console.WriteLine(DebugPrefix + formatMsg);
        }
    }
}
