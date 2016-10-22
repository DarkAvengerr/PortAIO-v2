// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivatorBase.cs" company="SurvivorSeriesAIO">
//     Copyright (c) SurvivorSeriesAIO. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Core
{
    public abstract class ActivatorBase : IActivator
    {
        protected ActivatorBase(IRootMenu menu)
        {
            Menu = menu;

            SummonerDot = new Spell(ObjectManager.Player.GetSpellSlot("SummonerDot"), 550);
            HPPotion = new Items.Item(2003, 0);
            Flask = new Items.Item(2041, 0);
            Biscuit = new Items.Item(2010, 0);
            FlaskRef = new Items.Item(2031, 0);
            FlaskHunterJG = new Items.Item(2032, 0);
            FlaskCorruptJG = new Items.Item(2033, 0);

            Protobelt = new Items.Item(3152, 850f);
            GLP800 = new Items.Item(3030, 800f);
            Hextech = new Items.Item(3146, 700f);

            Seraph = new Items.Item(3040, 0);
        }

        protected IRootMenu Menu { get; }

        protected AIHeroClient Player { get; } = ObjectManager.Player;

        public Items.Item Biscuit { get; }
        public Items.Item Flask { get; }
        public Items.Item FlaskCorruptJG { get; }
        public Items.Item FlaskHunterJG { get; }
        public Items.Item FlaskRef { get; }
        public Items.Item GLP800 { get; }
        public Items.Item Hextech { get; }
        public Items.Item HPPotion { get; }
        public SpellSlot IgniteSpell { get; private set; }
        public Items.Item Protobelt { get; }
        public Items.Item Seraph { get; }
        public object SpellSlot { get; private set; }
        public Spell SummonerDot { get; }

        //public abstract void UseItems();
    }
}