using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
namespace BadaoKingdom.BadaoChampion.BadaoGraves
{
    using static BadaoGravesVariables;
    using static BadaoMainVariables;
    public static class BadaoGraves
    {
        public static void BadaoActivate()
        {
            BadaoGravesConfig.BadaoActivate();
            BadaoGravesCombo.BadaoActivate();
            BadaoGravesBurst.BadaoActivate();
            BadaoGravesAuto.BadaoActivate();
            BadaoGravesJungle.BadaoActivate();
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            //Game.OnUpdate += Game_OnUpdate;
        }

        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None && !BurstKey.GetValue<KeyBind>().Active)
                return;
            if (!sender.IsMe)
                return;
            if (args.Order != GameObjectOrder.AttackUnit)
                return;
            if (args.Target == null)
                return;
            if (!(args.Target is Obj_AI_Base))
                return;
            if (Player.Position.To2D().Distance(args.Target.Position.To2D()) > Player.BoundingRadius + Player.AttackRange + args.Target.BoundingRadius - 20 && E.IsReady())
            {
                args.Process = false;
                return;
            }
            if (!BadaoGravesHelper.CanAttack() && E.IsReady())
            {
                args.Process = false;
                return;
            }
            BadaoGravesCombo.Obj_AI_Base_OnIssueOrder(sender, args);
            BadaoGravesBurst.Obj_AI_Base_OnIssueOrder(sender, args);
            BadaoGravesJungle.Obj_AI_Base_OnIssueOrder(sender, args);
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.E)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(0, () => Orbwalking.ResetAutoAttackTimer());
            }
        }
    }
}
