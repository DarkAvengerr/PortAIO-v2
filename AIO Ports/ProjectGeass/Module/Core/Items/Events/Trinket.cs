using System;
using LeagueSharp;
using LeagueSharp.Common;
using _Project_Geass.Data.Items;
using _Project_Geass.Functions;
using _Project_Geass.Humanizer.TickTock;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Core.Items.Events
{

    internal class Trinket
    {
        #region Private Fields

        /// <summary>
        ///     Initializes a new instance of the <see cref="Trinket" /> class.
        /// </summary>
        private readonly Trinkets _trinket;

        #endregion Private Fields

        #region Public Constructors

        public Trinket()
        {
            _trinket=new Trinkets();
            Game.OnUpdate+=OnUpdate;
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        private void OnUpdate(EventArgs args)
        {
            if (!Handler.CheckTrinket())
                return;

            if (!StaticObjects.ProjectMenu.Item(Names.Menu.TrinketItemBase+"Boolean.BuyOrb").GetValue<bool>())
                return;

            Handler.UseTrinket();

            if (StaticObjects.Player.Level<9)
                return;
            if (!StaticObjects.Player.InShop()||LeagueSharp.Common.Items.HasItem(_trinket.Orb.Id))
                return;

            StaticObjects.ProjectLogger.WriteLog("Buy Orb");
            _trinket.Orb.Buy();
        }

        #endregion Private Methods
    }

}