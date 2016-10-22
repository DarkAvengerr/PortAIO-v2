using EloBuddy; 
 using LeagueSharp.Common; 
 namespace xSaliceResurrected_Rework.Utilities
{
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Base;

    public class WardJumper : SpellBase
    {
        public static int LastPlaced;
        public static Vector3 LastWardPos;
        private static readonly AIHeroClient Player = ObjectManager.Player;

        public static void JumpKs(AIHeroClient target)
        {
            foreach (var ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
                E.IsReady() && Q.IsReady() && ward.Name.ToLower().Contains("ward") &&
                ward.Distance(target.ServerPosition) < Q.Range && ward.Distance(Player.Position) < E.Range))
            {
                E.Cast(ward);
                return;
            }

            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero =>
                E.IsReady() && Q.IsReady() && hero.Distance(target.ServerPosition) < Q.Range &&
                hero.Distance(Player.Position) < E.Range && hero.IsValidTarget(E.Range)))
            {
                E.Cast(hero);
                return;
            }

            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion =>
                E.IsReady() && Q.IsReady() && minion.Distance(target.ServerPosition) < Q.Range &&
                minion.Distance(Player.Position) < E.Range && minion.IsValidTarget(E.Range)))
            {
                E.Cast(minion);
                return;
            }

            if (Player.Distance(target.Position) < Q.Range)
            {
                Q.Cast(target);
                return;
            }

            if (E.IsReady() && Q.IsReady())
            {
                var position = Player.ServerPosition +
                                   Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 590;

                if (target.Distance(position) < Q.Range)
                {
                    var invSlot = FindBestWardItem();

                    if (invSlot == null)
                    {
                        return;
                    }

                    Player.Spellbook.CastSpell(invSlot.SpellSlot, position);
                    LastWardPos = position;
                    LastPlaced = Utils.TickCount;
                }
            }

            if (Player.Distance(target.Position) < Q.Range)
            {
                Q.Cast(target);
            }
        }

        public static void WardJump()
        {
            foreach (var ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
                ward.Name.ToLower().Contains("ward") && ward.Distance(Game.CursorPos) < 250))
            {
                if (E.IsReady())
                {
                    E.Cast(ward);
                    return;
                }
            }

            foreach (
                var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.Distance(Game.CursorPos) < 250 && !hero.IsDead))
            {
                if (E.IsReady())
                {
                    E.Cast(hero);
                    return;
                }
            }

            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion =>
                minion.Distance(Game.CursorPos) < 250))
            {
                if (E.IsReady())
                {
                    E.Cast(minion);
                    return;
                }
            }

            if (Utils.TickCount <= LastPlaced + 3000 || !E.IsReady())
            {
                return;
            }

            var cursorPos = Game.CursorPos;
            var myPos = Player.ServerPosition;
            var delta = cursorPos - myPos;

            delta.Normalize();

            var wardPosition = myPos + delta * (600 - 5);
            var invSlot = FindBestWardItem();

            if (invSlot == null)
            {
                return;
            }

            Items.UseItem((int)invSlot.Id, wardPosition);
            LastWardPos = wardPosition;
            LastPlaced = Utils.TickCount;
        }

        private static InventorySlot FindBestWardItem()
        {
            var slot = Items.GetWardSlot();

            return slot == default(InventorySlot) ? null : slot;
        }
    }
}
