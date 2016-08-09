using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using System.Drawing;
using EloBuddy;

namespace LSharpNunu
{
    enum Spells
    {
        Q, W, E, R
    }

    internal class Nunu
    {
        private const string ChampName = "Nunu";

        public static Orbwalking.Orbwalker Orbwalker;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static Obj_AI_Base Minionerimo;

        private static Menu _menu;
        private static Spell _q;
        private static Spell _w;
        private static Spell _e;
        private static Spell _r;
        private static SpellSlot _smiteSlot;
        private static bool _checkSmite = false;

        private static readonly string[] Epics =
        {
            "SRU_Baron", "SRU_Dragon"
        };
        private static readonly string[] Buffs =
        {
            "SRU_Red", "SRU_Blue"
        };
        private static readonly string[] Buffandepics =
        {
            "SRU_Red", "SRU_Blue", "SRU_Dragon", "SRU_Baron"
        };

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>()
        {
            { Spells.Q, new Spell(SpellSlot.Q, 220)},
            { Spells.W, new Spell(SpellSlot.W, 700)},
            { Spells.E, new Spell(SpellSlot.E, 550)},
            { Spells.R, new Spell(SpellSlot.R, 650)}
        };

        #region GameLoaded

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.BaseSkinName != ChampName)
                return;

            Notifications.AddNotification("LSharp - Nunu Loaded By BillyGG", 5000);

            NunuMenu.Initialize();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
            Drawing.OnEndScene += Drawings.OnDrawEndScene;
        }

        #endregion GameLoaded

        #region OnGameUpdate

        private static void OnGameUpdate(EventArgs args)
        {
            _smiteSlot = Player.GetSpellSlot(Smitetype());

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    if (NunuMenu._menu.Item("Nunu.Heal.AutoHeal").IsActive())
                        AutoQ();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                default:
                    if (NunuMenu._menu.Item("Nunu.Heal.AutoHeal").IsActive())
                        AutoQ();
                    break;
            }

            if (NunuMenu._menu.Item("Nunu.smiteEnabled").GetValue<KeyBind>().Active) Smiter();
        }

        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        private static string Smitetype()
        {
            if (SmiteBlue.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmiteplayerganker";
            }
            if (SmiteRed.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmiteduel";
            }
            if (SmiteGrey.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmitequick";
            }
            if (SmitePurple.Any(a => Items.HasItem(a)))
            {
                return "itemsmiteaoe";
            }
            return "summonersmite";
        }

        private static void Smiter()
        {
            var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(a => Buffandepics.Contains(a.BaseSkinName) && a.Distance(Player) <= 1300);
            if (minion != null)
            {
                if (NunuMenu._menu.Item(minion.BaseSkinName).GetValue<bool>())
                {
                    Minionerimo = minion;
                    if (SmiteDmg() > minion.Health && minion.IsValidTarget(780) && ParamBool("Nunu.normalSmite")) Player.Spellbook.CastSpell(_smiteSlot, minion);
                    if (minion.Distance(Player) < 100 && _checkSmite)
                    {
                        _checkSmite = false;
                        Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }

                }
            }
        }

        private static void QSteal()
        {
            var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(a => Buffandepics.Contains(a.BaseSkinName) && a.Distance(Player) <= 220);
            if (minion != null)
            {
                if (NunuMenu._menu.Item(minion.BaseSkinName).GetValue<bool>())
                {
                    Minionerimo = minion;
                    if (spells[Spells.Q].GetDamage(minion) > minion.Health && minion.IsValidTarget(780) && ParamBool("Nunu.stealq")) spells[Spells.Q].CastOnUnit(minion);
                    if (minion.Distance(Player) < 100 && spells[Spells.Q].IsReady())
                    {
                        spells[Spells.Q].CastOnUnit(minion);
                    }
                }
            }
        }

        private static bool ParamBool(String paramName)
        {
            return (NunuMenu._menu.Item(paramName).GetValue<bool>());
        }

        private static double SmiteDmg()
        {
            int[] dmg =
            {
                20*Player.Level + 370, 30*Player.Level + 330, 40*+Player.Level + 240, 50*Player.Level + 100
            };
            return Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready ? dmg.Max() : 0;
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Magical);

            if (NunuMenu._menu.Item("Nunu.Harass.E").GetValue<bool>() && spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast(target);
            }
        }

        private static void Clear()
        {
            var qLane = NunuMenu._menu.Item("Nunu.Clear.Q").GetValue<bool>();
            var wLane = NunuMenu._menu.Item("Nunu.Clear.W").GetValue<bool>();
            var eLane = NunuMenu._menu.Item("Nunu.Clear.E").GetValue<bool>();
            var minions = MinionManager.GetMinions(500, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);

            foreach (var minion in minions)
            {
                if (qLane)
                {
                    spells[Spells.Q].CastOnUnit(minion);
                }
                if (wLane)
                {
                    spells[Spells.W].Cast(Player);
                }
                if (eLane)
                {
                    spells[Spells.E].CastOnUnit(minion);
                }
            }
        }

        private static void Combo()
        {

            var rCombo = NunuMenu._menu.Item("Nunu.Combo.R").GetValue<bool>();
            var wCombo = NunuMenu._menu.Item("Nunu.Combo.W").GetValue<bool>();
            var eCombo = NunuMenu._menu.Item("Nunu.Combo.E").GetValue<bool>();
            var autoQ = NunuMenu._menu.Item("Nunu.Heal.AutoHeal").GetValue<bool>();

            var target = TargetSelector.GetTarget(1000f, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            if (autoQ && spells[Spells.Q].IsReady())
            {
                AutoQ();
            }

            if (eCombo && Player.Distance(target) <= spells[Spells.E].Range && spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast(target);
            }
            if (wCombo && Player.Distance(target) <= spells[Spells.W].Range && spells[Spells.W].IsReady())
            {
                spells[Spells.W].CastOnUnit(Player);
            }
            if (rCombo && Player.Distance(target) <= spells[Spells.R].Range / 2 && spells[Spells.R].IsReady())
            {
                CastR();
            }
        }

        private static void CastR()
        {
            if (!spells[Spells.R].IsReady())
                return;

            var Rcount = LeagueSharp.Common.Utility.CountEnemiesInRange(Player, spells[Spells.R].Range);
            var ReqRcount = NunuMenu._menu.Item("Nunu.Combo.RCount").GetValue<Slider>().Value;

            if (ReqRcount <= Rcount)
            {
                spells[Spells.R].Cast();
            }
        }

        private static void AutoQ()
        {
            var minions = MinionManager.GetMinions(500, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);
            var HealthPer = Player.Health / Player.MaxHealth * 100;
            var ReqHP = NunuMenu._menu.Item("Nunu.Heal.HP").GetValue<Slider>().Value;
            var autoQ = NunuMenu._menu.Item("Nunu.Heal.AutoHeal").GetValue<bool>();

            var autoQ2 = NunuMenu._menu.Item("Nunu.Heal.AutoHeal").GetValue<bool>();

            if (autoQ && minions.Count > 0 && HealthPer < ReqHP)
            {
                foreach (var minion in minions)
                {
                    if (minion.Health > 0)
                    {
                        spells[Spells.Q].Cast(minion);
                    }
                }
            }
        }

        #endregion OnGameUpdate

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (spells[Spells.E].IsReady())
            {
                damage += spells[Spells.E].GetDamage(enemy);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += spells[Spells.R].GetDamage(enemy);
            }

            return (float)(damage + Player.GetAutoAttackDamage(enemy));
        }
    }
}
