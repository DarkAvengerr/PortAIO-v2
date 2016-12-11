using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Manager.Events.Games.Mode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Orbwalking = Orbwalking;
    using Common;
    using SharpDX;
    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class Ward : Logic
    {
        internal static void Init()
        {
            if (Menu.Item("AutoWardEnable", true).GetValue<bool>() &&
                ((Menu.Item("OnlyCombo", true).GetValue<bool>() &&
                  Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) ||
                 !Menu.Item("OnlyCombo", true).GetValue<bool>()))
            {
                return;
            }

            if (Utils.TickCount - lastCheckTime < 400)
            {
                return;
            }

            lastCheckTime = Utils.TickCount;

            foreach (var obj in championObject.Where(c => c.Hero.IsVisible))
            {
                obj.LastSeen = Game.Time;
            }

            foreach (var obj in
                championObject.Where(c =>
                    !c.Hero.IsVisible && !c.Hero.IsDead && Game.Time - c.LastSeen <= 2 &&
                    c.Hero.Distance(Me) < 1000))
            {
                var pos = GetWardPos(obj.Hero.ServerPosition, 165, 2);

                if (!pos.Equals(obj.Hero.ServerPosition) && !pos.Equals(Vector3.Zero))
                {
                    CastWard(pos, true);
                }
            }
        }

        private static void CastWard(Vector3 pos, bool bush)
        {
            if (pos.Distance(ObjectManager.Player.Position) > (!bush ? 600 + 200 : 600) ||
                lastWardCast + 2f > Game.Time)
            {
                return;
            }

            var slot = GetRevealSlot(bush);

            if (slot != SpellSlot.Unknown)
            {
                ObjectManager.Player.Spellbook.CastSpell(slot, ObjectManager.Player.Position.Extend(pos, 600));
                lastWardCast = Game.Time;
            }
        }

        private static SpellSlot GetRevealSlot(bool InBush = false)
        {
            if (!InBush)
            {
                if (ItemData.Vision_Ward.GetItem().IsOwned() && ItemData.Vision_Ward.GetItem().IsReady())
                {
                    return ItemData.Vision_Ward.GetItem().Slots.FirstOrDefault();
                }
                if (ItemData.Oracle_Alteration.GetItem().IsOwned() && ItemData.Oracle_Alteration.GetItem().IsReady())
                {
                    return ItemData.Oracle_Alteration.GetItem().Slots.FirstOrDefault();
                }
                if (ItemData.Sweeping_Lens_Trinket.GetItem().IsOwned() &&
                    ItemData.Sweeping_Lens_Trinket.GetItem().IsReady())
                {
                    return ItemData.Sweeping_Lens_Trinket.GetItem().Slots.FirstOrDefault();
                }
            }
            else
            {
                if (ItemData.Trackers_Knife.GetItem().IsOwned() && ItemData.Trackers_Knife.GetItem().IsReady())
                {
                    return ItemData.Trackers_Knife.GetItem().Slots.FirstOrDefault();
                }
                if (ItemData.Sightstone.GetItem().IsOwned() && ItemData.Sightstone.GetItem().IsReady())
                {
                    return ItemData.Sightstone.GetItem().Slots.FirstOrDefault();
                }
                if (ItemData.Ruby_Sightstone.GetItem().IsOwned() && ItemData.Ruby_Sightstone.GetItem().IsReady())
                {
                    return ItemData.Ruby_Sightstone.GetItem().Slots.FirstOrDefault();
                }
                if (ItemData.Eye_of_the_Watchers.GetItem().IsOwned() &&
                    ItemData.Eye_of_the_Watchers.GetItem().IsReady())
                {
                    return ItemData.Eye_of_the_Watchers.GetItem().Slots.FirstOrDefault();
                }
                if (ItemData.Eye_of_the_Equinox.GetItem().IsOwned() &&
                    ItemData.Eye_of_the_Equinox.GetItem().IsReady())
                {
                    return ItemData.Eye_of_the_Equinox.GetItem().Slots.FirstOrDefault();
                }
                if (ItemData.Eye_of_the_Oasis.GetItem().IsOwned() && ItemData.Eye_of_the_Oasis.GetItem().IsReady())
                {
                    return ItemData.Eye_of_the_Oasis.GetItem().Slots.FirstOrDefault();
                }
                if (ItemData.Warding_Totem_Trinket.GetItem().IsOwned() &&
                    ItemData.Warding_Totem_Trinket.GetItem().IsReady())
                {
                    return ItemData.Warding_Totem_Trinket.GetItem().Slots.FirstOrDefault();
                }
            }

            return SpellSlot.Unknown;
        }

        private static Vector3 GetWardPos(Vector3 lastPos, int radius = 165, int precision = 3)
        {
            var count = precision;

            while (count > 0)
            {
                var vertices = radius;
                var wardLocations = new WardLocation[vertices];
                var angle = 2 * Math.PI / vertices;

                for (var i = 0; i < vertices; i++)
                {
                    var th = angle * i;
                    var pos = new Vector3(
                        (float)(lastPos.X + radius * Math.Cos(th)), (float)(lastPos.Y + radius * Math.Sin(th)), 0);

                    wardLocations[i] = new WardLocation(pos, NavMesh.IsWallOfGrass(pos, 10));
                }

                var grassLocations = new List<GrassLocation>();

                for (var i = 0; i < wardLocations.Length; i++)
                {
                    if (wardLocations[i].Grass)
                    {
                        if (i != 0 && wardLocations[i - 1].Grass)
                        {
                            grassLocations.Last().Count++;
                        }
                        else
                        {
                            grassLocations.Add(new GrassLocation(i, 1));
                        }
                    }
                }

                var grassLocation = grassLocations.OrderByDescending(x => x.Count).FirstOrDefault();

                if (grassLocation != null)
                {
                    var midelement = (int)Math.Ceiling(grassLocation.Count / 2f);

                    lastPos = wardLocations[grassLocation.Index + midelement - 1].Pos;
                    radius = (int)Math.Floor(radius / 2f);
                }

                count--;
            }

            return lastPos;
        }
    }
}
