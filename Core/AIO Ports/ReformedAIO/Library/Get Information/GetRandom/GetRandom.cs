using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Library.Get_Information.GetRandom
{
    using System;

    internal class GetRandom
    {
        private readonly Random random = new Random();

        public int GetRandomInt(int minimum, int maximum)
        {
            return random.Next(minimum, maximum);
        }
    }
}
