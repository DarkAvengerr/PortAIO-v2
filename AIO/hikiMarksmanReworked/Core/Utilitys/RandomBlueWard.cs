using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Core.Utilitys
{
    class RandomBlueWard
    {
        //public static Items.Item Farsight = new Items.Item(3342, 4000f);
        public static void BlueWardCast()
        {

          
        }

        public struct Positions
        {
            public readonly Vector3 Safepos;

            public Positions(Vector3 safepos)
            {
                Safepos = safepos;
            }
        }

        public static List<Positions> Spots;

        static RandomBlueWard()
        {
            Spots.Add(new Positions(new Vector3(10273.9f, 3257.76f, 49.03f)));
            Spots.Add(new Positions(new Vector3(4473.9f, 11457.76f, 51.4f)));
            Spots.Add(new Positions(new Vector3(3078.62f, 10868.39f, -67.95f)));
            Spots.Add(new Positions(new Vector3(5123.9f, 8457.76f, -21.23f)));
            Spots.Add(new Positions(new Vector3(6202.24f, 8132.12f, -67.39f)));
            Spots.Add(new Positions(new Vector3(8523.9f, 4707.76f, 51.24f)));
            Spots.Add(new Positions(new Vector3(9823.9f, 6507.76f, 23.47f)));
            Spots.Add(new Positions(new Vector3(8718.88f, 6764.86f, 95.75f)));
            Spots.Add(new Positions(new Vector3(12023.9f, 3757.76f, -66.25f)));
            Spots.Add(new Positions(new Vector3(6273.9f, 10307.76f, 53.67f)));
            Spots.Add(new Positions(new Vector3(8163.71f, 3436.05f, 51.6628f)));
            Spots.Add(new Positions(new Vector3(6678.08f, 11477.83f, 53.85f)));
            Spots.Add(new Positions(new Vector3(2773.9f, 11307.76f, -71.24f)));
            Spots.Add(new Positions(new Vector3(5123.9f, 8457.76f, -21.23f)));
            Spots.Add(new Positions(new Vector3(9773.9f, 6457.76f, 9.56f)));
            Spots.Add(new Positions(new Vector3(6723.9f, 2507.76f, 52.17f)));
            Spots.Add(new Positions(new Vector3(6723.9f, 2507.76f, 52.17f)));
            Spots.Add(new Positions(new Vector3(8323.9f, 12457.76f, 56.48f)));
        }
    }

}
