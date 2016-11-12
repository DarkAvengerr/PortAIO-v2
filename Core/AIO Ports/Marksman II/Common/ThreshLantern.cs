using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Marksman.Common
{
    public static class ThreshLantern
    {
        public static Menu MenuLocal { get; private set; }
        private static GameObject Lantern { get; set; }
        
        public static void Init(Menu nParentMenu)
        {
            MenuLocal = new Menu("Thresh", "Menu.Thresh");
            {
                MenuLocal.AddItem(new MenuItem("Use.Lantern", "Use").SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Press, true)));

                var useMenuItemName = "Use." + ObjectManager.Player.ChampionName;
                var useMenuItemText = "Use " + ObjectManager.Player.ChampionName;

                switch (ObjectManager.Player.ChampionName.ToLower())
                {
                    case "caitlyn":
                    {
                        MenuLocal.AddItem(new MenuItem(useMenuItemName, useMenuItemText + " E").SetValue(false));
                        break;
                    }

                    case "corki":
                    {
                        MenuLocal.AddItem(new MenuItem(useMenuItemName, useMenuItemText + " W").SetValue(false));
                        break;
                    }

                    case "ezreal":
                    {
                        MenuLocal.AddItem(new MenuItem(useMenuItemName, useMenuItemText + " E").SetValue(false));
                        break;
                    }

                    case "graves":
                    {
                        MenuLocal.AddItem(new MenuItem(useMenuItemName, useMenuItemText + " E").SetValue(false));
                        break;
                    }

                    case "lucian":
                    {
                        MenuLocal.AddItem(new MenuItem(useMenuItemName, useMenuItemText + " E").SetValue(false));
                        break;
                    }

                    case "tristana":
                    {
                        MenuLocal.AddItem(new MenuItem(useMenuItemName, useMenuItemText + " W").SetValue(false));
                        break;
                    }

                    case "vayne":
                    {
                        MenuLocal.AddItem(new MenuItem(useMenuItemName, useMenuItemText + " Q").SetValue(false));
                        break;
                    }
                }

            }
            nParentMenu.AddSubMenu(MenuLocal);
            
            Game.OnUpdate += OnGameUpdate;
            GameObject.OnCreate += OnGameObjectCreate;
            GameObject.OnDelete += OnGameObjectDelete;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || Lantern == null || !Lantern.IsValid || !MenuLocal.Item("Use.Lantern").GetValue<KeyBind>().Active)
            {
                return;
            }

            if (Lantern.Position.Distance(ObjectManager.Player.Position) <= 500)
            {
                ObjectManager.Player.Spellbook.CastSpell((SpellSlot)62, Lantern);
            }
            else if (ChampionSpell != null && ChampionSpell.IsReady() && Lantern.Position.Distance(ObjectManager.Player.Position) <= 500 + ChampionSpell.Range)
            {
                ChampionSpell.Cast(Lantern.Position);
            }
        }

        private static void OnGameObjectCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid || !sender.IsAlly || sender.Type != GameObjectType.obj_AI_Minion)
            {
                return;
            }

            if (sender.Name.Equals("Lantern", StringComparison.OrdinalIgnoreCase))
            {
                Lantern = sender;
            }
        }

        private static void OnGameObjectDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid || Lantern == null)
            {
                return;
            }
            if (sender.NetworkId == Lantern.NetworkId)
            {
                Lantern = null;
            }
        }

        private static Spell ChampionSpell
        {
            get
            {
                switch (ObjectManager.Player.ChampionName.ToLower())
                {
                    case "caitlyn":
                        return new Spell(SpellSlot.Q, 700);

                    case "corki":
                        return new Spell(SpellSlot.Q, 700);

                    case "ezreal":
                        return new Spell(SpellSlot.Q, 700);

                    case "graves":
                        return new Spell(SpellSlot.Q, 700);

                    case "lucian":
                        return new Spell(SpellSlot.Q, 700);

                    case "tristana":
                        return new Spell(SpellSlot.Q, 700);

                    case "vayne":
                        return new Spell(SpellSlot.Q, 700);
                }
                return null;
            }
        }
    }
}
