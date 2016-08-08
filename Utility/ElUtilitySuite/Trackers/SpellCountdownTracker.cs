using EloBuddy; namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;

    internal class SpellCountdownTracker : IPlugin
    {
        #region Constants

        /// <summary>
        ///     The box height
        /// </summary>
        private const int BoxHeight = 105;

        /// <summary>
        ///     The box spacing
        /// </summary>
        private const int BoxSpacing = 25;

        /// <summary>
        ///     The box width
        /// </summary>
        private const int BoxWidth = 235;

        /// <summary>
        ///     The color indicator width
        /// </summary>
        private const int ColorIndicatorWidth = 10;

        /// <summary>
        ///     The countdown
        /// </summary>
        private const int Countdown = 10;

        /// <summary>
        ///     The move right speed
        /// </summary>
        private const int MoveRightSpeed = 1000;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the cards.
        /// </summary>
        /// <value>
        ///     The cards.
        /// </value>
        private List<Card> Cards { get; } = new List<Card>();

        /// <summary>
        ///     Gets the countdown font.
        /// </summary>
        /// <value>
        ///     The countdown font.
        /// </value>
        private Font CountdownFont { get; } = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Arial", 25));

        /// <summary>
        ///     Gets the icons.
        /// </summary>
        /// <value>
        ///     The icons.
        /// </value>
        private Dictionary<string, Texture> Icons { get; } = new Dictionary<string, Texture>();
        private Dictionary<string, List<SpellSlot>> ChampionSpells { get; } = new Dictionary<string, List<SpellSlot>>();

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private Menu Menu { get; set; }

        /// <summary>
        ///     Gets the padding.
        /// </summary>
        /// <value>
        ///     The padding.
        /// </value>
        private Vector2 Padding { get; } = new Vector2(10, 5);

        /// <summary>
        ///     Gets the spell name font.
        /// </summary>
        /// <value>
        ///     The spell name font.
        /// </value>
        private Font SpellNameFont { get; } = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Arial", 15));

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        private List<SpellDatabaseEntry> Spells { get; } = new List<SpellDatabaseEntry>();

        /// <summary>
        ///     Gets the sprite.
        /// </summary>
        /// <value>
        ///     The sprite.
        /// </value>
        private Sprite Sprite { get; } = new Sprite(Drawing.Direct3DDevice);

        /// <summary>
        ///     Gets or sets the start x.
        /// </summary>
        /// <value>
        ///     The start x.
        /// </value>
        private int StartX => this.Menu.Item("XPos").GetValue<Slider>().Value;

        /// <summary>
        ///     Gets or sets the start y.
        /// </summary>
        /// <value>
        ///     The start y.
        /// </value>
        private int StartY => this.Menu.Item("YPos").GetValue<Slider>().Value;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var predicate = new Func<Menu, bool>(x => x.Name == "Trackers");
            var menu =
                (rootMenu.Children.Any(predicate)
                     ? rootMenu.Children.First(predicate)
                     : rootMenu.AddSubMenu(new Menu("Trackers", "Trackers"))).AddSubMenu(
                         new Menu("Cooldown Notification", "CDNotif2"));

            menu.AddItem(new MenuItem("XPos", "X Position").SetValue(new Slider(Drawing.Width - BoxWidth, 0, Drawing.Width)));
            menu.AddItem(new MenuItem("YPos", "Y Position").SetValue(new Slider(Drawing.Height - BoxHeight * 4, 0, Drawing.Height)));
            menu.AddItem(new MenuItem("DrawCards", "Draw Cards").SetValue(true));
            menu.AddItem(new MenuItem("DrawTeleport", "Draw Teleports").SetValue(true));
            menu.AddItem(new MenuItem("AddTestCard", "Draw Test Card").SetValue(false).DontSave());
            menu.AddItem(new MenuItem("empty-line-3000", string.Empty));

            foreach (var enemy in HeroManager.Enemies)
            {
                menu.AddItem(new MenuItem($"Track.{enemy.CharData.BaseSkinName}", "Track " + enemy.ChampionName))
                    .SetValue(true);
            }

            menu.Item("AddTestCard").ValueChanged += (sender, args) =>
            {
                args.Process = false;

                if (this.Cards.Any(x => x.Name.Equals("Test")) || args.GetOldValue<bool>())
                {
                    return;
                }

                this.Cards.Add(
                    new Card
                    {
                        EndMessage = "Ready",
                        EndTime = Game.Time + 10,
                        FriendlyName = "Test",
                        Name = "Test",
                        StartTime = Game.Time
                    });
            };

            this.Menu = menu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        //[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public void Load()
        {
            if (Game.MapId == GameMapId.HowlingAbyss)
            {
                return;
            }

            Game.OnUpdate += this.GameOnUpdate;
            Drawing.OnDraw += this.Drawing_OnDraw;

            JungleTracker.CampDied += this.JungleTrackerCampDied;
            Obj_AI_Base.OnTeleport += this.OnTeleport;
            Obj_AI_Base.OnBuffLose += this.OnBuffLose;

            Drawing.OnPreReset += args =>
            {
                this.SpellNameFont.OnLostDevice();
                this.CountdownFont.OnLostDevice();
                this.Sprite.OnLostDevice();
            };

            Drawing.OnPostReset += args =>
            {
                this.SpellNameFont.OnResetDevice();
                this.CountdownFont.OnResetDevice();
                this.Sprite.OnResetDevice();
            };

            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames().Skip(1).ToList();
            var neededSpells =
                 Data.Get<SpellDatabase>().Spells.Where(
                        x =>
                        HeroManager.Enemies.Any(
                            y => x.ChampionName.Equals(y.ChampionName, StringComparison.InvariantCultureIgnoreCase))).Select(x => x.SpellName).ToList();

            foreach (var name in names)
            {
                try
                {
                    var spellName = name.Split('.')[3];
                    if (spellName != "Dragon" && spellName != "Baron" && spellName != "Teleport"  && spellName != "Rebirthready" && spellName != "Zacrebirthready")
                    {
                        this.Spells.Add(Data.Get<SpellDatabase>().Spells.First(x => x.SpellName.Equals(spellName)));

                        if (!neededSpells.Contains(spellName))
                        {
                            continue;
                        }
                    }

                    this.Icons.Add(
                        spellName,
                        Texture.FromStream(
                            Drawing.Direct3DDevice,
                            Assembly.GetExecutingAssembly().GetManifestResourceStream(name)));
                }
                catch (Exception)
                {
                    Console.WriteLine($"Failed to find load image for {name}. Please notify jQuery/ChewyMoon!");
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
        }

        #endregion

        #region Methods

        /// <summary>
        ///     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsEnemy)
            {
                if (args.Buff.Name.Equals("rebirthready", StringComparison.InvariantCultureIgnoreCase))
                {
                    var card = new Card
                    {
                        EndTime = Game.Time + 240,
                        EndMessage = "Ready",
                        FriendlyName = "Rebirth",
                        StartTime = Game.Time
                    };
                    card.Name = "Rebirthready";
                    this.Cards.Add(card);
                }

                if (args.Buff.Name.Equals("zacrebirthready", StringComparison.InvariantCultureIgnoreCase))
                {
                    var card = new Card
                    {
                        EndTime = Game.Time + 300,
                        EndMessage = "Ready",
                        FriendlyName = "Cell Division",
                        StartTime = Game.Time
                    };

                    card.Name = "Zacrebirthready";
                    this.Cards.Add(card);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTeleport(Obj_AI_Base sender, GameObjectTeleportEventArgs args)
        {
            try
            {
               if (sender.IsAlly || !this.Menu.Item("DrawTeleport").IsActive())
                {
                    return;
                }
                
                var hero =  sender as AIHeroClient;
                if (hero != null)
                {
                    var packet = Packet.S2C.Teleport.Decoded(sender, args);
                    if (packet.Type == Packet.S2C.Teleport.Type.Teleport && 
                        (packet.Status == Packet.S2C.Teleport.Status.Finish || packet.Status == Packet.S2C.Teleport.Status.Abort))
                    {
                        var time = Game.Time;
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            250,
                            delegate
                            {
                                var cd = packet.Status == Packet.S2C.Teleport.Status.Finish ? 300 : 200;
                                var card = new Card
                                {
                                    EndTime = time + cd,
                                    EndMessage = "Ready",
                                    FriendlyName = $"{hero.ChampionName} Teleport",
                                    StartTime = Game.Time
                                };

                                card.Name = "Teleport";
                                this.Cards.Add(card);
                            });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
        }

        /// <summary>
        ///     Draws a box.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="color">The color.</param>
        /// <param name="borderwidth">The borderwidth.</param>
        /// <param name="borderColor">Color of the border.</param>
        private static void DrawBox(
            Vector2 position,
            int width,
            int height,
            Color color,
            int borderwidth,
            Color borderColor)
        {
            Drawing.DrawLine(position.X, position.Y, position.X + width, position.Y, height, color);

            if (borderwidth <= 0)
            {
                return;
            }

            Drawing.DrawLine(position.X, position.Y, position.X + width, position.Y, borderwidth, borderColor);
            Drawing.DrawLine(
                position.X,
                position.Y + height,
                position.X + width,
                position.Y + height,
                borderwidth,
                borderColor);

            Drawing.DrawLine(position.X, position.Y + 1, position.X, position.Y + height + 1, borderwidth, borderColor);
            Drawing.DrawLine(
                position.X + width,
                position.Y + 1,
                position.X + width,
                position.Y + height + 1,
                borderwidth,
                borderColor);
        }

        /// <summary>
        ///     Drawing_s the on draw.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Drawing_OnDraw(EventArgs args)
        {
            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed
                || !this.Menu.Item("DrawCards").IsActive())
            {
                return;
            }

            var i = 0;

            // TODO clean this shit up LMAO
            foreach (var enemy in HeroManager.Enemies)
            {
                List<SpellSlot> slots;
                if (!this.ChampionSpells.TryGetValue(enemy.ChampionName, out slots))
                {
                    continue;
                }

                if (!this.Menu.Item($"Track.{enemy.ChampionName}").IsActive())
                {
                    continue;
                }

                foreach (var spell in slots.Select(x => enemy.GetSpell(x)).Where(x => x.Level > 0 && x.CooldownExpires > 0 
                && x.CooldownExpires - Game.Time <= Countdown))
                {
                    if (spell.CooldownExpires - Game.Time <= -5
                        && this.StartX + (int)((-(spell.CooldownExpires - Game.Time) - 5) * MoveRightSpeed)
                        >= Drawing.Width + i * MoveRightSpeed)
                    {
                        continue;
                    }

                    // draw spell
                    var remainingTime = spell.CooldownExpires - Game.Time;
                    var spellReady = remainingTime <= 0;

                    var remainingTimePretty = remainingTime > 0 ? remainingTime.ToString("N1") : "Ready";

                    var indicatorColor = spellReady ? Color.LawnGreen : Color.Yellow;

                    // We only need to calculate the y axis since the boxes stack vertically
                    var boxY = this.StartY - i * BoxSpacing - (i * BoxHeight);
                    var boxX = this.StartX;

                    if (remainingTime <= -5)
                    {
                        boxX += (int)((-remainingTime - 5) * MoveRightSpeed);
                    }

                    var lineStart = new Vector2(boxX, boxY);

                    DrawBox(lineStart, ColorIndicatorWidth, BoxHeight, indicatorColor, 0, new Color());

                    // Draw the black rectangle
                    var boxStart = new Vector2(boxX + ColorIndicatorWidth, boxY);
                    DrawBox(boxStart, BoxWidth - ColorIndicatorWidth, BoxHeight, Color.Black, 0, new Color());

                    // Draw spell name
                    var spellNameStart = boxStart + this.Padding;
                    this.SpellNameFont.DrawText(
                        null,
                        $"{enemy.ChampionName} {spell.Slot}",
                        (int)spellNameStart.X,
                        (int)spellNameStart.Y,
                        new ColorBGRA(255, 255, 255, 255));

                    // draw icon
                    var textSize = this.SpellNameFont.MeasureText(null, $"{enemy.ChampionName} {spell.Slot}");
                    var iconStart = spellNameStart + new Vector2(0, textSize.Height + 5);

                    Texture texture;
                    if (this.Icons.TryGetValue(spell.SData.Name, out texture))
                    {
                        this.Sprite.Begin();
                        this.Sprite.Draw(texture, new ColorBGRA(255, 255, 255, 255), null, new Vector3(-1 * iconStart, 0));
                        this.Sprite.End();
                    }
                    else
                    {
                        DrawBox(iconStart, 52, 52, Color.White, 0, new Color());
                    }

                    // draw countdown, add [icon size + padding]
                    var countdownStart = iconStart + new Vector2(51 + 22, -7);
                    this.CountdownFont.DrawText(
                        null,
                        remainingTimePretty,
                        (int)countdownStart.X,
                        (int)countdownStart.Y,
                        new ColorBGRA(255, 255, 255, 255));

                    // Draw progress bar :(
                    var countdownSize = this.CountdownFont.MeasureText(null, remainingTimePretty);
                    var progressBarStart = countdownStart + new Vector2(0, countdownSize.Height + 9);
                    var progressBarFullSize = 125;
                    var progressBarActualSize = (Countdown - remainingTime) / Countdown * progressBarFullSize;

                    if (progressBarActualSize > progressBarFullSize) // broken
                    {
                        progressBarActualSize = progressBarFullSize;
                    }

                    // MAGICERINO
                    DrawBox(progressBarStart, progressBarFullSize, 15, Color.Black, 1, Color.LawnGreen);
                    DrawBox(
                        progressBarStart + new Vector2(3, 3),
                        (int)(progressBarActualSize - 6),
                        15 - 5,
                        Color.LawnGreen,
                        0,
                        new Color());

                    i++;
                }
            }

            foreach (var card in this.Cards.Where(x => x.EndTime - Game.Time <= Countdown))
            {
                // draw spell
                var remainingTime = card.EndTime - Game.Time;
                var spellReady = remainingTime <= 0;

                var remainingTimePretty = remainingTime > 0 ? remainingTime.ToString("N1") : card.EndMessage;

                var indicatorColor = spellReady ? Color.LawnGreen : Color.Yellow;

                // We only need to calculate the y axis since the boxes stack vertically
                var boxY = this.StartY - i * BoxSpacing - (i * BoxHeight);
                var boxX = this.StartX;

                if (remainingTime <= -5)
                {
                    boxX += (int)((-remainingTime - 5) * MoveRightSpeed);
                }

                var lineStart = new Vector2(boxX, boxY);

                DrawBox(lineStart, ColorIndicatorWidth, BoxHeight, indicatorColor, 0, new Color());

                // Draw the black rectangle
                var boxStart = new Vector2(boxX + ColorIndicatorWidth, boxY);
                DrawBox(boxStart, BoxWidth - ColorIndicatorWidth, BoxHeight, Color.Black, 0, new Color());

                // Draw spell name
                var spellNameStart = boxStart + this.Padding;
                this.SpellNameFont.DrawText(
                    null,
                    card.FriendlyName,
                    (int)spellNameStart.X,
                    (int)spellNameStart.Y,
                    new ColorBGRA(255, 255, 255, 255));

                // draw icon
                var textSize = this.SpellNameFont.MeasureText(null, card.FriendlyName);
                var iconStart = spellNameStart + new Vector2(0, textSize.Height + 5);

                Texture texture;
                if (this.Icons.TryGetValue(card.Name, out texture))
                {
                    this.Sprite.Begin();
                    this.Sprite.Draw(texture, new ColorBGRA(255, 255, 255, 255), null, new Vector3(-1 * iconStart, 0));
                    this.Sprite.End();
                }
                else
                {
                    DrawBox(iconStart, 52, 52, Color.White, 0, new Color());
                }

                // draw countdown, add [icon size + padding]
                var countdownStart = iconStart + new Vector2(51 + 22, -7);
                this.CountdownFont.DrawText(
                    null,
                    remainingTimePretty,
                    (int)countdownStart.X,
                    (int)countdownStart.Y,
                    new ColorBGRA(255, 255, 255, 255));

                // Draw progress bar :(
                var countdownSize = this.CountdownFont.MeasureText(null, remainingTimePretty);
                var progressBarStart = countdownStart + new Vector2(0, countdownSize.Height + 9);
                var progressBarFullSize = 125;
                var cooldown = card.EndTime - card.StartTime;
                var progressBarActualSize = (cooldown - remainingTime) / cooldown * progressBarFullSize;

                if (progressBarActualSize > progressBarFullSize)
                {
                    progressBarActualSize = progressBarFullSize;
                }

                // MAGICERINO
                DrawBox(progressBarStart, progressBarFullSize, 15, Color.Black, 1, Color.LawnGreen);
                DrawBox(
                    progressBarStart + new Vector2(3, 3),
                    (int)(progressBarActualSize - 6),
                    15 - 5,
                    Color.LawnGreen,
                    0,
                    new Color());

                i++;
            }
        }

        /// <summary>
        ///     Fired when the game updates
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void GameOnUpdate(EventArgs args)
        {
            this.Cards.RemoveAll(
                x =>
                x.EndTime - Game.Time <= -5
                && this.StartX + (int)((-(x.EndTime - Game.Time) - 5) * MoveRightSpeed)
                >= Drawing.Width + this.Cards.Count * MoveRightSpeed);
        }

        /// <summary>
        ///     Fired when a jungle camp died.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void JungleTrackerCampDied(object sender, JungleTracker.JungleCamp e)
        {
            if (!e.MobNames.Any(x => x.ToLower().Contains("baron") || x.ToLower().Contains("dragon")))
            {
                return;
            }

            var card = new Card
            {
                EndTime = e.NextRespawnTime,
                EndMessage = "Respawn",
                FriendlyName = e.MobNames.Any(x => x.ToLower().Contains("dragon")) ? "Dragon" : "Baron",
                StartTime = Game.Time
            };

            card.Name = card.FriendlyName;
            this.Cards.Add(card);
        }

        #endregion

        private class Card
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the end message.
            /// </summary>
            /// <value>
            ///     The end message.
            /// </value>
            public string EndMessage { get; set; }

            /// <summary>
            ///     Gets or sets the end time.
            /// </summary>
            /// <value>
            ///     The end time.
            /// </value>
            public float EndTime { get; set; }

            /// <summary>
            ///     Gets or sets the name of the friendly.
            /// </summary>
            /// <value>
            ///     The name of the friendly.
            /// </value>
            public string FriendlyName { get; set; }

            /// <summary>
            ///     Gets or sets the name.
            /// </summary>
            /// <value>
            ///     The name.
            /// </value>
            public string Name { get; set; }

            /// <summary>
            ///     Gets or sets the start time.
            /// </summary>
            /// <value>
            ///     The start time.
            /// </value>
            public float StartTime { get; set; }

            #endregion
        }
    }
}