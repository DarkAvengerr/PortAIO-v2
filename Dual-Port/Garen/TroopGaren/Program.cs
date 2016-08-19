using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TroopGaren
{
    class Program
    {
        public const string ChampionName = "Garen";

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

        public static Spell Q, W, E, R;

        private static Items.Item botrk;

        private static Items.Item Tiamat;

        private static Items.Item Hydra;

        private static Items.Item Randuins;

        private static Obj_AI_Base target;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Garen") return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 325f);
            R = new Spell(SpellSlot.R, 400f);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            Menu spellMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            spellMenu.AddItem(new MenuItem("comQ", "Use Q").SetValue(true));
            spellMenu.AddItem(new MenuItem("comW", "Use W").SetValue(true));
            spellMenu.AddItem(new MenuItem("comE", "Use E").SetValue(true));
            spellMenu.AddItem(new MenuItem("comR", "Check Misc Menu for R Options").SetValue(false));

            var harass = new Menu("Harass", "Harass");
            Menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("harassQ", "Use Q to harass").SetValue(true));
            harass.AddItem(new MenuItem("harassE", "Use E to harass").SetValue(true));


            Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Draw_Disabled", "Disable all Drawings").SetValue(false));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Wdraw", "Draw W Range").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(true));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("Rdraw", "Draw R Range").SetValue(true));

            Menu.AddSubMenu(new Menu("laneclear", "laneclear"));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("lQ", "use Q to Laneclear").SetValue(true));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("lE", "use E to Laneclear").SetValue(true));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("tQ", "use Q to Autoattack Tower").SetValue(true));

            Menu.AddSubMenu(new Menu("Jungleclear", "Jungleclear"));
            Menu.SubMenu("Jungleclear").AddItem(new MenuItem("jQ", "use Q to Jungleclear").SetValue(true));
            Menu.SubMenu("Jungleclear").AddItem(new MenuItem("jE", "use E to Jungleclear").SetValue(true));

            Menu.AddSubMenu(new Menu("Misc", "Misc"));
            Menu.SubMenu("Misc").AddItem(new MenuItem("rKS", "use R to KS").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("QGap", "use Q to gapclose").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("QInt", "use Q to Interrupt").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("EHyd", "use Hydra for E Swag").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Randuins", "use Randuins to Slow").SetValue(true));

            Menu.AddItem(new MenuItem("Credits", "Assembly created by trooperhdx"));
            ;

            Tiamat = new Items.Item(3077, 185);
            botrk = new Items.Item(3153, 450);
            Randuins = new Items.Item(3143, 500);
            Hydra = new Items.Item(3074, 185);

            Menu.AddToMainMenu();
            OnSpellCast();
            Drawing.OnDraw += OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalking.BeforeAttack += BeforeAA;
            Orbwalking.AfterAttack += AfterAA;
            Orbwalking.OnAttack += OnAa;
            Game.OnUpdate += OnUpdate;

            Chat.Print(
                "<font color='#00CC83'>trooperhdx:</font> <font color='#B6250B'>" + Player.ChampionName
                + " Loaded<font color='#00B4D2'> Dont forget to Upvote this Assembly on the Assembly Database! </font>");
        }

        private static void OnAa(AttackableUnit unit, AttackableUnit attackableUnit)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                AIHeroClient target = TargetSelector.GetTarget(500, TargetSelector.DamageType.Physical);
                if (Menu.Item("comQ").GetValue<bool>()) if (Player.Distance(target.Position) > 175 && (Q.IsReady()) && Q.IsReady()) Q.Cast(target);

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {

                }
            }
        }

        private static void BeforeAA(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var unit = args.Target as Obj_AI_Turret;
                if (unit != null)
                {
                    if (Menu.Item("tQ").GetValue<bool>())
                    {
                        if (((Obj_AI_Turret)args.Target).Health >= Player.TotalAttackDamage * 2)
                        {
                            Q.CastOnUnit(unit);
                        }
                    }
                }
            }
        }

        private static
            void AfterAA(AttackableUnit unit, AttackableUnit attackableUnit)
        {
            var d = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                if (Menu.Item("comE").GetValue<bool>())
                {
                    {
                        E.Cast(Game.CursorPos);
                    }
                        if (Menu.Item("comW").GetValue<bool>())
                        {
                            W.CastOnBestTarget();
                        }

                }
        }



        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Q.IsReady() && target.IsValidTarget(R.Range) && Menu.Item("QInt").GetValue<bool>()) Q.CastOnUnit(target);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Q.IsReady() && gapcloser.Sender.IsValidTarget(R.Range) && Menu.Item("QGap").GetValue<bool>()) Q.CastOnUnit(gapcloser.Sender);
        }

        private static void OnDraw(EventArgs args)
        {
            var Target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (Menu.Item("Draw_Disabled").GetValue<bool>()) return;

            if (Menu.Item("Qdraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.CadetBlue, 3);
            if (Menu.Item("Wdraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.IndianRed, 3);
            if (Menu.Item("Edraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.DarkSeaGreen, 3);
            if (Menu.Item("Rdraw").GetValue<bool>()) Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.BurlyWood, 3);
        }

        private static void OnSpellCast()
        {
        }

        private static
            void OnUpdate(EventArgs args)
        {
            if (Menu.Item("rKS").GetValue<bool>())
            {
                Ks();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Lane();
                Jungle();
            }
        }

        private static void Ks()
        {
            var useR = (Menu.Item("rKS").GetValue<bool>());
            var m = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (m != null && m.Health < R.GetDamage(m) && useR)
            {
                R.CastOnUnit(m);
            }
        }

        private static void Jungle()
        {
            var EActivated = false;
            foreach (var buff in Player.Buffs)
            {
                if (buff.Name == "GarenE") EActivated = true;
                var allMinions = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    175f,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);
                if (Menu.Item("jE").GetValue<bool>() && E.IsReady() && !EActivated)
                {

                    foreach (var minion in allMinions)
                    {
                        if (minion.IsValidTarget())
                        {
                            E.CastOnUnit(minion);
                        }
                        if (Menu.Item("jQ").GetValue<bool>() && Q.IsReady())
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

        private static void Lane()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 175f);
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);
            {
                foreach (var minion in allMinions)
                {
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                        if (Menu.Item("lQ").GetValue<bool>() && Q.IsReady())
                        {
                            if (minion.IsValidTarget())
                            {
                                Q.CastOnUnit(minion);
                            }
                        }
                    if (Menu.Item("lE").GetValue<bool>() && E.IsReady())
                    {
                        if (minion.IsValidTarget())
                        {
                            E.CastOnUnit(minion);
                        }
                    }
                }
            }
        }


        private static void Combo()
        {
            var m = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);


            if (Menu.Item("EHyd").GetValue<bool>())
                if (Hydra.IsOwned() && Player.Distance(m) < Hydra.Range && Hydra.IsReady())
                    Hydra.Cast();
            if (Tiamat.IsOwned() && Player.Distance(m) < Tiamat.Range && Tiamat.IsReady() && !E.IsReady())
                    Tiamat.Cast();


            if (Menu.Item("Randuins").GetValue<bool>())
            if (Randuins.IsOwned() && Player.Distance(m) < Randuins.Range && Randuins.IsReady())
                    Randuins.Cast();


        }
    }
}
