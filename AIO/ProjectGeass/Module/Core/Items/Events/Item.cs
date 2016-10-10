using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using _Project_Geass.Data.Items;
using _Project_Geass.Functions;
using _Project_Geass.Global.Data;
using _Project_Geass.Humanizer.TickTock;

using EloBuddy;
using LeagueSharp.Common;
namespace _Project_Geass.Module.Core.Items.Events
{

    internal class Item
    {
        #region Public Fields

        public Orbwalking.Orbwalker Orbwalker;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Item" /> class.
        /// </summary>
        /// <param name="orbwalker">
        ///     The orbwalker.
        /// </param>
        public Item(Orbwalking.Orbwalker orbwalker)
        {
            Orbwalker = orbwalker;
            _offensive = new Offensive();
            _defensive = new Defensive();
            Orbwalking.AfterAttack += After_Attack;
            Orbwalking.BeforeAttack += Before_Attack;
            Game.OnUpdate += OnUpdate;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        public void OnUpdate(EventArgs args)
        {
            #region Offensive

            if (!Handler.CheckItems())
                return;

            Handler.UseItems();
            var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
            if (target == null)
                return;

            if (StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Boolean.Bork").GetValue<bool>() && EloBuddy.SDK.Item.HasItem(_offensive.Botrk.Id))
                // If enabled and has item
                if (_offensive.Botrk.IsReady())
                    if (target.IsValidTarget(StaticObjects.Player.AttackRange + StaticObjects.Player.BoundingRadius)
                        || (StaticObjects.Player.HealthPercent < StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Slider.Bork.MinHp.Player").GetValue<Slider>().Value))
                        if ((StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                             && (target.HealthPercent < StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Slider.Bork.MaxHp").GetValue<Slider>().Value))
                            //in combo and target hp less then
                            || (!StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Boolean.ComboOnly").GetValue<bool>()
                               && (target.HealthPercent < StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Slider.Bork.MinHp").GetValue<Slider>().Value))
                            //not in combo but target HP less then
                            || (StaticObjects.Player.HealthPercent < StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveNameBase + "Slider.Bork.MinHp.Player").GetValue<Slider>().Value))
                        //Player hp less then
                        {
                            StaticObjects.ProjectLogger.WriteLog($"Use Bork on {target}");
                            EloBuddy.SDK.Item.UseItem(_offensive.Botrk.Id, target);
                            return;
                        }

            if (StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Boolean.Bork").GetValue<bool>() && EloBuddy.SDK.Item.HasItem(_offensive.Cutless.Id))
                // If enabled and has item
                if (_offensive.Cutless.IsReady())
                    if (target.IsValidTarget(StaticObjects.Player.AttackRange + StaticObjects.Player.BoundingRadius)
                        || (StaticObjects.Player.HealthPercent < StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Slider.Bork.MinHp.Player").GetValue<Slider>().Value))
                        if ((StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                             && (target.HealthPercent < StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Slider.Bork.MaxHp").GetValue<Slider>().Value))
                            //in combo and target hp less then
                            || (!StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Boolean.ComboOnly").GetValue<bool>()
                               && (target.HealthPercent < StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Slider.Bork.MinHp").GetValue<Slider>().Value))
                            //not in combo but target HP less then
                            || (StaticObjects.Player.HealthPercent < StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Slider.Bork.MinHp.Player").GetValue<Slider>().Value))
                        //Player hp less then
                        {
                            StaticObjects.ProjectLogger.WriteLog($"Use Cutless on {target}");
                            EloBuddy.SDK.Item.UseItem(_offensive.Cutless.Id, target);
                            return;
                        }

            if (StaticObjects.ProjectMenu.Item(Names.Menu.MenuOffensiveItemBase + "Boolean.Youmuu").GetValue<bool>() && EloBuddy.SDK.Item.HasItem(_offensive.GhostBlade.Id))
                // If enabled and has item
                if (_offensive.GhostBlade.IsReady() && target.IsValidTarget(StaticObjects.Player.AttackRange + StaticObjects.Player.BoundingRadius))
                    // Is ready and target is in auto range
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        StaticObjects.ProjectLogger.WriteLog($"Use Ghostblade on {target}");
                        EloBuddy.SDK.Item.UseItem(_offensive.GhostBlade.Id);
                        return;
                    }

            #endregion Offensive

            #region Defensive

            if (StaticObjects.ProjectMenu.Item(Names.Menu.MenuDefensiveItemBase + "Boolean.QSS").GetValue<bool>() && EloBuddy.SDK.Item.HasItem(_defensive.Qss.Id))
                if ((StaticObjects.ProjectMenu.Item(Names.Menu.MenuDefensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo))
                    || !StaticObjects.ProjectMenu.Item(Names.Menu.MenuDefensiveItemBase + "Boolean.ComboOnly").GetValue<bool>())
                    if (_defensive.Qss.IsReady())
                        foreach (var buff in
                            Buffs.GetTypes.Where(buff => StaticObjects.ProjectMenu.Item(Names.Menu.MenuDefensiveItemBase + "Boolean.QSS." + buff).GetValue<bool>()))
                            if (StaticObjects.Player.HasBuffOfType(buff))
                            {
                                StaticObjects.ProjectLogger.WriteLog($"Use QSS Reason {buff}");
                                LeagueSharp.Common.Utility.DelayAction.Add(StaticObjects.ProjectMenu.Item(Names.Menu.MenuDefensiveItemBase + "Slider.QSS.Delay").GetValue<Slider>().Value, () => EloBuddy.SDK.Item.UseItem(_defensive.Qss.Id));
                            }

            // ReSharper disable once RedundantNameQualifier
            if (StaticObjects.ProjectMenu.Item(Names.Menu.MenuDefensiveItemBase + "Boolean.Merc").GetValue<bool>() && EloBuddy.SDK.Item.HasItem(_defensive.Merc.Id))
                if ((StaticObjects.ProjectMenu.Item(Names.Menu.MenuDefensiveItemBase + "Boolean.ComboOnly").GetValue<bool>() && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo))
                    || !StaticObjects.ProjectMenu.Item(Names.Menu.MenuDefensiveItemBase + "Boolean.ComboOnly").GetValue<bool>())
                    if (_defensive.Merc.IsReady())
                        foreach (var buff in
                            Buffs.GetTypes.Where(buff => StaticObjects.ProjectMenu.Item(Names.Menu.MenuDefensiveItemBase + "Boolean.Merc." + buff).GetValue<bool>()))
                            if (StaticObjects.Player.HasBuffOfType(buff))
                            {
                                StaticObjects.ProjectLogger.WriteLog($"Use Merc Reason {buff}");
                                LeagueSharp.Common.Utility.DelayAction.Add(StaticObjects.ProjectMenu.Item(Names.Menu.MenuDefensiveItemBase + "Slider.Merc.Delay").GetValue<Slider>().Value, () => EloBuddy.SDK.Item.UseItem(_defensive.Qss.Id));
                            }

            #endregion Defensive
        }

        #endregion Public Methods

        #region Private Fields

        private readonly Defensive _defensive;
        private readonly Offensive _offensive;

        #endregion Private Fields

        #region Private Methods

        private void After_Attack(AttackableUnit unit, AttackableUnit target) { }
        private void Before_Attack(Orbwalking.BeforeAttackEventArgs args) { }

        #endregion Private Methods
    }

}