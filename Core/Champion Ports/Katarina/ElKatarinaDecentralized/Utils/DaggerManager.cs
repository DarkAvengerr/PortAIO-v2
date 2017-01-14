using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
using LeagueSharp.Common; 
namespace ElKatarinaDecentralized.Utils
{
    using ElKatarinaDecentralized.Enumerations;

    using LeagueSharp;

    using SharpDX;

    internal class DaggerManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly List<Daggers> ExistingDaggers = new List<Daggers>();

        /// <summary>
        ///     Daggers.
        /// </summary>
        internal class Daggers
        {
            /// <summary>
            ///     The object.
            /// </summary>
            public GameObject Object { get; set; }

            /// <summary>
            ///     The network ID.
            /// </summary>
            public float NetworkId { get; set; }

            /// <summary>
            ///     The dagger position.
            /// </summary>
            public Vector3 DaggerPos { get; set; }

            /// <summary>
            ///     The dagger expire time.
            /// </summary>
            public double ExpireTime { get; set; }
        }

        /// <summary>
        ///     Called upon game object creation.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnCreate(GameObject obj, EventArgs args)
        {
            if (!obj.Name.Contains("Katarina_Base_Q_Dagger_Land_"))
            {
                return;
            }

            ExistingDaggers.Add(
                   new Daggers
                   {
                       Object = obj,
                       NetworkId = obj.NetworkId,
                       DaggerPos = obj.Position,
                       ExpireTime = Game.Time + 3
                   });
        }

        /// <summary>
        ///     Called upon game object delete.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.Name.Contains("Katarina_Base_Q_Dagger_Land_") || obj.Name.Contains("PickUp"))
            {
                for (var i = 0; i < ExistingDaggers.Count; i++)
                {
                    if (ExistingDaggers[i].NetworkId == obj.NetworkId)
                    {
                        ExistingDaggers.RemoveAt(i);
                        return;
                    }
                }
            }
        }
    }
}
