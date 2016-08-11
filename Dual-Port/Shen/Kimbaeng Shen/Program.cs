using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kimbaeng_Shen
{

    class Program
    {
        private static Menu _Menu;

        private static Orbwalking.Orbwalker _Orbwalker;


        private static Spell Q, W, E, R, EFlash;

        private static SpellSlot IgniteSlot;

        private static SpellSlot FlashSlot;

        private static Vector2 PingLocation;

        private static int LastPingT = 0;

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Shen") return;

            Q = new Spell(SpellSlot.Q, 465);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R);
            EFlash = new Spell(SpellSlot.E, 980);
            EFlash.SetSkillshot(
                E.Instance.SData.SpellCastTime, E.Instance.SData.LineWidth, E.Speed, false, SkillshotType.SkillshotLine);

            Q.SetSkillshot(0f, 150f, float.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 150f, float.MaxValue, false, SkillshotType.SkillshotLine);
            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            FlashSlot = ObjectManager.Player.GetSpellSlot("SummonerFlash");

            (_Menu = new Menu("Kimbaeng Shen", "kimbaengshen", true)).AddToMainMenu();

            var targetSelectorMenu = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            _Menu.AddSubMenu(targetSelectorMenu);

            _Orbwalker = new Orbwalking.Orbwalker(_Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking")));

            var comboMenu = _Menu.AddSubMenu(new Menu("combo", "Combo"));
            comboMenu.AddItem(new MenuItem("useCQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("useCE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseI", "Use Ignite").SetValue(true));
            comboMenu.AddItem(new MenuItem("useEF", "Use E+Flash"))
                .SetValue(new KeyBind('T', KeyBindType.Press));
    //        comboMenu.AddItem(new MenuItem("useR", "Use R Need Help Ally"))
    //.SetValue(new KeyBind("R".ToCharArray()[0], KeyBindType.Press));

            var harassMenu = _Menu.AddSubMenu(new Menu("Harass", "Harass"));
            harassMenu.AddItem(new MenuItem("useHQ", "UseQ").SetValue(true));
            harassMenu.AddItem(new MenuItem("useHE", "UseE").SetValue(false));

            var laneMenu = _Menu.AddSubMenu(new Menu("Lane Clear", "laneclear"));
            laneMenu.AddItem(new MenuItem("useLQ", "Use Q").SetValue(true));
            laneMenu.AddItem(new MenuItem("useLW", "Use W").SetValue(true));

            var LastHitMenu = _Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            LastHitMenu.AddItem(new MenuItem("useLHQ", "Use Q").SetValue(true));

            var MiscMenu = _Menu.AddSubMenu(new Menu("Misc","misc"));
            var UltMenu = MiscMenu.AddSubMenu(new Menu("Ult Notification", "Ult"));
            UltMenu.AddItem(new MenuItem("ultping", "Ult Ping").SetValue(true));
            UltMenu.AddItem(new MenuItem("ulttext", "Ult Text").SetValue(true));
            foreach (var hero in HeroManager.Allies)
            {
                    UltMenu.AddItem(new MenuItem("ultnotifiy" + hero.ChampionName, "Ult Notify to " + hero.ChampionName).SetValue(true));
                    UltMenu.AddItem(new MenuItem("HP" + hero.ChampionName, "HP %").SetValue(new Slider(40, 0, 100)));
                    //UltMenu.AddItem(new MenuItem("priority" + hero.ChampionName, "Set Ult priority").SetValue(new Slider(4, 1, 5)));
                    //Console.WriteLine(hero.ChampionName);   
            }

            MiscMenu.AddItem(new MenuItem("autow", "Auto Sheid W").SetValue(true));


            var DrawMenu = _Menu.AddSubMenu(new Menu("Drawing", "drawing"));
            DrawMenu.AddItem(new MenuItem("noDraw", "Disable Drawing").SetValue(true));
            DrawMenu.AddItem(new MenuItem("drawQ", "DrawQ").SetValue(new Circle(true, System.Drawing.Color.Olive)));
            DrawMenu.AddItem(new MenuItem("drawE", "DrawE").SetValue(new Circle(true, System.Drawing.Color.Olive)));
            DrawMenu.AddItem(new MenuItem("drawEF", "DrawEFlash").SetValue(new Circle(true, System.Drawing.Color.Lime)));
            Game.OnUpdate += Game_onUpdate;
            Drawing.OnDraw += Drawing_Ondraw;
            Obj_AI_Base.OnSpellCast += OnProcessSpellCast;

            Chat.Print("<font color=\"#672FBB\">Kimbaeng Shen</font> Loaded ");
            Chat.Print("If You like this Assembly plz <font color=\"#41FF3A\">Upvote</font> XD ");
        }

        private static void Game_onUpdate(EventArgs args)
        {
            
            if (_Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (_Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                LastHit();
                Harass();
            }

            if (_Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear();
            }

            if (_Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                LastHit();
            }

            if (_Menu.Item("useEF").GetValue<KeyBind>().Active)
            {
                FlashECombo();
            }

            //if (_Menu.Item("UseR").GetValue<KeyBind>().Active)
            //{

            //}

            Auto();

        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (W.Level == 0 && !W.IsReady()) return;

            if (_Menu.Item("autow").GetValue<bool>())
            {

                if (W.IsReady() && !sender.IsMe && sender.IsEnemy && (sender is AIHeroClient || sender is Obj_AI_Turret)
                    && args.Target.IsMe)
                {
                    W.Cast();
                }
                    return;
            }
            return;
        }



        private static void Combo()
        {
            var Target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Target == null) return;
            var useQ = _Menu.Item("useCQ").GetValue<bool>();
            var useE = _Menu.Item("useCE").GetValue<bool>();
            var UseI = _Menu.Item("UseI").GetValue<bool>();

            if (Q.IsReady() && useQ && Target.IsValidTarget(Q.Range))
            {
                Q.Cast(Target);
            }
            if (E.IsReady() && useE && Target.IsValidTarget(E.Range))
            {
                E.CastIfHitchanceEquals(Target, HitChance.High);
            }

            if (IgniteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready &&
                ObjectManager.Player.Distance(Target.ServerPosition) < 600 &&
                ObjectManager.Player.GetSummonerSpellDamage(Target, Damage.SummonerSpell.Ignite) > Target.Health && UseI)
            {
                ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, Target);
            }
        }


        static void Harass()
        {
            var Target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (Target == null) return;

            var useQ = _Menu.Item("useHQ").GetValue<bool>();
            var useE = _Menu.Item("useHE").GetValue<bool>();

            if (Q.IsReady() && useQ && Target.IsValidTarget(Q.Range))
            {
                Q.Cast(Target);
            }

            if (E.IsReady() && useE && Target.IsValidTarget(E.Range) && ObjectManager.Player.HasBuff("shenwayoftheninjaaura") && W.IsReady())
            {
                E.Cast(Target.Position);
            }
        }

        static void LaneClear()
        {
            var useQ = _Menu.Item("useLQ").GetValue<bool>();
            var useW = _Menu.Item("useLW").GetValue<bool>();

            var Minions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
            var junglemobs = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (useQ && Q.IsReady() && Minions.Count > 0)
            {
                Q.Cast();

            }
        }

        static void LastHit()
        {
            var useQ = _Menu.Item("useLHQ").GetValue<bool>();

            if (!Q.IsReady() || !Orbwalking.CanMove(100))
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.Position,
                Q.Range,
                MinionTypes.All,
                MinionTeam.NotAlly,
                MinionOrderTypes.MaxHealth);
            if (minions.Count > 0 && useQ)
            {
                foreach (var minion in minions)
                {
                    if (ObjectManager.Player.Distance(minion) <= ObjectManager.Player.AttackRange)
                    {
                        return;
                    }
                    if (HealthPrediction.GetHealthPrediction(
                        minion,
                        (int)(Q.Delay + (minion.Distance(ObjectManager.Player.Position) * 0.7)))
                        < ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q))
                    {
                        Q.Cast(minion);
                    }
                }
            }
        }

        private static void FlashECombo()
        {
            var FTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var STarget = TargetSelector.GetTarget(EFlash.Range, TargetSelector.DamageType.Magical, false, FTarget != null ? new[] { FTarget } : null);
            var EFTarget = TargetSelector.GetSelectedTarget();

            if (EFTarget != null)
            {

                if (ObjectManager.Player.IsDashing() && ObjectManager.Player.Distance(EFTarget.Position) < 420)
                {
                    ObjectManager.Player.Spellbook.CastSpell(FlashSlot, EFTarget.Position);
                }
                if (E.IsReady() && FlashSlot != SpellSlot.Unknown
                    && ObjectManager.Player.Spellbook.CanUseSpell(FlashSlot) == SpellState.Ready && EFTarget.IsValidTarget(EFlash.Range))
                {
                    E.Cast(EFTarget.Position);
                }

            }


            if (FTarget != null && STarget != null)
            {


                if (E.IsReady() && FlashSlot != SpellSlot.Unknown
                    && ObjectManager.Player.Spellbook.CanUseSpell(FlashSlot) == SpellState.Ready && FTarget.Distance(STarget) < 410)
                
                    //var Endpos = ObjectManager.Player.Position.Extend(FTarget.Position, 600);
                    //E.Cast(Endpos);
                    E.CastIfHitchanceEquals(FTarget, HitChance.High); 
                if (FTarget.HasBuffOfType(BuffType.Taunt) && ObjectManager.Player.IsDashing() && ObjectManager.Player.Distance(STarget) < 420)

                    ObjectManager.Player.Spellbook.CastSpell(FlashSlot, STarget.Position);

            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
        }

        static void UltLogic()
        {
            
        }
        static void Auto()
        {
            var pos = Drawing.WorldToScreen(ObjectManager.Player.Position)[1] + 20;
            if (R.Level != 0 && R.IsReady())
                foreach (var hero in HeroManager.Allies.Where(x => x.IsValidTarget(R.Range, false) && _Menu.Item("ultnotifiy" + x.ChampionName).GetValue<bool>()))
                {

                    if (hero.Health * 100 / hero.MaxHealth < _Menu.Item("HP" + hero.ChampionName).GetValue<Slider>().Value && !hero.IsMe)
                    {
                        if (_Menu.Item("ultping").GetValue<bool>())
                            Ping(hero.Position.To2D());
                        if (_Menu.Item("ulttext").GetValue<bool>())
                            Drawing.DrawText(Drawing.WorldToScreen(ObjectManager.Player.Position)[0] - 30
                                , pos, System.Drawing.Color.Gold, hero.ChampionName + " Need Help!");
                        pos = pos + 20;
                    }

                }

        }


        private static void Ping(Vector2 position)
        {
            if (Utils.TickCount - LastPingT < 30 * 1000)
            {
                return;
            }

            LastPingT = Utils.TickCount;
            PingLocation = position;
            SimplePing();

            LeagueSharp.Common.Utility.DelayAction.Add(100, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(300, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(600, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(800, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(1200, SimplePing);

        }
        private static void SimplePing()
        {
            TacticalMap.ShowPing(PingCategory.AssistMe, PingLocation, true);




        }


        private static void Drawing_Ondraw(EventArgs args)
        {
            var FTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var STarget = TargetSelector.GetTarget(EFlash.Range, TargetSelector.DamageType.Magical, false, FTarget != null ? new[] { FTarget } : null);
            var EFTarget = TargetSelector.GetSelectedTarget();

            if (EFTarget != null)
            {
                Render.Circle.DrawCircle(EFTarget.Position, 100, System.Drawing.Color.Lime);
                Drawing.DrawText(
                    Drawing.WorldToScreen(EFTarget.Position)[0] - 30,
                    Drawing.WorldToScreen(EFTarget.Position)[1] + 20,
                    System.Drawing.Color.Lime,
                    "EF Target");
            }

            if (FTarget != null && EFTarget == null)
            {
                Render.Circle.DrawCircle(FTarget.Position, 50, System.Drawing.Color.Red);
                Drawing.DrawText(
                   Drawing.WorldToScreen(FTarget.Position)[0] - 30,
                   Drawing.WorldToScreen(FTarget.Position)[1] + 20,
                   System.Drawing.Color.Red,
                   "First Target");
            }

            if (STarget != null && EFTarget == null)
            {
                Render.Circle.DrawCircle(STarget.Position, 50, System.Drawing.Color.OrangeRed);
                Drawing.DrawText(
                    Drawing.WorldToScreen(STarget.Position)[0] - 30,
                    Drawing.WorldToScreen(STarget.Position)[1] + 20,
                    System.Drawing.Color.OrangeRed,
                    "Second Target");
            }


            if (_Menu.Item("noDraw").GetValue<bool>())
            {
                return;
            }

            var Qvalue = _Menu.Item("drawQ").GetValue<Circle>();
            var Evalue = _Menu.Item("drawE").GetValue<Circle>();
            var EFvalue = _Menu.Item("drawEF").GetValue<Circle>();


            if (Qvalue.Active)
            {
                if (Q.Instance.Level != 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Qvalue.Color);

            }

            if (Evalue.Active)
            {
                if (E.Instance.Level != 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Evalue.Color);
            }

            if (EFvalue.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, EFlash.Range, EFvalue.Color);
            }


        }
    }
}
