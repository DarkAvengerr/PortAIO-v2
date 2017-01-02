using System;
using System.Drawing;
using System.Linq;
using HikiCarry.Core.Plugins;
using HikiCarry.Core.Predictions;
using HikiCarry.Core.Utilities;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Champions
{
    internal class Sivir
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;

        public Sivir()
        {

            Q = new Spell(SpellSlot.Q, 1250f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);

            SpellDatabase.InitalizeSpellDatabase();

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("q.combo", "Use (Q)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("w.combo", "Use (W)", true).SetValue(true));
                Initializer.Config.AddSubMenu(comboMenu);
            }
            
            var shieldmenu = new Menu("Shield Settings", "Shield Settings");
            {
                shieldmenu.AddItem(new MenuItem("sivir.shield", "Use Shield!", true).SetValue(true));
                shieldmenu.AddItem(new MenuItem("info.sivir.1", "                       Evadeable Spells", true)).SetFontStyle(FontStyle.Bold, SharpDX.Color.Yellow);

                foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.EvadeableSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsSkillshot)))
                {
                    shieldmenu.AddItem(new MenuItem($"e.protect.{spell.SpellName}",
                        $"{spell.ChampionName} ({spell.Slot})").SetValue(true));
                }

                foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsTargeted)))
                {
                    shieldmenu.AddItem(new MenuItem($"e.protect.targetted.{spell.SpellName}",
                            $"{spell.ChampionName} ({spell.Slot})").SetValue(true));
                }
                Initializer.Config.AddSubMenu(shieldmenu);
            }

            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("q.harass", "Use (Q)", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.mana", "Harass Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(harassMenu);
            }

            var clearmenu = new Menu("LaneClear Settings","LaneClear Settings");
            {
                clearmenu.AddItem(new MenuItem("q.laneclear", "Use (Q)", true).SetValue(true));
                clearmenu.AddItem(new MenuItem("q.minion.count", "(Q) Min. Minion Count", true).SetValue(new Slider(3, 1, 5)));
                clearmenu.AddItem(new MenuItem("w.laneclear", "Use (W)", true).SetValue(true));
                clearmenu.AddItem(new MenuItem("w.minion.count", "(W) Min. Minion Count", true).SetValue(new Slider(3, 1, 5)));
                clearmenu.AddItem(new MenuItem("clear.mana", "Clear Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(clearmenu);
            }

            var junglemenu = new Menu("Jungle Settings", "Jungle Settings");
            {
                junglemenu.AddItem(new MenuItem("q.jungle", "Use (Q)", true).SetValue(true));
                junglemenu.AddItem(new MenuItem("w.jungle", "Use (Q)", true).SetValue(true));
                junglemenu.AddItem(new MenuItem("jungle.mana", "Jungle Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(junglemenu);
            }
            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo Damage").SetValue(true);
                var drawFill = new MenuItem("RushDrawEDamageFill", "Combo Damage Fill").SetValue(new Circle(true, System.Drawing.Color.Gold));

                drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
                drawMenu.SubMenu("Damage Draws").AddItem(drawFill);

                //DamageIndicator.DamageToUnit = TotalDamage;
                //DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
                //DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                //DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

                drawDamageMenu.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    //DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

                drawFill.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    //DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                    //DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
                Initializer.Config.AddSubMenu(drawMenu);
            }
            Game.OnUpdate += SivirOnUpdate;
            Obj_AI_Base.OnProcessSpellCast += SivirOnProcess;
            Obj_AI_Base.OnSpellCast += SivirOnSpellCast;

        }

        private float TotalDamage(AIHeroClient hero)
        {
            var damage = 0d;
            if (Q.IsReady())
            {
                damage += Q.GetDamage(hero);
            }
            if (W.IsReady())
            {
                damage += W.GetDamage(hero);
            }
           
            return (float)damage;
        }

        private void SivirOnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe && W.IsReady() && !Utilities.Enabled("w.combo"))
            {
                return;
            }

            if (args.SData.IsAutoAttack() && args.Target is AIHeroClient && Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                && sender.Type == GameObjectType.AIHeroClient)
            {
                W.Cast();
            }
        }

        private void SivirOnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Utilities.Enabled("sivir.shield"))
            {
                return;
            }

            if (sender.IsEnemy && sender is AIHeroClient && Initializer.Config.Item("e.protect." + args.SData.Name, true) != null && Initializer.Config.Item("e.protect." + args.SData.Name, true).GetValue<bool>()
                && args.End.Distance(ObjectManager.Player.Position) < 200)
            {
                E.Cast();
            }

            if (sender.IsEnemy && sender is AIHeroClient && args.Target.IsMe && args.Target != null &&
               Initializer.Config.Item("e.protect.targetted." + args.SData.Name, true) != null && Initializer.Config.Item("e.protect.targetted." + args.SData.Name, true).GetValue<bool>()
               && args.Target.IsMe)
            {
                E.Cast();
            }
        }

        private void SivirOnUpdate(EventArgs args)
        {
            switch (Initializer.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnMixed();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnClear();
                    OnJungle();
                    break;
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Q.IsReady() && Utilities.Enabled("q.combo") && target.IsValidTarget(Q.Range))
                {
                    Q.Do(target, Utilities.HikiChance("hitchance"));
                }
            }

        }

        private static void OnMixed()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("harass.mana"))
            {
                return;
            }

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Q.IsReady() && Utilities.Enabled("q.harass") && target.IsValidTarget(Q.Range))
                {
                    Q.Do(target, Utilities.HikiChance("hitchance"));
                }
                
            }
        }

        private static void OnClear()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("clear.mana"))
            {
                return;
            }

            if (Q.IsReady() && Utilities.Enabled("q.laneclear"))
            {
                var minionlist = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
                var whitlist = W.GetLineFarmLocation(minionlist);
                if (whitlist.MinionsHit >= Utilities.Slider("q.minion.count"))
                {
                    Q.Cast(whitlist.Position);
                }
            }

            if (W.IsReady() && Utilities.Enabled("w.laneclear"))
            {
                var minionlist = MinionManager.GetMinions(ObjectManager.Player.AttackRange);
                if (minionlist.Count() >= Utilities.Slider("w.minion.count") && minionlist != null)
                {
                    W.Cast();
                }
            }
        }

        private static void OnJungle()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("jungle.mana"))
            {
                return;
            }

            if (Q.IsReady() && Utilities.Enabled("q.jungle"))
            {
                var target = MinionManager.GetMinions(ObjectManager.Player.AttackRange, MinionTypes.All,MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(x => x.IsValidTarget(Q.Range));

                if (target != null)
                {
                    Q.Do(target,HitChance.High);
                }
            }

            if (W.IsReady() && Utilities.Enabled("w.jungle"))
            {
                var target = MinionManager.GetMinions(ObjectManager.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(x => x.IsValidTarget(ObjectManager.Player.AttackRange));

                if (target != null)
                {
                    W.Cast();
                }
            }
        }
    }
}
