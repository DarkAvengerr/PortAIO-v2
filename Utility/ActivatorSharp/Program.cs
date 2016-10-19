#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Activator/Program.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EloBuddy;
using LeagueSharp.Common;

#region Namespaces © 2015
using Activator.Base;
using Activator.Data;
using Activator.Handlers;
using Activator.Items;
using Activator.Spells;
using Activator.Summoners;
#endregion

namespace Activator
{
    internal class Activator
    {
        internal static Menu Origin;
        internal static AIHeroClient Player;
        internal static Random Rand;

        internal static int MapId;
        internal static int LastUsedTimeStamp;
        internal static int LastUsedDuration;

        internal static SpellSlot Smite;
        internal static bool SmiteInGame;
        internal static bool TroysInGame;
        internal static bool UseEnemyMenu, UseAllyMenu;

        //public static System.Version Version;
        public static List<Base.Champion> Heroes = new List<Base.Champion>();

        private static void Main(string[] args)
        {
            //Version = Assembly.GetExecutingAssembly().GetName().Version;
            EloBuddy.SDK.Events.Loading.OnLoadingComplete += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            try
            {
                Player = EloBuddy.Player.Instance;
                MapId = (int)LeagueSharp.Common.Utility.Map.GetMap().Type;
                Rand = new Random();

                GetSpellsInGame();
                GetSmiteSlot();
                GetGameTroysInGame();
                GetAurasInGame();
                GetHeroesInGame();
                GetComboDamage();
                //Helpers.CreateLogPath();

                Origin = new Menu("Activator", "activator", true);

                Menu cmenu = new Menu("Cleansers", "cmenu");
                GetItemGroup("Items.Cleansers").ForEach(t => NewItem((CoreItem) NewInstance(t), cmenu));
                CreateSubMenu(cmenu, false);
                Origin.AddSubMenu(cmenu);

                Menu dmenu = new Menu("Defensives", "dmenu");
                GetItemGroup("Items.Defensives").ForEach(t => NewItem((CoreItem) NewInstance(t), dmenu));
                CreateSubMenu(dmenu, false);
                Origin.AddSubMenu(dmenu);

                Menu smenu = new Menu("Summoners", "smenu");
                GetItemGroup("Summoners").ForEach(t => NewSumm((CoreSum) NewInstance(t), smenu));
                CreateSubMenu(smenu, true, true);
                Origin.AddSubMenu(smenu);

                Menu omenu = new Menu("Offensives", "omenu");
                GetItemGroup("Items.Offensives").ForEach(t => NewItem((CoreItem) NewInstance(t), omenu));
                CreateSubMenu(omenu, true);
                Origin.AddSubMenu(omenu);

                Menu imenu = new Menu("Consumables", "imenu");
                GetItemGroup("Items.Consumables").ForEach(t => NewItem((CoreItem) NewInstance(t), imenu));
                Origin.AddSubMenu(imenu);

                Menu amenu = new Menu("Auto Spells", "amenu");
                GetItemGroup("Spells.Evaders").ForEach(t => NewSpell((CoreSpell) NewInstance(t), amenu));
                GetItemGroup("Spells.Shields").ForEach(t => NewSpell((CoreSpell) NewInstance(t), amenu));
                GetItemGroup("Spells.Health").ForEach(t => NewSpell((CoreSpell) NewInstance(t), amenu));
                GetItemGroup("Spells.Slows").ForEach(t => NewSpell((CoreSpell) NewInstance(t), amenu));
                GetItemGroup("Spells.Heals").ForEach(t => NewSpell((CoreSpell) NewInstance(t), amenu));
                CreateSubMenu(amenu, false);
                Origin.AddSubMenu(amenu);

                Menu zmenu = new Menu("Misc/Settings", "settings");

                if (SmiteInGame)
                {
                    Menu ddmenu = new Menu("Drawings", "drawings");
                    ddmenu.AddItem(new MenuItem("drawsmitet", "Draw Smite Text")).SetValue(true);
                    ddmenu.AddItem(new MenuItem("drawfill", "Draw Smite Fill")).SetValue(true);
                    ddmenu.AddItem(new MenuItem("drawsmite", "Draw Smite Range")).SetValue(true);
                    zmenu.AddSubMenu(ddmenu);
                }

                var bbmenu = new Menu("Debug Tools", "bbmenu");
                bbmenu.AddItem(new MenuItem("acdebug", "Debug Income Damage")).SetValue(false);
                bbmenu.AddItem(new MenuItem("acdebug2", "Debug Item Priority")).SetValue(false);
                bbmenu.AddItem(new MenuItem("dumpdata", "Dump Spell Data")).SetValue(false);
                zmenu.AddSubMenu(bbmenu);

                zmenu.AddItem(new MenuItem("autolevelup", "Auto Level Ultimate")).SetValue(true).SetTooltip("Level 6 Only");
                zmenu.AddItem(new MenuItem("autotrinket", "Auto Upgrade Trinket")).SetValue(false);
                zmenu.AddItem(new MenuItem("healthp", "Ally Priority:")).SetValue(new StringList(new[] { "Low HP", "Most AD/AP", "Most HP" }, 1));
                zmenu.AddItem(new MenuItem("weightdmg", "Weight Income Damage (%)"))
                    .SetValue(new Slider(115, 100, 150))
                    .SetTooltip("Make Activator# think you are taking more damage than calulated.");
                zmenu.AddItem(new MenuItem("usecombo", "Combo (active)")).SetValue(new KeyBind(32, KeyBindType.Press, true));

                Menu uumenu = new Menu("Spell Database", "evadem");
                LoadSpellMenu(uumenu);
                zmenu.AddSubMenu(uumenu);

                Origin.AddSubMenu(zmenu);
                Origin.AddToMainMenu();

                // drawings
                Drawings.Init();

                // handlers
                Projections.Init();
                Trinkets.Init();

                // tracks dangerous or lethal buffs/auras
                Buffs.StartOnUpdate();

                // tracks gameobjects 
                Gametroys.StartOnUpdate();

                // on bought item
                //Obj_AI_Base.OnPlaceItemInSlot += Obj_AI_Base_OnPlaceItemInSlot;
                Shop.OnBuyItem += Obj_AI_Base_OnPlaceItemInSlot;

                // on level up
                Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;

                Chat.Print("<b><font color=\"#FF3366\">Activator#</font></b> - Loaded!");
                //Updater.UpdateCheck();

                // init valid auto spells
                foreach (CoreSpell autospell in Lists.Spells)
                    if (Player.GetSpellSlot(autospell.Name) != SpellSlot.Unknown)
                        Game.OnUpdate += autospell.OnTick;

                // init valid summoners
                foreach (CoreSum summoner in Lists.Summoners)
                    if (summoner.Slot != SpellSlot.Unknown ||
                        summoner.ExtraNames.Any(x => Player.GetSpellSlot(x) != SpellSlot.Unknown) ||
                        summoner.Name == "summonerteleport")
                        Game.OnUpdate += summoner.OnTick;

                // find items (if F5)
                foreach (CoreItem item in Lists.Items)
                {
                    if (!LeagueSharp.Common.Items.HasItem(item.Id))
                    {
                        continue;
                    }

                    if (!Lists.BoughtItems.Contains(item))
                    {
                        if (item.Category.Any())
                            Game.OnUpdate += item.OnTick;

                        if (item.Category.Any(t => t == MenuType.Gapcloser))
                            AntiGapcloser.OnEnemyGapcloser += item.OnEnemyGapcloser;

                        Lists.BoughtItems.Add(item);
                        Chat.Print("<b><font color=\"#FF3366\">Activator#</font></b> - <font color=\"#FFF280\">" + item.Name + "</font> active!");
                    }
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("Exception thrown at <font color=\"#FFF280\">Activator.OnGameLoad</font>");
            }
        }

        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!Origin.Item("autolevelup").GetValue<bool>())
            {
                return;
            }

