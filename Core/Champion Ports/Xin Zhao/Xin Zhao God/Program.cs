using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Drawing;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Xin
{
    class Program
    {
        private static AIHeroClient Player;
        private static Menu Config;
        private static Orbwalking.Orbwalker Orbwalker;

        private static Spell Q, W, E, R;
        private static SpellSlot ignite;
        private static Items.Item youmuu, cutlass, blade, tiamat, hydra;

        public static void GameOnOnGameLoad()
        {
            Player = ObjectManager.Player;

            if (Player.CharData.BaseSkinName != "XinZhao")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 500);

            ignite = Player.GetSpellSlot("summonerdot");
            youmuu = new Items.Item(3142, 0f);
            cutlass = new Items.Item(3144, 450f);
            blade = new Items.Item(3153, 450f);
            tiamat = new Items.Item(3077, 400f);
            hydra = new Items.Item(3074, 400f);
           
            Config = new Menu("Xin Zhao God", "XinZhaoGod", true);

            var orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Config.AddSubMenu(orbwalkerMenu);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            var comboMenu = new Menu("Combo", "Combo");
            comboMenu.AddItem(new MenuItem("UseQCombo", "Use Q in Combo").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseWCombo", "Use W in Combo").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseECombo", "Use E in Combo").SetValue(true));
            comboMenu.AddItem(new MenuItem("MinERangeCombo", "Minimum range to E").SetValue(new Slider(350, 0, 600)));
            comboMenu.AddItem(new MenuItem("UseRCombo", "Use R Always").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseRComboKillable", "Use R Killable").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseRAoE", "Use R AoE ").SetValue(true));
            comboMenu.AddItem(new MenuItem("MinRTargets", "Minimum targets to R").SetValue(new Slider(3, 1, 5)));
            Config.AddSubMenu(comboMenu);

            var harassMenu = new Menu("Harass", "Harass");
            harassMenu.AddItem(new MenuItem("UseQHarass", "Use Q in Harass").SetValue(true));
            harassMenu.AddItem(new MenuItem("UseWHarass", "Use W in Harass").SetValue(true));
            harassMenu.AddItem(new MenuItem("UseEHarass", "Use E in Harass").SetValue(true));
            harassMenu.AddItem(new MenuItem("MinERangeHarass", "Minimum Range to E").SetValue(new Slider(350, 0, 600)));
            Config.AddSubMenu(harassMenu);

            var laneclearMenu = new Menu("Laneclear", "Laneclear");
            laneclearMenu.AddItem(new MenuItem("UseQLaneclear", "Use Q in Laneclear").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("UseWLaneclear", "Use W in Laneclear").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("UseELaneclear", "Use E in Laneclear").SetValue(true));
            Config.AddSubMenu(laneclearMenu);

            var drawingsMenu = new Menu("Drawings", "Drawings");            
            drawingsMenu.AddItem(new MenuItem("eRangeMin", "E Range Minimum").SetValue(new Circle(true, Color.FromArgb(100, 255, 255, 255))));
            drawingsMenu.AddItem(new MenuItem("eRangeMax", "E Range Maximum").SetValue(new Circle(true, Color.FromArgb(100, 255, 255, 255))));
            drawingsMenu.AddItem(new MenuItem("rRange", "R Range").SetValue(new Circle(true, Color.FromArgb(100, 255, 255, 255))));
            drawingsMenu.AddItem(new MenuItem("challenged", "Circle Challenged Target").SetValue(new Circle(true, Color.FromArgb(100, 255, 255, 255))));
            var dmgAfterCombo = new MenuItem("DamageAfterCombo", "Draw Damage After Combo").SetValue(true);
            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.DamageToUnit = ComboDamage;
            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = dmgAfterCombo.GetValue<bool>();
            drawingsMenu.AddItem(dmgAfterCombo);
            Config.AddSubMenu(drawingsMenu);

            Config.AddItem(new MenuItem("KillstealE", "Killsteal with E").SetValue(true));
            Config.AddItem(new MenuItem("KillstealR", "Killsteal with R").SetValue(true));
            Config.AddItem(new MenuItem("UseIgnite", "Ignite if Combo Killable").SetValue(true));
            Config.AddItem(new MenuItem("UseItems", "Use Items").SetValue(true));

            Config.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Orbwalking.OnAttack += onAttack;
            Orbwalking.BeforeAttack += OrbwalkingOnBeforeAttack;
            Obj_AI_Base.OnSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        // taken from honda
        private static float ComboDamage(AIHeroClient hero)
        {
            var dmg = 0d;

            if (Q.IsReady() || Player.GetSpell(SpellSlot.Q).State == SpellState.Surpressed)
            {
                dmg += Player.GetSpellDamage(hero, SpellSlot.Q) * 3;
                dmg += Player.BaseAttackDamage + Player.FlatPhysicalDamageMod * 3;
            }

            if (E.IsReady())
            {
                dmg += Player.GetSpellDamage(hero, SpellSlot.E);
            }

            if (R.IsReady())
            {
                dmg += Player.GetSpellDamage(hero, SpellSlot.R);
            }

            if (Player.Spellbook.CanUseSpell(Player.GetSpellSlot("summonerdot")) == SpellState.Ready)
            {
                dmg += Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            return (float)dmg;
        }


        private static void OnUpdate(EventArgs args)
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
                    Laneclear();
                    break;
            }
            Killsteal();
        }

        private static void Laneclear()
        {
            var minions =
                MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault(m => m.IsValid);

            if (minions == null)
                return;

            if (Config.Item("UseELaneclear").GetValue<bool>() && E.IsReady())
                E.CastOnUnit(minions);
        }

        private static void Combo()
        {
            var dist = Config.Item("MinERangeCombo").GetValue<Slider>().Value;
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            var eCombo = Config.Item("UseECombo").GetValue<bool>();
            var rCombo = Config.Item("UseRCombo").GetValue<bool>();
            var rComboKillable = Config.Item("UseRComboKillable").GetValue<bool>();
            var rComboAoE = Config.Item("UseRAoE").GetValue<bool>();
            var useIgnite = Config.Item("UseIgnite").GetValue<bool>();
            var useItems = Config.Item("UseItems").GetValue<bool>();

            if (target == null)
                return;

            if (E.IsReady() && Player.Distance(target.ServerPosition) >= dist && eCombo && target.IsValidTarget(E.Range))
            {
                E.Cast(target);
            }

            if (R.IsReady() && rCombo && target.IsValidTarget(R.Range))
            {
                if (target.HasBuff("xenzhaointimidate"))
                {
                    R.Cast();
                }
            }

            if (ComboDamage(target) > target.Health && R.IsReady() && rComboKillable && target.IsValidTarget(R.Range))
            {
                if (target.HasBuff("xenzhaointimidate"))
                {
                    R.Cast();
                }
            }

            if (R.IsReady() && rComboAoE)
            {
                // xsalice :v)
                foreach (var target1 in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range)))
                {
                    var poly = new Geometry.Polygon.Circle(Player.Position, R.Range);
                    var nearByEnemies = 1;
                    nearByEnemies +=
                        HeroManager.Enemies.Where(x => x.NetworkId != target1.NetworkId)
                            .Count(enemy => poly.IsInside(enemy));
                    if (nearByEnemies >= Config.Item("MinRTargets").GetValue<Slider>().Value)
                    {
                        R.Cast();
                    }
                }
            }

            if (Player.Distance(target.ServerPosition) <= 600 && ComboDamage(target) >= target.Health && useIgnite)
            {
                Player.Spellbook.CastSpell(ignite, target);
            }

            if (useItems && youmuu.IsReady() && target.IsValidTarget(E.Range)) 
            {
                youmuu.Cast();
            }

            if (useItems && Player.Distance(target.ServerPosition) <= 450 && cutlass.IsReady())
            {
                cutlass.Cast(target);
            }

            if (useItems && Player.Distance(target.ServerPosition) <= 450 && blade.IsReady())
            {
                blade.Cast(target);
            }
        }



           //if (E.IsReady() && Player.Distance(target2.ServerPosition) > E.Range)
            //{
                // minion shit  
               // not sure if actually works, fuck it
                //foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.Health))
                //{
                    //if (target.Distance(minion.ServerPosition) <= 100)
                    //{
                     //   E.Cast(minion);
                    //}
                //}
            //}
        
            // e if minion near enemy and enemy out of range - not done

        private static void Harass()
        {
            var dist = Config.Item("MinERangeHarass").GetValue<Slider>().Value;
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            var eHarass = Config.Item("UseEHarass").GetValue<bool>();

            if (target == null)
                return;

            if (E.IsReady() && Player.Distance(target.ServerPosition) >= dist && eHarass)
            {
                E.Cast(target);
            }
        }

        private static void Killsteal()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            var eKS = Config.Item("KillstealE").GetValue<bool>();
            var rKS = Config.Item("KillstealR").GetValue<bool>();

            if (target == null)
                return;

            if (E.IsReady() && E.GetDamage(target) >= target.Health && eKS)
            {
                E.Cast(target);
            }

            if (R.IsReady() && R.GetDamage(target) >= target.Health && rKS)
            {
                R.Cast(target);
            }
        }

        private static void OrbwalkingOnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!W.IsReady()) return;

            if (args.Target.Type == GameObjectType.AIHeroClient && Config.Item("UseWCombo").GetValue<bool>() || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && args.Target.Type == GameObjectType.obj_AI_Minion && Config.Item("UseWLaneclear").GetValue<bool>())
            {
                W.Cast();
            }
        }

        private static void onAttack(AttackableUnit unit, AttackableUnit target)
        {
            var qCombo = Config.Item("UseQCombo").GetValue<bool>();
            var qHarass = Config.Item("UseQHarass").GetValue<bool>();
            var useItems = Config.Item("UseItems").GetValue<bool>();
            var aaDelay = Player.AttackDelay * 100 + Game.Ping / 2f;


            if (target == null)
                return;

            if (!unit.IsMe)
                return;

            // badao
            if (!target.Name.ToLower().Contains("minion") && !target.Name.ToLower().Contains("sru") && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Q.IsReady() && qCombo)
                {
                    // chewy
                    LeagueSharp.Common.Utility.DelayAction.Add(
                       (int)(aaDelay), () =>
                       {
                           Q.Cast();

                           if (Items.CanUseItem(3074) && useItems && Player.Distance(target) <= 400)
                               Items.UseItem(3074);

                           if (Items.CanUseItem(3077) && useItems && Player.Distance(target) <= 400)
                               Items.UseItem(3077);
                       });

                    
                }
            }

            // badao
            if (!target.Name.ToLower().Contains("minion") && !target.Name.ToLower().Contains("sru") && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (Q.IsReady() && qHarass)
                {
                    // chewy
                    LeagueSharp.Common.Utility.DelayAction.Add(
                       (int)(aaDelay), () =>
                       {
                           Q.Cast();
                       });
                }
            }

            if (Config.Item("UseQLaneclear").GetValue<bool>() && target.Type == GameObjectType.obj_AI_Minion && Q.IsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                LeagueSharp.Common.Utility.DelayAction.Add((int)(aaDelay), () => Q.Cast());
        }
               

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "XenZhaoComboTarget")
            {
                LeagueSharp.Common.Utility.DelayAction.Add(0, Orbwalking.ResetAutoAttackTimer); 
            }
        }

        
        private static void OnDraw(EventArgs args)
        {            
            var dist = Config.Item("MinERangeCombo").GetValue<Slider>().Value;
            var eRangeMin = Config.Item("eRangeMin").GetValue<Circle>();
            var eRangeMax = Config.Item("eRangeMax").GetValue<Circle>();
            var rRange = Config.Item("rRange").GetValue<Circle>();
            var challenged = Config.Item("challenged").GetValue<Circle>();
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);


            if (eRangeMin.Active)
            {
                Render.Circle.DrawCircle(Player.Position, dist, eRangeMin.Color);
            }

            if (eRangeMax.Active)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, eRangeMax.Color);
            }

            if (rRange.Active)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, rRange.Color);
            }

            if (target == null)
                return;

            if (target.HasBuff("xenzhaointimidate"))
            {
                Render.Circle.DrawCircle(target.Position, 100, challenged.Color);
            }
        }
    }
}
