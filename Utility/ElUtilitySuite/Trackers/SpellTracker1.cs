using EloBuddy; namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Security.Permissions;

    using PortAIO.Properties;
    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Font = SharpDX.Direct3D9.Font;
    using Rectangle = SharpDX.Rectangle;

    internal class SpellTracker : IPlugin
    {
        #region Constants
        // force commit kappa 123
        private const float TeleportCd = 300f;

        #endregion

        #region Fields

        private readonly Dictionary<int, List<SpellDataInst>> _spellDatas = new Dictionary<int, List<SpellDataInst>>();

        private readonly SpellSlot[] _spellSlots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };

        private readonly Dictionary<int, List<SpellDataInst>> _summonerDatas =
            new Dictionary<int, List<SpellDataInst>>();

        private readonly SpellSlot[] _summonerSlots = { SpellSlot.Summoner1, SpellSlot.Summoner2 };

        private readonly Dictionary<string, Texture> _summonerTextures = new Dictionary<string, Texture>();

        private readonly Dictionary<int, float> _teleports = new Dictionary<int, float>();

        private List<AIHeroClient> _heroes = new List<AIHeroClient>();

        private Texture _hudSelfTexture;

        private Texture _hudTexture;

        private Line _line;

        private Sprite _sprite;

        private Font _text;

        #endregion

        internal class ManualSpell
        {
            #region Constructors and Destructors

            public ManualSpell(string champ, string spell, SpellSlot slot, float[] cooldowns, float additional = 0)
            {
                this.Champ = champ;
                this.Spell = spell;
                this.Slot = slot;
                this.Cooldowns = cooldowns;
                this.Additional = additional;
            }

            #endregion

            #region Public Properties

            public float Additional { get; set; }

            public string Champ { get; private set; }

            public float Cooldown { get; set; }

            public float CooldownExpires { get; set; }

            public float[] Cooldowns { get; set; }

            public SpellSlot Slot { get; private set; }

            public string Spell { get; private set; }

            #endregion
        } // ReSharper disable StringLiteralTypo

        private readonly List<ManualSpell> _manualAllySpells = new List<ManualSpell>
                                                                   {
                                                                       new ManualSpell(
                                                                           "Lux",
                                                                           "LuxLightStrikeKugel",
                                                                           SpellSlot.E,
                                                                           new[] { 10f, 10f, 10f, 10f, 10f }),
                                                                       new ManualSpell(
                                                                           "Gragas",
                                                                           "GragasQ",
                                                                           SpellSlot.Q,
                                                                           new[] { 11f, 10f, 9f, 8f, 7f }),
                                                                       new ManualSpell(
                                                                           "Riven",
                                                                           "RivenFengShuiEngine",
                                                                           SpellSlot.R,
                                                                           new[] { 110f, 80f, 50f },
                                                                           15),
                                                                       new ManualSpell(
                                                                           "TwistedFate",
                                                                           "PickACard",
                                                                           SpellSlot.W,
                                                                           new[] { 6f, 6f, 6f, 6f, 6f }),
                                                                       new ManualSpell(
                                                                           "Velkoz",
                                                                           "VelkozQ",
                                                                           SpellSlot.Q,
                                                                           new[] { 7f, 7f, 7f, 7f, 7f },
                                                                           0.75f),
                                                                       new ManualSpell(
                                                                           "Xerath",
                                                                           "xeratharcanopulse2",
                                                                           SpellSlot.Q,
                                                                           new[] { 9f, 8f, 7f, 6f, 5f }),
                                                                       new ManualSpell(
                                                                           "Ziggs",
                                                                           "ZiggsW",
                                                                           SpellSlot.W,
                                                                           new[] { 26f, 24f, 22f, 20f, 18f }),
                                                                       new ManualSpell(
                                                                           "Rumble",
                                                                           "RumbleGrenade",
                                                                           SpellSlot.E,
                                                                           new[] { 10f, 10f, 10f, 10f, 10f }),
                                                                       new ManualSpell(
                                                                           "Riven",
                                                                           "RivenTriCleave",
                                                                           SpellSlot.Q,
                                                                           new[] { 13f, 13f, 13f, 13f, 13f }),
                                                                       new ManualSpell(
                                                                           "Fizz",
                                                                           "FizzJump",
                                                                           SpellSlot.E,
                                                                           new[] { 16f, 14f, 12f, 10f, 8f },
                                                                           0.75f)
                                                                   };

        private readonly List<ManualSpell> _manualEnemySpells = new List<ManualSpell>
                                                                    {
                                                                        new ManualSpell(
                                                                            "Lux",
                                                                            "LuxLightStrikeKugel",
                                                                            SpellSlot.E,
                                                                            new[] { 10f, 10f, 10f, 10f, 10f }),
                                                                        new ManualSpell(
                                                                            "Gragas",
                                                                            "GragasQ",
                                                                            SpellSlot.Q,
                                                                            new[] { 11f, 10f, 9f, 8f, 7f }),
                                                                        new ManualSpell(
                                                                            "Riven",
                                                                            "RivenFengShuiEngine",
                                                                            SpellSlot.R,
                                                                            new[] { 110f, 80f, 50f },
                                                                            15),
                                                                        new ManualSpell(
                                                                            "TwistedFate",
                                                                            "PickACard",
                                                                            SpellSlot.W,
                                                                            new[] { 6f, 6f, 6f, 6f, 6f }),
                                                                        new ManualSpell(
                                                                            "Velkoz",
                                                                            "VelkozQ",
                                                                            SpellSlot.Q,
                                                                            new[] { 7f, 7f, 7f, 7f, 7f },
                                                                            0.75f),
                                                                        new ManualSpell(
                                                                            "Xerath",
                                                                            "xeratharcanopulse2",
                                                                            SpellSlot.Q,
                                                                            new[] { 9f, 8f, 7f, 6f, 5f }),
                                                                        new ManualSpell(
                                                                            "Ziggs",
                                                                            "ZiggsW",
                                                                            SpellSlot.W,
                                                                            new[] { 26f, 24f, 22f, 20f, 18f }),
                                                                        new ManualSpell(
                                                                            "Rumble",
                                                                            "RumbleGrenade",
                                                                            SpellSlot.E,
                                                                            new[] { 10f, 10f, 10f, 10f, 10f }),
                                                                        new ManualSpell(
                                                                            "Riven",
                                                                            "RivenTriCleave",
                                                                            SpellSlot.Q,
                                                                            new[] { 13f, 13f, 13f, 13f, 13f }),
                                                                        new ManualSpell(
                                                                            "Fizz",
                                                                            "FizzJump",
                                                                            SpellSlot.E,
                                                                            new[] { 16f, 14f, 12f, 10f, 8f },
                                                                            0.75f)
                                                                    };

        public Menu Menu { get; set; }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var predicate = new Func<Menu, bool>(x => x.Name == "Trackers");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Trackers", "Trackers"))
                           : rootMenu.Children.First(predicate);

            var cooldownMenu = menu.AddSubMenu(new Menu("Cooldown tracker", "cdddddtracker"));
            {
                cooldownMenu.AddItem(
                    new MenuItem("cooldown-tracker-TimeFormat", "Time Format").SetValue(
                        new StringList(new[] { "mm:ss", "ss" })));
                cooldownMenu.AddItem(
                    new MenuItem("cooldown-tracker-FontSize", "Font Size").SetValue(new Slider(13, 3, 30)));
                cooldownMenu.AddItem(new MenuItem("cooldown-tracker-Enemy", "Enemy").SetValue(true));
                cooldownMenu.AddItem(new MenuItem("cooldown-tracker-Ally", "Ally").SetValue(true));
                cooldownMenu.AddItem(new MenuItem("cooldown-tracker-Self", "Self").SetValue(true));

                cooldownMenu.AddItem(new MenuItem("cooldown-tracker-Enabled", "Enabled").SetValue(true));
            }

            this.Menu = cooldownMenu;

            this.Menu.Item("cooldown-tracker-Enemy").ValueChanged += delegate(object o, OnValueChangeEventArgs args)
                {
                    if (_heroes == null)
                    {
                        return;
                    }
                    var ally = Menu.Item("cooldown-tracker-Ally").GetValue<bool>();
                    var enemy = args.GetNewValue<bool>();
                    _heroes = ally && enemy
                                  ? HeroManager.AllHeroes.ToList()
                                  : (ally ? HeroManager.Allies : (enemy ? HeroManager.Enemies : new List<AIHeroClient>()))
                                        .ToList();
                    if (Menu.Item("cooldown-tracker-Self").GetValue<bool>())
                    {
                        if (_heroes.All(h => h.NetworkId != ObjectManager.Player.NetworkId))
                        {
                            _heroes.Add(ObjectManager.Player);
                        }
                    }
                    else
                    {
                        _heroes.RemoveAll(h => h.NetworkId == ObjectManager.Player.NetworkId);
                    }
                };

            this.Menu.Item("cooldown-tracker-Ally").ValueChanged += delegate(object o, OnValueChangeEventArgs args)
                {
                    if (_heroes == null)
                    {
                        return;
                    }
                    var ally = args.GetNewValue<bool>();
                    var enemy = Menu.Item("cooldown-tracker-Enemy").GetValue<bool>();
                    _heroes = ally && enemy
                                  ? HeroManager.AllHeroes.ToList()
                                  : (ally
                                         ? HeroManager.Allies
                                         : (enemy ? HeroManager.Enemies : new List<AIHeroClient>())).ToList();
                    if (Menu.Item("cooldown-tracker-Self").GetValue<bool>()
                        && _heroes.All(h => h.NetworkId != ObjectManager.Player.NetworkId))
                    {
                        _heroes.Add(ObjectManager.Player);
                    }
                    if (Menu.Item("cooldown-tracker-Self").GetValue<bool>())
                    {
                        if (_heroes.All(h => h.NetworkId != ObjectManager.Player.NetworkId))
                        {
                            _heroes.Add(ObjectManager.Player);
                        }
                    }
                    else
                    {
                        _heroes.RemoveAll(h => h.NetworkId == ObjectManager.Player.NetworkId);
                    }
                };

            this.Menu.Item("cooldown-tracker-Self").ValueChanged += delegate(object o, OnValueChangeEventArgs args)
                {
                    if (_heroes == null)
                    {
                        return;
                    }
                    var ally = Menu.Item("cooldown-tracker-Ally").GetValue<bool>();
                    var enemy = Menu.Item("cooldown-tracker-Enemy").GetValue<bool>();
                    _heroes = ally && enemy
                                  ? HeroManager.AllHeroes.ToList()
                                  : (ally ? HeroManager.Allies : (enemy ? HeroManager.Enemies : new List<AIHeroClient>()))
                                        .ToList();
                    if (args.GetNewValue<bool>())
                    {
                        if (_heroes.All(h => h.NetworkId != ObjectManager.Player.NetworkId))
                        {
                            _heroes.Add(ObjectManager.Player);
                        }
                    }
                    else
                    {
                        _heroes.RemoveAll(h => h.NetworkId == ObjectManager.Player.NetworkId);
                    }
                };
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            try
            {
                this._hudTexture = Resources.CD_Hud.ToTexture();
                this._hudSelfTexture = Resources.CD_HudSelf.ToTexture();

                foreach (var enemy in HeroManager.AllHeroes)
                {
                    var lEnemy = enemy;
                    this._spellDatas.Add(
                        enemy.NetworkId,
                        this._spellSlots.Select(slot => lEnemy.GetSpell(slot)).ToList());
                    this._summonerDatas.Add(
                        enemy.NetworkId,
                        this._summonerSlots.Select(slot => lEnemy.GetSpell(slot)).ToList());
                }

                foreach (var sName in
                    HeroManager.AllHeroes.SelectMany(
                        h =>
                        this._summonerSlots.Select(summoner => h.Spellbook.GetSpell(summoner).Name.ToLower())
                            .Where(sName => !this._summonerTextures.ContainsKey(FixName(sName)))))
                {
                    this._summonerTextures[FixName(sName)] =
                        ((Bitmap)Resources.ResourceManager.GetObject(string.Format("CD_{0}", FixName(sName)))
                         ?? Resources.CD_SummonerBarrier).ToTexture();
                }

                this._heroes = this.Menu.Item("cooldown-tracker-Ally").GetValue<bool>()
                               && this.Menu.Item("cooldown-tracker-Enemy").GetValue<bool>()
                                   ? HeroManager.AllHeroes.ToList()
                                   : (this.Menu.Item("cooldown-tracker-Ally").GetValue<bool>()
                                          ? HeroManager.Allies
                                          : (this.Menu.Item("cooldown-tracker-Enemy").GetValue<bool>()
                                                 ? HeroManager.Enemies
                                                 : new List<AIHeroClient>())).ToList();

                if (!this.Menu.Item("cooldown-tracker-Self").GetValue<bool>())
                {
                    this._heroes.RemoveAll(h => h.NetworkId == ObjectManager.Player.NetworkId);
                }

                this._sprite = MDrawing.GetSprite();
                this._line = MDrawing.GetLine(4);
                this._text = MDrawing.GetFont(this.Menu.Item("cooldown-tracker-FontSize").GetValue<Slider>().Value);

                Drawing.OnEndScene += this.OnDrawingEndScene;

                Drawing.OnPreReset += args =>
                    {
                        this._line.OnLostDevice();
                        this._sprite.OnLostDevice();
                        this._text.OnLostDevice();
                    };

                Drawing.OnPostReset += args =>
                    {
                        this._line.OnResetDevice();
                        this._sprite.OnResetDevice();
                        this._text.OnResetDevice();
                    };

                Obj_AI_Base.OnProcessSpellCast += this.OnObjAiBaseProcessSpellCast;
                Obj_AI_Base.OnTeleport += this.OnObjAiBaseTeleport;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnObjAiBaseTeleport(Obj_AI_Base sender, GameObjectTeleportEventArgs args)
        {
            try
            {
                if (!this.Menu.Item("cooldown-tracker-Enabled").IsActive())
                {
                    return;
                }

                var packet = Packet.S2C.Teleport.Decoded(sender, args);
                if (packet.Type == Packet.S2C.Teleport.Type.Teleport
                    && (packet.Status == Packet.S2C.Teleport.Status.Finish
                        || packet.Status == Packet.S2C.Teleport.Status.Abort))
                {
                    var time = Game.Time;
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        250,
                        delegate
                            {
                                var cd = packet.Status == Packet.S2C.Teleport.Status.Finish ? 300 : 200;
                                _teleports[packet.UnitNetworkId] = time + cd;
                            });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static string FixName(string name)
        {
            try
            {
                return name.ToLower().Contains("smite")
                           ? "summonersmite"
                           : (name.ToLower().Contains("teleport") ? "summonerteleport" : name.ToLower());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return name;
        }

        private void OnDrawingEndScene(EventArgs args)
        {
            try
            {

                if (!this.Menu.Item("cooldown-tracker-Enabled").IsActive())
                {
                    return;
                }

                if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                {
                    return;
                }
                var totalSeconds = this.Menu.Item("cooldown-tracker-TimeFormat").GetValue<StringList>().SelectedIndex
                                   == 1;
                foreach (var hero in
                    this._heroes.Where(
                        hero => hero != null && hero.IsValid && hero.IsHPBarRendered && hero.Position.IsOnScreen()))
                {
                    try
                    {
                        var lHero = hero;
                        if (!hero.Position.IsValid() || !hero.HPBarPosition.IsValid())
                        {
                            return;
                        }

                        var x = (int)hero.HPBarPosition.X - (hero.IsMe ? -10 : 8);
                        var y = (int)hero.HPBarPosition.Y + (hero.IsEnemy ? 17 : (hero.IsMe ? 6 : 14));

                        this._sprite.Begin(SpriteFlags.AlphaBlend);
                        var summonerData = this._summonerDatas[hero.NetworkId];
                        for (var i = 0; i < summonerData.Count; i++)
                        {
                            var spell = summonerData[i];
                            if (spell != null)
                            {
                                var teleportCd = 0f;
                                if (spell.Name.Contains("Teleport") && this._teleports.ContainsKey(hero.NetworkId))
                                {
                                    this._teleports.TryGetValue(hero.NetworkId, out teleportCd);
                                }
                                var t = teleportCd > 0.1f
                                            ? teleportCd - Game.Time
                                            : (spell.IsReady() ? 0 : spell.CooldownExpires - Game.Time);
                                var sCd = teleportCd > 0.1f ? TeleportCd : spell.Cooldown;
                                var percent = Math.Abs(sCd) > float.Epsilon ? t / sCd : 1f;
                                var n = t > 0 ? (int)(19 * (1f - percent)) : 19;
                                if (t > 0)
                                {
                                    this._text.DrawTextCentered(
                                        t.FormatTime(totalSeconds),
                                        x - (hero.IsMe ? -160 : 13),
                                        y + 7 + 13 * i,
                                        new ColorBGRA(255, 255, 255, 255));
                                }
                                if (this._summonerTextures.ContainsKey(FixName(spell.Name)))
                                {
                                    this._sprite.Draw(
                                        this._summonerTextures[FixName(spell.Name)],
                                        new ColorBGRA(255, 255, 255, 255),
                                        new Rectangle(0, 12 * n, 12, 12),
                                        new Vector3(-x - (hero.IsMe ? 132 : 3), -y - 1 - 13 * i, 0));
                                }
                            }
                        }

                        this._sprite.Draw(
                            hero.IsMe ? this._hudSelfTexture : this._hudTexture,
                            new ColorBGRA(255, 255, 255, 255),
                            null,
                            new Vector3(-x, -y, 0));

                        this._sprite.End();

                        var x2 = x + (hero.IsMe ? 24 : 19);
                        var y2 = y + 21;

                        this._line.Begin();
                        var spellData = this._spellDatas[hero.NetworkId];
                        foreach (var spell in spellData)
                        {
                            var lSpell = spell;
                            if (spell != null)
                            {
                                var spell1 = spell;
                                var manual = hero.IsAlly
                                                 ? this._manualAllySpells.FirstOrDefault(
                                                     m =>
                                                     m.Slot.Equals(lSpell.Slot)
                                                     && m.Champ.Equals(
                                                         lHero.ChampionName,
                                                         StringComparison.OrdinalIgnoreCase))
                                                 : this._manualEnemySpells.FirstOrDefault(
                                                     m =>
                                                     m.Slot.Equals(spell1.Slot)
                                                     && m.Champ.Equals(
                                                         lHero.ChampionName,
                                                         StringComparison.OrdinalIgnoreCase));

                                var t = (manual != null ? manual.CooldownExpires : spell.CooldownExpires) - Game.Time;
                                var spellCooldown = manual != null ? manual.Cooldown : spell.Cooldown;
                                var percent = t > 0 && Math.Abs(spellCooldown) > float.Epsilon
                                                  ? 1f - t / spellCooldown
                                                  : 1f;


                                if (t > 0 && t < 100)
                                {
                                    this._text.DrawTextCentered(
                                        t.FormatTime(totalSeconds),
                                        x2 + 27 / 2,
                                        y2 + 13,
                                        new ColorBGRA(255, 255, 255, 255));
                                }

                                if (spell.Level > 0)
                                {
                                    this._line.Draw(
                                        new[] { new Vector2(x2, y2), new Vector2(x2 + percent * 23, y2) },
                                        t > 0 ? new ColorBGRA(235, 137, 0, 255) : new ColorBGRA(0, 168, 25, 255));
                                }
                                x2 = x2 + 27;
                            }
                        }
                        this._line.End();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: '{0}'", e);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private void OnObjAiBaseProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (!this.Menu.Item("cooldown-tracker-Enabled").IsActive())
                {
                    return;
                }

                var hero = sender as AIHeroClient;
                if (hero != null)
                {
                    var data = hero.IsAlly
                                   ? this._manualAllySpells.FirstOrDefault(
                                       m => m.Spell.Equals(args.SData.Name, StringComparison.OrdinalIgnoreCase))
                                   : this._manualEnemySpells.FirstOrDefault(
                                       m => m.Spell.Equals(args.SData.Name, StringComparison.OrdinalIgnoreCase));

                    if (data != null && data.CooldownExpires - Game.Time < 0.5)
                    {
                        var spell = hero.GetSpell(data.Slot);
                        if (spell != null)
                        {
                            var cooldown = data.Cooldowns[spell.Level - 1];
                            var cdr = hero.PercentCooldownMod * -1 * 100;
                            data.Cooldown = cooldown - cooldown / 100 * (cdr > 40 ? 40 : cdr) + data.Additional;
                            data.CooldownExpires = Game.Time + data.Cooldown;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}