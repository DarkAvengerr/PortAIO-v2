using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace VST_Auto_Carry_Standalone_Graves
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using System.Collections.Generic;

    internal class Graves
    {
        internal static AIHeroClient Me;
        internal static Menu Menu;
        internal static Spell Q, W, E, R;
        internal static float SearchERange;
        internal static List<BuffType> DebuffTypes = new List<BuffType>();
        internal static string[] AutoEnableList =
        {
             "Annie", "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana", "Evelynn", "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus",
             "Kassadin", "Katarina", "Kayle", "Kennen", "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna",
             "Ryze", "Sion", "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra", "Velkoz", "Azir", "Ekko",
             "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jayce", "Jinx", "KogMaw", "Lucian", "MasterYi", "MissFortune", "Quinn", "Shaco", "Sivir",
             "Talon", "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Yasuo", "Zed", "Kindred", "AurelionSol"
        };


        public Graves()
        {
            Me = GameObjects.Player;

            GravesSpell.Init();
            GravesMenu.Init();
            GravesEvents.Init();
        }

        internal static bool CanCaseE(Obj_AI_Base target, Vector3 Pos)
        {
            if (E.IsReady() && target.IsValidTarget(SearchERange) && !Me.IsUnderEnemyTurret())
            {
                var EndPos = Me.ServerPosition.Extend(Pos, E.Range);

                if (!EndPos.IsWall())
                {
                    if (EndPos.IsUnderEnemyTurret() && Menu["E"]["UnderTower"].GetValue<MenuBool>())
                    {
                        return false;
                    }

                    if (EndPos.CountEnemyHeroesInRange(E.Range) >= 3 && Me.HealthPercent >= 80)
                    {
                        return true;
                    }

                    if (EndPos.CountEnemyHeroesInRange(E.Range) < 3)
                    {
                        return true;
                    }

                    if (target.Distance(EndPos) < Me.GetRealAutoAttackRange())
                    {
                        return true;
                    }

                    if (!target.IsValidTarget(E.Range) && target.IsValidTarget(SearchERange) && Me.MoveSpeed > target.MoveSpeed)
                    {
                        return true;
                    }

                    // Need to search graves bullets calculate "gravesbasicattackammo1", "gravesbasicattackammo2"

                    if (!Me.HasBuff("gravesbasicattackammo2") && Me.HasBuff("gravesbasicattackammo1") && target.IsValidTarget(Me.GetRealAutoAttackRange()))
                    {
                        // this part have more question , so need to perfect it ........... path 6.10!!!!!!!
                        // i think can use BuffAdd and BuffMove Events
                        // 
                        return true;
                    }
                }
                else
                    return false;
            }

            return false;
        }

        internal static bool CanMove(AIHeroClient hero)
        {
            DebuffTypes.Add(BuffType.Blind);
            DebuffTypes.Add(BuffType.Charm);
            DebuffTypes.Add(BuffType.Fear);
            DebuffTypes.Add(BuffType.Flee);
            DebuffTypes.Add(BuffType.Stun);
            DebuffTypes.Add(BuffType.Snare);
            DebuffTypes.Add(BuffType.Taunt);
            DebuffTypes.Add(BuffType.Suppression);
            DebuffTypes.Add(BuffType.Polymorph);
            DebuffTypes.Add(BuffType.Blind);
            DebuffTypes.Add(BuffType.Silence);

            foreach (var buff in hero.Buffs)
            {
                if (DebuffTypes.Contains(buff.Type) && (buff.EndTime - Game.Time) * 1000 >= 600 && buff.IsActive)
                {
                    return false;
                }
            }

            return true;
        }

        internal static void AddBool(Menu mainMenu, string name, string displayName, bool value = true)
        {
            mainMenu.Add(new MenuBool(name, displayName, value));
        }

        internal static void AddSlider(Menu mainMenu, string name, string displayName, int value, int minValue = 0, int maxValue = 100)
        {
            mainMenu.Add(new MenuSlider(name, displayName, value, minValue, maxValue));
        }
    }
}
