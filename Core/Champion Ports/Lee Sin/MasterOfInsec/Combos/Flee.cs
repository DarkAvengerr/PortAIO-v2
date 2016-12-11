using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
using LeagueSharp.Common; 
namespace MasterOfInsec.Combos
{
    public static class Flee
    {
    public static void Do()
    {
        var useQ = Program.menu.Item("QFlee").GetValue<bool>();
        var useR = Program.menu.Item("RFlee").GetValue<bool>();
        var useRtarget = Program.menu.Item("RTarget").GetValue<bool>();
        Obj_AI_Base targetR;
     //   TargetSelector.SelectedTarget;
     //   if (target)
        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Program.Player.Position.Extend(Game.CursorPos, 150));
  
        if(useQ)
        {
     try{
         Obj_AI_Base minion = ObjectManager.Get<Obj_AI_Base>().Where(x => (x.IsEnemy|| x.IsPacified) && Program.Q.GetDamage(x) < x.Health && Program.Q.IsInRange(x) && Program.Q.CanCast(x)).MinOrDefault(x => x.Distance(Game.CursorPos) < x.Distance(Program.Player));
         Program.Q.CastIfHitchanceEquals(
                minion,
                 Combos.Combo.HitchanceCheck(Program.menu.Item("seth").GetValue<Slider>().Value));
        if (Program.Q.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne")
        {
            Program.Q.Cast();
        }
    }
         catch(Exception e){}
        }
        if (useR)
       {
           if (useRtarget == false)
           {

               targetR = TargetSelector.GetTarget(Program.R.Range, TargetSelector.DamageType.Physical);

           }
           else targetR = TargetSelector.SelectedTarget;
           if(Program.R.CanCast(targetR))
           {
               Program.R.Cast(targetR);
           }
       }
       
    
    
    }
        
    }
}
