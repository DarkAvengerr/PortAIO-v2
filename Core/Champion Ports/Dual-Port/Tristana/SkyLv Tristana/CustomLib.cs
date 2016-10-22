using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    using SDK = LeagueSharp.SDK;
    using LeagueSharp.SDK.TSModes;

    public static class CustomLib
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Tristana.Player;
            }
        }
        #endregion

        public static float EnemyMinionInMinionRange(Obj_AI_Minion Minion, float Range)
        {
            return ObjectManager.Get<Obj_AI_Minion>().Where(m => m.Team != ObjectManager.Player.Team && m.Distance(Minion) <= Range && !m.IsDead).Count();
        }

        public static float EnemyMinionInPlayerRange(float Range)
        {
            return ObjectManager.Get<Obj_AI_Minion>().Where(m => m.Team != ObjectManager.Player.Team && m.Distance(Player) <= Range && !m.IsDead).Count();
        }

        internal static AIHeroClient GetTarget
        {
            get
            {
                AIHeroClient target = null;
                var t = TargetSelector.GetSelectedTarget();
                if (t.IsValidTarget())
                {
                    target = t;
                }
                
                return target;
            }
        }

        #region Insec

        public static int RPushDistance()
        {
            if (SkyLv_Tristana.R.Level == 1)
            {
                return 600;
            }

            if (SkyLv_Tristana.R.Level == 2)
            {
                return 800;
            }

            if (SkyLv_Tristana.R.Level == 3)
            {
                return 1000;
            }
            return 0;
        }

        public static Vector3 GetBehindPosition(AIHeroClient target)
        {
            return target.ServerPosition.Extend(GetPushPosition(target), -(300));
        }

        public static Vector3 GetBehindPositionExtend(AIHeroClient target)
        {
            return target.ServerPosition.Extend(GetPushPosition(target), -(1000));
        }

        public static Vector3 GetPushPosition(AIHeroClient target)
        {
            var pos = Player.ServerPosition;

            switch (SkyLv_Tristana.Menu.Item("Tristana.InsecMode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    var turret =
                        SDK.GameObjects.AllyTurrets.Where(
                            i =>
                            !i.IsDead && target.Distance(i) <= RPushDistance() + 500
                            && i.Distance(target) - RPushDistance() <= 950 && i.Distance(target) > 400)
                            .MinOrDefault(i => i.Distance(Player));
                    if (turret != null)
                    {
                        pos = turret.ServerPosition;
                    }
                    else
                    {
                        var hero =
                            SDK.GameObjects.AllyHeroes.Where(
                                i =>
                                i.IsValidTarget(RPushDistance() + 700, false, target.ServerPosition) && !i.IsMe
                                && i.HealthPercent > 10 && i.Distance(target) > 350)
                                .MaxOrDefault(i => new Priority().GetDefaultPriority(i));
                        if (hero != null)
                        {
                            pos = hero.ServerPosition;
                        }
                    }
                    break;
                case 1:
                    pos = Game.CursorPos;
                    break;
            }
            return pos;
        }
        #endregion
    }
}
