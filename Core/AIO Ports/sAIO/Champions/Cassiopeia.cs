using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using sAIO.Core;

using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace sAIO.Champions
{
    public class Cassiopeia : Helper
    {
        private static int lastQ = 0, lastE = 0;
        public Cassiopeia()
        {
            Cassiopeia_OnGameLoad();
        }
        static void Cassiopeia_OnGameLoad()
        {
            Q = new Spell(SpellSlot.Q, 850f);
            W = new Spell(SpellSlot.W, 850f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 825f);
            Q.SetSkillshot(0.6f, 75f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.5f, 90f, 2500, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.3f, (float)(80 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);

            
            menu.AddSubMenu(new Menu("Combo", "Combo"));
            CreateMenuBool("Combo", "Combo.Q", "Use Q", true);
            CreateMenuBool("Combo", "Combo.W", "Use W", true);
            CreateMenuBool("Combo", "Combo.E", "Use E", true);
            CreateMenuBool("Combo", "Combo.R", "Use R", true);
            CreateMenuSlider("Combo", "Combo.EDelay", "Delay E", 0, 900, 2000);

            menu.AddSubMenu(new Menu("Harass", "Harass"));
            CreateMenuBool("Harass", "Harass.Q", "Use Q", true);
            CreateMenuBool("Harass", "Harass.W", "Use E", true);
            CreateMenuBool("Harass", "Harass.E", "Use E", true);
            CreateMenuSlider("Harass", "Harass.MinManaPercent", "Min Mana Percent To Use", 0, 15, 100);
            CreateMenuSlider("Harass", "Harass.EDelay", "Delay E", 0, 900, 2000);

            menu.AddSubMenu(new Menu("Gap closer", "GC"));
            CreateMenuBool("GC", "GC.W", "Use W", true);

            menu.AddSubMenu(new Menu("Kill Steal", "KS"));
            CreateMenuBool("KS", "KS.Q", "Use Q", true);
            CreateMenuBool("KS", "KS.W", "Use E", true);
            CreateMenuBool("KS", "KS.E", "Use E", true);

            menu.AddSubMenu(new Menu("Farm", "Farm"));
            CreateMenuBool("Farm", "Farm.Q", "Use Q", true);
            CreateMenuBool("Farm", "Farm.W", "Use W", true);
            CreateMenuBool("Farm", "Farm.E", "Use E", true);
            CreateMenuSlider("Farm", "Farm.MinManaPercent", "Min Mana Percent To Use", 0, 15, 100);

            menu.AddSubMenu(new Menu("Lane Clean", "LC"));
            CreateMenuBool("LC", "LC.Q", "Use Q", true);
            CreateMenuBool("LC", "LC.W", "Use W", true);
            CreateMenuBool("LC", "LC.E", "Use E", true);
            CreateMenuSlider("LC", "LC.MinManaPercent", "Min Mana Percent To Use", 0, 15, 100);

            menu.AddSubMenu(new Menu("Drawing", "Draw"));
            CreateMenuBool("Draw", "Draw.Q", "Draw Q & W", true);            
            CreateMenuBool("Draw", "Draw.E", "Draw E", true);
            CreateMenuBool("Draw", "Draw.R", "Draw R", true);
            CreateMenuBool("Draw", "Draw.CBDamage", "Draw Combo Damage", true);
            menu.SubMenu("Draw").AddItem(new MenuItem("Draw.DrawColor", "Fill color").SetValue(new Circle(true, Color.FromArgb(204, 255, 0, 1))));

            //DrawDamage.DamageToUnit = GetComboDamage;
            //DrawDamage.Enabled = GetValueMenuBool("Draw.CBDamage");
            //DrawDamage.Fill = menu.Item("Draw.DrawColor").GetValue<Circle>().Active;
            //DrawDamage.FillColor = menu.Item("Draw.DrawColor").GetValue<Circle>().Color;

            menu.Item("Draw.CBDamage").ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                //DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
            };

            menu.Item("Draw.DrawColor").ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                //DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                //DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
            menu.AddToMainMenu();                    


            Game.OnUpdate += Game_OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;

            Chat.Print("sAIO: " + player.ChampionName + " loaded");

        }

        static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == E.Instance.SData.Name)
                    lastE = Environment.TickCount;

                if (args.SData.Name == Q.Instance.SData.Name)
                    lastQ = Environment.TickCount;
                
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (GetValueMenuBool("Draw.Q"))
                Drawing.DrawCircle(player.Position, Q.Range, Color.Blue);

            if (GetValueMenuBool("Draw.E"))
                Drawing.DrawCircle(player.Position, E.Range, Color.Green);

            if (GetValueMenuBool("Draw.R"))
                Drawing.DrawCircle(player.Position, R.Range, Color.Red);
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
                return;

            if (GetValueMenuBool("GC.W"))
            {
                if (W.IsReady() && W.IsInRange(gapcloser.Sender))
                    W.Cast(gapcloser.Sender);
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo: Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed: Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear: LaneClean();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit: Farm();
                    break;
            }

            KillSteal();
        }
        static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var eDelay = GetValueMenuSlider("Combo.EDelay");

            if (target != null)
            {
                if(Q.IsReady() && Q.IsInRange(target) && GetValueMenuBool("Combo.Q"))
                {
                    
                        var qHitChance = Q.GetPrediction(target);

                        if (qHitChance.Hitchance >= HitChance.High)
                            Q.Cast(qHitChance.CastPosition);
                    
                    
                }

                if (W.IsReady() && W.IsInRange(target) && GetValueMenuBool("Combo.W") && Environment.TickCount - lastQ > (Q.Delay * 1000))//Humanzier
                {
                    var wHitChance = W.GetPrediction(target);

                    if (wHitChance.Hitchance >= HitChance.High)
                        W.Cast(wHitChance.CastPosition);
                }

                if (E.IsReady() && E.IsInRange(target) && GetValueMenuBool("Combo.E"))
                {
                    if(target.HasBuffOfType(BuffType.Poison))
                    {
                        if (Environment.TickCount - lastE >= eDelay)
                        {
                            E.CastOnUnit(target);
                            
                        }
                    }

                }

                if (R.IsReady() && R.IsInRange(target) && GetValueMenuBool("Combo.R"))
                    R.Cast(target);
            }
        }
        static void Harass()
        {
             var minManaToHarass = GetValueMenuSlider("Harass.MinManaPercent");

            if ((int)player.ManaPercent <= minManaToHarass)
                return;

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var eDelay = GetValueMenuSlider("Harass.EDelay");

            if (target != null)
            {
                if (Q.IsReady() && Q.IsInRange(target) && GetValueMenuBool("Harass.Q"))
                {
                    var qHitChance = Q.GetPrediction(target);

                    if (qHitChance.Hitchance >= HitChance.High)
                        Q.Cast(qHitChance.CastPosition);
                   
                }

                if (W.IsReady() && W.IsInRange(target) && GetValueMenuBool("Harass.W") && Environment.TickCount - lastQ > (Q.Delay * 1000))
                {
                    var wHitChance = W.GetPrediction(target);

                    if (wHitChance.Hitchance >= HitChance.High)
                        W.Cast(wHitChance.CastPosition);
                }

                if (E.IsReady() && E.IsInRange(target) && GetValueMenuBool("Harass.E"))
                {
                    if (target.HasBuffOfType(BuffType.Poison))
                    {
                        if (Environment.TickCount - lastE >= eDelay)
                        {
                            E.CastOnUnit(target);
                            
                        }
                    }

                }
            }
        }
        static void Farm()
        {
            var minManaToUse = GetValueMenuSlider("Farm.MinManaPercent");

            if ((int)player.ManaPercent <= minManaToUse)
                return;

            var minions = MinionManager.GetMinions(player.Position, E.Range);

            if (minions.Count == 0)
                return;

            var posQFarm = Q.GetCircularFarmLocation(minions);
            var posWFarm = W.GetCircularFarmLocation(minions);

            if (GetValueMenuBool("Farm.Q") && Q.IsReady() && posQFarm.MinionsHit > 1)
                Q.Cast(posQFarm.Position);

            if (GetValueMenuBool("Farm.W") && W.IsReady() && posWFarm.MinionsHit > 1)
                W.Cast(posWFarm.Position);

            if(GetValueMenuBool("Farm.E") && E.IsReady())
            {
                var eMinion = minions.Where(m => player.Distance(m.Position) < E.Range && m.Health < (E.GetDamage(m) * 0.9))
                    .OrderByDescending(m => m.Health);

               foreach(var minion in eMinion)
               {
                   if (minion.IsValidTarget())
                       E.CastOnUnit(minion);
               }
            }
        }
        static void LaneClean()
        {
            var minManaToUse = GetValueMenuSlider("LC.MinManaPercent");

            if ((int)player.ManaPercent <= minManaToUse)
                return;

            var minions = MinionManager.GetMinions(player.Position, E.Range);

            if (minions.Count == 0)
                return;

            var posQFarm = Q.GetCircularFarmLocation(minions);
            var posWFarm = W.GetCircularFarmLocation(minions);

            if (GetValueMenuBool("LC.Q") && Q.IsReady() && posQFarm.MinionsHit > 1)
                Q.Cast(posQFarm.Position);

            if (GetValueMenuBool("LC.W") && W.IsReady() && posWFarm.MinionsHit > 1)
                W.Cast(posWFarm.Position);

            if (GetValueMenuBool("LC.E") && E.IsReady())
            {
                var eMinion = minions.Where(m => player.Distance(m.Position) < E.Range && m.Health < (E.GetDamage(m) * 0.9))
                    .OrderByDescending(m => m.Health);

                foreach (var minion in eMinion)
                {
                    if (minion.IsValidTarget())
                        E.CastOnUnit(minion);
                }
            }
        }
        static void KillSteal()
        {
            foreach (var enemy in HeroManager.Enemies.Where(e => player.Distance(e.Position) < W.Range))
            {
                if (enemy == null) return;

                var qDamage = Q.GetDamage(enemy) * 0.9;
                var wDamage = W.GetDamage(enemy) * 0.9;
                var eDamage = E.GetDamage(enemy) * 0.9;

                if (GetValueMenuBool("KS.Q") && Q.IsReady() && Q.IsInRange(enemy) && enemy.Health < qDamage)
                {
                    var qHitChance = Q.GetPrediction(enemy);

                    if (qHitChance.Hitchance >= HitChance.High)
                        Q.Cast(qHitChance.CastPosition);
                    
                }

                if (GetValueMenuBool("KS.W") && W.IsReady() && W.IsInRange(enemy) && enemy.Health < wDamage)
                {
                    var wHitChance = W.GetPrediction(enemy);

                    if (wHitChance.Hitchance >= HitChance.High)
                        W.Cast(wHitChance.CastPosition);
                }

                if (GetValueMenuBool("KS.E") && E.IsReady() && E.IsInRange(enemy) && enemy.Health < eDamage)
                    E.CastOnUnit(enemy);
            }
        }
        static float GetComboDamage(Obj_AI_Base target)
        {
            float damage = 0f;

            if (Q.IsReady() && GetValueMenuBool("Combo.Q"))
                damage += Q.GetDamage(target);

            if (W.IsReady() && GetValueMenuBool("Combo.W"))
                damage += W.GetDamage(target);

            if (E.IsReady() && GetValueMenuBool("Combo.E"))
                damage += E.GetDamage(target);

            if (R.IsReady() && GetValueMenuBool("Combo.R"))
                damage += R.GetDamage(target);

            return damage;
        }
    }
}
