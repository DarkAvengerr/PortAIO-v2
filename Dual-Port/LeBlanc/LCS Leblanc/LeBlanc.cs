using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCS_LeBlanc.Extensions;
using LCS_LeBlanc.Modes;
using LCS_LeBlanc.Modes.Combo;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using Utilities = LCS_LeBlanc.Extensions.Utilities;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace LCS_LeBlanc
{
    public class LeBlanc
    {
        public LeBlanc()
        {
            OnLoad();
        }

        private static Vector3 WBackPosition;
        private static void OnLoad()
        {
            Spells.Initialize();
            Menus.Initialize();

            Game.OnUpdate += OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += OnGapcloser;
            GameObject.OnCreate += OnCreate;
        }


        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsValid && sender.Name.Contains("return_indicator") && sender.IsVisible)
            {
                WBackPosition = sender.Position;
            }
            else
            {
                WBackPosition = new Vector3(0,0,0);
            }
            
        }

        private static void OnGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsEnemy && gapcloser.Sender.IsValidTarget(Spells.E.Range) &&
                (gapcloser.Sender.LastCastedSpellTarget().IsMe || ObjectManager.Player.Distance(gapcloser.End) < 100) && Spells.E.IsReady() // unsure
                && Utilities.Enabled("anti-gapcloser.e"))
            {
                Spells.E.Cast(gapcloser.Sender.Position);
            }
        }

        private static void ComboSelector()
        {
            switch (Menus.Config.Item("combo.mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    QRWE.Init();
                    break;
                case 1:
                    WRQE.WRQECombo();
                    break;
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            Utilities.UpdateUltimateVariable();

            switch (Menus.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    ComboSelector();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed.Init();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear.WaveInit();
                    Clear.JungleInit();
                    break;
            }
        }

    }
}
