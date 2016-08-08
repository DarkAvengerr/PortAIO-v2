using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SAssemblies.Champions
{
    using System.Linq;
    using System.Runtime.CompilerServices;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class TahmKench
    {
        public static Menu.MenuItemSettings TahmKenchChampion = new Menu.MenuItemSettings(typeof(TahmKench));

        private static Orbwalking.Orbwalker orbwalker;

        private SwallowedUnit swallowedUnit;
        private string tahmPassive = "TahmKenchPDebuffCounter";

        private String tahmEatingPassive = "tahmkenchwdevoured";
        private String tahmEatPassive = "TahmKenchWHasDevouredTarget";

        private Vector3 lastPosBeforeSwallowing;

        //W Blacklist

        public TahmKench()
        {
            Game.OnUpdate += this.Game_OnGameUpdate;
            Drawing.OnEndScene += this.Drawing_OnEndScene;
            Obj_AI_Base.OnBuffGain += this.AIHeroClient_OnBuffAdd;
            Obj_AI_Base.OnBuffLose += this.AIHeroClient_OnBuffLose;
            Obj_AI_Base.OnLevelUp += this.AIHeroClient_OnLevelUp;
            Interrupter2.OnInterruptableTarget += this.Interrupter2_OnInterruptableTarget;
        }

        ~TahmKench()
        {
            Game.OnUpdate -= this.Game_OnGameUpdate;
            Drawing.OnEndScene -= this.Drawing_OnEndScene;
            Obj_AI_Base.OnBuffGain -= this.AIHeroClient_OnBuffAdd;
            Obj_AI_Base.OnBuffLose -= this.AIHeroClient_OnBuffLose;
            Obj_AI_Base.OnLevelUp -= this.AIHeroClient_OnLevelUp;
            Interrupter2.OnInterruptableTarget -= this.Interrupter2_OnInterruptableTarget;
        }

        public static bool IsActive()
        {
#if MISCS
            return Champion.Champions.GetActive() && TahmKenchChampion.GetActive();
#else
            return TahmKenchChampion.GetActive() && ObjectManager.Player.ChampionName.Equals("TahmKench");
#endif
        }

        public static Menu.MenuItemSettings SetupMenu(LeagueSharp.Common.Menu menu)
        {
            if (!ObjectManager.Player.ChampionName.Equals("TahmKench"))
            {
                return null;
            }
            var newMenu = Menu.GetSubMenu(menu, "SAssembliesChampionsTahmKench");
            if (newMenu == null)
            {
                TahmKenchChampion.Menu = menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_TAHMKENCH_MAIN"), "SAssembliesChampionsTahmKench"));
                var orbwalkerMenu = TahmKenchChampion.Menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_ORBWALKER"), "SAssembliesChampionsTahmKenchOrbwalker"));
                orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                var comboMenu = TahmKenchChampion.Menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_COMBO"), "SAssembliesChampionsTahmKenchCombo"));
                comboMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchComboQ", Language.GetString("CHAMPIONS_CHAMPION_Q")).SetValue(true));
                comboMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchComboW", Language.GetString("CHAMPIONS_CHAMPION_W")).SetValue(true));
                comboMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchComboWMax", Language.GetString("CHAMPIONS_TAHMKENCH_COMBO_WMAX")).SetValue(new Slider(1, 0, 2)));
                var harassMenu = TahmKenchChampion.Menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_HARASS"), "SAssembliesChampionsTahmKenchHarass"));
                harassMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchHarassQ", Language.GetString("CHAMPIONS_CHAMPION_Q")).SetValue(true));
                harassMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchHarassW", Language.GetString("CHAMPIONS_CHAMPION_W")).SetValue(true));
                harassMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchHarassMinMana", Language.GetString("CHAMPIONS_CHAMPION_MANAPERCENT")).SetValue(new Slider(50, 0, 100)));
                var clearMenu = TahmKenchChampion.Menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_FARM"), "SAssembliesChampionsTahmKenchFarm"));
                var lasthitMenu = clearMenu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_FARM_LASTHIT"), "SAssembliesChampionsTahmKenchFarmLasthit"));
                lasthitMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchFarmLasthitQ", Language.GetString("CHAMPIONS_CHAMPION_Q")).SetValue(true));
                lasthitMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchFarmLasthitMinMana", Language.GetString("CHAMPIONS_CHAMPION_MANAPERCENT")).SetValue(new Slider(50, 0, 100)));
                var jungleMenu = clearMenu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_FARM_JUNGLE"), "SAssembliesChampionsTahmKenchFarmJungle"));
                jungleMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchFarmJungleQ", Language.GetString("CHAMPIONS_CHAMPION_Q")).SetValue(true));
                jungleMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchFarmJungleW", Language.GetString("CHAMPIONS_CHAMPION_W")).SetValue(true));
                jungleMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchFarmJungleMinMana", Language.GetString("CHAMPIONS_CHAMPION_MANAPERCENT")).SetValue(new Slider(50, 0, 100)));
                var laneMenu = clearMenu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_FARM_LANE"), "SAssembliesChampionsTahmKenchFarmLane"));
                laneMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchFarmLaneQ", Language.GetString("CHAMPIONS_CHAMPION_Q")).SetValue(true));
                laneMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchFarmLaneW", Language.GetString("CHAMPIONS_CHAMPION_W")).SetValue(true));
                laneMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchFarmLaneMinMana", Language.GetString("CHAMPIONS_CHAMPION_MANAPERCENT")).SetValue(new Slider(50, 0, 100)));
                //var fleeMenu = TahmKenchChampion.Menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_FLEE"), "SAssembliesChampionsTahmKenchFlee"));
                //fleeMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchFleeQ", Language.GetString("CHAMPIONS_CHAMPION_Q")).SetValue(true));
                //fleeMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchFleeWithAlly", Language.GetString("CHAMPIONS_TAHMKENCH_FLEE_ALLY")).SetValue(true));
                //fleeMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchFleeWithAllyRange", Language.GetString("CHAMPIONS_TAHMKENCH_FLEE_ALLY_RANGE")).SetValue(new Slider(100, 100, 500)));
                //fleeMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchFleeKey", Language.GetString("GLOBAL_KEY")).SetValue(new KeyBind('Z', KeyBindType.Press)));
                var qMenu = TahmKenchChampion.Menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_Q"), "SAssembliesChampionsTahmKenchQ"));
                qMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchQInterrupt", Language.GetString("CHAMPIONS_CHAMPION_INTERRUPT")).SetValue(true));
                qMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchQKillsteal", Language.GetString("CHAMPIONS_CHAMPION_KILLSTEAL")).SetValue(true));
                qMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchQDraw", Language.GetString("CHAMPIONS_CHAMPION_DRAW")).SetValue(true));
                var wMenu = TahmKenchChampion.Menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_W"), "SAssembliesChampionsTahmKenchW"));
                wMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchWInterrupt", Language.GetString("CHAMPIONS_CHAMPION_INTERRUPT")).SetValue(true));
                wMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchWKillsteal", Language.GetString("CHAMPIONS_CHAMPION_KILLSTEAL")).SetValue(true));
                wMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchWDraw", Language.GetString("CHAMPIONS_CHAMPION_DRAW")).SetValue(true));
                wMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchWDrawMax", Language.GetString("CHAMPIONS_TAHMKENCH_W_MAXMOVE")).SetValue(true));
                wMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchWAutoMoveToAlly", Language.GetString("CHAMPIONS_TAHMKENCH_W_AUTOMOVE")).SetValue(true));
                wMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchWAutoShieldAlly", Language.GetString("CHAMPIONS_TAHMKENCH_W_AUTOSHIELD")).SetValue(true));
                wMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchWAutoShieldAllyPercent", Language.GetString("CHAMPIONS_TAHMKENCH_W_SHIELD_PERCENT")).SetValue(new Slider(20, 1, 99)));
                var eMenu = TahmKenchChampion.Menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_E"), "SAssembliesChampionsTahmKenchE"));
                eMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchEShield", Language.GetString("CHAMPIONS_TAHMKENCH_E_SHIELD")).SetValue(true));
                eMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchEShieldPercent", Language.GetString("CHAMPIONS_TAHMKENCH_E_SHIELD_PERCENT")).SetValue(new Slider(20, 1, 99)));
                var rMenu = TahmKenchChampion.Menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_R"), "SAssembliesChampionsTahmKenchR"));
                //rMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchR", Language.GetString("GLOBAL_KEY")).SetValue(new KeyBind('U', KeyBindType.Press)));
                rMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchRDraw", Language.GetString("CHAMPIONS_CHAMPION_DRAW")).SetValue(true));
                //var itemsMenu = TahmKenchChampion.Menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_ITEMS"), "SAssembliesChampionsTahmKenchItems"));
                var trollMenu = TahmKenchChampion.Menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("CHAMPIONS_CHAMPION_TROLL"), "SAssembliesChampionsTahmKenchTroll"));
                trollMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchTrollW", Language.GetString("GLOBAL_KEY")).SetValue(new KeyBind('I', KeyBindType.Press)));
                trollMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchTrollWToEnemyHero", Language.GetString("CHAMPIONS_CHAMPION_TROLL_ENEMY_HERO")).SetValue(true));
                trollMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchTrollWToEnemyTurret", Language.GetString("CHAMPIONS_CHAMPION_TROLL_ENEMY_TURRET")).SetValue(true));
                //trollMenu.AddItem(new MenuItem("SAssembliesChampionsTahmKenchTrollWR", Language.GetString("GLOBAL_KEY")).SetValue(true));
                TahmKenchChampion.CreateActiveMenuItem("SAssembliesChampionsTahmKenchActive", () => new TahmKench());
            }
            return TahmKenchChampion;
        }

        static class CustomSpell
        {
            public static Spell Q = new Spell(SpellSlot.Q, 800, TargetSelector.DamageType.Magical);
            public static Spell W = new Spell(SpellSlot.W, 250);
            public static Spell W2 = new Spell(SpellSlot.W, 900, TargetSelector.DamageType.Magical);
            public static Spell E = new Spell(SpellSlot.E);
            public static Spell R;

            static CustomSpell()
            {
                Q.SetSkillshot(0.1f, 75, 2000, true, SkillshotType.SkillshotLine);
                W2.SetSkillshot(0.1f, 75, 900, true, SkillshotType.SkillshotLine);

                switch (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level)
                {
                    case 2:
                        R = new Spell(SpellSlot.R, 5500);
                        break;

                    case 3:
                        R = new Spell(SpellSlot.R, 6000);
                        break;

                    default:
                        R = new Spell(SpellSlot.R, 5000);
                        break;
                }
            }

            public static void CastSpell(Spell spell, Obj_AI_Base target, HitChance hitchance)
            {
                if (target.LSIsValidTarget(spell.Range) && spell.GetPrediction(target).Hitchance >= hitchance)
                {
                    spell.Cast(target);
                }
            }
        }

        public enum SwallowedUnit
        {
            None,
            Ally,
            Enemy,
            Minion
        }

        private void AIHeroClient_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            switch (CustomSpell.R.Level)
            {
                case 2:
                    CustomSpell.R.Range = 5500;
                    break;

                case 3:
                    CustomSpell.R.Range = 6000;
                    break;
            }
        }

        private void AIHeroClient_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (args.Buff.Name.Equals(this.tahmEatingPassive))
            {
                this.lastPosBeforeSwallowing = ObjectManager.Player.Position;
                var hero = sender as AIHeroClient;
                if (hero != null)
                {
                    if (hero.IsAlly)
                    {
                        this.swallowedUnit = SwallowedUnit.Ally;
                    }
                    else
                    {
                        this.swallowedUnit = SwallowedUnit.Enemy;
                    }
                    return;
                }
                var minion = sender as Obj_AI_Minion;
                if (minion != null)
                {
                    this.swallowedUnit = SwallowedUnit.Minion;
                }
            }
            else if (args.Buff.Name.Equals(this.tahmEatPassive.ToLower()) && !this.swallowedUnit.HasFlag(SwallowedUnit.Enemy | SwallowedUnit.Ally))
            {
                this.lastPosBeforeSwallowing = ObjectManager.Player.Position;
                var hero = sender as AIHeroClient;
                if (hero != null)
                {
                    if (hero.IsAlly)
                    {
                        this.swallowedUnit = SwallowedUnit.Ally;
                    }
                }
            }
        }

        private void AIHeroClient_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            if (args.Buff.Name.Equals(this.tahmEatPassive.ToLower()))
            {
                this.swallowedUnit = SwallowedUnit.None;
                this.lastPosBeforeSwallowing = Vector3.Zero;
            }
        }

        private void Interrupter2_OnInterruptableTarget(
            AIHeroClient unit,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            switch (unit.GetBuffCount(this.tahmPassive))
            {
                case 1:
                    if (TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchQ")
                            .Item("SAssembliesChampionsTahmKenchQInterrupt")
                            .GetValue<bool>()
                        && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchW")
                               .Item("SAssembliesChampionsTahmKenchWInterrupt")
                               .GetValue<bool>())
                    {
                        if (Orbwalking.InAutoAttackRange(unit) && CustomSpell.Q.LSIsReady() && (CustomSpell.W.LSIsReady()
                            && this.swallowedUnit == SwallowedUnit.None))
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
                            LeagueSharp.Common.Utility.DelayAction.Add(100, () => CustomSpell.Q.Cast(unit));
                            LeagueSharp.Common.Utility.DelayAction.Add(200, () => CustomSpell.W.CastOnUnit(unit));
                        }
                    }
                    break;

                case 2:
                    if ((CustomSpell.Q.LSIsReady() && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchQ")
                            .Item("SAssembliesChampionsTahmKenchQInterrupt")
                            .GetValue<bool>()) || (CustomSpell.W.LSIsReady() && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchW")
                               .Item("SAssembliesChampionsTahmKenchWInterrupt")
                               .GetValue<bool>() && this.swallowedUnit == SwallowedUnit.None))
                    {
                        if (Orbwalking.InAutoAttackRange(unit))
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
                            if (CustomSpell.Q.LSIsReady())
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(100, () => CustomSpell.Q.Cast(unit));
                            }
                            else if (CustomSpell.W.LSIsReady())
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(100, () => CustomSpell.W.CastOnUnit(unit));
                            }
                        }
                        else if (CustomSpell.Q.LSIsReady() && CustomSpell.W.LSIsReady())
                        {
                            CustomSpell.Q.Cast(unit);
                            LeagueSharp.Common.Utility.DelayAction.Add(100, () => CustomSpell.W.CastOnUnit(unit));
                        }
                    }
                    break;

                case 3:
                    if ((CustomSpell.Q.LSIsReady() && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchQ")
                            .Item("SAssembliesChampionsTahmKenchQInterrupt")
                            .GetValue<bool>()) || (CustomSpell.W.LSIsReady() && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchW")
                               .Item("SAssembliesChampionsTahmKenchWInterrupt")
                               .GetValue<bool>() && this.swallowedUnit == SwallowedUnit.None))
                    {
                        if (CustomSpell.Q.LSIsReady())
                        {
                            CustomSpell.Q.Cast(unit);
                        }
                        else if (CustomSpell.W.LSIsReady())
                        {
                            CustomSpell.W.CastOnUnit(unit);
                        }
                    }
                    break;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive() || ObjectManager.Player.IsDead)
                return;

            this.Killsteal();
            this.Shield();
            this.SaveAlly();
            this.TrollMode();

            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    this.Combo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    this.Harass();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    this.LaneClear();
                    this.JungleClear();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    this.LastHit();
                    break;
            }
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (!IsActive())
                return;

            if (TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchQ")
                    .Item("SAssembliesChampionsTahmKenchQDraw")
                    .GetValue<bool>())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, CustomSpell.Q.Range, Color.Red);
            }
            if (TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchW")
                    .Item("SAssembliesChampionsTahmKenchWDraw")
                    .GetValue<bool>())
            {
                if (this.swallowedUnit != SwallowedUnit.Minion)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, CustomSpell.W.Range, Color.BlueViolet);
                }
                else
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, CustomSpell.W2.Range, Color.BlueViolet);
                }
            }
            if (TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchR")
                    .Item("SAssembliesChampionsTahmKenchRDraw")
                    .GetValue<bool>())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, CustomSpell.R.Range, Color.CadetBlue);
            }

            if (TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchW")
                    .Item("SAssembliesChampionsTahmKenchWDrawMax")
                    .GetValue<bool>())
            {
                if (this.swallowedUnit == SwallowedUnit.Enemy)
                {
                    BuffInstance buff = ObjectManager.Player.GetBuff(this.tahmEatPassive.ToLower());
                    if (buff != null)
                    {
                        float time = buff.EndTime - buff.StartTime;
                        float xPos = this.lastPosBeforeSwallowing.X + ((time) * ObjectManager.Player.MoveSpeed);
                        float radius = Math.Abs(this.lastPosBeforeSwallowing.X - xPos);
                        Console.WriteLine(radius);
                        Render.Circle.DrawCircle(this.lastPosBeforeSwallowing, radius, Color.Aqua);
                    }
                }
            }
        }

        private void Combo()
        {
            orbwalker.SetOrbwalkingPoint(Vector3.Zero);

            var target = TargetSelector.GetTarget(CustomSpell.Q.Range, TargetSelector.DamageType.Magical);
            var closestMinion = MinionManager.GetMinions(250, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault();

            if (target != null)
            {
                var buffCount = target.GetBuffCount(this.tahmPassive);
                switch (buffCount)
                {
                    case 3:
                        if (CustomSpell.W.LSIsReady()
                            && this.swallowedUnit == SwallowedUnit.None
                            && target.LSDistance(ObjectManager.Player) <= CustomSpell.W.Range
                            && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchCombo")
                                   .Item("SAssembliesChampionsTahmKenchComboW")
                                   .GetValue<bool>())
                        {
                            CustomSpell.W.CastOnUnit(target);
                        }
                        else if (CustomSpell.Q.LSIsReady() 
                            && target.LSDistance(ObjectManager.Player) <= CustomSpell.Q.Range
                                 && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchCombo")
                                        .Item("SAssembliesChampionsTahmKenchComboQ")
                                        .GetValue<bool>())
                        {
                            CustomSpell.Q.Cast(target);
                        }
                        break;
                    default:
                        if (CustomSpell.W.LSIsReady() && !Orbwalking.InAutoAttackRange(target)
                            && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchCombo")
                                .Item("SAssembliesChampionsTahmKenchComboW")
                                .GetValue<bool>())
                        {
                            if (buffCount >= TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchCombo")
                                    .Item("SAssembliesChampionsTahmKenchComboWMax")
                                    .GetValue<Slider>()
                                    .Value)
                            {
                                break;
                            }
                            if (this.swallowedUnit == SwallowedUnit.None && closestMinion != null)
                            {
                                CustomSpell.W.CastOnUnit(closestMinion);
                            }
                            else if (this.swallowedUnit == SwallowedUnit.Minion)
                            {
                                CustomSpell.W2.CastIfHitchanceEquals(target, HitChance.High);
                            }
                        }
                        if (CustomSpell.Q.LSIsReady()
                            && target.LSDistance(ObjectManager.Player) <= CustomSpell.Q.Range
                                 && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchCombo")
                                        .Item("SAssembliesChampionsTahmKenchComboQ")
                                        .GetValue<bool>())
                        {
                            CustomSpell.Q.Cast(target);
                        }
                        break;
                }
            }

            if (this.swallowedUnit == SwallowedUnit.Enemy
                    && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchW")
                        .Item("SAssembliesChampionsTahmKenchWAutoMoveToAlly")
                        .GetValue<bool>())
            {
                var hero = HeroManager.Allies
                    .OrderBy(x => ObjectManager.Player.LSDistance(x.Position))
                    .FirstOrDefault(x => !x.IsMe && ObjectManager.Player.LSDistance(x.Position) < 2000);
                var turret =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .OrderBy(x => ObjectManager.Player.LSDistance(x.Position))
                        .FirstOrDefault(x => ObjectManager.Player.LSDistance(x.Position) < 2000
                        && x.IsAlly);
                if (hero != null && turret != null)
                {
                    if (ObjectManager.Player.LSDistance(hero) < ObjectManager.Player.LSDistance(turret))
                    {
                        orbwalker.SetOrbwalkingPoint(hero.ServerPosition);
                    }
                    else if (ObjectManager.Player.LSDistance(hero) > ObjectManager.Player.LSDistance(turret))
                    {
                        orbwalker.SetOrbwalkingPoint(turret.ServerPosition);
                    }
                }
                else if (hero != null)
                {
                    orbwalker.SetOrbwalkingPoint(hero.ServerPosition);
                }
                else if (turret != null)
                {
                    orbwalker.SetOrbwalkingPoint(turret.ServerPosition);
                }
            }
        }

        private void Harass()
        {
            if (ObjectManager.Player.ManaPercent
                < TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchHarass")
                      .Item("SAssembliesChampionsTahmKenchHarassMinMana")
                      .GetValue<Slider>()
                      .Value)
            {
                return; 
            }

            if (CustomSpell.Q.LSIsReady() &&
                TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchHarass")
                    .Item("SAssembliesChampionsTahmKenchHarassQ")
                    .GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(CustomSpell.Q.Range, TargetSelector.DamageType.Magical);

                if (target != null)
                {
                    CustomSpell.CastSpell(CustomSpell.Q, target, HitChance.High);
                }
            }

            var closestMinion = MinionManager.GetMinions(250, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault();
            if (CustomSpell.W.LSIsReady()
                && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchHarass")
                                .Item("SAssembliesChampionsTahmKenchHarassW")
                                .GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(CustomSpell.W2.Range, TargetSelector.DamageType.Magical);
                if (target != null)
                {
                    if (this.swallowedUnit == SwallowedUnit.None && closestMinion != null)
                    {
                        CustomSpell.W.CastOnUnit(closestMinion);
                    }
                    else if (this.swallowedUnit == SwallowedUnit.Minion)
                    {
                        CustomSpell.W2.CastIfHitchanceEquals(target, HitChance.High);
                    }
                }
            }

        }

        private void LastHit()
        {
            if (ObjectManager.Player.ManaPercent
                < TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchFarm")
                      .SubMenu("SAssembliesChampionsTahmKenchFarmLasthit")
                      .Item("SAssembliesChampionsTahmKenchFarmLasthitMinMana")
                      .GetValue<Slider>()
                      .Value)
            {
                return;
            }

            var minion = MinionManager.GetMinions(CustomSpell.Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .FirstOrDefault(target => ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.Q) >= target.Health);

            if (minion != null)
            {
                if (CustomSpell.Q.LSIsReady() && minion.LSDistance(ObjectManager.Player) >= ObjectManager.Player.AttackRange &&
                TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchFarm")
                    .SubMenu("SAssembliesChampionsTahmKenchFarmLasthit")
                    .Item("SAssembliesChampionsTahmKenchFarmLasthitQ")
                    .GetValue<bool>())
                {
                    CustomSpell.CastSpell(CustomSpell.Q, minion, HitChance.High);
                }
            }
        }

        private void LaneClear()
        {
            if (TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchFarm")
                    .SubMenu("SAssembliesChampionsTahmKenchFarmLane")
                    .Item("SAssembliesChampionsTahmKenchFarmLaneQ")
                    .GetValue<bool>() && CustomSpell.Q.LSIsReady() &&
                    ObjectManager.Player.ManaPercent
                    >= TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchFarm")
                      .SubMenu("SAssembliesChampionsTahmKenchFarmLane")
                      .Item("SAssembliesChampionsTahmKenchFarmLaneMinMana")
                      .GetValue<Slider>()
                      .Value)
            {
                var minion = MinionManager.GetMinions(CustomSpell.Q.Range)
                    .FirstOrDefault();

                if (minion != null)
                {
                    if (CustomSpell.Q.LSIsReady())
                    {
                        CustomSpell.CastSpell(CustomSpell.Q, minion, HitChance.High);
                    }
                }
            }

            if (TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchFarm")
                    .SubMenu("SAssembliesChampionsTahmKenchFarmLane")
                    .Item("SAssembliesChampionsTahmKenchFarmLaneW")
                    .GetValue<bool>() && CustomSpell.W.LSIsReady())
            {

                if (ObjectManager.Player.ManaPercent
                    < TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchFarm")
                          .SubMenu("SAssembliesChampionsTahmKenchFarmLane")
                          .Item("SAssembliesChampionsTahmKenchFarmLaneMinMana")
                          .GetValue<Slider>()
                          .Value && this.swallowedUnit != SwallowedUnit.Minion)
                {
                    return;
                }

                var minion = MinionManager.GetMinions(CustomSpell.W.Range).FirstOrDefault();
                var minions = MinionManager.GetMinions(CustomSpell.W2.Range);

                if (minion != null)
                {
                    if (this.swallowedUnit == SwallowedUnit.None && minion != null)
                    {
                        CustomSpell.W.CastOnUnit(minion);
                    }
                    else if (this.swallowedUnit == SwallowedUnit.Minion && minions.Count > 0)
                    {
                        CustomSpell.W2.Cast(CustomSpell.W2.GetCircularFarmLocation(minions).Position);
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchFarm")
                    .SubMenu("SAssembliesChampionsTahmKenchFarmJungle")
                    .Item("SAssembliesChampionsTahmKenchFarmJungleQ")
                    .GetValue<bool>() && CustomSpell.Q.LSIsReady() &&
                    ObjectManager.Player.ManaPercent
                    >= TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchFarm")
                      .SubMenu("SAssembliesChampionsTahmKenchFarmJungle")
                      .Item("SAssembliesChampionsTahmKenchFarmJungleMinMana")
                      .GetValue<Slider>()
                      .Value)
            {
                var minion = MinionManager.GetMinions(CustomSpell.Q.Range, MinionTypes.All, MinionTeam.Neutral)
                    .FirstOrDefault();

                if (minion != null)
                {
                    if (CustomSpell.Q.LSIsReady())
                    {
                        CustomSpell.CastSpell(CustomSpell.Q, minion, HitChance.High);
                    }
                }
            }

            if (TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchFarm")
                    .SubMenu("SAssembliesChampionsTahmKenchFarmJungle")
                    .Item("SAssembliesChampionsTahmKenchFarmJungleW")
                    .GetValue<bool>() && CustomSpell.W.LSIsReady())
            {

                if (ObjectManager.Player.ManaPercent
                    < TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchFarm")
                          .SubMenu("SAssembliesChampionsTahmKenchFarmJungle")
                          .Item("SAssembliesChampionsTahmKenchFarmJungleMinMana")
                          .GetValue<Slider>()
                          .Value && this.swallowedUnit != SwallowedUnit.Minion)
                {
                    return;
                }

                var minion = MinionManager.GetMinions(CustomSpell.W.Range, MinionTypes.All, MinionTeam.Neutral).FirstOrDefault();
                var minions = MinionManager.GetMinions(CustomSpell.W2.Range, MinionTypes.All, MinionTeam.Neutral);

                if (minion != null)
                {
                    if (this.swallowedUnit == SwallowedUnit.None && minion != null)
                    {
                        CustomSpell.W.CastOnUnit(minion);
                    }
                    else if (this.swallowedUnit == SwallowedUnit.Minion && minions.Count > 0)
                    {
                        CustomSpell.W2.Cast(CustomSpell.W2.GetCircularFarmLocation(minions).Position);
                    }
                }
            }
        }

        private void Killsteal()
        {
            if (TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchQ")
                    .Item("SAssembliesChampionsTahmKenchQKillsteal")
                    .GetValue<bool>() && CustomSpell.Q.LSIsReady())
            {
                var target = HeroManager.Enemies.FirstOrDefault(enemy => enemy.LSIsValidTarget(CustomSpell.Q.Range) && 
                    enemy.Health < ObjectManager.Player.LSGetSpellDamage(enemy, SpellSlot.Q));

                if (target != null)
                {
                    CustomSpell.Q.CastIfHitchanceEquals(target, HitChance.High);
                }
            }

            if (TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchW")
                    .Item("SAssembliesChampionsTahmKenchWKillsteal")
                    .GetValue<bool>() && CustomSpell.W.LSIsReady())
            {
                var target = HeroManager.Enemies.FirstOrDefault(enemy => enemy.LSIsValidTarget(CustomSpell.W.Range) &&
                    enemy.Health < ObjectManager.Player.LSGetSpellDamage(enemy, SpellSlot.W));

                if (target != null)
                {
                    if (target.GetBuffCount(tahmPassive) == 3)
                    {
                        CustomSpell.W.CastOnUnit(target);
                        CustomSpell.W.Cast(target);
                    }
                }
            }
        }

        private void Shield() //TODO:Improve for incoming dmg
        {
            if (TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchE")
                    .Item("SAssembliesChampionsTahmKenchEShield")
                    .GetValue<bool>() && 
                    ObjectManager.Player.HealthPercent <= 
                    TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchE")
                    .Item("SAssembliesChampionsTahmKenchEShieldPercent")
                    .GetValue<Slider>().Value && CustomSpell.E.LSIsReady())
            {
                CustomSpell.E.Cast();
            }
        }

        private void SaveAlly()
        {
            if (TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchW")
                    .Item("SAssembliesChampionsTahmKenchWAutoShieldAlly")
                    .GetValue<bool>() && this.swallowedUnit == SwallowedUnit.None
                    && CustomSpell.W.LSIsReady())
            {
                var target = HeroManager.Allies.FirstOrDefault(ally => !ally.IsMe && !ally.IsDead &&
                   ally.HealthPercent <= TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchW")
                    .Item("SAssembliesChampionsTahmKenchWAutoShieldAllyPercent")
                    .GetValue<Slider>().Value && ObjectManager.Player.LSDistance(ally) < 500);

                if (target != null)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target.ServerPosition);
                    CustomSpell.W.CastOnUnit(target);
                }
            }
        }

        private void TrollMode()
        {
            if (!TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchTroll")
                     .Item("SAssembliesChampionsTahmKenchTrollW")
                     .GetValue<KeyBind>().Active || !CustomSpell.W.LSIsReady())
            {
                return;
            }

            var hero = HeroManager.Enemies
                .OrderBy(x => ObjectManager.Player.LSDistance(x.Position))
                .FirstOrDefault(x => !x.IsMe && ObjectManager.Player.LSDistance(x.Position) < 2000);
            var turret =
                ObjectManager.Get<Obj_AI_Turret>()
                    .OrderBy(x => ObjectManager.Player.LSDistance(x.Position))
                    .FirstOrDefault(x => ObjectManager.Player.LSDistance(x.Position) < 2000
                    && x.IsEnemy);

            if (hero != null || turret != null)
            {
                var allyHero = HeroManager.Allies
                    .OrderBy(x => ObjectManager.Player.LSDistance(x.Position))
                    .FirstOrDefault(x => !x.IsMe && ObjectManager.Player.LSDistance(x.Position) < CustomSpell.W.Range + 200);

                if (allyHero != null && this.swallowedUnit == SwallowedUnit.None)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, allyHero.ServerPosition);
                    CustomSpell.W.CastOnUnit(allyHero);
                }

                if (this.swallowedUnit == SwallowedUnit.Ally)
                {
                    Obj_AI_Base target = null;
                    if (hero != null && turret != null
                    && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchTroll")
                        .Item("SAssembliesChampionsTahmKenchTrollWToEnemyHero")
                        .GetValue<bool>()
                        && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchTroll")
                        .Item("SAssembliesChampionsTahmKenchTrollWToEnemyTurret")
                        .GetValue<bool>())
                    {
                        if (ObjectManager.Player.LSDistance(hero) < ObjectManager.Player.LSDistance(turret))
                        {
                            target = hero;
                        }
                        else if (ObjectManager.Player.LSDistance(hero) > ObjectManager.Player.LSDistance(turret))
                        {
                            target = turret;
                        }
                    }
                    else if (hero != null && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchTroll")
                            .Item("SAssembliesChampionsTahmKenchTrollWToEnemyHero")
                            .GetValue<bool>())
                    {
                        target = hero;
                    }
                    else if (turret != null && TahmKenchChampion.GetSubMenu("SAssembliesChampionsTahmKenchTroll")
                            .Item("SAssembliesChampionsTahmKenchTrollWToEnemyTurret")
                            .GetValue<bool>())
                    {
                        target = turret;
                    }

                    if (target != null)
                    {
                        if (ObjectManager.Player.LSDistance(target) < CustomSpell.W2.Range)
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.Stop, ObjectManager.Player.ServerPosition);
                            CustomSpell.W.Cast(target.ServerPosition);
                        }
                        else
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target.ServerPosition);
                        }
                    }
                }
            }
        }
    }
}