            AIHeroClient hero = sender as AIHeroClient;
            if (hero == null || !hero.IsMe || Shop.IsOpen)
            {
                return;
            }

            if (hero.ChampionName == "Jayce" || 
                hero.ChampionName == "Udyr" || 
                hero.ChampionName == "Elise")
            {
                return;
            }

            switch (Player.Level)
            {
                case 6:
                    LeagueSharp.Common.Utility.DelayAction.Add(Rand.Next(250, 950) + Math.Max(30, Game.Ping),
                        () => { Player.Spellbook.LevelSpell(SpellSlot.R); });
                    break;
            }
        }

        private static void Obj_AI_Base_OnPlaceItemInSlot(Obj_AI_Base sender, ShopActionEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            int itemid = (int) args.Id;

            foreach (CoreItem item in Lists.Items)
            {
                if (item.Id == itemid)
                {
                    if (!Lists.BoughtItems.Contains(item))
                    {
                        if (item.Category.Any())
                            Game.OnUpdate += item.OnTick;

                        if (item.Category.Any(t => t == MenuType.Gapcloser))
                            AntiGapcloser.OnEnemyGapcloser += item.OnEnemyGapcloser;

                        Lists.BoughtItems.Add(item);
                        Chat.Print("<b><font color=\"#FF3366\">Activator#</font></b> - <font color=\"#FFF280\">" + item.Name + "</font> active!");
                    }
                }
            }
        }

