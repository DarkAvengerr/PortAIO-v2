namespace xSaliceResurrected_Rework.Managers
{
    using Base;
    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    public static class ManaManager
    {
        public static void AddManaManagertoMenu(Menu myMenu, string source, int standard)
        {
            myMenu.AddItem(new MenuItem(source + "_Manamanager", "Mana Manager", true).SetValue(new Slider(standard)));
        }

        public static bool FullManaCast()
        {
            return ObjectManager.Player.Mana >= SpellManager.QSpell.SData.Mana + SpellManager.WSpell.SData.Mana + SpellManager.ESpell.SData.Mana + SpellManager.RSpell.SData.Mana;
        }

        public static bool HasMana(string source)
        {
            return ObjectManager.Player.ManaPercent > Base.Champion.Menu.Item(source + "_Manamanager", true).GetValue<Slider>().Value;
        }
    }
}
