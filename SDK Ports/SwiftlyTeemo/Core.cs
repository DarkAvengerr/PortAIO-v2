#region

using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Swiftly_Teemo
{
    internal class Core
    {
        public static Orbwalker Orbwalker => Variables.Orbwalker;
        public static AIHeroClient Target => Variables.TargetSelector.GetTarget(Player.GetRealAutoAttackRange(), DamageType.Physical);
        public static AIHeroClient Player => ObjectManager.Player;
       // public static bool NoxiousTrap => Target.Buffs.Any(x => x.Name.ToLower().Contains("Noxious Trap"));
    }

    public class Spells
    {
        public static SpellSlot Ignite;
        public static Spell Q { get; set; }
        public static Spell W { get; set; }
        public static Spell E { get; set; }
        public static Spell R { get; set; }

        public static void Load()
        {
            Q = new Spell(SpellSlot.Q, 680);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 300);

            Q.SetTargetted(0.5f, 1500f);
            R.SetSkillshot(0.5f, 120f, 1000f, false, SkillshotType.SkillshotCircle);

            Ignite = GameObjects.Player.GetSpellSlot("SummonerDot");
        }
    }
}
