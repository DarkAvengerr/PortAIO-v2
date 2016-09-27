using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using ShineCommon;
//typedefs
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace MoonDiana
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
            if (ObjectManager.Player.ChampionName.ToLowerInvariant() != "diana")
                return;

            Champion = new Diana();

            Champion.CreateConfigMenu();
            Champion.SetSpells();

            if (Champion.Spells[0] != null && Champion.Spells[0].Range > 0)
                Champion.drawing.AddItem(new MenuItem("DDRAWQ", "Draw Q").SetValue(new Circle(true, Color.Red, Champion.Spells[0].Range)));

            if (Champion.Spells[1] != null && Champion.Spells[1].Range > 0)
                Champion.drawing.AddItem(new MenuItem("DDRAWW", "Draw W").SetValue(new Circle(true, Color.Aqua, Champion.Spells[1].Range)));

            if (Champion.Spells[2] != null && Champion.Spells[2].Range > 0)
                Champion.drawing.AddItem(new MenuItem("DDRAWE", "Draw E").SetValue(new Circle(true, Color.Bisque, Champion.Spells[2].Range)));

            if (Champion.Spells[3] != null && Champion.Spells[3].Range > 0 && Champion.Spells[3].Range < 3000) //global ult ?
                Champion.drawing.AddItem(new MenuItem("DDRAWR", "Draw R").SetValue(new Circle(true, Color.Chartreuse, Champion.Spells[3].Range)));

            #region Events
            Game.OnUpdate += Champion.Game_OnUpdate;
            Drawing.OnDraw += Champion.Drawing_OnDraw;                                                                                          
            //MoonDiana.Orbwalking.BeforeAttack += Champion.Orbwalking_BeforeAttack;
            MoonDiana.Orbwalking.AfterAttack += Champion.Orbwalking_AfterAttack;
            AntiGapcloser.OnEnemyGapcloser += Champion.AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Champion.Interrupter_OnPossibleToInterrupt;
            Obj_AI_Base.OnBuffGain += Champion.Obj_AI_Base_OnBuffAdd;
            Obj_AI_Base.OnProcessSpellCast += Champion.Obj_AI_Base_OnProcessSpellCast;
            #endregion

            SPrediction.Prediction.Initialize(Champion.Config);
            
            Notifications.AddNotification(String.Format("Moon Diana Loaded !"), 3000);
        }
    }
}
