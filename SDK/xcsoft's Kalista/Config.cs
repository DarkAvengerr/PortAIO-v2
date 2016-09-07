using System.Linq;

using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.UI;

using SharpDX;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcKalista
{
    internal static class Config
    {
        private const string MenuName = "Kalista";

        internal static readonly Menu Menu;

        static Config()
        {
            Menu = new Menu(MenuName, MenuName, true).Attach();

            Keys.Initialize();
            Modes.Initialize();
            Auto.Initialize();
            Hitchance.Initialize();
            Misc.Initialize();
            SkinManager.Initialize(Menu);
            Drawings.Initialize();
        }

        internal static void Initialize() { }

        internal static class Keys
        {
            internal static readonly Menu Menu;

            private static readonly MenuKeyBind _comboKey;
            private static readonly MenuKeyBind _harassKey;
            private static readonly MenuKeyBind _laneClearKey;
            private static readonly MenuKeyBind _jugnleClearKey;
            private static readonly MenuKeyBind _fleeKey;

            static Keys()
            {
                Menu = Config.Menu.Add(new Menu("Keys", "Keys"));

                _comboKey = Menu.Add(new MenuKeyBind("ComboKey", "Combo", System.Windows.Forms.Keys.Space, KeyBindType.Press));
                _harassKey = Menu.Add(new MenuKeyBind("HarassKey", "Harass", System.Windows.Forms.Keys.C, KeyBindType.Press));
                _laneClearKey = Menu.Add(new MenuKeyBind("LaneClearKey", "LaneClear", System.Windows.Forms.Keys.V, KeyBindType.Press));
                _jugnleClearKey = Menu.Add(new MenuKeyBind("JungleClearKey", "JungleClear", System.Windows.Forms.Keys.V, KeyBindType.Press));
                _fleeKey = Menu.Add(new MenuKeyBind("FleeKey", "Flee", System.Windows.Forms.Keys.T, KeyBindType.Press));
            }

            internal static bool ComboActive => _comboKey.Active;

            internal static bool HarassActive => _harassKey.Active;

            internal static bool LaneClearActive => _laneClearKey.Active;

            internal static bool JungleClearActive => _jugnleClearKey.Active;

            internal static bool FleeActive => _fleeKey.Active;

            internal static void Initialize() { }
        }

        internal static class Modes
        {
            internal static readonly Menu Menu;

            static Modes()
            {
                Menu = Config.Menu.Add(new Menu("Modes", "Modes"));

                Combo.Initialize();
                Harass.Initialize();
                LaneClear.Initialize();
                JungleClear.Initialize();
                Flee.Initialize();
            }

            internal static void Initialize() { }

            internal static class Combo
            {
                internal static readonly Menu Menu;

                private static readonly MenuBool _useQ;
                private static readonly MenuBool _useQPierce;
                private static readonly MenuBool _useE;
                private static readonly MenuBool _keepManaForE;
                private static readonly MenuBool _attackminion;

                static Combo()
                {
                    Menu = Modes.Menu.Add(new Menu("Combo", "Combo"));

                    _useQ = Menu.Add(new MenuBool("UseQ", "Use Q", true));
                    _useQPierce = Menu.Add(new MenuBool("UseQPierce", "Use Q pierce", true));
                    _useE = Menu.Add(new MenuBool("UseE", "Use E", true));
                    _keepManaForE = Menu.Add(new MenuBool("keepManaForE", "Keep Mana For E", true));
                    _attackminion = Menu.Add(new MenuBool("attackMinion", "Attack Minion for chasing", true));
                }

                internal static bool UseQ => _useQ.Value;

                internal static bool UseQPierce => _useQPierce.Value;

                internal static bool UseE => _useE.Value;

                internal static bool KeepManaForE => _keepManaForE.Value;

                internal static bool AttackMinion => _attackminion.Value;

                internal static void Initialize() { }
            }

            internal static class Harass
            {
                internal static readonly Menu Menu;

                private static readonly MenuBool _useQ;
                private static readonly MenuBool _useQPierce;
                private static readonly MenuSliderButton _minMana;

                static Harass()
                {
                    Menu = Modes.Menu.Add(new Menu("Harass", "Harass"));

                    _useQ = Menu.Add(new MenuBool("UseQ", "Use Q", true));
                    _useQPierce = Menu.Add(new MenuBool("UseQPierce", "Use Q pierce", true));
                    _minMana = Menu.Add(new MenuSliderButton("Mana", "Min Mana %", 70, 0, 100)
                    {
                        BValue = true
                    });
                }

                internal static bool UseQ => _useQ.Value;

                internal static bool UseQPierce => _useQPierce.Value;

                internal static int Mana => _minMana.Value;

                internal static void Initialize() { }
            }

            internal static class LaneClear
            {
                internal static readonly Menu Menu;

                private static readonly MenuSliderButton _minMana;

                static LaneClear()
                {
                    Menu = Modes.Menu.Add(new Menu("LaneClear", "LaneClear"));

                    UseQ = Menu.Add(new MenuSliderButton("UseQ", "Use Q to kill minions", 3, 1, 10)
                    {
                        BValue = false
                    });
                    UseE = Menu.Add(new MenuSliderButton("UseE", "Use E to kill minions", 3, 1, 10)
                    {
                        BValue = true
                    });
                    _minMana = Menu.Add(new MenuSliderButton("MinMana", "Min Mana %", 70, 0, 100)
                    {
                        BValue = true
                    });
                }

                internal static MenuSliderButton UseQ { get; }

                internal static MenuSliderButton UseE { get; }

                internal static int MinMana => _minMana.Value;

                internal static void Initialize() { }
            }

            internal static class JungleClear
            {
                internal static readonly Menu Menu;

                private static readonly MenuBool _useQ;
                private static readonly MenuBool _useESmall;
                private static readonly MenuBool _useEBig;
                private static readonly MenuBool _useELegendary;
                private static readonly MenuSliderButton _minMana;

                static JungleClear()
                {
                    Menu = Modes.Menu.Add(new Menu("JungleClear", "JungleClear"));

                    _useQ = Menu.Add(new MenuBool("UseQ", "Use Q", true));
                    _useESmall = Menu.Add(new MenuBool("UseESmall", "Use E Small", false));
                    _useEBig = Menu.Add(new MenuBool("UseEBig", "Use E Big", true));
                    _useELegendary = Menu.Add(new MenuBool("UseELegendary", "Use E Legendary", true));
                    _minMana = Menu.Add(new MenuSliderButton("MinMana", "Min Mana %", 0, 0, 100)
                    {
                        BValue = true
                    });
                }

                internal static bool UseQ => _useQ.Value;

                internal static bool UseESmall => _useESmall.Value;

                internal static bool UseEBig => _useEBig.Value;

                internal static bool UseELegendary => _useELegendary.Value;

                internal static int MinMana => _minMana.Value;

                internal static void Initialize() { }
            }

            internal static class Flee
            {
                internal static readonly Menu Menu;

                private static readonly MenuBool _useQ;
                private static readonly MenuBool _attackminion;

                static Flee()
                {
                    Menu = Modes.Menu.Add(new Menu("Flee", "Flee"));

                    _useQ = Menu.Add(new MenuBool("UseQ", "Use Q", true));
                    _attackminion = Menu.Add(new MenuBool("attackMinion", "Attack Minion to escape", true));
                }

                internal static bool UseQ => _useQ.Value;

                internal static bool AttackMinion => _attackminion.Value;

                internal static void Initialize() { }
            }
        }

        internal static class Auto
        {
            internal static readonly Menu Menu;

            static Auto()
            {
                Menu = Config.Menu.Add(new Menu("Auto", "Auto"));

                AutoW.Initialize();
                AutoE.Initialize();
                AutoR.Initialize();
            }

            internal static void Initialize() { }

            internal static class AutoW
            {
                internal static readonly Menu Menu;

                private static readonly MenuBool _enabled;
                private static readonly MenuSliderButton _minMana;
                private static readonly MenuBool _keepWCharge;

                static AutoW()
                {
                    Menu = Auto.Menu.Add(new Menu("AutoW", "Auto W"));

                    _enabled = Menu.Add(new MenuBool("enabled", "Enabled", true));
                    _minMana = Menu.Add(new MenuSliderButton("minMana", "Min Mana %", 50, 0, 100)
                    {
                        BValue = true
                    });
                    _keepWCharge = Menu.Add(new MenuBool("keepWcharge", "Keep W Charge (1)", true));
                    Menu.Add(new MenuSeparator("xxx1", " "));

                    if (SoulHandler.WCastPositionList.All(x => x.MapID != Game.MapId))
                    {
                        Menu.Add(new MenuSeparator("xxx2", "This map is not supported."));
                    }
                    else
                    {
                        SoulHandler.WCastPositionList.Where(x => x.MapID == Game.MapId).ForEach(x =>
                        {
                            Menu.Add(new MenuBool(x.PositionName, x.PositionName, !x.PositionName.Contains("Ally")));
                        });
                    }
                }

                internal static bool Enabled => _enabled.Value;

                internal static bool KeepWCharge => _keepWCharge.Value;

                internal static int MinMana => _minMana.Value;

                internal static void Initialize() { }
            }

            internal static class AutoE
            {
                internal static readonly Menu Menu;

                private static readonly MenuBool _killEnemyHeros;
                private static readonly MenuSliderButton _killSiegeMinions;
                private static readonly MenuBool _killSuperMinions;
                private static readonly MenuBool _killSmallJungle;
                private static readonly MenuBool _killBigJungle;
                private static readonly MenuBool _killLegendaryJungle;
                private static readonly MenuSliderButton _killUnkillableMinions;
                private static readonly MenuSliderButton _KillMinionsToHarassEnemyHeros;

                static AutoE()
                {
                    Menu = Auto.Menu.Add(new Menu("AutoE", "Auto E"));

                    Menu.Add(new MenuSeparator("info1", "Slider = Min Mana %"));
                    _killEnemyHeros = Menu.Add(new MenuBool("KillEnemyHeros", "Kill Enemy Heros", true));
                    _killSiegeMinions = Menu.Add(new MenuSliderButton("KillSiegeMinions", "Kill Siege Minions", 0, 0, 100, true)
                    {
                        BValue = true
                    });
                    _killSuperMinions = Menu.Add(new MenuBool("KillSuperMinions", "Kill Super Minions", true));
                    _killSmallJungle = Menu.Add(new MenuBool("KillSmallJungle", "Kill Small Jungle", false));
                    _killBigJungle = Menu.Add(new MenuBool("KillBigJungle", "Kill Big Jungle", true));
                    _killLegendaryJungle = Menu.Add(new MenuBool("KillLegendaryJungle", "Kill Legendary Jungle (Dragon, Baron)", true));
                    _killUnkillableMinions = Menu.Add(new MenuSliderButton("KillUnkillableMinions", "Kill Minions unkillable with AutoAttack", 30, 0, 100, true)
                    {
                        BValue = true
                    });
                    _KillMinionsToHarassEnemyHeros = Menu.Add(new MenuSliderButton("KillMinionsToHarassEnemyHeros", "Kill Minions to Harass Enemy heros", 50, 0, 100, true)
                    {
                        BValue = true
                    });
                }

                internal static bool KillEnemyHeros => _killEnemyHeros.Value;

                internal static bool KillSiegeMinions => _killSiegeMinions.BValue;
                internal static int KillSiegeMinionsMinMana => _killSiegeMinions.SValue;

                internal static bool KillSuperMinions => _killSuperMinions.Value;

                internal static bool KillSmallJungle => _killSmallJungle.Value;

                internal static bool KillBigJungle => _killBigJungle.Value;

                internal static bool KillLegendaryJungle => _killLegendaryJungle.Value;

                internal static bool KillUnkillableMinions => _killUnkillableMinions.BValue;
                internal static int KillUnkillableMinionsMinMana => _killUnkillableMinions.SValue;

                internal static bool KillMinionsToHarassEnemyHeros => _KillMinionsToHarassEnemyHeros.BValue;
                internal static int KillMinionsToHarassEnemyHerosMinMana => _KillMinionsToHarassEnemyHeros.SValue;

                internal static void Initialize() { }
            }

            internal static class AutoR
            {
                internal static readonly Menu Menu;

                private static readonly MenuBool _balista;

                //internal static bool SaveOathsworn => _saveOathsworn.Value;

                static AutoR()
                {
                    Menu = Auto.Menu.Add(new Menu("AutoR", "Auto R"));

                    Menu.Add(new MenuSeparator("collabo", "Collaborate"));

                    //_saveOathsworn = Menu.Add(new MenuBool("SaveOathsworn", "Save Oathsworn", true));
                    _balista = Menu.Add(new MenuBool("Balista", "Collaborate with Blitzcrank Q", true));
                }

                //private static readonly MenuBool _saveOathsworn;

                internal static bool Balista => _balista.Value;

                internal static void Initialize() { }
            }
        }

        internal static class Hitchance
        {
            internal static readonly Menu Menu;

            private static readonly MenuList<HitChance> _QHitchance;

            static Hitchance()
            {
                Menu = Config.Menu.Add(new Menu("Hitchance", "Hitchance"));

                _QHitchance = Menu.Add(new MenuList<HitChance>("QHitchance", "Q Hitchance", new[]
                {
                    HitChance.Medium, HitChance.High, HitChance.VeryHigh
                })
                {
                    SelectedValue = HitChance.High
                });
                _QHitchance.ValueChanged += (sender, args) =>
                {
                    SpellManager.Q.MinHitChance = _QHitchance.SelectedValue;
                };
            }

            internal static HitChance QHitChance => _QHitchance.SelectedValue;

            internal static void Initialize() { }
        }

        internal static class Misc
        {
            internal static readonly Menu Menu;

            private static readonly MenuSliderButton _eDamageAdjust;

            static Misc()
            {
                Menu = Config.Menu.Add(new Menu("Misc", "Misc"));

                _eDamageAdjust = Menu.Add(new MenuSliderButton("EDmageAdjust", "E Damage Adjust", -20, -100, 0)
                {
                    BValue = false
                });
            }

            internal static bool UseEdamageAdjust => _eDamageAdjust.BValue;

            internal static int EdamageAdjustValue => _eDamageAdjust.SValue;

            internal static void Initialize() { }
        }

        internal static class Drawings
        {
            internal static readonly Menu Menu;

            private static readonly MenuBool _drawQRange;
            private static readonly MenuBool _drawWRange;
            private static readonly MenuBool _drawERange;
            private static readonly MenuBool _drawRRange;

            private static readonly MenuBool _DamageIndicatorEnabled;
            private static readonly MenuBool _HerosEnabled;
            private static readonly MenuBool _JunglesEnabled;
            private static readonly MenuBool _DrawQDamage;
            private static readonly MenuBool _DrawEDamage;
            private static readonly MenuColor _QDamageColor;
            private static readonly MenuColor _EDamageColor;

            private static readonly MenuBool _drawOathswornPos;

            private static readonly MenuBool _drawSoulPosition;
            private static readonly MenuBool _drawSoulWaypoints;
            private static readonly MenuBool _displayRemainingRotations;

            static Drawings()
            {
                Menu = Config.Menu.Add(new Menu("Drawings", "Drawings"));

                Menu.Add(new MenuSeparator("SpellRange", "Spell Range"));
                _drawQRange = Menu.Add(new MenuBool("drawQRange", "Draw Q Range"));
                _drawWRange = Menu.Add(new MenuBool("drawWRange", "Draw W Range"));
                _drawERange = Menu.Add(new MenuBool("drawERange", "Draw E Range"));
                _drawRRange = Menu.Add(new MenuBool("drawRRange", "Draw R Range"));

                Menu.Add(new MenuSeparator("Oathsworn", "Oathsworn"));
                _drawOathswornPos = Menu.Add(new MenuBool("drawOathswornPos", "Draw Oathsworn(Soulbound) Position", true));

                Menu.Add(new MenuSeparator("Soul", "Souls"));
                _drawSoulPosition = Menu.Add(new MenuBool("drawSoulPosition", "Draw Souls Position", true));
                _drawSoulWaypoints = Menu.Add(new MenuBool("drawSoulWaypoints", "Draw Souls Waypoints", true));
                _displayRemainingRotations = Menu.Add(new MenuBool("DisplayRemainingRotations", "Display Souls Remaining Rotations", true));

                Menu.Add(new MenuSeparator("DamageIndicator", "Damage Indicator"));
                _DamageIndicatorEnabled = Menu.Add(new MenuBool("DamageIndicatorEnabled", "DamageIndicator Enabled", true));
                _HerosEnabled = Menu.Add(new MenuBool("HerosEnabled", "Draw on Heros", true));
                _JunglesEnabled = Menu.Add(new MenuBool("JunglesEnabled", "Draw on Jungles", true));
                _DrawQDamage = Menu.Add(new MenuBool("DrawQDamage", "Draw Q Damage", true));
                _DrawEDamage = Menu.Add(new MenuBool("DrawEDamage", "Draw E Damage", true));
                _QDamageColor = Menu.Add(new MenuColor("QdamageColor", "Q Damage Color", Color.Orange));
                _EDamageColor = Menu.Add(new MenuColor("EdamageColor", "E Damage Color", Color.MediumSpringGreen));
            }

            internal static bool DrawQRange => _drawQRange.Value;

            internal static bool DrawWRange => _drawWRange.Value;

            internal static bool DrawERange => _drawERange.Value;

            internal static bool DrawRRange => _drawRRange.Value;

            internal static bool DamageIndicatorEnabled => _DamageIndicatorEnabled.Value;

            internal static bool HerosEnabled => _HerosEnabled.Value;

            internal static bool JunglesEnabled => _JunglesEnabled.Value;

            internal static bool DrawQDamage => _DrawQDamage.Value;

            internal static bool DrawEDamage => _DrawEDamage.Value;

            internal static Color QDamageColor => _QDamageColor.Color;

            internal static Color EDamageColor => _EDamageColor.Color;

            internal static bool DrawOathswornPosition => _drawOathswornPos.Value;

            internal static bool DrawSoulPosition => _drawSoulPosition.Value;

            internal static bool DrawSoulWaypoints => _drawSoulWaypoints.Value;

            internal static bool DisplayRemainingRotations => _displayRemainingRotations.Value;

            internal static void Initialize() { }
        }
    }
}
