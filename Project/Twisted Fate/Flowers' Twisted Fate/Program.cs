using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace FlowersTwistedFate
{
    public static class Program
    {
        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }
        internal static float getManaPer
        {
            get { return Player.Mana / Player.MaxMana * 100; }
        }

        private static Spell Q;
        public static Spell W;
        private static Spell R;
        private static Orbwalking.Orbwalker Orbwalker;
        private static Menu Menu;
        public const string ChampionName = "TwistedFate";

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != ChampionName)
            {
                return;
            }

            Chat.Print("Flowers " + Player.CharData.BaseSkinName + " Loaded!");
            Chat.Print("Credit : NightMoon!");

            Menu = new Menu("FL - Twisted Fate", "flowersKappa", true);

            Orbwalker = new Orbwalking.Orbwalker(Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker")));

            Menu.AddSubMenu(new Menu("Combo", "Combo"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("lzq", "Use Q")).SetValue(true);
            Menu.SubMenu("Combo").AddItem(new MenuItem("lzw", "Use W(Yellow or Blue)")).SetValue(true);
            Menu.SubMenu("Combo").AddItem(new MenuItem("lzwBMama", "Use Blue Mana <=%", true).SetValue(new Slider(20, 0, 50)));

            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("srq", "Use Q")).SetValue(true);
            Menu.SubMenu("Harass").AddItem(new MenuItem("AutoQ", "Auto Q").SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Toggle)));
            Menu.SubMenu("Harass").AddItem(new MenuItem("srw", "Use W(Blue Card)")).SetValue(true);
            Menu.SubMenu("Harass").AddItem(new MenuItem("srwr", "Use W(Red Card)")).SetValue(true);

            Menu.AddSubMenu(new Menu("Clear", "Clear"));
            Menu.SubMenu("Clear").AddItem(new MenuItem("qxq", "Use Q LaneClear").SetValue(true));
            Menu.SubMenu("Clear").AddItem(new MenuItem("qxw", "Use W LaneClear (Red or Blue)").SetValue(true));
            Menu.SubMenu("Clear").AddItem(new MenuItem("qxmp", "LC Use Blue Mana <=%", true).SetValue(new Slider(45, 0, 100)));
            Menu.SubMenu("Clear").AddItem(new MenuItem("qyq", "Use Q JungleClear").SetValue(true));
            Menu.SubMenu("Clear").AddItem(new MenuItem("qyw", "Use W JungleClear (Red or Blue)").SetValue(true));
            Menu.SubMenu("Clear").AddItem(new MenuItem("qymp", "JC Use Blue Mana <=%", true).SetValue(new Slider(45, 0, 100)));

            Menu.AddSubMenu(new Menu("Card Select", "CardSelect"));
            Menu.SubMenu("CardSelect").AddItem(new MenuItem("blue", "Blue Card").SetValue(new KeyBind("E".ToCharArray()[0], KeyBindType.Press)));
            Menu.SubMenu("CardSelect").AddItem(new MenuItem("yellow", "Yellow Card").SetValue(new KeyBind("W".ToCharArray()[0], KeyBindType.Press)));
            Menu.SubMenu("CardSelect").AddItem(new MenuItem("red", "Red Card").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            Menu.AddSubMenu(new Menu("Misc", "Misc"));
            Menu.SubMenu("Misc").AddItem(new MenuItem("KSQ", "Use Q KS/Stun")).SetValue(true);
            Menu.SubMenu("Misc").AddItem(new MenuItem("dd", "Use W Interrupt Spell")).SetValue(true);
            Menu.SubMenu("Misc").AddItem(new MenuItem("tj", "Use W Anti GapCloser")).SetValue(true);
            Menu.SubMenu("Misc").AddItem(new MenuItem("AutoYellow", "Auto Yellow Card In Uit").SetValue(true));

            Menu.AddSubMenu(new Menu("Draw", "Draw"));
            Menu.SubMenu("Draw").AddItem(new MenuItem("drawoff", "Disabled All Drawing").SetValue(false));
            Menu.SubMenu("Draw").AddItem(new MenuItem("drawingQ", "Q Range").SetValue(new Circle(true, Color.FromArgb(138, 101, 255))));
            Menu.SubMenu("Draw").AddItem(new MenuItem("drawingR", "R Range").SetValue(new Circle(true, Color.FromArgb(0, 255, 0))));
            Menu.SubMenu("Draw").AddItem(new MenuItem("drawingR2", "R Range (MiniMap)").SetValue(new Circle(true, Color.FromArgb(255, 255, 255))));
            Menu.SubMenu("Draw").AddItem(new MenuItem("drawingAA", "Real AA&W Range(Flowers Style)").SetValue(true));

            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw damage after combo").SetValue(true);
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };
            Menu.SubMenu("Draw").AddItem(dmgAfterComboItem);

            Menu.AddItem(new MenuItem("Credit", "Credit : NightMoon"));

            Menu.AddToMainMenu();

            Chat.Print("This Assembly will be obsolete, plz use new version");

            Q = new Spell(SpellSlot.Q, 1450f);
            W = new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(Player));
            R = new Spell(SpellSlot.R, 5500f);

            Q.SetSkillshot(0.25f, 40f, 1000f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.3f, 80f, 1600, true, SkillshotType.SkillshotLine);

            Game.OnUpdate += GAME_ONUPDATE;
            Drawing.OnDraw += DRAW;
            Drawing.OnEndScene += DRAWEND;
            Orbwalking.BeforeAttack += OrbwalkingOnBeforeAttack;//H7 
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;//H7 
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        static void Interrupter2_OnInterruptableTarget(AIHeroClient target, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("dd").GetValue<bool>() && W.IsReady() && W.IsInRange(target))
            {
                CardSelect.StartSelecting(Cards.Yellow);
            }

            if (Player.HasBuff("goldcardpreattack") && Orbwalking.InAutoAttackRange(target))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }
            if (Menu.Item("tj").GetValue<bool>() && W.IsReady() && W.IsInRange(gapcloser.End))
            {
                CardSelect.StartSelecting(Cards.Yellow);
            }

            if(Player.HasBuff("goldcardpreattack") && Orbwalking.InAutoAttackRange(gapcloser.Sender))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
            }
        }

        static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var AUTOYELLOW = Menu.Item("AutoYellow").GetValue<bool>();

            if (args.SData.Name.Equals("Gate", StringComparison.InvariantCultureIgnoreCase) && AUTOYELLOW)
            {
                CardSelect.StartSelecting(Cards.Yellow);
            }
        }

        static void OrbwalkingOnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target is AIHeroClient)
                args.Process = CardSelect.Status != SelectStatus.Selecting && Utils.TickCount - CardSelect.LastWSent > 300;
        }

        static void DRAW(EventArgs args)
        {
            var disdraw = Menu.Item("drawoff").GetValue<bool>();

            if (disdraw)
            {
                return;
            }

            var FlowersStyle = Menu.Item("drawingAA").GetValue<bool>();
            var Q范围 = Menu.Item("drawingQ").GetValue<Circle>();
            var R范围 = Menu.Item("drawingR").GetValue<Circle>();

            if (FlowersStyle)
            {
                    Color FlowersAAStyle = Color.LightGreen;
                    var Wbuff = Player.Spellbook.GetSpell(SpellSlot.W).Name;
                    if(Wbuff == "goldcardlock")
                    {
                        FlowersAAStyle = Color.Gold;
                    }
                    else if(Wbuff == "bluecardlock")
                    {
                        FlowersAAStyle = Color.Blue;
                    }
                    else if (Wbuff == "redcardlock")
                    {
                        FlowersAAStyle = Color.Red;
                    }
                    else if (Wbuff == "PickACard")
                    {
                        FlowersAAStyle = Color.GreenYellow;
                    }

                    Render.Circle.DrawCircle(Player.Position, Orbwalking.GetRealAutoAttackRange(Player), FlowersAAStyle ,2);
            }

            if (Q.IsReady() && Q范围.Active)
                Render.Circle.DrawCircle(Player.Position, Q.Range, Q范围.Color);

            if (R.IsReady() && R范围.Active)
                Render.Circle.DrawCircle(Player.Position, 5500, R范围.Color);

        }

        static float ComboDamage(AIHeroClient hero)
        {
            var dmg = 0d;
            dmg += Player.GetSpellDamage(hero, SpellSlot.Q) * 2;
            dmg += Player.GetSpellDamage(hero, SpellSlot.W);
            dmg += Player.GetSpellDamage(hero, SpellSlot.Q);

            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += ObjectManager.Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }

            return (float)dmg;
        }

        static void DRAWEND(EventArgs args)
        {
            var MINIR = Menu.Item("drawingR2").GetValue<Circle>();

            if (MINIR.Active)
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, 5500, MINIR.Color, 1, 30, true);
            }
        }

        static void GAME_ONUPDATE(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            CardSelects();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    HarassW();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
            }

            if (Menu.Item("AutoQ").GetValue<KeyBind>().Active)
            {
                Harass();
            }
            KSQ();
        }

        private static void CardSelects()
        {
            if (Menu.Item("yellow").GetValue<KeyBind>().Active)
            {
                CardSelect.StartSelecting(Cards.Yellow);
            }


            if (Menu.Item("blue").GetValue<KeyBind>().Active)
            {
                CardSelect.StartSelecting(Cards.Blue);
            }


            if (Menu.Item("red").GetValue<KeyBind>().Active)
            {
                CardSelect.StartSelecting(Cards.Red);
            }
        }

        static void Combo()
        {
            var Combotarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (Menu.Item("lzw").GetValue<bool>())
            {
                if (W.IsReady() || Player.HasBuff("PickACard"))
                {
                    if (Combotarget.IsValidTarget(W.Range))
                    {
                        foreach (var target in ObjectManager.Get<AIHeroClient>().Where
                            (target => !target.IsMe && target.Team != ObjectManager.Player.Team))
                        if (target.Health < W.GetDamage(target) && Player.Distance(target, true) < 600 && !target.IsDead && target.IsValidTarget())
                        {
                            CardSelect.StartSelecting(Cards.Blue);
                        }
                        else
                        {
                            CardSelect.StartSelecting(Cards.Yellow);
                        }
                    }
                }
            }
        }

        static void Harass()
        {
            var target = TargetSelector.GetTarget(1300, TargetSelector.DamageType.Physical);
            if (Q.IsReady() && (Menu.Item("srq").GetValue<bool>()))
            {
                var Qprediction = Q.GetPrediction(target);

                if (Qprediction.Hitchance >= HitChance.VeryHigh)
                {
                    Q.Cast(Qprediction.CastPosition);
                }
            }
        }

        static void HarassW()
        {
            var target = TargetSelector.GetTarget(1300, TargetSelector.DamageType.Physical);

            if (target.IsValidTarget(1300))
            {
                if (Player.Distance(target.ServerPosition) < Player.AttackRange - 40 && Menu.Item("srw").GetValue<bool>())
                {
                    CardSelect.StartSelecting(Cards.Blue);
                }

                if (Player.Distance(target, true) < Player.AttackRange - 150 && Menu.Item("srwr").GetValue<bool>())
                {
                    CardSelect.StartSelecting(Cards.Red);
                }
            }

        }

        static void LaneClear()
        {
            if (Q.IsReady() && Menu.Item("qxq").GetValue<bool>() && getManaPer > 40)
            {
                var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy);
                var locQ = Q.GetLineFarmLocation(allMinionsQ);

                if (locQ.MinionsHit >= 3)
                    Q.Cast(locQ.Position);
            }

            var minioncount = MinionManager.GetMinions(Player.Position, 1500).Count;

            if(!Menu.Item("qxw").GetValue<bool>())
            {
                if (minioncount > 0)
                {
                    if (getManaPer > Menu.Item("qxmp").GetValue<Slider>().Value)
                    {
                        if (minioncount >= 3)
                            CardSelect.StartSelecting(Cards.Red);
                        else
                            CardSelect.StartSelecting(Cards.Blue);
                    }
                    else
                        CardSelect.StartSelecting(Cards.Blue);
                }
            }
        }

        static void JungleClear()
        {

            var mobs = MinionManager.GetMinions(Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(Player) + 50,
                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs.Count <= 0)
                return;

            if (Q.IsReady() && Menu.Item("qyq").GetValue<bool>() && getManaPer > 45)
            {
                Q.Cast(mobs[0].Position);
            }

            if (W.IsReady() && Menu.Item("qyw").GetValue<bool>())
            {
                if (getManaPer > Menu.Item("qymp").GetValue<Slider>().Value)
                {
                    if (mobs.Count >= 2)
                        CardSelect.StartSelecting(Cards.Red);
                }
                else
                    CardSelect.StartSelecting(Cards.Blue);
            }
        }

        static void KSQ()
        {
            if (!Menu.Item("KSQ").GetValue<bool>())
                return;

            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.IsDead && !x.HasBuffOfType(BuffType.Invulnerability)))
            {
                if (target != null)
                {
                    if (Q.GetDamage(target) >= target.Health + 20 & Q.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                    {
                        if (Q.IsReady())
                            Q.Cast(target);
                    }
                }
            }
        }
    }
}
