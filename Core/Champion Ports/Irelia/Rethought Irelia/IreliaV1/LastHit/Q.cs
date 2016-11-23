//     Copyright (C) 2016 Rethought
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
//     Created: 04.10.2016 1:05 PM
//     Last Edited: 04.10.2016 1:44 PM

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Rethought_Irelia.IreliaV1.LastHit
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;

    using Rethought_Irelia.IreliaV1.Spells;

    #endregion

    internal class Q : OrbwalkingChild
    {
        #region Fields

        /// <summary>
        ///     The irelia q
        /// </summary>
        private readonly IreliaQ ireliaQ;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Q" /> class.
        /// </summary>
        /// <param name="ireliaQ">The Q logic</param>
        public Q(IreliaQ ireliaQ)
        {
            this.ireliaQ = ireliaQ;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "Q";

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= this.OnGameUpdate;
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += this.OnGameUpdate;
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            this.Menu.AddItem(
                new MenuItem(this.Path + "." + "clearmode", "Clear Mode: ").SetValue(
                    new StringList(new[] { "Unkillable", "Always" })));

            this.Menu.AddItem(new MenuItem(this.Path + "." + "noturretdive", "Don't dive turrets").SetValue(true));

            this.Menu.AddItem(new MenuItem("eplxaination", "Don't get closer than that to: "));

            foreach (var enemy in HeroManager.Enemies)
                this.Menu.AddItem(
                    new MenuItem(this.Path + "." + enemy.ChampionName, enemy.ChampionName).SetValue(
                        new Slider(300, 0, 1000)));
        }

        private void LogicJungleLastHit()
        {
            var units =
                MinionManager.GetMinions(
                    this.ireliaQ.Spell.Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.Health).Where(x => this.ireliaQ.WillReset(x)).ToList();
            var unit = units.FirstOrDefault();

            if (unit == null) return;

            this.ireliaQ.Spell.Cast(unit);
        }

        private void LogicLaneLastHit()
        {
            var units =
                MinionManager.GetMinions(this.ireliaQ.Spell.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(x => this.ireliaQ.WillReset(x))
                    .ToList();

            if (this.Menu.Item(this.Path + "." + "noturretdive").GetValue<bool>()) foreach (var unit2 in units.ToList()) if (unit2.UnderTurret(true)) units.Remove(unit2);

            foreach (var enemy in
                HeroManager.Enemies.Where(
                    x => !x.IsDead && (x.Distance(ObjectManager.Player) <= 1000 + this.ireliaQ.Spell.Range)))
                foreach (var entry in units.ToList())
                    if (entry.Distance(enemy)
                        <= this.Menu.Item(this.Path + "." + enemy.ChampionName).GetValue<Slider>().Value) units.Remove(entry);

            Obj_AI_Base unit = null;

            switch (this.Menu.Item(this.Path + "." + "clearmode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    unit =
                        units.FirstOrDefault(
                            x =>
                                (x.Distance(ObjectManager.Player) >= ObjectManager.Player.AttackRange)
                                && (x.HealthPercent <= 10));
                    break;
                case 1:
                    unit = units.FirstOrDefault();
                    break;
            }

            if (unit != null) this.ireliaQ.Spell.Cast(unit);
        }

        /// <summary>
        ///     Raises the <see cref="E:GameUpdate" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnGameUpdate(EventArgs args)
        {
            if (!this.CheckGuardians()) return;

            this.LogicLaneLastHit();

            this.LogicJungleLastHit();
        }

        #endregion
    }
}