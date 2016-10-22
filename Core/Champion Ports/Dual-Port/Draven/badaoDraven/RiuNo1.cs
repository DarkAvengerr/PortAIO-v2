using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoDraven
{
    class Riu
    {
        public Riu(GameObject Object,float CreatTime,Vector3 position,int NId)
        {
            obj = Object;
            CreationTime = CreatTime;
            NetworkId = NId;
            Position = position;
        }
        public GameObject obj { get; set; }
        public float CreationTime { get; set; }
        public int NetworkId { get; set; }
        public Vector3 Position { get; set; }

    }
    class RiuNo1
    {
        public RiuNo1(GameObject Object,float CreatTime,Vector3 position,int NId)
        {
            obj = Object;
            CreationTime = CreatTime;
            NetworkId = NId;
            Position = position;
        }
        public GameObject obj { get; set; }
        public float CreationTime { get; set; }
        public int NetworkId { get; set; }
        public Vector3 Position { get; set; }
        public static RiuNo1 RiuSo1()
        {
            float x = float.MaxValue;
            RiuNo1 RiuNo1 = null;
            if (Program.Riu != null)
            {
                foreach (var riu in Program.Riu)
                {
                    if (riu != null && riu.CreationTime <= x)
                    {
                        x = riu.CreationTime;
                        RiuNo1 = new RiuNo1 (riu.obj,riu.CreationTime,riu.Position,riu.NetworkId);
                    }
                }
            }
            return RiuNo1;
        }
    }
}
