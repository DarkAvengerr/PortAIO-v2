using System;
using System.Linq;

using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.UI;

using LeagueSharp.Data.Enumerations;

using SharpDX;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcSivir
{
    internal static class Config
    {
        private const string MenuName = "Sivir";

        internal static readonly Menu Menu;

        static Config()
        {
            Menu = new Menu(MenuName, MenuName, true).Attach();

            Keys.Initialize();
            Modes.Initialize();
            Auto.Initialize();
            Misc.Initialize();
            Hitchance.Initialize();
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
                private static readonly MenuBool _useW;

                static Combo()
                {
                    Menu = Modes.Menu.Add(new Menu("Combo", "Combo"));

                    _useQ = Menu.Add(new MenuBool("UseQ", "Use Q", true));
                    _useW = Menu.Add(new MenuBool("UseW", "Use W", true));
                }

                internal static bool UseQ => _useQ.Value;

                internal static bool UseW => _useW.Value;

                internal static void Initialize() { }
            }

            internal static class Harass
            {
                internal static readonly Menu Menu;

                private static readonly MenuBool _useQ;
                private static readonly MenuBool _useW;

                private static readonly MenuSliderButton _minMana;

                static Harass()
                {
                    Menu = Modes.Menu.Add(new Menu("Harass", "Harass"));

                    _useQ = Menu.Add(new MenuBool("UseQ", "Use Q", true));
                    _useW = Menu.Add(new MenuBool("UseW", "Use W", true));
                    _minMana = Menu.Add(new MenuSliderButton("Mana", "Min Mana %", 70, 0, 100)
                    {
                        BValue = true
                    });
                }

                internal static bool UseQ => _useQ.Value;

                internal static bool UseW => _useW.Value;

                internal static int MinMana => _minMana.Value;

                internal static void Initialize() { }
            }

            internal static class LaneClear
            {
                internal static readonly Menu Menu;

                private static readonly MenuSliderButton _minMana;

                private static readonly MenuBool _useQ;
                private static readonly MenuBool _useW;

                static LaneClear()
                {
                    Menu = Modes.Menu.Add(new Menu("LaneClear", "LaneClear"));

                    _useQ = Menu.Add(new MenuBool("UseQ", "Use Q", false));
                    _useW = Menu.Add(new MenuBool("UseW", "Use W", false));
                    _minMana = Menu.Add(new MenuSliderButton("MinMana", "Min Mana %", 70, 0, 100, true));
                }

                internal static bool UseQ => _useQ.Value;

                internal static bool UseW => _useW.Value;

                internal static int MinMana => _minMana.Value;

                internal static void Initialize() { }
            }

            internal static class JungleClear
            {
                internal static readonly Menu Menu;

                private static readonly MenuBool _useQ;
                private static readonly MenuBool _useW;
                private static readonly MenuSliderButton _minMana;

                static JungleClear()
                {
                    Menu = Modes.Menu.Add(new Menu("JungleClear", "JungleClear"));

                    _useQ = Menu.Add(new MenuBool("UseQ", "Use Q", true));
                    _useW = Menu.Add(new MenuBool("UseW", "Use W", true));
                    _minMana = Menu.Add(new MenuSliderButton("MinMana", "Min Mana %", 0, 0, 100, true));
                }

                internal static bool UseQ => _useQ.Value;

                internal static bool UseW => _useW.Value;

                internal static int MinMana => _minMana.Value;

                internal static void Initialize() { }
            }

            internal static class Flee
            {
                internal static readonly Menu Menu;

                private static readonly MenuBool _useR;

                static Flee()
                {
                    Menu = Modes.Menu.Add(new Menu("Flee", "Flee"));

                    _useR = Menu.Add(new MenuBool("UseR", "Use R", false));
                }

                internal static bool UseR => _useR.Value;

                internal static void Initialize() { }
            }
        }

        internal static class Auto
        {
            internal static readonly Menu Menu;

            static Auto()
            {
                Menu = Config.Menu.Add(new Menu("Auto", "Auto"));

                //AutoHarass.Initialize();
                AutoE.Initialize();
            }

            internal static void Initialize() { }

            //internal static class AutoHarass
            //{
            //    internal static readonly Menu Menu;

            //    static AutoHarass()
            //    {
            //        Menu = Auto.Menu.Add(new Menu("AutoHarass", "Auto Harass"));
            //    }

            //    internal static void Initialize() { }
            //}

            internal static class AutoE
            {
                internal static readonly Menu Menu;

                private static readonly MenuBool _autoEAgainstTargetedSpells;

                static AutoE()
                {
                    Menu = Auto.Menu.Add(new Menu("AutoE", "Auto E"));

                    _autoEAgainstTargetedSpells = Menu.Add(new MenuBool("AutoEAgainstTargetedSpells", "Auto E Against Targeted Spells", true));

                    TargetedSpells.Initialize();
                }

                internal static bool AutoEAgainstTargetedSpells => _autoEAgainstTargetedSpells.Value;

                internal static void Initialize() { }

                internal static class TargetedSpells
                {
                    internal static readonly Menu Menu;

                    static TargetedSpells()
                    {
                        Menu = AutoE.Menu.Add(new Menu("TargetedSpells", "Targeted Spells"));

                        foreach (var enemy in GameObjects.EnemyHeroes)
                        {
                            foreach (var spell in SpellDatabase.Get().Where(
                                x => x.ChampionName.Equals(enemy.ChampionName, StringComparison.OrdinalIgnoreCase)
                            && (x.SpellType.HasFlag(SpellType.Targeted) || x.SpellType.HasFlag(SpellType.TargetedMissile))
                            && x.CastType.Contains(CastType.EnemyChampions)))
                            {
                                Menu.Add(new MenuBool($"{enemy.ChampionName}.{spell.SpellName}", $"Enemy {spell.ChampionName}: {spell.SpellName} ({spell.Slot})", true));
                            }

                            //추가 스펠 옵션
                            if (enemy.ChampionName.Equals("Udyr", StringComparison.OrdinalIgnoreCase))
                            {
                                Menu.Add(new MenuBool("Udyr.AttackStun", "Enemy Udyr: AttackStun (E)", true));
                            }
                            else
                            if (enemy.ChampionName.Equals("Braum", StringComparison.OrdinalIgnoreCase))
                            {
                                Menu.Add(new MenuBool("Braum.PassiveStun", "Enemy Braum: PassiveStun (Passive)", true));
                            }
                            else
                            if (enemy.ChampionName.Equals("Alistar", StringComparison.OrdinalIgnoreCase))
                            {
                                Menu.Add(new MenuBool("Alistar.Pulverize", "Enemy Alistar: Pulverize (Q)", true));
                            }
                            else
                            if (enemy.ChampionName.Equals("Leona", StringComparison.OrdinalIgnoreCase))
                            {
                                Menu.Add(new MenuBool("Leona.AttackStun", "Enemy Leona: AttackStun (Q)", true));
                            }
                            else
                            if (enemy.ChampionName.Equals("Renekton", StringComparison.OrdinalIgnoreCase))
                            {
                                Menu.Add(new MenuBool("Renekton.AttackStun", "Enemy Renekton: AttackStun (W)", true));
                            }
                            else
                            if (enemy.ChampionName.Equals("Garen", StringComparison.OrdinalIgnoreCase))
                            {
                                Menu.Add(new MenuBool("Garen.QAttack", "Enemy Garen: Q Silence Attack (Q)", true));
                            }
                        }
                    }

                    internal static void Initialize() { }
                }
            }
        }

        internal static class Hitchance
        {
            internal static readonly Menu Menu;

            private static readonly MenuList<HitChance> _QHitchance;
            //private static readonly MenuList<HitChance> _WHitchance;
            //private static readonly MenuList<HitChance> _EHitchance;
            //private static readonly MenuList<HitChance> _RHitchance;

            internal static HitChance QHitChance => _QHitchance.SelectedValue;

            //internal static HitChance WHitChance => _WHitchance.SelectedValue;

            //internal static HitChance EHitChance => _EHitchance.SelectedValue;

            //internal static HitChance RHitChance => _RHitchance.SelectedValue;

            static Hitchance()
            {
                Menu = Config.Menu.Add(new Menu("Hitchance", "Hitchance"));

                _QHitchance = Menu.Add(new MenuList<HitChance>("QHitchance", "Q Hitchance", new[] { HitChance.Medium, HitChance.High, HitChance.VeryHigh }) { SelectedValue = HitChance.High });
                _QHitchance.ValueChanged += (sender, args) =>
                {
                    SpellManager.Q.MinHitChance = _QHitchance.SelectedValue;
                };

                //_WHitchance = Menu.Add(new MenuList<HitChance>("WHitchance", "W Hitchance", new[] { HitChance.Medium, HitChance.High, HitChance.VeryHigh }) { SelectedValue = HitChance.High });
                //_WHitchance.ValueChanged += (sender, args) =>
                //{
                //    SpellManager.W.MinHitChance = _WHitchance.SelectedValue;
                //};

                //_EHitchance = Menu.Add(new MenuList<HitChance>("EHitchance", "E Hitchance", new[] { HitChance.Medium, HitChance.High, HitChance.VeryHigh }) { SelectedValue = HitChance.High });
                //_EHitchance.ValueChanged += (sender, args) =>
                //{
                //    SpellManager.E.MinHitChance = _EHitchance.SelectedValue;
                //};

                //_RHitchance = Menu.Add(new MenuList<HitChance>("RHitchance", "R Hitchance", new[] { HitChance.Medium, HitChance.High, HitChance.VeryHigh }) { SelectedValue = HitChance.High });
                //_RHitchance.ValueChanged += (sender, args) =>
                //{
                //    SpellManager.R.MinHitChance = _RHitchance.SelectedValue;
                //};
            }

            internal static void Initialize() { }
        }

        internal static class Misc
        {
            internal static readonly Menu Menu;

            static Misc()
            {
                Menu = Config.Menu.Add(new Menu("Misc", "Misc"));

                AntiGapcloser.Initialize();
                //AutoInterrupt.Initialize();
            }

            internal static void Initialize() { }

            internal static class AntiGapcloser
            {
                internal static readonly Menu Menu;

                private static readonly MenuBool _enabled;
                private static readonly MenuBool _useR;

                static AntiGapcloser()
                {
                    Menu = Misc.Menu.Add(new Menu("AntiGapcloser", "Anti Gapcloser"));

                    _enabled = Menu.Add(new MenuBool("Enabled", "Enabled", true));

                    Menu.Add(new MenuSeparator("1", " "));
                    _useR = Menu.Add(new MenuBool("UseR", "Use R", true));
                }

                internal static bool Enabled => _enabled.Value;

                internal static bool UseR => _useR.Value;

                internal static void Initialize() { }
            }

            //internal static class AutoInterrupt
            //{
            //    internal static readonly Menu Menu;

            //    private static readonly MenuBool _enabled;
            //    private static readonly MenuBool _useQ;

            //    static AutoInterrupt()
            //    {
            //        Menu = Misc.Menu.Add(new Menu("AutoInterrupt", "Auto Interrupt"));

            //        _enabled = Menu.Add(new MenuBool("Enabled", "Enabled", true));

            //        Menu.Add(new MenuSeparator("1", " "));
            //        _useQ = Menu.Add(new MenuBool("UseQ", "Use Q", true));
            //    }

            //    internal static bool Enabled => _enabled.Value;

            //    internal static bool UseQ => _useQ.Value;

            //    internal static void Initialize() { }
            //}
        }

        internal static class Drawings
        {
            internal static readonly Menu Menu;

            private static readonly MenuBool _drawQRange;
            //private static readonly MenuBool _drawWRange;
            //private static readonly MenuBool _drawERange;
            private static readonly MenuBool _drawRRange;

            static Drawings()
            {
                Menu = Config.Menu.Add(new Menu("Drawings", "Drawings"));

                Menu.Add(new MenuSeparator("SpellRange", "Spell Range"));

                _drawQRange = Menu.Add(new MenuBool("drawQRange", "Draw Q Range"));
                //_drawWRange = Menu.Add(new MenuBool("drawWRange", "Draw W Range"));
                //_drawERange = Menu.Add(new MenuBool("drawERange", "Draw E Range"));
                _drawRRange = Menu.Add(new MenuBool("drawRRange", "Draw R Range"));
            }

            internal static bool DrawQRange => _drawQRange.Value;

            //internal static bool DrawWRange => _drawWRange.Value;

            //internal static bool DrawERange => _drawERange.Value;

            internal static bool DrawRRange => _drawRRange.Value;

            internal static void Initialize() { }
        }
    }
}
