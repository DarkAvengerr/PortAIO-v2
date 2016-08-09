using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using S_Plus_Class_Kalista.Libaries;
using EloBuddy;

namespace S_Plus_Class_Kalista.Handlers
{
    class ItemHandler : Core
    {

        private const string _MenuNameBase = ".Item Menu";
        private const string _MenuItemBase = ".Item.";

        private const string _MenuOffensiveNameBase = ".Offensive Menu";
        private const string _MenuOffensiveItemBase = _MenuItemBase + ".Offensive.";

        private const string _MenuDefensiveNameBase = ".Defensive Menu";
        private const string _MenuDefensiveItemBase = _MenuItemBase + ".Defensive.";


        public static void Load()
        {
            SMenu.AddSubMenu(Menu());
            Orbwalking.AfterAttack += After_Attack;
            Orbwalking.BeforeAttack += Before_Attack;
            Game.OnUpdate += OnUpdate;
        }
        
        private static Menu Menu()
        {
            var _Menu = new Menu(_MenuNameBase, "itemMenu");

            var OffensiveMenu = new Menu(_MenuOffensiveNameBase,"offensiveMenu");
            OffensiveMenu.AddItem(new MenuItem(_MenuOffensiveItemBase + "Boolean.Bork", "Use BotRK/Cutlass").SetValue(true));
            OffensiveMenu.AddItem(new MenuItem(_MenuOffensiveItemBase + "Boolean.Youmuu", "Use Youmuu's").SetValue(true));
            OffensiveMenu.AddItem(new MenuItem(_MenuOffensiveItemBase + "Slider.Bork.MinHp", "(BotRK/Cutlass) Min% HP Remaining(Target)").SetValue(new Slider(20)));
            OffensiveMenu.AddItem(new MenuItem(_MenuOffensiveItemBase + "Slider.Bork.MaxHp", "(BotRK/Cutlass) Max% HP Remaining(Target)").SetValue(new Slider(55)));
            OffensiveMenu.AddItem(new MenuItem(_MenuOffensiveItemBase + "Slider.Bork.MinHp.Player", "(BotRK/Cutlass) Min% HP Remaining(Player)").SetValue(new Slider(20)));
            OffensiveMenu.AddItem(new MenuItem(_MenuOffensiveItemBase + "Boolean.ComboOnly", "Only use offensive items in combo").SetValue(true));

            var DefensiveMenu = new Menu(_MenuDefensiveNameBase, "defensiveMenu");

            var qssMenu = new Menu(".QSS Menu","qssMenu");

            qssMenu.AddItem(new MenuItem(_MenuDefensiveItemBase + "Boolean.QSS", "Use QSS").SetValue(true));
            qssMenu.AddItem(new MenuItem(_MenuDefensiveItemBase + "Slider.QSS.Delay", "QSS Delay").SetValue(new Slider(300,250,1500)));


            foreach (var buff in Bufftype)
            {
                qssMenu.AddItem(new MenuItem(_MenuDefensiveItemBase + "Boolean.QSS." + buff, "Use QSS On" + buff).SetValue(true));
            }

            var mercMenu = new Menu(".Merc Menu", "MercMenu");

            mercMenu.AddItem(new MenuItem(_MenuDefensiveItemBase + "Boolean.Merc", "Use Merc").SetValue(true));
            mercMenu.AddItem(new MenuItem(_MenuDefensiveItemBase + "Slider.Merc.Delay", "Merc Delay").SetValue(new Slider(300, 250, 1500)));


            foreach (var buff in Bufftype)
            {
                mercMenu.AddItem(new MenuItem(_MenuDefensiveItemBase + "Boolean.Merc." + buff, "Use Merc On" + buff).SetValue(true));
            }

            DefensiveMenu.AddSubMenu(qssMenu);
            DefensiveMenu.AddSubMenu(mercMenu);
            DefensiveMenu.AddItem(new MenuItem(_MenuDefensiveItemBase + "Boolean.ComboOnly", "Only use offensive items in combo").SetValue(true));

            _Menu.AddSubMenu(OffensiveMenu);
            _Menu.AddSubMenu(DefensiveMenu);
            return _Menu;
        }

