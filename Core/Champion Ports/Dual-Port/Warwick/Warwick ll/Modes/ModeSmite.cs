using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using WarwickII.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace WarwickII.Modes
{
    internal static class ModeSmite
    {
        public static Menu MenuLocal { get; private set; }

        public static SpellSlot SmiteSlot = SpellSlot.Unknown;

        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };

        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };

        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };

        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        private static readonly int SmiteRange = 550;

        public static void Init(Menu MenuParent)
        {
            MenuLocal = new Menu("Smite:", "Menu.Smite").SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor());

            SetSmiteSlot();
            if (SmiteSlot != SpellSlot.Unknown)
            {
                MenuLocal.AddItem(new MenuItem("Spells.Smite.Enemy", "Use Smite for Enemy!").SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Toggle)));
                MenuLocal.AddItem(new MenuItem("Spells.Smite.Monster", "Use Smite for Monsters!").SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Toggle)));
            }

            MenuParent.AddSubMenu(MenuLocal);

            Game.OnUpdate += args =>
            {
                if (SmiteSlot != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(SmiteSlot) == SpellState.Ready)
                {
                    if (Modes.ModeConfig.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        SmiteOnTarget();
                    }

                    SmiteOnMonters();
                }
            };
        }

        private static string Smitetype
        {
            get
            {
                if (SmiteBlue.Any(i => Items.HasItem(i)))
                {
                    return "s5_summonersmiteplayerganker";
                }

                if (SmiteRed.Any(i => Items.HasItem(i)))
                {
                    return "s5_summonersmiteduel";
                }

                if (SmiteGrey.Any(i => Items.HasItem(i)))
                {
                    return "s5_summonersmitequick";
                }

                if (SmitePurple.Any(i => Items.HasItem(i)))
                {
                    return "itemsmiteaoe";
                }

                return "summonersmite";
            }
        }

        private static void SetSmiteSlot()
        {
            foreach (
                var spell in
                    ObjectManager.Player.Spellbook.Spells.Where(
                        spell => string.Equals(spell.Name, Smitetype, StringComparison.CurrentCultureIgnoreCase)))
            {
                SmiteSlot = spell.Slot;
            }
        }

        private static int GetSmiteDmg()
        {
            int level = ObjectManager.Player.Level;
            int index = ObjectManager.Player.Level / 5;
            float[] dmgs = { 370 + 20 * level, 330 + 30 * level, 240 + 40 * level, 100 + 50 * level };
            return (int)dmgs[index];
        }

        private static void SmiteOnTarget()
        {
            if (!MenuLocal.Item("Spells.Smite.Enemy").GetValue<KeyBind>().Active)
            {
                return;
            }

            var t = TargetSelector.GetTarget(SmiteRange, TargetSelector.DamageType.Magical);

            if (!t.IsValidTarget())
            {
                return;
            }

            if (!t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65) || t.Health < GetSmiteDmg())
            {
                var itemCheck = SmiteBlue.Any(i => Items.HasItem(i)) || SmiteRed.Any(i => Items.HasItem(i));
                if (itemCheck && ObjectManager.Player.Spellbook.CanUseSpell(SmiteSlot) == SpellState.Ready &&
                    t.Distance(ObjectManager.Player.Position) < 700f)
                {
                    ObjectManager.Player.Spellbook.CastSpell(SmiteSlot, t);
                }
            }
        }

        private static void SmiteOnMonters()
        {
            if (!MenuLocal.Item("Spells.Smite.Monster").GetValue<KeyBind>().Active)
                return;

            if (ObjectManager.Player.Spellbook.CanUseSpell(SmiteSlot) != SpellState.Ready)
                return;

            string[] jungleMinions;
            if (LeagueSharp.Common.Utility.Map.GetMap().Type.Equals(LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline))
            {
                jungleMinions = new string[] { "TT_Spiderboss", "TT_NWraith", "TT_NGolem", "TT_NWolf" };
            }
            else
            {
                jungleMinions = new string[]
                {
                    "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Red", "SRU_Krug", "SRU_Dragon",
                    "SRU_Baron", "Sru_Crab"
                };
            }
            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, 1000, MinionTypes.All, MinionTeam.Neutral);
            if (minions.Any())
            {
                int smiteDmg = GetSmiteDmg();

                foreach (Obj_AI_Base minion in minions)
                {
                    if (LeagueSharp.Common.Utility.Map.GetMap().Type.Equals(LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline) &&
                        minion.Health <= smiteDmg &&
                        jungleMinions.Any(name => minion.Name.Substring(0, minion.Name.Length - 5).Equals(name)))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(SmiteSlot, minion);
                    }
                    if (minion.Health <= smiteDmg && jungleMinions.Any(name => minion.Name.StartsWith(name)) &&
                        !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(SmiteSlot, minion);
                    }
                }
            }
        }
    }
}
