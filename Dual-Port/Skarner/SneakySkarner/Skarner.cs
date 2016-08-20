using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SneakySkarner.Logging;
using SneakySkarner.Utility;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SneakySkarner
{
    class Skarner : Champion
    {
        private const string ChampionName = "Skarner";
        private DamageBuffer DamageBuffer { get; set; }

        protected override void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;
            if (Player.CharData.BaseSkinName != ChampionName) return;
            //Init Spells
            InitSpells();

            InitMenu();
            Config.AddToMainMenu();
            DamageBuffer = new DamageBuffer(.03f);
        }

        private static void InitSpells()
        {
            Q = new Spell(SpellSlot.Q, 350f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 1000f);
            E.SetSkillshot(E.Instance.SData.SpellCastTime, E.Instance.SData.LineWidth, E.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 350f);
        }

        private bool UseQCombo
        {
            get { return Config.Item("UseQCombo").GetValue<bool>(); }
        }

        private bool UseECombo
        {
            get { return Config.Item("UseECombo").GetValue<bool>(); }
        }

        private bool UseQFarm
        {
            get { return Config.Item("UseQFarm").GetValue<bool>(); }
        }

        private bool UseEFarm
        {
            get { return Config.Item("UseEFarm").GetValue<bool>(); }
        }

        private bool UseSmartShield
        {
            get { return Config.Item("UseSmartShield").GetValue<bool>(); }
        }

        private static void InitMenu()
        {
            Config = new Menu("SneakySkarner", ChampionName, true);


            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            /* Orbwalker Menu */
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));


            /* Combo Menu */
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            //Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));

            /* Harass Menu */
            //Config.AddSubMenu(new Menu("Harass", "Harass"));
            //Config.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "Use E").SetValue(true));
            //Config.SubMenu("Harass")
            //    .AddItem(new MenuItem("HarassActive", "Harass").SetValue(new KeyBind(67, KeyBindType.Press)));

            /* Farm Menu */
            Config.AddSubMenu(new Menu("Farm", "Farm"));
            Config.SubMenu("Farm").AddItem(new MenuItem("UseQFarm", "Use Q").SetValue(true));
            Config.SubMenu("Farm").AddItem(new MenuItem("UseEFarm", "Use E").SetValue(true));

            /* Misc Menu */
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("UseSmartShield", "SmartShield Toggle").SetValue(true));
            
            /* Drawing Menu */
            var drawMenu = new Menu("Damage Drawing", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_W", "Draw W").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_E", "Draw E").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", "Draw R").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R_Killable", "Draw R Mark on Killable").SetValue(true));

                var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage").SetValue(true);
                var drawFill =
                    new MenuItem("Draw_Fill", "Draw Combo Damage Fill").SetValue(new Circle(true,
                        Color.FromArgb(90, 255, 169, 4)));
                drawMenu.AddItem(drawComboDamageMenu);
                drawMenu.AddItem(drawFill);
                DamageIndicator.DamageToUnit = GetComboDamage;
                DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Render.Circle>().Color;
                    };

                Config.AddSubMenu(drawMenu);
            }
        }

        protected override void Game_OnUpdate(EventArgs args)
        {
            Orbwalker.SetAttack(!ImpaleActive);

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Farm();
            }
        }


        private void Combo()
        {


            if (Q.IsReady() && UseQCombo)
            {
                var target = TargetSelector.GetTarget(Player, Q.Range, TargetSelector.DamageType.Physical);
                if (!target.IsValidTarget(Q.Range)) return;
                Q.Cast();
            }
            if (E.IsReady() && UseECombo)
            {
                var target = TargetSelector.GetTarget(Player, E.Range, TargetSelector.DamageType.Magical);
                var prediction = E.GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High)
                {
                    E.Cast(prediction.CastPosition);
                }
            }
        }

        private void Harass()
        {
        }

        private void Farm()
        {
            /* Get minions and their positions */
            var jungleMinions = MinionManager.GetMinions(Player.Position, Q.Range + 100, MinionTypes.All, MinionTeam.Neutral);
            List<Vector2> jungleMinionPositions = jungleMinions.Select(min => min.Position.To2D()).ToList();
            var laneMinions = MinionManager.GetMinions(Player.Position, Q.Range + 100, MinionTypes.All, MinionTeam.Enemy);
            List<Vector2> laneMinionPositions = laneMinions.Select(min => min.Position.To2D()).ToList();

            if (laneMinions.Count(min => min.Health <= Q.GetDamage(min)) >= 1)
            {
                Q.Cast();
            }

            if (Q.IsReady() && UseQFarm)
            {
                if (jungleMinionPositions.NumInRange(Player, Q.Range) == jungleMinions.Count && jungleMinions.Count > 0)
                {
                    Q.Cast();
                }
            }
            if (E.IsReady() && UseEFarm)
            {
                var eJungleMinions = MinionManager.GetMinions(Player.Position, E.Range, MinionTypes.All, MinionTeam.Neutral);
                var biggestJungleMinion = eJungleMinions.OrderByDescending(min => min.MaxHealth).First();
                var pred = E.GetPrediction(biggestJungleMinion);
                if (pred.Hitchance > HitChance.Medium)
                {
                    E.Cast(pred.CastPosition);
                }
            }

            /* Titanic Hydra Lane Clear*/
            if (Items.HasItem(3748) && Items.CanUseItem(3748))
            {
                if (laneMinionPositions.NumInRange(Player, 400) >= 5 && laneMinions.Count > 0)
                {
                    Items.UseItem(3748);
                }else if (jungleMinionPositions.NumInRange(Player, 400) == jungleMinions.Count && jungleMinions.Count > 0)
                {
                    Items.UseItem(3748);
                }
            }

            /* Titanic Hydra Jungle Clear */
            if (Items.HasItem(3748) && Items.CanUseItem(3748))
            {
                
            }
        }

        private static bool ImpaleActive
        {
            get { return Player.Level >= 6 && Player.Buffs.Any(buff => buff.Name == "skarnerimpalevo"); 
            }
        }

        protected override void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.Team != GameObjectTeam.Neutral && sender.Team != GameObjectTeam.Chaos && sender.Team != GameObjectTeam.Unknown) return;

            /* If spell will do more than 3% of max health, cast W */
            if (args.Target.IsMe)
            {
                var damage = sender.GetSpellDamage(Player, args.SData.Name);
                if ((damage > Player.Health || DamageBuffer.IsBufferOverload((float)damage, Player.Health, Player.MaxHealth)) && UseSmartShield)
                {
                    W.Cast();
                }
            }
            

        }

        protected override void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var jungleMinions = MinionManager.GetMinions(Player.Position, Q.Range + 100, MinionTypes.All, MinionTeam.Neutral);
                List<Vector2> jungleMinionPositions = jungleMinions.Select(min => min.Position.To2D()).ToList();
                var laneMinions = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy);
                List<Vector2> laneMinionPositions = laneMinions.Select(min => min.Position.To2D()).ToList();
                if (laneMinions.Count > 0)
                {
                    if (laneMinions.Count(min => min.Health <= Q.GetDamage(min)) >= 1)
                    {
                        Q.Cast();
                    }
                    /* Tiamat */
                    if (Items.HasItem(3077) && Items.CanUseItem(3077))
                    {
                        if (laneMinionPositions.NumInRange(Player, 400) >= 2 && laneMinions.Count > 0)
                        {
                            Items.UseItem(3077);
                        }
                    }

                    /* Ravenous Hydra */
                    if (Items.HasItem(3074) && Items.CanUseItem(3074))
                    {
                        if (laneMinionPositions.NumInRange(Player, 400) >= 2 && laneMinions.Count > 0)
                        {
                            Items.UseItem(3074);
                        }
                    }

                    
                }
                else if (jungleMinions.Count > 0)
                {
                    /* Tiamat */
                    if (Items.HasItem(3077) && Items.CanUseItem(3077))
                    {
                        if (jungleMinionPositions.NumInRange(Player, 400) == jungleMinions.Count && jungleMinions.Count > 0)
                        {
                            Items.UseItem(3077);
                        }
                    }

                    /* Ravenous Hydra */
                    if (Items.HasItem(3074) && Items.CanUseItem(3074))
                    {
                        if (jungleMinionPositions.NumInRange(Player, 400) == jungleMinions.Count && jungleMinions.Count > 0)
                        {
                            Items.UseItem(3074);
                        }
                    }
                    
                }
                
            }
        }
    }
}
