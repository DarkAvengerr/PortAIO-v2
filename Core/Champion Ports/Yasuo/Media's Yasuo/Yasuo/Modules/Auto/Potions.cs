using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Modules.Auto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::YasuoMedia.CommonEx;
    using global::YasuoMedia.CommonEx.Classes;
    using global::YasuoMedia.CommonEx.Utility;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Potions : FeatureChild<Modules>
    {
        #region Fields

        /// <summary>
        ///     The potions
        /// </summary>
        private readonly List<PotionStruct> potions = new List<PotionStruct>
        {
            //TODO: I forgot refillable potion

            // Jungle version of Crystal Flask
            new PotionStruct(
                "ItemCrystalFlaskJungle",
                (ItemId)2032,
                3,
                12,
                120,
                0,
                true,
                new[] { PotionMode.Delayed }),

            // Dark Crystal Flask
            new PotionStruct(
                "ItemDarkCrystalFlask",
                (ItemId)2033,
                3,
                12,
                60,
                0,
                true,
                new[] { PotionMode.Delayed }),

            // Normal Health Potion
            new PotionStruct(
                "RegenerationPotion",
                (ItemId)2003,
                1,
                15,
                150,
                0,
                false,
                new[] { PotionMode.Delayed }),

            // Cookies
            new PotionStruct(
                "ItemMiniRegenPotion",
                (ItemId)2052,
                1,
                15,
                150,
                20,
                false,
                new[]
                    {
                        PotionMode.Delayed,
                        PotionMode.Instant
                    })
        };

        /// <summary>
        ///     The dots
        /// </summary>
        private List<string> dots = new List<string>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Potions" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public Potions(Modules parent)
            : base(parent)
        {
            this.OnLoad();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name => "Potions";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Executes the specified item identifier.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        public void Execute(ItemId itemId)
        {
            InventorySlot first = null;
            foreach (var potion in this.potions)
            {
                foreach (var item in ObjectManager.Player.InventoryItems.Where(item => itemId == potion.ItemId))
                {
                    first = item;
                    break;
                }
            }
            GlobalVariables.Player.Spellbook.CastSpell(first.SpellSlot);
        }

        /// <summary>
        ///     Raises the <see cref="E:Draw" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnDraw(EventArgs args)
        {
        }

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnUpdate(EventArgs args)
        {
            var availablePotions = new List<PotionStruct>();

            // Loops trough all items in the current inventory and adds all potions to the availablePotions list.
            foreach (var item in GlobalVariables.Player.InventoryItems)
            {
                if (this.potions.Any(x => x.ItemId == item.Id))
                {
                    availablePotions.Add(this.potions.FirstOrDefault(x => x.ItemId == item.Id));
                }
            }

            // No potions, our Health is near Max Health or it is Max Health, or we are recalling or teleporting, or we are in Fountain
            if (availablePotions.Count == 0
                || GlobalVariables.Player.Health
                == GlobalVariables.Player.MaxHealth - GlobalVariables.Player.HPRegenRate * 2
                || GlobalVariables.Player.InFountain()
                || GlobalVariables.Player.Buffs.Any(
                    buff =>
                    buff.Name.Contains("Recall") || buff.Name.Contains("Teleport") || buff.Name.Contains("Healing")))
            {
                return;
            }

            // Anti Damage over Time
            // TODO: Write Wrapper to access .json file containing most debuffs and dots more easily
            if (this.Menu.Item(this.Name + "AutoDOTS").GetValue<bool>())
            {
                // PSEUDO CODE
                //foreach (var debuff in SDK.SpellDatabase.Spells.Where(debuff => Variables.Player.HasBuff(debuff.AppliedBuffName)))
                //{
                //    var potion = potions.FirstOrDefault(x => x.HealValue > debuff && x.Time < debuff.Time);
                //    if (potion != null)
                //    {
                //        this.Execute(potion.ItemId);
                //    }
                //}
            }

            // Auto use on low X% Health with X enemies near
            if (GlobalVariables.Player.HealthPercent
                >= this.Menu.Item(this.Name + "MinHealthPercentage").GetValue<Slider>().Value
                && GlobalVariables.Player.CountEnemiesInRange(
                    this.Menu.Item(this.Name + "Range").GetValue<Slider>().Value)
                >= this.Menu.Item(this.Name + "MinEnemies").GetValue<Slider>().Value)
            {
                if (availablePotions.Any(x => x.AutoRefill)
                    && this.Menu.Item(this.Name + "AutoRefillableFirst").GetValue<bool>())
                {
                    this.Execute(availablePotions.FirstOrDefault(x => x.AutoRefill).ItemId);
                }
                else
                {
                    this.Execute(availablePotions.MaxOrDefault(x => x.HealValue).ItemId);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable()
        {
            Events.OnUpdate -= this.OnUpdate;
            Drawing.OnDraw -= this.OnDraw;
            base.OnDisable();
        }

        /// <summary>
        ///     Called when [enable].
        /// </summary>
        protected override void OnEnable()
        {
            Events.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            base.OnEnable();
        }

        /// <summary>
        ///     Called when [initialize].
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);
            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Menu.AddItem(
                new MenuItem(this.Name + "MinHealthPercentage", "Min Health %").SetValue(new Slider(35, 0, 99)));
            this.Menu.AddItem(new MenuItem(this.Name + "MinEnemies", "Min Enemies").SetValue(new Slider(1, 1, 5)));
            this.Menu.AddItem(new MenuItem(this.Name + "Range", "In Range Of").SetValue(new Slider(1000, 0, 4000)));
            this.Menu.AddItem(
                new MenuItem(this.Name + "AutoRefillableFirst", "Use potions that will refill first").SetValue(true)
                    .SetTooltip(
                        "If this is enabled and the assembly wants to cast a potion it will use potions that will refill at base first. Hint: It is worth enabling this, because you can sell unused potions then!"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "AutoDOTS", "Auto Heal against DOTs").SetValue(true)
                    .SetTooltip(
                        "If this is enabled and the player is for example ignited the assemblie will use a potion if you won't die from the DOT (Damage Over Time) anyway"));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        #endregion
    }

    /// <summary>
    ///     Potion Mode
    /// </summary>
    public enum PotionMode
    {
        Instant,

        Delayed
    }

    // TODO: Move to Data
    /// <summary>
    ///     Potion Data
    /// </summary>
    public struct PotionStruct
    {
        #region Fields

        /// <summary>
        ///     The automatic refill
        /// </summary>
        public readonly bool AutoRefill;

        /// <summary>
        ///     The buff name
        /// </summary>
        public readonly string BuffName;

        /// <summary>
        ///     The heal value
        /// </summary>
        public readonly int HealValue;

        /// <summary>
        ///     The instant heal value
        /// </summary>
        public readonly int InstantHealValue;

        /// <summary>
        ///     The item identifier
        /// </summary>
        public readonly ItemId ItemId;

        /// <summary>
        ///     The minimum charges
        /// </summary>
        public readonly int MinCharges;

        /// <summary>
        ///     The modes
        /// </summary>
        public readonly PotionMode[] Modes;

        /// <summary>
        ///     The time
        /// </summary>
        public readonly int Time;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PotionStruct" /> struct.
        /// </summary>
        /// <param name="buffName">Name of the buff.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="minCharges">The minimum charges.</param>
        /// <param name="time">The time.</param>
        /// <param name="healValue">The heal value.</param>
        /// <param name="instanHealValue">The instan heal value.</param>
        /// <param name="autoRefill">if set to <c>true</c> [automatic refill].</param>
        /// <param name="modes">The modes.</param>
        public PotionStruct(
            string buffName,
            ItemId itemId,
            int minCharges,
            int time,
            int healValue,
            int instanHealValue,
            bool autoRefill,
            PotionMode[] modes)
        {
            this.BuffName = buffName;
            this.ItemId = itemId;
            this.MinCharges = minCharges;
            this.Time = time;
            this.HealValue = healValue;
            this.InstantHealValue = instanHealValue;
            this.AutoRefill = autoRefill;
            this.Modes = modes;
        }

        #endregion
    }
}