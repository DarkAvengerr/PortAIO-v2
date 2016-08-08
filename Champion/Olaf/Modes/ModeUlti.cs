using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Olaf.Common;
using Olaf.Evade;
using SpellData = Olaf.Evade.SpellData;
using EloBuddy;

namespace Olaf.Modes
{
    internal static class ModeUlti
    {
        public static Menu MenuLocal { get; private set; }
        private static Spell R => Champion.PlayerSpells.R;

        private static readonly BuffType[] BuffList =
        {
            BuffType.Stun, BuffType.Blind, BuffType.Charm, BuffType.Fear,
            BuffType.Knockback, BuffType.Knockup, BuffType.Taunt, BuffType.Slow, BuffType.Silence, BuffType.Disarm,
            BuffType.Snare
        };

        private static readonly string[] BuffListCaption =
        {
            "Stun", "Blind", "Charm", "Fear", "Knockback", "Knockup",
            "Taunt", "Slow", "Silence", "Disarm", "Snare"
        };

        public static void Init(Menu ParentMenu)
        {
            MenuLocal = new Menu("R:", "MenuR");

            var buffMenu = new Menu("Buffs:", "SubMenu.Buffs");
            foreach (var displayName in BuffListCaption)
            {
                buffMenu.AddItem(new MenuItem("Buff." + displayName, displayName).SetValue(true));
            }
            MenuLocal.AddSubMenu(buffMenu);

            //foreach (var hero in HeroManager.Enemies)
            //{

            //    foreach (var spell in SpellDatabase.Spells)
            //    {


            //        if (String.Equals(spell.ChampionName, hero.ChampionName, StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            MenuLocal.AddItem(new MenuItem(spell.ChampionName + spell.Slot + "AAA", spell.ChampionName + " : " + spell.Slot).SetValue(true));
            //        }
            //    }

            //}

            var championMenu = new Menu("Enemy Spells:", "SubMenu.Champions");
            foreach (
                var c in
                    HeroManager.Enemies.SelectMany(
                        t =>
                            Evade.SpellDatabase.Spells.Where(s => s.Type == SpellData.SkillShotType.SkillshotTargeted)
                                .Where(
                                    c =>
                                        string.Equals(c.ChampionName, t.ChampionName,
                                            StringComparison.InvariantCultureIgnoreCase))
                                .OrderBy(s => s.ChampionName)))
            {
                championMenu.AddItem(
                    new MenuItem("BuffT." + c.ChampionName + c.Slot, c.ChampionName + " : " + c.Slot).SetValue(true));
            }

            MenuLocal.AddItem(
                new MenuItem("MenuR.R.Enabled", "Enabled:").SetValue(new KeyBind("K".ToCharArray()[0],
                    KeyBindType.Toggle, true))).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor());
            MenuLocal.AddItem(
                new MenuItem("MenuR.R.OnyChampionSpells", "Dodge Only Champion Spells:").SetValue(
                    new KeyBind(Modes.ModeConfig.MenuConfig.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)))
                .SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.R.MenuColor());

            MenuLocal.AddSubMenu(championMenu);
            ParentMenu.AddSubMenu(MenuLocal);

            Obj_AI_Base.OnProcessSpellCast += ObjAiHeroOnOnProcessSpellCast;
            Game.OnUpdate += GameOnOnUpdate;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (!MenuLocal.Item("MenuR.R.Enabled").GetValue<KeyBind>().Active)
            {
                return;
            }

            if (MenuLocal.Item("MenuR.R.OnyChampionSpells").GetValue<KeyBind>().Active)
            {
                return;
            }

            if (!R.LSIsReady())
            {
                return;
            }

            ExecuteUltimateForBuffs();
        }

        private static void ExecuteUltimateForBuffs()
        {
            for (int i = 0; i < BuffListCaption.Length; i++)
            {
                if (MenuLocal.Item("Buff." + BuffListCaption[i]).GetValue<bool>() &&
                    ObjectManager.Player.HasBuffOfType(BuffList[i]))
                {
                    R.Cast();
                }
            }
        }

        private static void ObjAiHeroOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!R.LSIsReady() || !MenuLocal.Item("MenuR.R.Enabled").GetValue<KeyBind>().Active)
            {
                return;
            }

            if (!(sender is AIHeroClient) || !sender.LSIsValidTarget(1500) || sender.IsMe || sender.IsAlly)
            {
                return;
            }

            if (sender.IsDead)
            {

                return;
            }

            if (!sender.LSIsValidTarget(Champion.PlayerSpells.Q.Range))
            {
                return;
            }

            if (sender.Team == ObjectManager.Player.Team || !args.Target.IsMe)
            {
                return;
            }

            if (sender.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            foreach (
                var spell in
                    SpellDatabase.Spells.Where(s => s.Type == SpellData.SkillShotType.SkillshotTargeted)
                        .Where(
                            spell =>
                                args.Target.IsMe && spell.Slot == args.Slot &&
                                string.Equals(((AIHeroClient) sender).ChampionName, spell.ChampionName,
                                    StringComparison.InvariantCultureIgnoreCase) &&
                                MenuLocal.Item("BuffT." + spell.ChampionName + spell.Slot) != null &&
                                MenuLocal.Item("BuffT." + spell.ChampionName + spell.Slot).GetValue<bool>()))
            {
                R.Cast();
            }
        }
    }
}