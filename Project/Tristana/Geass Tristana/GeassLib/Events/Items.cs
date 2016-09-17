using System;
using System.Linq;
using GeassLib.Menus;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Events
{
    class Items
    {
        public Items()
        {

            Orbwalking.AfterAttack += After_Attack;
            Orbwalking.BeforeAttack += Before_Attack;
            Game.OnUpdate += OnUpdate;
        }

        void After_Attack(AttackableUnit unit, AttackableUnit target)
        {
        }

        void Before_Attack(Orbwalking.BeforeAttackEventArgs args)
        {

        }

        public void OnUpdate(EventArgs args)
        {
            #region Offensive

            if (!DelayHandler.CheckItems()) return;

            DelayHandler.UseItems();
            var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
            if (target == null) return;


            if (Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Boolean.Bork").GetValue<bool>() && LeagueSharp.Common.Items.HasItem(Data.Items.Offensive.Botrk.Id))
            // If enabled and has item
            {
                if (Data.Items.Offensive.Botrk.IsReady())
                {
                    if (
                        target.IsValidTarget(Globals.Objects.Player.AttackRange + Globals.Objects.Player.BoundingRadius) || Globals.Objects.Player.HealthPercent < Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Slider.Bork.MinHp.Player").GetValue<Slider>().Value)
                    {
                        // In auto Range or about to die
                        if (Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() && Globals.Variables.InCombo &&
                            target.HealthPercent < Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Slider.Bork.MaxHp").GetValue<Slider>().Value
                            //in combo and target hp less then
                            ||
                            !Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() &&
                            target.HealthPercent < Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Slider.Bork.MinHp").GetValue<Slider>().Value
                            //not in combo but target HP less then
                            ||
                            (Globals.Objects.Player.HealthPercent <
                             Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveNameBase + "Slider.Bork.MinHp.Player").GetValue<Slider>().Value))
                        //Player hp less then
                        {
                            Globals.Objects.Logger.WriteLog($"Use Bork on {target}");
                            LeagueSharp.Common.Items.UseItem(Data.Items.Offensive.Botrk.Id, target);
                            return;
                        }
                    }
                }
            }

            if (Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Boolean.Bork").GetValue<bool>() && LeagueSharp.Common.Items.HasItem(Data.Items.Offensive.Cutless.Id))
            // If enabled and has item
            {
                if (Data.Items.Offensive.Cutless.IsReady())
                {
                    if (
                        target.IsValidTarget(Globals.Objects.Player.AttackRange +
                                           Globals.Objects.Player.BoundingRadius) ||
                        Globals.Objects.Player.HealthPercent <
                       Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Slider.Bork.MinHp.Player").GetValue<Slider>().Value)
                    {
                        // In auto Range or about to die
                        if (Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() && Globals.Variables.InCombo &&
                            target.HealthPercent <
                            Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Slider.Bork.MaxHp").GetValue<Slider>().Value
                            //in combo and target hp less then
                            ||
                            !Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() &&
                            target.HealthPercent <
                            Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Slider.Bork.MinHp").GetValue<Slider>().Value
                            //not in combo but target HP less then
                            ||
                            (Globals.Objects.Player.HealthPercent <
                             Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Slider.Bork.MinHp.Player").GetValue<Slider>().Value))
                        //Player hp less then
                        {

                            Globals.Objects.Logger.WriteLog($"Use Cutless on {target}");
                            LeagueSharp.Common.Items.UseItem(Data.Items.Offensive.Cutless.Id, target);
                            return;
                        }
                    }
                }
            }

            if (Globals.Objects.GeassLibMenu.Item(Names.MenuOffensiveItemBase + "Boolean.Youmuu").GetValue<bool>() && LeagueSharp.Common.Items.HasItem(Data.Items.Offensive.GhostBlade.Id))
            // If enabled and has item
            {
                if (Data.Items.Offensive.GhostBlade.IsReady() &&
                    target.IsValidTarget(Globals.Objects.Player.AttackRange + Globals.Objects.Player.BoundingRadius))
                // Is ready and target is in auto range
                {
                    if (Globals.Variables.InCombo)
                    {

                        Globals.Objects.Logger.WriteLog($"Use Ghostblade on {target}");
                        LeagueSharp.Common.Items.UseItem(Data.Items.Offensive.GhostBlade.Id);
                        return;
                    }
                }
            }

            #endregion Offensive

            #region Defensive

            if (Globals.Objects.GeassLibMenu.Item(Names.MenuDefensiveItemBase + "Boolean.QSS").GetValue<bool>() && LeagueSharp.Common.Items.HasItem(Data.Items.Defensive.Qss.Id))
            {
                if (Globals.Objects.GeassLibMenu.Item(Names.MenuDefensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() && Globals.Variables.InCombo ||
                    !Globals.Objects.GeassLibMenu.Item(Names.MenuDefensiveItemBase + "Boolean.ComboOnly").GetValue<bool>())
                {
                    if (Data.Items.Defensive.Qss.IsReady())
                    {
                        foreach (var buff in Data.Buffs.GetTypes.Where(buff => Globals.Objects.GeassLibMenu.Item(Names.MenuDefensiveItemBase + "Boolean.QSS." + buff).GetValue<bool>()))
                        {
                            if (Globals.Objects.Player.HasBuffOfType(buff))
                            {

                                Globals.Objects.Logger.WriteLog($"Use QSS Reason {buff}");
                                LeagueSharp.Common.Utility.DelayAction.Add(
                                    Globals.Objects.GeassLibMenu.Item(Names.MenuDefensiveItemBase + "Slider.QSS.Delay").GetValue<Slider>().Value,
                                    () => LeagueSharp.Common.Items.UseItem(Data.Items.Defensive.Qss.Id));

                            }
                        }
                    }
                }
            }

            if (Globals.Objects.GeassLibMenu.Item(Names.MenuDefensiveItemBase + "Boolean.Merc").GetValue<bool>() && LeagueSharp.Common.Items.HasItem(Data.Items.Defensive.Merc.Id))
            {
                if (Globals.Objects.GeassLibMenu.Item(Names.MenuDefensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() && Globals.Variables.InCombo ||
                    !Globals.Objects.GeassLibMenu.Item(Names.MenuDefensiveItemBase + "Boolean.ComboOnly").GetValue<bool>())
                {
                    if (Data.Items.Defensive.Merc.IsReady())
                    {
                        foreach (var buff in Data.Buffs.GetTypes.Where(buff => Globals.Objects.GeassLibMenu.Item(Names.MenuDefensiveItemBase + "Boolean.Merc." + buff).GetValue<bool>()))
                        {
                            if (Globals.Objects.Player.HasBuffOfType(buff))
                            {
                                 Globals.Objects.Logger.WriteLog($"Use Merc Reason {buff}");
                                LeagueSharp.Common.Utility.DelayAction.Add(
                                    Globals.Objects.GeassLibMenu.Item(Names.MenuDefensiveItemBase + "Slider.Merc.Delay").GetValue<Slider>().Value,
                                    () => LeagueSharp.Common.Items.UseItem(Data.Items.Defensive.Qss.Id));

                            }
                        }
                    }
                }
            }

            #endregion Defensive
        }

    }
}
