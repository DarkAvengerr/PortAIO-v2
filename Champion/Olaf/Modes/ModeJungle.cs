using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Olaf.Common;
using Color = SharpDX.Color;
using EloBuddy;

namespace Olaf.Modes
{
    internal static class ModeJungle
    {
        public static Menu MenuLocal { get; private set; }
        private static Spell Q => Champion.PlayerSpells.Q;
        private static Spell W => Champion.PlayerSpells.W;
        private static Spell E => Champion.PlayerSpells.E;
        private static Spell R => Champion.PlayerSpells.R;

        public static void Init(Menu mainMenu)
        {
            MenuLocal = new Menu("Jungle", "Jungle");
            {

                InitSimpleMenu();
                //InitAdvancedMenu();

                MenuLocal.AddItem(new MenuItem("Jungle.Youmuu.BaronDragon", "Items: Use for Baron/Dragon").SetValue(new StringList(new[] {"Off", "Dragon", "Baron", "Both"}, 3))).SetFontStyle(FontStyle.Regular, Colors.ColorItems);
                MenuLocal.AddItem(new MenuItem("Jungle.Youmuu.BlueRed", "Items: Use for Blue/Red").SetValue(new StringList(new[] { "Off", "Red", "Blue", "Both" }, 3))).SetFontStyle(FontStyle.Regular, Colors.ColorItems);
                MenuLocal.AddItem(new MenuItem("Jungle.Item", "Items: Other (Like Tiamat/Hydra)").SetValue(new StringList(new[] { "Off", "On" }, 1))).SetFontStyle(FontStyle.Regular, Colors.ColorItems);

            }
            mainMenu.AddSubMenu(MenuLocal);
            //InitRefreshMenuItems();
            Game.OnUpdate += OnUpdate;
        }

        static void InitSimpleMenu()
        {
            string[] strQSimple = new string[4];
            {
                strQSimple[0] = "Off";
                strQSimple[1] = "Just Big Mobs";
                for (var i = 2; i < 4; i++)
                {
                    strQSimple[i] = "Mob Count >= " + i;
                }
                MenuLocal.AddItem(new MenuItem("Jungle.Simple.UseQ", "Q:").SetValue(new StringList(strQSimple, 0))).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor()).SetTag(1);
            }

