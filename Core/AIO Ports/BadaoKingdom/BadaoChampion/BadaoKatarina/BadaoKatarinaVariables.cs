using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
namespace BadaoKingdom.BadaoChampion.BadaoKatarina
{
    using static BadaoMainVariables;
    // KatarinaRMis
    public static class BadaoKatarinaVariables
    {
        public static AIHeroClient Player => ObjectManager.Player;

        public static int LastRMis = 0;
        public static List<MissileClient> RMis = new List<MissileClient>();
        public static List<MissileClient> WMis = new List<MissileClient>();
        public static List<GameObject> MyBeam = new List<GameObject>();
        public static List<KatarinaDagger> Daggers = new List<KatarinaDagger>();
        public static List<KatarinaDagger> WDaggers => Daggers.Where(x => WMis.Any(y => y.EndPosition.To2D().Distance(x.Dagger.Position.To2D()) <= 20)).ToList();
        public static List<KatarinaDagger> PickableDaggers => Daggers.Where(x => Environment.TickCount - x.CreationTime >= 1175
            && x.Dagger.Position.Distance(Player.Position) <= E.Range && MyBeam.Any(y => y.Position.Distance(x.Dagger.Position) <= 20)).ToList();

        //menu
        public static MenuItem FleeKey;
        public static MenuItem JumpKey;

        public static MenuItem ComboCancelRForKS;
        public static MenuItem ComboCancelRNoTarget;

        public static MenuItem AutoKs;

        public static MenuItem HarassWE;

        public static MenuItem LastHitQ;

        public static MenuItem LaneClearQ;
        public static MenuItem LaneClearW;


    }
    public class KatarinaDagger
    {
        public int CreationTime;
        public GameObject Dagger;
    }
}
