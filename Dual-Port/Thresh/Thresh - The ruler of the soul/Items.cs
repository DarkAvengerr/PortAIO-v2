using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using LCItems = LeagueSharp.Common.Items;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ThreshTherulerofthesoul
{
    class Items
    {
        public struct Item
        {
            public string Name;
            public int Id;
            public float Range;
        }

        static List<Item> ItemList = new List<Item>();
        private static Menu config = Program.config;

        static Items()
        {
            #region Locket of the Iron Solari

            ItemList.Add(new Item
                {
                    Name = "Locket of the Iron Solari",
                    Id = 3190,
                    Range = 600f
                });

            #endregion

            #region Randuin's Omen

            ItemList.Add(new Item
                {
                    Name = "Randuin's Omen",
                    Id = 3143,
                    Range = 500f
                });

            #endregion

            #region Face of the Mountain

            ItemList.Add(new Item
            {
                Name = "Face of the Mountain",
                Id = 3401,
                Range = 750f
            });

            #endregion

            #region Mikael's Crucible

            ItemList.Add(new Item
            {
                Name = "Mikael's Crucible",
                Id = 3222,
                Range = 600f
            });

            #endregion
            //Console.WriteLine("Load");
        }

        public static void LoadItems()
        {
            #region ItemsMenu

            var ItemMenu = new Menu("Items", "Items");
            {
                ItemMenu.AddItem(new MenuItem("UseItems", "Only Use Combo Key Press", true).SetValue(new KeyBind(32, KeyBindType.Press)));

                #region Solari

                var Sloarimenu = new Menu("Locket of the Iron Solari", "Locket of the Iron Solari");
                {
                    Sloarimenu.AddItem(new MenuItem("Use" + "Locket of the Iron Solari", "Use Item", true).SetValue(true));
                    ItemMenu.AddSubMenu(Sloarimenu);
                }

                #endregion

                #region Randuin

                var Randuinmenu = new Menu("Randuin's Omen", "Randuin's Omen");
                {
                    Randuinmenu.AddItem(new MenuItem("Use" + "Randuin's Omen", "Use Item", true).SetValue(true));
                    Randuinmenu.AddItem(new MenuItem("Randuin", "Use X Enemies In Range", true).SetValue(new Slider(2, 1, 5)));
                    ItemMenu.AddSubMenu(Randuinmenu);
                }

                #endregion

                #region FOMuntain

                var Mountainmenu = new Menu("Face of the Mountain", "Face of the Mountain");
                {
                    Mountainmenu.AddItem(new MenuItem("Use" + "Face of the Mountain", "Use Item", true).SetValue(true));
                    ItemMenu.AddSubMenu(Mountainmenu);
                }

                #endregion

                #region Mikael

                var Mikaelmenu = new Menu("Mikael's Crucible", "Mikael's Crucible");
                {
                    Mikaelmenu.AddItem(new MenuItem("Use" + "Mikael's Crucible", "Use Item", true).SetValue(true));

                    var BuffTypemenu = new Menu("Buff Type", "Buff Type");
                    {
                        BuffTypemenu.AddItem(new MenuItem("blind", "Blind", true).SetValue(false));
                        BuffTypemenu.AddItem(new MenuItem("charm", "Charm", true).SetValue(true));
                        BuffTypemenu.AddItem(new MenuItem("fear", "Fear", true).SetValue(false));
                        BuffTypemenu.AddItem(new MenuItem("flee", "Flee", true).SetValue(true));
                        BuffTypemenu.AddItem(new MenuItem("snare", "Snare", true).SetValue(true));
                        BuffTypemenu.AddItem(new MenuItem("taunt", "Taunt", true).SetValue(true));
                        BuffTypemenu.AddItem(new MenuItem("suppression", "Suppression", true).SetValue(true));
                        BuffTypemenu.AddItem(new MenuItem("stun", "Stun", true).SetValue(true));
                        BuffTypemenu.AddItem(new MenuItem("polymorph", "Polymorph", true).SetValue(false));
                        BuffTypemenu.AddItem(new MenuItem("silence", "Silence", true).SetValue(false));

                        Mikaelmenu.AddSubMenu(BuffTypemenu);
                    }

                    var Allymenu = new Menu("Use For Him", "Use For Him");
                    {
                        foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly))
                        {
                            Allymenu.AddItem(new MenuItem(hero.ChampionName, hero.ChampionName, true).SetValue(true));
                        }

                        Mikaelmenu.AddSubMenu(Allymenu);
                    }

                    ItemMenu.AddSubMenu(Mikaelmenu);
                }

                #endregion

                config.AddSubMenu(ItemMenu);
            }

            #endregion

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!config.IsActive("UseItems"))
                return;

            foreach (var item in ItemList.
                Where(x => 
                    LCItems.HasItem(x.Id) &&
                    LCItems.CanUseItem(x.Id)))
            {
                if (config.IsBool("Use" + item.Name))
                {
                    UseItem(item.Id, item.Range);
                }
            }
        }

        private static void UseItem(int id, float range)
        {
            if (id == 3190)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>()
                    .Where(x => 
                        x.IsAlly &&
                        ObjectManager.Player.Distance(x.Position) <= range &&
                        !x.IsDead))
                {
                    if (hero.HpPercents() < 20)
                    {
                        LCItems.UseItem(id, hero);
                    }
                }
            }

            if (id == 3143)
            {
                var ReqValue = config.GetValue("Randuin");
                
                if (HeroManager.Enemies.Where(x => x.IsValidTarget(range)).Count() >= ReqValue)
                {
                    LCItems.UseItem(3143);
                }
            }

            if (id == 3401)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>()
                    .Where(x => 
                        x.IsAlly &&
                        ObjectManager.Player.Distance(x.Position) <= range &&
                        !x.IsDead))
                {
                    if (hero.HpPercents() < 20)
                    {
                        LCItems.UseItem(id, hero);
                    }
                }
            }

            if (id == 3222)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>()
                    .Where(x => 
                        x.IsAlly &&
                        ObjectManager.Player.Distance(x.Position) <= range &&
                        !x.IsDead))
                {
                    if (config.IsBool(hero.ChampionName))
                    {
                        if (config.IsBool("blind") && hero.HasBuffOfType(BuffType.Blind))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (config.IsBool("charm") && hero.HasBuffOfType(BuffType.Charm))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (config.IsBool("fear") && hero.HasBuffOfType(BuffType.Fear))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (config.IsBool("flee") && hero.HasBuffOfType(BuffType.Flee))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (config.IsBool("snare") && hero.HasBuffOfType(BuffType.Snare))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (config.IsBool("taunt") && hero.HasBuffOfType(BuffType.Taunt))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (config.IsBool("suppression") && hero.HasBuffOfType(BuffType.Suppression))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (config.IsBool("stun") && hero.HasBuffOfType(BuffType.Stun))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (config.IsBool("polymorph") && hero.HasBuffOfType(BuffType.Polymorph))
                        {
                            LCItems.UseItem(id, hero);
                        }

                        if (config.IsBool("silence") && hero.HasBuffOfType(BuffType.Silence))
                        {
                            LCItems.UseItem(id, hero);
                        }
                    }
                }
            }
        }
    }
}