            string[] strSimpleW = new string[6];
            {
                strSimpleW[0] = "Off";
                strSimpleW[1] = "Just Big Mobs";
                for (var i = 2; i < 6; i++)
                {
                    strSimpleW[i] = "If need to AA count >= " + (i + 2);
                }
                MenuLocal.AddItem(new MenuItem("Jungle.Simple.UseW", "W:").SetValue(new StringList(strSimpleW, 0))).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor()).SetTag(1);
            }
            MenuLocal.AddItem(new MenuItem("Jungle.Simple.UseE", "E:").SetValue(new StringList(new[] { "Off", "On", "Just Big Mobs" }, 2)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor())).SetTag(1);
        }
        
        static void InitAdvancedMenu()
        {
            MenuLocal.AddItem(new MenuItem("Jungle.Advanced.UseQ.Big1", "Q: [Blue/Red]").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor())).SetTag(2);
            MenuLocal.AddItem(new MenuItem("Jungle.Advanced.UseQ.Big2", "Q: [Baron/Dragon]").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor())).SetTag(2);
            MenuLocal.AddItem(new MenuItem("Jungle.Advanced.UseQ.Big3", "Q: [Other Big Mobs]").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor())).SetTag(2);

            string[] strQ = new string[5];
            {
                strQ[0] = "Off";
                for (var i = 1; i < 5; i++)
                {
                    strQ[i] = "Mob Count >= " + i;
                }
                MenuLocal.AddItem(new MenuItem("Jungle.Advanced.UseQ.Small", "Q: [Small Mobs]").SetValue(new StringList(strQ, 0))).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor()).SetTag(2);
            }

            MenuLocal.AddItem(new MenuItem("Jungle.Advanced.UseW.Big1", "W: [Blue/Red]").SetValue(new StringList(new[] { "Off", "On" }, 1))).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.E.MenuColor()).SetTag(2);
            MenuLocal.AddItem(new MenuItem("Jungle.Advanced.UseW.Big2", "W: [Baron/Dragon]").SetValue(new StringList(new[] { "Off", "On" }, 1))).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.E.MenuColor()).SetTag(2);
            MenuLocal.AddItem(new MenuItem("Jungle.Advanced.UseW.Big3", "W: [Other Big Mobs]").SetValue(new StringList(new[] { "Off", "On" }, 1))).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.E.MenuColor()).SetTag(2);
            string[] strW = new string[6];
            {
                strW[0] = "Off";
                for (var i = 1; i < 6; i++)
                {
                    strW[i] = "Need to AA count >= " + i;
                }
                MenuLocal.AddItem(new MenuItem("Jungle.Advanced.UseE.Small", "E: [Small Mobs]").SetValue(new StringList(strW, 0))).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor()).SetTag(2);
            }
            
            MenuLocal.AddItem(new MenuItem("Jungle.Advanced.UseE.Big1", "E: [Blue/Red]").SetValue(new StringList(new[] { "Off", "On" }, 1))).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.E.MenuColor()).SetTag(2);
            MenuLocal.AddItem(new MenuItem("Jungle.Advanced.UseE.Big2", "E: [Baron/Dragon]").SetValue(new StringList(new[] { "Off", "On" }, 1))).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.E.MenuColor()).SetTag(2);
            MenuLocal.AddItem(new MenuItem("Jungle.Advanced.UseE.Big3", "E: [Other Big Mobs]").SetValue(new StringList(new[] { "Off", "On" }, 1))).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.E.MenuColor()).SetTag(2);
            MenuLocal.AddItem(new MenuItem("Jungle.Advanced.UseE.Big4", "E: [Small Mobs]").SetValue(new StringList(new[] { "Off", "On" }, 0))).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.E.MenuColor()).SetTag(2);
        }

        private static void InitRefreshMenuItems()
        {
            int argsValue = MenuLocal.Item("Jungle.Mode").GetValue<StringList>().SelectedIndex;

            foreach (var item in MenuLocal.Items)
            {
                item.Show(true);
                switch (argsValue)
                {
                    case 0:
                        if (item.Tag == 1)
                        {
                            item.Show(false);
                        }
                        break;
                    case 1:
                        if (item.Tag == 2)
                        {
                            item.Show(false);
                        }
                        break;
                }
            }
        }


        private static void OnUpdate(EventArgs args)
        {
            if (ModeConfig.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ModeConfig.MenuFarm.Item("Farm.Enable").GetValue<KeyBind>().Active)
            {
                ExecuteSimpleMode();
                //ExecuteAdvancedMode();
            }
        }

        private static void ExecuteSimpleMode()
        {
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mobs.Count <= 0)
            {
                return;
            }

            if (ObjectManager.Player.ManaPercent < CommonManaManager.JungleMinManaPercent(mobs[0]))
            {
                return;
            }

            if (Q.LSIsReady() && MenuLocal.Item("Jungle.Simple.UseQ").GetValue<StringList>().SelectedIndex != 0)
            {
                var qCount = MenuLocal.Item("Jungle.Simple.UseQ").GetValue<StringList>().SelectedIndex;

                if (qCount == 1)
                {
                    if (CommonManaManager.GetMobType(mobs[0], CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big)
                    {
                        Q.Cast(mobs[0].Position - 15);
                    }
                }
                else
                {
                    if (mobs.Count >= qCount)
                    {
                        Q.Cast(mobs[0].Position - 15);
                    }
                }
            }

            if (W.LSIsReady() && mobs[0].LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 165) && MenuLocal.Item("Jungle.Simple.UseW").GetValue<StringList>().SelectedIndex != 0)
            {
                var wCount = MenuLocal.Item("Jungle.Simple.UseW").GetValue<StringList>().SelectedIndex;
                if (wCount == 1)
                {
                    if (CommonManaManager.GetMobType(mobs[0], CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big)
                    {
                        W.Cast();
                    }
                }
                else
                {
                    var totalAa = ObjectManager.Get<Obj_AI_Minion>().Where(m =>m.Team == GameObjectTeam.Neutral &&m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 165)).Sum(mob => (int)mob.Health);

                    totalAa = (int)(totalAa / ObjectManager.Player.TotalAttackDamage);
                    if (totalAa >= wCount + 2 || CommonManaManager.GetMobType(mobs[0], CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big)
                    {
                        W.Cast();
                    }
                }
            }
                
            if (E.CanCast(mobs[0]) && MenuLocal.Item("Jungle.Simple.UseE").GetValue<StringList>().SelectedIndex != 0)
            {
                var qCount = MenuLocal.Item("Jungle.Simple.UseE").GetValue<StringList>().SelectedIndex;

                if (qCount == 1)
                {
                    if (CommonManaManager.GetMobType(mobs[0], CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big)
                    {
                        E.CastOnUnit(mobs[0]);
                    }
                }
                else
                {
                    E.CastOnUnit(mobs[0]);
                }
            }
        }


        private static void ExecuteAdvancedMode()
        {
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Champion.PlayerSpells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs.Count <= 0)
            {
                return;
            }

            var mob = mobs[0];

            if (ObjectManager.Player.ManaPercent < CommonManaManager.JungleMinManaPercent(mob))
            {
                return;
            }
            
            if (Champion.PlayerSpells.Q.LSIsReady() && mob.LSIsValidTarget(Champion.PlayerSpells.Q.Range))
            {
                if (MenuLocal.Item("Jungle.Advanced.UseQ.Big1").GetValue<StringList>().SelectedIndex != 0)
                {
                    if (CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Blue || CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Red)
                    {
                        if (Champion.PlayerSpells.Q.LSIsReady())
                        {
                            Champion.PlayerSpells.Q.Cast(mob);
                        }
                    }
                }

                if (MenuLocal.Item("Jungle.Advanced.UseQ.Big2").GetValue<StringList>().SelectedIndex != 0)
                {
                    if (CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Baron || CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Dragon)
                    {
                        if (Champion.PlayerSpells.Q.LSIsReady())
                        {
                            Champion.PlayerSpells.Q.Cast(mob);
                        }
                    }
                }

                if (mob.Health > ObjectManager.Player.TotalAttackDamage*2)
                {
                    if (MenuLocal.Item("Jungle.Advanced.UseQ.Big3").GetValue<StringList>().SelectedIndex != 0)
                    {
                        if (CommonManaManager.GetMobType(mob, CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big &&
                            (
                                CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Dragon || CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Baron
                                || CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Red || CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Blue)
                            )
                        {
                            if (Champion.PlayerSpells.Q.LSIsReady())
                            {
                                Champion.PlayerSpells.Q.Cast(mob);
                            }
                        }
                    }

                    if (MenuLocal.Item("Jungle.Advanced.UseQ.Small").GetValue<StringList>().SelectedIndex != 0)
                    {
                        if (CommonManaManager.GetMobType(mob, CommonManaManager.FromMobClass.ByType) != CommonManaManager.MobTypes.Big)
                        {
                            if (mobs.Count >= MenuLocal.Item("Jungle.Advanced.UseQ.Small").GetValue<StringList>().SelectedIndex)
                            {
                                if (Champion.PlayerSpells.Q.LSIsReady())
                                {
                                    Champion.PlayerSpells.Q.Cast(mob);
                                }
                            }
                        }
                    }
                }
            }

            if (Champion.PlayerSpells.E.LSIsReady() && mob.LSIsValidTarget(Champion.PlayerSpells.E.Range) && mob.Health > ObjectManager.Player.TotalAttackDamage * 3)
            {
                if (MenuLocal.Item("Jungle.Advanced.UseE.Big1").GetValue<StringList>().SelectedIndex != 0)
                {
                    if (CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Blue || CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Red)
                    {
                        if (Champion.PlayerSpells.E.LSIsReady())
                        {
                            Champion.PlayerSpells.E.Cast(mob);
                        }
                    }
                }

                if (MenuLocal.Item("Jungle.Advanced.UseE.Big2").GetValue<StringList>().SelectedIndex != 0)
                {
                    if (CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Baron || CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Dragon)
                    {
                        if (Champion.PlayerSpells.E.LSIsReady())
                        {
                            Champion.PlayerSpells.E.Cast(mob);
                        }
                    }
                }

                if (MenuLocal.Item("Jungle.Advanced.UseE.Big3").GetValue<StringList>().SelectedIndex != 0)
                {
                    if (CommonManaManager.GetMobType(mob, CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big &&
                        (
                            CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Dragon || CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Baron
                            || CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Red || CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Blue)
                        )
                    {
                        if (Champion.PlayerSpells.E.LSIsReady())
                        {
                            Champion.PlayerSpells.E.Cast(mob);
                        }
                    }
                }

                if (MenuLocal.Item("Jungle.Advanced.UseE.Big4").GetValue<StringList>().SelectedIndex != 0)
                {
                    if (CommonManaManager.GetMobType(mob, CommonManaManager.FromMobClass.ByType) != CommonManaManager.MobTypes.Big)
                    {
                        if (Champion.PlayerSpells.E.LSIsReady())
                        {
                            Champion.PlayerSpells.E.Cast(mob);
                        }
                    }
                }
            }
        }
    }
}
