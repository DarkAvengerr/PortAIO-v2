using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace MasterOfInsec
{
   static class WardJump
    {
       public static Vector3 posforward;
       public static float lastwardjump = 0;

       private static AIHeroClient Player
       {
           get { return ObjectManager.Player; }
       }

       public static InventorySlot getBestWardItem()
       {
         var ward = Items.GetWardSlot();
           return ward == default(InventorySlot) ? null : ward;
       }
//------------------------------------------------JUMP--------------------------------------------------------------------------------

       public static int LastPlaced = new int();
       public static Vector3 wardPosition = new Vector3();
       public static int SecondWTime = new int();

       public static bool jumped;
      static bool dontread=false;

       public static bool Newjump()
       {
           EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Player.Position.Extend(Game.CursorPos, 150));


           if (Program.W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "BlindMonkWOne")
           {
               wardPosition = Game.CursorPos;
               Obj_AI_Minion Wards;
               if (Game.CursorPos.Distance(Program.Player.Position) <= 700)
               {
                   Wards = ObjectManager.Get<Obj_AI_Minion>().Where(ward => ward.Distance(Game.CursorPos) < 150 && !ward.IsDead).FirstOrDefault();
               }
               else
               {
                   Vector3 cursorPos = Game.CursorPos;
                   Vector3 myPos = Player.ServerPosition;
                   Vector3 delta = cursorPos - myPos;
                   delta.Normalize();
                   wardPosition = myPos + delta * (600 - 5);
                   Wards = ObjectManager.Get<Obj_AI_Minion>().Where(ward => ward.Distance(wardPosition) < 150 && !ward.IsDead).FirstOrDefault();
               }
               if (Wards == null && Items.GetWardSlot() != null)
               {
                   if (jumped == false)
                       if (!wardPosition.IsWall())
                       {
                           InventorySlot invSlot = Items.GetWardSlot();
                           Items.UseItem((int)invSlot.Id, wardPosition);
                           jumped = true;
                       }
               }

               else
                   if (Program.W.CastOnUnit(Wards))
                   {
                       jumped = false;

                   }
           }

           return false;
       }
       public static Vector3 wardpos;
       public static bool wardj;
       public static Vector3 InsecposN2(AIHeroClient ts)
       {
           return Game.CursorPos.Extend(ts.Position, Game.CursorPos.Distance(ts.Position) - 125);
       }
       public static bool JumpToFlash(Vector3 position)
       {
           if (wardj == false)
           {
               wardpos = position;
               wardj = true;
           }
           if (Program.W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "BlindMonkWOne")
           {
               var Wards = ObjectManager.Get<Obj_AI_Minion>().Where(ward => ward.Distance(wardpos) < 150 && !ward.IsDead).FirstOrDefault();
               if (Wards == null)
               {
                   InventorySlot invSlot = Items.GetWardSlot();
                   Items.UseItem((int)invSlot.Id, wardpos);
               }
               Wards = ObjectManager.Get<Obj_AI_Minion>().Where(ward => ward.Distance(wardpos) < 150 && !ward.IsDead).FirstOrDefault();
               if (Program.W.CastOnUnit(Wards))
               {
                   wardj = false;
               }
           }

           return false;
       }

       public static bool JumpTo(Vector3 position)
       {
           if(wardj==false)
           {
               wardpos = position;
               wardj = true;
           }
           if (Program.W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "BlindMonkWOne")
           {
               var Wards = ObjectManager.Get<Obj_AI_Minion>().Where(ward => ward.Distance(wardpos) < 150 && !ward.IsDead).FirstOrDefault();
                     if (Wards == null)
                     {
                         InventorySlot invSlot = Items.GetWardSlot();
                         Items.UseItem((int)invSlot.Id, wardpos);
                     }
                     Wards = ObjectManager.Get<Obj_AI_Minion>().Where(ward => ward.Distance(wardpos) < 150 && !ward.IsDead).FirstOrDefault();
                if(Wards!=null)
                if (Program.W.CastOnUnit(Wards))
               {
                   NormalInsec.Steps = NormalInsec.steps.R;
                   wardj = false;
               }
           }

           return false;
       }
       public static void Wcast(Obj_AI_Base ward)
       {
           if (Program.W.CastOnUnit(ward))
           {
               MasterOfInsec.NormalInsec.Steps = MasterOfInsec.NormalInsec.steps.R;
           }
       }

       public static bool inDistance(Vector2 pos1, Vector2 pos2, float distance)
       {
           float dist2 = Vector2.DistanceSquared(pos1, pos2);
           return (dist2 <= distance * distance) ? true : false;
       }
       public static Obj_AI_Turret getNearTower(AIHeroClient ts)
       {
           return  ObjectManager.Get<Obj_AI_Turret>().Where(tur => tur.IsAlly && tur.Health > 0 && !tur.IsMe).OrderBy(tur => tur.Distance(Player.ServerPosition)).First();
       }
       public static Vector3 Insecpos(AIHeroClient ts)
       {
           return Game.CursorPos.Extend(ts.Position, Game.CursorPos.Distance(ts.Position) + 250);
       }
       public static Vector3 InsecposTower(AIHeroClient target)
       {
           Obj_AI_Turret turret = ObjectManager.Get<Obj_AI_Turret>().Where(tur => tur.IsAlly && tur.Health > 0 && !tur.IsMe).OrderBy(tur => tur.Distance(Player.ServerPosition)).Where(tur => tur.Distance(target.Position) <= 1500).First();
           return target.Position + Vector3.Normalize(turret.Position - target.Position) + 100;

       }
       public static AIHeroClient InsecgetAlly(AIHeroClient target)
       {

           return HeroManager.Allies
                    .FindAll(hero => hero.Distance(Game.CursorPos, true) < 40000) // 200 * 200
                    .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();
       }
       public static Vector3 InsecposToAlly(AIHeroClient target,AIHeroClient ally)
       {
           return ally.Position.Extend(target.Position ,ally.Position.Distance(target.Position)+250);
       }
       public static Vector3 getward(AIHeroClient target)
       {
           Obj_AI_Turret turret = ObjectManager.Get<Obj_AI_Turret>().Where(tur => tur.IsAlly && tur.Health > 0 && !tur.IsMe).OrderBy(tur => tur.Distance(Player.ServerPosition)).First();
           return target.ServerPosition + Vector3.Normalize(turret.ServerPosition - target.ServerPosition) * (-300);
       }
       public static bool putWard(Vector2 pos)
       {
           InventorySlot invSlot = getBestWardItem();
           Items.UseItem((int)invSlot.Id, pos);
           return true;
       }
       public static void moveTo(Vector2 Pos)
       {
           EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Pos.To3D());
       }


    }
}
