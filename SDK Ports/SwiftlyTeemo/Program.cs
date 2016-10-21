#region

using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using SharpDX;
using System;
using System.Linq;
using Swiftly_Teemo.Draw;
using Swiftly_Teemo.Handler;
using Swiftly_Teemo.Menu;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Swiftly_Teemo
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
            if (GameObjects.Player.ChampionName != "Teemo")
            {
                Chat.Print("Failed to load Swiftly Teemo!");
                return;
            }

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Swiftly Teemo</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Version: 5</font></b>");
            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Update</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Compiling Error</font></b>");

             Spells.Load();
             MenuConfig.Load();

            Drawing.OnDraw += Drawings.OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;

            Obj_AI_Base.OnSpellCast += ModeHandler.OnSpellCast;
            
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling()) return;

            Killsteal.KillSteal();
            Mode.Skin();
            Mode.Flee();
          
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.LaneClear:
                    Mode.Jungle();
                    break;
                case OrbwalkingMode.Combo:
                   Mode.Combo();
                    break;
            }
            
        }

        private static readonly HpBarIndicator Indicator = new HpBarIndicator();

        private static void Drawing_OnEndScene(EventArgs args)
        {
           foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1500) && !ene.IsDead && ene.IsVisible))
            {
                Indicator.Unit = enemy;
                Indicator.DrawDmg(Dmg.ComboDmg(enemy), enemy.Health <= Dmg.ComboDmg(enemy) * 1.65 ? Color.LawnGreen : Color.Yellow);
            }
        }
    }
}
