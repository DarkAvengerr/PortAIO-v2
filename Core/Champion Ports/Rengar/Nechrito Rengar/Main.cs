using System;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using Nechrito_Rengar.Main;
using Nechrito_Rengar.Handlers;
using Nechrito_Rengar.Drawings;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Rengar
{
    class MAIN : Core
    {
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Rengar")
                return;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Nechrito Rengar</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Version: 5 (Date: 5/5/2016)</font></b>");
            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Update</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Completely Re-written</font></b>");
            Game.OnUpdate += OnUpdate;
            Game.OnUpdate += Modes.Game_OnUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Drawing.OnDraw += DRAWING.Drawing_OnDraw;
            MenuConfig.Load();
            Champion.Load();
        }
        private static void OnUpdate(EventArgs args)
        {

            //   var target = TargetSelector.GetSelectedTarget();
            //    Chat.Print("Buffs: {0}", string.Join(" | ", Player.Buffs.Where(b => b.Caster.NetworkId == Player.NetworkId).Select(b => b.DisplayName)));
           
            ITEMS.SmiteJungle();
            Killsteal.KillSteal();
            if (Orb.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                switch (MenuConfig.Menu.Item("ComboMode").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        Modes.Combo();
                        ITEMS.SmiteCombo();
                        break;
                    case 1:
                        ITEMS.SmiteCombo();
                        Modes.TripleQ();
                        break;
                    case 2:
                        ITEMS.SmiteCombo();
                        Modes.ApCombo();
                        break;
                }
            }
            
            switch (Orb.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Modes.Lane();
                    Modes.Jungle();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    
                    break;

            }
        }
        
        private static void Drawing_OnEndScene(EventArgs args)
        {
        }
    }
}
