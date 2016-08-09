using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace UnderratedAIO.Helpers
{
    public class Jungle
    {
        public static AIHeroClient player = ObjectManager.Player;

        private static readonly string[] jungleMonsters =
        {
            "TT_Spiderboss", "SRU_Blue", "SRU_Red", "SRU_Dragon",
            "SRU_Baron"
        };

        public static readonly string[] bosses = { "TT_Spiderboss", "SRU_Dragon", "SRU_Baron" };
        public static SpellSlot smiteSlot = SpellSlot.Unknown;
        public static Spell smite;
        public static Menu CommonmenuMenu;

        public static Obj_AI_Base GetNearest(Vector3 pos, float range = 1500f)
        {
            return
                MinionManager.GetMinions(pos, range, MinionTypes.All, MinionTeam.NotAlly)
                    .FirstOrDefault(
                        minion =>
                            minion.IsValidTarget() && minion.IsValid && minion.Distance(pos) < range &&
                            jungleMonsters.Any(name => minion.Name.StartsWith(name)) && !minion.Name.Contains("Mini") &&
                            !minion.Name.Contains("Spawn"));
        }

        public static double smiteDamage(Obj_AI_Base target)
        {
            return player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Smite);
        }

        public static Menu addJungleOptions(Menu config)
        {
            Menu menuS = new Menu("Smite ", "Smitesettings");
            menuS.AddItem(new MenuItem("useSmite", "Use Smite"))
                .SetValue(new KeyBind("M".ToCharArray()[0], KeyBindType.Toggle))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            menuS.AddItem(new MenuItem("smiteStatus", "Show status")).SetValue(false);
            config.AddSubMenu(menuS);
            CommonmenuMenu = config;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            return config;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            ShowSmiteStatus();
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            CastSmite();
        }

        public static void ShowSmiteStatus()
        {
            if (CommonmenuMenu.Item("smiteStatus").GetValue<bool>() && smiteSlot != SpellSlot.Unknown)
            {
                if (CommonmenuMenu.Item("useSmite").GetValue<KeyBind>().Active)
                {
                    Drawing.DrawCircle(player.Position, 570f, System.Drawing.Color.LimeGreen);
                }
                else
                {
                    Drawing.DrawCircle(player.Position, 570f, System.Drawing.Color.Red);
                }
            }
        }

        public static void CastSmite()
        {
            if (CommonmenuMenu.Item("useSmite").GetValue<KeyBind>().Active && smiteSlot != SpellSlot.Unknown)
            {
                var target = GetNearest(player.Position);
                bool smiteReady = ObjectManager.Player.Spellbook.CanUseSpell(smiteSlot) == SpellState.Ready;
                if (target != null)
                {
                    if (smite.CanCast(target) && smiteReady && player.Distance(target.Position) <= smite.Range &&
                        smiteDamage(target) >= target.Health)
                    {
                        smite.Cast(target);
                    }
                }
            }
        }

        public static bool SmiteReady(bool enabled)
        {
            if (enabled && smiteSlot != SpellSlot.Unknown)
            {
                return ObjectManager.Player.Spellbook.CanUseSpell(smiteSlot) == SpellState.Ready;
            }
            return false;
        }

        //Kurisu
        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3724, 3723, 3933 };

        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719, 3932, 1410, 1409, 1408, 1411 };
        // 1410, 1409, 1408, 1411

        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714, 3931, 1415, 1412, 1419, 1414, 1413 };
        //1414, 1413

        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707, 3930, 1402, 1401, 1400, 1403 };
        // 1402, 1401, 1400, 1403

        public static string smitetype()
        {
            if (SmiteBlue.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteplayerganker";
            }
            if (SmiteRed.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteduel";
            }
            if (SmiteGrey.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmitequick";
            }
            if (SmitePurple.Any(id => Items.HasItem(id)))
            {
                return "itemsmiteaoe";
            }
            return "summonersmite";
        }

        public static void setSmiteSlot()
        {
            foreach (var spell in
                ObjectManager.Player.Spellbook.Spells.Where(
                    spell => String.Equals(spell.Name, smitetype(), StringComparison.CurrentCultureIgnoreCase)))
            {
                smiteSlot = spell.Slot;
                smite = new Spell(smiteSlot, 700);
                return;
            }
        }

        public static void CastSmite(Obj_AI_Minion target)
        {
            smite.Cast(target);
        }
    }
}