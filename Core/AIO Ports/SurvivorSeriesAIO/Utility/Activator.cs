using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SurvivorSeriesAIO.Core;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Utility
{
    internal class Activator : ActivatorBase
    {
        public Activator(IRootMenu menu) : base(menu)
        {
            Config = new Configuration(menu.Activator);
            Chat.Print(
                "<font color='#0993F9'>[SurvivorSeries AIO]</font> <font color='#FF8800'>Activator Loaded.</font>");
            Game.OnUpdate += Game_OnGameUpdate;
        }

        public Configuration Config { get; }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!Menu.PlugActivator.GetValue<bool>())
                return;
            if (ObjectManager.Player.InFountain() || ObjectManager.Player.IsRecalling() || ObjectManager.Player.IsDead)
                return;

            SeraphUsage();
            ProHexGLPUsage();

            if (Config.UsePotions.GetValue<bool>())
                PotionUsage();
            if (SummonerDot.Slot != EloBuddy.SpellSlot.Unknown)
                if (Config.UseIgnite.GetValue<bool>())
                    Ignite();
        }

        /// <summary>
        ///     Potion Usage - Smart Usage
        /// </summary>
        public void PotionUsage()
        {
            if (Config.UseSmartPotion.GetValue<bool>())
                if (ObjectManager.Player.CountEnemiesInRange(800) == 0)
                    return;

            if (ObjectManager.Player.HasBuff("RegenerationPotion") ||
                ObjectManager.Player.HasBuff("ItemMiniRegenPotion") ||
                ObjectManager.Player.HasBuff("ItemCrystalFlaskJungle") ||
                ObjectManager.Player.HasBuff("ItemDarkCrystalFlask") || ObjectManager.Player.HasBuff("ItemCrystalFlask"))
                return;

            if (ObjectManager.Player.HealthPercent <= Config.UsePotionsAtHPPercent.GetValue<Slider>().Value)
                if (HPPotion.IsReady())
                    HPPotion.Cast();
                else if (Biscuit.IsReady())
                    Biscuit.Cast();
                else if (FlaskHunterJG.IsReady())
                    FlaskHunterJG.Cast();
                else if (FlaskCorruptJG.IsReady())
                    FlaskCorruptJG.Cast();
                else if (FlaskRef.IsReady())
                    FlaskRef.Cast();
        }

        /// <summary>
        ///     Defensive Item Usage - Seraph
        /// </summary>
        public void SeraphUsage()
        {
            var incomingdmg = OktwCommon.GetIncomingDamage2(ObjectManager.Player, 1);
            if (Seraph.IsReady() && Config.UseSeraph.GetValue<bool>())
            {
                var shieldint = ObjectManager.Player.Mana*0.2 + 150;
                if ((incomingdmg > ObjectManager.Player.Health) &&
                    (incomingdmg < ObjectManager.Player.Health + shieldint))
                    Seraph.Cast();
            }
        }

        /// <summary>
        ///     AutoIgnite for AIO
        /// </summary>
        public void Ignite()
        {
            if ((ObjectManager.Player.Spellbook.CanUseSpell(SummonerDot.Slot) == SpellState.Ready) &&
                Config.UseIgnite.GetValue<bool>())
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(600))) // Ignite Range
                {
                    var ignitedamage = ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
                    var hpprediction = enemy.Health - OktwCommon.GetIncomingDamage(enemy); //OKTW LIB

                    if ((hpprediction <= 2*ignitedamage) && OktwCommon.ValidUlt(enemy))
                        if (enemy.Health > ObjectManager.Player.Health)
                            ObjectManager.Player.Spellbook.CastSpell(SummonerDot.Slot, enemy);
                }
        }

        /// <summary>
        ///     Pro/Hex/GLP Usage, Perfect.
        /// </summary>
        public void ProHexGLPUsage()
        {
            if (GLP800.IsReady())
            {
                var t = TargetSelector.GetTarget(GLP800.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                    if (Config.GLP800.GetValue<bool>())
                        GLP800.Cast(Prediction.GetPrediction(t, 0.5f).CastPosition);
            }

            if (Protobelt.IsReady())
            {
                var t = TargetSelector.GetTarget(Protobelt.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                    if (Config.Protobelt.GetValue<bool>())
                        Protobelt.Cast(Prediction.GetPrediction(t, 0.5f).CastPosition);
            }

            if (Hextech.IsReady())
            {
                var t = TargetSelector.GetTarget(Hextech.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                    if (Config.Hextech.GetValue<bool>())
                        Hextech.Cast(t);
            }
        }

        public class Configuration
        {
            public Configuration(Menu root)
            {
                PotionMenu = MenuFactory.CreateMenu(root, "Potions Usage");
                OffensiveMenu = MenuFactory.CreateMenu(root, "Offensive Items");
                DefensiveMenu = MenuFactory.CreateMenu(root, "Defensive Items");
                SpellMenu = MenuFactory.CreateMenu(root, "Spells Usage");

                Potion(MenuItemFactory.Create(PotionMenu));
                OffensiveItems(MenuItemFactory.Create(OffensiveMenu));
                DefensiveItems(MenuItemFactory.Create(DefensiveMenu));
                SpellsUsage(MenuItemFactory.Create(SpellMenu));
            }

            public Menu PotionMenu { get; set; }
            public MenuItem UsePotions { get; set; }
            public MenuItem UsePotionsAtHPPercent { get; set; }
            public MenuItem UseSmartPotion { get; set; }
            public MenuItem UseSeraph { get; set; }
            public MenuItem UseSeraphAtHP { get; set; }
            public MenuItem GLP800 { get; set; }
            public MenuItem Hextech { get; set; }
            public MenuItem Protobelt { get; set; }
            public Spell SummonerDot { get; set; }
            public MenuItem UseIgnite { get; set; }
            public Menu SpellMenu { get; set; }
            public Menu DefensiveMenu { get; set; }
            public Menu OffensiveMenu { get; set; }

            public void Potion(MenuItemFactory factory)
            {
                // Potion Menu
                UsePotions = factory.WithName("Use Potions").WithValue(true).Build();
                UsePotionsAtHPPercent =
                    factory.WithName("Use Potions at HP Percent 'X'").WithValue(new Slider(30, 0, 100)).Build();
                UseSmartPotion =
                    factory.WithName("Use Smart Potion Logic")
                        .WithValue(true)
                        .WithTooltip("If Enabled, it'll check if enemy's around so it doesn't waste potions.")
                        .Build();

                UseSmartPotion.ValueChanged += (sender, eventArgs) =>
                {
                    if (!UsePotions.GetValue<bool>() && eventArgs.GetNewValue<bool>())
                        UsePotions.SetValue(true);
                };
            }

            public void SpellsUsage(MenuItemFactory factory)
            {
                SummonerDot = new Spell(ObjectManager.Player.GetSpellSlot("SummonerDot"), 550);
                if (SummonerDot.Slot != EloBuddy.SpellSlot.Unknown)
                    UseIgnite = factory.WithName("Use Smart Ignite").WithValue(true).Build();
                else
                    UseIgnite =
                        factory.WithName("Use Smart Ignite (No Ignite Found)")
                            .WithValue(false)
                            .WithTooltip("Turning it On/Off won't make a difference lad!")
                            .Build();
            }

            public void OffensiveItems(MenuItemFactory factory)
            {
                GLP800 = factory.WithName("Use GLP800 in Combo").WithValue(true).Build();
                Hextech = factory.WithName("Use Hextech in Combo").WithValue(true).Build();
                Protobelt = factory.WithName("Use Protobelt in Combo").WithValue(true).Build();
            }

            public void DefensiveItems(MenuItemFactory factory)
            {
                UseSeraph = factory.WithName("Use Seraph to Survive").WithValue(true).Build();
                UseSeraphAtHP = factory.WithName("Use Seraph At HP (%)").WithValue(new Slider(30, 1, 100)).Build();
            }
        }
    }
}