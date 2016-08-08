using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoSeries
{
    public class Program
    {
        public static Spell Q, Q2, W, W2, E, E2, R, R2;
        public static SpellSlot Smite, Ignite, Flash;
        public static Items.Item Bilgewater, BotRK, Youmuu, Tiamat, Hydra, Sheen, LichBane, IcebornGauntlet, TrinityForce, LudensEcho;
        public static LeagueSharp.Common.Menu MainMenu;
        public static BadaoSeries.CustomOrbwalker.Orbwalking.Orbwalker Orbwalker;
        public static bool enabled = true;

        public static bool Enable
        {
            get
            {
                return enabled;
            }

            set
            {
                enabled = value;
                if (MainMenu != null)
                {
                    MainMenu.Item("Enable").SetValue(value);
                }
            }
        }
        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static void OnLoad()
        {
            var plugin = Type.GetType("BadaoSeries.Plugin." + Player.ChampionName);
            if (plugin == null)
            {
                AddUI.Notif(Player.ChampionName + ": Not Supported !", 10000);
                return;
            }
            AddUI.Notif(Player.ChampionName + ": Loaded !", 10000);
            //Bootstrap.Init(null);
            //if (Player.ChampionName == "Rammus")
            //{
            //    LeagueSharp.SDK.Core.Orbwalker.Enabled = false;
            //    Menu Orb = new Menu("Orbwalker", "Orbwalker", true).Attach();
            //    Orbwalker.Orbwalker.Initialize(Orb);
            //}
            Bilgewater = new Items.Item(ItemData.Bilgewater_Cutlass.Id, 550);
            BotRK = new Items.Item(ItemData.Blade_of_the_Ruined_King.Id, 550);
            Youmuu = new Items.Item(ItemData.Youmuus_Ghostblade.Id, 0);
            Tiamat = new Items.Item(ItemData.Tiamat_Melee_Only.Id, 400);
            Hydra = new Items.Item(ItemData.Ravenous_Hydra_Melee_Only.Id, 400);
            Sheen = new Items.Item(ItemData.Sheen.Id, 0);
            LichBane = new Items.Item(ItemData.Lich_Bane.Id, 0);
            TrinityForce = new Items.Item(ItemData.Trinity_Force.Id, 0);
            IcebornGauntlet = new Items.Item(ItemData.Iceborn_Gauntlet.Id, 0);
            LudensEcho = new Items.Item(ItemData.Ludens_Echo.Id, 0);

            foreach (var spell in
                Player.Spellbook.Spells.Where(
                    i =>
                        i.Name.ToLower().Contains("smite") &&
                        (i.Slot == SpellSlot.Summoner1 || i.Slot == SpellSlot.Summoner2)))
            {
                Smite = spell.Slot;
            }
            Ignite = Player.LSGetSpellSlot("summonerdot");
            Flash = Player.LSGetSpellSlot("summonerflash");

            MainMenu = new LeagueSharp.Common.Menu("BadaoSeries", "BadaoSeries", true);
            AddUI.Bool(MainMenu, "Enable", Player.ChampionName + " Enable", true).ValueChanged += Program_ValueChanged;
            MainMenu.AddToMainMenu();
            NewInstance(plugin);
        }

        private static void Program_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (sender != null)
            {
                enabled = e.GetNewValue<bool>();
                CustomOrbwalker.Orbwalking.Orbwalker.Enabled = e.GetNewValue<bool>();
                if (e.GetNewValue<bool>())
                    AddUI.Notif(Player.ChampionName + ": Enabled !", 4000);
                else
                    AddUI.Notif(Player.ChampionName + ": Disabled !", 4000);
            }
        }



        private static void NewInstance(Type type)
        {
            var target = type.GetConstructor(Type.EmptyTypes);
            var dynamic = new DynamicMethod(string.Empty, type, new Type[0], target.DeclaringType);
            var il = dynamic.GetILGenerator();
            il.DeclareLocal(target.DeclaringType);
            il.Emit(OpCodes.Newobj, target);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            ((Func<object>)dynamic.CreateDelegate(typeof(Func<object>)))();
        }

    }
    public class AddUI : Program
    {
        public static void Notif(string msg, int time)
        {
            var x = new Notification("BadaoSeries:  " + msg,time);
            Notifications.AddNotification(x);
        }

        public static LeagueSharp.Common.MenuItem Separator(LeagueSharp.Common.Menu subMenu, string name, string display)
        {
            return subMenu.AddItem(new LeagueSharp.Common.MenuItem(name, display));
        }

        public static LeagueSharp.Common.MenuItem Bool(LeagueSharp.Common.Menu subMenu, string name, string display, bool state = true)
        {
            return subMenu.AddItem(new LeagueSharp.Common.MenuItem(name, display).SetValue(state));
        }

        public static LeagueSharp.Common.MenuItem KeyBind(LeagueSharp.Common.Menu subMenu,
            string name,
            string display,
            char key,
            KeyBindType type)
        {
            return subMenu.AddItem(new LeagueSharp.Common.MenuItem(name, display).SetValue<KeyBind>(new KeyBind(key,type)));
        }

        public static LeagueSharp.Common.MenuItem List(LeagueSharp.Common.Menu subMenu, string name, string display, string[] array)
        {
            return subMenu.AddItem(new LeagueSharp.Common.MenuItem(name, display).SetValue<StringList>(new StringList(array)));
        }

        public static LeagueSharp.Common.MenuItem Slider(LeagueSharp.Common.Menu subMenu, string name, string display, int cur, int min = 0, int max = 100)
        {
            return subMenu.AddItem(new LeagueSharp.Common.MenuItem(name, display).SetValue<Slider>(new Slider(cur,min,max)));
        }
    }
}
