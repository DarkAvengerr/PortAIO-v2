using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace MasterOfInsec
{
    static class RFlashInsec
    {
        public static void Combo(AIHeroClient target)
        {
            if (Program.menu.Item("OrbwalkFlashInsec").GetValue<bool>())
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Program.Player.Position.Extend(Game.CursorPos, 150));
              var useW = Program.menu.Item("useWardHoop").GetValue<bool>();
              if (MasterOfInsec.Program.R.IsReady())
                  if (useW && WardJump.Insecpos(target).Distance(Program.Player.Position) > 375)
                  {
                      WardJump.wardj = false;
                      WardJump.JumpToFlash(WardJump.InsecposN2(target));
                  }
                if (WardJump.InsecposN2(target).Distance(Program.Player.Position) < 375)
                {
                    if (Program.R.Cast(target)==Spell.CastStates.SuccessfullyCasted)
                    {
                   //     if (Program.R.IsCharging)
                   //     {

                     //   }
LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 50, () => ObjectManager.Player.Spellbook.CastSpell(ObjectManager.Player.GetSpellSlot("SummonerFlash"), WardJump.Insecpos(target)));
                        LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 150, () => qCast(target));
                    }
                }

        }
        public static void qCast(AIHeroClient target)
        {
            if (Program.Q.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne")
            {
                Program.Q.CastIfHitchanceEquals(target, Combos.Combo.HitchanceCheck(Program.menu.Item("seth").GetValue<Slider>().Value));
            }

        }
    }
}
