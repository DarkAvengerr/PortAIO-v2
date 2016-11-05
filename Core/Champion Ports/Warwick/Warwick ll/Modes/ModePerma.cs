using System;
using System.Drawing;
using System.Linq;
using WarwickII.Common;
using LeagueSharp;
using LeagueSharp.Common;
using WarwickII.Champion;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace WarwickII.Modes
{
    internal class ModePerma
    {
        public static Menu MenuLocal { get; private set; }
        public static void Initialize(Menu MenuParent)
        {
            if (MenuParent == null)
                throw new ArgumentNullException(nameof(MenuParent));

            MenuLocal = new Menu("Perma Active", "Perma");
            {
                MenuLocal.AddItem(new MenuItem("Perma.Q", "Q:").SetValue(true)).SetFontStyle(FontStyle.Regular, PlayerSpells.Q.MenuColor());
                MenuLocal.AddItem(new MenuItem("Perma.Ignite", "Ignite: Killable Enemy:").SetValue(new StringList(new []{ "Off", "Combo Mode", "Everytime" }, 2))).SetFontStyle(FontStyle.Regular, PlayerSpells.W.MenuColor());
                MenuLocal.AddItem(new MenuItem("Perma.Smite", "Smite: Killable Enemy:").SetValue(new StringList(new[] { "Off", "Combo Mode", "Everytime" }, 2))).SetFontStyle(FontStyle.Regular, PlayerSpells.W.MenuColor());
            }
            MenuParent.AddSubMenu(MenuLocal);

            Game.OnUpdate += GameOnOnUpdate;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            #region Spell: Q
            if (MenuLocal.Item("Perma.Q").GetValue<bool>() && Modes.ModeConfig.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                var oEnemy = HeroManager.Enemies.FirstOrDefault(e => e.IsValidTarget(PlayerSpells.Q.Range));
                if (oEnemy != null)
                {
                    PlayerSpells.Cast.Q(oEnemy);
                }
            }
            #endregion Spell: Q

            #region Ignite
            if (MenuLocal.Item("Perma.Ignite").GetValue<StringList>().SelectedIndex != 0 && PlayerSpells.IgniteSlot.IsReady())
            {
                var oEnemy = HeroManager.Enemies.FirstOrDefault(e => e.IsValidTarget(550));
                if (oEnemy != null && ObjectManager.Player.GetSummonerSpellDamage(oEnemy, Damage.SummonerSpell.Ignite) > oEnemy.Health)
                {
                    if (MenuLocal.Item("Perma.Ignite").GetValue<StringList>().SelectedIndex == 1 && ModeConfig.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                        PlayerSpells.Cast.Ignite(oEnemy);

                    if (MenuLocal.Item("Perma.Ignite").GetValue<StringList>().SelectedIndex == 2 && !ObjectManager.Player.IsRecalling())
                        PlayerSpells.Cast.Ignite(oEnemy);
                }
            }
            #endregion Ignite

            #region Smite
            if (MenuLocal.Item("Perma.Smite").GetValue<StringList>().SelectedIndex != 0 && PlayerSpells.SmiteSlot.IsReady())
            {
                var oEnemy = HeroManager.Enemies.FirstOrDefault(e => e.IsValidTarget(550));
                if (oEnemy != null && ObjectManager.Player.GetSummonerSpellDamage(oEnemy, Damage.SummonerSpell.Smite) > oEnemy.Health)
                {
                    if (MenuLocal.Item("Perma.Smite").GetValue<StringList>().SelectedIndex == 1 && ModeConfig.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                        PlayerSpells.Cast.Smite(oEnemy);

                    if (MenuLocal.Item("Perma.Smite").GetValue<StringList>().SelectedIndex == 2 && !ObjectManager.Player.IsRecalling())
                        PlayerSpells.Cast.Smite(oEnemy);
                }
            }
            #endregion Smite
        }
    }
}
