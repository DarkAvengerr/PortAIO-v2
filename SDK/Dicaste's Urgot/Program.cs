using EloBuddy;
using LeagueSharp.SDK;
namespace DicasteUrgot
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    internal static class Program
    {
        private static string ChampionName { get; } = "Urgot";

        private static Spell E { get; set; }

        private static AIHeroClient Me { get; } = ObjectManager.Player;

        private static Menu Menu { get; set; }

        private static Spell Q { get; set; }

        private static Spell Q2 { get; set; }

        private static Spell R { get; set; }

        private static string UrgotEBuffName { get; } = "urgotcorrosivedebuff";

        private static Spell W { get; set; }

        private static void Combo()
        {
            if (GetMenuBool("Combo", "Q"))
            {
                Q2Logic();
                QLogic();
            }

            if (GetMenuBool("Combo", "E"))
            {
                ELogic();
            }

            if (GetMenuBool("Combo", "R"))
            {
                RLogic();
            }
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            if (Me.IsDead) return;

            if (Q.IsReady() && GetMenuBool("Drawings", "Q"))
            {
                Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Red);
            }

            if (E.IsReady() && GetMenuBool("Drawings", "E"))
            {
                Render.Circle.DrawCircle(Me.Position, E.Range, Color.Green);
            }

            if (R.IsReady() && GetMenuBool("Drawings", "R"))
            {
                Render.Circle.DrawCircle(Me.Position, R.Range, Color.Blue);
            }
        }

        private static void ELogic()
        {
            if (Q.IsReady() && GetMenuBool("Combo", "E"))
            {
                var target = E.GetTarget();

                if (!Equals(target, null))
                {
                    E.Cast(target);
                }
            }
        }

        private static void EventsOnOnDash(object sender, Events.DashArgs dashArgs)
        {
            if (GetMenuBool("Misc", "Dash") && R.IsReady() && Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                var target = sender as AIHeroClient;

                if (!Equals(target, null) && target.Health < Me.Health)
                {
                    R.Cast(target);
                }
            }
        }

        private static void EventsOnOnInterruptableTarget(
            object sender,
            Events.InterruptableTargetEventArgs interruptableTargetEventArgs)
        {
            if (R.IsReady() && GetMenuBool("Misc", "Interrupt")
                && Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                var target = sender as AIHeroClient;

                if (!Equals(target, null) && target.DistanceToPlayer() <= R.Range && target.IsEnemy
                    && target.HealthPercent <= 20 && Me.HealthPercent >= 85)
                {
                    R.Cast(target);
                }
            }
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Combo();
                    break;
                case OrbwalkingMode.Hybrid:
                    Harass();
                    break;
                case OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
            }

            switch (R.Instance.Level)
            {
                case 1:
                    R.Range = 550;
                    break;
                case 2:
                    R.Range = 700;
                    break;
                case 3:
                    R.Range = 850;
                    break;
            }
        }

        private static bool GetMenuBool(string subMenuName, string spellSlot)
            => Menu[subMenuName][string.Concat(subMenuName, spellSlot)].GetValue<MenuBool>();

        private static void Harass()
        {
            if (GetMenuBool("Harass", "Q"))
            {
                Q2Logic();
                QLogic();
            }

            if (GetMenuBool("Harass", "E"))
            {
                ELogic();
            }
        }

        private static void InitializeMenu()
        {
            Menu = new Menu("Urgot", "Urgot", true);

            var comboMenu = new Menu("Combo", "Combo");
            comboMenu.Add(new MenuBool("ComboQ", "Q Usage", true));
            comboMenu.Add(new MenuBool("ComboE", "E Usage", true));
            comboMenu.Add(new MenuBool("ComboR", "R Usage", true));
            Menu.Add(comboMenu);

            var harassMenu = new Menu("Harass", "Harass");
            harassMenu.Add(new MenuBool("HarassQ", "Q Usage", true));
            harassMenu.Add(new MenuBool("HarassE", "E Usage"));
            Menu.Add(harassMenu);

            var laneClearMenu = new Menu("Laneclear", "Laneclear");
            laneClearMenu.Add(new MenuBool("LaneclearQ", "Q Usage", true));
            Menu.Add(laneClearMenu);

            var drawingsMenu = new Menu("Drawings", "Drawings");
            drawingsMenu.Add(new MenuBool("DrawingsQ", "Draw Q Range", true));
            drawingsMenu.Add(new MenuBool("DrawingsE", "Draw E Range", true));
            drawingsMenu.Add(new MenuBool("DrawingsR", "Draw R Range", true));
            Menu.Add(drawingsMenu);

            var miscMenu = new Menu("Misc", "Misc");
            miscMenu.Add(new MenuBool("MiscSlow", "Auto W for Slow Enemy", true));
            miscMenu.Add(new MenuBool("MiscDash", "Auto R in Combo for Dashing Enemy", true));
            miscMenu.Add(new MenuBool("MiscInterrupt", "Auto R in Combo to Interrupt Spells", true));
            Menu.Add(miscMenu);

            Menu.Attach();
        }

        private static void InitializeSpells()
        {
            Q = new Spell(SpellSlot.Q, 980);
            Q.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);

            Q2 = new Spell(SpellSlot.Q, 1200);
            Q2.SetSkillshot(0.25f, 60f, 1600f, false, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W);

            E = new Spell(SpellSlot.E, 900);
            E.SetSkillshot(0.25f, 120f, 1500f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 850);
        }

        private static void LaneClear()
        {
            if (GetMenuBool("Laneclear", "Q"))
            {
                var targetMinionEBuffed =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(
                            minion =>
                            minion.IsEnemy && minion.IsValid && minion.Distance(Me) <= Q.Range
                            && minion.Health < Q.GetDamage(minion) && minion.HasBuff(UrgotEBuffName));

                if (!Equals(targetMinionEBuffed, null))
                {
                    Q2.Cast(targetMinionEBuffed);
                }

                var targetMinion =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(
                            minion =>
                            minion.IsEnemy && minion.IsValid && minion.Distance(Me) <= Q.Range
                            && minion.Health < Q.GetDamage(minion));

                if (!Equals(targetMinion, null))
                {
                    Q.Cast(targetMinion);
                }
            }
        }

        public static void Main()
        {
            Bootstrap.Init();
            OnLoad();
        }

        private static void OnLoad()
        {
            if (Me.ChampionName != ChampionName)
            {
                return;
            }

            Chat.Print("Dicaste's Urgot Loaded!");

            InitializeMenu();

            InitializeSpells();

            Game.OnUpdate += GameOnOnUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
            Events.OnInterruptableTarget += EventsOnOnInterruptableTarget;
            Spellbook.OnCastSpell += SpellbookOnOnCastSpell;
            Events.OnDash += EventsOnOnDash;
        }

        private static void Q2Logic()
        {
            if (Q2.IsReady())
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .Where(hero => hero.IsValid && hero.IsValidTarget(Q2.Range) && hero.HasBuff(UrgotEBuffName))
                        .MinOrDefault(hero => hero.Health);

                if (!Equals(target, null))
                {
                    Q2.Cast(target);
                }
            }
        }

        private static void QLogic()
        {
            if (Q.IsReady())
            {
                var target = Q.GetTarget();

                if (!Equals(target, null))
                {
                    Q.Cast(target);
                }
            }
        }

        private static void RLogic()
        {
            if (R.IsReady())
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            hero =>
                            !hero.IsMe && hero.IsValid
                            && (Me.HealthPercent <= 45 || hero.Health <= Q.GetDamage(hero) * 2))
                        .MaxOrDefault(hero => hero.PhysicalDamageDealtPlayer);

                if (!Equals(target, null))
                {
                    R.Cast(target);
                }
            }
        }

        private static void SpellbookOnOnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == SpellSlot.Q && GetMenuBool("Misc", "Slow"))
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(
                            hero => hero.IsValid && hero.IsValidTarget(Q2.Range) && hero.HasBuff(UrgotEBuffName));

                if (Equals(target, null))
                {
                    return;
                }

                switch (Variables.Orbwalker.ActiveMode)
                {
                    case OrbwalkingMode.Combo:
                        W.Cast();
                        break;

                    case OrbwalkingMode.Hybrid:
                        {
                            W.Cast();
                            break;
                        }
                }

                args.Process = true;
            }
        }
    }
}