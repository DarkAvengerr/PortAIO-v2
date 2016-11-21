using EloBuddy; 
using LeagueSharp.Common; 
 namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Ignite : IPlugin
    {
        #region Public Properties

        public static Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        public Spell IgniteSpell { get; set; }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonerdot") == SpellSlot.Unknown)
            {
                return;
            }
            var predicate = new Func<Menu, bool>(x => x.Name == "SummonersMenu");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Summoners", "SummonersMenu"))
                           : rootMenu.Children.First(predicate);

            var igniteMenu = menu.AddSubMenu(new Menu("Ignite", "Ignite"));
            {
                igniteMenu.AddItem(new MenuItem("Ignite.Activated", "Ignite").SetValue(true));
                foreach (var x in HeroManager.Enemies)
                {
                    igniteMenu.AddItem(new MenuItem($"igniteon{x.ChampionName}", "Use on " + x.ChampionName))
                        .SetValue(true);
                }

                igniteMenu.SubMenu("Do not use ignite when").AddItem(new MenuItem("Block.Q", "Q is ready").SetValue(false));
                igniteMenu.SubMenu("Do not use ignite when").AddItem(new MenuItem("Block.W", "W is ready").SetValue(false));
                igniteMenu.SubMenu("Do not use ignite when").AddItem(new MenuItem("Block.E", "E is ready").SetValue(false));
                igniteMenu.SubMenu("Do not use ignite when").AddItem(new MenuItem("Block.R", "R is ready").SetValue(false));
            }

            Menu = igniteMenu;
        }

        /// <summary>
        /// Loads this instance
        /// </summary>
        public void Load()
        {
            try
            {
                var igniteSlot = this.Player.GetSpellSlot("summonerdot");

                if (igniteSlot == SpellSlot.Unknown)
                {
                    return;
                }

                this.IgniteSpell = new Spell(igniteSlot);

                Game.OnUpdate += this.OnUpdate;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        private void IgniteKs()
        {
            try
            {
                if (!Menu.Item("Ignite.Activated").IsActive())
                {
                    return;
                }

                if (Menu.Item("Block.Q").IsActive() && this.Player.Spellbook.GetSpell(SpellSlot.Q).State == SpellState.Ready)
                {
                    return;
                }

                if (Menu.Item("Block.W").IsActive() && this.Player.Spellbook.GetSpell(SpellSlot.W).State == SpellState.Ready)
                {
                    return;
                }

                if (Menu.Item("Block.E").IsActive() && this.Player.Spellbook.GetSpell(SpellSlot.E).State == SpellState.Ready)
                {
                    return;
                }

                if (Menu.Item("Block.R").IsActive() && this.Player.Spellbook.GetSpell(SpellSlot.R).State == SpellState.Ready)
                {
                    return;
                }

                var kSableEnemy =
                    HeroManager.Enemies.FirstOrDefault(
                        hero =>
                        hero.IsValidTarget(600) && !hero.IsZombie
                        && this.Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite) > hero.Health);


                if (kSableEnemy != null)
                {
                   /* if (this.Player.CanAttack && this.Player.Distance(kSableEnemy) < Orbwalking.GetAttackRange(this.Player) 
                        && this.Player.GetAutoAttackDamage(kSableEnemy) * 2 > kSableEnemy.Health)
                    {
                        return;
                    }
                    */
                    if (!Menu.Item($"igniteon{kSableEnemy.ChampionName}").IsActive())
                    {
                        return;
                    }

                    this.Player.Spellbook.CastSpell(this.IgniteSpell.Slot, kSableEnemy);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (this.Player.IsDead)
                {
                    return;
                }

                this.IgniteKs();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}
