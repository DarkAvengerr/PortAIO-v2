using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CarryAshe
{
    internal static class ItemManager
    {
        private static string ITEMMANAGER_ROOT = Program.Champion.Player.ChampionName + ".ItemManager";

        internal static MenuItem GetItemManagerSetting(ItemData.Item item, string settingKey)
        {
            return Menu.Item(String.Format("{0}.{1}.Settings.{2}", ITEMMANAGER_ROOT, item.Name.ToCamelCase(), settingKey));
        }


        #region ItemType
        internal class ItemType
        {
            public Func<ItemData.Item, bool> IsUsableNow{get;private set;}
            public Action<ItemData.Item> Use{get;private set;}
            public List<MenuItem> Properties{get;private set;}
            public GameObject Target{get;private set;}

            private string _typeName = null;

            public string TypeName
            {
                get
                {
                    if (_typeName == null)
                        _typeName = this.GetType().Name.Substring(this.GetType().Name.LastIndexOf(".") + 1).ToTitleCase();
                    return _typeName;
                }
            }

            private ItemType(){}

            #region Damage
            public static ItemType Damage = new ItemType
            {
                Properties = new List<MenuItem>(){
                    new MenuItem("MinTargetHPPercentMissing","Min Target Hp Percent").SetValue(new Slider(90,0,100)),
                },
                IsUsableNow = (item) =>
                {
                    var minTargetHPPercentMissing = ItemManager.GetItemManagerSetting(item,"MinTargetHPPercentMissing").GetValue<Slider>().Value;
                    return minTargetHPPercentMissing >= Program.Orbwalker.GetTarget().HealthPercent;
                },

                Use = (item) =>
                {
                    item.GetItem().Cast((Obj_AI_Base)Program.Orbwalker.GetTarget());
                }
            };
            #endregion

            #region Slow
            public static ItemType Slow = new ItemType
            {
                Properties = new List<MenuItem>(){
                    new MenuItem("ForceUseOfCCTarget","Force Use On CC Targets").SetValue(false),
                },
                IsUsableNow = (item) =>
                {
                    var itemName = item.Name.ToCamelCase();
                    if (ItemManager.GetItemManagerSetting(item, "ForceUseOfCCTarget").GetValue<bool>())
                    {
                        return true;
                    }

                    var target = Program.Orbwalker.GetTarget();
                    return target.IsValid<AIHeroClient>()
                        && !((AIHeroClient)target).IsCharmed
                        && !((AIHeroClient)target).IsRooted
                        && !((AIHeroClient)target).IsStunned;
                },

                Use = (item) =>
                {
                    item.GetItem().Cast((Obj_AI_Base)Program.Orbwalker.GetTarget());
                }
            };
            #endregion

            #region Shield

            public static ItemType Shield = new ItemType
            {
                Properties = new List<MenuItem>(){
                    new MenuItem("MinAllyHealth","Minimum Ally Health Percent").SetValue(new Slider(70,0,100)),
                },
                IsUsableNow = (item) =>
                {
                    var minAllyHealth = GetItemManagerSetting(item, "MinAllyHealth").GetValue<Slider>().Value;
                    return (Shield.Target = HeroManager.Allies.Where(ally => item.GetItem().IsInRange(ally)
                                                                    && ally.HealthPercent <= minAllyHealth)
                                                                    .OrderBy(ally => ally.Health)
                                                                    .FirstOrDefault()) != null;
                },

                Use = (item) =>
                {
                    if(Shield.Target != null)
                        item.GetItem().Cast((Obj_AI_Base)Shield.Target);
                }
            };
            #endregion

            #region Cleanser
            public static ItemType Cleanser = new ItemType
            {
                Properties = new List<MenuItem>(){
                    new MenuItem("CleanseCCOnly","Cleanse Only CC").SetValue(true),
                },
                IsUsableNow = (item) =>
                {
                    var onlyCC = GetItemManagerSetting(item, "CleanseCCOnly").GetValue<bool>();
                    return (Cleanser.Target = HeroManager.Allies.Where(ally => item.GetItem().IsInRange(ally)
                                                                    && (ally.IsCC()
                                                                        || (!onlyCC && false))) //TODO
                                                                    .OrderBy(ally => ally.Health)
                                                                    .FirstOrDefault()) != null;
                },

                Use = (item) =>
                {
                    if (Cleanser.Target != null)
                        item.GetItem().Cast((Obj_AI_Base)Shield.Target);
                }
            };

            public static ItemType SelfCleanser = new ItemType
            {
                Properties = new List<MenuItem>(){
                    new MenuItem("CleanseCCOnly","Cleanse Only CC").SetValue(true),
                },
                IsUsableNow = (item) =>
                {
                    var onlyCC = GetItemManagerSetting(item, "CleanseCCOnly").GetValue<bool>();
                    return (Cleanser.Target = Program.Champion.Player.IsCC() || (!onlyCC && false) ? Program.Champion.Player : null) != null;
                },

                Use = (item) =>
                {
                    if (Cleanser.Target != null)
                        item.GetItem().Cast();
                }
            };
            #endregion

        }

        #endregion

        #region TypedItem
        internal class TypedItem
        {
            internal Menu _menu;
            internal ItemData.Item Item { get; private set; }
            internal IEnumerable<ItemType> Types { get; private set; }

            internal TypedItem(ItemData.Item item, IEnumerable<ItemType> types)
            {
                Item = item;
                Types = types;
            }

            public bool CanBeUsed()
            {
                foreach (var type in Types)
                {
                    if(! type.IsUsableNow(Item))
                        return false;
                }

                return true;
            }

            public void Use()
            {
                Types.First().Use(Item);
            }

            public virtual Menu InitMenu(string customDisplayAddition = "")
            {
                var itemTrimed = Item.Name.ToCamelCase();
                Console.WriteLine("-------------------------------------------------------");
                _menu = new Menu(String.Format("{0} {1}",Item.Name,customDisplayAddition), String.Format("{0}.{1}", ITEMMANAGER_ROOT, itemTrimed));
                _menu.AddItem(new MenuItem(String.Format("{0}.Use",_menu.Name),"Use").SetValue(true));
                Menu subMenu = new Menu("Settings",String.Format("{0}.Settings",_menu.Name, itemTrimed));
                foreach (var type in Types)
                {
                    foreach (var prop in type.Properties) {
                        var val = prop.GetValue<Object>();
                        Console.WriteLine(val.GetType().ToString());
                            var me = new MenuItem(
                                String.Format("{0}.{1}", subMenu.Name, prop.Name),
                                prop.DisplayName)
                            .SetValue(val);
                        subMenu.AddItem(me
                            );
                    }
                }
                Console.WriteLine("-------------------------------------------------------");


                _menu.AddSubMenu(subMenu);

                return _menu;
            }
        }

        public class BetaTypedItem : TypedItem
        {
            public BetaTypedItem(ItemData.Item item, IEnumerable<ItemType> types):base(item,types) { }
            public override Menu InitMenu(string customDisplayAddition)
            {
                return base.InitMenu(String.Format("(beta) {0}", customDisplayAddition));
            }
        }

        #endregion
        internal static Menu Menu { get; private set; }


        private static  List<TypedItem> SUPPORTED_ITEMS = new List<TypedItem>{
            new TypedItem(ItemData.Bilgewater_Cutlass,new List<ItemType>{ItemType.Damage}),
            new TypedItem(ItemData.Blade_of_the_Ruined_King,new List<ItemType>{ItemType.Slow,ItemType.Damage}),

            new BetaTypedItem(ItemData.Quicksilver_Sash,new List<ItemType>{ItemType.SelfCleanser}),
            new BetaTypedItem(ItemData.Mercurial_Scimitar,new List<ItemType>{ItemType.SelfCleanser}),
            new BetaTypedItem(ItemData.Dervish_Blade,new List<ItemType>{ItemType.SelfCleanser}),

            new BetaTypedItem(ItemData.Mikaels_Crucible,new List<ItemType>{ItemType.Cleanser}),

            new BetaTypedItem(ItemData.Face_of_the_Mountain,new List<ItemType>{ItemType.Shield}),

        };

        private static List<TypedItem> _availableItems = new List<TypedItem>();

        static ItemManager()
        {
            Console.WriteLine("Set Up");
            Menu = new Menu(Program.Champion.Player.ChampionName + " : Item Manager", ITEMMANAGER_ROOT);
            foreach (var item in SUPPORTED_ITEMS)
            {
                Menu.AddSubMenu(item.InitMenu());
            }

            Shop.OnBuyItem += (hero, args) =>
            {
                if (!hero.IsMe || _availableItems.FirstOrDefault(tItem => tItem.Item.GetItem().Id == (int)args.Id) != null) return;
                var item = SUPPORTED_ITEMS.FirstOrDefault(tItem => tItem.Item.GetItem().Id == (int)args.Id);
                if (item != null)
                    _availableItems.Add(item);
            };

            Game.OnUpdate += OnUpdate;

        }

        internal static void OnUpdate(EventArgs args)
        {
            if (Program.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || Program.Orbwalker.GetTarget() == null) return;
            foreach (var item in _availableItems.Where(tItem =>
                         tItem.Item.GetItem().IsOwned(Program.Champion.Player)
                         && tItem.Item.GetItem().IsReady()
                         && tItem.CanBeUsed()))
            {
                item.Use();
            }
        }
    }
}
