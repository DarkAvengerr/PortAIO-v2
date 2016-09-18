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
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace sAIO.Champions
{
    class Darius : Helper
    {
        
         public Darius()
        {
            Darius_OnGameLoad();
        }
        static void Darius_OnGameLoad()
        {
            Q = new Spell(SpellSlot.Q, 420f);
            W = new Spell(SpellSlot.W, 145f);
            E = new Spell(SpellSlot.E, 540f);
            R = new Spell(SpellSlot.R, 460f);
            E.SetSkillshot(0.3f, 80, int.MaxValue, false, SkillshotType.SkillshotCone);
            Q.SetSkillshot(0.75f, 42.5f, int.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetTargetted(1.25f, int.MaxValue);

            menu.AddSubMenu(new Menu("Combo", "Combo"));
            CreateMenuBool("Combo", "Combo.Q", "Use Q", true);
            CreateMenuBool("Combo", "Combo.W", "Use W", true);
            CreateMenuBool("Combo", "Combo.E", "Use E", true);
            CreateMenuBool("Combo", "Combo.R", "Use R", true);
            CreateMenuBool("Combo", "Combo.RK", "Use R if killable", true);

            menu.AddSubMenu(new Menu("Harass", "Harass"));
            CreateMenuBool("Harass", "Harass.Q", "Use Q", true);
            CreateMenuBool("Harass", "Harass.W", "Use W", true);
            CreateMenuBool("Harass", "Harass.E", "Use E", true);
            CreateMenuSlider("Harass", "Harass.MinManaPercent", "Min Mana Percent To Use", 0, 15, 100);

            menu.AddSubMenu(new Menu("Items", "Items"));
            CreateMenuBool("Items", "Items.Cutlass", "Use Cutlass", true);
            CreateMenuBool("Items", "Items.Blade", "Use Blade of the Ruined King", true);
            CreateMenuBool("Items", "Items.Youmuu", "Use Youmuu", true);

            menu.AddSubMenu(new Menu("Interrupter", "Interrupter"));
            CreateMenuBool("Interrupter", "Interrupter.E", "Use E", true);
            

            menu.AddSubMenu(new Menu("Kill Steal", "KS"));
            CreateMenuBool("KS", "KS.Q", "Use Q", true);
            CreateMenuBool("KS", "KS.R", "Use R", false);

            menu.AddSubMenu(new Menu("Farm", "Farm"));
            CreateMenuBool("Farm", "Farm.Q", "Use Q", true);
            
            
            CreateMenuSlider("Farm", "Farm.MinManaPercent", "Min Mana Percent To Use", 0, 15, 100);

            menu.AddSubMenu(new Menu("Lane Clean", "LC"));
            CreateMenuBool("LC", "LC.Q", "Use Q", true);
 
            CreateMenuSlider("LC", "LC.MinManaPercent", "Min Mana Percent To Use", 0, 15, 100);

            menu.AddSubMenu(new Menu("Drawing", "Draw"));
            CreateMenuBool("Draw", "Draw.Q", "Draw Q ", true);
            CreateMenuBool("Draw", "Draw.E", "Draw E", true);
            CreateMenuBool("Draw", "Draw.R", "Draw R", true);
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
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if(sender.IsMe)
            {
                if (args.SData.Name == R.Instance.SData.Name)
                {
                    
                    Orbwalking.ResetAutoAttackTimer();
                }
                    
            }
        }

        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsInRange(sender) && GetValueMenuBool("Interrupter.E") && E.IsReady())
            {
                E.Cast(sender);
            }
        }

        static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (W.IsReady() && (GetValueMenuBool("Combo.W") || GetValueMenuBool("Harass.W")) && W.IsInRange(target))
                    W.Cast();

                UseItems((AIHeroClient)target);
            }
            
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (GetValueMenuBool("Draw.Q"))
                Drawing.DrawCircle(player.Position, Q.Range, Color.Green);

            if (GetValueMenuBool("Draw.E"))
                Drawing.DrawCircle(player.Position, E.Range, Color.Wheat);

            if (GetValueMenuBool("Draw.R"))
                Drawing.DrawCircle(player.Position, R.Range, Color.Red);
        }

        static void Game_OnUpdate(EventArgs args)
        {
            switch(orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    Farm();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClean();
                    break;
            }

            KillSteal();
        }
        static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if (E.IsInRange(target) && E.IsReady() && GetValueMenuBool("Combo.E"))
                E.Cast(target);

            if (Q.IsInRange(target) && Q.IsReady() && GetValueMenuBool("Combo.Q"))
                Q.Cast();

            if (R.IsReady() && R.IsInRange(target) && GetValueMenuBool("Combo.R"))
            {
                foreach (var hero in
                                  ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsValidTarget(R.Range)))
                {
                    if (!GetValueMenuBool("Combo.RK"))
                    {
                        R.CastOnUnit(target);
                    }

                    else if (player.GetSpellDamage(target, SpellSlot.R) < hero.Health && GetValueMenuBool("Combo.RK"))
                    {
                        foreach (var buff in hero.Buffs.Where(buff => buff.Name == "dariushemo"))
                        {
                            if (player.GetSpellDamage(target, SpellSlot.R, 1) * (1 + buff.Count / 5) - 1 > target.Health)
                            {
                                R.CastOnUnit(target);
                            }
                        }
                    }
                }
            }
        }
        static void Harass()
        {
            if ((int)player.Mana < GetValueMenuSlider("Harass.MinManaPercent"))
                return;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if (E.IsInRange(target) && E.IsReady() && GetValueMenuBool("Combo.E"))
                E.Cast(target);

            if (Q.IsInRange(target) && Q.IsReady() && GetValueMenuBool("Combo.Q"))
                Q.Cast();
        }
        static void Farm()
        {
            if ((int)player.Mana < GetValueMenuSlider("Farm.MinManaPercent"))
                return;

            var minions = MinionManager.GetMinions(player.Position, Q.Range);

            var pos = Q.GetCircularFarmLocation(minions);

            if (Q.IsReady() && pos.MinionsHit > 1 && GetValueMenuBool("Farm.Q") && player.Distance(pos.Position) < player.AttackRange)
                Q.Cast();
        }
        static void LaneClean()
        {
            if ((int)player.Mana < GetValueMenuSlider("Harass.MinManaPercent"))
                return;

            var minions = MinionManager.GetMinions(player.Position, Q.Range);

            if (minions.Count < 1)
                return;

            Q.Cast();
        }
        static void KillSteal()
        {
            foreach(var enemy in HeroManager.Enemies.Where(e => player.Distance(e.Position) < R.Range))
            {
                var qDamage = Q.GetDamage(enemy) * 0.9;
                var rDamage = R.GetDamage(enemy) * 0.9;

                if (enemy.Health <= qDamage && Q.IsInRange(enemy) && Q.IsReady() && GetValueMenuBool("KS.Q"))
                    Q.Cast();

                if (enemy.Health <= rDamage && R.IsInRange(enemy) && R.IsReady() && GetValueMenuBool("KS.R"))
                    R.CastOnUnit(enemy);
            }
        }
        static void UseItems(AIHeroClient target)
        {
            var bladeOTRKing = ItemData.Blade_of_the_Ruined_King.GetItem();
            var cutLass = ItemData.Bilgewater_Cutlass.GetItem();
            var youmuu = ItemData.Youmuus_Ghostblade.GetItem();

            if (bladeOTRKing.IsOwned() && bladeOTRKing.IsReady() && bladeOTRKing.IsInRange(target) && GetValueMenuBool("Items.Blade"))
                bladeOTRKing.Cast(target);

            if (cutLass.IsOwned() && cutLass.IsReady() && cutLass.IsInRange(target) && GetValueMenuBool("Items.Cutlass"))
                cutLass.Cast(target);

            if (youmuu.IsOwned() && youmuu.IsReady() && GetValueMenuBool("Items.Youmuu") && youmuu.IsInRange(target))
                youmuu.Cast();
        }
        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Q.IsReady())
            {
                damage += player.GetSpellDamage(enemy, SpellSlot.Q);
            }

            if (W.IsReady())
            {
                damage += player.GetSpellDamage(enemy, SpellSlot.W);
            }

            if (E.IsReady())
            {
                damage += player.GetSpellDamage(enemy, SpellSlot.E);
            }

            if (R.IsReady())
            {
                

                damage +=
                    enemy.Buffs.Where(buff => buff.Name == "dariushemo")
                        .Sum(buff => player.GetSpellDamage(enemy, SpellSlot.R, 1) * (1 + buff.Count / 5) - 1);
            }

            return (float)damage;
        }
    }
}
