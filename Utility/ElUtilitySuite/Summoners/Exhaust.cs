using EloBuddy; namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;

    public class Exhaust : IPlugin
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the exhaust spell.
        /// </summary>
        /// <value>
        ///     The exhaust spell.
        /// </value>
        public Spell ExhaustSpell { get; set; }

        /// <summary>
        ///     The Menu
        /// </summary>
        public Menu Menu { get; set; }

        #endregion

        #region Properties

        private Dictionary<string, List<SpellSlot>> ChampionSpells { get; } = new Dictionary<string, List<SpellSlot>>();

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        private List<SpellDatabaseEntry> Spells { get; } = new List<SpellDatabaseEntry>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonerexhaust") == SpellSlot.Unknown)
            {
                return;
            }

            var predicate = new Func<Menu, bool>(x => x.Name == "SummonersMenu");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Summoners", "SummonersMenu"))
                           : rootMenu.Children.First(predicate);

            var exhaustMenu = menu.AddSubMenu(new Menu("Exhaust", "Exhaust"));
            {
                exhaustMenu.AddItem(new MenuItem("Exhaust.Activated", "Exhaust activated").SetValue(false));
                exhaustMenu.AddItem(new MenuItem("blank_line3000", ""));
                foreach (var x in HeroManager.Enemies)
                {
                    exhaustMenu.AddItem(
                        new MenuItem($"exhauston{x.CharData.BaseSkinName}", "Use for " + x.ChampionName)).SetValue(true);
                }
            }

            this.Menu = exhaustMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            try
            {
                var exhaustSlot = this.Player.GetSpellSlot("summonerexhaust");
                if (exhaustSlot == SpellSlot.Unknown)
                {
                    return;
                }

                this.ExhaustSpell = new Spell(exhaustSlot, 550f);

                Obj_AI_Base.OnProcessSpellCast += this.ObjAiBaseOnProcessSpellCast;

                var dangerousSpells =
                    Data.Get<SpellDatabase>()
                        .Spells.Where(
                            x =>
                            x.DangerValue >= 5
                            && HeroManager.Enemies.Any(
                                y => x.ChampionName.Equals(y.ChampionName, StringComparison.InvariantCultureIgnoreCase)))
                        .Select(x => x.SpellName)
                        .ToList();

                foreach (var source in dangerousSpells)
                {
                    var spellName = source;
                    this.Spells.Add(Data.Get<SpellDatabase>().Spells.First(x => x.SpellName.Equals(source)));
                    if (!dangerousSpells.Contains(spellName))
                    {
                        continue;
                    }
                }

                foreach (var spell in this.Spells)
                {
                    if (!this.ChampionSpells.ContainsKey(spell.ChampionName))
                    {
                        this.ChampionSpells[spell.ChampionName] = new List<SpellSlot>();
                    }

                    this.ChampionSpells[spell.ChampionName].Add(spell.Slot);
                }

                Random = new Random(Environment.TickCount);
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed to load exhaust");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets or sets the random.
        /// </summary>
        /// <value>
        ///     The random.
        /// </value>
        private static Random Random { get; set; }

        private void ObjAiBaseOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy || !this.ExhaustSpell.IsInRange(sender)
                || !this.Spells.Any(
                    x => x.SpellName.Equals(args.SData.Name, StringComparison.InvariantCultureIgnoreCase))
                || !this.Menu.Item($"exhauston{sender.CharData.BaseSkinName}").IsActive() || args.SData.Name.ToLower().Contains("lux"))
            {
                return;
            }

            if (this.ExhaustSpell.IsReady())
            {
               LeagueSharp.Common.Utility.DelayAction.Add(Random.Next(50, 100), () => this.ExhaustSpell.Cast(sender));
               Console.WriteLine($"Use exhaust on: {sender.CharData.BaseSkinName} - for spell {args.SData.Name}");
            }
        }

        #endregion
    }
}