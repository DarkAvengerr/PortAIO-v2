using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iLucian.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using MenuHelper;

    class AutoCleanse
    {
        #region Static Fields

        private static readonly BuffType[] Buffs =
            {
                BuffType.Blind, BuffType.Charm, BuffType.CombatDehancer,
                BuffType.Fear, BuffType.Flee, BuffType.Knockback, BuffType.Knockup,
                BuffType.Polymorph, BuffType.Silence, //BuffType.Slow,
                BuffType.Snare, BuffType.Stun, BuffType.Suppression, BuffType.Taunt
            };

        private static readonly List<CleanseSpell> QssSpells = new List<CleanseSpell>
                                                               {
                                                                   new CleanseSpell
                                                                       {
                                                                           ChampName = "Warwick", IsEnabled = true, 
                                                                           SpellBuff = "InfiniteDuress", 
                                                                           SpellName = "Warwick R", RealName = "warwickR", 
                                                                           OnlyKill = false, Slot = SpellSlot.R, 
                                                                           Delay = 100f
                                                                       }, 
                                                                   new CleanseSpell
                                                                       {
                                                                           ChampName = "Amumu", IsEnabled = true, 
                                                                           SpellBuff = "CurseoftheSadMummy", 
                                                                           SpellName = "Amumum R", RealName = "Amumu R", 
                                                                           OnlyKill = false, Slot = SpellSlot.R, 
                                                                           Delay = 250f
                                                                       },
                                                                   new CleanseSpell
                                                                       {
                                                                           ChampName = "Rammus", IsEnabled = true, 
                                                                           SpellBuff = "PuncturingTaunt", 
                                                                           SpellName = "Rammus E", RealName = "rammusE", 
                                                                           OnlyKill = false, Slot = SpellSlot.E, 
                                                                           Delay = 100f
                                                                       }, /** Danger Level 4 Spells*/
                                                                   new CleanseSpell
                                                                       {
                                                                           ChampName = "Skarner", IsEnabled = true, 
                                                                           SpellBuff = "SkarnerImpale", 
                                                                           SpellName = "Skaner R", RealName = "skarnerR", 
                                                                           OnlyKill = false, Slot = SpellSlot.R, 
                                                                           Delay = 100f
                                                                       }, 
                                                                   new CleanseSpell
                                                                       {
                                                                           ChampName = "Fizz", IsEnabled = true, 
                                                                           SpellBuff = "FizzMarinerDoom", 
                                                                           SpellName = "Fizz R", RealName = "FizzR", 
                                                                           OnlyKill = false, Slot = SpellSlot.R, 
                                                                           Delay = 100f
                                                                       }, 
                                                                   new CleanseSpell
                                                                       {
                                                                           ChampName = "Galio", IsEnabled = true, 
                                                                           SpellBuff = "GalioIdolOfDurand", 
                                                                           SpellName = "Galio R", RealName = "GalioR", 
                                                                           OnlyKill = false, Slot = SpellSlot.R, 
                                                                           Delay = 100f
                                                                       }, 
                                                                   new CleanseSpell
                                                                       {
                                                                           ChampName = "Malzahar", IsEnabled = true, 
                                                                           SpellBuff = "AlZaharNetherGrasp", 
                                                                           SpellName = "Malz R", RealName = "MalzaharR", 
                                                                           OnlyKill = false, Slot = SpellSlot.R, 
                                                                           Delay = 200f
                                                                       }, /** Danger Level 3 Spells*/
                                                                   new CleanseSpell
                                                                       {
                                                                           ChampName = "Zilean", IsEnabled = false, 
                                                                           SpellBuff = "timebombenemybuff", 
                                                                           SpellName = "Zilean Q", OnlyKill = true, 
                                                                           Slot = SpellSlot.Q, Delay = 700f
                                                                       }, 
                                                                   new CleanseSpell
                                                                       {
                                                                           ChampName = "Vladimir", IsEnabled = false, 
                                                                           SpellBuff = "VladimirHemoplague", 
                                                                           SpellName = "Vlad R", RealName = "VladimirR", 
                                                                           OnlyKill = true, Slot = SpellSlot.R, 
                                                                           Delay = 700f
                                                                       }, 
                                                                   new CleanseSpell
                                                                       {
                                                                           ChampName = "Mordekaiser", IsEnabled = true, 
                                                                           SpellBuff = "MordekaiserChildrenOfTheGrave", 
                                                                           SpellName = "Morde R", OnlyKill = true, 
                                                                           Slot = SpellSlot.R, Delay = 800f
                                                                       }
                                                               };

        private static float lastCheckTick;

        #endregion

        #region Public Properties

        public static float Delay => Variables.Menu.Item("ilucian.cleanser.delay").GetValue<Slider>().Value;

        public static double HealthBuffer => Variables.Menu.Item("ilucian.cleanser.hpbuffer").GetValue<Slider>().Value;

        #endregion

        #region Public Methods and Operators

        public static void OnLoad()
        {
            var cName = ObjectManager.Player.ChampionName;
            var spellSubmenu = new Menu(":: i" + cName + " - Cleanser", cName + "Cleanser");

            // Spell Cleanser Menu
            var spellCleanserMenu = new Menu("Cleanser - Spell Cleanser", "ilucian.cleanser.spell");
            foreach (var spell in QssSpells.Where(h => GetChampByName(h.ChampName) != null))
            {
                var sMenu = new Menu(spell.SpellName, cName + spell.SpellBuff);
                sMenu.AddItem(
                    new MenuItem("ilucian.cleanser.spell." + spell.SpellBuff + "A", "Always").SetValue(!spell.OnlyKill));
                sMenu.AddItem(
                    new MenuItem("ilucian.cleanser.spell." + spell.SpellBuff + "K", "Only if killed by it").SetValue(
                        spell.OnlyKill));
                sMenu.AddItem(
                    new MenuItem("ilucian.cleanser.spell." + spell.SpellBuff + "D", "Delay before cleanse").SetValue(
                        new Slider((int)spell.Delay, 0, 10000)));
                spellCleanserMenu.AddSubMenu(sMenu);
            }

            // Bufftype cleanser menu
            var buffCleanserMenu = new Menu("Cleanser - Bufftype Cleanser", cName + "ilucian.cleanser.bufftype");

            foreach (var buffType in Buffs)
            {
                buffCleanserMenu.AddItem(new MenuItem(cName + buffType, buffType.ToString()).SetValue(true));
            }

            buffCleanserMenu.AddItem(
                new MenuItem("ilucian.cleanser.bufftype.minbuffs", "Min Buffs").SetValue(new Slider(2, 1, 5)));

            var allyMenu = new Menu("Cleanser - Use On", "UseOn");
            foreach (var ally in HeroManager.Allies)
            {
                allyMenu.AddItem(
                    new MenuItem("ilucian.cleanser.allies.useon." + ally.ChampionName, ally.ChampionName).SetValue(true));
            }

            spellSubmenu.AddItem(new MenuItem("ilucian.cleanser.items.qss", "Use QSS").SetValue(true));
            spellSubmenu.AddItem(
                new MenuItem("ilucian.cleanser.items.scimitar", "Use Mercurial Scimitar").SetValue(true));
            spellSubmenu.AddItem(new MenuItem("ilucian.cleanser.items.dervish", "Use Dervish Blade").SetValue(true));
            spellSubmenu.AddItem(new MenuItem("ilucian.cleanser.items.michael", "Use Mikael's Crucible").SetValue(true));
            spellSubmenu.AddItem(new MenuItem("ilucian.cleanser.items.cleanse", "Use Cleanse").SetValue(true));
            spellSubmenu.AddItem(new MenuItem("ilucian.cleanser.hpbuffer", "Health Buffer").SetValue(new Slider(20)));
            spellSubmenu.AddItem(
                new MenuItem("ilucian.cleanser.delay", "Global Delay (Prevents Lag)").SetValue(new Slider(100, 0, 200)));

            spellSubmenu.AddSubMenu(spellCleanserMenu);
            spellSubmenu.AddSubMenu(buffCleanserMenu);
            spellSubmenu.AddSubMenu(allyMenu);
            Variables.Menu.AddSubMenu(spellSubmenu);

            // Subscribe the Events
            Game.OnUpdate += Game_OnGameUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Cleanses by BuffType on player
        /// </summary>
        static void BuffTypeCleansing()
        {
            if (OneReady())
            {
                var buffCount = Buffs.Count(buff => ObjectManager.Player.HasBuffOfType(buff) && BuffTypeEnabled(buff));
                if (buffCount >= Variables.Menu.Item("ilucian.cleanser.bufftype.minbuffs").GetValue<Slider>().Value)
                {
                    CastCleanseItem(ObjectManager.Player);
                }
            }

            // Ally Cleansing
            if (!MichaelReady())
            {
                return;
            }

            var allies = ObjectManager.Player.GetAlliesInRange(600f);
            var highestAlly = ObjectManager.Player;
            var highestCount = 0;
            foreach (var ally in allies)
            {
                var allyBCount = Buffs.Count(buff => ally.HasBuffOfType(buff) && BuffTypeEnabled(buff));
                if (allyBCount > highestCount
                    && allyBCount >= Variables.Menu.Item("ilucian.cleanser.bufftype.minbuffs").GetValue<Slider>().Value
                    && Variables.Menu.IsEnabled("ilucian.cleanser.allies.useon." + ally.ChampionName))
                {
                    highestCount = allyBCount;
                    highestAlly = ally;
                }
            }

            if (!highestAlly.IsMe)
            {
                CastCleanseItem(highestAlly);
            }
        }

        private static bool BuffTypeEnabled(BuffType buffType)
        {
            return Variables.Menu.IsEnabled(ObjectManager.Player.ChampionName + buffType);
        }

        static void CastCleanseItem(AIHeroClient target)
        {
            if (target == null)
            {
                return;
            }

            if (Variables.Menu.IsEnabled("ilucian.cleanser.items.michael") && Items.HasItem(3222)
                && Items.CanUseItem(3222) && target.IsValidTarget(600f))
            {
                Items.UseItem(3222, target);
                return;
            }

            if (Variables.Menu.IsEnabled("ilucian.cleanser.items.qss") && Items.HasItem(3140) && Items.CanUseItem(3140)
                && target.IsMe)
            {
                Items.UseItem(3140, ObjectManager.Player);
                return;
            }

            if (Variables.Menu.IsEnabled("ilucian.cleanser.items.scimitar") && Items.HasItem(3139)
                && Items.CanUseItem(3139) && target.IsMe)
            {
                Items.UseItem(3139, ObjectManager.Player);
                return;
            }

            if (Variables.Menu.IsEnabled("ilucian.cleanser.items.dervish") && Items.HasItem(3137)
                && Items.CanUseItem(3137) && target.IsMe)
            {
                Items.UseItem(3137, ObjectManager.Player);
            }
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (Environment.TickCount - lastCheckTick < Delay) return;
            lastCheckTick = Environment.TickCount;

            KillCleansing();
            SpellCleansing();
            BuffTypeCleansing();
        }

        private static AIHeroClient GetChampByName(string enemyName)
        {
            return ObjectManager.Get<AIHeroClient>().Find(h => h.IsEnemy && h.ChampionName == enemyName);
        }

        /// <summary>
        ///     Will Cleanse only on Kill
        /// </summary>
        static void KillCleansing()
        {
            if (OneReady())
            {
                CleanseSpell mySpell = null;
                if (
                    QssSpells.Where(
                        spell =>
                        ObjectManager.Player.HasBuff(spell.SpellBuff) && SpellEnabledOnKill(spell.SpellBuff)
                        && GetChampByName(spell.ChampName).GetSpellDamage(ObjectManager.Player, spell.Slot)
                        > ObjectManager.Player.Health + HealthBuffer)
                        .OrderBy(
                            spell => GetChampByName(spell.ChampName).GetSpellDamage(ObjectManager.Player, spell.Slot))
                        .Any())
                {
                    mySpell =
                        QssSpells.Where(
                            spell =>
                            ObjectManager.Player.HasBuff(spell.SpellBuff) && SpellEnabledOnKill(spell.SpellBuff))
                            .OrderBy(
                                spell =>
                                GetChampByName(spell.ChampName).GetSpellDamage(ObjectManager.Player, spell.Slot))
                            .First();
                }

                if (mySpell != null)
                {
                    UseCleanser(mySpell, ObjectManager.Player);
                }
            }

            if (!MichaelReady())
            {
                return;
            }

            // Ally Cleansing
            var allies = ObjectManager.Player.GetAlliesInRange(600f);
            var highestAlly = ObjectManager.Player;
            var highestDamage = 0f;
            CleanseSpell highestSpell = null;
            foreach (var ally in allies)
            {
                CleanseSpell theSpell = null;
                if (
                    QssSpells.Where(
                        spell =>
                        ally.HasBuff(spell.SpellBuff) && SpellEnabledOnKill(spell.SpellBuff)
                        && GetChampByName(spell.ChampName).GetSpellDamage(ally, spell.Slot) > ally.Health + HealthBuffer)
                        .OrderBy(spell => GetChampByName(spell.ChampName).GetSpellDamage(ally, spell.Slot))
                        .Any())
                {
                    theSpell =
                        QssSpells.Where(spell => ally.HasBuff(spell.SpellBuff) && SpellEnabledOnKill(spell.SpellBuff))
                            .OrderBy(spell => GetChampByName(spell.ChampName).GetSpellDamage(ally, spell.Slot))
                            .First();
                }

                if (theSpell != null)
                {
                    var damageDone = GetChampByName(theSpell.ChampName).GetSpellDamage(ally, theSpell.Slot);
                    if (damageDone >= highestDamage
                        && Variables.Menu.IsEnabled("ilucian.cleanser.allies.useon." + ally.ChampionName))
                    {
                        highestSpell = theSpell;
                        highestDamage = (float)damageDone;
                        highestAlly = ally;
                    }
                }
            }

            if (!highestAlly.IsMe && highestSpell != null)
            {
                UseCleanser(highestSpell, highestAlly);
            }
        }

        private static bool MichaelReady()
        {
            return Variables.Menu.IsEnabled("ilucian.cleanser.items.michael") && Items.HasItem(3222)
                   && Items.CanUseItem(3222);
        }

        private static bool OneReady()
        {
            return (Variables.Menu.IsEnabled("ilucian.cleanser.items.qss") && Items.HasItem(3140)
                    && Items.CanUseItem(3140))
                   || (Variables.Menu.IsEnabled("ilucian.cleanser.items.scimitar") && Items.HasItem(3139)
                       && Items.CanUseItem(3139))
                   || (Variables.Menu.IsEnabled("ilucian.cleanser.items.dervish") && Items.HasItem(3137)
                       && Items.CanUseItem(3137));
        }

        /// <summary>
        ///     Cleanses using the SpellList buffs as input
        /// </summary>
        static void SpellCleansing()
        {
            if (OneReady())
            {
                CleanseSpell mySpell = null;
                if (
                    QssSpells.Where(
                        spell => ObjectManager.Player.HasBuff(spell.SpellBuff) && SpellEnabledAlways(spell.SpellBuff))
                        .OrderBy(
                            spell => GetChampByName(spell.ChampName).GetSpellDamage(ObjectManager.Player, spell.Slot))
                        .Any())
                {
                    mySpell =
                        QssSpells.Where(
                            spell =>
                            ObjectManager.Player.HasBuff(spell.SpellBuff) && SpellEnabledAlways(spell.SpellBuff))
                            .OrderBy(
                                spell =>
                                GetChampByName(spell.ChampName).GetSpellDamage(ObjectManager.Player, spell.Slot))
                            .First();
                }

                if (mySpell != null)
                {
                    UseCleanser(mySpell, ObjectManager.Player);
                }
            }

            if (!MichaelReady())
            {
                return;
            }

            // Ally Cleansing
            var allies = ObjectManager.Player.GetAlliesInRange(600f);
            var highestAlly = ObjectManager.Player;
            var highestDamage = 0f;
            CleanseSpell highestSpell = null;
            foreach (var ally in allies)
            {
                CleanseSpell theSpell = null;
                if (
                    QssSpells.Where(spell => ally.HasBuff(spell.SpellBuff) && SpellEnabledAlways(spell.SpellBuff))
                        .OrderBy(spell => GetChampByName(spell.ChampName).GetSpellDamage(ally, spell.Slot))
                        .Any())
                {
                    theSpell =
                        QssSpells.Where(spell => ally.HasBuff(spell.SpellBuff) && SpellEnabledAlways(spell.SpellBuff))
                            .OrderBy(spell => GetChampByName(spell.ChampName).GetSpellDamage(ally, spell.Slot))
                            .First();
                }

                if (theSpell != null)
                {
                    var damageDone = GetChampByName(theSpell.ChampName).GetSpellDamage(ally, theSpell.Slot);
                    if (damageDone >= highestDamage
                        && Variables.Menu.IsEnabled("ilucian.cleanser.allies.useon." + ally.ChampionName))
                    {
                        highestSpell = theSpell;
                        highestDamage = (float)damageDone;
                        highestAlly = ally;
                    }
                }
            }

            if (!highestAlly.IsMe && highestSpell != null)
            {
                UseCleanser(highestSpell, highestAlly);
            }
        }

        private static int SpellDelay(string sName)
        {
            return Variables.Menu.Item("ilucian.cleanser.spell." + sName + "D").GetValue<Slider>().Value;
        }

        private static bool SpellEnabledAlways(string sName)
        {
            return Variables.Menu.IsEnabled("ilucian.cleanser.spell." + sName + "A");
        }

        private static bool SpellEnabledOnKill(string sName)
        {
            return Variables.Menu.IsEnabled("ilucian.cleanser.spell." + sName + "K");
        }

        static void UseCleanser(CleanseSpell spell, AIHeroClient target)
        {
            LeagueSharp.Common.Utility.DelayAction.Add(SpellDelay(spell.RealName), () => CastCleanseItem(target));
        }

        #endregion
    }

    internal class CleanseSpell
    {
        #region Public Properties

        public string ChampName { get; set; }

        public float Delay { get; set; }

        public bool IsEnabled { get; set; }

        public bool OnlyKill { get; set; }

        public string RealName { get; set; }

        public SpellSlot Slot { get; set; }

        public string SpellBuff { get; set; }

        public string SpellName { get; set; }

        #endregion
    }
}