        private static void NewItem(CoreItem item, Menu parent)
        {
            try
            {
                if (item.Maps.Contains((MapType) MapId) || 
                    item.Maps.Contains(MapType.Common))
                {
                    Lists.Items.Add(item.CreateMenu(parent));
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("Exception thrown at <font color=\"#FFF280\">Activator.NewItem</font>");
            }
        }

        private static void NewSpell(CoreSpell spell, Menu parent)
        {
            try
            {
                if (Player.GetSpellSlot(spell.Name) != SpellSlot.Unknown)
                    Lists.Spells.Add(spell.CreateMenu(parent));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("Exception thrown at <font color=\"#FFF280\">Activator.NewSpell</font>");
            }
        }

        private static void NewSumm(CoreSum summoner, Menu parent)
        {
            try
            {
                if (summoner.Name.Contains("smite") && SmiteInGame)
                    Lists.Summoners.Add(summoner.CreateMenu(parent));

                if (!summoner.Name.Contains("smite"))
                {
                    if (Player.GetSpellSlot(summoner.Name) != SpellSlot.Unknown)
                    {
                        if (summoner.Name == "summonerteleport")
                            Drawing.OnDraw += summoner.OnDraw;

                        Lists.Summoners.Add(summoner.CreateMenu(parent));
                    }

                    else if (summoner.Name == "summonerteleport")
                        Lists.Summoners.Add(summoner.CreateMenu(parent));
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("Exception thrown at <font color=\"#FFF280\">Activator.NewSumm</font>");
            }
        }

        private static List<Type> GetItemGroup(string nspace)
        {
            try
            {
                Type[] allowedTypes = { typeof (CoreItem), typeof (CoreSpell), typeof (CoreSum) };

                return
                    Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(
                            t =>
                                t.IsClass && t.Namespace == "Activator." + nspace && !t.Name.Contains("Core") &&
                                allowedTypes.Any(x => x.IsAssignableFrom(t)))
                        .ToList();
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("Exception thrown at <font color=\"#FFF280\">Activator.GetItemGroup</font>");
                return null;
            }
        }

        private static void GetComboDamage()
        {
            foreach (KeyValuePair<string, List<DamageSpell>> entry in Damage.Spells)
            {
                if (entry.Key == Player.ChampionName)
                    foreach (DamageSpell spell in entry.Value)
                        Gamedata.DamageLib.Add(spell.Damage, spell.Slot);
            }
        }

        private static void GetHeroesInGame()
        {
            foreach (AIHeroClient i in ObjectManager.Get<AIHeroClient>().Where(h => h.Team != Player.Team))
                Heroes.Add(new Base.Champion(i, 0));

            foreach (AIHeroClient i in ObjectManager.Get<AIHeroClient>().Where(h => h.Team == Player.Team))
                Heroes.Add(new Base.Champion(i, 0));
        }

        private static void GetSmiteSlot()
        {
            if (Player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite"))
            {
                SmiteInGame = true;
                Smite = SpellSlot.Summoner1;
            }

            if (Player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite"))
            {
                SmiteInGame = true;
                Smite = SpellSlot.Summoner2;
            }
        }

        private static void GetGameTroysInGame()
        {
            foreach (AIHeroClient i in ObjectManager.Get<AIHeroClient>().Where(h => h.Team != Player.Team))
            {
                foreach (Troydata item in Troydata.Troys.Where(x => x.ChampionName == i.ChampionName))
                {
                    TroysInGame = true;
                    Gametroy.Troys.Add(new Gametroy(i.ChampionName, item.Slot, item.Name, 0, false));
                }
            }
        }

        private static void GetSpellsInGame()
        {
            foreach (AIHeroClient i in ObjectManager.Get<AIHeroClient>().Where(h => h.Team != Player.Team))
                foreach (Gamedata item in Gamedata.Spells.Where(x => x.ChampionName == i.ChampionName.ToLower()))
                    Gamedata.CachedSpells.Add(item);

            foreach (var i in Smitedata.SpellList.Where(x => x.Name == Player.ChampionName))
                Smitedata.CachedSpellList.Add(i);
        }

        private static void GetAurasInGame()
        {
            foreach (AIHeroClient i in ObjectManager.Get<AIHeroClient>().Where(h => h.Team != Player.Team))
                foreach (Auradata aura in Auradata.BuffList.Where(x => x.Champion == i.ChampionName && x.Champion != null))
                    Auradata.CachedAuras.Add(aura);

            foreach (Auradata generalaura in Auradata.BuffList.Where(x => string.IsNullOrEmpty(x.Champion)))
                Auradata.CachedAuras.Add(generalaura);
        }

        public static IEnumerable<Base.Champion> Allies()
        {
            switch (Origin.Item("healthp").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return Heroes.Where(h => h.Player.IsAlly && !h.Player.IsDead)
                        .OrderBy(h => h.Player.Health / h.Player.MaxHealth * 100);
                case 1:
                    return Heroes.Where(h => h.Player.IsAlly && !h.Player.IsDead)
                        .OrderByDescending(h => h.Player.FlatPhysicalDamageMod + h.Player.FlatMagicDamageMod);
                case 2:
                    return Heroes.Where(h => h.Player.IsAlly && !h.Player.IsDead)
                        .OrderByDescending(h => h.Player.Health);
            }

            return null;
        }

        private static void CreateSubMenu(Menu parent, bool enemy, bool both = false)
        {
            Menu menu = new Menu("-> Config", parent.Name + "sub");

            MenuItem ireset = new MenuItem(parent.Name + "clear", "Deselect [All]");
            menu.AddItem(ireset).SetValue(false);

            foreach (AIHeroClient hero in both ? HeroManager.AllHeroes : enemy ? HeroManager.Enemies : HeroManager.Allies)
            {
                string side = hero.Team == Player.Team ? "[Ally]" : "[Enemy]";
                MenuItem mitem = new MenuItem(parent.Name + "useon" + hero.NetworkId, "Use for " + hero.ChampionName + " " + side);

                menu.AddItem(mitem.DontSave()).SetValue(true);

                if (both)
                {
                    mitem.Show(hero.IsAlly && UseAllyMenu || hero.IsEnemy && UseEnemyMenu);
                }
            }

            ireset.ValueChanged += (sender, args) =>
            {
                if (args.GetNewValue<bool>())
                {
                    foreach (AIHeroClient hero in 
                     both ? HeroManager.AllHeroes
                          : enemy
                            ? HeroManager.Enemies
                            : HeroManager.Allies)
                        menu.Item(parent.Name + "useon" + hero.NetworkId).SetValue(hero.IsMe);

                    LeagueSharp.Common.Utility.DelayAction.Add(100, () => ireset.SetValue(false));
                }
            };

            parent.AddSubMenu(menu);
        }

        private static void LoadSpellMenu(Menu parent)
        {
            foreach (Base.Champion unit in Heroes.Where(h => h.Player.Team != Player.Team))
            {
                Menu menu = new Menu(unit.Player.ChampionName, unit.Player.NetworkId + "menu");

                // new menu per spell
                foreach (Gamedata entry in Gamedata.Spells)
                {
                    if (entry.ChampionName == unit.Player.ChampionName.ToLower())
                    {
                        Menu newmenu = new Menu(entry.SDataName, entry.SDataName);

                        // activation parameters
                        newmenu.AddItem(new MenuItem(entry.SDataName + "predict", "enabled").DontSave())
                            .SetValue(true);
                        newmenu.AddItem(new MenuItem(entry.SDataName + "danger", "danger").DontSave())
                            .SetValue(entry.HitTypes.Contains(HitType.Danger));
                        newmenu.AddItem(new MenuItem(entry.SDataName + "crowdcontrol", "crowdcontrol").DontSave())
                            .SetValue(entry.HitTypes.Contains(HitType.CrowdControl));
                        newmenu.AddItem(new MenuItem(entry.SDataName + "ultimate", "danger ultimate").DontSave())
                            .SetValue(entry.HitTypes.Contains(HitType.Ultimate));
                        newmenu.AddItem(new MenuItem(entry.SDataName + "forceexhaust", "force exhaust").DontSave())
                            .SetValue(entry.HitTypes.Contains(HitType.ForceExhaust));
                        menu.AddSubMenu(newmenu);

                        LeagueSharp.Common.Utility.DelayAction.Add(5000, 
                            () => newmenu.Item(entry.SDataName + "predict").SetValue(entry.CastRange != 0));
                    }
                }

                parent.AddSubMenu(menu);
            }
        }

        private static object NewInstance(Type type)
        {
            try
            {
                ConstructorInfo target = type.GetConstructor(Type.EmptyTypes);
                DynamicMethod dynamic = new DynamicMethod(string.Empty, type, new Type[0], target.DeclaringType);
                ILGenerator il = dynamic.GetILGenerator();

                il.DeclareLocal(target.DeclaringType);
                il.Emit(OpCodes.Newobj, target);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);

                Func<object> method = (Func<object>) dynamic.CreateDelegate(typeof(Func<object>));
                return method();
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("Exception thrown at <font color=\"#FFF280\">Activator.NewInstance</font>");
                return null;
            }
        }
    }
}