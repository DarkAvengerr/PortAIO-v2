using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ThreshTherulerofthesoul
{
    public class GameObjectEscapeDetectorEventArgs
    {
        public AIHeroClient Sender;
        public SpellSlot Slot;
        public Vector3 Start;
        public Vector3 End;
        public int StartTickCount;
        public string SpellData;
    }

    public delegate void EscapeDetector(AIHeroClient sender, GameObjectEscapeDetectorEventArgs args);

    public class EscapeBlocker
    {
        public static event EscapeDetector OnDetectEscape;

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static List<string> EscapeSpells = new List<string>();
        private static List<GameObjectEscapeDetectorEventArgs> ActiveEscapeSpells = new List<GameObjectEscapeDetectorEventArgs>();

        static EscapeBlocker()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnUpdate += Game_OnUpdate;

            InitializeSpells();
        }

        private static void InitializeSpells()
        {
            #region Spells

            //EscapeSpells.Add("summonerflash"); // ALL Champions Flash
            EscapeSpells.Add("LucianE"); // Lucian E
            EscapeSpells.Add("LeesinW"); // Leesin W
            EscapeSpells.Add("BlindMonkQTwo"); // Leesin Q2
            EscapeSpells.Add("KhazixE"); // Khazix E khazixelong
            EscapeSpells.Add("KhazixELong"); // Khazix E Long
            EscapeSpells.Add("Pounce"); // Nidalee W
            EscapeSpells.Add("TristanaW"); // Tristana W
            EscapeSpells.Add("SejuaniArcticAssault"); // Sejuani Q
            EscapeSpells.Add("ShenShadowDash"); // Shen E
            EscapeSpells.Add("AatroxQ"); // Aatorx Q
            EscapeSpells.Add("RenektonSliceAndDice"); // Renekton E
            EscapeSpells.Add("GravesMove"); // Graves E
            EscapeSpells.Add("JarvanIVDragonStrike"); // Jarvan EQ
            EscapeSpells.Add("GragasE"); // Gragas E
            EscapeSpells.Add("GnarE"); // Gnar E
            EscapeSpells.Add("ViQ"); // Vi Q
            //EscapeSpells.Add("EzrealE"); // Ezreal E
            EscapeSpells.Add("RivenE"); // Riven E
            //EscapeSpells.Add("KatarinaE"); // Katarina E
            EscapeSpells.Add("CorkiW"); // Corki W

            EscapeSpells.Add("VayneQ"); // Vayne Q
            EscapeSpells.Add("ShacoQ"); // shaco Q
            EscapeSpells.Add("CayitlynE"); // Cayitlyn E
            EscapeSpells.Add("ZacE"); // Zac E
            EscapeSpells.Add("ShyvanaR"); // Shyvana R

            // Leblanc W
            // Ahri R 
            // Jax Q
            // Ziqs W
            // Kassadin R
            // Queen
            // Hecarim R

            #endregion
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            ActiveEscapeSpells.RemoveAll(x => Environment.TickCount > x.StartTickCount + 900);

            if (OnDetectEscape == null)
                return;

            foreach (var a in ActiveEscapeSpells)
            {
                OnDetectEscape(a.Sender, a);
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is AIHeroClient))
                return;

            var _sender = sender as AIHeroClient;

            if (!CheckSpells(_sender, args))
                return;

            ActiveEscapeSpells.Add(new GameObjectEscapeDetectorEventArgs
            {
                Sender = _sender,
                Slot = _sender.GetSpellSlot(args.SData.Name),
                Start = args.Start,
                End = args.End,
                StartTickCount = Environment.TickCount,
                SpellData = args.SData.Name
            });
        }

        private static bool CheckSpells(AIHeroClient sender, GameObjectProcessSpellCastEventArgs args)
        {
            return EscapeSpells.Contains(args.SData.Name) || EscapeSpells.Contains(sender.ChampionName + sender.GetSpellSlot(args.SData.Name));
        }
    }
}
