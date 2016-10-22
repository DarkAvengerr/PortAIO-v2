using System;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using Damage = S_Plus_Class_Kalista.Libaries.Damage;
using EloBuddy;

namespace S_Plus_Class_Kalista.Drawing
{
    internal class DrawingOnMonsters : Core
    {
        private const string _MenuNameBase = ".Monster Menu";
        private const string _MenuItemBase = ".Monster.";

        public static Menu DrawingMonsterMenu()
        {
            var menu = new Menu(_MenuNameBase, "monsterMenu");
            var epicMenu = new Menu(".Epics", "epicMenu");
            epicMenu.AddItem(
                new MenuItem(_MenuItemBase + "Boolean.DrawOnEpics", "Draw Damage On epics(Dragon/Baron)").SetValue(true));
            epicMenu.AddItem(
                new MenuItem(_MenuItemBase + "Boolean.DrawOnEpics.InnerColor", "Inner Market Color").SetValue(
                    new Circle(true, Color.DeepSkyBlue)));
            epicMenu.AddItem(
                new MenuItem(_MenuItemBase + "Boolean.DrawOnEpics.OuterColor", "Outer Marker Color").SetValue(
                    new Circle(true, Color.Crimson)));

            var monsterMenu = new Menu(".NormalMonsters", "normalMonsterMenu");
            monsterMenu.AddItem( new MenuItem(_MenuItemBase + "Boolean.DrawOnMonsters", "Draw Damage On Monsters").SetValue(true));
            monsterMenu.AddItem(new MenuItem(_MenuItemBase + "Boolean.DrawOnMonsters.Fill", "Fill Damage Color").SetValue(
                    new Circle(true, Color.DarkGray)));

            monsterMenu.AddItem(new MenuItem(_MenuItemBase + "Boolean.DrawOnMonsters.NonFill", "NonFill Damage Color").SetValue(new Circle(false, Color.LightSlateGray)));

            menu.AddSubMenu(epicMenu);
            menu.AddSubMenu(monsterMenu);

            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.DrawOnMonsters.KillableColor", "Killable Text").SetValue( new Circle(true, Color.Red)));

            return menu;
        }

        private static LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnitDelegate _damageToMonster;

        public static LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnitDelegate DamageToMonster
        {
            get { return _damageToMonster; }

            set
            {
                if (_damageToMonster == null)
                {
                    EloBuddy.Drawing.OnDraw += OnDrawMonster;
                }
                _damageToMonster = value;
            }
        }

