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
    class Katarina : Helper
    {
        private static int _lastPlaced;
        private static Vector3 _lastWardPos;
        static int lastE = 0, lastQ = 0;

        public Katarina()
        {
            Katarina_OnGameLoad();
        }
        static void Katarina_OnGameLoad()
        {
            Q = new Spell(SpellSlot.Q, 675f);
            W = new Spell(SpellSlot.W, 375f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 550f);


            menu.AddSubMenu(new Menu("Combo", "Combo"));
            menu.SubMenu("Combo").AddItem(new MenuItem("Combo.Mode", "Mode").SetValue(new StringList(new[] { "Q+E+W+R", "E+Q+W+R" })));
            CreateMenuBool("Combo", "Combo.Q", "Use Q", true);
            CreateMenuBool("Combo", "Combo.W", "Use W", true);
            CreateMenuBool("Combo", "Combo.E", "Use E", true);
            CreateMenuBool("Combo", "Combo.R", "Use R", true);
            menu.SubMenu("Combo").AddItem(new MenuItem("Combo.EDelay", "E delay").SetValue(new Slider(0, 0, 1000)));
            

            menu.AddSubMenu(new Menu("Harass", "Harass"));
            menu.SubMenu("Harass").AddItem(new MenuItem("Harass.Mode", "Mode").SetValue(new StringList(new[] { "Q+E+W", "E+Q+W" })));
            CreateMenuBool("Harass", "Harass.Q", "Use Q", true);
            CreateMenuBool("Harass", "Harass.W", "Use E", true);
            CreateMenuBool("Harass", "Harass.E", "Use E", true);
            menu.SubMenu("Harass").AddItem(new MenuItem("Harass.EDelay", "E delay").SetValue(new Slider(0, 0, 1000)));
            

            menu.AddSubMenu(new Menu("Gap closer", "GC"));
            CreateMenuBool("GC", "GC.W", "Use W", true);

            menu.AddSubMenu(new Menu("Kill Steal", "KS"));
            CreateMenuBool("KS", "KS.Q", "Use Q", true);
            CreateMenuBool("KS", "KS.W", "Use E", true);
            CreateMenuBool("KS", "KS.E", "Use E", false);

            menu.AddSubMenu(new Menu("Farm", "Farm"));
            CreateMenuBool("Farm", "Farm.Q", "Use Q", true);
            CreateMenuBool("Farm", "Farm.W", "Use W", true);
            CreateMenuBool("Farm", "Farm.E", "Use E", true);
            

            menu.AddSubMenu(new Menu("Lane Clean", "LC"));
            CreateMenuBool("LC", "LC.Q", "Use Q", true);
            CreateMenuBool("LC", "LC.W", "Use W", true);
            CreateMenuBool("LC", "LC.E", "Use E", true);
            

            menu.AddSubMenu(new Menu("Drawing", "Draw"));
            CreateMenuBool("Draw", "Draw.Q", "Draw Q & W", true);
            CreateMenuBool("Draw", "Draw.E", "Draw E", true);
            CreateMenuBool("Draw", "Draw.R", "Draw R", true);
            CreateMenuBool("Draw", "Draw.CBDamage", "Draw Combo Damage", true);
            menu.SubMenu("Draw").AddItem(new MenuItem("Draw.DrawColor", "Fill color").SetValue(new Circle(true, Color.FromArgb(204, 255, 0, 1))));

            menu.AddSubMenu(new Menu("Ward jump", "WJ"));
            CreateMenuKeyBind("WJ", "WJ.Key", "Key", 'G', KeyBindType.Press);

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
            GameObject.OnCreate+=GameObject_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            new AssassinManager();
            Chat.Print("sAIO: " + player.ChampionName + " loaded");

        }

        static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if(sender.IsMe)
            {
                if (args.SData.Name == E.Instance.SData.Name)
                    lastE = Environment.TickCount;

                if (args.SData.Name == Q.Instance.SData.Name)
                    lastQ = Environment.TickCount;

                if (player.IsChannelingImportantSpell() || player.HasBuff("KatarinaR") || player.HasBuff("katarinarsound",true))
                {
                    orbwalker.SetAttack(false);
                    orbwalker.SetMovement(false);
                }
                else
                {
                    orbwalker.SetAttack(true);
                    orbwalker.SetMovement(true);
                }
            }
        }
        private static InventorySlot GetBestWardSlot()
        {
            var slot = Items.GetWardSlot();
            if (slot == default(InventorySlot))
            {
                return null;
            }
            return slot;

        }
        static void WardJump()
        {
            if (Environment.TickCount <= _lastPlaced + 3000 || !E.IsReady())
                return;

            Vector3 cursorPos = Game.CursorPos;
            Vector3 myPos = player.ServerPosition;
            Vector3 delta = cursorPos - myPos;

            delta.Normalize();

            Vector3 wardPosition = myPos + delta * (600 - 5);
            InventorySlot invSlot = GetBestWardSlot();

            if (invSlot == null)
                return;

            Items.UseItem((int)invSlot.Id, wardPosition);
            _lastWardPos = wardPosition;
            _lastPlaced = Environment.TickCount;

            E.Cast();
        }
        static void Drawing_OnDraw(EventArgs args)
        {
            if (GetValueMenuBool("Draw.Q"))
                Drawing.DrawCircle(player.Position, Q.Range, Color.Blue);

            if (GetValueMenuBool("Draw.W"))
                Drawing.DrawCircle(player.Position, Q.Range, Color.YellowGreen);

            if (GetValueMenuBool("Draw.E"))
                Drawing.DrawCircle(player.Position, E.Range, Color.Green);

            if (GetValueMenuBool("Draw.R"))
                Drawing.DrawCircle(player.Position, R.Range, Color.Red);
        }

        static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!E.IsReady() || !(sender is Obj_AI_Minion) || Environment.TickCount >= _lastPlaced + 300)
                return;

            if (Environment.TickCount >= _lastPlaced + 300) return;
            var ward = (Obj_AI_Minion)sender;

            if (ward.Name.ToLower().Contains("ward") && ward.Distance(_lastWardPos) < 500)
            {
                E.Cast(ward);
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            if (player.IsChannelingImportantSpell() || player.HasBuff("KatarinaR") || player.HasBuff("katarinarsound", true))
            {
                orbwalker.SetAttack(false);
                orbwalker.SetMovement(false);
                return;
            }
            else
            {
                orbwalker.SetAttack(true);
                orbwalker.SetMovement(true);
            }

            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo: Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed: Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit: Farm();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear: LaneClear();
                    break;
            }

            KillSteal();
            var wardjump = menu.Item("WJ.Key").GetValue<KeyBind>().Active;

            if (wardjump)
                WardJump();
        }
        static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var mode = menu.Item("Combo.Mode").GetValue<StringList>().SelectedIndex;
            var eDelay = GetValueMenuSlider("Combo.EDelay");

            if (target == null) return;

            switch(mode)
            {
                case 0:
                    {
                        if (Q.IsReady() && Q.IsInRange(target) && GetValueMenuBool("Combo.Q"))
                            Q.CastOnUnit(target);

                        if (E.IsReady() && E.IsInRange(target) && GetValueMenuBool("Combo.E") && target.HasBuff("katarinaqmark"))
                        {
                            if (Environment.TickCount - lastE > eDelay)
                            {
                                E.CastOnUnit(target);
                                
                            }
                        }

                        if (W.IsReady() && W.IsInRange(target) && GetValueMenuBool("Combo.W"))
                            W.Cast();

                        if (R.IsReady() && R.IsInRange(target) && GetValueMenuBool("Combo.R") && !W.IsReady() && !E.IsReady())
                        {
                            orbwalker.SetAttack(false);
                            orbwalker.SetMovement(false);
                            R.Cast();
                        }
                    }
                    break;

                case 1:
                    {
                        if (E.IsReady() && E.IsInRange(target) && GetValueMenuBool("Combo.E"))
                        {
                            if (Environment.TickCount - lastE > eDelay)
                            {
                                E.CastOnUnit(target);
                               
                            }
                        }

                        if (Q.IsReady() && Q.IsInRange(target) && GetValueMenuBool("Combo.Q"))
                            Q.CastOnUnit(target);

                        if (W.IsReady() && W.IsInRange(target) && GetValueMenuBool("Combo.W") && target.HasBuff("katarinaqmark"))
                            W.Cast();

                        if (R.IsReady() && R.IsInRange(target) && GetValueMenuBool("Combo.R") && !W.IsReady() && !E.IsReady())
                        {
                            orbwalker.SetAttack(false);
                            orbwalker.SetMovement(false);
                            R.Cast();
                        }
                    }
                    break;
            }
        }
        static void Harass()
        {

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var mode = menu.Item("Harass.Mode").GetValue<StringList>().SelectedIndex;
            var eDelay = GetValueMenuSlider("Harass.EDelay");

            if (target == null) return;

            switch (mode)
            {
                case 0:
                    {
                        if (Q.IsReady() && Q.IsInRange(target) && GetValueMenuBool("Harass.Q"))
                            Q.CastOnUnit(target);

                        if (E.IsReady() && E.IsInRange(target) && GetValueMenuBool("Harass.E") && target.HasBuff("katarinaqmark"))
                        {
                            if (Environment.TickCount - lastE > eDelay)
                            {
                                E.CastOnUnit(target);
                               
                            }
                        }

                        if (W.IsReady() && W.IsInRange(target) && GetValueMenuBool("Harass.W"))
                            W.Cast();
                    }
                    break;

                case 1:
                    {
                        if (E.IsReady() && E.IsInRange(target) && GetValueMenuBool("Harass.E"))
                        {
                            if (Environment.TickCount - lastE > eDelay)
                            {
                                E.CastOnUnit(target);
                                
                            }
                        }

                        if (Q.IsReady() && Q.IsInRange(target) && GetValueMenuBool("Harass.Q"))
                            Q.CastOnUnit(target);

                        if (W.IsReady() && W.IsInRange(target) && GetValueMenuBool("Harass.W") && target.HasBuff("katarinaqmark"))
                            W.Cast();                      
                    }
                    break;
            }
        }
        static void Farm()
        {
            var minions = MinionManager.GetMinions(player.Position, Q.Range);

            if (Q.IsReady() && GetValueMenuBool("Farm.Q"))
            {
                foreach(var minion in minions)
                {
                    if (minion.IsValidTarget(Q.Range) && minion.Health < (Q.GetDamage(minion) * 0.9))
                        Q.CastOnUnit(minion);                                        
                }
            }

            if (W.IsReady() && GetValueMenuBool("Farm.W"))
            {
                foreach (var minion in minions)
                {
                    if(minion.HasBuff("katarinaqmark"))
                    {
                        var Markdmg = (player.CalcDamage(minion, Damage.DamageType.Magical, player.FlatMagicDamageMod * 0.15 + Q.Level * 15)) * 0.9;
                        var totalWdamage = (Markdmg + W.GetDamage(minion)) * 0.9;

                        if (minion.IsValidTarget(W.Range) && minion.Health < totalWdamage)
                            W.Cast();
                    }

                    if (minion.IsValidTarget(W.Range) && minion.Health < (W.GetDamage(minion) * 0.9))
                        W.Cast();
                }
            }

            if (E.IsReady() && GetValueMenuBool("Farm.E"))
            {
                foreach (var minion in minions)
                {
                    if (minion.IsValidTarget(E.Range) && minion.Health < (E.GetDamage(minion) * 0.9))
                        E.CastOnUnit(minion);
                }
            }
        }                    
        static void LaneClear()
        {
            var minions = MinionManager.GetMinions(player.Position, Q.Range);

            if (Q.IsReady() && GetValueMenuBool("LC.Q"))
            {
                foreach (var minion in minions)
                {
                    if (minion.IsValidTarget(Q.Range) && minion.Health < (Q.GetDamage(minion) * 0.9))
                        Q.CastOnUnit(minion);
                }
            }

            if (W.IsReady() && GetValueMenuBool("LC.W"))
            {
                foreach (var minion in minions)
                {
                    if (minion.HasBuff("katarinaqmark"))
                    {
                        var Markdmg = (player.CalcDamage(minion, Damage.DamageType.Magical, player.FlatMagicDamageMod * 0.15 + Q.Level * 15)) * 0.9;
                        var totalWdamage = (Markdmg + W.GetDamage(minion)) * 0.9;

                        if (minion.IsValidTarget(W.Range) && minion.Health < totalWdamage)
                            W.Cast();
                    }

                    if (minion.IsValidTarget(W.Range) && minion.Health < (W.GetDamage(minion) * 0.9))
                        W.Cast();
                }
            }

            if (E.IsReady() && GetValueMenuBool("LC.E"))
            {
                foreach (var minion in minions)
                {
                    if (minion.IsValidTarget(E.Range) && minion.Health < (E.GetDamage(minion) * 0.9))
                        E.CastOnUnit(minion);
                }
            }
        }
        static void KillSteal()
        {
            foreach(var enemy in HeroManager.Enemies)
            {
                var qDamage = Q.GetDamage(enemy) * 0.9;
                var wDamage = W.GetDamage(enemy) * 0.9;
                var eDamage = E.GetDamage(enemy) * 0.9;

                if (enemy.Health < qDamage && GetValueMenuBool("KS.Q") && Q.IsInRange(enemy))
                    Q.CastOnUnit(enemy);

                if (enemy.Health < wDamage && GetValueMenuBool("KS.W") && W.IsInRange(enemy))
                    W.Cast();

                if (enemy.Health < eDamage && GetValueMenuBool("KS.E") && E.IsInRange(enemy))
                    E.CastOnUnit(enemy);
            }
        }
        static float GetComboDamage(AIHeroClient enemy)
        {
            double damage = 0d;

            if (Q.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.Q);           

            if (W.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady() && R.Level > 0)
                damage += player.GetSpellDamage(enemy, SpellSlot.R) * 8;

            var Markdmg = (player.CalcDamage(enemy, Damage.DamageType.Magical, player.FlatMagicDamageMod * 0.15 + Q.Level * 15)) * 0.9;

            if (enemy.HasBuff("katarinaqmark"))
                damage += Markdmg;

            return (float)damage;
        }

    }
}
