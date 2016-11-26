using EloBuddy; 
using LeagueSharp.Common; 
 namespace DevCommom2
{
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    public static class DevHelper
    {
        private static readonly string[] AttackResets =
        {
            "dariusnoxiantacticsonh", "fiorae", "garenq", "gravesmove",
            "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge",
            "leonashieldofdaybreak", "luciane", "monkeykingdoubleattack",
            "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze",
            "netherblade", "gangplankqwrapper", "powerfist",
            "renektonpreexecute", "rengarq", "shyvanadoubleattack",
            "sivirw", "takedown", "talonnoxiandiplomacy",
            "trundletrollsmash", "vaynetumble", "vie", "volibearq",
            "xenzhaocombotarget", "yorickspectral", "reksaiq",
            "itemtitanichydracleave", "masochism", "illaoiw",
            "elisespiderw", "fiorae", "meditate", "sejuaninorthernwinds",
            "asheq"
        };

        private static readonly string[] NoAttacks =
        {
            "volleyattack", "volleyattackwithsound",
            "jarvanivcataclysmattack", "monkeykingdoubleattack",
            "shyvanadoubleattack", "shyvanadoubleattackdragon",
            "zyragraspingplantattack", "zyragraspingplantattack2",
            "zyragraspingplantattackfire", "zyragraspingplantattack2fire",
            "viktorpowertransfer", "sivirwattackbounce", "asheqattacknoonhit",
            "elisespiderlingbasicattack", "heimertyellowbasicattack",
            "heimertyellowbasicattack2", "heimertbluebasicattack",
            "annietibbersbasicattack", "annietibbersbasicattack2",
            "yorickdecayedghoulbasicattack", "yorickravenousghoulbasicattack",
            "yorickspectralghoulbasicattack", "malzaharvoidlingbasicattack",
            "malzaharvoidlingbasicattack2", "malzaharvoidlingbasicattack3",
            "kindredwolfbasicattack"
        };

        private static readonly string[] Attacks =
        {
            "caitlynheadshotmissile", "frostarrow", "garenslash2",
            "kennenmegaproc", "masteryidoublestrike", "quinnwenhanced",
            "renektonexecute", "renektonsuperexecute",
            "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust",
            "xenzhaothrust2", "xenzhaothrust3", "viktorqbuff",
            "lucianpassiveshot"
        };

        public static List<AIHeroClient> GetEnemyList()
        {
            return ObjectManager.Get<AIHeroClient>()
                .Where(x => x.IsEnemy && x.IsValid)
                .OrderBy(x => ObjectManager.Player.ServerPosition.Distance(x.ServerPosition))
                .ToList();
        }

        public static List<AIHeroClient> GetAllyList()
        {
            return ObjectManager.Get<AIHeroClient>()
                .Where(x => x.IsAlly && x.IsValid)
                .OrderBy(x => ObjectManager.Player.ServerPosition.Distance(x.ServerPosition))
                .ToList();
        }

        public static AIHeroClient GetNearestEnemy(this Obj_AI_Base unit)
        {
            return ObjectManager.Get<AIHeroClient>()
                .Where(x => x.IsEnemy && x.IsValid && x.NetworkId != unit.NetworkId)
                .OrderBy(x => unit.ServerPosition.Distance(x.ServerPosition))
                .FirstOrDefault();
        }

        public static AIHeroClient GetNearestAlly(this Obj_AI_Base unit)
        {
            return ObjectManager.Get<AIHeroClient>()
                .Where(x => x.IsAlly && x.IsValid && x.NetworkId != unit.NetworkId)
                .OrderBy(x => unit.ServerPosition.Distance(x.ServerPosition))
                .FirstOrDefault();
        }

        public static AIHeroClient GetNearestEnemyFromUnit(this Obj_AI_Base unit)
        {
            return ObjectManager.Get<AIHeroClient>()
                .Where(x => x.IsEnemy && x.IsValid)
                .OrderBy(x => unit.ServerPosition.Distance(x.ServerPosition))
                .FirstOrDefault();
        }

        public static float GetHealthPerc(this Obj_AI_Base unit)
        {
            return unit.Health / unit.MaxHealth * 100;
        }

        public static float GetManaPerc(this Obj_AI_Base unit)
        {
            return unit.Mana / unit.MaxMana * 100;
        }

        public static bool IsUnderEnemyTurret(this Obj_AI_Base unit)
        {
            IEnumerable<Obj_AI_Turret> query;

            if (unit.IsEnemy)
            {
                query = ObjectManager.Get<Obj_AI_Turret>()
                    .Where(
                        x => x.IsAlly && x.IsValid && !x.IsDead && unit.ServerPosition.Distance(x.ServerPosition) < 950);
            }
            else
            {
                query = ObjectManager.Get<Obj_AI_Turret>()
                    .Where(
                        x => x.IsEnemy && x.IsValid && !x.IsDead && unit.ServerPosition.Distance(x.ServerPosition) < 950);
            }

            return query.Any();
        }

        public static bool IsKillable(this AIHeroClient source, Obj_AI_Base target, IEnumerable<SpellSlot> spellCombo)
        {
            return source.GetComboDamage(target, spellCombo) * 0.9 > target.Health;
        }

        public static int CountEnemyInPositionRange(Vector3 position, float range)
        {
            return GetEnemyList().Count(x => x.ServerPosition.Distance(position) <= range);
        }

        public static bool IsAutoAttack(string spellName)
        {
            return (spellName.ToLower().Contains("attack") && !NoAttacks.Contains(spellName.ToLower())) ||
                   Attacks.Contains(spellName.ToLower());
        }

        public static bool IsMinion(AttackableUnit unit, bool includeWards = false)
        {
            var objAiMinion = unit as Obj_AI_Minion;

            if (objAiMinion != null)
            {
                var minion = objAiMinion;
                var name = minion.BaseSkinName.ToLower();

                return name.Contains("minion") || (includeWards && (name.Contains("ward") || name.Contains("trinket")));
            }

            return false;
        }

        public static float GetRealDistance(GameObject unit, GameObject target)
        {
            return unit.Position.Distance(target.Position) + unit.BoundingRadius + target.BoundingRadius;
        }
    }
}
