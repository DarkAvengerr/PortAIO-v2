using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using HybridCommon;
using HikiCarry_Vayne_Masterrace.Champions;
//typedefs
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Vayne_Masterrace
{
    class Program
    {
        public static BaseChamp Champion;
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            switch (ObjectManager.Player.Hero)
            {
                case EloBuddy.Champion.Vayne:
                    Champion = new Vayne();
                    break;
            }

            Champion.CreateConfigMenu();
            Champion.SetSpells();

            #region Events
            Game.OnUpdate += Champion.Game_OnUpdate;                                                                                       
            Orbwalking.BeforeAttack += Champion.Orbwalking_BeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += Champion.AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Champion.Interrupter_OnPossibleToInterrupt;
            Obj_AI_Base.OnBuffGain += Champion.Obj_AI_Base_OnBuffAdd;
            Obj_AI_Base.OnProcessSpellCast += Champion.Obj_AI_Base_OnProcessSpellCast;
            #endregion

            
            
            Notifications.AddNotification(String.Format("HikiCarry Vayne Masterrace - {0} Loaded !", ObjectManager.Player.ChampionName), 3000);
        }
    }
}