        public static void OnDrawMonster(EventArgs args)
        {

            foreach (var monster in ObjectManager.Get<Obj_AI_Minion>())
            {

                if (monster.Team != GameObjectTeam.Neutral || !monster.IsValidTarget(Champion.E.Range) ||
                    !monster.IsHPBarRendered)
                    continue;

                var rendDamage = Damage.DamageCalc.CalculateRendDamage(monster);

                if (rendDamage > 1)
                {

                    if (string.Equals(monster.CharData.BaseSkinName, "sru_dragon",
                        StringComparison.CurrentCultureIgnoreCase)
                        ||
                        string.Equals(monster.CharData.BaseSkinName, "sru_baron",
                            StringComparison.CurrentCultureIgnoreCase)
                            ||
                        string.Equals(monster.CharData.BaseSkinName, "sru_red",
                            StringComparison.CurrentCultureIgnoreCase)
                            ||
                        string.Equals(monster.CharData.BaseSkinName, "sru_blue",
                            StringComparison.CurrentCultureIgnoreCase))

                    {
                        if (!SMenu.Item(_MenuItemBase + "Boolean.DrawOnEpics").GetValue<bool>()) continue;

                        if (rendDamage > monster.Health) // Is killable
                        {
                            Render.Circle.DrawCircle(monster.Position, monster.BoundingRadius + 50,
                                SMenu.Item(_MenuItemBase + "Boolean.DrawOnEpics.KillableColor").GetValue<Circle>().Color,
                                2);
                        }

                        else // Not killable
                        {
                            Render.Circle.DrawCircle(monster.Position, monster.BoundingRadius + 50,
                                SMenu.Item(_MenuItemBase + "Boolean.DrawOnEpics.InnerColor").GetValue<Circle>().Color, 2);
                            var remainingHp = (int) 100*(monster.Health - Damage.DamageCalc.CalculateRendDamage(monster)) / monster.Health;      
                            Render.Circle.DrawCircle(monster.Position, monster.BoundingRadius + (float) remainingHp + 50,
                                SMenu.Item(_MenuItemBase + "Boolean.DrawOnEpics.OuterColor").GetValue<Circle>().Color, 2);
                        }

                        if (SMenu.Item(_MenuItemBase + "Boolean.DrawOnMonsters.KillableColor").GetValue<Circle>().Active)
                        {
                            if (rendDamage > monster.Health)
                                EloBuddy.Drawing.DrawText(monster.Position.X, monster.Position.Y + 100,
                                    SMenu.Item(_MenuItemBase + "Boolean.DrawOnMonsters.KillableColor")
                                        .GetValue<Circle>()
                                        .Color, "Killable");
                        }
                    }

                    else
                    {
                        foreach (var monsterBar in Structures.Monster.MonsterBarDictionary)
                        {
                            if (string.Equals(monster.CharData.BaseSkinName, monsterBar.Key,
                                StringComparison.CurrentCultureIgnoreCase))
                            {
                                var barPos = monster.HPBarPosition;
                                var percentHealthAfterDamage = Math.Max(0, monster.Health - rendDamage)/
                                                               monster.MaxHealth;
                                var yPos = barPos.Y + monsterBar.Value.YOffsetBegin;
                                var xPosDamage = barPos.X + monsterBar.Value.XOffset +
                                                 monsterBar.Value.BarWidth*percentHealthAfterDamage;
                                var xPosCurrentHp = barPos.X + monsterBar.Value.XOffset +
                                                    monsterBar.Value.BarWidth*monster.Health/monster.MaxHealth;

                                if (SMenu.Item(_MenuItemBase + "Boolean.DrawOnMonsters.Fill").GetValue<Circle>().Active)
                                {
                                    var differenceInHp = xPosCurrentHp - xPosDamage;
                                    var pos1 = barPos.X + monsterBar.Value.XOffset;
                                    for (var i = 0; i < differenceInHp; i++)
                                    {
                                        EloBuddy.Drawing.DrawLine(pos1 + i, yPos, pos1 + i,
                                            yPos + monsterBar.Value.YOffsetEnd, 1,
                                            SMenu.Item(_MenuItemBase + "Boolean.DrawOnMonsters.Fill")
                                                .GetValue<Circle>()
                                                .Color);
                                    }
                                }
                                else
                                    EloBuddy.Drawing.DrawLine(xPosDamage, yPos, xPosDamage,
                                        yPos + monsterBar.Value.YOffsetEnd, 1,
                                        SMenu.Item(_MenuItemBase + "Boolean.DrawOnMonsters.NonFill")
                                            .GetValue<Circle>()
                                            .Color);


                                if (
                                    SMenu.Item(_MenuItemBase + "Boolean.DrawOnMonsters.KillableColor")
                                        .GetValue<Circle>()
                                        .Active)
                                {
                                    if (rendDamage > monster.Health)
                                        EloBuddy.Drawing.DrawText(
                                            monster.HPBarPosition.X + monsterBar.Value.XOffset, monster.HPBarPosition.Y,
                                            SMenu.Item(_MenuItemBase + "Boolean.DrawOnMonsters.KillableColor")
                                                .GetValue<Circle>()
                                                .Color, "Killable");
                                }
                            }
                        }
                    }
                }

            }
        }


    }
}