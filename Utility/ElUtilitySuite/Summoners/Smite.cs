using EloBuddy; namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = SharpDX.Color;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Smite : IPlugin
    {
        #region Constants

        /// <summary>
        ///     The smite range
        /// </summary>
        public const float SmiteRange = 570f;

        #endregion

        #region Static Fields

        public static Obj_AI_Minion Minion;

        private static readonly Dictionary<LeagueSharp.Common.Utility.Map.MapType, Dictionary<string, string>> SmiteableObjects =
            new Dictionary<LeagueSharp.Common.Utility.Map.MapType, Dictionary<string, string>>
                {
                    {
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                        new Dictionary<string, string>
                            {
                                { "Baron Nashor", "SRU_Baron" },
                                { "Blue Sentinel", "SRU_Blue" },
                                { "Water Dragon", "SRU_Dragon_Water" },
                                { "Fire Dragon", "SRU_Dragon_Fire" },
                                { "Earth Dragon", "SRU_Dragon_Earth" },
                                { "Air Dragon", "SRU_Dragon_Air" },
                                { "Elder Dragon", "SRU_Dragon_Elder" },
                                { "Red Brambleback", "SRU_Red" },
                                { "Rift Herald", "SRU_RiftHerald" },
                                // should be off by default
                                // pls mobo fix
                                { "Crimson Raptor", "SRU_Razorbeak" },
                                { "Greater Murk Wolf", "SRU_Murkwolf" },
                                { "Gromp", "SRU_Gromp" },
                                { "Rift Scuttler", "Sru_Crab" },
                                { "Ancient Krug", "SRU_Krug" }
                            }
                    },
                    {
                        LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline,
                        new Dictionary<string, string>
                            {
                                { "Big Golem", "TTNGolem" },
                                { "Giant Wolf", "TTNWolf" },
                                { "Vilemaw", "TT_Spiderboss" },
                                { "Wraith", "TTNWraith" }
                            }
                    }
                };

        #endregion

        #region Fields

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>
        ///     The spell type.
        /// </value>
        public SpellDataTargetType TargetType;

        #endregion

        #region Constructors and Destructors

        static Smite()
        {
            Spells = new List<Smite>
                         {
                             new Smite
                                 {
                                     ChampionName = "ChoGath", Range = 325f, Slot = SpellSlot.R, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Elise", Range = 475f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Fizz", Range = 550f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "LeeSin", Range = 1100f, Slot = SpellSlot.Q, Stage = 1,
                                     TargetType = SpellDataTargetType.Self
                                 },
                             new Smite
                                 {
                                     ChampionName = "MonkeyKing", Range = 375f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Nunu", Range = 300f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Olaf", Range = 325f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Pantheon", Range = 600f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Volibear", Range = 400f, Slot = SpellSlot.W, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "XinZhao", Range = 600f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Mundo", Range = 1050f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "KhaZix", Range = 325f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Evelynn", Range = 225f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Diana", Range = 895f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Alistar", Range = 365f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Nocturne", Range = 1200f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Maokai", Range = 600f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Twitch", Range = 950f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Self
                                 },
                             new Smite
                                 {
                                     ChampionName = "JarvanIV", Range = 770f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Rengar", Range = 325f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Aatrox", Range = 650f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Amumu", Range = 1100f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Gragas", Range = 600f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Trundle", Range = 325f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Fiddlesticks", Range = 750f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Jax", Range = 700f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Ekko", Range = 1075f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Vi", Range = 325f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Self
                                 },
                             new Smite
                                 {
                                     ChampionName = "Shaco", Range = 625f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Warwick", Range = 400f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Sejuani", Range = 625f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Tryndamere", Range = 660f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Zac", Range = 550f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "TahmKench", Range = 880f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Quinn", Range = 1025f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Poppy", Range = 525f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Kayle", Range = 650f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Hecarim", Range = 350f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Renekton", Range = 350f, Slot = SpellSlot.W, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Irelia", Range = 750f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 }
                         };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public static List<Smite> Spells { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string ChampionName { get; set; }

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

        /// <summary>
        ///     Gets or sets the range.
        /// </summary>
        /// <value>
        ///     The range.
        /// </value>
        public float Range { get; set; }

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The slot.
        /// </value>
        public SpellSlot Slot { get; set; }

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        public Spell SmiteSpell { get; set; }

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The stage.
        /// </value>
        public int Stage { get; set; }

        #endregion

        #region Properties

        private Menu Menu { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player => ObjectManager.Player;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var smiteSlot = this.Player.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));

            if (smiteSlot == null)
            {
                return;
            }

            var predicate = new Func<Menu, bool>(x => x.Name == "SummonersMenu");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Summoners", "SummonersMenu"))
                           : rootMenu.Children.First(predicate);

            var smiteMenu = menu.AddSubMenu(new Menu("Smite", "Smite"));
            {
                smiteMenu.AddItem(
                    new MenuItem("ElSmite.Activated", "Smite Activated").SetValue(
                        new KeyBind("M".ToCharArray()[0], KeyBindType.Toggle, true)));

                smiteMenu.AddItem(new MenuItem("Smite.Spell", "Use spell smite combo").SetValue(true));
                smiteMenu.AddItem(new MenuItem("Smite.Ammo", "Save 1 smite charge").SetValue(true))
                    .SetTooltip("Will not smite a champion when there is only 1 smite charge!")
                    .SetFontStyle(FontStyle.Regular, Color.Green);

                var mobMenu = smiteMenu.SubMenu("Jungle camps").SetFontStyle(FontStyle.Bold, Color.GreenYellow);
                foreach (var entry in SmiteableObjects[LeagueSharp.Common.Utility.Map.GetMap().Type])
                {
                    mobMenu.AddItem(new MenuItem($"{entry.Value}", $"{entry.Key}").SetValue(true));
                }

                //Champion Smite
                smiteMenu.SubMenu("Champion smite").AddItem(new MenuItem("ElSmite.Combo.Mode", "Smite:"))
                        .SetValue(new StringList(new[] { "Killsteal", "Combo", "Never" }, 1));

                //Drawings
                smiteMenu.SubMenu("Drawings")
                    .AddItem(new MenuItem("ElSmite.Draw.Range", "Draw smite Range").SetValue(new Circle()));
                smiteMenu.SubMenu("Drawings")
                    .AddItem(new MenuItem("ElSmite.Draw.Text", "Draw smite text").SetValue(true));
                smiteMenu.SubMenu("Drawings")
                    .AddItem(new MenuItem("ElSmite.Draw.Damage", "Draw smite Damage").SetValue(false));
            }

            this.Menu = smiteMenu;
        }

        public void Load()
        {
            try
            {
                var smiteSlot = this.Player.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));
                if (smiteSlot != null)
                {
                    this.SmiteSpell = new Spell(smiteSlot.Slot, SmiteRange, TargetSelector.DamageType.True);

                    Drawing.OnDraw += this.OnDraw;
                    Game.OnUpdate += this.OnUpdate;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }
        }

        #endregion

        #region Methods
        private void ChampionSpellSmite(float damage, Obj_AI_Base mob)
        {
            try
            {
                foreach (var spell in
                    Spells.Where(
                        x =>
                        x.ChampionName.Equals(this.Player.ChampionName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (this.Player.LSGetSpellDamage(mob, spell.Slot, spell.Stage) + damage >= mob.Health && this.Player.Spellbook.GetSpell(spell.Slot).State == SpellState.Ready)
                    {
                        if (mob.LSIsValidTarget(this.SmiteSpell.Range))
                        {
                            if (spell.TargetType == SpellDataTargetType.Unit)
                            {
                                this.Player.Spellbook.CastSpell(spell.Slot, mob);
                            }
                            else if (spell.TargetType == SpellDataTargetType.Self)
                            {
                                this.Player.Spellbook.CastSpell(spell.Slot);
                            }
                            else if (spell.TargetType == SpellDataTargetType.Location)
                            {
                                this.Player.Spellbook.CastSpell(spell.Slot, mob.ServerPosition);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }
        }

        private void OnDraw(EventArgs args)
        {
            try
            {
                if (this.Player.IsDead)
                {
                    return;
                }

                var smiteActive = this.Menu.Item("ElSmite.Activated").GetValue<KeyBind>().Active;
                var drawSmite = this.Menu.Item("ElSmite.Draw.Range").GetValue<Circle>();
                var drawText = this.Menu.Item("ElSmite.Draw.Text").IsActive();
                var playerPos = Drawing.WorldToScreen(this.Player.Position);
                var drawDamage = this.Menu.Item("ElSmite.Draw.Damage").IsActive();

                if (smiteActive && this.SmiteSpell != null)
                {
                    if (drawText && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) == SpellState.Ready)
                    {
                        Drawing.DrawText(
                            playerPos.X - 70,
                            playerPos.Y + 40,
                            System.Drawing.Color.GhostWhite,
                            "Smite active");
                    }

                    if (drawText && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) != SpellState.Ready)
                    {
                        Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, System.Drawing.Color.Red, "Smite cooldown");
                    }

                    if (drawDamage && this.SmiteDamage() != 0)
                    {
                        var minions =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(
                                    m =>
                                    m.Team == GameObjectTeam.Neutral && m.LSIsValidTarget()   );

                        foreach (var minion in minions.Where(m => m.IsHPBarRendered))
                        {
                            var hpBarPosition = minion.HPBarPosition;
                            var maxHealth = minion.MaxHealth;
                            var sDamage = this.SmiteDamage();
                            //SmiteDamage : MaxHealth = x : 100
                            //Ratio math for this ^
                            var x = this.SmiteDamage() / maxHealth;
                            var barWidth = 0;

                            /*
                        * DON'T STEAL THE OFFSETS FOUND BY ASUNA DON'T STEAL THEM JUST GET OUT WTF MAN.
                        * EL SMITE IS THE BEST SMITE ASSEMBLY ON LEAGUESHARP AND YOU WILL NOT FIND A BETTER ONE.
                        * THE DRAWINGS ACTUALLY MAKE FUCKING SENSE AND THEY ARE FUCKING GOOD
                        * GTFO HERE SERIOUSLY OR I CALL DETUKS FOR YOU GUYS
                        * NO STEAL OR DMC FUCKING A REPORT.
                        * HELLO COPYRIGHT BY ASUNA 2015 ALL AUSTRALIAN RIGHTS RESERVED BY UNIVERSAL GTFO SERIOUSLY THO
                        * NO ALSO NO CREDITS JUST GET OUT DUDE GET OUTTTTTTTTTTTTTTTTTTTTTTT
                        */

                            switch (minion.CharData.BaseSkinName)
                            {
                                case "SRU_RiftHerald":
                                    barWidth = 145;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 3 + barWidth * x, hpBarPosition.Y + 17),
                                        new Vector2(hpBarPosition.X + 3 + barWidth * x, hpBarPosition.Y + 30),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + barWidth * x,
                                        hpBarPosition.Y - 5,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Dragon_Air":
                                case "SRU_Dragon_Water":
                                case "SRU_Dragon_Fire":
                                case "SRU_Dragon_Elder":
                                case "SRU_Dragon_Earth":
                                    barWidth = 145;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 3 + barWidth * x, hpBarPosition.Y + 22),
                                        new Vector2(hpBarPosition.X + 3 + barWidth * x, hpBarPosition.Y + 30),
                                        2f,
                                        System.Drawing.Color.Orange);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + barWidth * x,
                                        hpBarPosition.Y - 5,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Red":
                                case "SRU_Blue":
                                    barWidth = 145;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 3 + barWidth * x, hpBarPosition.Y + 20),
                                        new Vector2(hpBarPosition.X + 3 + barWidth * x, hpBarPosition.Y + 30),
                                        2f,
                                        System.Drawing.Color.Orange);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + barWidth * x,
                                        hpBarPosition.Y - 5,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Baron":
                                    barWidth = 194;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 18 + barWidth * x, hpBarPosition.Y + 20),
                                        new Vector2(hpBarPosition.X + 18 + barWidth * x, hpBarPosition.Y + 35),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + barWidth * x,
                                        hpBarPosition.Y - 3,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Gromp":
                                    barWidth = 87;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + barWidth * x,
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Murkwolf":
                                    barWidth = 75;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + barWidth * x,
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "Sru_Crab":
                                    barWidth = 61;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 8),
                                        new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + barWidth * x,
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Razorbeak":
                                    barWidth = 75;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + barWidth * x,
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Krug":
                                    barWidth = 81;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + barWidth * x, hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + barWidth * x,
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (drawText && this.SmiteSpell != null)
                    {
                        Drawing.DrawText(
                            playerPos.X - 70,
                            playerPos.Y + 40,
                            System.Drawing.Color.Red,
                            "Smite not active!");
                    }
                }

                var smiteSpell = this.SmiteSpell;
                if (smiteSpell != null)
                {
                    if (smiteActive && drawSmite.Active
                        && this.Player.Spellbook.CanUseSpell(smiteSpell.Slot) == SpellState.Ready)
                    {
                        Render.Circle.DrawCircle(this.Player.Position, SmiteRange, System.Drawing.Color.Green);
                    }

                    if (drawSmite.Active && this.Player.Spellbook.CanUseSpell(smiteSpell.Slot) != SpellState.Ready)
                    {
                        Render.Circle.DrawCircle(this.Player.Position, SmiteRange, System.Drawing.Color.Red);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
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
                if (this.Player.IsDead || this.SmiteSpell == null || !this.Menu.Item("ElSmite.Activated").GetValue<KeyBind>().Active)
                {
                    return;
                }

                foreach (var mob in MinionManager.GetMinions(950f, MinionTypes.All, MinionTeam.Neutral)
                    .Where(m => !m.Name.Contains("Mini") && !m.Name.Contains("Respawn")))
                {
                    if (mob.LSDistance(this.Player, false) - (this.Player.BoundingRadius)
                        > 500)
                    {
                        continue;
                    }

                    if (this.Menu.Item(mob.CharData.BaseSkinName).IsActive())
                    {
                        if (this.Menu.Item("Smite.Spell").IsActive())
                        {
                            this.ChampionSpellSmite(
                                (float)this.Player.GetSummonerSpellDamage(mob, Damage.SummonerSpell.Smite),
                                mob);
                        }

                        if (mob.Health < this.Player.GetSummonerSpellDamage(mob, Damage.SummonerSpell.Smite))
                        {
                            this.Player.Spellbook.CastSpell(this.SmiteSpell.Slot, mob);
                            break;
                        }
                    }
                }

                if (!this.Menu.Item("Smite.Ammo").IsActive() || this.Menu.Item("Smite.Ammo").IsActive() && this.Player.GetSpell(this.SmiteSpell.Slot).Ammo > 1)
                {
                    if (this.Menu.Item("ElSmite.Combo.Mode").GetValue<StringList>().SelectedIndex == 0
                        && this.Player.GetSpell(this.SmiteSpell.Slot)
                               .Name.Equals("s5_summonersmiteduel", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var kSableEnemy = HeroManager.Enemies.FirstOrDefault(hero => hero.LSIsValidTarget(SmiteRange) && this.SmiteSpell.GetDamage(hero) >= hero.Health);

                        if (kSableEnemy != null)
                        {
                            this.Player.Spellbook.CastSpell(this.SmiteSpell.Slot, kSableEnemy);
                        }
                    }

                    if (this.Menu.Item("ElSmite.Combo.Mode").GetValue<StringList>().SelectedIndex == 1
                        && this.Player.GetSpell(this.SmiteSpell.Slot).Name.Equals("s5_summonersmiteplayerganker", StringComparison.InvariantCultureIgnoreCase) || this.Player.GetSpell(this.SmiteSpell.Slot).Name.Equals("s5_summonersmiteduel", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (this.ComboModeActive)
                        {
                            var smiteComboEnemy = HeroManager.Enemies.FirstOrDefault(hero => hero.LSIsValidTarget(SmiteRange));
                            if (smiteComboEnemy != null)
                            {
                                this.Player.Spellbook.CastSpell(this.SmiteSpell.Slot, smiteComboEnemy);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }
        }

        private float SmiteDamage()
        {
            try
            {
                return this.Player.Spellbook.GetSpell(this.SmiteSpell.Slot).State == SpellState.Ready
                           ? (float)this.Player.GetSummonerSpellDamage(Minion, Damage.SummonerSpell.Smite)
                           : 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }

            return 0;
        }

        #endregion
    }
}