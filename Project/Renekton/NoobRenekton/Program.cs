using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NoobRenekton
{
    class Program
    {
        public const string ChampionName = "Renekton";

        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        static Orbwalking.Orbwalker orbwalker;
        static Menu menu;
        static AIHeroClient Player { get { return ObjectManager.Player; } }
        static AIHeroClient Target = null;

        private static Items.Item tiamat = new Items.Item(3077, 185);
        private static Items.Item hydra = new Items.Item(3074, 185);
        private static Items.Item titanic;
        private static bool IsEUsed
        {
            get { return Player.HasBuff("renektonsliceanddicedelay"); }
        }
        private static bool IsWUsed
        {
            get { return Player.HasBuff("renektonpreexecute"); }
        }

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        private static void Game_OnGameLoad(EventArgs args)
        {           
            if (Player.ChampionName != "Renekton")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 325);
            W = new Spell(SpellSlot.W, 176);
            E = new Spell(SpellSlot.E, 450);
            R = new Spell(SpellSlot.R);

            menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            var combo = new Menu("Combo", "Combo");
            menu.AddSubMenu(combo);
            combo.AddItem(new MenuItem("ComboMode", "ComboMode").SetValue(new StringList(new[] {"Burst", "Normal"})));
            combo.AddItem(new MenuItem("BurstExplained", "Burst will cast E+AA+W+Hydra+Q"));
            combo.AddItem(new MenuItem("NormalExplained", "Normal will cast E+AA+W+Hydra+AA+Q"));
            combo.AddItem(new MenuItem("----", ""));
            combo.AddItem(new MenuItem("-----", "Combo"));
            combo.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("useE", "Use E").SetValue(false));
            combo.AddItem(new MenuItem("useR", "Use R").SetValue(false));
            combo.AddItem(new MenuItem("-------", "Auto R saver"));
            combo.AddItem(new MenuItem("comboSliderR", "Use Auto R at Health (%)").SetValue(new Slider(15, 1, 100)));


            var lc = new Menu("Laneclear", "Laneclear");
            menu.AddSubMenu(lc);
            lc.AddItem(new MenuItem("laneclearQ", "Use Q to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearW", "Use W to LaneClear").SetValue(true));

            var harass = new Menu("Harass", "Harass");
            menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("AutoHarassQ", "Auto Q if enemy is in Range").SetValue(false));

            var ks = new Menu("KillSteal", "KillSteal");
            menu.AddSubMenu(ks);
            ks.AddItem(new MenuItem("useQKS", "Auto Q to KillSteal").SetValue(true));

            hydra = new Items.Item((int)ItemId.Ravenous_Hydra_Melee_Only, 250);
            tiamat = new Items.Item((int)ItemId.Tiamat_Melee_Only, 250);
            titanic = new Items.Item(3748, 450);
            menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalking.AfterAttack += AfterAa;
            Chat.Print("NoobRenekton by 1Shinigamix3");
        }
        private static void AfterAa(AttackableUnit unit, AttackableUnit target)
        {
            AIHeroClient o = TargetSelector.GetTarget(450, TargetSelector.DamageType.Physical);
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (menu.Item("ComboMode").GetValue<StringList>().SelectedIndex == 0)//burst
                {
                    if (menu.Item("useW").GetValue<bool>()) W.Cast();

                    if (hydra.IsOwned() && Player.Distance(o) < hydra.Range && hydra.IsReady() && !W.IsReady()) hydra.Cast();
                    if (tiamat.IsOwned() && Player.Distance(o) < tiamat.Range && tiamat.IsReady() && !W.IsReady()) tiamat.Cast();
                    if (titanic.IsOwned() && Player.Distance(o) < titanic.Range && titanic.IsReady() && !W.IsReady()) titanic.Cast();
                }
                if (menu.Item("ComboMode").GetValue<StringList>().SelectedIndex == 1)//normal
                {
                    if (menu.Item("useW").GetValue<bool>()) W.Cast();

                    if (hydra.IsOwned() && Player.Distance(o) < hydra.Range && hydra.IsReady() && !W.IsReady()) hydra.Cast();
                    if (tiamat.IsOwned() && Player.Distance(o) < tiamat.Range && tiamat.IsReady() && !W.IsReady()) tiamat.Cast();
                    if (titanic.IsOwned() && Player.Distance(o) < titanic.Range && titanic.IsReady() && !W.IsReady()) titanic.Cast();
                    if (menu.Item("useQ").GetValue<bool>() && !W.IsReady() && !IsEUsed) Q.Cast();
                }
            }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Obj_AI_Base minion = MinionManager.GetMinions(600, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault();
                if (menu.Item("laneclearW").GetValue<bool>())
                    W.Cast(minion);
                if (hydra.IsOwned() && Player.Distance(minion) < hydra.Range && hydra.IsReady())
                    hydra.Cast(minion);
                if (tiamat.IsOwned() && Player.Distance(minion) < tiamat.Range && tiamat.IsReady())
                    tiamat.Cast(minion);
                if (menu.Item("laneclearQ").GetValue<bool>() && !W.IsReady() && !IsWUsed)
                    Q.Cast(minion);
            }
        }
        private static void OnUpdate(EventArgs args)
        {
            AIHeroClient o = TargetSelector.GetTarget(450, TargetSelector.DamageType.Physical);
            if (menu.Item("useQKS").GetValue<bool>())
                if (o.Health < Q.GetDamage(o))
                {
                    Q.Cast(o);
                }
            if (menu.Item("AutoHarassQ").GetValue<bool>())
            {
                Q.Cast(o);
            }
            if (Player.HealthPercent < menu.Item("comboSliderR").GetValue<Slider>().Value && R.IsReady())
                R.Cast();
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (menu.Item("ComboMode").GetValue<StringList>().SelectedIndex == 0)//burst
                {
                    if (menu.Item("useE").GetValue<bool>()) if (Player.Distance(o.Position) > 125 && (W.IsReady()) && !IsEUsed) E.Cast(o);
                    if (menu.Item("useR").GetValue<bool>() && !E.IsReady()) R.Cast();
                    if (menu.Item("useQ").GetValue<bool>() && !W.IsReady() && !IsWUsed) Q.Cast();
                }
                if (menu.Item("ComboMode").GetValue<StringList>().SelectedIndex == 1)//normal
                {
                    if (menu.Item("useE").GetValue<bool>()) if (Player.Distance(o.Position) > 125 && (W.IsReady()) && !IsEUsed) E.Cast(o);
                    if (menu.Item("useR").GetValue<bool>() && !W.IsReady() && !IsEUsed) R.Cast();
                }
            }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (hydra.IsOwned() && Player.Distance(o) < hydra.Range && hydra.IsReady() && !W.IsReady()) hydra.Cast();
                if (tiamat.IsOwned() && Player.Distance(o) < tiamat.Range && tiamat.IsReady() && !W.IsReady()) tiamat.Cast();

                if (menu.Item("useQ").GetValue<bool>() && !W.IsReady())
                    Q.Cast(o);
            }           
        }      
    }
  }