        private static void OnUpdate(EventArgs args)
        {
            #region Offensive

            if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.ItemDelay")) return;

            Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.ItemDelay");

            var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
            if (target == null) return;

            var inCombo = CommonOrbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo;

            if (SMenu.Item(_MenuOffensiveItemBase + "Boolean.Bork").GetValue<bool>() && Items.HasItem(Structures.Items.Offensive.Botrk.Id))
            // If enabled and has item
            {
                if (Structures.Items.Offensive.Botrk.IsReady())
                {
                    if (
                        target.IsValidTarget(Player.AttackRange + Player.BoundingRadius) || Player.HealthPercent < SMenu.Item(_MenuOffensiveItemBase + "Slider.Bork.MinHp.Player").GetValue<Slider>().Value)
                    {
                        // In auto Range or about to die
                        if (SMenu.Item(_MenuOffensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() && inCombo &&
                            target.HealthPercent < SMenu.Item(_MenuOffensiveItemBase + "Slider.Bork.MaxHp").GetValue<Slider>().Value
                            //in combo and target hp less then
                            ||
                            !SMenu.Item(_MenuOffensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() &&
                            target.HealthPercent < SMenu.Item(_MenuOffensiveItemBase + "Slider.Bork.MinHp").GetValue<Slider>().Value
                            //not in combo but target HP less then
                            ||
                            (Player.HealthPercent <
                             SMenu.Item(_MenuOffensiveItemBase + "Slider.Bork.MinHp.Player").GetValue<Slider>().Value))
                        //Player hp less then
                        {
                            Items.UseItem(Structures.Items.Offensive.Botrk.Id, target);
                            return;
                        }

                    }
                }
            }

            if (SMenu.Item(_MenuOffensiveItemBase + "Boolean.Bork").GetValue<bool>() && Items.HasItem(Structures.Items.Offensive.Cutless.Id))
            // If enabled and has item
            {
                if (Structures.Items.Offensive.Cutless.IsReady())
                {
                    if (
                        target.IsValidTarget(Player.AttackRange +
                                           Player.BoundingRadius) ||
                        Player.HealthPercent <
                       SMenu.Item(_MenuOffensiveItemBase + "Slider.Bork.MinHp.Player").GetValue<Slider>().Value)
                    {
                        // In auto Range or about to die
                        if (SMenu.Item(_MenuOffensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() && inCombo &&
                            target.HealthPercent <
                            SMenu.Item(_MenuOffensiveItemBase + "Slider.Bork.MaxHp").GetValue<Slider>().Value
                            //in combo and target hp less then
                            ||
                            !SMenu.Item(_MenuOffensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() &&
                            target.HealthPercent <
                            SMenu.Item(_MenuOffensiveItemBase + "Slider.Bork.MinHp").GetValue<Slider>().Value
                            //not in combo but target HP less then
                            ||
                            (Player.HealthPercent <
                             SMenu.Item(_MenuOffensiveItemBase + "Slider.Bork.MinHp.Player").GetValue<Slider>().Value))
                        //Player hp less then
                        {
                            Items.UseItem(Structures.Items.Offensive.Cutless.Id, target);
                            return;
                        }
                    }
                }
            }

            if (SMenu.Item(_MenuOffensiveItemBase + "Boolean.Youmuu").GetValue<bool>() && Items.HasItem(Structures.Items.Offensive.GhostBlade.Id))
            // If enabled and has item
            {
                if (Structures.Items.Offensive.GhostBlade.IsReady() &&
                    target.IsValidTarget(Player.AttackRange + Player.BoundingRadius))
                // Is ready and target is in auto range 
                {
                    if (inCombo)
                    {
                        Items.UseItem(Structures.Items.Offensive.GhostBlade.Id);
                        return;
                    }
                }
            }

            #endregion

            #region Defensive

            if (SMenu.Item(_MenuDefensiveItemBase + "Boolean.QSS").GetValue<bool>() && Items.HasItem(Structures.Items.Defensive.Qss.Id))
            {
                if (SMenu.Item(_MenuDefensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() && inCombo ||
                    !SMenu.Item(_MenuDefensiveItemBase + "Boolean.ComboOnly").GetValue<bool>())
                {
                    if (Structures.Items.Defensive.Qss.IsReady())
                    {

                        foreach (var buff in Bufftype.Where(buff => SMenu.Item(_MenuDefensiveItemBase + "Boolean.QSS." + buff).GetValue<bool>()))
                        {
                            if (Player.HasBuffOfType(buff))
                                LeagueSharp.Common.Utility.DelayAction.Add(SMenu.Item(_MenuDefensiveItemBase + "Slider.QSS.Delay").GetValue<Slider>().Value, () => Items.UseItem(Structures.Items.Defensive.Qss.Id));

                        }
                    }
                }
            }


            if (SMenu.Item(_MenuDefensiveItemBase + "Boolean.Merc").GetValue<bool>() && Items.HasItem(Structures.Items.Defensive.Merc.Id))
            {
                if (SMenu.Item(_MenuDefensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() && inCombo ||
                    !SMenu.Item(_MenuDefensiveItemBase + "Boolean.ComboOnly").GetValue<bool>())
                {
                    if (Structures.Items.Defensive.Merc.IsReady())
                    {
                        foreach (var buff in Bufftype.Where(buff => SMenu.Item(_MenuDefensiveItemBase + "Boolean.Merc." + buff).GetValue<bool>()))
                        {
                            if (Player.HasBuffOfType(buff))
                                LeagueSharp.Common.Utility.DelayAction.Add(SMenu.Item(_MenuDefensiveItemBase + "Slider.Merc.Delay").GetValue<Slider>().Value, () => Items.UseItem(Structures.Items.Defensive.Qss.Id));

                        }
                    }
                }
            }

            #endregion
        }

        private static void Before_Attack(Orbwalking.BeforeAttackEventArgs args)
        {

        }

        private static void After_Attack(AttackableUnit unit, AttackableUnit target)
        {

        }
    }
}
