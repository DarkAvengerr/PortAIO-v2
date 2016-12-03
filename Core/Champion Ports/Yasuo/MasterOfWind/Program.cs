using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Drawing;

using EloBuddy; 
using LeagueSharp.Common; 
namespace MasterOfWind
{
    class Program : Modes
    {
        private Menu menu;
        private String name;
        private String version;
        private AIHeroClient player;
        MenuItem dmgAfterComboItem;
        public Program()
        {
            menu = new Menu("Master Of Wind","MasterOfWind",true);
            name = "[Yasuo]Master Of Wind";
            version = "0.0.0.2";
        }

        public AIHeroClient getPlayer()
        {
            return player;
        }
        public Menu getMenu()
        {
            return menu;
        }
        public String getName()
        {
            return name;
        }
        public String getVersion()
        {
            return version;
        }
        public void loadMenu()
        {
            var orbWalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalking.Orbwalker orb = new Orbwalking.Orbwalker(orbWalkerMenu);
            var TargetSelectorMenu = new Menu("TargetSelector", "TargetSelector");
            var comboMenu = new Menu("Combo", "Combo");
            {
                comboMenu.AddItem(new MenuItem("QC", "Use Q in combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("EC", "Use E in combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("RC", "Use R in combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("NumER", "Num Enemys for R >=").SetValue(new Slider(1, 1, 5)));
                comboMenu.AddItem(new MenuItem("Ignite", "Use ignite for kill").SetValue(true));
                comboMenu.AddItem(new MenuItem("E to turretC", "Dont E to turret").SetValue(false));
                comboMenu.AddItem(new MenuItem("combokey", "Combo key").SetValue(new KeyBind(32, KeyBindType.Press)));
            }
            var HarrashMenu = new Menu("Harrash", "Harrash");
            {
                HarrashMenu.AddItem(new MenuItem("QH", "Use Q in Harrash").SetValue(true));
                HarrashMenu.AddItem(new MenuItem("EH", "Use E in Harrash").SetValue(true));
                HarrashMenu.AddItem(new MenuItem("Harrash key", "Harrash key").SetValue(new KeyBind('C', KeyBindType.Press)));
            }
            var DodgeRetards = new Menu("DodgePLS", "DodGeFor Retarding");
            {
                DodgeRetards.AddItem(new MenuItem("DW", "Use W for stop skillshots").SetValue(true));
                //  DodgeRetards.AddItem(new MenuItem("DWa", "Use W for stop autoAttacks").SetValue(false));
                //    DodgeRetards.AddItem(new MenuItem("DE", "Use E for stop skillshots").SetValue(true));
            }
            var LastHitMenu = new Menu("LastHit", "LastHit");
            {
                LastHitMenu.AddItem(new MenuItem("QLH", "Use Q in LastHit").SetValue(true));
                LastHitMenu.AddItem(new MenuItem("ELH", "Use E in combo").SetValue(true));
                LastHitMenu.AddItem(new MenuItem("LastHitkey", "Lasthit key").SetValue(new KeyBind('A', KeyBindType.Press)));
            }
            var LaneclearMenu = new Menu("Laneclear", "Laneclear");
            {
                LaneclearMenu.AddItem(new MenuItem("QL", "Use Q in Laneclear").SetValue(true));
                LaneclearMenu.AddItem(new MenuItem("EL", "Use E in Laneclear").SetValue(true));
                LaneclearMenu.AddItem(new MenuItem("ELHH", "E only killeble minion").SetValue(false));
                LaneclearMenu.AddItem(new MenuItem("E to turretL", "Dont E to turret").SetValue(false));
                LaneclearMenu.AddItem(new MenuItem("laneclearkey", "LaneClear key").SetValue(new KeyBind('V', KeyBindType.Press)));
            }
            var JungleclearMenu = new Menu("Jungleclear", "Jungleclear");
            {
                JungleclearMenu.AddItem(new MenuItem("QJ", "Use Q in JungleClear").SetValue(true));
                JungleclearMenu.AddItem(new MenuItem("EJ", "Use E in JungleClear").SetValue(true));
                JungleclearMenu.AddItem(new MenuItem("jungleclearkey", "JungleClear key").SetValue(new KeyBind('V', KeyBindType.Press)));
            }

            var FleeMenu = new Menu("Flee", "Flee");
            {
                FleeMenu.AddItem(new MenuItem("E to turretF", "Dont E to turret").SetValue(false));
                FleeMenu.AddItem(new MenuItem("Qflee", "Stack q in flee").SetValue(false));
                FleeMenu.AddItem(new MenuItem("Q3flee", "Use 3Q").SetValue(false));
                FleeMenu.AddItem(new MenuItem("fleekey", "Flee key").SetValue(new KeyBind('Z', KeyBindType.Press)));
            }
            /*  var ItemMenu = new Menu("Item Menu", "itemmenu");
              {
                  ItemMenu.AddItem(new MenuItem("Zhonias", "zhonias").SetValue(true));
              //    ItemMenu.AddItem(new MenuItem("xxxx", "xxxx").SetValue(true));
              //    ItemMenu.AddItem(new MenuItem("xxxx", "xxxx").SetValue(true));
              }*/
            var MiscMenu = new Menu("Misc", "Misc");
            {
                MiscMenu.AddItem(new MenuItem("QAuto","Auto use q to heros").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                  MiscMenu.AddItem(new MenuItem("QAutoMi", "Stack Q on minions").SetValue(new KeyBind("M".ToCharArray()[0], KeyBindType.Toggle)));
            }
            var DrawSettingsMenu = new Menu("Draw Settings", "Draw Settings");
            {
                DrawSettingsMenu.AddItem(new MenuItem("Draw Q Range", "Draw Q Range").SetValue(true));
                DrawSettingsMenu.AddItem(new MenuItem("Draw W Range", "Draw W Range").SetValue(true));
                DrawSettingsMenu.AddItem(new MenuItem("Draw E Range", "Draw E Range").SetValue(true));
                DrawSettingsMenu.AddItem(new MenuItem("Draw R Range", "Draw R Range").SetValue(true));
            }
            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw damage after combo").SetValue(true);
            DrawSettingsMenu.AddItem(dmgAfterComboItem);
            TargetSelector.AddToMenu(TargetSelectorMenu);
            menu.AddSubMenu(orbWalkerMenu);        //ORBWALKER
            menu.AddSubMenu(TargetSelectorMenu);   //TS
            menu.AddSubMenu(comboMenu);//COMBO
            menu.AddSubMenu(HarrashMenu);  //Harrash
            //  menu.AddSubMenu(ItemMenu);
            menu.AddSubMenu(DodgeRetards);
            menu.AddSubMenu(FleeMenu);
            menu.AddSubMenu(LastHitMenu);
            menu.AddSubMenu(LaneclearMenu);        //LANECLEAR
            menu.AddSubMenu(JungleclearMenu);      //JUNGLECLEAR
            menu.AddSubMenu(MiscMenu); 
            menu.AddSubMenu(DrawSettingsMenu);     //DRAWS
            menu.AddToMainMenu();
        }
  
        public void draw(EventArgs args)
        {
            if (player.IsDead) return;
            if (menu.Item("Draw Q Range").GetValue<bool>())
            {
                if(!base.getSkills().HaveQ3)
                Render.Circle.DrawCircle(getPlayer().Position, 475f, System.Drawing.Color.GreenYellow, 3);
                else 
                Render.Circle.DrawCircle(getPlayer().Position, 1060f, System.Drawing.Color.GreenYellow, 3);
            }
            if (menu.Item("Draw W Range").GetValue<bool>())
                Render.Circle.DrawCircle(getPlayer().Position, 400f, System.Drawing.Color.GreenYellow, 3);
            if (menu.Item("Draw E Range").GetValue<bool>())
                Render.Circle.DrawCircle(getPlayer().Position, 475f, System.Drawing.Color.GreenYellow, 3);
            if (menu.Item("Draw R Range").GetValue<bool>())
                Render.Circle.DrawCircle(getPlayer().Position, 1200f, System.Drawing.Color.GreenYellow,3);

            /*    LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = base.getMainComboDamage;
                LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
                dmgAfterComboItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };/
            */

            //laneclear jungleclear
      
        }
        
        public void load(EventArgs args)
        {
            player = ObjectManager.Player;
            if (player.ChampionName != "Yasuo") return;
            Utils.ShowNotification(getName() + " load good luck ;) " + getVersion(), System.Drawing.Color.White, 100);
            loadMenu();
            Drawing.OnDraw += draw;
            GameObject.OnCreate += base.gameObject_OnCreate;
            Game.OnUpdate += update;
        }
        public void update(EventArgs args)
        {
            if (getPlayer().IsDead) return;
            updateModes();
        }
        public void updateModes()
        {
            if (getMenu().Item("combokey").GetValue<KeyBind>().Active) base.combo(base.getTarget());
            if (getMenu().Item("jungleclearkey").GetValue<KeyBind>().Active) base.jungleClear();
            if (getMenu().Item("laneclearkey").GetValue<KeyBind>().Active) base.laneClear();
            if (getMenu().Item("Harrash key").GetValue<KeyBind>().Active) base.harrash(base.getTarget());
            if (getMenu().Item("fleekey").GetValue<KeyBind>().Active) base.flee(base.getTarget());
            if (getMenu().Item("LastHitkey").GetValue<KeyBind>().Active) base.lastHit(base.getTarget());
            if (getMenu().Item("QAuto").GetValue<KeyBind>().Active) base.autoQTarget(base.getTarget());
            if (getMenu().Item("QAutoMi").GetValue<KeyBind>().Active) base.autoQMinion(base.getTarget());

        }

        public static void Main()
        {
            Program p = new Program();
            p.load(p);
            p.load(new EventArgs());
        }


    }
}
