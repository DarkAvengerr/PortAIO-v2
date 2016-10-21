using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xDTrundle
{
    /* Thanks to Doug - Nightmoon */
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    internal class Program
    {
        private const string Headline = "[SDK] Trundle";

        public static AIHeroClient Oyuncu => ObjectManager.Player;

        private const string Troll = "Trundle";

        private static Spell q, w, e, r;

        public static Menu Config;

        public static MenuBool UseQ;

        public static MenuBool UseW;

        /*
        public static MenuBool UseR;
*/

        public static MenuBool UseE;

        public static MenuBool UseQh;

        public static MenuBool Interruptenemyskills;

        public static MenuBool Drawskills;

        public static MenuBool DrawR;

        public static void Main()
        {
            Bootstrap.Init();
            Load_OnLoad();
        }

        private static void Load_OnLoad()
        {
            if (Oyuncu.ChampionName != Troll)
            {
                return;
            }
            Menu();
            Spells();
            Link();
        }

        private static void Link()
        {
            Game.OnUpdate += OnGameUpdate;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnSpellCast += OndoCast;
        }

        /// Aa cancel thanks Doug!
        private static void OndoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !AutoAttack.IsAutoAttack(args.SData.Name)) return;
            if (args.Target == null || !args.Target.IsValid) return;

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                if (Variables.Orbwalker.GetTarget() == null) return;

                if (
                    !(args.Target is Obj_AI_Turret || args.Target is Obj_Barracks || args.Target is Obj_BarracksDampener
                      || args.Target is Obj_Building) && q.IsReady() && UseQ)
                {
                    DelayAction.Add(
                        50,
                        () =>
                            {
                                q.Cast();
                                Variables.Orbwalker.ResetSwingTimer();
                            });
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Oyuncu.IsDead) return;
            {
                if (Drawskills && e.IsReady())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, e.Range, Color.DarkGoldenrod);
                }
                if (DrawR && r.IsReady())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, r.Range, Color.LightBlue);
                }
            }
        }

        /// <summary>
        /// Menu.
        /// </summary>
        private static void Menu()
        {
            Config = new Menu(Headline, Headline, true).Attach();

            Config.Add(new MenuSeparator("nah", "Disabled laneclear and lasthit until I fix it"));

            var combomenu = Config.Add(new Menu("Combo", "Combo"));

            combomenu.Add(new MenuSeparator("nah", "Didn't add Trundle R, It can ruin the teamfight."));

            UseQ = combomenu.Add(new MenuBool("CastQ", "Use Q", true));

            UseW = combomenu.Add(new MenuBool("CastW", "Use W", true));

            UseE = combomenu.Add(new MenuBool("CastE", "Use E", true));
            {
                /*
                UseR = combomenu.Add(new MenuBool("CastR", "Use R", true));

                var WhiteList = combomenu.Add(new Menu("WhiteList", "R WhiteList"));
                if (GameObjects.EnemyHeroes.Any())
                {
                   GameObjects.EnemyHeroes.ForEach(i => WhiteList.Add(new MenuBool(i.ChampionName, i.ChampionName, false)));
                }
                */
            }
            var hybridmenu = Config.Add(new Menu("Combo", "Hybrid"));

            UseQh = hybridmenu.Add(new MenuBool("CastQH", "Cast Q Harass", true));

            var drawings = Config.Add(new Menu("Drawings", "Drawings"));

            Drawskills = drawings.Add((new MenuBool("DrawE", "Draw E Range", true)));

            DrawR = drawings.Add((new MenuBool("DrawR", "Draw R Range", true)));

            var miscmenu = Config.Add(new Menu("Misc", "Misc"));

            Interruptenemyskills = miscmenu.Add((new MenuBool("inter", "Interrupt skills with E", true)));
        }

        /// <summary>
        /// Spells.
        /// </summary>
        private static void Spells()
        {
            q = new Spell(SpellSlot.Q, 175);
            w = new Spell(SpellSlot.W, 900);
            e = new Spell(SpellSlot.E, 1000);
            e.SetSkillshot(0.5f, 188f, 1600f, false, SkillshotType.SkillshotCircle);
            r = new Spell(SpellSlot.R, 700);
        }

        /// <summary>
        /// Mode handler.
        /// </summary>
        /// <param name="args"></param>
        private static void OnGameUpdate(EventArgs args)
        {
            if (Oyuncu.IsDead)
            {
                return;
            }
            Fountain();
            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Combo();
                    break;
                case OrbwalkingMode.Hybrid:
                    Hybrid();
                    break;
            }
        }

        private static void Fountain()
        {
            if (!Oyuncu.InFountain() || !w.IsReady()) return;
            DelayAction.Add(1500, () => w.Cast());
        }

        private static void Hybrid()
        {
            var target = Variables.TargetSelector.GetTarget(q.Range, DamageType.Physical);
            {
                if (target.IsValid && UseQh && q.IsReady())
                {
                    q.Cast();
                    Variables.Orbwalker.ForceTarget = target;
                }
            }
        }

        private static void Combo()
        {
            if (UseW && w.IsReady())
            {
                var target = Variables.TargetSelector.GetTarget(1000, DamageType.Physical);
                w.Cast(w.GetPrediction(Oyuncu).CastPosition.Extend(target.ServerPosition, 700));

                if (UseE && e.IsReady() && target.IsValidTarget(e.Range)) return;

                e.Cast(e.GetPrediction(target).CastPosition.Extend(target.ServerPosition, 150));
            }
        }

        private static void OnInterruptableTarget(
            object sender,
            Events.InterruptableTargetEventArgs interruptableTargetEventArgs)
        {
            if (Interruptenemyskills && e.IsReady()
                && interruptableTargetEventArgs.Sender.Distance(ObjectManager.Player) < 1000)
            {
                e.Cast(interruptableTargetEventArgs.Sender.Position);
            }
        }
    }
}
