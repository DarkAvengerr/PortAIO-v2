using EloBuddy; namespace KoreanZed
{
    using System;

    using LeagueSharp;

    class ZedFlee
    {
        private readonly ZedMenu zedMenu;

        private readonly ZedShadows zedShadows;

        public ZedFlee(ZedMenu zedMenu, ZedShadows zedShadows)
        {
            this.zedMenu = zedMenu;
            this.zedShadows = zedShadows;

            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (zedMenu.GetParamKeyBind("koreanzed.miscmenu.flee"))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                zedShadows.Cast(Game.CursorPos);
                zedShadows.Switch();
            }
        }
    }
}
