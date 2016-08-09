using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PlebNautilus
{
    class Program
    {
        private static Orbwalking.Orbwalker _orbwalker;
        private static readonly AIHeroClient Player = ObjectManager.Player;

        public static Spell Q { get; set; }
        public static Spell W { get; set; }
        public static Spell E { get; set; }
        public static Spell R { get; set; }

        private static SpellSlot _smiteSlot = SpellSlot.Unknown;
        private static Spell _smite;

        //credits Kurisu
        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        public static HpBarIndicator Hpi = new HpBarIndicator();

        private static SpellSlot _ignite;
 
        private static Menu _menu;

        public static void Game_OnGameLoad()
        {

            if (Player.ChampionName != "Nautilus")
                return;

            Q = new Spell(SpellSlot.Q, 900);
            Q.SetSkillshot(250, 90, 2000, true, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 550);
            R = new Spell(SpellSlot.R, 755);

            SettingupSmite();
            _ignite = Player.GetSpellSlot("summonerdot");

            _menu = new Menu("Kyon's " + Player.ChampionName, Player.ChampionName, true);

            Menu orbwalkerMenu = _menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            _orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            Menu ts = _menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(ts);

            Menu combo = _menu.AddSubMenu(new Menu("combo", "combo"));
            {
                combo.AddItem(new MenuItem("CombouseQ", "Use Q").SetValue(true)); //y
                combo.AddItem(new MenuItem("CombouseW", "Use W").SetValue(true)); //y
                combo.AddItem(new MenuItem("CombouseE", "Use E").SetValue(true)); //y
                combo.AddItem(new MenuItem("CombouseR", "Use R").SetValue(true)); //y
                combo.AddItem(new MenuItem("CombouseSmite", "Use Smite").SetValue(true)); //y
                combo.AddItem(new MenuItem("CombouseIgnite", "Use Ignite").SetValue(true)); //y
            }

            var usageR = _menu.AddSubMenu((new Menu("Ult Settings", "Ultwork")));
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(target => target.IsEnemy))
                    usageR.AddItem(new MenuItem("DontR" + target.ChampionName, target.ChampionName).SetValue(false));
            }
           
            Menu killsteal = _menu.AddSubMenu(new Menu("killsteal", "Killsteal"));
            {
                killsteal.AddItem(new MenuItem("ksinuse", "Killsteal").SetValue(true));
                killsteal.AddItem(new MenuItem("ksQ", "Use Q").SetValue(true)); //y
                killsteal.AddItem(new MenuItem("ksE", "Use E").SetValue(true)); //y

            }

            Menu laneclear = _menu.AddSubMenu(new Menu("laneclear", "laneclear"));
            {
                laneclear.AddItem(new MenuItem("laneuseQ", "Use Q").SetValue(true)); //y
                laneclear.AddItem(new MenuItem("laneuseW", "Use W").SetValue(true)); //y
                laneclear.AddItem(new MenuItem("laneuseE", "Use E").SetValue(true)); //y
                laneclear.AddItem(new MenuItem("laneE", "when x minions").SetValue(new Slider(3,1,10)));
                laneclear.AddItem(new MenuItem("laneuntilmana", "min mana in %").SetValue(new Slider(25)));
            }

            Menu flee = _menu.AddSubMenu(new Menu("flee", "flee"));
            {
                flee.AddItem(new MenuItem("fleekey", "flee ! ").SetValue(new KeyBind('A', KeyBindType.Press))); //A
                flee.AddItem(new MenuItem("fleeuseQ", "Use Q").SetValue(true)); //y
                flee.AddItem(new MenuItem("fleeuseW", "Use W").SetValue(true)); //y
                flee.AddItem(new MenuItem("fleeusewalls", "use walls").SetValue(true)); //y
                flee.AddItem(new MenuItem("fleeuseminions", "use minions").SetValue(true)); //y
            }

            Menu drawings = _menu.AddSubMenu(new Menu("drawings", "drawings"));
            {
                drawings.AddItem(new MenuItem("drawingsdrawQ", "Draw Q").SetValue(true)); //y
                drawings.AddItem(new MenuItem("drawingsdrawW", "Draw W").SetValue(true)); //y
                drawings.AddItem(new MenuItem("drawingsdrawE", "Draw E").SetValue(true)); //y
                drawings.AddItem(new MenuItem("drawingsdrawR", "Draw R").SetValue(true)); //y
                drawings.AddItem(new MenuItem("drawingsdrawHP", "Draw Damage Indicator").SetValue(true)); //y
            }

            Menu misc = _menu.AddSubMenu(new Menu("misc", "misc"));
            {
                misc.AddItem(new MenuItem("miscigniteuse", "Use Ignite").SetValue(true)); //y
                misc.AddItem(new MenuItem("DrawD", "Damage Indicator").SetValue(true));
            }

            _menu.AddToMainMenu();
            Interrupter2.OnInterruptableTarget += Interrupter2OnOnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void SettingupSmite()
        {
            foreach (
                var spell in
                    ObjectManager.Player.Spellbook.Spells.Where(
                        spell => String.Equals(spell.Name, Smitetype(), StringComparison.CurrentCultureIgnoreCase)))
            {
                _smiteSlot = spell.Slot;
                _smite = new Spell(_smiteSlot, 700);
                return;
            }
        }

        public static string Smitetype()
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

        private static void Interrupter2OnOnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && sender.Distance(Player) <= Q.Range && args.DangerLevel == Interrupter2.DangerLevel.High || args.DangerLevel == Interrupter2.DangerLevel.Medium )
            {
                var hitchance = Q.GetPrediction(sender, false, 0,
                    new[]
                    {
                        CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.Walls,
                        CollisionableObjects.YasuoWall
                    }).Hitchance;

                if (hitchance == HitChance.VeryHigh || hitchance == HitChance.High || hitchance == HitChance.Immobile ||
                    hitchance == HitChance.Dashing)
                {
                    Q.Cast(sender);
                }
            }
            else if (sender.IsEnemy && _orbwalker.InAutoAttackRange(sender) && args.DangerLevel == Interrupter2.DangerLevel.High || args.DangerLevel == Interrupter2.DangerLevel.Medium)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, sender);
            }

        }

        private static void OnEndScene(EventArgs args)
        {
            if (_menu.Item("drawingsdrawHP").GetValue<bool>())
            {
                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    Hpi.unit = enemy;
                    Hpi.DrawDmg(CalcDamage(enemy), Color.Green);
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (_menu.Item("fleekey").GetValue<KeyBind>().Active)
            {
                Flee();
            }

            switch (_orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear();
                    break;
            }

            Killsteal();
        }

        private static int CalcDamage(Obj_AI_Base target)
        {
            var aa = Player.GetAutoAttackDamage(target, true) * (1 + Player.Crit);
            var damage = aa;
            _ignite = Player.GetSpellSlot("summonerdot");

            if (_ignite.IsReady())
                damage += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            if (R.IsReady()) // rdamage
            {
                    damage += R.GetDamage(target);
            }

            if (Q.IsReady()) // qdamage
            {

                damage += Q.GetDamage(target);
            }

            if (E.IsReady()) // edamage
            {

                damage += E.GetDamage(target);
            }

            if (_smite.IsReady()) // edamage
            {

                damage += GetSmiteDmg();
            }

            return (int)damage;
        }

        private static int GetSmiteDmg()
        {
            int level = Player.Level;
            int index = Player.Level / 5;
            float[] dmgs = { 370 + 20 * level, 330 + 30 * level, 240 + 40 * level, 100 + 50 * level };
            return (int)dmgs[index];
        }

        private static void Laneclear()
        {

            var minion = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.All);

            if(Player.ManaPercent <= _menu.Item("laneuntilmana").GetValue<Slider>().Value)
                return;

            if (minion.Count >= _menu.Item("laneE").GetValue<Slider>().Value && E.IsReady() && minion.First().IsValidTarget(E.Range - 50) && _menu.Item("laneuseE").GetValue<bool>())
            {
                E.Cast(true);
            }

            if (Q.IsReady() && _menu.Item("laneuseQ").GetValue<bool>())
            {
                var jungleMobs = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                if (jungleMobs.Count >= 3)
                {
                    var target = jungleMobs.First();
                    Q.CastIfHitchanceEquals(target, HitChance.High, true);
                }
            }

            if (W.IsReady() && Player.HealthPercent <= 75 && _menu.Item("laneuseE").GetValue<bool>())
            {
                W.Cast(true);
            }

        }

        private static void Combo()
        {
            bool vQ = Q.IsReady() && _menu.Item("CombouseQ").GetValue<bool>();
            bool vW = W.IsReady() && _menu.Item("CombouseW").GetValue<bool>();
            bool vE = E.IsReady() && _menu.Item("CombouseE").GetValue<bool>();
            bool vR = R.IsReady() && _menu.Item("CombouseR").GetValue<bool>();
            bool ign = _ignite.IsReady() && _menu.Item("CombouseIgnite").GetValue<bool>();
 
            var tsQ = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var tsR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (tsQ == null|| tsR == null)
                return;

            if (vR && tsR.IsValidTarget(R.Range) && tsR.Health > R.GetDamage(tsR))
            {
                var useR = (_menu.Item("DontR" + tsR.ChampionName) != null &&
                           _menu.Item("DontR" + tsR.ChampionName).GetValue<bool>() == false);
                if (useR)
                {
                    R.CastOnUnit(tsR);
                }
            }
                
            UseSmite(tsQ);

            if (vQ && tsQ.IsValidTarget())
            {
                var qpred = Q.GetPrediction(tsQ);
                if (qpred.CollisionObjects.Count(c => c.IsEnemy && !c.IsDead) < 4 && qpred.Hitchance >= HitChance.High)
                {
                    Q.Cast(qpred.CastPosition);
                }
            }

            if (vW && tsQ.IsValidTarget(W.Range))
                W.Cast();

            if (vE && tsQ.IsValidTarget(E.Range))
                E.Cast();

            if (Player.Distance(tsQ.Position) <= 600 && IgniteDamage(tsQ) >= tsQ.Health && ign)
                Player.Spellbook.CastSpell(_ignite, tsQ);

        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }
        //thanks Justy
        private static void Killsteal()
        {
            if (!_menu.Item("ksinuse").GetValue<bool>())
                return;

            foreach (AIHeroClient target in
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        hero =>
                            hero.IsValidTarget(Q.Range) && !hero.HasBuffOfType(BuffType.Invulnerability) && hero.IsEnemy)
                )
            {
                var qDmg = Player.GetSpellDamage(target, SpellSlot.Q);
                if (_menu.Item("ksQ").GetValue<bool>() && target.IsValidTarget(Q.Range) && target.Health <= qDmg)
                {
                    var qpred = Q.GetPrediction(target);
                    if (qpred.Hitchance >= HitChance.High && qpred.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                        Q.Cast(qpred.CastPosition);
                }
                var eDmg = Player.GetSpellDamage(target, SpellSlot.E);
                if (_menu.Item("ksE").GetValue<bool>() && target.IsValidTarget(E.Range) && target.Health <= eDmg)
                {
                    E.Cast();
                }
            }
        }
        //Credits to metaphorce
        public static void UseSmite(AIHeroClient target)
        {
            var usesmite = _menu.Item("CombouseSmite").GetValue<bool>();
            var itemscheck = SmiteBlue.Any(i => Items.HasItem(i)) || SmiteRed.Any(i => Items.HasItem(i));
            if (itemscheck && usesmite &&
                ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready &&
                target.Distance(Player.Position) < _smite.Range)
            {
                ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, target);
            }
        }

        private static void Flee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos, false);

            bool vQ = Q.IsReady() && _menu.Item("fleeuseQ").GetValue<bool>();
            bool vW = W.IsReady() && _menu.Item("fleeuseW").GetValue<bool>();

            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget(Q.Range)).ToList(); // Hopefully this is enough...
            var step = Q.Range / 2; // Or whatever step value...
            for (var i = step; i <= Q.Range; i += step)
            {
                if (ObjectManager.Player.Position.Extend(Game.CursorPos, i).IsWall() && Player.Distance(Game.CursorPos) >= Q.Range/2 && vQ)
                {
                    Q.Cast(Game.CursorPos);
                }

                var target =
                    minions.FirstOrDefault(
                     minion =>
                       Geometry.CircleCircleIntersection(
                                Player.Position.To2D(),
                                minion.Position.To2D(),
                                Q.Range,
                                minion.BoundingRadius).Count() > 0);

                if (target != null && target.Distance(Player.Position) >= Q.Range/2 && vQ )
                {
                    Q.Cast(target.Position);
                }
            }

            if (vW && ObjectManager.Get<Obj_AI_Base>().Any(x => x.IsEnemy && x.Distance(Player.Position) <= Q.Range && Player.IsTargetable))
            {
                W.Cast();
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if(Player.IsDead) return;

            if (Q.IsReady() && _menu.Item("drawingsdrawQ").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Crimson);
            }

            if (W.IsReady() && _menu.Item("drawingsdrawW").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.CornflowerBlue);
            }

            if (E.IsReady() && _menu.Item("drawingsdrawE").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.FloralWhite);
            }

            if (R.IsReady() && _menu.Item("drawingsdrawR").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Orange);
            }

        }
    }
}
