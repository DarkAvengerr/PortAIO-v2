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
    public class Ryze : Helper
    {
        public Ryze()
        {
            Ryze_OnGameLoad();
        }
        static void Ryze_OnGameLoad()
        {
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R);
            Q.SetSkillshot(250f, Q.Instance.SData.LineWidth, Q.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);

            
            menu.AddSubMenu(new Menu("Combo", "Combo"));
            CreateMenuBool("Combo", "Combo.Q", "Use Q", true);
            CreateMenuBool("Combo", "Combo.W", "Use W", true);
            CreateMenuBool("Combo", "Combo.E", "Use E", true);
            CreateMenuBool("Combo", "Combo.R", "Use R", true);

            menu.AddSubMenu(new Menu("Harass", "Harass"));
            CreateMenuBool("Harass", "Harass.Q", "Use Q", true);
            CreateMenuBool("Harass", "Harass.W", "Use W", true);
            CreateMenuBool("Harass", "Harass.E", "Use E", true);
            CreateMenuSlider("Harass", "Harass.MinManaPercent", "Min Mana Percent To Use", 0, 15, 100);

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
            CreateMenuBool("Draw", "Draw.Q", "Draw Q", true);
            CreateMenuBool("Draw", "Draw.W", "Draw W", true);
            CreateMenuBool("Draw", "Draw.E", "Draw E", true);
            CreateMenuBool("Draw", "Draw.CBDamage", "Draw Combo Damage", true);
            menu.SubMenu("Draw").AddItem(new MenuItem("Draw.DrawColor", "Fill color").SetValue(new Circle(true, Color.FromArgb(204, 255, 0, 1))));

            DrawDamage.DamageToUnit = GetComboDamage;
            DrawDamage.Enabled = GetValueMenuBool("Draw.CBDamage");
            DrawDamage.Fill = menu.Item("Draw.DrawColor").GetValue<Circle>().Active;
            DrawDamage.FillColor = menu.Item("Draw.DrawColor").GetValue<Circle>().Color;

            menu.Item("Draw.CBDamage").ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
            };

            menu.Item("Draw.DrawColor").ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
            menu.AddToMainMenu();                    

            Game.OnUpdate += Game_OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;

            Chat.Print("sAIO: " + player.ChampionName + " loaded");

        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (GetValueMenuBool("Draw.Q"))
                Drawing.DrawCircle(player.Position, Q.Range, Color.Blue);

            if (GetValueMenuBool("Draw.W"))
                Drawing.DrawCircle(player.Position, W.Range, Color.Red);

            if (GetValueMenuBool("Draw.E"))
                Drawing.DrawCircle(player.Position, E.Range, Color.Green);
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
                return;

            if(GetValueMenuBool("GC.W"))
            {
                if (W.IsReady() && W.IsInRange(gapcloser.Sender))
                    W.CastOnUnit(gapcloser.Sender);
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
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if(target != null)
            {
                if (player.Buffs.Count(b => b.Name == "RyzePassiveStack") < 4)
                {
                    if (GetValueMenuBool("Combo.R") && R.IsReady())
                        R.Cast();

                    if (GetValueMenuBool("Combo.E") && E.IsReady() && E.IsInRange(target))
                        E.CastOnUnit(target);

                    if (GetValueMenuBool("Combo.Q") && Q.IsReady() && Q.IsInRange(target))
                    {
                        var qHitChance = Q.GetPrediction(target);

                        if (qHitChance.Hitchance >= HitChance.High && qHitChance.CollisionObjects.Count < 1)
                            Q.Cast(qHitChance.CastPosition);
                    }

                    if (GetValueMenuBool("Combo.W") && W.IsReady() && W.IsInRange(target))
                        W.CastOnUnit(target);
                }

                if (player.Buffs.Count(b => b.Name == "RyzePassiveStack") >= 4)
                {
                    if (GetValueMenuBool("Combo.W") && W.IsReady() && W.IsInRange(target))
                        W.CastOnUnit(target);

                    if (GetValueMenuBool("Combo.Q") && Q.IsReady() && Q.IsInRange(target))
                    {
                        var qHitChance = Q.GetPrediction(target);

                        if (qHitChance.Hitchance >= HitChance.High && qHitChance.CollisionObjects.Count < 1)
                            Q.Cast(qHitChance.CastPosition);
                    }

                    if (GetValueMenuBool("Combo.R") && R.IsReady())
                        R.Cast();

                    if (GetValueMenuBool("Combo.E") && E.IsReady() && E.IsInRange(target))
                        E.CastOnUnit(target);
                }
            }           
        }
        static void Harass()
        {
            var minManaToHarass = GetValueMenuSlider("Harass.MinManaPercent");

            if ((int)player.ManaPercent <= minManaToHarass)
                return;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target != null)
            {
                if (GetValueMenuBool("Harass.Q") && Q.IsReady() && Q.IsInRange(target))
                {
                    var qHitChance = Q.GetPrediction(target);

                    if (qHitChance.Hitchance >= HitChance.High && qHitChance.CollisionObjects.Count < 1)
                        Q.Cast(qHitChance.CastPosition);
                }

                if (GetValueMenuBool("Harass.W") && W.IsReady() && W.IsInRange(target))
                    W.CastOnUnit(target);

                if (GetValueMenuBool("Harass.E") && E.IsReady() && E.IsInRange(target))
                    E.CastOnUnit(target);
            }            
        }
        static void Farm()
        {
            var minManaToUse = GetValueMenuSlider("Farm.MinManaPercent");

            if ((int)player.ManaPercent <= minManaToUse)
                return;

            var minions = MinionManager.GetMinions(player.Position, W.Range);

            if (minions.Count == 0)
                return;

            if (Q.IsReady() && GetValueMenuBool("Farm.Q"))
            {
                var qMinions = MinionManager.GetMinions(player.Position, Q.Range)
                    .Where(m => m.Health < (Q.GetDamage(m) * 0.9) && Q.GetPrediction(m).Hitchance >= HitChance.High);

                foreach(var minion in qMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        Q.Cast(minion);
                    }
                }                
            }

            if (W.IsReady() && GetValueMenuBool("Farm.W"))
            {
                var wMinion = MinionManager.GetMinions(player.Position, W.Range).Where(m => m.Health < (W.GetDamage(m) * 0.9));

                foreach(var minion in wMinion)
                {
                    if (minion.IsValidTarget())
                        W.CastOnUnit(minion);
                }               
            }

            if (E.IsReady() && GetValueMenuBool("Farm.E"))
            {
                var eMinions = MinionManager.GetMinions(player.Position, E.Range).Where(m => m.Health < (E.GetDamage(m) * 0.9));

                foreach(var minion in eMinions)
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

            var minions = MinionManager.GetMinions(player.Position, W.Range);

            if (minions.Count == 0)
                return;

            if (Q.IsReady() && GetValueMenuBool("LC.Q"))
            {
                var qMinions = MinionManager.GetMinions(player.Position, Q.Range)
                    .Where(m => m.Health < (Q.GetDamage(m) * 0.9) && Q.GetPrediction(m).Hitchance >= HitChance.High);

                foreach (var minion in qMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        Q.Cast(minion);
                    }
                }
            }

            if (W.IsReady() && GetValueMenuBool("LC.W"))
            {
                var wMinion = MinionManager.GetMinions(player.Position, W.Range).Where(m => m.Health < (W.GetDamage(m) * 0.9));

                foreach (var minion in wMinion)
                {
                    if (minion.IsValidTarget())
                        W.CastOnUnit(minion);
                }
            }

            if (E.IsReady() && GetValueMenuBool("LC.E"))
            {
                var eMinions = MinionManager.GetMinions(player.Position, E.Range).Where(m => m.Health < (E.GetDamage(m) * 0.9));

                foreach (var minion in eMinions)
                {
                    if (minion.IsValidTarget())
                        E.CastOnUnit(minion);
                }
            }
        }
        static void KillSteal()
        {
            foreach(var enemy in HeroManager.Enemies.Where(e => player.Distance(e.Position) < W.Range))
            {
                if (enemy == null) return;

                var qDamage = Q.GetDamage(enemy) * 0.9;
                var wDamage = W.GetDamage(enemy) * 0.9;
                var eDamage = E.GetDamage(enemy) * 0.9;

                if (GetValueMenuBool("KS.Q") && Q.IsReady() && Q.IsInRange(enemy) && enemy.Health < qDamage)
                {
                    var qHitChance = Q.GetPrediction(enemy);

                    if (qHitChance.Hitchance >= HitChance.High && qHitChance.CollisionObjects.Count < 1)
                        Q.Cast(qHitChance.CastPosition);
                }

                if (GetValueMenuBool("KS.W") && W.IsReady() && W.IsInRange(enemy) && enemy.Health < wDamage)
                    W.CastOnUnit(enemy);

                if (GetValueMenuBool("KS.E") && E.IsReady() && E.IsInRange(enemy) && enemy.Health < eDamage)
                    E.CastOnUnit(enemy);
            }
        }
        static float GetComboDamage(AIHeroClient target)
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
