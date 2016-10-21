using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Plugings
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using System;
    using System.Linq;
    using static Common.Manager;

    public class TwistedFate
    {
        private static Spell Q;
        public static Spell W;
        private static Spell R;
        private static SpellSlot Flash;
        private static HpBarDraw HpBarDraw = new HpBarDraw();
        public static Random Random = new Random();

        private static Menu Menu => Program.Menu;
        private static AIHeroClient Me => Program.Me;

        public static void Init()
        {
            Flash = Me.GetSpellSlot("SummonerFlash");
            Q = new Spell(SpellSlot.Q, 1450f);
            W = new Spell(SpellSlot.W, 1000f);
            R = new Spell(SpellSlot.R, 5500f);

            Q.SetSkillshot(0.25f, 40f, 1000, false, SkillshotType.SkillshotLine);

            var PickCardMenu = Menu.Add(new Menu("TwistedFate_Pick", "Pick A Card"));
            {
                PickCardMenu.Add(new MenuKeyBind("SelectBlue", "Pick A Blue Card", System.Windows.Forms.Keys.E, KeyBindType.Press));
                PickCardMenu.Add(new MenuKeyBind("SelectYellow", "Pick A Yellow Card", System.Windows.Forms.Keys.W, KeyBindType.Press));
                PickCardMenu.Add(new MenuKeyBind("SelectRed", "Pick A Red Card", System.Windows.Forms.Keys.T, KeyBindType.Press));
            }

            var ComboMenu = Menu.Add(new Menu("TwistedFate_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q"));
                ComboMenu.Add(new MenuBool("WRed", "Use W|Pick A Red Card", true));
                ComboMenu.Add(new MenuBool("WMP", "Use W|If Low Mp Pick Blue Card", true));
                ComboMenu.Add(new MenuBool("WBlue", "Use W|If Target Can Kill Pick Blue Card", true));
                ComboMenu.Add(new MenuBool("SaveMana", "Save Mana to Cast W", true));
            }

            var HarassMenu = Menu.Add(new Menu("TwistedFate_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuBool("W", "Use W|Only Pick A Blue Card", true));
                HarassMenu.Add(new MenuSlider("Mana", "When Player ManaPercent >= %", 40));
            }

            var LaneClearMenu = Menu.Add(new Menu("TwistedFate_LaneClear", "LaneClear"));
            {
                LaneClearMenu.Add(new MenuBool("Q", "Use Q", true));
                LaneClearMenu.Add(new MenuSlider("QMin", "Use Q|Min Hit Minions Count >= ", 3, 1, 5));
                LaneClearMenu.Add(new MenuBool("W", "Use W|Smart Pick Card", true));
                LaneClearMenu.Add(new MenuSlider("WPickRed", "Use W|Pick Red Card Min ManaPercent >= %", 70));
                LaneClearMenu.Add(new MenuSlider("Mana", "When Player ManaPercent >= %", 70));
            }

            var JungleClearMenu = Menu.Add(new Menu("TwistedFate_JungleClear", "JungleClear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("W", "Use W", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "When Player ManaPercent >= %", 20));
            }

            var KillStealMenu = Menu.Add(new Menu("TwistedFate_KillSteal", "KillSteal"));
            {
                KillStealMenu.Add(new MenuBool("Q", "Use Q", true));
            }

            var AutoMenu = Menu.Add(new Menu("TwistedFate_Auto", "Auto"));
            {
                AutoMenu.Add(new MenuBool("DebuffQ", "Use Q|Only Enemy Can't Move", true));
                AutoMenu.Add(new MenuBool("AntiW", "Use W|Anti Gapcloser", true));
                AutoMenu.Add(new MenuBool("InterW", "Use W|Interrupt Spell", true));
            }

            var HumanizerMenu = Menu.Add(new Menu("TwistedFate_Humanizer", "Humanizer Pick"));
            {
                HumanizerMenu.Add(new MenuBool("EnableHumanizer", "Enable", false));
                HumanizerMenu.Add(new MenuSlider("MinHumanizer", "Min Humanizer Pick Time(ms)", 750, 500, 1500));
                HumanizerMenu.Add(new MenuSlider("MaxHumanizer", "Max Humanizer Pick Time(ms)", 1500, 1500, 3500));
                HumanizerMenu.Add(new MenuSliderButton("LowHp", "If Player Hp <= % Disable Humanizer!", 30, 0, 100, true));
            }

            var MiscMenu = Menu.Add(new Menu("TwistedFate_Misc", "Misc"));
            {
                MiscMenu.Add(new MenuBool("ADMode", "AD Mode", false));
                MiscMenu.Add(new MenuBool("ComboDisableAA", "Disable Auto Attack |When Picking Card", true));
                MiscMenu.Add(new MenuBool("AutoYellow", "Auto Select Gold Card |When Ult Active", true));
            }

            var DrawMenu = Menu.Add(new Menu("TwistedFate_DrawMenu", "Drawing"));
            {
                DrawMenu.Add(new MenuBool("DrawQ", "Q Range"));
                DrawMenu.Add(new MenuBool("DrawW", "Beautiful W Draw", true));
                DrawMenu.Add(new MenuBool("DrawR", "R Range"));
                DrawMenu.Add(new MenuBool("DrawRMin", "MinMap&Map R Range"));
                DrawMenu.Add(new MenuBool("DrawAF", "AA + Flash Range"));
                DrawMenu.Add(new MenuBool("DrawComboDamage", "Draw Combo Damage", true));
            }

            Menu.Add(new MenuKeyBind("TwistedFate_QKey", "Cast Q Key (Press)", System.Windows.Forms.Keys.D3, KeyBindType.Press));

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            Variables.Orbwalker.OnAction += OnAction;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Events.OnGapCloser += OnGapCloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }
             
            PickACard();

            if (InCombo)
            {
                ComboLogic();
            }

            if (InHarass)
            {
                HarassLogic();
            }

            if (InClear)
            {
                LaneClearLogic();
                JungleClearLogic();
            }

            Auto();
        }

        private static void PickACard()
        {
            if (Menu["TwistedFate_Pick"]["SelectBlue"].GetValue<MenuKeyBind>().Active)
            {
                Card.ToSelect(Cards.Blue);
            }

            if (Menu["TwistedFate_Pick"]["SelectYellow"].GetValue<MenuKeyBind>().Active)
            {
                Card.ToSelect(Cards.Yellow);
            }

            if (Menu["TwistedFate_Pick"]["SelectRed"].GetValue<MenuKeyBind>().Active)
            {
                Card.ToSelect(Cards.Red);
            }
        }

        private static void ComboLogic()
        {
            var target = Variables.TargetSelector.GetTarget(Q.Range, Q.DamageType);

            if (CheckTarget(target))
            {
                if (Menu["TwistedFate_Combo"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (Menu["TwistedFate_Combo"]["SaveMana"] && Me.Mana > Q.Instance.SData.Mana + W.Instance.SData.Mana)
                    {
                        Q.Cast(target, false, true);
                    }
                    else
                    {
                        Q.Cast(target, false, true);
                    }
                }

                if (W.IsReady() && target.IsValidTarget(W.Range))
                {
                    if (Menu["TwistedFate_Combo"]["WRed"] && target.CountAllyHeroesInRange(220) >= 2)
                    {
                        Card.ToSelect(Cards.Red);
                    }
                    else if ((Me.Mana + W.Instance.SData.Mana) <= (Q.Instance.SData.Mana + W.Instance.SData.Mana) && Menu["TwistedFate_Combo"]["WMP"])
                    {
                        Card.ToSelect(Cards.Blue);
                    }
                    else if (Menu["TwistedFate_Combo"]["WBlue"] && target.Health < W.GetDamage(target) - 50 && target.DistanceToPlayer() < 590)
                    {
                        Card.ToSelect(Cards.Blue);

                        if (Card.Status == SelectStatus.Selected && target.DistanceToPlayer() < 590)
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                    }
                    else
                    {
                        Card.ToSelect(Cards.Yellow);
                    }
                }
            }

        }

        private static void HarassLogic()
        {
            if (Me.ManaPercent >= Menu["TwistedFate_Harass"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var target = Variables.TargetSelector.GetTarget(Q.Range, Q.DamageType);

                if (CheckTarget(target))
                {
                    if (Menu["TwistedFate_Harass"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target, false, true);
                    }

                    if (Menu["TwistedFate_Harass"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        if (Me.Mana >= (Q.Instance.SData.Mana + W.Instance.SData.Mana))
                        {
                            Card.ToSelect(Cards.Blue);

                            if (Card.Status == SelectStatus.Selected && target.DistanceToPlayer() < 590)
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                            }
                        }
                    }
                }
            }
        }

        private static void LaneClearLogic()
        {
            if (Me.ManaPercent >= Menu["TwistedFate_LaneClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var min = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.IsValidTarget(Q.Range)).ToList();

                if (min.Count() > 0)
                {
                    if (Menu["TwistedFate_LaneClear"]["Q"] && Q.IsReady())
                    {
                        var QFarm = Q.GetLineFarmLocation(min, Q.Width);

                        if (QFarm.MinionsHit >= Menu["TwistedFate_LaneClear"]["QMin"].GetValue<MenuSlider>().Value)
                        {
                            Q.Cast(QFarm.Position);
                        }
                    }

                    if (Menu["TwistedFate_LaneClear"]["W"] && W.IsReady())
                    {
                        if (min.Count >= 3 && Me.ManaPercent > Menu["TwistedFate_LaneClear"]["WPickRed"].GetValue<MenuSlider>().Value)
                        {
                            Card.ToSelect(Cards.Red);
                        }
                        else
                        {
                            Card.ToSelect(Cards.Blue);
                        }
                    }
                }
            }
        }

        private static void JungleClearLogic()
        {
            if (Me.ManaPercent >= Menu["TwistedFate_JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var mobs = GetMobs(Me.Position, Q.Range, true);

                if (mobs.Count() > 0)
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu["TwistedFate_JungleClear"]["Q"] && Q.IsReady() && mob.IsValidTarget(Q.Range))
                    {
                        if (mob.Health >= Me.GetAutoAttackDamage(mob))
                        {
                            Q.Cast(mob);
                        }
                    }

                    if (Menu["TwistedFate_JungleClear"]["W"] && W.IsReady() && mob.IsValidTarget(W.Range))
                    {
                        var wmobs = GetMobs(Me.Position, W.Range);

                        if (wmobs.Count >= 2)
                        {
                            Card.ToSelect(Cards.Red);
                        }
                        else if (wmobs.Count < 2)
                        {
                            Card.ToSelect(Cards.Blue);
                        }
                        else if (mob.Health < W.GetDamage(mob))
                        {
                            Card.ToSelect(Cards.Blue);

                            if (Card.Status == SelectStatus.Selected)
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, mob);
                            }
                        }
                    }
                }
            }
        }

        private static void Auto()
        {
            if (Menu["TwistedFate_QKey"].GetValue<MenuKeyBind>().Active)
            {
                var e = Variables.TargetSelector.GetTarget(Q.Range, Q.DamageType);

                if (e != null && e.IsHPBarRendered)
                {
                    Q.Cast(e);
                    return;
                }
            }

            foreach (var e in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(Q.Range) && !e.IsZombie && e.IsHPBarRendered))
            {
                if (Menu["TwistedFate_Auto"]["DebuffQ"] && !CanMove(e))
                {
                    Q.Cast(e);
                }

                if (Menu["TwistedFate_KillSteal"]["Q"] && Q.GetDamage(e) > e.Health - 40)
                {
                    Q.Cast(e);
                }
            }
        }

        private static void OnCreate(GameObject sender, EventArgs Args)
        {
            if (Menu["TwistedFate_Auto"]["AntiW"])
            {
                var Tryndamere = GameObjects.EnemyHeroes.Find(heros => heros.ChampionName.Equals("Tryndamere"));

                if (Tryndamere != null)
                {
                    if (sender.Position.Distance(Me.Position) < 590)
                    {
                        if (Tryndamere.HasBuff("UndyingRage"))
                        {
                            if (sender.Position.Distance(Me.Position) < 350)
                            {
                                Card.ToSelect(Cards.Yellow);

                                if (Me.HasBuff("GoldCardPreAttack") && Tryndamere.ServerPosition.Distance(Me.Position) < 590)
                                {
                                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, Tryndamere);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void OnAction(object sender, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.BeforeAttack)
            {
                if (InCombo && Args.Target is AIHeroClient && Menu["TwistedFate_Misc"]["ComboDisableAA"] && !Menu["TwistedFate_Misc"]["ADMode"])
                {
                    if (Card.Status == SelectStatus.Selecting && Variables.TickCount - Card.PickCardTickCount > 300)
                    {
                        Args.Process = false;
                    }
                }

                if (InClear && Args.Target is Obj_AI_Turret && Me.CountEnemyHeroesInRange(800) < 1)
                {
                    if (W.IsReady() && Card.Status == SelectStatus.Ready)
                    {
                        Card.ToSelect(Cards.Blue);

                        if (Card.Status == SelectStatus.Selected && Args.Target.InAutoAttackRange())
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, Args.Target);
                        }
                    }
                }
            }
        }

        private static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs Args)
        {
            if (Args.Sender.IsEnemy)
            {
                if (W.IsReady() && Args.Sender.IsValidTarget(W.Range) && Menu["TwistedFate_Auto"]["InterW"])
                {
                    Card.ToSelect(Cards.Yellow);

                    if (Card.Status == SelectStatus.Selected && Args.Sender.ServerPosition.Distance(Me.ServerPosition) < 590)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, Args.Sender);
                    }
                }
            }
        }

        private static void OnGapCloser(object sender, Events.GapCloserEventArgs Args)
        {
            if (Args.Sender.IsEnemy)
            {
                if (W.IsReady() && W.IsInRange(Args.End) && Menu["TwistedFate_Auto"]["AntiW"])
                {
                    Card.ToSelect(Cards.Yellow);

                    if (Card.Status == SelectStatus.Selected && Args.Sender.ServerPosition.Distance(Me.ServerPosition) < 590)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, Args.Sender);
                    }
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe)
            {
                if (Args.Slot == SpellSlot.R && Args.SData.Name.Equals("Gate", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (Menu["TwistedFate_Misc"]["AutoYellow"] && W.IsReady())
                    {
                        Card.ToSelect(Cards.Yellow);
                    }
                }
            }
        }

        private static void OnEndScene(EventArgs Args)
        {
            var DrawRMin = Menu["TwistedFate_DrawMenu"]["DrawRMin"];

            if (R.IsReady() && DrawRMin)
            {
                DrawEndScene(R.Range);
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            var DrawQ = Menu["TwistedFate_DrawMenu"]["DrawQ"];
            var DrawW = Menu["TwistedFate_DrawMenu"]["DrawW"];
            var DrawR = Menu["TwistedFate_DrawMenu"]["DrawR"];
            var DrawAF = Menu["TwistedFate_DrawMenu"]["DrawAF"];
            var DrawDamage = Menu["TwistedFate_DrawMenu"]["DrawComboDamage"];

            if (Q.IsReady() && DrawQ)
            {
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue);
            }

            if (DrawW)
            {
                System.Drawing.Color FlowersStyle = System.Drawing.Color.LightGreen;

                if (GameObjects.Player.HasBuff("GoldCardPreAttack") || Card.Select == Cards.Yellow)
                {
                    FlowersStyle = System.Drawing.Color.Gold;
                }
                else if (GameObjects.Player.HasBuff("BlueCardPreAttack") || Card.Select == Cards.Blue)
                {
                    FlowersStyle = System.Drawing.Color.Blue;
                }
                else if (GameObjects.Player.HasBuff("RedCardPreAttack") || Card.Select == Cards.Red)
                {
                    FlowersStyle = System.Drawing.Color.Red;
                }
                else
                {
                    FlowersStyle = System.Drawing.Color.Teal;
                }

                Render.Circle.DrawCircle(Me.Position, 590, FlowersStyle, 1);
            }

            if (Flash.IsReady() && DrawAF)
            {
                Render.Circle.DrawCircle(Me.Position, 590 + 475, System.Drawing.Color.Gold, 1);
            }

            if (R.IsReady() && DrawR)
            {
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.White, 1);
            }

            if (DrawDamage)
            {
                foreach (var e in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsZombie))
                {
                    HpBarDraw.Unit = e;
                    HpBarDraw.DrawDmg((float)GetDamage(e), new ColorBGRA(255, 204, 0, 170));
                }
            }
        }

        public static int GetRamdonTime()
        {
            if (Menu["TwistedFate_Humanizer"]["EnableHumanizer"])
            {
                if (Menu["TwistedFate_Humanizer"]["LowHp"].GetValue<MenuSliderButton>().BValue && Me.HealthPercent <= Menu["TwistedFate_Humanizer"]["LowHp"].GetValue<MenuSliderButton>().SValue)
                {
                    return 0;
                }
                else
                    return Random.Next(Menu["TwistedFate_Humanizer"]["MinHumanizer"].GetValue<MenuSlider>().Value, Menu["TwistedFate_Humanizer"]["MaxHumanizer"].GetValue<MenuSlider>().Value);
            }
            else
                return 0;
        }
    }
}
