using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

// ReSharper disable InconsistentNaming

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace yol0Thresh
{
    internal class Program
    {
        private static readonly AIHeroClient Player = ObjectManager.Player;

        private static readonly Spell _Q = new Spell(SpellSlot.Q, 1075);
        private static readonly Spell _W = new Spell(SpellSlot.W, 950);
        private static readonly Spell _E = new Spell(SpellSlot.E, 500);
        private static readonly Spell _R = new Spell(SpellSlot.R, 400);

        private static Menu Config;

        private static int qTick;
        private static int hookTick;
        private static Obj_AI_Base hookedUnit;


        private static List<Vector3> escapeSpots = new List<Vector3>();
        private static readonly List<GameObject> soulList = new List<GameObject>();

        private static AIHeroClient currentTarget
        {
            get
            {
                if (TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget is AIHeroClient && TargetSelector.SelectedTarget.Team != Player.Team)
                    return (AIHeroClient) TargetSelector.SelectedTarget;
                if (TargetSelector.GetSelectedTarget() != null)
                    return TargetSelector.GetSelectedTarget();
                return TargetSelector.GetTarget(qRange + 175, TargetSelector.DamageType.Physical);
            }
        }

        private static float qRange
        {
            get { return Config.SubMenu("Misc").Item("qRange").GetValue<Slider>().Value; }
        }

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Thresh")
                return;

            _Q.SetSkillshot(0.5f, 70, 1900, true, SkillshotType.SkillshotLine);
            _W.SetSkillshot(0f, 200, 1750, false, SkillshotType.SkillshotCircle);
            _E.SetSkillshot(0.3f, 60, float.MaxValue, false, SkillshotType.SkillshotLine);

            Config = new Menu("yol0 Thresh", "Thresh", true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            xSLxOrbwalker.AddToMenu(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(Config.SubMenu("Target Selector"));

            Config.AddSubMenu(new Menu("Combo Settings", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("useQ1", "Use Q1").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useQ2", "Use Q2").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useE", "Use Flay").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useW", "Throw Lantern to Ally").SetValue(true));


            Config.AddSubMenu(new Menu("Harass Settings", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("useQ1", "Use Q1").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("useE", "Use Flay").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("manaPercent", "Mana %").SetValue(new Slider(40, 1)));

            Config.AddSubMenu(new Menu("Flay Settings", "Flay"));
            Config.SubMenu("Flay")
                .AddItem(new MenuItem("pullEnemy", "Pull Enemy").SetValue(new KeyBind(90, KeyBindType.Press)));
            Config.SubMenu("Flay")
                .AddItem(new MenuItem("pushEnemy", "Push Enemy").SetValue(new KeyBind(88, KeyBindType.Press)));
            Config.SubMenu("Flay").AddSubMenu(new Menu("Per-Enemy Settings", "ActionToTake"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(unit => unit.Team != Player.Team))
            {
                Config.SubMenu("Flay")
                    .SubMenu("ActionToTake")
                    .AddItem(
                        new MenuItem(enemy.ChampionName, enemy.ChampionName).SetValue(
                            new StringList(new[] {"Pull", "Push"})));
            }

            Config.AddSubMenu(new Menu("Lantern Settings", "Lantern"));
            Config.SubMenu("Lantern").AddItem(new MenuItem("useW", "Throw to Ally").SetValue(true));
            Config.SubMenu("Lantern")
                .AddItem(new MenuItem("numEnemies", "Throw if # Enemies").SetValue(new Slider(2, 1, 5)));
            Config.SubMenu("Lantern").AddItem(new MenuItem("useWCC", "Throw to CC'd Ally").SetValue(true));

            Config.AddSubMenu(new Menu("Box Settings", "Box"));
            Config.SubMenu("Box").AddItem(new MenuItem("useR", "Auto Use Box").SetValue(true));
            Config.SubMenu("Box").AddItem(new MenuItem("minEnemies", "Minimum Enemies").SetValue(new Slider(3, 1, 5)));

            Config.AddSubMenu(new Menu("Misc Settings", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("qRange", "Q Range").SetValue(new Slider(1075, 700, 1075)));
            Config.SubMenu("Misc")
                .AddItem(
                    new MenuItem("qHitChance", "Q HitChance").SetValue(
                        new StringList(new[] {"Low", "Medium", "High", "Very High"}, 3)));

            Config.SubMenu("Misc").AddSubMenu(new Menu("Gapclosers", "Gapclosers"));
            if (ObjectManager.Get<AIHeroClient>().Any(unit => unit.Team != Player.Team && unit.ChampionName == "Rengar"))
            {
                Config.SubMenu("Misc")
                    .SubMenu("Gapclosers")
                    .AddItem(new MenuItem("rengarleap", "Rengar - Unseen Predator").SetValue(true));
            }
            foreach (var spell in from spell in AntiGapcloser.Spells
                from enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(unit => unit.Team != Player.Team)
                        .Where(enemy => spell.ChampionName == enemy.ChampionName)
                select spell)
            {
                Config.SubMenu("Misc")
                    .SubMenu("Gapclosers")
                    .AddItem(
                        new MenuItem(spell.SpellName, spell.ChampionName + " - " + spell.SpellName).SetValue(
                            true));
            }

            Config.SubMenu("Misc").AddSubMenu(new Menu("Interruptble Spells", "InterruptSpells"));
            foreach (var spell in Interrupter.Spells)
            {
                var spell1 = spell;
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(unit => unit.Team != Player.Team)
                            .Where(enemy => spell1.ChampionName == enemy.ChampionName))
                {
                    Config.SubMenu("Misc")
                        .SubMenu("InterruptSpells")
                        .AddSubMenu(new Menu(enemy.ChampionName + " - " + spell.SpellName, spell.SpellName));
                    Config.SubMenu("Misc")
                        .SubMenu("InterruptSpells")
                        .SubMenu(spell.SpellName)
                        .AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
                    Config.SubMenu("Misc")
                        .SubMenu("InterruptSpells")
                        .SubMenu(spell.SpellName)
                        .AddItem(new MenuItem("useE", "Interrupt with Flay").SetValue(true));
                    Config.SubMenu("Misc")
                        .SubMenu("InterruptSpells")
                        .SubMenu(spell.SpellName)
                        .AddItem(new MenuItem("useQ", "Interrupt with Hook").SetValue(true));
                }
            }

            Config.AddSubMenu(new Menu("KS Settings", "KS"));
            Config.SubMenu("KS").AddItem(new MenuItem("ksQ", "KS with Q").SetValue(false));
            Config.SubMenu("KS").AddItem(new MenuItem("ksE", "KS with E").SetValue(false));

            Config.AddSubMenu(new Menu("Draw Settings", "Draw"));
            Config.SubMenu("Draw")
                .AddItem(new MenuItem("drawQMax", "Draw Q Max Range").SetValue(new Circle(true, Color.Red)));
            Config.SubMenu("Draw")
                .AddItem(new MenuItem("drawQEffective", "Draw Q Effective").SetValue(new Circle(true, Color.Blue)));
            Config.SubMenu("Draw")
                .AddItem(new MenuItem("drawW", "Draw W Range").SetValue(new Circle(false, Color.Green)));
            Config.SubMenu("Draw")
                .AddItem(new MenuItem("drawE", "Draw E Range").SetValue(new Circle(false, Color.Aqua)));
            Config.SubMenu("Draw").AddItem(new MenuItem("drawQCol", "Draw Q Line").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("drawTargetC", "Draw Target (Circle)").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("drawTargetT", "Draw Target (Text)").SetValue(true));
            Config.SubMenu("Draw")
                .AddItem(new MenuItem("drawSouls", "Draw Circle on Souls").SetValue(new Circle(true, Color.DeepSkyBlue)));
            Config.AddToMainMenu();
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Obj_AI_Base.OnPlayAnimation += OnAnimation;
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnGameUpdate;
            GameObject.OnCreate += OnCreateObj;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapCloser;
            Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
        }


        public static void OnGameUpdate(EventArgs args)
        {
            AutoBox();
            KS();
            Lantern();
            UpdateSouls();
            UpdateBuffs();

            if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo)
                Combo();

            if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Harass)
                Harass();

            if (Config.SubMenu("Flay").Item("pullEnemy").GetValue<KeyBind>().Active)
            {
                var target = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Physical);
                if (target != null)
                    PullFlay(target);
            }

            if (Config.SubMenu("Flay").Item("pushEnemy").GetValue<KeyBind>().Active)
            {
                var target = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Physical);
                if (target != null)
                    PushFlay(target);
            }
        }

        public static void OnDraw(EventArgs args)
        {
            if (Config.SubMenu("Draw").Item("drawQMax").GetValue<Circle>().Active && !Player.IsDead)
            {
                Render.Circle.DrawCircle(Player.Position, _Q.Range,
                    Config.SubMenu("Draw").Item("drawQMax").GetValue<Circle>().Color);
            }

            if (Config.SubMenu("Draw").Item("drawQEffective").GetValue<Circle>().Active && !Player.IsDead)
            {
                Render.Circle.DrawCircle(Player.Position, qRange,
                    Config.SubMenu("Draw").Item("drawQEffective").GetValue<Circle>().Color);
            }

            if (Config.SubMenu("Draw").Item("drawW").GetValue<Circle>().Active && !Player.IsDead)
            {
                Render.Circle.DrawCircle(Player.Position, _W.Range,
                    Config.SubMenu("Draw").Item("drawW").GetValue<Circle>().Color);
            }

            if (Config.SubMenu("Draw").Item("drawE").GetValue<Circle>().Active && !Player.IsDead)
            {
                Render.Circle.DrawCircle(Player.Position, _E.Range,
                    Config.SubMenu("Draw").Item("drawE").GetValue<Circle>().Color);
            }

            if (Config.SubMenu("Draw").Item("drawQCol").GetValue<bool>() && !Player.IsDead)
            {
                if (currentTarget != null && Player.Distance(currentTarget) < qRange + 200)
                {
                    var playerPos = Drawing.WorldToScreen(Player.Position);
                    var targetPos = Drawing.WorldToScreen(currentTarget.Position);
                    Drawing.DrawLine(playerPos, targetPos, 4,
                        _Q.GetPrediction(currentTarget, overrideRange: qRange).Hitchance < GetSelectedHitChance()
                            ? Color.Red
                            : Color.Green);
                }
            }

            if (currentTarget != null &&
                (Config.SubMenu("Draw").Item("drawTargetC").GetValue<bool>() && currentTarget.IsVisible &&
                 !currentTarget.IsDead))
            {
                Render.Circle.DrawCircle(currentTarget.Position, currentTarget.BoundingRadius + 10, Color.Red);
                Render.Circle.DrawCircle(currentTarget.Position, currentTarget.BoundingRadius + 25, Color.Red);
                Render.Circle.DrawCircle(currentTarget.Position, currentTarget.BoundingRadius + 45, Color.Red);
            }

            if (currentTarget != null &&
                (Config.SubMenu("Draw").Item("drawTargetT").GetValue<bool>() && !currentTarget.IsDead))
            {
                Drawing.DrawText(100, 150, Color.Red, "Current Target: " + currentTarget.ChampionName);
            }

            if (Config.SubMenu("Draw").Item("drawSouls").GetValue<Circle>().Active && !Player.IsDead)
            {
                foreach (var soul in soulList.Where(s => s.IsValid))
                {
                    Render.Circle.DrawCircle(soul.Position, 50,
                        Config.SubMenu("Draw").Item("drawSouls").GetValue<Circle>().Color);
                }
            }
        }

        public static void OnAnimation(GameObject unit, GameObjectPlayAnimationEventArgs args)
        {
            var hero = unit as AIHeroClient;
            if (hero != null)
            {
                if (hero.Team == Player.Team) return;
                if (hero.ChampionName == "Rengar" && args.Animation == "Spell5" && Player.Distance(hero) <= 725)
                {
                    if (_E.IsReady() &&
                        Config.SubMenu("Misc").SubMenu("Gapclosers").Item("rengarleap").GetValue<bool>())
                    {
                        _E.Cast(unit.Position);
                    }
                }
            }
        }

        public static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.SData.Name == "ThreshQ")
            {
                qTick = Environment.TickCount + 500;
            }

            if (args.SData.Name == "ThreshE")
            {
                xSLxOrbwalker.ResetAutoAttackTimer();
            }
        }

        public static void OnCreateObj(GameObject obj, EventArgs args)
        {
            if (obj.Name.Contains("Thresh_Base_soul"))
            {
                soulList.Add(obj);
            }
        }

        public static void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (
                Config.SubMenu("Misc")
                    .SubMenu("InterruptSpells")
                    .SubMenu(spell.SpellName)
                    .Item("enabled")
                    .GetValue<bool>())
            {
                if (
                    Config.SubMenu("Misc")
                        .SubMenu("InterruptSpells")
                        .SubMenu(spell.SpellName)
                        .Item("useE")
                        .GetValue<bool>() && _E.IsReady() &&
                    Player.Distance(unit) < _E.Range)
                {
                    if (ShouldPull((AIHeroClient) unit))
                        PullFlay(unit);
                    else
                        PushFlay(unit);
                }
                else if (
                    Config.SubMenu("Misc")
                        .SubMenu("InterruptSpells")
                        .SubMenu(spell.SpellName)
                        .Item("useQ")
                        .GetValue<bool>() && _Q.IsReady() &&
                    !_Q.GetPrediction(unit).CollisionObjects.Any())
                {
                    _Q.Cast(unit);
                }
            }
        }

        public static void OnEnemyGapCloser(ActiveGapcloser gapcloser)
        {
            if (_E.IsReady() &&
                Config.SubMenu("Misc").SubMenu("Gapclosers").Item(gapcloser.SpellName.ToLower()).GetValue<bool>() &&
                Player.Distance(gapcloser.Sender) < _E.Range + 100)
            {
                if (gapcloser.SpellName == "LeonaZenithBlade")
                {
                    if (Player.Distance(gapcloser.Start) < Player.Distance(gapcloser.End))
                        PullFlay(gapcloser.Sender);
                    else
                        LeagueSharp.Common.Utility.DelayAction.Add(75, delegate { PushFlay(gapcloser.Sender); });
                }
                else
                {
                    if (Player.Distance(gapcloser.Start) < Player.Distance(gapcloser.End))
                        PullFlay(gapcloser.Sender);
                    else
                        PushFlay(gapcloser.Sender);
                }
            }
        }

        private static void UpdateBuffs()
        {
            if (hookedUnit == null)
            {
                foreach (
                    var obj in
                        ObjectManager.Get<Obj_AI_Base>()
                            .Where(unit => unit.Team != Player.Team)
                            .Where(obj => obj.HasBuff("threshqfakeknockup")))
                {
                    hookedUnit = obj;
                    hookTick = Environment.TickCount + 1500;
                    return;
                }
            }
            hookTick = 0;
            hookedUnit = null;
        }

        private static void UpdateSouls()
        {
            var remove = soulList.Where(soul => !soul.IsValid).ToList();
            foreach (var soul in remove)
            {
                soulList.Remove(soul);
            }
        }

        private static bool ShouldPull(AIHeroClient unit)
        {
            return
                Config.SubMenu("Flay")
                    .SubMenu("ActionToTake")
                    .Item(unit.ChampionName)
                    .GetValue<StringList>()
                    .SelectedIndex == 0;
        }

        private static bool IsFirstQ()
        {
            return _Q.Instance.Name == "ThreshQ";
        }

        private static bool IsSecondQ()
        {
            return _Q.Instance.Name == "threshqleap";
        }

        private static bool IsImmune(Obj_AI_Base unit)
        {
            return unit.HasBuff("BlackShield") || unit.HasBuff("SivirE") || unit.HasBuff("NocturneShroudofDarkness") ||
                   unit.HasBuff("deathdefiedbuff");
        }

        private static void KS()
        {
            if (Config.SubMenu("KS").Item("ksE").GetValue<bool>())
            {
                foreach (
                    var enemy in
                        from enemy in
                            ObjectManager.Get<AIHeroClient>().Where(unit => unit.Team != Player.Team && !unit.IsDead)
                        let eDmg = Player.GetSpellDamage(enemy, SpellSlot.E)
                        where eDmg > enemy.Health && Player.Distance(enemy) <= _E.Range && _E.IsReady()
                        select enemy)
                {
                    PullFlay(enemy);
                    return;
                }
            }

            if (Config.SubMenu("KS").Item("ksQ").GetValue<bool>())
            {
                foreach (
                    var enemy in
                        from enemy in
                            ObjectManager.Get<AIHeroClient>().Where(unit => unit.Team != Player.Team && !unit.IsDead)
                        let qDmg = Player.GetSpellDamage(enemy, SpellSlot.Q)
                        where qDmg > enemy.Health && Player.Distance(enemy) <= qRange && IsFirstQ() && _Q.IsReady() &&
                              _Q.GetPrediction(enemy, overrideRange: qRange).Hitchance >= GetSelectedHitChance()
                        select enemy)
                {
                    _Q.Cast(enemy);
                    return;
                }
            }
        }

        private static HitChance GetSelectedHitChance()
        {
            switch (Config.SubMenu("Misc").Item("qHitChance").GetValue<StringList>().SelectedIndex)
            {
                case 3:
                    return HitChance.VeryHigh;
                case 2:
                    return HitChance.High;
                case 1:
                    return HitChance.Medium;
                case 0:
                    return HitChance.Low;
            }
            return HitChance.Medium;
        }

        private static void AutoBox()
        {
            if (Config.SubMenu("Box").Item("useR").GetValue<bool>() && _R.IsReady() &&
                ObjectManager.Get<AIHeroClient>()
                    .Count(unit => unit.Team != Player.Team && Player.Distance(unit) <= _R.Range) >=
                Config.SubMenu("Box").Item("minEnemies").GetValue<Slider>().Value)
            {
                _R.Cast();
            }
        }

        private static void Combo()
        {
            if (Config.SubMenu("Combo").Item("useE").GetValue<bool>() && _E.IsReady() &&
                Player.Distance(currentTarget) < _E.Range &&
                (!_Q.IsReady() && Environment.TickCount > qTick || _Q.IsReady() && IsFirstQ()))
            {
                Flay(currentTarget);
            }
            else if (Config.SubMenu("Combo").Item("useQ2").GetValue<bool>() && Player.Distance(currentTarget) > _E.Range &&
                     _Q.IsReady() &&
                     Environment.TickCount >= hookTick - 500 && IsSecondQ() &&
                     ObjectManager.Get<AIHeroClient>().FirstOrDefault(unit => unit.HasBuff("ThreshQ")) != null)
            {
                _Q.Cast();
            }
            else if (Config.SubMenu("Combo").Item("useQ2").GetValue<bool>() &&
                     Config.SubMenu("Combo").Item("useE").GetValue<bool>() && _Q.IsReady() &&
                     _E.IsReady() &&
                     ObjectManager.Get<Obj_AI_Minion>()
                         .FirstOrDefault(unit => unit.HasBuff("ThreshQ") && unit.Distance(currentTarget) <= _E.Range) !=
                     null && IsSecondQ())
            {
                _Q.Cast();
            }

            if (Config.SubMenu("Combo").Item("useQ1").GetValue<bool>() && _Q.IsReady() && IsFirstQ() &&
                !IsImmune(currentTarget))
            {
                _Q.CastIfHitchanceEquals(currentTarget, GetSelectedHitChance());
            }

            if (Config.SubMenu("Lantern").Item("useW").GetValue<bool>() && _W.IsReady() &&
                ObjectManager.Get<AIHeroClient>().FirstOrDefault(unit => unit.HasBuff("ThreshQ")) != null)
            {
                var nearAlly = GetNearAlly();
                if (nearAlly != null)
                {
                    _W.Cast(nearAlly);
                }
            }
        }

        private static void Harass()
        {
            var percentManaAfterQ = 100*((Player.Mana - _Q.Instance.SData.Mana)/Player.MaxMana);
            var percentManaAfterE = 100*((Player.Mana - _E.Instance.SData.Mana)/Player.MaxMana);
            var minPercentMana = Config.SubMenu("Harass").Item("manaPercent").GetValue<Slider>().Value;

            if (Config.SubMenu("Harass").Item("useQ1").GetValue<bool>() && _Q.IsReady() && IsFirstQ() &&
                !IsImmune(currentTarget) && percentManaAfterQ >= minPercentMana)
            {
                if (_Q.GetPrediction(currentTarget, false, qRange).Hitchance >= GetSelectedHitChance())
                {
                    _Q.Cast(currentTarget);
                }
            }
            else if (Config.SubMenu("Harass").Item("useE").GetValue<bool>() && !IsImmune(currentTarget) && _E.IsReady() &&
                     Player.Distance(currentTarget) < _E.Range && percentManaAfterE >= minPercentMana)
            {
                Flay(currentTarget);
            }
        }

        private static void Lantern()
        {
            if (Config.SubMenu("Lantern").Item("useWCC").GetValue<bool>() && GetCCAlly() != null && _W.IsReady())
            {
                _W.Cast(GetCCAlly());
                return;
            }

            if (Config.SubMenu("Lantern").Item("useW").GetValue<bool>() && GetLowAlly() != null && _W.IsReady())
            {
                if (GetLowAlly().Position.CountEnemiesInRange(950) >=
                    Config.SubMenu("Lantern").Item("numEnemies").GetValue<Slider>().Value)
                {
                    _W.Cast(GetLowAlly());
                }
            }
        }

        private static AIHeroClient GetCCAlly()
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        unit =>
                            !unit.IsMe && unit.Team == Player.Team && !unit.IsDead &&
                            Player.Distance(unit) <= _W.Range + 200)
                    .FirstOrDefault(
                        ally =>
                            ally.HasBuffOfType(BuffType.Charm) || ally.HasBuffOfType(BuffType.CombatDehancer) ||
                            ally.HasBuffOfType(BuffType.Fear) || ally.HasBuffOfType(BuffType.Knockback) ||
                            ally.HasBuffOfType(BuffType.Knockup) || ally.HasBuffOfType(BuffType.Polymorph) ||
                            ally.HasBuffOfType(BuffType.Snare) || ally.HasBuffOfType(BuffType.Stun) ||
                            ally.HasBuffOfType(BuffType.Suppression) || ally.HasBuffOfType(BuffType.Taunt));
        }

        private static AIHeroClient GetLowAlly()
        {
            AIHeroClient lowAlly = null;
            foreach (
                var ally in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            unit => unit.Team == Player.Team && !unit.IsDead && Player.Distance(unit) <= _W.Range + 200)
                )
            {
                if (lowAlly == null)
                    lowAlly = ally;
                else if (!lowAlly.IsDead && ally.Health/ally.MaxHealth < lowAlly.Health/lowAlly.MaxHealth)
                    lowAlly = ally;
            }
            return lowAlly;
        }

        private static AIHeroClient GetNearAlly()
        {
            if (TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget is AIHeroClient && TargetSelector.SelectedTarget.Team == Player.Team &&
                Player.Distance(TargetSelector.SelectedTarget.Position) <= _W.Range + 200)
            {
                return (AIHeroClient) TargetSelector.SelectedTarget;
            }

            AIHeroClient nearAlly = null;
            foreach (
                var ally in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            unit => unit.Team == Player.Team && !unit.IsDead && Player.Distance(unit) <= _W.Range + 200)
                )
            {
                if (nearAlly == null)
                    nearAlly = ally;
                else if (!nearAlly.IsDead && Player.Distance(ally) < Player.Distance(nearAlly))
                    nearAlly = ally;
            }
            return nearAlly;
        }

        private static void PushFlay(Obj_AI_Base unit)
        {
            if (Player.Distance(unit) <= _E.Range)
            {
                _E.Cast(unit.ServerPosition);
            }
        }

        private static void PullFlay(Obj_AI_Base unit)
        {
            if (Player.Distance(unit) <= _E.Range)
            {
                var pX = Player.Position.X + (Player.Position.X - unit.Position.X);
                var pY = Player.Position.Y + (Player.Position.Y - unit.Position.Y);
                _E.Cast(new Vector2(pX, pY));
            }
        }

        private static void Flay(AIHeroClient unit)
        {
            if (ShouldPull(unit))
            {
                PullFlay(unit);
            }
            else
            {
                PushFlay(unit);
            }
        }
    }
}
