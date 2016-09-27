using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeFizz
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static Orbwalking.Orbwalker Orbwalker;

        private static Spell Q, W, E, E2, R;

        private static Menu Menu;
        private static List<string> list = new List<string>{ "FizzJump", "fizzjumpbuffer" };
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            //Verify Champion
            if (Player.ChampionName != "Fizz")
                return;


            //Spells
            Q = new Spell(SpellSlot.Q, 550);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 400);
            E2 = new Spell(SpellSlot.E, 300);
            R = new Spell(SpellSlot.R, 1275);
            R.SetSkillshot(0.25f, 120, 1350, true, SkillshotType.SkillshotLine);
            R.MinHitChance = HitChance.Medium;

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            //Orbwalker
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            //Targetsleector
            Menu.AddSubMenu(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);
            //spell menu
            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            Menu Combo = spellMenu.AddSubMenu(new Menu("Combo", "Combo"));
            Combo.AddItem(new MenuItem("Qcombo", "use Q").SetValue(true));
            Combo.AddItem(new MenuItem("QcomboGap", "Q gap selected (E is on)").SetValue(true));
            Combo.AddItem(new MenuItem("Ecombo", "use E").SetValue(true));
            Combo.AddItem(new MenuItem("Rcombo", "use R any").SetValue(true));
            Combo.AddItem(new MenuItem("RcomboSelected", "use R only selected").SetValue(true));

            Menu Harass = spellMenu.AddSubMenu(new Menu("Harass", "Harass"));
            Harass.AddItem(new MenuItem("Qharass", "use Q").SetValue(true));
            Harass.AddItem(new MenuItem("Eharass", "use E").SetValue(true));

            Menu Draw = Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Draw.AddItem(new MenuItem("DrawQ", "Q").SetValue(true));
            Draw.AddItem(new MenuItem("DrawE", "E").SetValue(true));
            //Attach to root
            Menu.AddToMainMenu();

            //Listen to events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Orbwalking.OnAttack += Orbwalking_OnAttack;
            //AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }


        private static int blockmovecount;
        private static bool blockmovebool;
        private static bool Qcombo { get { return Menu.Item("Qcombo").GetValue<bool>(); } }
        private static bool QcomboGap { get { return Menu.Item("QcomboGap").GetValue<bool>(); } }
        private static bool Ecombo { get { return Menu.Item("Ecombo").GetValue<bool>(); } }
        private static bool Rcombo { get { return Menu.Item("Rcombo").GetValue<bool>(); } }
        private static bool RcomboSelected { get { return Menu.Item("RcomboSelected").GetValue<bool>(); } }
        private static bool Qharass { get { return Menu.Item("Qharass").GetValue<bool>(); } }
        private static bool Eharass { get { return Menu.Item("Eharass").GetValue<bool>(); } }
        private static bool DrawQ { get { return Menu.Item("DrawQ").GetValue<bool>(); } }
        private static bool DrawE { get { return Menu.Item("DrawE").GetValue<bool>(); } }
        private static void Orbwalking_OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe) return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                W.Cast();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && (target as Obj_AI_Base).IsChampion())
                W.Cast();

        }
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            var spell = args.SData;
            //Chat.Say(spell.Name);
            if (spell.Name == "FizzPiercingStrike" && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed))
            {
                if (W.IsReady()) W.Cast();
            }
            if (spell.Name == "FizzSeastonePassive")
            {
            }
            if (spell.Name == "FizzJump")
            {
                blockmovecount = Utils.GameTimeTickCount;
                blockmovebool = true;
                Orbwalker.SetMovement(false);
            }
            if (spell.Name == "fizzjumptwo")
            {
                blockmovebool = false;
                Orbwalker.SetMovement(true);
            }
            if (spell.Name == "FizzMarinerDoom")
            {
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (DrawQ)
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Green);
            if (DrawE)
                Render.Circle.DrawCircle(Player.Position, E.Range + E2.Range, Color.Gold);
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            //Chat.Say(Player.Distance(Game.CursorPos).ToString());
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                combo();

            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                harass();

            }
            //if (E.Instance.Name == "fizzjumpbuffer" && E.IsReady())
            //{
            //    Chat.Say("hmm");
            //    E.Cast(Game.CursorPos.Extend(Player.Position, Player.Distance(Game.CursorPos) + 500));
            //}
            if (blockmovebool == true && Utils.GameTimeTickCount - blockmovecount >= 1500)
            {
                Orbwalker.SetMovement(true);
                blockmovebool = false;
            }

        }

        private static void harass()
        {
            // Q target
            if (Qharass)
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (Q.IsReady() && target.IsValidTarget() && !target.IsZombie)
                    Q.Cast(target);
            }
            if (Eharass)
            {
                // E1  slow target
                {
                    var target = TargetSelector.GetTarget(E.Range + 200 + 330, TargetSelector.DamageType.Magical);
                    if (E.Instance.Name == "FizzJump" && E.IsReady() && target.IsValidTarget() && !target.IsZombie)
                    {
                        if (Prediction.GetPrediction(target, 1.5f).UnitPosition.Distance(Player.Position) <= E.Range + 200 + 330)
                        {
                            var x = Player.Distance(Prediction.GetPrediction(target, 0.5f).UnitPosition) > E.Range ?
                                Player.Position.Extend(Prediction.GetPrediction(target, 0.5f).UnitPosition, E.Range) : target.Position;
                            E.Cast(x);
                        }
                    }
                }
                // E2
                {
                    var target = TargetSelector.GetTarget(140 + 200 + 330, TargetSelector.DamageType.Magical);
                    if (blockmovebool && target.IsValidTarget() && !target.IsZombie)
                    {
                        if (Prediction.GetPrediction(target, 1).UnitPosition.Distance(Player.Position) <= 200 + 330 + target.BoundingRadius)
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Prediction.GetPrediction(target, 1).UnitPosition);
                        }
                        else
                        {
                            var target2 = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && x.NetworkId != target.NetworkId)
                                .OrderByDescending(x => Prediction.GetPrediction(x, 1).UnitPosition.Distance(Player.Position)).LastOrDefault();
                            if (target2.IsValidTarget() && Prediction.GetPrediction(target2, 1).UnitPosition.Distance(Player.Position) <= 200 + 330 + target2.BoundingRadius)
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Prediction.GetPrediction(target2, 1).UnitPosition);
                            }
                            else
                            {
                                var target3 = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && x.NetworkId != target.NetworkId && x.NetworkId != target2.NetworkId)
                                    .OrderByDescending(x => Prediction.GetPrediction(x, 0.5f).UnitPosition.Distance(Player.Position)).LastOrDefault();
                                if (target3.IsValidTarget() && Prediction.GetPrediction(target3, 0.5f).UnitPosition.Distance(Player.Position) <= E2.Range + 50)
                                {
                                    E.Cast(Prediction.GetPrediction(target3, 0.5f).UnitPosition);
                                }
                                else
                                {
                                    var hero = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie)
                                        .OrderByDescending(x => Prediction.GetPrediction(x, 0.5f).UnitPosition.Distance(Player.Position)).LastOrDefault();
                                    E.Cast(Prediction.GetPrediction(hero, 0.5f).UnitPosition);
                                }

                            }
                        }
                    }
                    else if (blockmovebool)
                    {
                        var hero = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie)
                            .OrderByDescending(x => Prediction.GetPrediction(x, 0.5f).UnitPosition.Distance(Player.Position)).LastOrDefault();
                        E.Cast(Prediction.GetPrediction(hero, 0.5f).UnitPosition);
                    }
                }
                // E fast target
                {
                    var target = TargetSelector.GetTarget(E.Range + E2.Range + 50, TargetSelector.DamageType.Magical);
                    if (E.Instance.Name == "FizzJump" && E.IsReady() && target.IsValidTarget() && !target.IsZombie)
                    {
                        if (Prediction.GetPrediction(target, 1f).UnitPosition.Distance(Player.Position) <= E.Range + E2.Range + 150)
                        {
                            var x = Player.Distance(Prediction.GetPrediction(target, 0.5f).UnitPosition) > E.Range ?
                                Player.Position.Extend(Prediction.GetPrediction(target, 0.5f).UnitPosition, E.Range) : target.Position;
                            E.Cast(x);
                        }
                    }
                }
            }
        }
        private static void combo()
        {
            // Q target
            if (Qcombo)
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (Q.IsReady() && target.IsValidTarget() && !target.IsZombie)
                    Q.Cast(target);
            }
            if (Ecombo)
            {
                // E1  slow target
                {
                    var target = TargetSelector.GetTarget(E.Range + 200 + 330, TargetSelector.DamageType.Magical);
                    if (E.Instance.Name == "FizzJump" && E.IsReady() && target.IsValidTarget() && !target.IsZombie)
                    {
                        if (Prediction.GetPrediction(target, 1.5f).UnitPosition.Distance(Player.Position) <= E.Range + 200 + 330)
                        {
                            var x = Player.Distance(Prediction.GetPrediction(target, 0.5f).UnitPosition) > E.Range ?
                                Player.Position.Extend(Prediction.GetPrediction(target, 0.5f).UnitPosition, E.Range) : target.Position;
                            E.Cast(x);
                        }
                    }
                }
                // E2
                {
                    var target = TargetSelector.GetTarget(140 + 200 + 330, TargetSelector.DamageType.Magical);
                    if (blockmovebool && target.IsValidTarget() && !target.IsZombie)
                    {
                        if (Prediction.GetPrediction(target, 1).UnitPosition.Distance(Player.Position) <= 200 + 330 + target.BoundingRadius)
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Prediction.GetPrediction(target, 1).UnitPosition);
                        }
                        else
                        {
                            var target2 = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && x.NetworkId != target.NetworkId)
                                .OrderByDescending(x => Prediction.GetPrediction(x, 1).UnitPosition.Distance(Player.Position)).LastOrDefault();
                            if (target2.IsValidTarget() && Prediction.GetPrediction(target2, 1).UnitPosition.Distance(Player.Position) <= 200 + 330 + target2.BoundingRadius)
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Prediction.GetPrediction(target2, 1).UnitPosition);
                            }
                            else
                            {
                                var target3 = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && x.NetworkId != target.NetworkId && x.NetworkId != target2.NetworkId)
                                    .OrderByDescending(x => Prediction.GetPrediction(x, 0.5f).UnitPosition.Distance(Player.Position)).LastOrDefault();
                                if (target3.IsValidTarget() && Prediction.GetPrediction(target3, 0.5f).UnitPosition.Distance(Player.Position) <= E2.Range + 50)
                                {
                                    E.Cast(Prediction.GetPrediction(target3, 0.5f).UnitPosition);
                                }
                                else
                                {
                                    var hero = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie)
                                        .OrderByDescending(x => Prediction.GetPrediction(x, 0.5f).UnitPosition.Distance(Player.Position)).LastOrDefault();
                                    E.Cast(Prediction.GetPrediction(hero, 0.5f).UnitPosition);
                                }

                            }
                        }
                    }
                    else if (blockmovebool)
                    {
                        var hero = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie)
                            .OrderByDescending(x => Prediction.GetPrediction(x, 0.5f).UnitPosition.Distance(Player.Position)).LastOrDefault();
                        E.Cast(Prediction.GetPrediction(hero, 0.5f).UnitPosition);
                    }
                }
                // E fast target
                {
                    var target = TargetSelector.GetTarget(E.Range + E2.Range + 50, TargetSelector.DamageType.Magical);
                    if (E.Instance.Name == "FizzJump" && E.IsReady() && target.IsValidTarget() && !target.IsZombie)
                    {
                        if (Prediction.GetPrediction(target, 1f).UnitPosition.Distance(Player.Position) <= E.Range + E2.Range + 150)
                        {
                            var x = Player.Distance(Prediction.GetPrediction(target, 0.5f).UnitPosition) > E.Range ?
                                Player.Position.Extend(Prediction.GetPrediction(target, 0.5f).UnitPosition, E.Range) : target.Position;
                            E.Cast(x);
                        }
                    }
                }
            }
            // R target
            if (Rcombo)
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                {
                    if(R.IsReady() && target.IsValidTarget() && !target.IsZombie)
                    {
                        var x = R.GetPrediction(target).CastPosition;
                        var y = R.GetPrediction(target).CollisionObjects;
                        if (!y.Any(z => z.IsChampion()) && Player.Distance(x) <= R.Range)
                        {
                            R.Cast(x);
                        }
                    }

                }
            }
            // R Selected
            if (RcomboSelected)
            {
                var target = TargetSelector.GetSelectedTarget();
                {
                    if (R.IsReady() && target.IsValidTarget() && !target.IsZombie)
                    {
                        var x = R.GetPrediction(target).CastPosition;
                        var y = R.GetPrediction(target).CollisionObjects;
                        if (!y.Any(z => z.IsChampion()) && Player.Distance(x) <= R.Range)
                        {
                            R.Cast(x);
                        }
                    }

                }
            }
            // Q gap selected E ready
            if (QcomboGap && E.IsReady() && E.Instance.Name == "FizzJump")
            {
                var target = TargetSelector.GetSelectedTarget();
                if (Q.IsReady() && target.IsValidTarget() && !target.IsZombie)
                {
                    var minion = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.All)
                        .Where(x =>
                            x.Team != Player.Team && x.CharData.BaseSkinName != "gangplankbarrel")
                            .OrderByDescending(y => Player.Position.Extend(y.Position,Q.Range).Distance(target.Position)).LastOrDefault();
                    var hero = HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range))
                        .OrderByDescending(y => Player.Position.Extend(y.Position, Q.Range).Distance(target.Position)).LastOrDefault();
                    if (minion != null && hero != null)
                    {
                        if(Player.Position.Extend(minion.Position,Q.Range).Distance(target.Position)
                            > Player.Position.Extend(hero.Position,Q.Range).Distance(target.Position))
                        {
                            if (Player.Position.Extend(minion.Position, Q.Range).Distance(target.Position) <= 800 && !target.IsValidTarget(Q.Range + 200))
                                Q.Cast(minion);
                        }
                        else
                        {
                            if (Player.Position.Extend(hero.Position, Q.Range).Distance(target.Position) <= 800 && !target.IsValidTarget(Q.Range + 200))
                                Q.Cast(hero);
                        }
                    }
                    else
                    {
                        if (minion != null)
                        {
                            if (Player.Position.Extend(minion.Position, Q.Range).Distance(target.Position) <= 800 && !target.IsValidTarget(Q.Range + 200))
                                Q.Cast(minion);
                        }
                        if (hero != null)
                        {
                            if (Player.Position.Extend(hero.Position, Q.Range).Distance(target.Position) <= 800 && !target.IsValidTarget(Q.Range + 200))
                                Q.Cast(hero);
                        }
                    }
                }
            }
        }

    }
}
