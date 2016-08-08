using EloBuddy; namespace KoreanZed
{
    using System.Linq;

    using KoreanZed.QueueActions;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    class ZedAntiGapCloser
    {
        private readonly ZedMenu zedMenu;

        private readonly ZedSpell w;

        private readonly ZedSpell e;

        private readonly ActionQueue actionQueue;

        private readonly ActionQueueList antiGapCloserList;

        private readonly ZedShadows shadows;

        private readonly AIHeroClient player;

        public ZedAntiGapCloser(ZedMenu menu, ZedSpells spells, ZedShadows shadows)
        {
            zedMenu = menu;
            w = spells.W;
            e = spells.E;
            this.shadows = shadows;
            player = ObjectManager.Player;

            actionQueue = new ActionQueue();
            antiGapCloserList = new ActionQueueList();

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            
        }

        private void Game_OnUpdate(System.EventArgs args)
        {
            actionQueue.ExecuteNextAction(antiGapCloserList);
            if (antiGapCloserList.Count == 0)
            {
                Game.OnUpdate -= Game_OnUpdate;
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (ObjectManager.Player.LSDistance(gapcloser.Sender.Position) > e.Range)
            {
                return;
            }

            if (zedMenu.GetParamBool("koreanzed.miscmenu.useeantigc") && e.LSIsReady())
            {
                e.Cast();
            }

            if (zedMenu.GetParamBool("koreanzed.miscmenu.usewantigc") && w.LSIsReady() && antiGapCloserList.Count == 0)
            {
                if (shadows.CanCast)
                {
                    actionQueue.EnqueueAction(
                        antiGapCloserList,
                        () => player.Mana > w.ManaCost && player.HealthPercent - 10 < gapcloser.Sender.HealthPercent,
                        () => shadows.Cast(Vector3.Negate(gapcloser.Sender.Position)),
                        () => true);
                    actionQueue.EnqueueAction(
                        antiGapCloserList,
                        () => w.Instance.ToggleState != 0,
                        () => shadows.Switch(),
                        () => !w.LSIsReady());
                    Game.OnUpdate += Game_OnUpdate;
                    return;
                }
                else if (!shadows.CanCast && shadows.CanSwitch)
                {
                    int champCount =
                        HeroManager.Enemies.Count(enemy => enemy.LSDistance(shadows.Instance.Position) < 1500F);

                    if ((player.HealthPercent > 80 && champCount <= 3)
                        || (player.HealthPercent > 40 && champCount <= 2)
                        )
                    {
                        shadows.Switch();
                    }
                   
                }
            }
        }
    }
}
