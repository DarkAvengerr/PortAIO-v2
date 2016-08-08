using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSaliceResurrected.Base;

using EloBuddy; namespace xSaliceResurrected.Utilities
{
    class WardJumper : SpellBase
    {
        //items
        public static int LastPlaced;
        public static Vector3 LastWardPos;
        private static readonly AIHeroClient Player = ObjectManager.Player;

        public static void JumpKs(AIHeroClient target)
        {
            foreach (Obj_AI_Minion ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
                E.LSIsReady() && Q.LSIsReady() && ward.Name.ToLower().Contains("ward") &&
                ward.LSDistance(target.ServerPosition) < Q.Range && ward.LSDistance(Player.Position) < E.Range))
            {
                E.Cast(ward);
                return;
            }

            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>().Where(hero =>
                E.LSIsReady() && Q.LSIsReady() && hero.LSDistance(target.ServerPosition) < Q.Range &&
                hero.LSDistance(Player.Position) < E.Range && hero.LSIsValidTarget(E.Range)))
            {
                E.Cast(hero);
                return;
            }

            foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion =>
                E.LSIsReady() && Q.LSIsReady() && minion.LSDistance(target.ServerPosition) < Q.Range &&
                minion.LSDistance(Player.Position) < E.Range && minion.LSIsValidTarget(E.Range)))
            {
                E.Cast(minion);
                return;
            }

            if (Player.LSDistance(target.Position) < Q.Range)
            {
                Q.Cast(target);
                return;
            }

            if (E.LSIsReady() && Q.LSIsReady())
            {
                Vector3 position = Player.ServerPosition +
                                   Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 590;

                if (target.LSDistance(position) < Q.Range)
                {
                    InventorySlot invSlot = FindBestWardItem();
                    if (invSlot == null) return;

                    Player.Spellbook.CastSpell(invSlot.SpellSlot, position);
                    LastWardPos = position;
                    LastPlaced = Utils.TickCount;
                }
            }

            if (Player.LSDistance(target.Position) < Q.Range)
            {
                Q.Cast(target);
            }
        }

        public static void WardJump()
        {
            //wardWalk(Game.CursorPos);

            foreach (Obj_AI_Minion ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
                ward.Name.ToLower().Contains("ward") && ward.LSDistance(Game.CursorPos) < 250))
            {
                if (E.LSIsReady())
                {
                    E.Cast(ward);
                    return;
                }
            }

            foreach (
                AIHeroClient hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.LSDistance(Game.CursorPos) < 250 && !hero.IsDead))
            {
                if (E.LSIsReady())
                {
                    E.Cast(hero);
                    return;
                }
            }

            foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion =>
                minion.LSDistance(Game.CursorPos) < 250))
            {
                if (E.LSIsReady())
                {
                    E.Cast(minion);
                    return;
                }
            }

            if (Utils.TickCount <= LastPlaced + 3000 || !E.LSIsReady()) return;

            Vector3 cursorPos = Game.CursorPos;
            Vector3 myPos = Player.ServerPosition;

            Vector3 delta = cursorPos - myPos;
            delta.Normalize();

            Vector3 wardPosition = myPos + delta * (600 - 5);

            InventorySlot invSlot = FindBestWardItem();
            if (invSlot == null) return;

            Items.UseItem((int)invSlot.Id, wardPosition);
            LastWardPos = wardPosition;
            LastPlaced = Utils.TickCount;
        }

        private static InventorySlot FindBestWardItem()
        {
            InventorySlot slot = Items.GetWardSlot();
            if (slot == default(InventorySlot)) return null;
            return slot;
        }
    }
}
