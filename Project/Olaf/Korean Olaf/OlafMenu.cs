using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanOlaf
{
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    class OlafMenu
    {
        public Menu MainMenu { get; set; }

        private readonly OlafSpells olafSpells;

        public OlafMenu(OlafSpells olafSpells, out Orbwalking.Orbwalker orbwalker)
        {
            MainMenu = new Menu("Korean Olaf", "mainmenu", true);
            this.olafSpells = olafSpells;

            AddTargetSelector();
            AddOrbwalker(out orbwalker);
            ComboMenu();
            HarassMenu();
            LaneClearMenu();
            LastHitMenu();
            MiscMenu();
            DrawingMenu();

            MainMenu.AddToMainMenu();

            GetInitialSpellValues();
        }

        private void AddTargetSelector()
        {
            Menu targetSelectorMenu = new Menu("Target Selector", "olaftargetselector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            MainMenu.AddSubMenu(targetSelectorMenu);
        }

        private void AddOrbwalker(out Orbwalking.Orbwalker orbwalker)
        {
            Menu orbwalkerMenu = new Menu("Orbwalker", "olaforbwalker");
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            MainMenu.AddSubMenu(orbwalkerMenu);
        }

        private void ComboMenu()
        {
            string prefix = "koreanolaf.combomenu";
            Menu comboMenu = new Menu("Combo", prefix);

            comboMenu.AddItem(new MenuItem(prefix + ".useq", "Use Q").SetValue(true)).ValueChanged += (sender, args) =>
            { olafSpells.Q.UseOnCombo = args.GetNewValue<bool>(); };

            comboMenu.AddItem(new MenuItem(prefix + ".usew", "Use W").SetValue(true)).ValueChanged += (sender, args) =>
            { olafSpells.W.UseOnCombo = args.GetNewValue<bool>(); };

            comboMenu.AddItem(new MenuItem(prefix + ".usee", "Use E").SetValue(true)).ValueChanged += (sender, args) =>
            { olafSpells.E.UseOnCombo = args.GetNewValue<bool>(); };

            //comboMenu.AddItem(new MenuItem(prefix + ".user", "Use R").SetValue(true)).ValueChanged += (sender, args) =>
            //{ olafSpells.R.UseOnCombo = args.GetNewValue<bool>(); };

            string useItemsPrefix = prefix + ".items";
            Menu useItems = new Menu("Use Items", useItemsPrefix);

            useItems.AddItem(new MenuItem(useItemsPrefix + ".bilgewater", "Use Bilgewater Cutlass").SetValue(true));
            useItems.AddItem(new MenuItem(useItemsPrefix + ".botrk", "Use BotRK").SetValue(true));
            useItems.AddItem(new MenuItem(useItemsPrefix + ".yomuus", "Use Yoomuu's GhostBlade").SetValue(true));
            useItems.AddItem(new MenuItem(useItemsPrefix + ".hydra", "Use Hydra").SetValue(true));
            useItems.AddItem(new MenuItem(useItemsPrefix + ".tiamat", "Use Tiamat").SetValue(true));

            comboMenu.AddSubMenu(useItems);
            MainMenu.AddSubMenu(comboMenu);
        }

        private void HarassMenu()
        {
            string prefix = "koreanolaf.harasmenu";
            Menu harasMenu = new Menu("Harass", prefix);

            harasMenu.AddItem(new MenuItem(prefix + ".useq", "Use Q").SetValue(true)).ValueChanged += (sender, args) =>
            { olafSpells.Q.UseOnHarass = args.GetNewValue<bool>(); };

            harasMenu.AddItem(new MenuItem(prefix + ".usew", "Use W").SetValue(true)).ValueChanged += (sender, args) =>
            { olafSpells.W.UseOnHarass = args.GetNewValue<bool>(); };

            harasMenu.AddItem(new MenuItem(prefix + ".usee", "Use E").SetValue(true)).ValueChanged += (sender, args) =>
            { olafSpells.E.UseOnHarass = args.GetNewValue<bool>(); };

            harasMenu.AddItem(new MenuItem(prefix + ".manalimit", "Mana limit").SetValue(new Slider(65)));

            string blackListPrefix = prefix + ".blacklist";
            Menu blackListHaras = new Menu("Harass Target(s)", blackListPrefix + "");
            foreach (var objAiHero in HeroManager.Enemies)
            {
                blackListHaras.AddItem(
                    new MenuItem(
                        string.Format("{0}.{1}", blackListPrefix, objAiHero.BaseSkinName.ToLowerInvariant()),
                        objAiHero.BaseSkinName).SetValue(true));
            }

            string useItemsPrefix = prefix + ".items";
            Menu useItems = new Menu("Use Items", useItemsPrefix);
            useItems.AddItem(new MenuItem(useItemsPrefix + ".hydra", "Use Hydra").SetValue(true));
            useItems.AddItem(new MenuItem(useItemsPrefix + ".tiamat", "Use Tiamat").SetValue(true));

            harasMenu.AddSubMenu(blackListHaras);
            harasMenu.AddSubMenu(useItems);
            MainMenu.AddSubMenu(harasMenu);
        }

        private void LaneClearMenu()
        {
            string prefix = "koreanolaf.laneclearmenu";
            Menu laneClearMenu = new Menu("Lane / Jungle Clear", prefix);

            laneClearMenu.AddItem(new MenuItem(prefix + ".useq", "Use Q").SetValue(true)).ValueChanged +=
                (sender, args) =>
                {
                    olafSpells.Q.UseOnLaneClear = args.GetNewValue<bool>();
                };

            laneClearMenu.AddItem(new MenuItem(prefix + ".useqif", "Min. Minions to Q").SetValue(new Slider(3, 1, 6)));

            laneClearMenu.AddItem(new MenuItem(prefix + ".usew", "Use W").SetValue(true)).ValueChanged +=
                (sender, args) =>
                {
                    olafSpells.W.UseOnLaneClear = args.GetNewValue<bool>();
                };

            laneClearMenu.AddItem(new MenuItem(prefix + ".usee", "Use E").SetValue(true)).ValueChanged +=
                (sender, args) =>
                {
                    olafSpells.E.UseOnLaneClear = args.GetNewValue<bool>();
                };

            laneClearMenu.AddItem(new MenuItem(prefix + ".manalimit", "Mana limit").SetValue(new Slider(50)));
            laneClearMenu.AddItem(new MenuItem(prefix + ".healthlimit", "Health limit (E)").SetValue(new Slider(15)));

            string useItemsPrefix = prefix + ".items";
            Menu useItems = new Menu("Use Items", useItemsPrefix);

            useItems.AddItem(new MenuItem(useItemsPrefix + ".hydra", "Use Hydra").SetValue(true));
            useItems.AddItem(new MenuItem(useItemsPrefix + ".tiamat", "Use Tiamat").SetValue(true));
            useItems.AddItem(new MenuItem(useItemsPrefix + ".when", "When will hit X minions").SetValue(new Slider(3, 1, 10)));

            laneClearMenu.AddSubMenu(useItems);

            MainMenu.AddSubMenu(laneClearMenu);
        }

        private void LastHitMenu()
        {
            string prefix = "koreanolaf.lasthitmenu";
            Menu lastHitMenu = new Menu("Last Hit", prefix);

            lastHitMenu.AddItem(new MenuItem(prefix + ".useq", "Use Q").SetValue(true)).ValueChanged +=
                (sender, args) =>
                {
                    olafSpells.Q.UseOnLastHit = args.GetNewValue<bool>();
                };

            lastHitMenu.AddItem(new MenuItem(prefix + ".usee", "Use E").SetValue(true)).ValueChanged +=
                (sender, args) =>
                {
                    olafSpells.E.UseOnLastHit = args.GetNewValue<bool>();
                };

            lastHitMenu.AddItem(new MenuItem(prefix + ".manalimit", "Mana limit").SetValue(new Slider(70)));

            MainMenu.AddSubMenu(lastHitMenu);
        }

        private void GetInitialSpellValues()
        {
            olafSpells.Q.UseOnCombo = GetParamBool("koreanolaf.combomenu.useq");
            olafSpells.W.UseOnCombo = GetParamBool("koreanolaf.combomenu.usew");
            olafSpells.E.UseOnCombo = GetParamBool("koreanolaf.combomenu.usee");
            //olafSpells.R.UseOnCombo = GetParamBool("koreanolaf.combomenu.user");

            olafSpells.Q.UseOnHarass = GetParamBool("koreanolaf.harasmenu.useq");
            olafSpells.W.UseOnHarass = GetParamBool("koreanolaf.harasmenu.usew");
            olafSpells.E.UseOnHarass = GetParamBool("koreanolaf.harasmenu.usee");

            olafSpells.Q.UseOnLastHit = GetParamBool("koreanolaf.lasthitmenu.useq");
            olafSpells.E.UseOnLastHit = GetParamBool("koreanolaf.lasthitmenu.usee");

            olafSpells.Q.UseOnLaneClear = GetParamBool("koreanolaf.laneclearmenu.useq");
            olafSpells.W.UseOnLaneClear = GetParamBool("koreanolaf.laneclearmenu.usew");
            olafSpells.E.UseOnLaneClear = GetParamBool("koreanolaf.laneclearmenu.usee");
        }

        private void MiscMenu()
        {
            string prefix = "koreanolaf.miscmenu";
            Menu miscMenu = new Menu("Misc Menu", prefix);

            miscMenu.AddItem(new MenuItem(prefix + ".savee", "Save E to Siege minion").SetValue(true));

            string potPrefix = prefix + ".pot";
            Menu usePotionMenu = new Menu("Use Potion", potPrefix);
            usePotionMenu.AddItem(new MenuItem(potPrefix + ".healthactive", "Health Potion").SetValue(true));
            usePotionMenu.AddItem(new MenuItem(potPrefix + ".healthwhen", "Health trigger").SetValue(new Slider(45)));

            miscMenu.AddItem(new MenuItem(prefix + ".forceultimate", "Force R Using Mouse Buttons (Cursor Sprite)").SetValue(true));

            miscMenu.AddSubMenu(usePotionMenu);
            MainMenu.AddSubMenu(miscMenu);
        }

        private void DrawingMenu()
        {
            string prefix = "koreanolaf.drawing";
            Menu drawingMenu = new Menu("Drawings", prefix);

            drawingMenu.AddItem(new MenuItem(prefix + ".skillranges", "Skill Ranges").SetValue(true));
            drawingMenu.AddItem(new MenuItem(prefix + ".legacysaxe", "Legacy's axe indicator").SetValue(true));
            drawingMenu.AddItem(new MenuItem(prefix + ".legacysaxecolor", "Axe indicator color").SetValue(
                    new StringList(new string[] { "Green", "Red", "White" })));
            drawingMenu.AddItem(new MenuItem(prefix + ".legacysaxewidth", "Axe indicator width").SetValue(new Slider(1, 1, 3)));

            MainMenu.AddSubMenu(drawingMenu);
        }

        public int GetParamSlider(string paramName)
        {
            return MainMenu.Item(paramName).GetValue<Slider>().Value;
        }

        public bool GetParamBool(string paramName)
        {
            return MainMenu.Item(paramName).GetValue<bool>();
        }

        public int GetParamStringList(string paramName)
        {
            return MainMenu.Item(paramName).GetValue<StringList>().SelectedIndex;
        }

        public bool CheckMenuItem(string paramName)
        {
            return (MainMenu.Item(paramName) != null);
        }

        public List<AIHeroClient> GetHarasBlockList()
        {
            List<AIHeroClient> blackList = new List<AIHeroClient>();

            foreach (AIHeroClient objAiHero in HeroManager.Enemies)
            {
                if (!GetParamBool("koreanolaf.harasmenu.blacklist." + objAiHero.BaseSkinName.ToLowerInvariant()))
                {
                    blackList.Add(objAiHero);
                }
            }

            return blackList;
        }
    }
}