using System.Collections.Generic;
using LeagueSharp;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Camille
{
    enum EvadeType
    {
        Target,
        SkillshotLine,
        SkillshotCirce,
        SelfCast
    }

    class Evadeable
    {
        public string SDataName;
        public EvadeType EvadeType;
        public string ChampionName;
        public SpellSlot Slot;

        public Evadeable(string name, EvadeType type, string championName)
        {
            this.SDataName = name;
            this.EvadeType = type;
            this.ChampionName = championName;
        }

        internal static Dictionary<string, Evadeable> DangerList = new Dictionary<string, Evadeable>
        {
            {
                "infernalguardian",
                new Evadeable("infernalguardian", EvadeType.SkillshotCirce, "Annie")
            },
            {
                "curseofthesadmummy",
                new Evadeable("curseofthesadmummy", EvadeType.SelfCast, "Amumu")},
            {
                "enchantedcystalarrow",
                new Evadeable("enchantedcystalarrow", EvadeType.SkillshotLine, "Ashe")
            },
            {
                "aurelionsolr",
                new Evadeable("aurelionsolr", EvadeType.SkillshotLine, "AurelionSol")
            },
            {
                "azirr",
                new Evadeable("azirr", EvadeType.SkillshotLine, "Azir")
            },
            {
                "cassiopeiar",
                new Evadeable("cassiopeiar", EvadeType.SkillshotCirce, "Cassiopeia")
            },
            {
                "feast",
                new Evadeable("feast", EvadeType.Target, "Chogath")
            },
            {
                "dariusexecute",
                new Evadeable("dariusexecute", EvadeType.Target, "Darius")
            },
            {
                "evelynnr",
                new Evadeable("evelynnr", EvadeType.SkillshotCirce, "Evelynn")
            },
            {
                "galioidolofdurand",
                new Evadeable("galioidolofdurand", EvadeType.SelfCast, "Galio")
            },
            {
                "gnarult",
                new Evadeable("gnarult", EvadeType.SelfCast, "Gnar")
            },
            {
                "garenr",
                new Evadeable("garenr", EvadeType.Target, "Garen")
            },
            {
                "gravesr",
                new Evadeable("gravesr", EvadeType.SkillshotLine, "Graves")
            },
            {
                "hecarimult",
                new Evadeable("hecarimult", EvadeType.SkillshotLine, "Hecarim")
            },
            {
                "illaoir",
                new Evadeable("illaoir", EvadeType.SelfCast, "Illaoi")
            },
            {
                "jarvanivcataclysm",
                new Evadeable("jarvanivcataclysm", EvadeType.Target, "JarvanIV")
            },
            {
                "blindmonkrkick",
                new Evadeable("blindmonkrkick", EvadeType.Target, "LeeSin")
            },
            {
                "lissandrar",
                new Evadeable("lissandrar", EvadeType.Target, "Lissandra")
            },
            {
                "ufslash",
                new Evadeable("ufslash", EvadeType.SkillshotCirce, "Malphite")
            },
            {
                "monkeykingspintowin",
                new Evadeable("monkeykingspintowin", EvadeType.SelfCast, "MonkeyKing")
            },
            {
                "rivenizunablade",
                new Evadeable("rivenizunablade", EvadeType.SkillshotLine, "Riven")
            },
            {
                "sejuaniglacialprisoncast",
                new Evadeable("sejuaniglacialprisoncast", EvadeType.SkillshotLine, "Sejuani")
            },
            {
                "shyvanatransformcast",
                new Evadeable("shyvanatrasformcast", EvadeType.SkillshotLine, "Shyvana")
            },
            {
                "sonar",
                new Evadeable("sonar", EvadeType.SkillshotLine, "Sona")
            },
            {
                "syndrar",
                new Evadeable("syndrar", EvadeType.Target, "Syndra")
            },
            {
                "varusr",
                new Evadeable("varusr", EvadeType.SkillshotLine, "Varus")
            },
            {
                "veigarprimordialburst",
                new Evadeable("veigarprimordialburst", EvadeType.Target, "Veigar")
            },
            {
                "viktorchaosstorm",
                new Evadeable("viktorchaosstorm", EvadeType.SkillshotCirce, "Viktor")
            },
        };
    }
}