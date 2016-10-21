using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Reforged_Riven
{
    internal class Core
    {
        public static int Qstack = 1;

        public const string IsFirstR = "RivenFengShuiEngine";
        public const string IsSecondR = "RivenIzunaBlade";
        public static void AttackMove(AttackableUnit x) => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, x); // NOTE: This is just for test!
        public static Orbwalker Orbwalker => Variables.Orbwalker;
        public static AIHeroClient Player => ObjectManager.Player;

    }

    public class Spells
    {
        public static SpellSlot Ignite { get; set; }
        public static SpellSlot Flash { get; set; }
        public static Spell Q { get; set; }
        public static Spell W { get; set; }
        public static Spell E { get; set; }
        public static Spell R { get; set; }

        public static void Load()
        {
            Q = new Spell(SpellSlot.Q, 260f);
            W = new Spell(SpellSlot.W, 250f);
            E = new Spell(SpellSlot.E, 270);
            R = new Spell(SpellSlot.R, 900);

            R.SetSkillshot(0.25f, (float)(45 * 0.5), 1600, false, SkillshotType.SkillshotCone);
            Q.SetSkillshot(0.25f, 100f, 2200f, false, SkillshotType.SkillshotCircle);

            Ignite = Core.Player.GetSpellSlot("SummonerDot");
            Flash = Core.Player.GetSpellSlot("summonerflash");
        }
    }
}
