using EloBuddy; 
using LeagueSharp.Common; 
namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using ElUtilitySuite.Logging;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Color = SharpDX.Color;
    using ItemData = LeagueSharp.Common.Data.ItemData;

    public class Cleanse2 : IPlugin
    {
        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        ///     Gets the buff indexes handled.
        /// </summary>
        /// <value>
        ///     The buff indexes handled.
        /// </value>
        private Dictionary<int, List<int>> BuffIndexesHandled { get; } = new Dictionary<int, List<int>>();

        /// <summary>
        ///     Gets or sets the buffs to cleanse.
        /// </summary>
        /// <value>
        ///     The buffs to cleanse.
        /// </value>
        private IEnumerable<BuffType> BuffsToCleanse { get; set; }

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        private List<CleanseItem> Items { get; set; }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private static Menu Menu { get; set; }

        /// <summary>
        ///     Gets or sets the random.
        /// </summary>
        /// <value>
        ///     The random.
        /// </value>
        private Random Random { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the combo mode is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if combo mode is active; otherwise, <c>false</c>.
        /// </value>
        public bool ComboModeActive
            =>
            Entry.Menu.Item("usecombo").GetValue<KeyBind>().Active
            || Orbwalking.Orbwalker.Instances.Any(x => x.ActiveMode == Orbwalking.OrbwalkingMode.Combo);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public static List<CleanseIgnore> Spells { get; set; }

        /// <summary>
        ///     Long hair dont care
        /// </summary>
        public static readonly List<string> TrueStandard = new List<string>
                                                               {
                                                                   "Stun", "Charm", "Flee", "Fear", "Taunt",
                                                                   "Polymorph", "Suppression"
                                                               };

        /// <summary>
        ///     Credits to Exory
        ///     Invalid snare buff casters
        /// </summary>
        public static readonly List<string> InvalidSnareCasters = new List<string> { "Leona", "Zyra", "Lissandra" };

        /// <summary>
        ///     Invalid stun buff casters
        /// </summary>
        public static readonly List<string> InvalidStunCasters = new List<string>
                                                                     {
                                                                         "Amumu", "LeeSin", "Alistar", "Hecarim",
                                                                         "Blitzcrank"
                                                                     };

        /// <summary>
        ///     Invalid root buff casters
        /// </summary>
        public static readonly List<string> InvalidRootCasters = new List<string> { "Caitlyn" };

        /// <summary>
        ///     Gets a value indicating the cleanse version
        /// </summary>
        /// <value>
        ///     <c>true</c> if Cleanse.Version is active; otherwise, <c>false</c>.
        /// </value>
        public static int CleansingVersion => Menu.Item("Cleanse.Version").GetValue<StringList>().SelectedIndex;

        /// <summary>
        ///     Initializes the <see cref="Cleanse" /> class.
        /// </summary>
        static Cleanse2()
        {
            Spells = new List<CleanseIgnore>
                         {
                             new CleanseIgnore { Champion = "Ashe", Spellname = "frostarrow" },
                             new CleanseIgnore { Champion = "Ashe", Spellname = "ashepassiveslow" },
                             new CleanseIgnore { Champion = "Vi", Spellname = "vir" },
                             new CleanseIgnore { Champion = "Yasuo", Spellname = "yasuorknockupcombo" },
                             new CleanseIgnore { Champion = "Yasuo", Spellname = "yasuorknockupcombotar" },
                             new CleanseIgnore { Champion = "Zyra", Spellname = "zyrabramblezoneknockup" },
                             new CleanseIgnore { Champion = "Velkoz", Spellname = "velkozresearchstack" },
                             new CleanseIgnore { Champion = "Darius", Spellname = "dariusaxebrabcone" },
                             new CleanseIgnore { Champion = "Fizz", Spellname = "fizzmoveback" },
                             new CleanseIgnore { Champion = "Blitzcrank", Spellname = "rocketgrab2" },
                             new CleanseIgnore { Champion = "Alistar", Spellname = "pulverize" },
                             new CleanseIgnore { Champion = "Azir", Spellname = "azirqslow" },
                             new CleanseIgnore { Champion = "Rammus", Spellname = "powerballslow" },
                             new CleanseIgnore { Champion = "Rammus", Spellname = "powerballstun" },
                             new CleanseIgnore { Champion = "MonkeyKing", Spellname = "monkeykingspinknockup" },
                             new CleanseIgnore { Champion = "Alistar", Spellname = "headbutttarget" },
                             new CleanseIgnore { Champion = "Hecarim", Spellname = "hecarimrampstuncheck" },
                             new CleanseIgnore { Champion = "Hecarim", Spellname = "hecarimrampattackknockback" },
                             new CleanseIgnore { Spellname = "frozenheartaura" },
                             new CleanseIgnore { Spellname = "frozenheartauracosmetic" },
                             new CleanseIgnore { Spellname = "itemsunfirecapeaura" },
                             new CleanseIgnore { Spellname = "blessingofthelizardelderslow" },
                             new CleanseIgnore { Spellname = "dragonburning" },
                             new CleanseIgnore { Spellname = "chilled" }
                         };
        }

        /// <summary>
        ///     Represents a spell that cleanse can be used on.
        /// </summary>
        public class CleanseIgnore
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the champion.
            /// </summary>
            /// <value>
            ///     The champion.
            /// </value>
            public string Champion { get; set; }

            /// <summary>
            ///     Gets or sets the spellname.
            /// </summary>
            /// <value>
            ///     The spellname.
            /// </value>
            public string Spellname { get; set; }

            #endregion
        }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        public void CreateMenu(Menu rootMenu)
        {
            this.CreateItems();
            this.BuffsToCleanse = this.Items.SelectMany(x => x.WorksOn).Distinct();

            Menu = new Menu("Cleanse/QSS", "BuffTypeStyleCleanser").SetFontStyle(FontStyle.Bold, Color.Red);
            {
                var newCleanseMenu = Menu.SubMenu("Cleanse NEW").SetFontStyle(FontStyle.Bold, Color.Green);

                newCleanseMenu.SubMenu("Humanizer Delay")
                    .AddItem(
                        new MenuItem("MinHumanizerDelay", "Min Humanizer Delay (MS)").SetValue(new Slider(100, 0, 500)));
                newCleanseMenu.SubMenu("Humanizer Delay")
                    .AddItem(
                        new MenuItem("MaxHumanizerDelay", "Max Humanizer Delay (MS)").SetValue(new Slider(150, 0, 500)));
                newCleanseMenu.SubMenu("Humanizer Delay")
                    .AddItem(new MenuItem("HumanizerEnabled", "Enabled").SetValue(false));

                foreach (var buffType in this.BuffsToCleanse.Select(x => x.ToString()))
                {
                    newCleanseMenu.SubMenu("Buff Types")
                        .AddItem(
                            new MenuItem($"3Cleanse{buffType}", buffType).SetValue(TrueStandard.Contains($"{buffType}")));
                }

                newCleanseMenu.AddItem(
                        new MenuItem("MinDuration", "Minimum Duration (MS)").SetValue(new Slider(500, 0, 25000)))
                    .SetTooltip("The minimum duration of the spell to get cleansed");

                newCleanseMenu.AddItem(new MenuItem("CleanseEnabled.ComboOnly", "Only use in combo").SetValue(false));

                newCleanseMenu.AddItem(new MenuItem("CleanseEnabled.Health", "Cleanse on health").SetValue(false));
                newCleanseMenu.AddItem(
                    new MenuItem("Cleanse.HealthPercent", "Cleanse when HP <=").SetValue(new Slider(75)));

                newCleanseMenu.AddItem(new MenuItem("CleanseEnabled", "Enabled").SetValue(true))
                    .SetTooltip("Settings also apply on mikael's crucible.");
                newCleanseMenu.AddItem(new MenuItem("sep-112-cleanse", "Settings:"))
                    .SetFontStyle(FontStyle.Bold, Color.GreenYellow)
                    .SetTooltip("Counts for QSS and Mikaels usage");

                foreach (var allies in HeroManager.Allies)
                {
                    newCleanseMenu.AddItem(
                            new MenuItem($"3cleanseon{allies.ChampionName}", "Use on " + allies.ChampionName))
                        .SetValue(true);
                }

                Menu.AddItem(new MenuItem("Cleanse.Version", "Cleanse preference:"))
                    .SetValue(new StringList(new[] { "Old", "New", }, 1));
            }

            rootMenu.AddSubMenu(Menu);
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Random = new Random(Environment.TickCount);
            HeroManager.Allies.ForEach(x => this.BuffIndexesHandled.Add(x.NetworkId, new List<int>()));

            Game.OnUpdate += this.OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the items.
        /// </summary>
        private void CreateItems()
        {
            this.Items = new List<CleanseItem>
                             {
                                 new CleanseItem
                                     {
                                         Slot =
                                             () =>
                                                 Player.GetSpellSlot("summonerboost") == SpellSlot.Unknown
                                                     ? SpellSlot.Unknown
                                                     : Player.GetSpellSlot("summonerboost"),
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                     BuffType.Polymorph, BuffType.Silence, BuffType.Snare, BuffType.Stun,
                                                     BuffType.Taunt, BuffType.Damage
                                                 },
                                         Priority = 2
                                     },
                                 new CleanseItem
                                     {
                                         Slot = () =>
                                             {
                                                 var slots = ItemData.Quicksilver_Sash.GetItem().Slots;
                                                 return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                             },
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                     BuffType.Polymorph, BuffType.Silence, BuffType.Snare, BuffType.Stun,
                                                     BuffType.Taunt, BuffType.Damage, BuffType.Suppression
                                                 },
                                         Priority = 0
                                     },
                                 new CleanseItem
                                     {
                                         Slot = () =>
                                             {
                                                 var slots = ItemData.Dervish_Blade.GetItem().Slots;
                                                 return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                             },
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                     BuffType.Polymorph, BuffType.Silence, BuffType.Snare, BuffType.Stun,
                                                     BuffType.Taunt, BuffType.Damage, BuffType.Suppression
                                                 },
                                         Priority = 0
                                     },
                                 new CleanseItem
                                     {
                                         Slot = () =>
                                             {
                                                 var slots = ItemData.Mercurial_Scimitar.GetItem().Slots;
                                                 return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                             },
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                     BuffType.Polymorph, BuffType.Silence, BuffType.Snare, BuffType.Stun,
                                                     BuffType.Taunt, BuffType.Damage, BuffType.Suppression
                                                 },
                                         Priority = 0
                                     },
                                 new CleanseItem
                                     {
                                         Slot = () =>
                                             {
                                                 var slots = ItemData.Mikaels_Crucible.GetItem().Slots;
                                                 return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                             },
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Stun, BuffType.Snare, BuffType.Taunt, BuffType.Silence,
                                                     BuffType.Slow, BuffType.Fear, BuffType.Suppression
                                                 },
                                         WorksOnAllies = true, Priority = 1
                                     }
                             };
        }

        /// <summary>
        ///     Gets the best cleanse item.
        /// </summary>
        /// <param name="ally">The ally.</param>
        /// <param name="buff">The buff.</param>
        /// <returns></returns>
        private Spell GetBestCleanseItem(GameObject ally, BuffInstance buff)
        {
            return
                this.Items.OrderBy(x => x.Priority)
                    .Where(x => x.WorksOn.Any(y => buff.Type.HasFlag(y)))
                    .Where(x => ally.IsMe || x.WorksOnAllies)
                    .Where(x => x.Spell.IsReady() && x.Spell.IsInRange(ally) && x.Spell.Slot != SpellSlot.Unknown)
                    .Select(item => item.Spell)
                    .FirstOrDefault();
        }

        private void OnUpdate(EventArgs args)
        {
            if (CleansingVersion == 0 || !Menu.Item("CleanseEnabled").IsActive())
            {
                return;
            }

            if (Menu.Item("CleanseEnabled.ComboOnly").IsActive() && !this.ComboModeActive)
            {
                return;
            }

            foreach (var ally in HeroManager.Allies.Where(x => x.IsValidTarget(800f, false)))
            {
                foreach (var buff in
                    ally.Buffs.Where(
                        x =>
                            this.BuffsToCleanse.Contains(x.Type) && x.Caster.Type == GameObjectType.AIHeroClient
                            && x.Caster.IsEnemy))
                {
                    if (!Menu.Item($"3Cleanse{buff.Type}").IsActive()
                        || Menu.Item("MinDuration").GetValue<Slider>().Value / 1000f > buff.EndTime - buff.StartTime
                        || this.BuffIndexesHandled[ally.NetworkId].Contains(buff.Index)
                        || Spells.Any(
                            b =>
                                buff.Name.Equals(b.Spellname, StringComparison.InvariantCultureIgnoreCase)
                                || !Menu.Item($"3cleanseon{ally.ChampionName}").IsActive()) || buff.Type == BuffType.Knockback || buff.Type == BuffType.Knockup)
                    {
                        continue;
                    }

                    if (buff.Type == BuffType.Snare
                        && InvalidRootCasters.Contains(
                            ((AIHeroClient)buff.Caster).ChampionName,
                            StringComparer.InvariantCultureIgnoreCase)
                        || buff.Type == BuffType.Snare
                        && InvalidSnareCasters.Contains(
                            ((AIHeroClient)buff.Caster).ChampionName,
                            StringComparer.InvariantCultureIgnoreCase)
                        || buff.Type == BuffType.Stun
                        && InvalidStunCasters.Contains(
                            ((AIHeroClient)buff.Caster).ChampionName,
                            StringComparer.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    var cleanseItem = this.GetBestCleanseItem(ally, buff);
                    if (cleanseItem == null)
                    {
                        continue;
                    }

                    this.BuffIndexesHandled[ally.NetworkId].Add(buff.Index);

                    if (Menu.Item("HumanizerEnabled").IsActive())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            (int)
                            Math.Min(
                                this.Random.Next(
                                    Menu.Item("MinHumanizerDelay").GetValue<Slider>().Value,
                                    Menu.Item("MaxHumanizerDelay").GetValue<Slider>().Value),
                                (buff.StartTime - buff.EndTime) * 1000),
                            () =>
                                {
                                    if (Menu.Item("CleanseEnabled.Health").IsActive())
                                    {
                                        if (
                                            !(Menu.Item("Cleanse.HealthPercent").GetValue<Slider>().Value
                                              <= ObjectManager.Player.HealthPercent))
                                        {
                                            return;
                                        }

                                        cleanseItem.Cast(ally);
                                        this.BuffIndexesHandled[ally.NetworkId].Remove(buff.Index);
                                    }
                                    else
                                    {
                                        cleanseItem.Cast(ally);
                                        this.BuffIndexesHandled[ally.NetworkId].Remove(buff.Index);
                                    }
                                });
                    }
                    else
                    {
                        if (Menu.Item("CleanseEnabled.Health").IsActive())
                        {
                            if (
                                !(Menu.Item("Cleanse.HealthPercent").GetValue<Slider>().Value
                                  <= ObjectManager.Player.HealthPercent))
                            {
                                continue;
                            }

                            cleanseItem.Cast(ally);
                            this.BuffIndexesHandled[ally.NetworkId].Remove(buff.Index);
                        }
                        else
                        {
                            cleanseItem.Cast(ally);
                            this.BuffIndexesHandled[ally.NetworkId].Remove(buff.Index);
                        }
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     An item/spell that can be used to cleanse a spell.
    /// </summary>
    public class CleanseItem
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the priority.
        /// </summary>
        /// <value>
        ///     The priority.
        /// </value>
        public int Priority { get; set; }

        /// <summary>
        ///     Gets or sets the range.
        /// </summary>
        /// <value>
        ///     The range.
        /// </value>
        public float Range { get; set; } = float.MaxValue;

        /// <summary>
        ///     Gets or sets the slot delegate.
        /// </summary>
        /// <value>
        ///     The slot delegate.
        /// </value>
        public Func<SpellSlot> Slot { get; set; }

        /// <summary>
        ///     Gets or sets the spell.
        /// </summary>
        /// <value>
        ///     The spell.
        /// </value>
        public Spell Spell => new Spell(this.Slot(), this.Range);

        /// <summary>
        ///     Gets or sets what the spell works on.
        /// </summary>
        /// <value>
        ///     The buff types the spell works on.
        /// </value>
        public BuffType[] WorksOn { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the spell works on allies.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the spell works on allies; otherwise, <c>false</c>.
        /// </value>
        public bool WorksOnAllies { get; set; } = false;

        #endregion
    }
}