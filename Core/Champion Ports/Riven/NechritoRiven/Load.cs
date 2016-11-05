using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven
{
    #region

    using System;

    using Core;

    using Draw;

    using Event.OrbwalkingModes;

    using LeagueSharp;
    using LeagueSharp.Common;

    using NechritoRiven.Event.Animation;
    using NechritoRiven.Event.Interrupters_Etc;
    using NechritoRiven.Event.Misc;

    #endregion

    internal class Load
    {
        #region Public Methods and Operators

        public static void LoadAssembly()
        {
            MenuConfig.LoadMenu();
            Spells.Load();

            Obj_AI_Base.OnSpellCast += AfterAuto.OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast += ProcessSpell.OnProcessSpell;
            Obj_AI_Base.OnProcessSpellCast += BackgroundData.OnCast;
            Obj_AI_Base.OnPlayAnimation += Animation.OnPlay;

            Drawing.OnEndScene += DrawDmg.DmgDraw;
            Drawing.OnDraw += DrawMisc.RangeDraw;
            Drawing.OnDraw += DrawWallSpot.WallDraw;

            Game.OnUpdate += KillSteal.Update;
            Game.OnUpdate += PermaActive.Update;
            Game.OnUpdate += Skinchanger.Update;

            Interrupter2.OnInterruptableTarget += Interrupt2.OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += Gapclose.Gapcloser;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Nechrito Riven</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Loaded!</font></b>");
            Console.WriteLine("Nechrito Riven: Loaded");
        }

        #endregion
    }
}