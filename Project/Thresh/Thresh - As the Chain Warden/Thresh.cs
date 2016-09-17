using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using SharpDX.Direct3D9;
using Orbwalking = SebbyLib.Orbwalking;

using EloBuddy;
using LeagueSharp.Common;
namespace ThreshAsurvil
{
    class Thresh
    {

        public static AIHeroClient Player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static List<Vector3> MobList = new List<Vector3>();
        public static Obj_AI_Base QTarget = null;
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Font font;
        public static Obj_AI_Base DrawTarget;
        public static List<AIHeroClient> Qignored = new List<AIHeroClient>();

        public static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != "Thresh") return;



            LoadSpell();
            LoadMenu();

            Chat.Print(
                "魂锁典狱 - 锤石".ToHtml(Color.Orange, FontStlye.Bold)
                + "  "
                + "游走的孤魂野鬼啊，我其实是绿灯侠 ".ToHtml(Color.Yellow, FontStlye.Cite));

            //font = new Font(Drawing.Direct3DDevice,new FontDescription { FaceName = "微软雅黑", Height = 30 });

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnWndProc += Game_OnWndProc;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Config.Item("辅助模式").GetValue<bool>()
                && GetAdc(Config.Item("辅助模式距离").GetValue<Slider>().Value) != null
                && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed
                    || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit
                    || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Player.GetAutoAttackDamage(args.Target as Obj_AI_Base) * 1.5f > args.Target.Health))
            {
                args.Process = false;
            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == 'Q')
            {
                var Qtarget = Q.GetTarget(0, Qignored);
                if (Qtarget != null && SpellQ.GetState() == QState.ThreshQ)
                {
                    SpellQ.CastQ1(Qtarget);
                }
                else
                {
                    args.Process = false;
                }
            }
            if (args.Msg == 'W')
            {
                var FurthestAlly = GetFurthestAlly();
                if (FurthestAlly != null)
                {
                    W.Cast(Prediction.GetPrediction(FurthestAlly, W.Delay).CastPosition);
                }

            }
            if (args.Msg == 'E')
            {
                var Etarget = E.GetTarget();
                if (Etarget != null)
                {
                    ELogic(Etarget);
                }
                else
                {
                    args.Process = false;
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            #region 自动QE塔下敌人
            if (Config.Item("控制塔攻击的敌人").GetValue<bool>() && sender.IsAlly && sender is Obj_AI_Turret && args.Target.IsEnemy && args.Target.Type == GameObjectType.AIHeroClient)
            {
                var target = args.Target as AIHeroClient;
                var turret = sender as Obj_AI_Turret;

                if (turret.IsAlly && E.CanCast(target) && target.Distance(turret) < turret.AttackRange + E.Range)
                {
                    if (target.Distance(turret) < Player.Distance(turret))
                    {
                        E.Cast(target);
                    }
                    else
                    {
                        E.CastToReverse(target);
                    }
                }
                if (Player.Distance(turret) < turret.AttackRange && SpellQ.GetState() == QState.ThreshQ)
                {
                    SpellQ.CastQ1(target);
                }
            }
            #endregion

            #region 自动W
            if (!W.IsReady() || !sender.IsEnemy || !sender.IsValidTarget(1500))
                return;
            double value = 20 + (Player.Level * 20) + (0.4 * Player.FlatMagicDamageMod);

            foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && !ally.IsDead && Player.Distance(ally.ServerPosition) < W.Range + 200))
            {
                double dmg = 0;
                if (args.Target != null && args.Target.NetworkId == ally.NetworkId)
                {
                    dmg = dmg + sender.GetSpellDamage(ally, args.SData.Name);
                }
                else
                {
                    var castArea = ally.Distance(args.End) * (args.End - ally.ServerPosition).Normalized() + ally.ServerPosition;
                    if (castArea.Distance(ally.ServerPosition) < ally.BoundingRadius / 2)
                        dmg = dmg + sender.GetSpellDamage(ally, args.SData.Name);
                    else
                        continue;
                }

                if (dmg > 0)
                {
                    if (dmg > value)
                        W.Cast(ally.Position);
                    else if (Player.Health - dmg < Player.CountEnemiesInRange(700) * Player.Level * 20)
                        W.Cast(ally.Position);
                    else if (ally.Health - dmg < ally.Level * 10)
                        W.Cast(ally.Position);
                }
            }
            #endregion

        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if ((args.Slot == SpellSlot.E || args.Slot == SpellSlot.R) && sender.Owner.IsDashing())
            {
                args.Process = false;
            }

            if (Config.Item("Q不进敌塔").GetValue<bool>())
            {
                if (sender.Owner.IsMe && args.Slot == SpellSlot.Q
                    && SpellQ.GetState() == QState.threshqleap
                    && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    //if (QTarget.UnderTurret(true) || QTarget.InFountain())
                    //{
                    //	args.Process = false;
                    //}
                    var tower = QTarget.GetMostCloseTower();
                    if ((tower != null && QTarget.IsInTurret(tower) && tower.IsEnemy) || (QTarget.Type == GameObjectType.AIHeroClient && ((AIHeroClient)QTarget).InFountain()))
                    {
                        args.Process = false;
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            #region 设置Q到的目标
            if (QTarget == null || QTarget.IsValid || QTarget.IsDead)
            {
                foreach (var unit in ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsEnemy && o.Distance(Player) < Q.Range + 100))
                {
                    if (unit.HasBuff("ThreshQ"))
                    {
                        QTarget = unit;
                        break;
                    }
                    else
                    {
                        QTarget = null;
                    }
                }
            }
            #endregion

            Flee();

            AutoPushTower();

            AutoBox();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
            }
        }

        private static void Flee()
        {
            if (Config.Item("逃跑").GetValue<KeyBind>().Active)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                if (Config.Item("E推人").GetValue<bool>())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(e => !e.IsDead && !e.HasBuffOfType(BuffType.SpellShield)))
                    {
                        if (E.CanCast(enemy))
                        {
                            E.Cast(enemy);
                        }
                    }
                }
            }
        }

        private static void AutoBox()
        {

            var AutoBoxCount = Config.Item("大招人数").GetValue<Slider>().Value;
            var EnemiesCount = Config.Item("自动大招模式").GetValue<StringList>().SelectedIndex == 0
                ? Player.CountEnemiesInRangeDeley(R.Range, R.Delay - 0.1f)
                : Player.CountEnemiesInRange(R.Range);

            if (R.IsReady() && EnemiesCount >= AutoBoxCount)
            {
                R.Cast();
            }
        }

        private static void Combo()
        {
            var target = GetTarget();
            if (target != null && target.IsValid)
            {
                ELogic(target);

                //Q2逻辑
                if (SpellQ.GetState() == QState.threshqleap
                    && QTarget.Position.CountEnemiesInRange(700) - Player.Position.CountEnemiesInRange(700) <= Config.Item("人数比").GetValue<Slider>().Value)
                {
                    SpellQ.CastQ2();
                }

                //Q1逻辑
                if (!E.IsInRange(target) && SpellQ.GetState() == QState.ThreshQ)
                {
                    SpellQ.CastQ1(target);
                }

                if (SpellQ.GetState() == QState.threshqleap)
                {
                    //W拉最远队友
                    var FurthestAlly = GetFurthestAlly();
                    if (FurthestAlly != null)
                    {
                        W.Cast(Prediction.GetPrediction(FurthestAlly, W.Delay).CastPosition);
                    }
                }
            }
        }

        private static void ELogic(AIHeroClient target)
        {
            if (!E.CanCast(target) || target.HasBuffOfType(BuffType.SpellShield)) return;

            var tower = target.GetMostCloseTower();
            if (tower != null && tower.IsAlly && E.CanCast(target) && target.Distance(tower) < tower.AttackRange + E.Range)
            {
                if (target.Distance(tower) < Player.Distance(tower))
                {
                    E.Cast(target);
                }
                else
                {
                    E.CastToReverse(target);
                }
            }

            var adc = GetAdc();
            if (adc != null)
            {
                if (target.IsFleeing(Player))
                {
                    E.Cast(target);
                }
                else if (target.IsHunting(Player))
                {
                    E.CastToReverse(target);
                }
            }

            //if (target.HealthPercent>Player.HealthPercent)
            //{
            //	E.Cast(target);
            //}
            //else
            //{
            //	E.CastToReverse(target);
            //}
            if (target.Distance(Player) > E.Range / 2 || Player.HealthPercent < 50)
            {
                E.CastToReverse(target);
            }

        }

        private static AIHeroClient GetFurthestAlly()
        {
            AIHeroClient FurthestAlly = null;
            foreach (var ally in HeroManager.Allies.Where(a => a.Distance(Player) > W.Range / 2 + 100 && a.Distance(Player) < W.Range + 100 && !a.IsDead && !a.IsMe))
            {
                if (FurthestAlly == null)
                {
                    FurthestAlly = ally;
                }
                else if (FurthestAlly != null && Player.Distance(ally) > Player.Distance(FurthestAlly))
                {
                    FurthestAlly = ally;
                }
            }
            return FurthestAlly;
        }

        private static void LaneClear()
        {
            if (E.IsReady() && Player.Mana > Q.ManaCost + W.ManaCost + E.ManaCost + R.ManaCost)
            {
                var minions = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);
                var Efarm = Q.GetLineFarmLocation(minions, E.Width);
                if (Efarm.MinionsHit >= 3)
                {
                    E.Cast(Efarm.Position);
                }
            }
        }

        private static void Mixed()
        {
            var target = GetTarget();
            if (target != null && target.IsValid)
            {
                ELogic(target);
            }
        }

        private static void AutoPushTower()
        {
        }

        private static AIHeroClient GetTarget()
        {


            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical, false, Qignored);
            var adc = GetAdc();
            if (adc != null && Config.Item("辅助目标").GetValue<bool>())
            {
            }
            DrawTarget = target;
            return target;
        }

        private static Obj_AI_Base GetAdc(float range = 1075)
        {
            Obj_AI_Base Adc = null;
            foreach (var ally in HeroManager.Allies.Where(a => !a.IsMe && !a.IsDead))
            {
                if (Adc == null)
                {
                    Adc = ally;
                }
                else if (Adc.TotalAttackDamage < ally.TotalAttackDamage)
                {
                    Adc = ally;
                }
            }
            return Adc;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            #region 技能范围
            var QShow = Config.Item("显示Q").GetValue<Circle>();
            if (QShow.Active)
            {
                if (Config.Item("技能可用才显示").GetValue<bool>())
                {
                    if (Q.IsReady())
                    {
                        Render.Circle.DrawCircle(Player.Position, Q.Range, QShow.Color, 2);
                    }

                }
                else
                {
                    Render.Circle.DrawCircle(Player.Position, Q.Range, QShow.Color, 2);
                }
            }

            var WShow = Config.Item("显示W").GetValue<Circle>();
            if (WShow.Active)
            {
                if (Config.Item("技能可用才显示").GetValue<bool>())
                {
                    if (W.IsReady())
                        Render.Circle.DrawCircle(Player.Position, W.Range, WShow.Color, 2);
                }
                else
                {
                    Render.Circle.DrawCircle(Player.Position, W.Range, WShow.Color, 2);
                }
            }

            var EShow = Config.Item("显示E").GetValue<Circle>();
            if (EShow.Active)
            {
                if (Config.Item("技能可用才显示").GetValue<bool>())
                {
                    if (E.IsReady())
                        Render.Circle.DrawCircle(Player.Position, E.Range, EShow.Color, 2);
                }
                else
                {
                    Render.Circle.DrawCircle(Player.Position, E.Range, EShow.Color, 2);
                }
            }

            var RShow = Config.Item("显示R").GetValue<Circle>();
            if (RShow.Active)
            {
                if (Config.Item("技能可用才显示").GetValue<bool>())
                {
                    if (R.IsReady())
                        Render.Circle.DrawCircle(Player.Position, R.Range, RShow.Color, 2);
                }
                else
                {
                    Render.Circle.DrawCircle(Player.Position, R.Range, RShow.Color, 2);
                }
            }
            #endregion

            if (Config.Item("辅助目标").GetValue<bool>())
            {
                var target = DrawTarget;
                if (target != null)
                {
                    Render.Circle.DrawCircle(target.Position, target.BoundingRadius, Color.Red, 5);
                }
            }
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {

            if (sender.IsEnemy && Player.Distance(args.EndPos) > Player.Distance(args.StartPos))
            {
                if (E.IsInRange(args.StartPos))
                {
                    Utill.Debug("DEBUG:Unit_OnDash E");
                    E.Cast(sender);
                }

                if (Config.Item("位移Q").GetValue<bool>() && SpellQ.GetState() == QState.ThreshQ && Q.IsInRange(args.EndPos) && !E.IsInRange(args.EndPos) && Math.Abs(args.Duration - args.EndPos.Distance(sender) / Q.Speed * 1000) < 150)
                {
                    List<Vector2> to = new List<Vector2>();
                    to.Add(args.EndPos);
                    var QCollision = Q.GetCollision(Player.Position.To2D(), to);
                    if (QCollision == null || QCollision.Count == 0 || QCollision.All(a => !a.IsMinion))
                    {
                        if (Q.Cast(args.EndPos))
                        {
                            Utill.Debug("DEBUG:Unit_OnDash Q");
                            return;
                        }
                    }
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.ChampionName == "MasterYi" && gapcloser.Slot == SpellSlot.Q)
            {
                return;
            }

            if (E.CanCast(gapcloser.Sender) && E.CastToReverse(gapcloser.Sender))
            {
                Utill.Debug("DEBUG:AntiGapcloser E");
                return;
            }
            else if (Q.CanCast(gapcloser.Sender) && SpellQ.GetState() == QState.ThreshQ)
            {
                if (gapcloser.Sender.ChampionName == "JarvanIV" && gapcloser.Slot == SpellSlot.Q)
                {
                    return;
                }
                Utill.Debug("DEBUG:AntiGapcloser Q");
                SpellQ.CastQ1(gapcloser.Sender);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {

            if (E.CanCast(sender))
            {
                if (Player.CountAlliesInRange(E.Range + 50) < sender.CountAlliesInRange(E.Range + 50))
                {
                    E.Cast(sender);
                }
                else
                {
                    E.CastToReverse(sender);
                }
            }
            if (Q.CanCast(sender) && SpellQ.GetState() == QState.ThreshQ)
            {
                Q.Cast(sender);
            }

        }

        private static void LoadSpell()
        {
            Q = new Spell(SpellSlot.Q, 1075);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 450);
            R = new Spell(SpellSlot.R, 430);

            Q.SetSkillshot(0.5f, 80, 1900f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 100, float.MaxValue, false, SkillshotType.SkillshotLine);
        }

        private static void LoadMenu()
        {
            Config = new Menu("Thresh As the Chain Warden", "锤石As", true);
            Config.AddToMainMenu();
            var OrbMenu = new Menu("Orbwalker", "走砍设置");
            Orbwalker = new Orbwalking.Orbwalker(OrbMenu);
            Config.AddSubMenu(OrbMenu);

            var SpellConfig = Config.AddSubMenu(new Menu("Spell Settings", "技能设置"));
            SpellConfig.AddItem(new MenuItem("位移Q", "Auto Q Dash Enemy").SetValue(true));
            SpellConfig.AddItem(new MenuItem("不用Q2", "Don't Auto Q2").SetValue(false));
            SpellConfig.AddItem(new MenuItem("人数比", "Don't Q2 if Enemies > allies").SetValue(new Slider(1, 0, 5)));
            var QListConfig = SpellConfig.AddSubMenu(new Menu("Q List", "Q名单"));
            QListConfig.AddItem(new MenuItem("提示", "if you find some gays seems use L# too,Dont Q him"));
            foreach (var enemy in HeroManager.Enemies)
            {
                QListConfig.AddItem(new MenuItem("QList" + enemy.NetworkId, enemy.ChampionName + "[ " + enemy.Name.ToGBK() + " ]").SetValue(true)).ValueChanged += QListValue_Changed; ;
            }

            var FleeConfig = Config.AddSubMenu(new Menu("Flee Settings", "逃跑设置"));
            FleeConfig.AddItem(new MenuItem("逃跑", "Flee").SetValue(new KeyBind('S', KeyBindType.Press)));
            FleeConfig.AddItem(new MenuItem("E推人", "Auto E push").SetValue(true));
            //FleeConfig.AddItem(new MenuItem("Q野怪", "Auto Q Jungle [TEST]").SetValue(true));

            var PredictConfig = Config.AddSubMenu(new Menu("Predict Settings", "预判设置"));
            PredictConfig.AddItem(new MenuItem("预判模式", "Prediction Mode").SetValue(new StringList(new[] { "Common", "OKTW", "S Prediction" }, 1)));
            PredictConfig.AddItem(new MenuItem("命中率", "HitChance").SetValue(new StringList(new[] { "Very High", "High", "Medium" })));
            SPrediction.Prediction.Initialize(PredictConfig);

            var BoxConfig = Config.AddSubMenu(new Menu("Box Settings", "大招设置"));
            BoxConfig.AddItem(new MenuItem("大招人数", "Box Count").SetValue(new Slider(2, 1, 6)));
            BoxConfig.AddItem(new MenuItem("自动大招模式", "Box Mode").SetValue(new StringList(new[] { "Prediction", "Now" })));

            var SupportConfig = Config.AddSubMenu(new Menu("Support Mode", "辅助模式"));
            SupportConfig.AddItem(new MenuItem("辅助模式", "Support Mode").SetValue(true));
            SupportConfig.AddItem(new MenuItem("辅助模式距离", "Support Mode Range").SetValue(new Slider((int)Player.AttackRange + 200, (int)Player.AttackRange, 2000)));
            SupportConfig.AddItem(new MenuItem("辅助目标", "Attack ADC's Target [TEST]").SetValue(false));

            var DrawConfig = Config.AddSubMenu(new Menu("Drawing Settings", "显示设置"));
            DrawConfig.AddItem(new MenuItem("技能可用才显示", "Draw when skill is ready").SetValue(true));
            DrawConfig.AddItem(new MenuItem("显示Q", "Draw Q Range").SetValue(new Circle(false, Color.YellowGreen)));
            DrawConfig.AddItem(new MenuItem("显示W", "Draw W Range").SetValue(new Circle(false, Color.Yellow)));
            DrawConfig.AddItem(new MenuItem("显示E", "Draw E Range").SetValue(new Circle(false, Color.GreenYellow)));
            DrawConfig.AddItem(new MenuItem("显示R", "Draw R Range").SetValue(new Circle(false, Color.LightGreen)));
            DrawConfig.AddItem(new MenuItem("标识目标", "Draw Target").SetValue(new Circle(false, Color.Red)));

            var SmartKeyConfig = Config.AddSubMenu(new Menu("Smart Cast", "智能施法"));
            SmartKeyConfig.AddItem(new MenuItem("智能施法标签", "Enable Follow Options,Prss Q/W/E Auto Cast Spell"));
            SmartKeyConfig.AddItem(new MenuItem("智能Q", "Smart Cast Q").SetValue(true));
            SmartKeyConfig.AddItem(new MenuItem("智能W", "Smart Cast W").SetValue(true));
            SmartKeyConfig.AddItem(new MenuItem("智能E", "Smart Cast E").SetValue(true));

            var TowerConfig = Config.AddSubMenu(new Menu("Turret Settings", "防御塔设置"));
            TowerConfig.AddItem(new MenuItem("控制塔攻击的敌人", "Q/E ally Turret’s target").SetValue(true));
            TowerConfig.AddItem(new MenuItem("拉敌人进塔", "Q/E target into ally turret").SetValue(true));
            TowerConfig.AddItem(new MenuItem("Q不进敌塔", "Don't Q2 in enemy turret").SetValue(true));

            var MultiLanguageConfig = Config.AddSubMenu(new Menu("MultiLanguage Settings", "语言选择"));
            MultiLanguageConfig.AddItem(new MenuItem("选择语言", "Selecte Language").SetValue(new StringList(new[] { "English", "中文" }))).ValueChanged += MultiLanguage_ValueChanged;

            Config.AddItem(new MenuItem("调试", "调试").SetValue(false));

            ChangeLanguage(MultiLanguageConfig.Item("选择语言").GetValue<StringList>().SelectedIndex);
        }

        private static void QListValue_Changed(object sender, OnValueChangeEventArgs e)
        {
            var menuItem = sender as MenuItem;
            foreach (var enemy in HeroManager.Enemies)
            {
                if (menuItem.Name == "QList" + enemy.NetworkId)
                {
                    if (e.GetNewValue<bool>())
                    {
                        Qignored.Remove(enemy);
                    }
                    else
                    {
                        Qignored.Add(enemy);
                    }
                    Console.WriteLine(Qignored.Count);
                    break;
                }
            }
        }

        private static void ChangeLanguage(int SelectedIndex)
        {
            List<Dictionary<string, string>> Languages = new List<Dictionary<string, string>> {
                MultiLanguage.English,
                MultiLanguage.Chinese
            };
            var Language = Languages[SelectedIndex];

            List<object> menus = GetSubMenus(Config);

            foreach (var item in menus)
            {

                if (item is Menu)
                {
                    var m = item as Menu;
                    var DisplayName = Language.Find(l => l.Key == m.Name).Value;
                    if (!string.IsNullOrEmpty(DisplayName))
                    {
                        m.DisplayName = DisplayName;
                    }
                }
                else
                {
                    var m = item as MenuItem;
                    var DisplayName = Language.Find(l => l.Key == m.Name).Value;
                    if (!string.IsNullOrEmpty(DisplayName))
                    {
                        m.DisplayName = DisplayName;
                    }
                }
            }
        }

        private static List<object> GetSubMenus(Menu menu)
        {
            List<object> AllMenus = new List<object>();
            AllMenus.Add(menu);
            foreach (var item in menu.Items)
            {
                AllMenus.Add(item);
            }
            foreach (var item in menu.Children)
            {
                AllMenus.AddRange(GetSubMenus(item));
            }
            return AllMenus;
        }

        private static void MultiLanguage_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            ChangeLanguage(e.GetNewValue<StringList>().SelectedIndex);
        }

        private static void TestQDraw()
        {
        }
    }
}
