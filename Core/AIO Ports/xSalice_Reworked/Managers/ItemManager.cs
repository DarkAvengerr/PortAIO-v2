using EloBuddy; 
using LeagueSharp.Common; 
namespace xSaliceResurrected_Rework.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class ItemManager
    {
        private static Menu _myMenu;

        private string ActiveName { get; set; }

        private int ActiveId { get; set; }

        private int Range { get; set; }

        private string BuffName { get; set; }

        private int Mode { get; set; }

        private static readonly List<ItemManager> ItemList = new List<ItemManager>();

        private static readonly SpellSlot IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
        public static bool UseTargetted;
        public static bool KillableTarget;
        public static AIHeroClient Target;

        private static void CreateList()
        {
            ItemList.Add(new ItemManager
            {
                ActiveId = 3144,
                ActiveName = "Bilgewater Cutlass",
                BuffName = "Offensive",
                Range = 550,
                Mode = 0,
            });

            ItemList.Add(new ItemManager
            {
                ActiveId = 3153,
                ActiveName = "Blade of the Ruined King",
                BuffName = "Offensive",
                Range = 550,
                Mode = 0,
            });

            ItemList.Add(new ItemManager
            {
                ActiveId = 3146,
                ActiveName = "Hextech Gunblade",
                BuffName = "Offensive",
                Range = 700,
                Mode = 0,
            });

            ItemList.Add(new ItemManager
            {
                ActiveId = ItemData.Frost_Queens_Claim.Id,
                ActiveName = "Frost Queen's Claim",
                BuffName = "Offensive",
                Range = 850,
                Mode = 0,
            });

            ItemList.Add(new ItemManager
            {
                ActiveId = 3042,
                ActiveName = "Muramana",
                BuffName = "Offensive",
                Range = int.MaxValue,
                Mode = 2,
            });

            ItemList.Add(new ItemManager
            {
                ActiveId = 3074,
                ActiveName = "Ravenous Hydra",
                BuffName = "Offensive",
                Range = 400,
                Mode = 1,
            });

            ItemList.Add(new ItemManager
            {
                ActiveId = 3077,
                ActiveName = "Tiamat",
                BuffName = "Offensive",
                Range = 400,
                Mode = 1,
            });

            ItemList.Add(new ItemManager
            {
                ActiveId = 3142,
                ActiveName = "Youmuu's Ghostblade",
                BuffName = "Offensive",
                Range = (int)(ObjectManager.Player.AttackRange * 2),
                Mode = 1,
            });

            ItemList.Add(new ItemManager
            {
                ActiveId = ItemData.Refillable_Potion.Id,
                ActiveName = "Refillable_Potion",
                BuffName = "ItemRefillablePotion",
                Range = int.MaxValue,
                Mode = 3,
            });

            ItemList.Add(new ItemManager
            {
                ActiveId = ItemData.Health_Potion.Id,
                ActiveName = "Health Potion",
                BuffName = "RegenerationPotion",
                Range = int.MaxValue,
                Mode = 4,
            });

        }

        public static void AddToMenu(Menu theMenu)
        {
            _myMenu = theMenu;

            CreateList();

            var offensiveItem = new Menu("Offensive Items", "Offensive Items");
            {
                foreach (var item in ItemList.Where(x => x.Mode < 3))
                {
                    AddOffensiveItem(offensiveItem, item);
                }
                _myMenu.AddSubMenu(offensiveItem);
            }

            var potions = new Menu("Potions", "Potions");
            {
                foreach (var item in ItemList.Where(x => (x.Mode == 3 || x.Mode == 5 || x.Mode == 4)))
                {
                    AddPotion(potions, item);
                }
                _myMenu.AddSubMenu(potions);
            }

            var summoners = new Menu("Summoners", "Summoners");
            {
                var ignite = new Menu("Ignite", "Ignite");
                {
                    ignite.AddItem(new MenuItem("ignite", "Use Ignite", true).SetValue(true));
                    ignite.AddItem(
                        new MenuItem("igniteMode", "Ignite Mode", true).SetValue(new StringList(new[] { "Combo", "KS" })));
                    summoners.AddSubMenu(ignite);
                }
                _myMenu.AddSubMenu(summoners);
            }

            Orbwalking.AfterAttack += AfterAttack;
            Game.OnUpdate += Game_OnGameUpdate;
        }

        private static void AddOffensiveItem(Menu subMenu, ItemManager item)
        {
            var active = new Menu(item.ActiveName, item.ActiveName);
            {
                active.AddItem(new MenuItem(item.ActiveName, item.ActiveName, true).SetValue(true));
                active.AddItem(new MenuItem(item.ActiveName + "dmgCalc", "Add to damage Calculation", true).SetValue(true));
                active.AddItem(new MenuItem(item.ActiveName + "killAble", "Use only if enemy is killable", true).SetValue(false));
                active.AddItem(new MenuItem(item.ActiveName + "always", "Always use", true).SetValue(item.Mode == 1 || item.Mode == 2));
                active.AddItem(new MenuItem(item.ActiveName + "myHP", "Use if HP <= %", true).SetValue(new Slider(25)));
                active.AddItem(new MenuItem(item.ActiveName + "enemyHP", "Use if target HP <= %", true).SetValue(new Slider(50)));

                subMenu.AddSubMenu(active);
            }
        }

        private static void AddPotion(Menu subMenu, ItemManager item)
        {
            var active = new Menu(item.ActiveName, item.ActiveName);
            {
                active.AddItem(new MenuItem(item.ActiveName, item.ActiveName, true).SetValue(true));
                if (item.Mode == 3 || item.Mode == 4)
                    active.AddItem(new MenuItem(item.ActiveName + "myHP", "Use if HP <= %", true).SetValue(new Slider(50)));
                if (item.Mode == 3 || item.Mode == 5)
                    active.AddItem(new MenuItem(item.ActiveName + "myMP", "Use if MP <= %", true).SetValue(new Slider(50)));

                subMenu.AddSubMenu(active);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsUsingHealthPot)
            {
                foreach (var potion in from potion in ItemList.Where(x => (x.Mode == 3 || x.Mode == 4) &&
                    Items.HasItem(x.ActiveId) &&
                    Items.CanUseItem(x.ActiveId) &&
                    ShouldUse(x.ActiveName))
                    where !ObjectManager.Player.IsRecalling() && !ObjectManager.Player.InFountain()
                    where ObjectManager.Player.HealthPercent <= UsePotAtHp(potion.ActiveName)
                    select potion)
                {
                    Items.UseItem(potion.ActiveId);
                }
            }

            if (!IsUsingManaPot)
            {
                foreach (var potion in from potion in ItemList.Where(x => (x.Mode == 3 || x.Mode == 5) &&
                    Items.HasItem(x.ActiveId) &&
                    Items.CanUseItem(x.ActiveId) &&
                    ShouldUse(x.ActiveName))
                    where !ObjectManager.Player.IsRecalling() && !ObjectManager.Player.InFountain()
                    where ObjectManager.Player.ManaPercent <= UsePotAtMp(potion.ActiveName)
                    select potion)
                {
                    Items.UseItem(potion.ActiveId);
                }
            }

            if (Target == null || ObjectManager.Player.IsDead)
            {
                return;
            }

            if (_myMenu.Item("ignite", true).GetValue<bool>())
            {
                var igniteMode = _myMenu.Item("igniteMode", true).GetValue<StringList>().SelectedIndex;

                if (KillableTarget && igniteMode == 0 && Ignite_Ready())
                {
                    Use_Ignite(Target);
                }
                else if (ObjectManager.Player.GetSummonerSpellDamage(Target, Damage.SummonerSpell.Ignite) >
                         Target.Health + 20 && Ignite_Ready())
                {
                    Use_Ignite(Target);
                }
            }

            if (!UseTargetted)
            {
                return;
            }

            foreach (var item in ItemList.Where(x => x.Mode == 0 && Items.HasItem(x.ActiveId) && ShouldUse(x.ActiveName)))
            {
                if (Target != null && Items.CanUseItem(item.ActiveId))
                {
                    if (AlwaysUse(item.ActiveName))
                    {
                        Items.UseItem(item.ActiveId, Target);
                    }

                    if (KillableTarget)
                    {
                        Items.UseItem(item.ActiveId, Target);
                    }

                    if (ObjectManager.Player.HealthPercent <= UseAtMyHp(item.ActiveName) && !OnlyIfKillable(item.ActiveName))
                    {
                        Items.UseItem(item.ActiveId, Target);
                    }

                    if (Target.HealthPercent <= UseAtEnemyHp(item.ActiveName) && !OnlyIfKillable(item.ActiveName))
                    {
                        Items.UseItem(item.ActiveId, Target);
                    }
                }
            }

            UseTargetted = false;
            Target = null;
            KillableTarget = false;
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe || !(target is AIHeroClient))
                return;

            foreach (var item in ItemList.Where(x => x.Mode == 1 && Items.CanUseItem(x.ActiveId) && ShouldUse(x.ActiveName)))
            {
                if (AlwaysUse(item.ActiveName))
                {
                    Items.UseItem(item.ActiveId);
                }

                if (KillableTarget)
                {
                    Items.UseItem(item.ActiveId);
                }

                if (ObjectManager.Player.HealthPercent <= UseAtMyHp(item.ActiveName) && !OnlyIfKillable(item.ActiveName))
                {
                    Items.UseItem(item.ActiveId);
                }

                if (Target.HealthPercent <= UseAtEnemyHp(item.ActiveName) && !OnlyIfKillable(item.ActiveName))
                {
                    Items.UseItem(item.ActiveId);
                }
            }
        }

        public static float CalcDamage(Obj_AI_Base target, double currentDmg)
        {
            if (target == null)
            {
                return 0;
            }

            double dmg = currentDmg;

            foreach (var item in ItemList.Where(x => x.Mode <= 1 && Items.HasItem(x.ActiveId)  && Items.CanUseItem(x.ActiveId)))
            {
                if (ShouldUse(item.ActiveName) && AddToDmgCalc(item.ActiveName))
                {
                    //bilge
                    if (item.ActiveId == 3144)
                    {
                        dmg += ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Bilgewater);
                    }

                    //Botrk
                    if (item.ActiveId == 3153)
                    {
                        dmg += ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk);
                    }

                    //hextech
                    if (item.ActiveId == 3146)
                    {
                        dmg += ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Hexgun);
                    }

                    //hydra
                    if (item.ActiveId == 3074)
                    {
                        dmg += ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Hydra);
                    }

                    //tiamat
                    if (item.ActiveId == 3077)
                    {
                        dmg += ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Tiamat);
                    }

                    //sheen
                    if (Items.HasItem(3057))
                    {
                        dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, SheenDamage());
                    }

                    //lich bane
                    if (Items.HasItem(3100))
                    {
                        dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, LichDamage());
                    }
                }
            }

            if (Ignite_Ready())
            {
                dmg += ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            }

            return (float)dmg;
        }

        private static double SheenDamage()
        {
            double dmg = 0;

            dmg += ObjectManager.Player.FlatPhysicalDamageMod;

            return dmg;
        }

        private static double LichDamage()
        {
            double dmg = 0;

            dmg += .75 * ObjectManager.Player.FlatPhysicalDamageMod;
            dmg += .5 * ObjectManager.Player.FlatMagicDamageMod;

            return dmg;
        }

        private static bool Ignite_Ready()
        {
            return IgniteSlot != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready;
        }

        private static void Use_Ignite(AIHeroClient target)
        {
            if (target != null && IgniteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready &&
                ObjectManager.Player.Distance(target.Position) < 650)
            {
                ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, target);
            }
        }

        private static bool ShouldUse(string name)
        {
            return _myMenu.Item(name, true).GetValue<bool>();
        }

        private static bool AlwaysUse(string name)
        {
            return _myMenu.Item(name + "always", true).GetValue<bool>();
        }

        private static bool AddToDmgCalc(string name)
        {
            return _myMenu.Item(name + "dmgCalc", true).GetValue<bool>();
        }

        private static bool OnlyIfKillable(string name)
        {
            return _myMenu.Item(name + "killAble", true).GetValue<bool>();
        }

        private static int UseAtMyHp(string name)
        {
            return _myMenu.Item(name + "myHP", true).GetValue<Slider>().Value;
        }

        private static int UseAtEnemyHp(string name)
        {
            return _myMenu.Item(name + "enemyHP", true).GetValue<Slider>().Value;
        }

        private static int UsePotAtHp(string name)
        {
            return _myMenu.Item(name + "myHP", true).GetValue<Slider>().Value;
        }

        private static int UsePotAtMp(string name)
        {
            return _myMenu.Item(name + "myMP", true).GetValue<Slider>().Value;
        }

        private static bool IsUsingHealthPot
        {
            get
            {
                return
                    ItemList.Where(x => (x.Mode == 3 || x.Mode == 4))
                        .Any(x => ObjectManager.Player.HasBuff(x.BuffName));
            }
        }

        private static bool IsUsingManaPot
        {
            get
            {
                return
                    ItemList.Where(x => (x.Mode == 3 || x.Mode == 5))
                        .Any(x => ObjectManager.Player.HasBuff(x.BuffName));
            }
        }
    }
}
