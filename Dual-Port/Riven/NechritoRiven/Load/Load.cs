#region

using LeagueSharp;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Draw;
using NechritoRiven.Event;
using NechritoRiven.Menus;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Load
{
    internal class Load
    {
        public static void LoadAssembly()
        {
            MenuConfig.LoadMenu();
            Spells.Load();

            Obj_AI_Base.OnProcessSpellCast += OnCasted.OnCasting;
            Obj_AI_Base.OnSpellCast += Modes.OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast += Core.Core.OnCast;
            Obj_AI_Base.OnPlayAnimation += Anim.OnPlay;

            Drawing.OnEndScene += DrawDmg.DmgDraw;
            Drawing.OnDraw += DrawRange.RangeDraw;
            Drawing.OnDraw += DrawWallSpot.WallDraw;

            Game.OnUpdate += KillSteal.Update;
            Game.OnUpdate += AlwaysUpdate.Update;
            Game.OnUpdate += Skinchanger.Update;

            Interrupter2.OnInterruptableTarget += Interrupt2.OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += Gapclose.Gapcloser;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Nechrito Riven</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Loaded!</font></b>");
        }
    }
}
