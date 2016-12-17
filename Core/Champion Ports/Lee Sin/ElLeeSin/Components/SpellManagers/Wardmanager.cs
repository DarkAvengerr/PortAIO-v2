using EloBuddy; 
using LeagueSharp.Common; 
namespace ElLeeSin.Components.SpellManagers
{
    using System;
    using System.Linq;

    using ElLeeSin.Utilities;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class Wardmanager
    {
        #region Public Methods and Operators

        public static Vector2 JumpPos;

        public static bool reCheckWard = true;

        public static int Wcasttime;

        public static Vector3 lastWardPos;

        public static float LastWard;

        public static bool castWardAgain = true;


        public enum WCastStage
        {
            First,

            Second,

            Cooldown
        }

        private static SpellDataInst GetItemSpell(InventorySlot invSlot)
        {
            return ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => (int)spell.Slot == invSlot.Slot + 4);
        }

        public static InventorySlot FindBestWardItem()
        {
            try
            {
                var slot = Items.GetWardSlot();
                if (slot == default(InventorySlot))
                {
                    return null;
                }

                var sdi = GetItemSpell(slot);
                if ((sdi != default(SpellDataInst)) && (sdi.State == SpellState.Ready))
                {
                    return slot;
                }
                return slot;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        public static void WardJump(
            Vector3 pos,
            bool m2M = true,
            bool maxRange = false,
            bool reqinMaxRange = false,
            bool minions = true,
            bool champions = true)
        {
            if (Misc.WStage != WCastStage.First)
            {
                return;
            }

            var basePos = ObjectManager.Player.Position.To2D();
            var newPos = pos.To2D() - ObjectManager.Player.Position.To2D();

            if (JumpPos == new Vector2())
            {
                if (reqinMaxRange)
                {
                    JumpPos = pos.To2D();
                }
                else if (maxRange || (ObjectManager.Player.Distance(pos) > 590))
                {
                    JumpPos = basePos + newPos.Normalized() * 590;
                }
                else
                {
                    JumpPos = basePos + newPos.Normalized() * ObjectManager.Player.Distance(pos);
                }
            }
            if ((JumpPos != new Vector2()) && reCheckWard)
            {
                reCheckWard = false;
                LeagueSharp.Common.Utility.DelayAction.Add(
                    20,
                    () =>
                        {
                            if (JumpPos != new Vector2())
                            {
                                JumpPos = new Vector2();
                                reCheckWard = true;
                            }
                        });
            }
            if (m2M)
            {
                Misc.Orbwalk(pos);
            }
            if (!LeeSin.spells[LeeSin.Spells.W].IsReady() || (Misc.WStage != WCastStage.First)
                || (reqinMaxRange && (ObjectManager.Player.Distance(pos) >LeeSin.spells[LeeSin.Spells.W].Range)))
            {
                return;
            }

            if (minions || champions)
            {
                if (champions)
                {
                    var wardJumpableChampion =
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                x =>
                                    x.IsAlly && (x.Distance(ObjectManager.Player) < LeeSin.spells[LeeSin.Spells.W].Range)
                                    && (x.Distance(pos) < 200) && !x.IsMe)
                            .OrderByDescending(i => i.Distance(ObjectManager.Player))
                            .ToList()
                            .FirstOrDefault();

                    if ((wardJumpableChampion != null) && (Misc.WStage == WCastStage.First))
                    {
                        if ((500 >= Utils.TickCount - Wcasttime) || (Misc.WStage != WCastStage.First))
                        {
                            return;
                        }

                        LeeSin.CastW(wardJumpableChampion);
                        return;
                    }
                }
                if (minions)
                {
                    var wardJumpableMinion =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                m =>
                                    m.IsAlly && (m.Distance(ObjectManager.Player) < LeeSin.spells[LeeSin.Spells.W].Range)
                                    && (m.Distance(pos) < 200) && !m.Name.ToLower().Contains("ward"))
                            .OrderByDescending(i => i.Distance(ObjectManager.Player))
                            .ToList()
                            .FirstOrDefault();

                    if ((wardJumpableMinion != null) && (Misc.WStage == WCastStage.First))
                    {
                        if ((500 >= Utils.TickCount - Wcasttime) || (Misc.WStage != WCastStage.First))
                        {
                            return;
                        }

                        LeeSin.CastW(wardJumpableMinion);
                        return;
                    }
                }
            }

            var isWard = false;

            var wardObject =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(o => o.IsAlly && o.Name.ToLower().Contains("ward") && (o.Distance(JumpPos) < 200))
                    .ToList()
                    .FirstOrDefault();

            if (wardObject != null)
            {
                isWard = true;
                if ((500 >= Utils.TickCount - Wcasttime) || (Misc.WStage != WCastStage.First))
                {
                    return;
                }

                LeeSin.CastW(wardObject);
            }

            if (!isWard && castWardAgain)
            {
                var ward = FindBestWardItem();
                if (ward == null)
                {
                    return;
                }

                if (LeeSin.spells[LeeSin.Spells.W].IsReady() && Misc.IsWOne && (LastWard + 400 < Utils.TickCount))
                {
                    ObjectManager.Player.Spellbook.CastSpell(ward.SpellSlot, JumpPos.To3D());
                    lastWardPos = JumpPos.To3D();
                    LastWard = Utils.TickCount;
                }
            }
        }

        public static void WardjumpToMouse()
        {
            WardJump(
                Game.CursorPos,
                Misc.GetMenuItem("ElLeeSin.Wardjump.Mouse"),
                false,
                false,
                Misc.GetMenuItem("ElLeeSin.Wardjump.Minions"),
                Misc.GetMenuItem("ElLeeSin.Wardjump.Champions"));
        }

        #endregion
    }
}