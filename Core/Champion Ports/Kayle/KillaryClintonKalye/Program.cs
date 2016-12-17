#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

using EloBuddy; 
using LeagueSharp.Common; 
namespace kck
{
    internal class Program
    {
        public const string ChampionName = "Kayle";

        //Orbwalker instance
        public static Orbwalking.Orbwalker Orbwalker;

        //Spells
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        //Menu
        public static Menu Config;

        private static AIHeroClient Player;

        private static Vector2 PingLocation;
        private static int LastPingT = 0;
        private static bool AttacksEnabled
        {
            get
            { 

                return true;
            }
        }     

        public static void Main()
        {
            Game_OnGameLoad(new EventArgs());
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            if (Player.ChampionName != ChampionName) return;

            //Create the spells
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E );
            R = new Spell(SpellSlot.R, 900);

           

           

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            //Create the menu
            Config = new Menu(ChampionName, ChampionName, true);

            //Orbwalker submenu
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            //Add the target selector to the menu as submenu.
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Load the orbwalker and add it to the menu as submenu.
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Combo menu:
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("catchup", "catchupQ").SetValue(true));
            Config.SubMenu("Combo")
                .AddItem(
                    new MenuItem("ComboActive", "Combo!").SetValue(
                        new KeyBind(Config.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //Misc
                     

            //Harass menu:
            Config.AddSubMenu(new Menu("Harass", "Harass"));
          
            Config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassActive", "Harass!").SetValue(
                        new KeyBind(Config.Item("Farm").GetValue<KeyBind>().Key, KeyBindType.Press)));
            Config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassActiveT", "Harass (toggle)!").SetValue(new KeyBind('Y',
                        KeyBindType.Toggle)));

            //Farming menu:
            Config.AddSubMenu(new Menu("Farm", "Farm"));
           
            Config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("LaneClearActive", "LaneClear!").SetValue(
                        new KeyBind(Config.Item("LaneClear").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //JungleFarm menu:
           
            Config.SubMenu("JungleFarm")
                .AddItem(
                    new MenuItem("JungleFarmActive", "JungleFarm!").SetValue(
                        new KeyBind(Config.Item("LaneClear").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //Misc
            
            //Damage after combo:
           

            //Drawings menu:
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("QRange", "Q range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("WRange", "W range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("ERange", "E range").SetValue(new Circle(false, Color.FromArgb(150, Color.DodgerBlue))));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("RRange", "R range").SetValue(new Circle(false, Color.FromArgb(150, Color.DodgerBlue))));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("RRangeM", "R range (minimap)").SetValue(new Circle(false,
                        Color.FromArgb(150, Color.DodgerBlue))));

            Config.AddToMainMenu();

            //Add the events we are going to use:
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            
           

            
            
            Orbwalking.BeforeAttack += OrbwalkingOnBeforeAttack;
            
        }

        

        

        private static void OrbwalkingOnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            args.Process = AttacksEnabled;
        }

      

      


        private static void Combo()
        {
            CastE();
            if (Q.IsReady()) castQ();
        }

        private static void Harass()
        {
            
            CastE();
            if (Q.IsReady()) castQ();

        }
        

       

        private static void Farm(bool laneClear)
        {
                E.CastOnUnit(Player);
        }

        private static void JungleFarm()
        {
            E.CastOnUnit(Player);
        }

        private static void castQ()
        {


                var target = TargetSelector.GetTarget(650, TargetSelector.DamageType.Magical);
                int ff = 15;
                if (Player.Position.Distance(target.Position) > 200 && Player.CountEnemiesInRange(1000) < 3 && Orbwalking.CanMove(80))
                {
                    if (target.GetBuffCount("judicatorholyfervordebuff") == 5)
                    {
                        Q.CastOnUnit(target);
                    if (W.IsReady()) { W.CastOnUnit(Player); }
                    LeagueSharp.Common.Utility.DelayAction.Add(ff, () => Orbwalking.Orbwalk(target, target.Position));
                        return;
                    }
                    if (target.GetBuffCount("judicatorholyfervordebuff") == 4 && Player.Position.Distance(target.Position) > 350)
                    {
                        Q.CastOnUnit(target);
                    if (W.IsReady()) { W.CastOnUnit(Player); }
                    LeagueSharp.Common.Utility.DelayAction.Add(ff, () => Orbwalking.Orbwalk(target, target.Position));
                        return;
                    }
                    if (target.GetBuffCount("judicatorholyfervordebuff") == 3 && Player.Position.Distance(target.Position) > 450)
                    {
                        Q.CastOnUnit(target);
                    if (W.IsReady()) { W.CastOnUnit(Player); }
                    LeagueSharp.Common.Utility.DelayAction.Add(ff, () => Orbwalking.Orbwalk(target, target.Position));
                        return;
                    }
                    if (target.GetBuffCount("judicatorholyfervordebuff") == 2 && Player.Position.Distance(target.Position) > 550)
                    {
                        Q.CastOnUnit(target);
                    if (W.IsReady()) { W.CastOnUnit(Player); }
                    LeagueSharp.Common.Utility.DelayAction.Add(ff, () => Orbwalking.Orbwalk(target, target.Position));
                        return;
                    }
                    if (target.GetBuffCount("judicatorholyfervordebuff") == 1 && Player.Position.Distance(target.Position) > 600)
                    {
                        Q.CastOnUnit(target);
                    if (W.IsReady()) { W.CastOnUnit(Player); }
                    LeagueSharp.Common.Utility.DelayAction.Add(ff, () => Orbwalking.Orbwalk(target, target.Position));
                        return;
                    }
                    if (!target.HasBuff("judicatorholyfervordebuff") && Player.Position.Distance(target.Position) < 650 && Config.Item("catchup").GetValue<bool>())
                    {
                        Q.CastOnUnit(target);
                    if (W.IsReady()) { W.CastOnUnit(Player); }
                    LeagueSharp.Common.Utility.DelayAction.Add(ff, () => Orbwalking.Orbwalk(target, target.Position));
                        return;

                    }
                }
                else Q.CastOnUnit(target);
                LeagueSharp.Common.Utility.DelayAction.Add(ff, () => Orbwalking.Orbwalk(target, target.Position));
          

        }
        private static void CastE()
        {
            if (Player.GetEnemiesInRange(650).Count > 0)
            {
                
                E.CastOnUnit(Player);
            }
            if (MinionManager.GetMinions(650).Count>0){
                E.CastOnUnit(Player);
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

            LeagueSharp.Common.Utility.DelayAction.Add(150, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(300, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(400, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(800, SimplePing);
        }

        private static void SimplePing()
        {
            TacticalMap.ShowPing(PingCategory.Fallback, PingLocation, true);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {

            UltTarget();
           if (Player.HealthPercent < Player.ManaPercent && Player.Mana-getManacost() > 0 && !Player.IsRecalling() && Orbwalking.CanMove(80))
            {
                W.CastOnUnit(Player);
            }
            if (Config.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else
            {
                if (Config.Item("HarassActive").GetValue<KeyBind>().Active ||
                    Config.Item("HarassActiveT").GetValue<KeyBind>().Active)
                    Harass();

                var lc = Config.Item("LaneClearActive").GetValue<KeyBind>().Active;
                if (lc || Config.Item("FreezeActive").GetValue<KeyBind>().Active)
                    Farm(lc);

                if (Config.Item("JungleFarmActive").GetValue<KeyBind>().Active)
                    JungleFarm();
            }
        }

        private static float getManacost()
        {
            float mc = 0;
            if (R.Cooldown < 20)
            {
                mc += R.ManaCost;
            }
            if (Q.Cooldown < 4)
            {
                mc += Q.ManaCost;
            }
            if (E.Cooldown<4)
            {
                mc += E.ManaCost;
            }
            return mc;
        }

        private static void UltTarget()
        {
            var nearbyfriends = ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && x.IsChampion() && !x.IsDead);
            string[] heronames = new string[5];
            double[] AD = new double[5];
            double[] AP = new double[5];
            foreach (var hero in nearbyfriends)
            {
                int i = 0;

                AD[i] = hero.AttackSpeedMod * hero.TotalAttackDamage * (hero.PercentCritDamageMod * hero.FlatCritChanceMod);
                AP[i] = hero.TotalMagicalDamage;
            }

            foreach (var hero in nearbyfriends.Where(x => x.Distance(Player) < 900))
            {
                if (hero.AttackSpeedMod * hero.TotalAttackDamage * (hero.PercentCritDamageMod * hero.FlatCritChanceMod) == AD.Max() && hero.CountEnemiesInRange (1200)>0 )
                {
                    if (hero.Health < hero.MaxHealth * 0.2 || hero.Health<200)
                    {
                        R.CastOnUnit(hero);
                    }

                }
                if (hero.AbilityPower() == AP.Max() && hero.CountEnemiesInRange(1200) > 0)
                {
                    if (hero.Health < hero.MaxHealth * 0.2 || hero.Health < 200)
                    {
                        R.CastOnUnit(hero);
                    }
                }
            }
        }




        private static void Drawing_OnDraw(EventArgs args)
        {
            

            //Draw the ranges of the spells.
            foreach (var spell in SpellList)
            {
                var menuItem = Config.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active && (spell.Slot != SpellSlot.R || R.Level > 0))
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color);
            }
        }
    }
}