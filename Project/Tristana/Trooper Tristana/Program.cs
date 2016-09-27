using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TrooperTristana
{

    class Program
    {
        public const string ChampionName = "Tristana";

        public static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        public static Orbwalking.Orbwalker Orbwalker;

        //Menu
        public static Menu Menu;

        //Spells
        public static List<Spell> SpellList = new List<Spell>();

        private static bool ETris => Player.HasBuff("Tristanaecharge");

        public static Spell Q, W, E, R;

        private static Items.Item cutlass;

        private static Items.Item botrk;

        private static Obj_AI_Base target;
        private static Obj_AI_Base turretAiBase;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Tristana") return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 875f);
            W.SetSkillshot(0.6f, 875f, float.MaxValue, false, SkillshotType.SkillshotLine);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 700f);


            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            //SpellMenu
            Menu spellMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            spellMenu.AddItem(new MenuItem("comQ", "Use Q").SetValue(true));
            spellMenu.AddItem(new MenuItem("comW", "Use W").SetValue(true));
            spellMenu.AddItem(new MenuItem("comE", "Use E").SetValue(true));
            //spellMenu.AddItem(new MenuItem("comR", "Use R").SetValue(true));
            //spellMenu.AddItem(new MenuItem("comE_R", "Ult if E explode will kill enemy").SetValue(true));
            //Harass Menu
            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("harassE", "use E to Harass").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("harassQ", "use Q to Harass").SetValue(true));

            //LaneClear Menu
            Menu.AddSubMenu(new Menu("Laneclear", "Laneclear"));
            Menu.SubMenu("Laneclear").AddItem(new MenuItem("laneE", "use E to Laneclear").SetValue(true));
            Menu.SubMenu("Laneclear").AddItem(new MenuItem("laneQ", "use Q to Laneclear").SetValue(true));
            Menu.SubMenu("Laneclear").AddItem(new MenuItem("towerE", "use E to push turrets").SetValue(true));
            Menu.SubMenu("Laneclear").AddItem(new MenuItem("towerQ", "use Q to push turrets").SetValue(true));
            //Drawings
            Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Draw_Disabled", "Disable all Drawings").SetValue(false));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Wdraw", "Draw W Range").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Rdraw", "Draw R Range").SetValue(true));
            //Misc
            Menu.AddSubMenu(new Menu("Misc", "Misc"));
            Menu.SubMenu("Misc").AddItem(new MenuItem("useRks", "use R to Ks / Kill").SetValue(true));
            Menu.SubMenu("Misc")
                .AddItem(
                    new MenuItem("Flee", "Flee Key").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
            Menu.SubMenu("Misc").AddItem(new MenuItem("RGap", "use R to gapclose").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("RInt", "use R to Interrupt").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("WFlee", "Use W to Flee").SetValue(true));
            //Jungleclear
            Menu.AddSubMenu(new Menu("Jungleclear", "Jungleclear"));
            Menu.SubMenu("Jungleclear").AddItem(new MenuItem("eJungle", "Use E for Jungleclear").SetValue(true));
            Menu.SubMenu("Jungleclear").AddItem(new MenuItem("qJungle", "Use Q to Jungleclear").SetValue(true));
            //Credits
            Menu.AddItem(new MenuItem("Credits", "Assembly created by trooperhdx"));





            cutlass = new Items.Item(3144, 450);
            botrk = new Items.Item(3153, 450);

            Menu.AddToMainMenu();
            OnSpellCast();
            Drawing.OnDraw += OnDraw;
            R_E_EXPLODE();
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalking.BeforeAttack += BeforeAA;
            Orbwalking.OnAttack += OnAa;
            Game.OnUpdate += OnUpdate;
            Chat.Print(
"<font color='#00CC83'>trooperhdx:</font> <font color='#B6250B'>" + Player.ChampionName
+ " Loaded<font color='#00B4D2'> Dont forget to Upvote this Assembly on the Assembly Database! </font>");
        }

        private static void R_E_EXPLODE()
        {
            //Coming Soon
        }

        private static void OnAa(AttackableUnit unit, AttackableUnit attackableUnit)
        {
            var x = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (Menu.Item("comQ").GetValue<bool>() && Q.IsReady())
                        Q.CastOnBestTarget();
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && target.IsValidTarget(R.Range) && Menu.Item("RInt").GetValue<bool>())
                R.CastOnUnit(target);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady() && gapcloser.Sender.IsValidTarget(R.Range) && Menu.Item("RGap").GetValue<bool>())
                R.CastOnUnit(gapcloser.Sender);
        }

        private static void BeforeAA(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var unit = args.Target as Obj_AI_Turret;
                if (unit != null)
                {
                    if (Menu.Item("towerE").GetValue<bool>())
                    {
                        if (((Obj_AI_Turret)args.Target).Health >= Player.TotalAttackDamage * 3)
                        {
                            E.CastOnUnit(unit);
                        }
                        if (Menu.Item("towerQ").GetValue<bool>())
                        {
                            if (((Obj_AI_Turret)args.Target).Health >= Player.TotalAttackDamage * 1)
                            {
                                Q.CastOnUnit(unit);
                            }
                        }
                    }
                }
            }
        }

        private static
            void OnDraw(EventArgs args)
        {
            var Target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (Menu.Item("Draw_Disabled").GetValue<bool>()) return;

            if (Menu.Item("Qdraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.CadetBlue, 3);
            if (Menu.Item("Wdraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.IndianRed, 3);
            if (Menu.Item("Edraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.DarkSeaGreen, 3);
            if (Menu.Item("Rdraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.BurlyWood, 3);
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Menu.Item("useRks").GetValue<bool>())
            {
                Killsecure();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                //Harass
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Lane();
                Jungle();
            }
            if (Menu.Item("Flee").GetValue<KeyBind>().Active)
            {
                Flee();
            }
        }

        private static void Flee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (Menu.Item("WFlee").GetValue<bool>() && W.IsReady())
            {
                W.Cast(Game.CursorPos);
            }
        }

        private static void Killsecure()
        {
            var useR = (Menu.Item("useRks").GetValue<bool>());
            var l = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (l != null && l.Health < R.GetDamage(l) && useR)
            {
                R.CastOnUnit(l);
            }
        }

        private static void Combo()
        {
            var useQ = (Menu.Item("comQ").GetValue<bool>());
            var useE = (Menu.Item("comE").GetValue<bool>());
            var useR = (Menu.Item("comR").GetValue<bool>());

            var x = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);




            if (x != null && Player.Distance(x) <= botrk.Range)
            {
                botrk.Cast(x);
            }
            if (x != null && Player.Distance(x) <= cutlass.Range)
            {
                cutlass.Cast(x);
            }

            //combo
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
            }
        }


        // private static void Harass()
        //{
        //var y = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
        //if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
        //if (Menu.Item("harassE").GetValue<bool>())
        //   {
        //  if (Player.Distance(y.Position) > 700 && (E.IsReady() && Q.IsReady()))
        //{
        // E.CastOnBestTarget();
        // }
        // if (Menu.Item("harassQ").GetValue<bool>())
        // {
        //  if (Player.Distance(y.Position) > 700 && (Q.IsReady() && !E.IsReady()))
        //  {
        //     Q.Cast();
        //  }
        // }
        //  }
        //}

        private static void OnSpellCast() => Obj_AI_Base.OnSpellCast += (sender, args) =>
            {
                if (sender.IsMe && args.SData.IsAutoAttack())
                {
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        if (Menu.Item("comE").GetValue<bool>() && E.IsReady()) E.CastOnBestTarget();
                    }
                }
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                    if (Menu.Item("harassE").GetValue<bool>())
                    {
                        {
                            E.CastOnBestTarget();
                        }

                        if (Menu.Item("harassQ").GetValue<bool>())
                        {
                            Q.CastOnBestTarget();
                        }

                    }
            };

        private static void Lane()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 290f);
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            {
                foreach (var minion in allMinions)
                {
                    if (Menu.Item("laneE").GetValue<bool>() && E.IsReady())
                    {
                        if (minion.IsValidTarget())
                        {
                            E.CastOnUnit(minion);
                        }
                    }
                    if (Menu.Item("laneQ").GetValue<bool>() && Q.IsReady())
                    {
                        if (minion.IsValidTarget())
                        {
                            Q.CastOnUnit(minion);
                        }
                    }
                }
            }
        }
    

        private static void Jungle()
        {
            var allMinions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                Q.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (Menu.Item("eJungle").GetValue<bool>() && E.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        E.CastOnUnit(minion);
                    }
                    if (Menu.Item("qJungle").GetValue<bool>() && Q.IsReady())
                    {
                        if (minion.IsValidTarget())
                        {
                            Q.CastOnUnit(minion);
                        }
                    }
                }
            }
        }
    }
}