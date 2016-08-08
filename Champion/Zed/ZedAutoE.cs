using EloBuddy; namespace KoreanZed
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    class ZedAutoE
    {
        private readonly ZedMenu zedMenu;

        private readonly ZedShadows zedShadows;

        private readonly ZedSpell e;

        public ZedAutoE(ZedMenu zedMenu, ZedShadows zedShadows, ZedSpells zedSpells)
        {
            this.zedMenu = zedMenu;
            this.zedShadows = zedShadows;
            e = zedSpells.E;

            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (!e.LSIsReady() || ObjectManager.Player.Mana < e.ManaCost || !zedMenu.GetParamBool("koreanzed.miscmenu.autoe"))
            {
                return;
            }

            if (
                HeroManager.Enemies.Any(
                    enemy =>
                    !enemy.IsDead && !enemy.IsZombie && enemy.LSDistance(ObjectManager.Player) < e.Range
                    && enemy.LSIsValidTarget())
                || zedShadows.GetShadows()
                       .Any(
                           shadow =>
                           HeroManager.Enemies.Any(
                               enemy =>
                               !enemy.IsDead && !enemy.IsZombie && enemy.LSDistance(shadow) < e.Range
                               && enemy.LSIsValidTarget())))
            {
                e.Cast();
            }
        }
    }
}