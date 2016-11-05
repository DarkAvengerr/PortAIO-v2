using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using WarwickII.Champion;
using WarwickII.Common;
using Color = SharpDX.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace WarwickII.Modes
{
    internal static class ModeJungle
    {
        public static Menu MenuLocal { get; private set; }
        public static Menu MenuMinMana { get; private set; }

        private static Spell Q => PlayerSpells.Q;
        private static Spell W => PlayerSpells.W;
        private static Spell E => PlayerSpells.E;

        public static void Initialize(Menu ParentMenu)
        {
            MenuLocal = new Menu("Jungle", "Jungle");
            {
                MenuLocal.AddItem(new MenuItem("Jungle.Q.Use", "Q:").SetValue(new StringList(new[] { "Off", "On: Just for Big Mobs", "Use for All Mobs" }, 1))).SetFontStyle(FontStyle.Regular, PlayerSpells.Q.MenuColor());

                string[] strESimple = new string[5];
                {
                    strESimple[0] = "Off";
                    strESimple[1] = "Big Mobs";
                    for (var i = 2; i < 5; i++)
                    {
                        strESimple[i] = "If Need to AA Count >= " + (i + 2);
                    }
                    MenuLocal.AddItem(new MenuItem("Jungle.W.Use", "W:").SetValue(new StringList(strESimple, 4))).SetFontStyle(FontStyle.Regular, PlayerSpells.W.MenuColor());
                }

                MenuLocal.AddItem(new MenuItem("Jungle.MinMana", "Min. Mana %:").SetValue(new Slider(20, 100, 0))).SetFontStyle(FontStyle.Regular, Color.LightGreen);
                MenuLocal.AddItem(new MenuItem("Jungle.Item.Use", "Use Items").SetValue(true)).SetFontStyle(FontStyle.Regular, Colors.ColorItems);
            }
            ParentMenu.AddSubMenu(MenuLocal);
            Game.OnUpdate += OnUpdate;
        }

        static void InitSimpleMenu()
        {
 
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ModeConfig.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                ExecuteSimpleMode();
            }
        }

        private static void ExecuteSimpleMode()
        {
            if (!ModeConfig.MenuFarm.Item("Farm.Enable").GetValue<KeyBind>().Active)
            {
                return;
            }

            if (ObjectManager.Player.ManaPercent <= CommonManaManager.MinManaPercent(CommonManaManager.FarmMode.Jungle))
            {
                return;
            }

            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mobs.Count <= 0)
            {
                return;
            }

            var mob = mobs[0];

            if (!Common.CommonHelper.ShouldCastSpell(mob))
            {
                return;
            }


            var jUseQ = MenuLocal.Item("Jungle.Q.Use").GetValue<StringList>().SelectedIndex;
            if (jUseQ != 0 && Q.CanCast(mob))
            {
                if (CommonManaManager.GetMobType(mob, CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big)
                {
                    Champion.PlayerSpells.Cast.Q(mob);
                }

                if (jUseQ == 2 && mob.Health < PlayerSpells.WarwickDamage.Q(PlayerSpells.WarwickDamage.QFor.Enemy))
                {
                    Champion.PlayerSpells.Cast.Q(mob);
                }
            }


            if (MenuLocal.Item("Jungle.W.Use").GetValue<StringList>().SelectedIndex != 0 && W.IsReady() &&
                mob.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
            {
                var totalAa =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            m =>
                                m.Team == GameObjectTeam.Neutral &&
                                m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
                        .Sum(m => (int) m.Health);

                totalAa = (int) (totalAa / ObjectManager.Player.TotalAttackDamage);
                if (totalAa >= MenuLocal.Item("Jungle.W.Use").GetValue<StringList>().SelectedIndex + 2 ||
                    CommonManaManager.GetMobType(mobs[0], CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big)
                {
                    Champion.PlayerSpells.Cast.W();
                }
            }
        }
    }
}
