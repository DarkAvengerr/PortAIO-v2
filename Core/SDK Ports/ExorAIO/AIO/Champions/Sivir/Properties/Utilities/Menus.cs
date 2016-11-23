
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Sivir
{
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The menu class.
    /// </summary>
    internal class Menus
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Sets the menu.
        /// </summary>
        public static void Initialize()
        {
            /// <summary>
            ///     Sets the spells menu.
            /// </summary>
            Vars.SpellsMenu = new Menu("spells", "Spells");
            {
                /// <summary>
                ///     Sets the menu for the Q.
                /// </summary>
                Vars.QMenu = new Menu("q", "Use Q to:");
                {
                    Vars.QMenu.Add(new MenuBool("combo", "Combo", true));
                    Vars.QMenu.Add(new MenuBool("killsteal", "KillSteal", true));
                    Vars.QMenu.Add(new MenuBool("logical", "Logical", true));
                    Vars.QMenu.Add(new MenuSliderButton("harass", "Harass / if Mana >= x%", 50, 0, 99, true));
                    Vars.QMenu.Add(new MenuSliderButton("laneclear", "LaneClear / if Mana >= x%", 50, 0, 99, true));
                    Vars.QMenu.Add(new MenuSliderButton("jungleclear", "JungleClear / if Mana >= x%", 50, 0, 99, true));
                }
                Vars.SpellsMenu.Add(Vars.QMenu);

                /// <summary>
                ///     Sets the menu for the W.
                /// </summary>
                Vars.WMenu = new Menu("w", "Use W to:");
                {
                    Vars.WMenu.Add(new MenuBool("combo", "Combo", true));
                    Vars.WMenu.Add(new MenuSliderButton("laneclear", "LaneClear / if Mana >= x%", 50, 0, 99, true));
                    Vars.WMenu.Add(new MenuSliderButton("jungleclear", "JungleClear / if Mana >= x%", 50, 0, 99, true));
                    Vars.WMenu.Add(new MenuSliderButton("buildings", "Buildings / if Mana >= x%", 50, 0, 99, true));
                }
                Vars.SpellsMenu.Add(Vars.WMenu);

                /// <summary>
                ///     Sets the menu for the E.
                /// </summary>
                Vars.EMenu = new Menu("e", "Use E to:");
                {
                    Vars.EMenu.Add(new MenuSeparator("separator", "Evade#/EzEvade: Can shield skillshots."));
                    Vars.EMenu.Add(new MenuSeparator("separator2", "ExorSivir: Can shield everything else."));
                    Vars.EMenu.Add(new MenuBool("logical", "Logical", true));
                    Vars.EMenu.Add(new MenuSlider("delay", "E Delay (ms)", 0, 0, 250));
                    {
                        /// <summary>
                        ///     Sets the menu for the E Whitelist.
                        /// </summary>
                        Vars.WhiteListMenu = new Menu("whitelist", "Shield: Whitelist Menu", true);
                        {
                            Vars.WhiteListMenu.Add(new MenuBool("minions", "Shield: Dragon's Attacks", true));
                            foreach (var enemy in GameObjects.EnemyHeroes)
                            {
                                if (enemy.ChampionName.Equals("Alistar"))
                                {
                                    Vars.WhiteListMenu.Add(
                                        new MenuBool(
                                            $"{enemy.ChampionName.ToLower()}.pulverize",
                                            $"Shield: {enemy.ChampionName}'s Pulverize (Q)",
                                            true));
                                }
                                if (enemy.ChampionName.Equals("Braum"))
                                {
                                    Vars.WhiteListMenu.Add(
                                        new MenuBool(
                                            $"{enemy.ChampionName.ToLower()}.passive",
                                            $"Shield: {enemy.ChampionName}'s Passive",
                                            true));
                                }
                                if (enemy.ChampionName.Equals("Jax"))
                                {
                                    Vars.WhiteListMenu.Add(
                                        new MenuBool(
                                            $"{enemy.ChampionName.ToLower()}.jaxcounterstrike",
                                            $"Shield: {enemy.ChampionName}'s JaxCounterStrike (E)",
                                            true));
                                }
                                if (enemy.ChampionName.Equals("KogMaw"))
                                {
                                    Vars.WhiteListMenu.Add(
                                        new MenuBool(
                                            $"{enemy.ChampionName.ToLower()}.kogmawicathiansurprise",
                                            $"Shield: {enemy.ChampionName}'s KogMawIcathianSurprise (Passive)",
                                            true));
                                }
                                if (enemy.ChampionName.Equals("Nautilus"))
                                {
                                    Vars.WhiteListMenu.Add(
                                        new MenuBool(
                                            $"{enemy.ChampionName.ToLower()}.nautilusravagestrikeattack",
                                            $"Shield: {enemy.ChampionName}'s NautilusRavageStrikeAttack (Passive)",
                                            true));
                                }
                                if (enemy.ChampionName.Equals("Udyr"))
                                {
                                    Vars.WhiteListMenu.Add(
                                        new MenuBool(
                                            $"{enemy.ChampionName.ToLower()}.udyrbearattack",
                                            $"Shield: {enemy.ChampionName}'s UdyrBearAttack (E)",
                                            true));
                                }

                                string[] excludedSpellsList = { "KatarinaE", "nautiluspiercinggaze" };
                                string[] assassinList = { "Akali", "Leblanc", "Talon" };

                                foreach (var spell in
                                    SpellDatabase.Get()
                                        .Where(
                                            s =>
                                            !excludedSpellsList.Contains(s.SpellName)
                                            && s.ChampionName.Equals(enemy.ChampionName)))
                                {
                                    if (spell.CastType != null)
                                    {
                                        if (enemy.IsMelee && spell.CastType.Contains(CastType.Activate)
                                            && spell.SpellType.HasFlag(SpellType.Activated)
                                            && AutoAttack.IsAutoAttackReset(spell.SpellName)
                                            || spell.CastType.Contains(CastType.EnemyChampions)
                                            && (spell.SpellType.HasFlag(SpellType.Targeted)
                                                || spell.SpellType.HasFlag(SpellType.TargetedMissile)))
                                        {
                                            Vars.WhiteListMenu.Add(
                                                new MenuBool(
                                                    $"{enemy.ChampionName.ToLower()}.{spell.SpellName.ToLower()}",
                                                    $"Shield: {enemy.ChampionName}'s {spell.SpellName} ({spell.Slot})"
                                                    + (assassinList.Contains(enemy.ChampionName) ? "[May not work]" : ""),
                                                    true));
                                        }
                                    }
                                }
                            }
                        }

                        Vars.EMenu.Add(Vars.WhiteListMenu);
                    }
                }

                Vars.SpellsMenu.Add(Vars.EMenu);
            }

            Vars.Menu.Add(Vars.SpellsMenu);

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = new Menu("drawings", "Drawings");
            {
                Vars.DrawingsMenu.Add(new MenuBool("q", "Q Range"));
            }
            Vars.Menu.Add(Vars.DrawingsMenu);
        }

        #endregion
    }
}