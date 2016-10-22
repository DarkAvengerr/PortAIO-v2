using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Utility
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using System.Linq;
    using System.Collections.Generic;
    using Orbwalking = Orbwalking;
    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class AutoWard : Program //This Part From SFX Utility 
    {
        private const float CheckInterval = 333f;
        private const float MaxRange = 600f;
        private const float Delay = 2f;

        private static float lastCheckTime = Environment.TickCount;
        private static float lastRevealTime;

        private static readonly List<ChampionObject> championObject = new List<ChampionObject>();

        private static readonly HashSet<SpellData> SpellList = new HashSet<SpellData>
        {
            new SpellData("Akali", SpellSlot.W),
            new SpellData("Rengar", SpellSlot.R, true),
            new SpellData("KhaZix", SpellSlot.R),
            new SpellData("KhaZix", SpellSlot.R, false, "khazixrlong"),
            new SpellData("Monkeyking", SpellSlot.W),
            new SpellData("Shaco", SpellSlot.Q),
            new SpellData("Talon", SpellSlot.R),
            new SpellData("Vayne", SpellSlot.Q, true),
            new SpellData("Twitch", SpellSlot.Q)
        };

        private new static readonly Menu Menu = Utilitymenu;

        internal static void Init()
        {
            var AutoWardMenu = Menu.AddSubMenu(new Menu("Auto Ward", "Auto Ward"));
            {
                AutoWardMenu.AddItem(new MenuItem("AutoWardEnable", "Enabled", true).SetValue(true));
                AutoWardMenu.AddItem(new MenuItem("AutoBush", "When target In Bush", true).SetValue(true));
                AutoWardMenu.AddItem(new MenuItem("AutoPink", "Auto Pink Ward", true).SetValue(true));
                AutoWardMenu.AddItem(new MenuItem("OnlyCombo", "Only Combo Mode Active", true).SetValue(true));
            }

            GameObject.OnCreate += OnCreate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
        }

        private static bool IsActive
            =>
            Menu.Item("AutoWardEnable", true).GetValue<bool>() &&
            ((Menu.Item("OnlyCombo", true).GetValue<bool>() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) ||
             !Menu.Item("OnlyCombo", true).GetValue<bool>());

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (!IsActive)
            {
                return;
            }

            if (!sender.IsEnemy)
            {
                return;
            }

            if (!Menu.Item("AutoPink", true).GetValue<bool>())
            {
                return;
            }

            var Rengar = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Rengar"));

            if (Rengar != null)
            {
                if (sender.Name.Contains("Rengar_Base_R_Alert"))
                {
                    if (ObjectManager.Player.HasBuff("rengarralertsound") && !Rengar.IsVisible && !Rengar.IsDead)
                    {
                        CastWard(ObjectManager.Player.Position, false);
                    }
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!IsActive)
            {
                return;
            }

            var hero = sender as AIHeroClient;

            if (!sender.IsEnemy || hero == null)
            {
                return;
            }

            var spell =
                SpellList.FirstOrDefault(
                    s =>
                        !string.IsNullOrEmpty(s.Name) &&
                        s.Name.Equals(Args.SData.Name, StringComparison.OrdinalIgnoreCase));

            if (spell != null && Menu.Item("AutoPink", true).GetValue<bool>())
            {
                if (!spell.Custom)
                {
                    CastWard(Args.End, false);
                }

                var Vayne = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Vayne"));

                if (Vayne != null &&
                    spell.Hero.Equals(Vayne.ChampionName, StringComparison.OrdinalIgnoreCase))
                {
                    var buff =
                        Vayne.Buffs.FirstOrDefault(
                            b => b.Name.Equals("VayneInquisition", StringComparison.OrdinalIgnoreCase));

                    if (buff != null)
                    {
                        CastWard(Args.End, false);
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!IsActive)
            {
                return;
            }

            if (!Menu.Item("AutoBush", true).GetValue<bool>())
            {
                return;
            }

            if (lastCheckTime + CheckInterval > Environment.TickCount)
            {
                return;
            }

            lastCheckTime = Environment.TickCount;

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
            if (pos.Distance(ObjectManager.Player.Position) > (!bush ? MaxRange + 200 : MaxRange) ||
                lastRevealTime + Delay > Game.Time)
            {
                return;
            }

            var slot = GetRevealSlot(bush);

            if (slot != SpellSlot.Unknown)
            {
                ObjectManager.Player.Spellbook.CastSpell(slot, ObjectManager.Player.Position.Extend(pos, MaxRange));
                lastRevealTime = Game.Time;
            }
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

        internal class SpellData
        {
            public SpellData(string hero, SpellSlot slot, bool custom = false, string name = null)
            {
                Hero = hero;
                Slot = slot;
                Custom = custom;
                Name = name;
            }

            public string Hero { get; private set; }
            public SpellSlot Slot { get; private set; }
            public string Name { get; private set; }
            public bool Custom { get; private set; }

            public void Init()
            {
                if (Name == null && Slot != SpellSlot.Unknown)
                {
                    var champ =
                        HeroManager.Enemies.FirstOrDefault(
                            h => h.ChampionName.Equals(Hero, StringComparison.OrdinalIgnoreCase));

                    var spell = champ?.GetSpell(Slot);

                    if (spell != null)
                    {
                        Name = spell.Name;
                    }
                }
            }
        }

        internal class ChampionObject
        {
            public ChampionObject(AIHeroClient hero)
            {
                Hero = hero;
            }

            public AIHeroClient Hero { get; private set; }
            public float LastSeen { get; set; }
        }

        internal class GrassLocation
        {
            public readonly int Index;
            public int Count;

            public GrassLocation(int index, int count)
            {
                Index = index;
                Count = count;
            }
        }

        internal class WardLocation
        {
            public readonly bool Grass;
            public readonly Vector3 Pos;

            public WardLocation(Vector3 pos, bool grass)
            {
                Pos = pos;
                Grass = grass;
            }
        }
    }
}