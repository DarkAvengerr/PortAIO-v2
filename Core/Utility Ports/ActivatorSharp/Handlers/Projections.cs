using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using Activator.Base;
using Activator.Data;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Handlers
{
    public class HPInstance
    {
        public int Id;
        public string Name;
        public int Decay;
        public float PredictedDmg;
        public AIHeroClient TargetHero;
        public Obj_AI_Base Attacker;
        public HitType HitType = HitType.None;
        public Gamedata Data;
    }

    public class Projections
    {
        internal static int Id;
        internal static AIHeroClient Player => ObjectManager.Player;
        internal static Dictionary<int, HPInstance> IncomeDamage = new Dictionary<int, HPInstance>();

        internal delegate void OnPredictDamageHanlder();
        internal static event OnPredictDamageHanlder OnPredictDamage;

        public static void Init()
        {
            GameObject.OnCreate += MissileClient_OnSpellMissileCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnUnitSpellCast;
            Obj_AI_Base.OnPlayAnimation += AIHeroClient_OnPlayAnimation;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnStealth;
        }

        public static void PredictTheDamage(Obj_AI_Base sender, Base.Champion hero, Gamedata data, HitType dmgType,
             string notes = null, float dmgEntry = 0f, int expiry = 500)
        {
            var hpred = new HPInstance();
            hpred.HitType = dmgType;
            hpred.TargetHero = hero.Player;
            hpred.Data = data;
            hpred.Name = string.Empty;

            if (!string.IsNullOrEmpty(data?.SDataName))
            {
                hpred.Name = data.SDataName;
            }

            if (sender != null)
            {
                hpred.Attacker = sender;
            }

            if (dmgEntry == 0f && sender != null)
            {
                switch (dmgType)
                {
                    case HitType.AutoAttack:                
                        hpred.PredictedDmg = (float) sender.GetAutoAttackDamage(hero.Player, true);
                        break;
                    case HitType.MinionAttack:
                    case HitType.TurretAttack:
                        hpred.PredictedDmg =
                            (float)
                                Math.Max(
                                    sender.CalcDamage(hero.Player, Damage.DamageType.Physical,
                                        sender.BaseAttackDamage + sender.FlatPhysicalDamageMod), 0);
                        break;
                    default:
                        if (!string.IsNullOrEmpty(data?.SDataName))
                            hpred.PredictedDmg = (float) Math.Max(0, sender.GetSpellDamage(hero.Player, data.SDataName));
                        break;
                }
            }
            else
            {
                var idmg = dmgEntry;
                hpred.PredictedDmg = (float) Math.Round(idmg);
            }

            if (hpred.PredictedDmg > 0)
            {
                var idmg = hpred.PredictedDmg * Activator.Origin.Item("weightdmg").GetValue<Slider>().Value / 100;
                hpred.PredictedDmg = (float) Math.Round(idmg);
            }
            else
            {
                var idmg = (hero.Player.Health / hero.Player.MaxHealth) * 5;            
                hpred.PredictedDmg = (float) Math.Round(idmg);
            }

            if (dmgType != HitType.Buff && dmgType != HitType.Troy)
            {
                // check duplicates (missiles and process spell)
                if (IncomeDamage.Select(entry => entry.Value).Any(o => o.Name == data.SDataName))
                {
                    return;
                }
            }

            var dmg = AddDamage(hpred, hero, notes);
            var extendedEndtime = Activator.Origin.Item("lagtolerance").GetValue<Slider>().Value * 10;
            LeagueSharp.Common.Utility.DelayAction.Add(expiry + extendedEndtime, () => RemoveDamage(dmg));
        }

        public static int AddDamage(HPInstance hpi, Base.Champion hero, string notes)
        {
            Id++;
            var id = Id;

            var aiHero = Activator.Allies().Find(x => x.Player.NetworkId == hero.Player.NetworkId);
            if (aiHero != null && !IncomeDamage.ContainsKey(id))
            {
                bool checkmenu = false;

                switch (hpi.HitType)
                {
                    case HitType.Spell:
                        aiHero.AbilityDamage += hpi.PredictedDmg;
                        aiHero.HitTypes.Add(HitType.Spell);
                        checkmenu = true;
                        break;
                    case HitType.Buff:
                        aiHero.BuffDamage += hpi.PredictedDmg;
                        aiHero.HitTypes.Add(HitType.Buff);
                        break;
                    case HitType.Troy:
                        aiHero.TroyDamage += hpi.PredictedDmg;
                        aiHero.HitTypes.Add(HitType.Troy);
                        checkmenu = true;
                        break;
                    case HitType.Item:
                        aiHero.ItemDamage += hpi.PredictedDmg;
                        aiHero.HitTypes.Add(HitType.Spell);
                        break;
                    case HitType.TurretAttack:
                        aiHero.TowerDamage += hpi.PredictedDmg;
                        aiHero.HitTypes.Add(HitType.TurretAttack);
                        break;
                    case HitType.MinionAttack:
                        aiHero.MinionDamage += hpi.PredictedDmg;
                        aiHero.HitTypes.Add(HitType.MinionAttack);
                        break;
                    case HitType.AutoAttack:
                        aiHero.AbilityDamage += hpi.PredictedDmg;
                        aiHero.HitTypes.Add(HitType.AutoAttack);
                        break;
                    case HitType.Stealth:
                        aiHero.HitTypes.Add(HitType.Stealth);
                        checkmenu = true;
                        break;
                }

                if (checkmenu && !string.IsNullOrEmpty(hpi.Name)) // QWER Only
                {
                    // add spell flags
                    hero.HitTypes.AddRange(
                        Lists.MenuTypes.Where(
                            x => Activator.Origin.Item(
                                hpi.Name.ToLower() + x.ToString().ToLower()).GetValue<bool>()));
                }

                if (hpi.HitType == HitType.Stealth)
                    hpi.PredictedDmg = 0;

                if (Activator.Origin.Item("acdebug").GetValue<bool>())
                {
                    Console.WriteLine(hpi.TargetHero.ChampionName + " [added]: " + hpi.Name + " - " 
                        + hpi.PredictedDmg + " / " + hpi.HitType + " / " + notes);
                }

                if (hero.Player.IsValidTarget(float.MaxValue, false))
                {
                    hpi.Id = id;
                    OnPredictDamage?.Invoke();
                    IncomeDamage.Add(id, hpi);
                }
            }

            return id;
        }

        public static void RemoveDamage(int id)
        {
            var entry = IncomeDamage.Find(x => x.Key == id);
            if (IncomeDamage.ContainsKey(entry.Key))
            {
                var hpi = entry.Value;
                var aiHero = Activator.Allies().Find(x => x.Player.NetworkId == hpi.TargetHero.NetworkId);
                if (aiHero != null)
                {
                    bool checkmenu = false;

                    switch (hpi.HitType)
                    {
                        case HitType.Spell:
                            aiHero.AbilityDamage -= hpi.PredictedDmg;
                            aiHero.HitTypes.Remove(HitType.Spell);
                            checkmenu = true;
                            break;
                        case HitType.Buff:
                            aiHero.BuffDamage -= hpi.PredictedDmg;
                            aiHero.HitTypes.Remove(HitType.Buff);
                            break;
                        case HitType.Troy:
                            aiHero.TroyDamage -= hpi.PredictedDmg;
                            aiHero.HitTypes.Remove(HitType.Troy);
                            checkmenu = true;
                            break;
                        case HitType.Item:
                            aiHero.ItemDamage -= hpi.PredictedDmg;
                            aiHero.HitTypes.Remove(HitType.Item);
                            break;
                        case HitType.TurretAttack:
                            aiHero.TowerDamage -= hpi.PredictedDmg;
                            aiHero.HitTypes.Remove(HitType.TurretAttack);
                            break;
                        case HitType.MinionAttack:
                            aiHero.MinionDamage -= hpi.PredictedDmg;
                            aiHero.HitTypes.Remove(HitType.MinionAttack);
                            break;
                        case HitType.AutoAttack:
                            aiHero.AbilityDamage -= hpi.PredictedDmg;
                            aiHero.HitTypes.Remove(HitType.AutoAttack);
                            break;
                        case HitType.Stealth:
                            aiHero.HitTypes.Remove(HitType.Stealth);
                            checkmenu = true;
                            break;
                    }

                    if (checkmenu && !string.IsNullOrEmpty(hpi.Name)) // QWER Only
                    {
                        // remove spell flags
                        aiHero.HitTypes.RemoveAll(
                            x =>
                                !x.Equals(HitType.Spell) &&
                                Activator.Origin.Item(hpi.Name + x.ToString().ToLower())
                                    .GetValue<bool>());
                    }

                    if (Activator.Origin.Item("acdebug").GetValue<bool>())
                    {
                        Console.WriteLine(hpi.TargetHero.ChampionName + " [removed]: " + hpi.Name + " - "
                                          + hpi.PredictedDmg + " / " + hpi.HitType);
                    }

                    IncomeDamage.Remove(id);
                }
                else
                {
                    var nullHero = Activator.Heroes.FirstOrDefault(x => x.Player.NetworkId == hpi.TargetHero.NetworkId);
                    if (nullHero != null)
                    {
                        Helpers.ResetIncomeDamage(nullHero);
                    }
                }
            }
        }

        private static void MissileClient_OnSpellMissileCreate(GameObject sender, EventArgs args)
        {
            #region FoW / Missile

            if (!sender.IsValid<MissileClient>())
            {
                return;
            }

            var missile = (MissileClient) sender;
            if (missile.SpellCaster is AIHeroClient && missile.SpellCaster?.Team != Player.Team)
            {
                var startPos = missile.StartPosition.To2D();
                var endPos = missile.EndPosition.To2D();

                var data = Gamedata.GetByMissileName(missile.SData.Name.ToLower());
                if (data == null)
                {
                    return;
                }

                // set line width
                if (data.Radius == 0f)
                    data.Radius = missile.SData.LineWidth;

                var direction = (endPos - startPos).Normalized();

                if (startPos.Distance(endPos) > data.CastRange)
                    endPos = startPos + direction * data.CastRange;

                if (startPos.Distance(endPos) < data.CastRange && data.FixedRange)
                    endPos = startPos + direction * data.CastRange;

                foreach (var hero in Activator.Allies())
                {
                    var distance = (1000 * (startPos.Distance(hero.Player.ServerPosition) / data.MissileSpeed));
                    var endtime = -100 + Game.Ping / 2 + distance;

                    // setup projection
                    var proj = hero.Player.ServerPosition.To2D().ProjectOn(startPos, endPos);
                    var projdist = hero.Player.ServerPosition.To2D().Distance(proj.SegmentPoint);

                    // get the evade time 
                    var evadetime = (int) (1000 * 
                       (data.Radius - projdist + hero.Player.BoundingRadius) / hero.Player.MoveSpeed);

                    // check if hero on segment
                    if (proj.IsOnSegment && projdist <= data.Radius + hero.Player.BoundingRadius + 35)
                    {
                        if (data.CastRange > 10000)
                        {
                            // ignore if can evade
                            if (hero.Player.NetworkId == Player.NetworkId)
                            {
                                if (evadetime < endtime)
                                {
                                    // check next player
                                    continue;
                                }
                            }
                        }

                        if (Activator.Origin.Item(data.SDataName + "predict").GetValue<bool>())
                        {
                            PredictTheDamage(missile.SpellCaster, hero, data, HitType.Spell, "missile.OnCreate", 0f, (int) endtime);
                        }
                    }
                }
            }

            #endregion
        }

        private static void Obj_AI_Base_OnUnitSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var aiHero = sender as AIHeroClient;
            if (aiHero != null && Activator.Origin.Item("dumpdata").GetValue<bool>())
            {
                var clientdata = new Gamedata
                {
                    SDataName = args.SData.Name.ToLower(),
                    ChampionName = aiHero.CharData.BaseSkinName.ToLower(),
                    Slot = args.Slot,
                    Radius = args.SData.LineWidth > 0
                        ? args.SData.LineWidth : (args.SData.CastRadiusSecondary > 0 
                            ? args.SData.CastRadiusSecondary : args.SData.CastRadius),
                    CastRange = args.SData.CastRange,
                    Delay = 250f,
                    MissileSpeed = (int) args.SData.MissileSpeed
                };

                Helpers.ExportSpellData(clientdata, args.SData.TargettingType.ToString().ToLower());
            }

            if (Helpers.IsEpicMinion(sender) || Helpers.IsCrab(sender))
            {
                return;
            }

            #region Hero

            if (sender.IsEnemy && sender is AIHeroClient)
            {
                var attacker = (AIHeroClient) sender;
                if (attacker.IsValid<AIHeroClient>())
                {
                    foreach (var hero in Activator.Allies())
                    {
                        #region auto attack

                        if (args.SData.Name.ToLower().Contains("attack") && args.Target != null)
                        {
                            if (args.Target.NetworkId == hero.Player.NetworkId)
                            {
                                float dmg = 0;

                                dmg += (int) Math.Max(attacker.GetAutoAttackDamage(hero.Player, true), 0);

                                if (attacker.HasBuff("sheen"))
                                    dmg += (int) Math.Max(attacker.GetAutoAttackDamage(hero.Player, true) +
                                                          attacker.GetCustomDamage("sheen", hero.Player), 0);

                                if (attacker.HasBuff("lichbane"))
                                    dmg += (int) Math.Max(attacker.GetAutoAttackDamage(hero.Player, true) +
                                                          attacker.GetCustomDamage("lichbane", hero.Player), 0);

                                if (attacker.HasBuff("itemstatikshankcharge") &&
                                    attacker.GetBuff("itemstatikshankcharge").Count == 100)
                                    dmg += new[] { 62, 120, 200 }[attacker.Level / 6];

                                if (args.SData.Name.ToLower().Contains("crit"))
                                    dmg += (int) Math.Max(attacker.GetAutoAttackDamage(hero.Player, true), 0);

                                PredictTheDamage(attacker, hero, new Gamedata(), HitType.AutoAttack, "enemy.AutoAttack", dmg);
                            }
                        }

                        #endregion
                        
                        var data = Gamedata.CachedSpells.Find(x => x.SDataName.ToLower() == args.SData.Name.ToLower());
                        if (data == null)
                        {
                            continue;
                        }

                        #region self/selfaoe

                        if (args.SData.TargettingType == SpellDataTargetType.Self ||
                            args.SData.TargettingType == SpellDataTargetType.SelfAoe)
                        {

                            if (data.Radius == 0f)
                                data.Radius = args.SData.CastRadiusSecondary != 0 
                                    ? args.SData.CastRadiusSecondary : args.SData.CastRadius;

                            GameObject fromobj = null;
                            if (data.FromObject != null)
                            {
                                fromobj =
                                    ObjectManager.Get<GameObject>()
                                        .FirstOrDefault(
                                            x =>
                                                data.FromObject != null && x.Name.ToLower().Contains("red") &&
                                                data.FromObject.Any(y => x.Name.Contains(y)));
                            }

                            var correctpos = fromobj?.Position ?? attacker.ServerPosition;
                            if (hero.Player.Distance(correctpos) <= data.CastRange + 125)
                            {
                                if (data.SDataName == "kalistaexpungewrapper" && 
                                    !hero.Player.HasBuff("kalistaexpungemarker"))
                                    continue;

                                if (Activator.Origin.Item(data.SDataName + "predict").GetValue<bool>())
                                {
                                    PredictTheDamage(attacker, hero, data, HitType.Spell, "enemy.SelfAoE");
                                }
                            }
                        }

                        #endregion

                        #region skillshot

                        if (args.SData.TargettingType == SpellDataTargetType.Cone ||
                            args.SData.TargettingType.ToString().Contains("Location"))
                        {
                            GameObject fromobj = null;
                            if (data.FromObject != null)
                            {
                                fromobj =
                                    ObjectManager.Get<GameObject>()
                                        .FirstOrDefault(
                                            x =>
                                                data.FromObject != null && x.Name.ToLower().Contains("red") &&
                                                data.FromObject.Any(y => x.Name.Contains(y)));
                            }

                            var isline = args.SData.TargettingType == SpellDataTargetType.Cone ||
                                         args.SData.LineWidth > 0;

                            if (!(args.SData.LineWidth > 0) && data.Radius == 0f)
                            {
                                data.Radius = args.SData.CastRadiusSecondary != 0
                                    ? args.SData.CastRadiusSecondary : args.SData.CastRadius;
                            }

                            var startpos = fromobj?.Position ?? attacker.ServerPosition;

                            if (hero.Player.Distance(startpos) > data.CastRange + 35)
                                continue;

                            if ((data.SDataName == "azirq" || data.SDataName == "azire") && fromobj == null)
                                continue;

                            var distance = (int) (1000 * (startpos.Distance(hero.Player.ServerPosition) / data.MissileSpeed));
                            var endtime = data.Delay + distance - Game.Ping/2f;

                            var iscone = args.SData.TargettingType == SpellDataTargetType.Cone;
                            var direction = (args.End.To2D() - startpos.To2D()).Normalized();
                            var endpos = startpos.To2D() + direction * startpos.To2D().Distance(args.End.To2D());

                            if (startpos.To2D().Distance(endpos) > data.CastRange)
                                endpos = startpos.To2D() + direction * data.CastRange;

                            if (startpos.To2D().Distance(endpos) < data.CastRange && data.FixedRange)
                                endpos = startpos.To2D() + direction * data.CastRange;

                            var proj = hero.Player.ServerPosition.To2D().ProjectOn(startpos.To2D(), endpos);
                            var projdist = hero.Player.ServerPosition.To2D().Distance(proj.SegmentPoint);

                            int evadetime = 0;

                            if (isline)
                                evadetime =
                                    (int) (1000 * (data.Radius - projdist + hero.Player.BoundingRadius) / hero.Player.MoveSpeed);

                            if (!isline || iscone)
                                 evadetime =
                                     (int) (1000 * (data.Radius - hero.Player.Distance(startpos) + hero.Player.BoundingRadius) / hero.Player.MoveSpeed);

                            if (proj.IsOnSegment && projdist <= data.Radius + hero.Player.BoundingRadius + 35 && isline ||
                               (iscone || !isline) && hero.Player.Distance(endpos) <= data.Radius + hero.Player.BoundingRadius + 35)
                            {
                                if (data.CastRange > 10000 && hero.Player.NetworkId == Player.NetworkId)
                                {
                                    if (evadetime < endtime)
                                    {
                                        continue;
                                    }
                                }

                                if (Activator.Origin.Item(data.SDataName + "predict").GetValue<bool>())
                                {
                                    PredictTheDamage(attacker, hero, data, HitType.Spell, "enemy.Skillshot", 0f, (int) endtime);
                                }
                            }
                        }

                        #endregion

                        #region unit type

                        if (args.SData.TargettingType == SpellDataTargetType.Unit ||
                            args.SData.TargettingType == SpellDataTargetType.SelfAndUnit)
                        {
                            if (args.Target == null || args.Target.Type != GameObjectType.AIHeroClient)
                                continue;

                            // check if is targeteting the hero on our table
                            if (hero.Player.NetworkId != args.Target.NetworkId)
                                continue;

                            // target spell dectection
                            if (hero.Player.Distance(attacker.ServerPosition) > data.CastRange + 100)
                                continue;

                            var distance =
                                (int) (1000 * (attacker.Distance(hero.Player.ServerPosition) / data.MissileSpeed));

                            var endtime = data.Delay + distance - Game.Ping/2f;

                            if (Activator.Origin.Item(data.SDataName + "predict").GetValue<bool>())
                            {
                                PredictTheDamage(attacker, hero, data, HitType.Spell, "enemy.TargetSpell", 0f, (int) endtime);
                            }
                        }

                        #endregion
                    }
                }
            }

            #endregion

            #region Turret

            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Turret && args.Target.Type == Player.Type)
            {
                var turret = sender as Obj_AI_Turret;
                if (turret != null && turret.IsValid<Obj_AI_Turret>())
                {
                    foreach (var hero in Activator.Allies())
                    {
                        if (args.Target.NetworkId == hero.Player.NetworkId)
                        {
                            if (turret.Distance(hero.Player.ServerPosition) <= 900 &&
                                Player.Distance(hero.Player.ServerPosition) <= 1000)
                            {
                                PredictTheDamage(turret, hero, new Gamedata(), HitType.TurretAttack, "enemy.Turret");
                            }                         
                        }
                    }
                }
            }

            #endregion

            #region Minion

            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Minion && args.Target.Type == Player.Type)
            {
                var minion = sender as Obj_AI_Minion;
                if (minion != null && minion.IsValid<Obj_AI_Minion>())
                {
                    foreach (var hero in Activator.Allies())
                    {
                        if (hero.Player.NetworkId == args.Target.NetworkId)
                        {
                            if (hero.Player.Distance(minion.ServerPosition) <= 750 &&
                                Player.Distance(hero.Player.ServerPosition) <= 1000)
                            {
                                PredictTheDamage(minion, hero, new Gamedata(), HitType.MinionAttack, "enemy.Minion");
                            }
                        }
                    }
                }
            }

            #endregion

            #region Gangplank Barrel

            if (sender.IsEnemy && sender.Type == GameObjectType.AIHeroClient)
            {
                var attacker = sender as AIHeroClient;
                if (attacker.ChampionName == "Gangplank" && attacker.IsValid<AIHeroClient>())
                {
                    foreach (var hero in Activator.Allies())
                    {
                        List<Obj_AI_Minion> gplist = new List<Obj_AI_Minion>();

                        gplist.AddRange(ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                x =>
                                    x.CharData.BaseSkinName == "gangplankbarrel" &&
                                    x.Position.Distance(x.Position) <= 375 && x.IsHPBarRendered)
                            .OrderBy(y => y.Position.Distance(hero.Player.ServerPosition)));

                        foreach (var obj in gplist)
                        {
                            if (hero.Player.Distance(obj.Position) <= 375 && args.Target.Name == "Barrel")
                            {
                                var dmg = (float) Math.Abs(attacker.GetAutoAttackDamage(hero.Player, true) * 1.2 + 150);
                                if (args.SData.Name.ToLower().Contains("crit"))
                                {
                                    dmg = dmg * 2;
                                }

                                PredictTheDamage(aiHero, hero, new Gamedata(), HitType.Spell, "enemy.GankplankBarrel", dmg);
                            }
                        }
                    }
                }
            }

            #endregion      

            #region Items

            if (sender.IsEnemy && sender.Type == GameObjectType.AIHeroClient)
            {
                var attacker = sender as AIHeroClient;
                if (attacker != null && attacker.IsValid<AIHeroClient>())
                {
                    if (args.SData.TargettingType == SpellDataTargetType.Unit)
                    {
                        foreach (var hero in Activator.Allies())
                        {
                            if (args.Target.NetworkId != hero.Player.NetworkId)
                                continue;
  
                            if (args.SData.Name.ToLower() == "bilgewatercutlass")
                            {
                                var dmg = (float) attacker.GetItemDamage(hero.Player, Damage.DamageItems.Bilgewater);
                                PredictTheDamage(attacker, hero, new Gamedata(), HitType.Item, "enemy.ItemCast", dmg);
                            }

                            if (args.SData.Name.ToLower() == "itemswordoffeastandfamine")
                            {
                                var dmg = (float) attacker.GetItemDamage(hero.Player, Damage.DamageItems.Botrk);
                                PredictTheDamage(attacker, hero, new Gamedata(), HitType.Item, "enemy.ItemCast", dmg);
                            }

                            if (args.SData.Name.ToLower() == "hextechgunblade")
                            {
                                var dmg = (float) attacker.GetItemDamage(hero.Player, Damage.DamageItems.Hexgun);
                                PredictTheDamage(attacker, hero, new Gamedata(), HitType.Item, "enemy.ItemCast", dmg);
                            }
                        }
                    }

                    if (args.SData.TargettingType == SpellDataTargetType.SelfAoe)
                    {
                        foreach (var hero in Activator.Allies())
                        {
                            if (args.SData.Name.ToLower() == "itemtiamatcleave")
                            {
                                if (attacker.Distance(hero.Player.ServerPosition) <= 375)
                                {
                                    var dmg = (float) attacker.GetItemDamage(hero.Player, Damage.DamageItems.Tiamat);
                                    PredictTheDamage(attacker, hero, new Gamedata(), HitType.Item, "enemy.ItemCast", dmg);
                                }
                            }
                        }
                    }

                    if (args.SData.TargettingType.ToString().Contains("Location"))
                    {
                        
                    }
                }
            }

            #endregion

            #region LucianQ

            if (sender.IsEnemy && args.SData.Name.ToLower() == "lucianq") 
            {
                var data = Gamedata.CachedSpells.Find(x => x.SDataName.ToLower() == "lucianq");
                if (data != null)
                {
                    foreach (var hero in Activator.Allies())
                    {
                        var delay = ((350 - Game.Ping) / 1000f);

                        var herodir = (hero.Player.ServerPosition - hero.Player.Position).Normalized();
                        var expectedpos = args.Target.Position + herodir * hero.Player.MoveSpeed * (delay);

                        if (args.Start.Distance(expectedpos) < 1100)
                            expectedpos = args.Target.Position +
                                          (args.Target.Position - sender.ServerPosition).Normalized() * 800;

                        var proj = hero.Player.ServerPosition.To2D().ProjectOn(args.Start.To2D(), expectedpos.To2D());
                        var projdist = hero.Player.ServerPosition.To2D().Distance(proj.SegmentPoint);

                        if (Activator.Origin.Item(data.SDataName + "predict").GetValue<bool>())
                        {
                            if (100 + hero.Player.BoundingRadius > projdist)
                            {
                                PredictTheDamage(sender, hero, data, HitType.Spell, "enemy.LucianQ");
                            }
                        }
                    }
                }
            }

            #endregion    
        }

        private static void AIHeroClient_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!(sender is AIHeroClient))
                return;

            var aiHero = (AIHeroClient) sender;

            #region Jax

            var data = Gamedata.CachedSpells.Find(x => x.SDataName.ToLower() == "jaxcounterstrike");
            if (data != null)
            {
                if (aiHero.ChampionName == "Jax" && aiHero.IsEnemy)
                {
                    if (args.Animation == "Spell3")
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 100, () =>
                        {
                            if (aiHero.HasBuff("JaxCounterStrike"))
                            {
                                var buff = aiHero.GetBuff("JaxCounterStrike");
                                var time = (int) ((buff.EndTime - buff.StartTime) * 1000);

                                foreach (var hero in Activator.Allies())
                                {
                                    if (aiHero.Distance(hero.Player) <= 250)
                                    {
                                        PredictTheDamage(aiHero, hero, data, HitType.Spell, "enemy.JaxE", 0f, time);
                                    }
                                }
                            }
                        });
                    }
                }
            }

            #endregion

            // more bro science soon ;)
        }

        private static void Obj_AI_Base_OnStealth(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            #region Stealth

            var attacker = sender as AIHeroClient;
            if (attacker == null || attacker.IsAlly || !attacker.IsValid<AIHeroClient>())
            {
                return;
            }

            foreach (var hero in Activator.Heroes.Where(h => h.Player.Distance(attacker) <= 1000))
            {
                foreach (var entry in Gamedata.CachedSpells.Where(s => s.HitTypes.Contains(HitType.Stealth)))
                {
                    if (entry.SDataName.ToLower() == args.SData.Name.ToLower())
                    {
                        PredictTheDamage(sender, hero, new Gamedata(), HitType.Stealth, "process.OnStealth");
                        break;
                    }
                }
            }

            #endregion
        }
    }
}