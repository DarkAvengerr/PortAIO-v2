using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;
using SharpDX;
using System;
using System.Linq;
using Nechrito_Rengar;
using PrideStalker_Rengar.Main;
using PrideStalker_Rengar.Handlers;
using PrideStalker_Rengar.Draw;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace PrideStalker_Rengar
{
    internal class Program : Core
    {
        public static void Main()
        {
            Bootstrap.Init();
            Load();
        }

        private static void Load()
        {
            if (GameObjects.Player.ChampionName != "Rengar")
            {
                return;
            }

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Nechrito Rengar</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Loaded</font></b>");;

            Spells.Load();
            MenuConfig.Load();

            Obj_AI_Base.OnSpellCast += QReset.OnSpellCast;

            Drawing.OnDraw += DRAW.OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if(Player.IsDead || Player.IsRecalling()) return;
            
            DelayAction.Add(600, Mode.ChangeComboMode);
            KillSteal.Killsteal();
            Mode.Skin();

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                switch(MenuConfig.ComboMode.SelectedValue)
                {
                    case "Gank":
                        Mode.Combo();
                        break;
                    case "Triple Q":
                        Mode.TripleQ();
                        break;
                    case "Ap Combo":
                        Mode.ApCombo();
                        break;
                    case "OneShot":
                        Mode.OneShot();
                        break;
                }
            }

            switch(Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.LaneClear:
                    Mode.Lane();
                    Mode.Jungle();
                    break;
                case OrbwalkingMode.LastHit:
                    Mode.LastHit();
                    break;
            }
        }

        private static readonly HpBarDraw Indicator = new HpBarDraw();

        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1500)))
            {
                if (!MenuConfig.Dind) continue;

                Indicator.Unit = enemy;
                Indicator.DrawDmg(Dmg.ComboDmg(enemy), Color.LawnGreen);
            }
        }
    }
}
