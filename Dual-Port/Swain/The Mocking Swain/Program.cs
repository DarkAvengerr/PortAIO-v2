#region Credits

//  Special Thanks:
//      xQx - For his Swain, Some of the functions here are from that assembly. 
//      xSalice - For his blitzcrank assembly, I modified one of its function and adapted it to swain.
//      SSJ4 - For his base template

#endregion

#region

using System;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace The_Mocking_Swain
{
    internal class Program
    {
        private const string Champion = "Swain";
        private static Orbwalking.Orbwalker Orbwalker;
        private static Spell Q, W, E, R;
        private static Menu Config;
        private static Items.Item Zhonya;
        private static bool RavenForm;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }


        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.BaseSkinName != Champion) return;

            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R, 625);


            Q.SetTargetted(0.5f, float.MaxValue);
            W.SetSkillshot(0.5f, 275, 1250, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0.5f, 1400);

            Zhonya = new Items.Item(3157);

            Config = new Menu("The Mocking Swain", "Swain", true);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);


            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Combo Menu
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("C_UseQ", "Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("C_UseW", "W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("C_UseE", "E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("C_UseR", "R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("C_MockingSwain", "Use Zhonya while Ult").SetValue(true));
            Config.SubMenu("Combo")
                .AddItem(
                    new MenuItem("C_MockingSwainSlider", "Zhonya ult at Health (%)").SetValue(new Slider(30, 1)));

            //Harass Menu
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("H_UseQ", "Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("H_UseW", "W").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("H_UseE", "E").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("H_AutoE", "Auto-E enemies").SetValue(true));
            Config.SubMenu("Harass")
                .AddItem(new MenuItem("H_ESlider", "Stop Auto E at Mana (%)").SetValue(new Slider(80, 1)));

            //Lane Clear
            Config.AddSubMenu(new Menu("Lane Clear", "Lane Clear"));
            Config.SubMenu("Lane Clear").AddItem(new MenuItem("LC_UseW", "W").SetValue(true));
            Config.SubMenu("Lane Clear").AddItem(new MenuItem("LC_UseR", "R").SetValue(true));

            //Last Hit
            Config.AddSubMenu(new Menu("Last Hit", "Last Hit"));
            Config.SubMenu("Last Hit").AddItem(new MenuItem("LH_UseQ", "Q").SetValue(true));
            Config.SubMenu("Last Hit").AddItem(new MenuItem("LH_UseE", "E").SetValue(true));
            Config.AddItem(new MenuItem("urfmode", "URF Mode").SetValue(false));
            Chat.Print("The Mocking Swain Loaded!");
            //Nerd Shit
            Config.AddToMainMenu();
            Game.OnUpdate+= OnGameUpdate;
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }

            if (Config.Item("H_AutoE").GetValue<bool>())
            {
                AutoE();
            }
        }

        //Credits to: xQx for this function
        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (!(sender.Name.Contains("swain_demonForm")))
                return;
            RavenForm = true;
        }

        //Credits to: xQx for this function
        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            if (!(sender.Name.Contains("swain_demonForm")))
                return;
            RavenForm = false;
        }

        //Credits to: xSalice, this is from his Blitzcrank script, I just modified it
        private static bool SafeWCast(AIHeroClient target)
        {
            if (target == null) return false;

            if (W.GetPrediction(target).Hitchance == HitChance.Immobile) return true;

            if (!Config.Item("urfmode").GetValue<bool>())
            {
                if (Q.LSIsReady()) return false;
            }
            
            if (target.HasBuffOfType(BuffType.Slow) && W.GetPrediction(target).Hitchance >= HitChance.High)
                return true;
                
            return W.GetPrediction(target).Hitchance == HitChance.VeryHigh;
        }

        //Thanks to xQx
        private static void AutoE()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var ManaLimit = Player.MaxMana/100*Config.Item("H_ESlider").GetValue<Slider>().Value;
            
            if (!Config.Item("urfmode").GetValue<bool>())
            {
                if (Player.Mana <= ManaLimit) return;
            }
            
            if (E.LSIsReady())
            {
                E.Cast(target);
            }
        }

        private static void MockingSwain()
        {
            var HealthLimit = Player.MaxHealth/100*Config.Item("C_MockingSwainSlider").GetValue<Slider>().Value;
            if (!RavenForm || !(Player.Health <= HealthLimit)) return;
            if (Zhonya.IsReady())
            {
                Zhonya.Cast();
            }
        }

        private static void Combo()
        {
            CastSpells(Config.Item("C_UseQ").GetValue<bool>(),
                Config.Item("C_UseW").GetValue<bool>(),
                Config.Item("C_UseE").GetValue<bool>(),
                Config.Item("C_UseR").GetValue<bool>());

            if (Config.Item("C_MockingSwain").GetValue<bool>())
            {
                MockingSwain();
            }
        }

        private static void Harass()
        {
            CastSpells(Config.Item("H_UseQ").GetValue<bool>(),
                Config.Item("H_UseW").GetValue<bool>(),
                Config.Item("H_UseE").GetValue<bool>(), false);
        }

        //Kortaru's Xerath W laneclear adapted to swain
        private static void LaneClear()
        {
            var useW = Config.Item("LC_UseW").GetValue<bool>();
            var useR = Config.Item("LC_UseR").GetValue<bool>();

            var minions = MinionManager.GetMinions(Player.ServerPosition, R.Range);

            var WRangedMinions = MinionManager.GetMinions(Player.ServerPosition, W.Range + W.Width, MinionTypes.Ranged,
                MinionTeam.NotAlly, MinionOrderTypes.Health);
            var WAllMinions = MinionManager.GetMinions(Player.ServerPosition, W.Range + W.Width, MinionTypes.All,
                MinionTeam.NotAlly, MinionOrderTypes.Health);
            var PredictWRangedMinions = W.GetCircularFarmLocation(WRangedMinions, W.Width*0.75f);
            var PredictWAllMinions = W.GetCircularFarmLocation(WAllMinions, W.Width*0.75f);


            if (useW && W.LSIsReady())
            {
                if (PredictWRangedMinions.MinionsHit >= 3 && W.IsInRange(PredictWRangedMinions.Position.To3D()))
                {
                    W.Cast(PredictWRangedMinions.Position);
                    return;
                }
                if (PredictWAllMinions.MinionsHit >= 3 && W.IsInRange(PredictWAllMinions.Position.To3D()))
                {
                    W.Cast(PredictWAllMinions.Position);
                    return;
                }
            }

            if (useR && R.LSIsReady())
            {
                if (minions.Count >= 3 && Player.LSDistance(minions[0]) <= R.Range)
                {
                    if (!RavenForm)
                    {
                        R.Cast();
                    }
                }
                else if (RavenForm && !Config.Item("urfmode").GetValue<bool>())
                {
                    R.Cast();
                }
            }
        }

        private static void LastHit()
        {
            var useQ = Config.Item("LH_UseQ").GetValue<bool>();
            var useE = Config.Item("LH_UseE").GetValue<bool>();

            var Minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            foreach (var minion in Minions)
            {
                if (useQ)
                {
                    if (minion.Health < Player.LSGetSpellDamage(minion, SpellSlot.Q) && Q.LSIsReady())
                    {
                        Q.Cast(minion);
                        return;
                    }
                }

                if (useE)
                {
                    if (minion.Health < E.GetDamage(minion)/4 && E.LSIsReady())
                    {
                        E.Cast(minion);
                        return;
                    }
                }
            }
        }

        private static void CastSpells(bool useQ, bool useW, bool useE, bool useR)
        {
            var target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Magical);
            if (target == null) return;

            //E
            if (E.LSIsReady() && useE)
            {
                E.Cast(target);
            }

            //Q
            if (Q.LSIsReady() && useQ)
            {
                Q.Cast(target);
            }

            //W
            if (target.LSIsValidTarget(W.Range) && W.LSIsReady() && SafeWCast(target) && useW)
            {
                var prediction = W.GetPrediction(target);
                W.Cast(prediction.CastPosition);
            }

            //R
            if (R.LSIsReady() && target.LSIsValidTarget(R.Range) && !RavenForm && useR)
            {
                R.Cast();
            }
        }
    }
}