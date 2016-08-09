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
        public static readonly AIHeroClient FiddleStick = ObjectManager.Player;

        /// <summary>
        /// OnLoad section
        /// </summary>
        /// <param name="args"></param>
        public static void Game_OnGameLoad()
        {
            Menus.Config = new Menu("Feedlestick", "Feedlestick", true);
            {
                Spells.Init();
                Menus.OrbwalkerInit();
                Menus.Init();
            }

            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += OnDraw;
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
                Helper.Circle("q.draw",Spells.Q.Range);
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
        ///  Process spell cast. thats need for last w game time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            Helper.OnProcessSpellCast(sender, args);
        }
        /// <summary>
        /// W lock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            Helper.Spellbook_OnCastSpell(sender, args);
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

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Helper.Slider("harass.mana"))
            {
                return;
            }

            if (Spells.Q.IsReady() && Helper.Enabled("q.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(Spells.Q.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Helper.Enabled("q.enemy." + enemy.ChampionName))
                    {
                        Spells.Q.CastOnUnit(enemy);
                    }
                }
            }
            if (Spells.E.IsReady() && Helper.Enabled("e.harass"))
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
            if (ObjectManager.Player.ManaPercent < Helper.Slider("jungle.mana"))
            {
                return;
            }

            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob.Count > 0)
            {
                if (Spells.Q.IsReady() && Helper.Enabled("q.jungle"))
                {
                    Spells.Q.CastOnUnit(mob[0]);
                }
                if (Spells.W.IsReady() && Helper.Enabled("w.jungle"))
                {
                    Spells.W.CastOnUnit(mob[0]);
                }
                if (Spells.E.IsReady() && Helper.Enabled("e.jungle"))
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
            if (Spells.Q.IsReady() && Helper.Enabled("q.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(o=> o.IsValidTarget(Spells.Q.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Helper.Enabled("q.enemy."+enemy.ChampionName))
                    {
                        Spells.Q.CastOnUnit(enemy);
                    }
                }
            }
            if (Spells.W.IsReady() && Helper.Enabled("w.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(o=> o.IsValidTarget(Spells.W.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Helper.Enabled("w.enemy."+enemy.ChampionName))
                    {
                        Spells.W.CastOnUnit(enemy);
                    }
                }
            }
            if (Spells.E.IsReady() && Helper.Enabled("e.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(o=> o.IsValidTarget(Spells.E.Range) && !o.IsDead && !o.IsZombie))
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
            if (ObjectManager.Player.ManaPercent < Helper.Slider("clear.mana"))
            {
                return;
            }

            var min = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
            if (Spells.E.IsReady() && Helper.Enabled("e.clear"))
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
