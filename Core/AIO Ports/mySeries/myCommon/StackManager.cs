using EloBuddy; 
using LeagueSharp.Common; 
namespace myCommon
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;

    public static class StackManager
    {
        private static float lastSpellCast;
        private static Menu Menu;

        public static void AddToMenu(Menu mainMenu, bool useQ = true, bool useW = true, bool useE = true)
        {
            Menu = mainMenu;

            mainMenu.AddItem(new MenuItem("AutoStack", "Auto Stack?", true).SetValue(true));
            mainMenu.AddItem(new MenuItem("AutoStackQ", "Use Q", true).SetValue(useQ));
            mainMenu.AddItem(new MenuItem("AutoStackW", "Use W", true).SetValue(useW));
            mainMenu.AddItem(new MenuItem("AutoStackE", "Use E", true).SetValue(useE));
            mainMenu.AddItem(
                new MenuItem("AutoStackMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(80)));

            Spellbook.OnCastSpell += OnCastSpell;
            Game.OnUpdate += OnUpdate;
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs Args)
        {
            if (sender.Owner.IsMe)
            {
                if (Args.Slot == SpellSlot.Q || Args.Slot == SpellSlot.W || Args.Slot == SpellSlot.E)
                {
                    lastSpellCast = Utils.TickCount;
                }
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (Menu.Item("AutoStack", true).GetValue<bool>() &&
                ObjectManager.Player.ManaPercent >= Menu.Item("AutoStackMana", true).GetValue<Slider>().Value)
            {
                if (Utils.TickCount - lastSpellCast < 4100)
                {
                    return;
                }

                if (!Items.HasItem(3003) && !Items.HasItem(3004) && !Items.HasItem(3070))
                {
                    return;
                }

                if (!HeroManager.Enemies.Any(x => x.DistanceToPlayer() <= 1800) &&
                    !MinionManager.GetMinions(ObjectManager.Player.Position, 1800, MinionTypes.All, MinionTeam.NotAlly).Any())
                {
                    if (Menu.Item("AutoStackQ", true).GetValue<bool>() &&
                        ObjectManager.Player.GetSpell(SpellSlot.Q).IsReady() &&
                        Utils.TickCount - lastSpellCast > 4100)
                    {
                        ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Q, Game.CursorPos);
                        lastSpellCast = Utils.TickCount;
                    }
                    else if (Menu.Item("AutoStackW", true).GetValue<bool>() &&
                             ObjectManager.Player.GetSpell(SpellSlot.W).IsReady() &&
                             Utils.TickCount - lastSpellCast > 4100)
                    {
                        ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, Game.CursorPos);
                        lastSpellCast = Utils.TickCount;
                    }
                    else if (Menu.Item("AutoStackE", true).GetValue<bool>() &&
                             ObjectManager.Player.GetSpell(SpellSlot.E).IsReady() &&
                             Utils.TickCount - lastSpellCast > 4100)
                    {
                        ObjectManager.Player.Spellbook.CastSpell(SpellSlot.E, Game.CursorPos);
                        lastSpellCast = Utils.TickCount;
                    }
                }
            }
        }
    }
}
