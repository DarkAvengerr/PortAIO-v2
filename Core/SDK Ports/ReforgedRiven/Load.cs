#region

using LeagueSharp;
using LeagueSharp.SDK;
using Reforged_Riven.Draw;
using Reforged_Riven.Extras;
using Reforged_Riven.Menu;
using Reforged_Riven.Update;
using Reforged_Riven.Update.Process;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Reforged_Riven
{
    internal class Load
    {
        public static void LoadAssembly()
        {
            MenuConfig.Load();
            Spells.Load();

            AssemblyVersion.CheckVersion();

            Events.OnInterruptableTarget += Interrupt.OnInterruptableTarget;

            Obj_AI_Base.OnSpellCast += ModeHandler.OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast += Logic.OnCast;
            Obj_AI_Base.OnProcessSpellCast += AntiSpell.OnCasting;
            Obj_AI_Base.OnPlayAnimation += Animation.OnPlay;

            Drawing.OnEndScene += DrawDmg.DmgDraw;
            Drawing.OnDraw += SpellRange.Draw;

            Game.OnUpdate += KillSteal.Update;
            Game.OnUpdate += PermaActive.Update;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reforged Riven</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Loaded</font></b>");
        }
    }
}
