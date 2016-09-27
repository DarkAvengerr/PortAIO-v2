using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NoobWukong
{
    class Program
    {
        public const string ChampionName = "MonkeyKing";
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static Orbwalking.Orbwalker Orbwalker;
        //Menu
        public static Menu Menu;
        //Spells
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static int Ulti;

        private static Items.Item tiamat = new Items.Item(3077, 185);
        private static Items.Item hydra = new Items.Item(3074, 185);
        private static Obj_AI_Base target;
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "MonkeyKing") return;

            Q = new Spell(SpellSlot.Q, 375);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 640);
            R = new Spell(SpellSlot.R, 375);



            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(ts);

            //Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            //spellMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            //spellMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            //spellMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));

            var Combo = new Menu("Combo", "Combo");
            Menu.AddSubMenu(Combo);
            Combo.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            Combo.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            Combo.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            Combo.AddItem(new MenuItem("howmanyenemys", "Use R if Enemy Count >= (0 = off)").SetValue(new Slider(1, 5, 0)));

            var Harass = new Menu("Harass", "Harass");
            Menu.AddSubMenu(Harass);
            Harass.AddItem(new MenuItem("HarassQ", "Use Q").SetValue(true));
            Harass.AddItem(new MenuItem("HarassW", "Use W").SetValue(true));
            Harass.AddItem(new MenuItem("HarassE", "Use E").SetValue(true));

            hydra = new Items.Item((int)ItemId.Ravenous_Hydra_Melee_Only, 200);
            tiamat = new Items.Item((int)ItemId.Tiamat_Melee_Only, 200);

            Menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalking.OnAttack += OnAa;
            Spellbook.OnCastSpell += OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Orbwalking.AfterAttack += AfterAa;
            Chat.Print("NoobWukong by trooperhdx & 1Shinigamix3");

        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "MonkeyKingSpinToWin")
            {
                Ulti = (int)Game.Time;
            }
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E))
            {
                if (Player.HasBuff("MonkeyKingSpinToWin")) args.Process = false;
            }
        }




        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                AIHeroClient target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
                if (Menu.Item("useW").GetValue<bool>())
                    if (Player.Distance(target.Position) > 175 && (Q.IsReady()) && E.IsReady())
                        W.Cast(target);

                if (Menu.Item("useE").GetValue<bool>() && E.IsReady())
                {
                    E.CastOnUnit(target);
                }
                if (hydra.IsOwned() && Player.Distance(target) < hydra.Range && hydra.IsReady()) hydra.Cast();
                if (tiamat.IsOwned() && Player.Distance(target) < tiamat.Range && tiamat.IsReady()) tiamat.Cast();
                if (R.IsReady() && !Q.IsReady())
                {
                    var valR = Menu.Item("howmanyenemys").GetValue<Slider>().Value;
                    if (valR > 0 && Player.CountEnemiesInRange(R.Range) >= valR && !(Player.HasBuff("MonkeyKingSpinToWin")))
                    {
                        R.Cast();
                    }
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                AIHeroClient n = TargetSelector.GetTarget(640, TargetSelector.DamageType.Physical);
                if (Menu.Item("useE").GetValue<bool>())
                    if (Player.Distance(n.Position) > 175 && (Q.IsReady()))
                        E.Cast(n);
            }
        }
        private static void OnAa(AttackableUnit unit, AttackableUnit target)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (hydra.IsOwned() && Player.Distance(target) < hydra.Range && hydra.IsReady()) hydra.Cast();
                if (tiamat.IsOwned() && Player.Distance(target) < tiamat.Range && tiamat.IsReady()) tiamat.Cast();
            }
        }
        private static void AfterAa(AttackableUnit unit, AttackableUnit target)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {              
                if (Menu.Item("HarassQ").GetValue<bool>() || Menu.Item("useQ").GetValue<bool>())
                    Q.CastOnBestTarget();
            }            
        }
    }
}
