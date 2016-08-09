using EloBuddy; namespace ElKatarina
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal static class Program
    {
        #region Static Fields

        private static readonly bool castWardAgain = true;

        private static readonly AIHeroClient Player = ObjectManager.Player;

        private static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                                       {
                                                                           { Spells.Q, new Spell(SpellSlot.Q, 675) },
                                                                           { Spells.W, new Spell(SpellSlot.W, 375) },
                                                                           { Spells.E, new Spell(SpellSlot.E, 700) },
                                                                           { Spells.R, new Spell(SpellSlot.R, 550) }
                                                                       };

        private static Vector3 _lastWardPos;

        private static Menu config;

        private static SpellSlot igniteSlot;

        private static bool IsChanneling;

        private static Vector2 JumpPos;

        private static long lastECast;

        private static int lastPlaced;

        private static Vector3 lastWardPos;

        private static Orbwalking.Orbwalker orbwalker;

        private static bool reCheckWard = true;

        private static float rStart;

        private static float wcasttime;

        #endregion

        #region Methods

        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe)
            {
                args.Process = !Player.HasBuff("KatarinaR");
            }
        }

        private static void CastE(Obj_AI_Base unit)
        {
            var playLegit = config.Item("playLegit").GetValue<bool>();
            var legitCastDelay = config.Item("legitCastDelay").GetValue<Slider>().Value;

            if (playLegit)
            {
                if (Environment.TickCount > lastECast + legitCastDelay)
                {
                    spells[Spells.E].CastOnUnit(unit);
                    lastECast = Environment.TickCount;
                }
            }
            else
            {
                spells[Spells.E].CastOnUnit(unit);
                lastECast = Environment.TickCount;
            }
        }

        private static void CastEWard(Obj_AI_Base obj)
        {
            if (500 >= Environment.TickCount - wcasttime)
            {
                return;
            }

            spells[Spells.E].CastOnUnit(obj);
            wcasttime = Environment.TickCount;
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            UseItems(target);

            var rdmg = spells[Spells.R].GetDamage(target, 1);

            if (spells[Spells.Q].IsInRange(target))
            {
                if (spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast(target);
                    return;
                }
                if (spells[Spells.E].IsReady())
                {
                    CastE(target);
                    return;
                }
            }
            else
            {
                if (spells[Spells.E].IsReady())
                {
                    CastE(target);
                    return;
                }
                if (spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast(target);
                    return;
                }
            }

            if (spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
            {
                spells[Spells.W].Cast();
                return;
            }

            //Smart R
            if (config.Item("smartR").GetValue<bool>())
            {
                if (spells[Spells.R].IsReady() && target.Health - rdmg < 0 && !spells[Spells.E].IsReady())
                {
                    orbwalker.SetMovement(false);
                    orbwalker.SetAttack(false);
                    spells[Spells.R].Cast();

                    rStart = Environment.TickCount;
                }
            }
            else if (spells[Spells.R].IsReady() && !spells[Spells.E].IsReady())
            {
                orbwalker.SetMovement(false);
                orbwalker.SetAttack(false);
                spells[Spells.R].Cast();

                rStart = Environment.TickCount;
            }
        }

        private static void Drawings(EventArgs args)
        {
            var drawOff = config.Item("mDraw").GetValue<bool>();
            var drawQ = config.Item("QDraw").GetValue<Circle>();
            var drawW = config.Item("WDraw").GetValue<Circle>();
            var drawE = config.Item("EDraw").GetValue<Circle>();
            var drawR = config.Item("RDraw").GetValue<Circle>();

            if (drawOff)
            {
                return;
            }

            if (drawQ.Active)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawW.Active)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, Color.White);
                }
            }

            if (drawE.Active)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (drawR.Active)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White);
                }
            }

            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (config.Item("Target").GetValue<Circle>().Active && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 50, config.Item("Target").GetValue<Circle>().Color);
            }
        }

        private static void Farm()
        {
            foreach (var minion in
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        minion =>
                        minion.IsValidTarget() && minion.IsEnemy
                        && minion.Distance(Player.ServerPosition) < spells[Spells.E].Range))
            {
                var qdmg = spells[Spells.Q].GetDamage(minion);
                var wdmg = spells[Spells.W].GetDamage(minion);
                var edmg = spells[Spells.E].GetDamage(minion);
                var markDmg = Player.CalcDamage(
                    minion,
                    Damage.DamageType.Magical,
                    Player.FlatMagicDamageMod * 0.15 + Player.Level * 15);

                //Killable with Q
                if (minion.Health - qdmg <= 0 && minion.Distance(Player.ServerPosition) <= spells[Spells.Q].Range
                    && spells[Spells.Q].IsReady() && (config.Item("wFarm").GetValue<bool>()))
                {
                    spells[Spells.Q].CastOnUnit(minion);
                }

                if (minion.Health - wdmg <= 0 && minion.Distance(Player.ServerPosition) <= spells[Spells.W].Range
                    && spells[Spells.W].IsReady() && (config.Item("wFarm").GetValue<bool>()))
                {
                    spells[Spells.Q].Cast();
                    return;
                }

                if (minion.Health - edmg <= 0 && minion.Distance(Player.ServerPosition) <= spells[Spells.E].Range
                    && spells[Spells.E].IsReady() && (config.Item("eFarm").GetValue<bool>()))
                {
                    CastE(minion);
                    return;
                }

                if (minion.Health - wdmg - qdmg <= 0 && minion.Distance(Player.ServerPosition) <= spells[Spells.W].Range
                    && spells[Spells.Q].IsReady() && spells[Spells.W].IsReady()
                    && (config.Item("qFarm").GetValue<bool>()) && (config.Item("wFarm").GetValue<bool>()))
                {
                    spells[Spells.Q].Cast(minion);
                    spells[Spells.W].Cast();
                    return;
                }

                if (minion.Health - wdmg - qdmg - markDmg <= 0
                    && minion.Distance(Player.ServerPosition) <= spells[Spells.W].Range && spells[Spells.Q].IsReady()
                    && spells[Spells.W].IsReady() && (config.Item("qFarm").GetValue<bool>())
                    && (config.Item("wFarm").GetValue<bool>()))
                {
                    spells[Spells.Q].Cast(minion);
                    spells[Spells.W].Cast();
                    return;
                }

                if (minion.Health - wdmg - qdmg - markDmg - edmg <= 0
                    && minion.Distance(Player.ServerPosition) <= spells[Spells.W].Range && spells[Spells.E].IsReady()
                    && spells[Spells.Q].IsReady() && spells[Spells.W].IsReady()
                    && (config.Item("qFarm").GetValue<bool>()) && (config.Item("wFarm").GetValue<bool>())
                    && (config.Item("eFarm").GetValue<bool>()))
                {
                    CastE(minion);
                    spells[Spells.Q].Cast(minion);
                    spells[Spells.W].Cast();
                    return;
                }
            }
        }

        private static InventorySlot FindBestWardItem()
        {
            var slot = Items.GetWardSlot();
            if (slot == default(InventorySlot))
            {
                return null;
            }

            var sdi = GetItemSpell(slot);

            if (sdi != default(SpellDataInst) && sdi.State == SpellState.Ready)
            {
                return slot;
            }
            return slot;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!spells[Spells.E].IsReady() || !(sender is Obj_AI_Minion) || Environment.TickCount >= lastPlaced + 300)
            {
                return;
            }

            if (Environment.TickCount >= lastPlaced + 300)
            {
                return;
            }
            var ward = (Obj_AI_Minion)sender;

            if (ward.Name.ToLower().Contains("ward") && ward.Distance(_lastWardPos) < 500)
            {
                spells[Spells.E].Cast(ward);
            }
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (spells[Spells.Q].IsReady())
            {
                damage += spells[Spells.Q].GetDamage(enemy);
            }

            if (spells[Spells.W].IsReady())
            {
                damage += spells[Spells.W].GetDamage(enemy);
            }

            if (spells[Spells.E].IsReady())
            {
                damage += spells[Spells.E].GetDamage(enemy);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += spells[Spells.R].GetDamage(enemy);
            }

            if (igniteSlot == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(igniteSlot) != SpellState.Ready)
            {
                damage += (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            return damage;
        }

        private static SpellDataInst GetItemSpell(InventorySlot invSlot)
        {
            return Player.Spellbook.Spells.FirstOrDefault(spell => (int)spell.Slot == invSlot.Slot + 4);
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var menuItem = config.Item("hMode").GetValue<StringList>().SelectedIndex;

            switch (menuItem)
            {
                case 0:
                    if (spells[Spells.Q].IsReady())
                    {
                        spells[Spells.Q].CastOnUnit(target);
                    }
                    break;
                case 1:
                    if (spells[Spells.Q].IsReady() && spells[Spells.W].IsReady())
                    {
                        spells[Spells.Q].Cast(target);
                        if (spells[Spells.W].IsInRange(target))
                        {
                            spells[Spells.W].Cast();
                        }
                    }
                    break;
                case 2:
                    if (spells[Spells.Q].IsReady() && spells[Spells.W].IsReady() && spells[Spells.E].IsReady())
                    {
                        spells[Spells.Q].Cast(target);
                        CastE(target);
                        spells[Spells.W].Cast();
                    }
                    break;
            }
        }

        private static bool HasRBuff()
        {
            return Player.HasBuff("KatarinaR") || Player.IsChannelingImportantSpell()
                   || Player.HasBuff("KatarinaRSound");
        }

        private static void JungleClear()
        {
            var mobs = MinionManager.GetMinions(
                Player.ServerPosition,
                spells[Spells.W].Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (mobs.Count <= 0)
            {
                return;
            }

            var mob = mobs[0];
            if (mob == null)
            {
                return;
            }

            if (config.Item("qJungle").IsActive() && spells[Spells.Q].IsReady())
            {
                spells[Spells.Q].CastOnUnit(mob);
            }

            if (config.Item("wJungle").IsActive() && spells[Spells.W].IsReady())
            {
                spells[Spells.W].CastOnUnit(mob);
            }

            if (config.Item("eJungle").IsActive() && spells[Spells.E].IsReady())
            {
                spells[Spells.E].CastOnUnit(mob);
            }
        }

        private static void KillSteal()
        {
            if (config.Item("KillSteal").IsActive())
            {
                foreach (var enemy in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(spells[Spells.E].Range + spells[Spells.Q].Range) && !x.IsZombie)
                        .OrderBy(x => x.Health))
                {
                    if (spells[Spells.E].IsInRange(enemy))
                    {
                        if (spells[Spells.Q].IsReady() && spells[Spells.Q].GetDamage(enemy) > enemy.Health)
                        {
                            spells[Spells.Q].Cast();
                            return;
                        }

                        if (spells[Spells.E].IsReady() && spells[Spells.E].GetDamage(enemy) > enemy.Health)
                        {
                            spells[Spells.E].CastOnUnit(enemy);
                            return;
                        }

                        if (spells[Spells.W].IsReady() && spells[Spells.W].GetDamage(enemy) > enemy.Health)
                        {
                            spells[Spells.W].Cast();
                            return;
                        }
                    }
                }
            }
        }

        private static void Laneclear()
        {
            var useQ = config.Item("qFarm").IsActive();
            var useW = config.Item("qFarm").IsActive();
            var useE = config.Item("eFarm").IsActive();

            var minions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range);
            if (minions.Count <= 0)
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && useQ)
            {
                foreach (var minion in
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            minion =>
                            minion.IsValidTarget() && minion.IsEnemy
                            && minion.Distance(Player.ServerPosition) < spells[Spells.E].Range))
                {
                    spells[Spells.Q].CastOnUnit(minion);
                    return;
                }
            }

            if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(minions.FirstOrDefault())) //check
            {
                if (minions.Count > 2)
                {
                    spells[Spells.W].Cast();
                    return;
                }
            }

            foreach (var minion in
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        minion =>
                        minion.IsValidTarget() && minion.IsEnemy
                        && minion.Distance(Player.ServerPosition) < spells[Spells.E].Range))
            {
                var edmg = spells[Spells.E].GetDamage(minion);

                if (useE && minion.Health - edmg <= 0
                    && minion.Distance(Player.ServerPosition) <= spells[Spells.E].Range && spells[Spells.E].IsReady()
                    && (config.Item("eFarm").GetValue<bool>()))
                {
                    CastE(minion);
                    return;
                }
            }
        }

        private static void MenuLoad()
        {
            config = new Menu("ElKatarina", "Katarina", true);

            //Orbwalker Menu
            config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            orbwalker = new Orbwalking.Orbwalker(config.SubMenu("Orbwalking"));

            //Target Selector Menu
            var tsMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(tsMenu);
            config.AddSubMenu(tsMenu);

            config.AddSubMenu(new Menu("Combo", "combo"));
            config.SubMenu("combo").AddItem(new MenuItem("smartR", "Use Smart R").SetValue(true));
            config.SubMenu("combo").AddItem(new MenuItem("wjCombo", "Use WardJump in Combo").SetValue(true));
            config.SubMenu("combo")
                .AddItem(new MenuItem("ElKatarina.Items.hextech", "Use Hextech Gunblade").SetValue(true));

            config.AddSubMenu(new Menu("Harass", "harass"));
            config.SubMenu("harass")
                .AddItem(
                    new MenuItem("hMode", "Harass Mode: ").SetValue(new StringList(new[] { "Q only", "Q+W", "Q+E+W" })));

            config.SubMenu("harass")
                .SubMenu("AutoHarass settings")
                .AddItem(
                    new MenuItem("ElKatarina.AutoHarass.Activated", "Auto harass", true).SetValue(
                        new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle)));
            config.SubMenu("harass")
                .SubMenu("AutoHarass settings")
                .AddItem(new MenuItem("ElKatarina.AutoHarass.Q", "Use Q").SetValue(true));
            config.SubMenu("harass")
                .SubMenu("AutoHarass settings")
                .AddItem(new MenuItem("ElKatarina.AutoHarass.W", "Use W").SetValue(true));

            config.AddSubMenu(new Menu("Farm", "farm"));
            config.SubMenu("farm").AddItem(new MenuItem("smartFarm", "Use Smart Farm").SetValue(true));
            config.SubMenu("farm").AddItem(new MenuItem("qFarm", "Lane Q").SetValue(true));
            config.SubMenu("farm").AddItem(new MenuItem("wFarm", "Lane W").SetValue(true));
            config.SubMenu("farm").AddItem(new MenuItem("eFarm", "Lane E").SetValue(true));

            config.SubMenu("farm").AddItem(new MenuItem("qJungle", "Jungle Q").SetValue(true));
            config.SubMenu("farm").AddItem(new MenuItem("wJungle", "Jungle W").SetValue(true));
            config.SubMenu("farm").AddItem(new MenuItem("eJungle", "Jungle E").SetValue(true));

            config.AddSubMenu(new Menu("Killsteal", "KillSteal"));
            config.SubMenu("KillSteal").AddItem(new MenuItem("KillSteal", "Smart").SetValue(true));
            config.SubMenu("KillSteal").AddItem(new MenuItem("jumpsS", "Use E").SetValue(true));

            config.AddSubMenu(new Menu("Draw", "drawing"));
            config.SubMenu("drawing").AddItem(new MenuItem("mDraw", "Disable all drawings").SetValue(false));
            config.SubMenu("drawing")
                .AddItem(
                    new MenuItem("Target", "Highlight Target").SetValue(
                        new Circle(true, Color.FromArgb(255, 255, 255, 0))));
            config.SubMenu("drawing")
                .AddItem(new MenuItem("QDraw", "Draw Q").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            config.SubMenu("drawing")
                .AddItem(new MenuItem("WDraw", "Draw W").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            config.SubMenu("drawing")
                .AddItem(new MenuItem("EDraw", "Draw E").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            config.SubMenu("drawing")
                .AddItem(new MenuItem("RDraw", "Draw R").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));

            var dmgAfterE = new MenuItem("ElKatarina.DrawComboDamage", "Draw combo damage").SetValue(true);
            var drawFill =
                new MenuItem("ElKatarina.DrawColour", "Fill colour", true).SetValue(
                    new Circle(true, Color.FromArgb(204, 255, 0, 1)));
            config.SubMenu("drawing").AddItem(drawFill);
            config.SubMenu("drawing").AddItem(dmgAfterE);

            DrawDamage.DamageToUnit = GetComboDamage;
            DrawDamage.Enabled = dmgAfterE.GetValue<bool>();
            DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;

            dmgAfterE.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                    };

            drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                    DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };

            //Misc Menu
            config.AddSubMenu(new Menu("Misc", "misc"));
            config.SubMenu("misc").AddItem(new MenuItem("playLegit", "Legit E").SetValue(false));
            config.SubMenu("misc")
                .AddItem(new MenuItem("legitCastDelay", "Legit E Delay").SetValue(new Slider(1000, 0, 2000)));

            var wMenu = new Menu("Wardjump", "Wardjump");
            wMenu.AddItem(
                new MenuItem("ElEasy.Katarina.Wardjump", "Wardjump key").SetValue(
                    new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));

            wMenu.AddItem(new MenuItem("ElEasy.Wardjump.Mouse", "Move to mouse").SetValue(true));
            wMenu.AddItem(new MenuItem("ElEasy.Wardjump.Minions", "Jump to minions").SetValue(false));
            wMenu.AddItem(new MenuItem("ElEasy.Wardjump.Champions", "Jump to champions").SetValue(false));

            config.AddSubMenu(wMenu);

            if (igniteSlot != SpellSlot.Unknown)
            {
                config.SubMenu("misc").AddItem(new MenuItem("autoIgnite", "Auto ignite when killable").SetValue(true));
            }

            var credits = new Menu("Credits", "jQuery");
            credits.AddItem(new MenuItem("ElKatarina.Paypal", "if you would like to donate via paypal:"));
            credits.AddItem(new MenuItem("ElKatarina.Email", "info@zavox.nl"));
            config.AddSubMenu(credits);

            config.AddItem(new MenuItem("422442fsaafs4242f", ""));
            config.AddItem(new MenuItem("422442fsaafsf", "Version: 1.0.0.9"));
            config.AddItem(new MenuItem("fsasfafsfsafsa", "Made By Jouza - jQuery "));

            config.AddToMainMenu();
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || args.SData.Name != "KatarinaR" || !Player.HasBuff("katarinarsound"))
            {
                return;
            }

            IsChanneling = true;
            orbwalker.SetMovement(false);
            orbwalker.SetAttack(false);
            LeagueSharp.Common.Utility.DelayAction.Add(1, () => IsChanneling = false);
        }

        private static void AIHeroClient_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && Environment.TickCount < rStart + 300 && args.Order == GameObjectOrder.MoveTo)
            {
                args.Process = false;
            }
        }

        private static void OnAutoHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid || orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            var useQ = config.Item("ElKatarina.AutoHarass.Q").GetValue<bool>();
            var useW = config.Item("ElKatarina.AutoHarass.W").GetValue<bool>();

            if (spells[Spells.Q].IsReady() && target.IsValidTarget() && useQ)
            {
                spells[Spells.Q].Cast(target);
            }
            ;

            if (spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range) && useW)
            {
                spells[Spells.W].Cast();
            }
        }

        public static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Katarina")
            {
                return;
            }

            igniteSlot = Player.GetSpellSlot("summonerdot");

            spells[Spells.R].SetCharged("KatarinaR", "KatarinaR", 550, 550, 1.0f);

            Drawing.OnDraw += Drawings;
            MenuLoad();
            Game.OnUpdate += OnUpdate;
            EloBuddy.Player.OnIssueOrder += AIHeroClient_OnIssueOrder;
            GameObject.OnCreate += GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Orbwalking.BeforeAttack += BeforeAttack;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }

            if (HasRBuff())
            {
                orbwalker.SetAttack(false);
                orbwalker.SetMovement(false);
                return;
            }

            orbwalker.SetAttack(true);
            orbwalker.SetMovement(true);

            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    Farm();
                    break;
            }

            KillSteal();

            if (config.Item("ElEasy.Katarina.Wardjump").GetValue<KeyBind>().Active)
            {
                WardjumpToMouse();
            }

            var autoHarass = config.Item("ElKatarina.AutoHarass.Activated", true).GetValue<KeyBind>().Active;
            if (autoHarass)
            {
                OnAutoHarass();
            }
        }

        private static void Orbwalk(Vector3 pos, AIHeroClient target = null)
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, pos);
        }

        private static void UseItems(Obj_AI_Base target)
        {
            var useHextech = config.Item("ElKatarina.Items.hextech").GetValue<bool>();
            if (useHextech)
            {
                var cutlass = ItemData.Bilgewater_Cutlass.GetItem();
                var hextech = ItemData.Hextech_Gunblade.GetItem();

                if (cutlass.IsReady() && cutlass.IsOwned(Player) && cutlass.IsInRange(target))
                {
                    cutlass.Cast(target);
                }

                if (hextech.IsReady() && hextech.IsOwned(Player) && hextech.IsInRange(target))
                {
                    hextech.Cast(target);
                }
            }
        }

        private static void WardJump(
            Vector3 pos,
            bool m2M = true,
            bool maxRange = false,
            bool reqinMaxRange = false,
            bool minions = true,
            bool champions = true)
        {
            if (!spells[Spells.E].IsReady())
            {
                return;
            }

            var basePos = Player.Position.To2D();
            var newPos = (pos.To2D() - Player.Position.To2D());

            if (JumpPos == new Vector2())
            {
                if (reqinMaxRange)
                {
                    JumpPos = pos.To2D();
                }
                else if (maxRange || Player.Distance(pos) > 590)
                {
                    JumpPos = basePos + (newPos.Normalized() * (590));
                }
                else
                {
                    JumpPos = basePos + (newPos.Normalized() * (Player.Distance(pos)));
                }
            }
            if (JumpPos != new Vector2() && reCheckWard)
            {
                reCheckWard = false;
                LeagueSharp.Common.Utility.DelayAction.Add(
                    20,
                    () =>
                        {
                            if (JumpPos != new Vector2())
                            {
                                JumpPos = new Vector2();
                                reCheckWard = true;
                            }
                        });
            }
            if (m2M)
            {
                Orbwalk(pos);
            }
            if (!spells[Spells.E].IsReady() || reqinMaxRange && Player.Distance(pos) > spells[Spells.E].Range)
            {
                return;
            }

            if (minions || champions)
            {
                if (champions)
                {
                    var champs = (from champ in ObjectManager.Get<AIHeroClient>()
                                  where
                                      champ.IsAlly && champ.Distance(Player) < spells[Spells.E].Range
                                      && champ.Distance(pos) < 200 && !champ.IsMe
                                  select champ).ToList();
                    if (champs.Count > 0 && spells[Spells.E].IsReady())
                    {
                        if (500 >= Environment.TickCount - wcasttime || !spells[Spells.E].IsReady())
                        {
                            return;
                        }

                        CastEWard(champs[0]);
                        return;
                    }
                }
                if (minions)
                {
                    var minion2 = (from minion in ObjectManager.Get<Obj_AI_Minion>()
                                   where
                                       minion.IsAlly && minion.Distance(Player) < spells[Spells.E].Range
                                       && minion.Distance(pos) < 200 && !minion.Name.ToLower().Contains("ward")
                                   select minion).ToList();
                    if (minion2.Count > 0)
                    {
                        if (500 >= Environment.TickCount - wcasttime || !spells[Spells.E].IsReady())
                        {
                            return;
                        }

                        CastEWard(minion2[0]);
                        return;
                    }
                }
            }

            var isWard = false;
            foreach (var ward in ObjectManager.Get<Obj_AI_Base>())
            {
                if (ward.IsAlly && ward.Name.ToLower().Contains("ward") && ward.Distance(JumpPos) < 200)
                {
                    isWard = true;
                    if (500 >= Environment.TickCount - wcasttime || !spells[Spells.E].IsReady())
                    {
                        return;
                    }

                    CastEWard(ward);
                    wcasttime = Environment.TickCount;
                }
            }

            if (!isWard && castWardAgain)
            {
                var ward = FindBestWardItem();
                if (ward == null || !spells[Spells.E].IsReady())
                {
                    return;
                }

                Player.Spellbook.CastSpell(ward.SpellSlot, JumpPos.To3D());
                lastWardPos = JumpPos.To3D();
            }
        }

        private static void WardjumpToMouse()
        {
            WardJump(
                Game.CursorPos,
                config.Item("ElEasy.Wardjump.Mouse").GetValue<bool>(),
                false,
                false,
                config.Item("ElEasy.Wardjump.Minions").GetValue<bool>(),
                config.Item("ElEasy.Wardjump.Champions").GetValue<bool>());
        }

        #endregion
    }
}