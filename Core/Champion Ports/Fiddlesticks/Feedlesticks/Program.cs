using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Feedlesticks.Core;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace Feedlesticks
{
    class Program
    {
        /// <summary>
        /// Fiddle (easy)
        /// </summary>
        public static AIHeroClient FiddleStick = ObjectManager.Player;

        /// <summary>
        /// OnLoad section
        /// </summary>
        /// <param name="args"></param>
        public static void Game_OnGameLoad()
        {
            FiddleStick = ObjectManager.Player;
            Menus.Config = new Menu("Feedlestick", "Feedlestick", true);
            {
                Spells.Init();
                Menus.OrbwalkerInit();
                Menus.Init();
            }

            Obj_AI_Base.OnProcessSpellCast += Spellbook_OnCastSpell;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell1;
        }

        private static void Spellbook_OnCastSpell1(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && IsWActive)
            {
                args.Process = false;
            }
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (IsWActive)
                args.Process = false;
        }

        private static bool IsWActive
        {
            get
            {
                return Player.HasBuff("Drain") || Spells.W.IsChanneling || Player.Instance.Spellbook.IsChanneling;
            }
        }

        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
           // if (IsWActive)
                //args.Process = false;
        }

        /// <summary>
        /// Drawing Stuff
        /// </summary>
        /// <param name="args"></param>
        private static void OnDraw(EventArgs args)
        {
            if (FiddleStick.IsDead)
            {
                return;
            }
            if (Spells.Q.IsReady() && Helper.Active("q.draw"))
            {
                Helper.Circle("q.draw", Spells.Q.Range);
            }
            if (Spells.W.IsReady() && Helper.Active("w.draw"))
            {
                Helper.Circle("w.draw", Spells.W.Range);
            }
            if (Spells.E.IsReady() && Helper.Active("e.draw"))
            {
                Helper.Circle("e.draw", Spells.E.Range);
            }
            if (Spells.R.IsReady() && Helper.Active("r.draw"))
            {
                Helper.Circle("r.draw", Spells.R.Range);
            }
        }

        /// <summary>
        /// W lock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Spellbook_OnCastSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "Drain")
            {
                Menus.Orbwalker.SetAttack(false);
                Menus.Orbwalker.SetAttack(false);
            }
        }

        private static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.DisplayName == "Drain")
            {
                Menus.Orbwalker.SetAttack(true);
                Menus.Orbwalker.SetAttack(true);
            }
        }

        /// <summary>
        /// Combo stuff and immobile stuff
        /// </summary>
        /// <param name="args"></param>
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (FiddleStick.IsDead)
            {
                return;
            }

            if (IsWActive)
            {
                Menus.Orbwalker.SetAttack(false);
                Menus.Orbwalker.SetMovement(false);
            }
            else
            {
                Menus.Orbwalker.SetAttack(true);
                Menus.Orbwalker.SetMovement(true);
                switch (Menus.Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        Combo();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        Harass();
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        Jungle();
                        WaveClear();

                        break;

                }

                if (Helper.Enabled("auto.q.immobile"))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range) && Helper.Enabled("q.enemy." + x.ChampionName) && Helper.IsEnemyImmobile(x)))
                    {
                        Spells.Q.Cast(enemy);
                    }
                }
                if (Helper.Enabled("auto.q.channeling"))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range) && Helper.Enabled("q.enemy." + x.ChampionName) && x.IsChannelingImportantSpell()))
                    {
                        Spells.Q.Cast(enemy);
                    }
                }
                if (Helper.Enabled("auto.e.immobile"))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range) && Helper.Enabled("e.enemy." + x.ChampionName) && Helper.IsEnemyImmobile(x)))
                    {
                        Spells.E.Cast(enemy);
                    }
                }
                if (Helper.Enabled("auto.e.channeling"))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range) && Helper.Enabled("e.enemy." + x.ChampionName) && x.IsChannelingImportantSpell()))
                    {
                        Spells.E.Cast(enemy);
                    }
                }
            }
        }

        private static void Harass()
        {
            if (IsWActive)
                return;
            if (ObjectManager.Player.ManaPercent < Helper.Slider("harass.mana"))
            {
                return;
            }

            if (Spells.Q.IsReady() && Helper.Enabled("q.harass") && !IsWActive)
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(Spells.Q.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Helper.Enabled("q.enemy." + enemy.ChampionName))
                    {
                        Spells.Q.CastOnUnit(enemy);
                    }
                }
            }
            if (Spells.E.IsReady() && Helper.Enabled("e.harass") && !IsWActive)
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(Spells.E.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Helper.Enabled("e.enemy." + enemy.ChampionName))
                    {
                        Spells.E.CastOnUnit(enemy);
                    }
                }
            }
        }

        private static void Jungle()
        {
            if (IsWActive)
                return;
            if (ObjectManager.Player.ManaPercent < Helper.Slider("jungle.mana"))
            {
                return;
            }

            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob.Count > 0)
            {
                if (Spells.Q.IsReady() && Helper.Enabled("q.jungle") && !IsWActive)
                {
                    Spells.Q.CastOnUnit(mob[0]);
                }
                if (Spells.W.IsReady() && Helper.Enabled("w.jungle"))
                {
                    Spells.W.CastOnUnit(mob[0]);
                }
                if (Spells.E.IsReady() && Helper.Enabled("e.jungle") && !IsWActive)
                {
                    Spells.E.CastOnUnit(mob[0]);
                }

            }
        }
        /// <summary>
        /// Combo
        /// </summary>
        private static void Combo()
        {
            if (IsWActive)
                return;
            if (Spells.Q.IsReady() && Helper.Enabled("q.combo") && !IsWActive)
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(Spells.Q.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Helper.Enabled("q.enemy." + enemy.ChampionName))
                    {
                        Spells.Q.CastOnUnit(enemy);
                    }
                }
            }
            if (Spells.W.IsReady() && Helper.Enabled("w.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(Spells.W.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Helper.Enabled("w.enemy." + enemy.ChampionName))
                    {
                        Spells.W.CastOnUnit(enemy);
                    }
                }
            }
            if (Spells.E.IsReady() && Helper.Enabled("e.combo") && !IsWActive)
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(Spells.E.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Helper.Enabled("e.enemy." + enemy.ChampionName) && enemy.CountEnemiesInRange(Spells.E.Range) >= Helper.Slider("e.enemy.count"))
                    {
                        Spells.E.CastOnUnit(enemy);
                    }
                }
            }
        }
        /// <summary>
        /// WaveClear
        /// </summary>
        private static void WaveClear()
        {
            if (IsWActive)
                return;
            if (ObjectManager.Player.ManaPercent < Helper.Slider("clear.mana"))
            {
                return;
            }

            var min = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
            if (Spells.E.IsReady() && Helper.Enabled("e.clear") && !IsWActive)
            {
                if (min.Count > Helper.Slider("e.minion.hit.count"))
                {
                    Spells.E.CastOnUnit(min[0]);
                }
            }
            if (Spells.W.IsReady() && Helper.Enabled("w.clear"))
            {
                Spells.W.CastOnUnit(min[0]);
            }

        }

    }
}
