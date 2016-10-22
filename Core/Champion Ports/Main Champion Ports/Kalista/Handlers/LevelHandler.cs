using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using S_Plus_Class_Kalista.Structures;
using EloBuddy;

namespace S_Plus_Class_Kalista.Handlers
{
    class LevelHandler : Core
    {

        private const string _MenuNameBase = ".Level Menu";
        private const string _MenuItemBase = ".Level.";

        public static void Load()
        {
            SMenu.AddSubMenu(_Menu());
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.LevelDelay")) return;

            Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.LevelDelay");

            if (SMenu.Item(_MenuItemBase + "Boolean.AutoLevelUp").GetValue<bool>())
                LevelUpSpells();
        }

        private static int QOff = 0;
        private static int WOff = 0;
        private static int EOff = 0;
        private static int ROff = 0;


        private static void LevelUpSpells()
        {
            var qL = Player.Spellbook.GetSpell(Champion.Q.Slot).Level + QOff;
            var wL = Player.Spellbook.GetSpell(Champion.W.Slot).Level + WOff;
            var eL = Player.Spellbook.GetSpell(Champion.E.Slot).Level + EOff;
            var rL = Player.Spellbook.GetSpell(Champion.R.Slot).Level + ROff;

            if (qL + wL + eL + rL >= Player.Level) return;

            int[] level = { 0, 0, 0, 0 };
            if (SMenu.Item(_MenuItemBase + "Boolean.StartE").GetValue<bool>())
            {
                for (var i = 0; i < Player.Level; i++)
                {
                    level[LevelStructure.StartE[i] - 1] = level[LevelStructure.StartE[i] - 1] + 1;
                }
            }
            else
            {
                for (var i = 0; i < Player.Level; i++)
                {
                    level[LevelStructure.StartW[i] - 1] = level[LevelStructure.StartW[i] - 1] + 1;
                }
            }
            if (qL < level[0]) Player.Spellbook.LevelSpell(SpellSlot.Q);
            if (wL < level[1]) Player.Spellbook.LevelSpell(SpellSlot.W);
            if (eL < level[2]) Player.Spellbook.LevelSpell(SpellSlot.E);
            if (rL < level[3]) Player.Spellbook.LevelSpell(SpellSlot.R);

 
        }

        private static Menu _Menu()
        {
            var menu = new Menu(_MenuNameBase, "levelMenu");
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.AutoLevelUp", "Auto level-up abilities").SetValue(true));
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.StartE", "Start with E").SetValue(true));
            return menu;
        }

    }
}
