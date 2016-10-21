using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DarkEzreal.Main
{
    using System;
    using System.Drawing;
    using System.Linq;

    using DarkEzreal.Common;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Utils;

    internal class EventsHandler
    {
        public static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellsManager.Spells.Where(s => Config.DrawMenu.GetBool(s.Slot.ToString())))
            {
                Render.Circle.DrawCircle(Config.Player.Position, spell.Range, spell.IsReady() ? Color.Chartreuse : Color.OrangeRed);
            }
        }

        public static void Events_OnGapCloser(object sender, Events.GapCloserEventArgs e)
        {
            if (sender == null || e.Sender.IsAlly || e.Sender == null || !SpellsManager.E.IsReady() || !Config.MiscMenu["Egap"])
            {
                return;
            }

            SpellsManager.E.Cast(Config.Player.ServerPosition.Extend(e.Sender.ServerPosition, -SpellsManager.E.Range));
        }

        public static void ObjAiBaseOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || sender == null || args.Slot != SpellSlot.W)
            {
                return;
            }

            var epos = Config.Player.ServerPosition.Extend(args.End, 325);
            if (Misc.ComboMode && SpellsManager.E.IsReady() && SpellsManager.E.ManaManager(Config.EMenu["Ec"]) && Config.EMenu["Ec"].GetBool("autoE") && epos.SafetyManager(Config.EMenu["Ec"]))
            {
                DelayAction.Add(100, () => SpellsManager.E.Cast(epos));
            }

            if (Misc.HybridMode && SpellsManager.E.IsReady() && Config.EMenu["Eh"].GetBool("autoE") && epos.SafetyManager(Config.EMenu["Eh"]))
            {
                DelayAction.Add(100, () => SpellsManager.E.Cast(epos));
            }

            if (Config.MiscMenu.GetKeyBind("EW") && SpellsManager.E.IsReady())
            {
                DelayAction.Add(100, () => SpellsManager.E.Cast(epos));
            }
        }

        public static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe || !SpellsManager.E.IsReady() || !Misc.Hooks.Contains(args.Buff.Name.ToLower()) || !Config.MiscMenu.GetBool("hooks"))
            {
                return;
            }

            SpellsManager.E.Cast(Config.Player.ServerPosition.Extend(args.Buff.Caster.Position, -SpellsManager.E.Range));
        }
    }
}
