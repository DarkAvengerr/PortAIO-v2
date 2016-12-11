using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Manager.Spells
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using SharpDX;
    using LeagueSharp;
    using LeagueSharp.Common;
    using static Common.Common;
    using SpellData = Evade.EvadeTargetManager.SpellData;

    internal class SpellManager : Logic
    {
        public static readonly List<SpellData> Spells = new List<SpellData>();

        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 475f);
            Q3 = new Spell(SpellSlot.Q, 1000f);
            W = new Spell(SpellSlot.W, 400f);
            E = new Spell(SpellSlot.E, 475f);
            R = new Spell(SpellSlot.R, 1200f);

            Q.SetSkillshot(0.4f, 30f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q3.SetSkillshot(0.35f, 90f, 1200f, false, SkillshotType.SkillshotLine);

            Ignite = ObjectManager.Player.GetSpellSlot("summonerdot");
            Flash = ObjectManager.Player.GetSpellSlot("SummonerFlash");

            Spells.Add(
                new SpellData
                    {ChampionName = "Ahri", SpellNames = new[] {"ahrifoxfiremissiletwo"}, Slot = SpellSlot.W});
            Spells.Add(
                new SpellData
                { ChampionName = "Ahri", SpellNames = new[] { "ahritumblemissile" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { ChampionName = "Akali", SpellNames = new[] { "akalimota" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Anivia", SpellNames = new[] { "frostbite" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Annie", SpellNames = new[] { "disintegrate" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Brand",
                    SpellNames = new[] { "brandconflagrationmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Brand",
                    SpellNames = new[] { "brandwildfire", "brandwildfiremissile" },
                    Slot = SpellSlot.R
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Caitlyn",
                    SpellNames = new[] { "caitlynaceintheholemissile" },
                    Slot = SpellSlot.R
                });
            Spells.Add(
                new SpellData
                { ChampionName = "Cassiopeia", SpellNames = new[] { "cassiopeiatwinfang" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Elise", SpellNames = new[] { "elisehumanq" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ezreal",
                    SpellNames = new[] { "ezrealarcaneshiftmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "FiddleSticks",
                    SpellNames = new[] { "fiddlesticksdarkwind", "fiddlesticksdarkwindmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData { ChampionName = "Gangplank", SpellNames = new[] { "parley" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Janna", SpellNames = new[] { "sowthewind" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData { ChampionName = "Kassadin", SpellNames = new[] { "nulllance" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Katarina",
                    SpellNames = new[] { "katarinaq", "katarinaqmis" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData
                { ChampionName = "Kayle", SpellNames = new[] { "judicatorreckoning" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leblanc",
                    SpellNames = new[] { "leblancchaosorb", "leblancchaosorbm" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(new SpellData { ChampionName = "Lulu", SpellNames = new[] { "luluw" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                { ChampionName = "Malphite", SpellNames = new[] { "seismicshard" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "MissFortune",
                    SpellNames = new[] { "missfortunericochetshot", "missFortunershotextra" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nami",
                    SpellNames = new[] { "namiwenemy", "namiwmissileenemy" },
                    Slot = SpellSlot.W
                });
            Spells.Add(
                new SpellData { ChampionName = "Nunu", SpellNames = new[] { "iceblast" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Pantheon", SpellNames = new[] { "pantheonq" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ryze",
                    SpellNames = new[] { "spellflux", "spellfluxmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData { ChampionName = "Shaco", SpellNames = new[] { "twoshivpoison" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Shen", SpellNames = new[] { "shenvorpalstar" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Sona", SpellNames = new[] { "sonaqmissile" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Swain", SpellNames = new[] { "swaintorment" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Syndra", SpellNames = new[] { "syndrar" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { ChampionName = "Taric", SpellNames = new[] { "dazzle" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Teemo", SpellNames = new[] { "blindingdart" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                { ChampionName = "Tristana", SpellNames = new[] { "detonatingshot" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData
                { ChampionName = "TwistedFate", SpellNames = new[] { "bluecardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                { ChampionName = "TwistedFate", SpellNames = new[] { "goldcardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                { ChampionName = "TwistedFate", SpellNames = new[] { "redcardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Urgot",
                    SpellNames = new[] { "urgotheatseekinghomemissile" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData { ChampionName = "Vayne", SpellNames = new[] { "vaynecondemn" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData
                { ChampionName = "Veigar", SpellNames = new[] { "veigarprimordialburst" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData
                { ChampionName = "Viktor", SpellNames = new[] { "viktorpowertransfer" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Vladimir",
                    SpellNames = new[] { "vladimirtidesofbloodnuke" },
                    Slot = SpellSlot.E
                });
        }

        internal static bool HaveQ3 => Me.HasBuff("YasuoQ3W");

        internal static bool CanCastE(Obj_AI_Base target)
        {
            return !target.HasBuff("YasuoDashWrapper");
        }

        internal static void CastQ3()
        {
            //copy from valvesharp
            var targets = HeroManager.Enemies.Where(x => x.DistanceToPlayer() <= Q3.Range);
            var castPos = Vector3.Zero;

            if (!targets.Any())
            {
                return;
            }

            foreach (var pred in
                targets.Select(i => Q3.GetPrediction(i, true))
                    .Where(
                        i => i.Hitchance >= HitChance.VeryHigh ||
                             (i.Hitchance >= HitChance.High && i.AoeTargetsHitCount > 1))
                    .OrderByDescending(i => i.AoeTargetsHitCount))
            {
                castPos = pred.CastPosition;
                break;
            }

            if (castPos.IsValid())
            {
                Q3.Cast(castPos, true);
            }
        }

        internal static void EGapTarget(AIHeroClient target, bool UnderTurret, int GapcloserDis, bool includeChampion = true)
        {
            var dashtargets = new List<Obj_AI_Base>();
            dashtargets.AddRange(
                HeroManager.Enemies.Where(
                    x =>
                        !x.IsDead && (includeChampion || x.NetworkId != target.NetworkId) && x.IsValidTarget(E.Range) &&
                        CanCastE(x)));
            dashtargets.AddRange(
                MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(CanCastE));

            if (dashtargets.Any())
            {
                var dash = dashtargets.Where(x =>x.IsValidTarget(E.Range))
                    .OrderBy(x => target.Position.Distance(PosAfterE(x)))
                    .FirstOrDefault(x => Evade.EvadeManager.IsSafe(PosAfterE(x).To2D()).IsSafe);

                if (dash != null && dash.DistanceToPlayer() <= E.Range && CanCastE(dash) &&
                    target.DistanceToPlayer() >= GapcloserDis &&
                    target.Position.Distance(PosAfterE(dash)) <= target.DistanceToPlayer() && 
                    Me.IsFacing(dash) && (UnderTurret || !UnderTower(PosAfterE(dash))))
                {
                    E.CastOnUnit(dash, true);
                }
            }
        }

        internal static void EGapMouse(AIHeroClient target, bool UnderTurret, int GapcloserDis, bool includeChampion = true)
        {
            if (target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) * 1.2 ||
                target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(target) * 0.8 ||
                Game.CursorPos.DistanceToPlayer() >= Orbwalking.GetRealAutoAttackRange(Me) * 1.2)
            {
                var dashtargets = new List<Obj_AI_Base>();
                dashtargets.AddRange(
                    HeroManager.Enemies.Where(
                        x =>
                            !x.IsDead && (includeChampion || x.NetworkId != target.NetworkId) && x.IsValidTarget(E.Range) &&
                            CanCastE(x)));
                dashtargets.AddRange(
                    MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(CanCastE));

                if (dashtargets.Any())
                {
                    var dash =
                        dashtargets.Where(x => x.IsValidTarget(E.Range) && Evade.EvadeManager.IsSafe(PosAfterE(x).To2D()).IsSafe)
                            .MinOrDefault(x => PosAfterE(x).Distance(Game.CursorPos));

                    if (dash != null && dash.DistanceToPlayer() <= E.Range && CanCastE(dash) &&
                        target.DistanceToPlayer() >= GapcloserDis && Me.IsFacing(dash) &&
                        (UnderTurret || !UnderTower(PosAfterE(dash))))
                    {
                        E.CastOnUnit(dash, true);
                    }
                }
            }
        }

        internal static double GetQDmg(Obj_AI_Base target)
        {
            if (target == null)
            {
                return 0d;
            }

            var dmgItem = 0d;

            if (Items.HasItem(3057) && (Items.CanUseItem(3057) || Me.HasBuff("Sheen")))
            {
                dmgItem = Me.BaseAttackDamage;
            }

            if (Items.HasItem(3078) && (Items.CanUseItem(3078) || Me.HasBuff("Sheen")))
            {
                dmgItem = Me.BaseAttackDamage * 2;
            }

            var damageModifier = 1d;
            var reduction = 0d;
            var result = dmgItem
                         + Me.TotalAttackDamage * (Me.Crit >= 0.85f ? (Items.HasItem(3031) ? 1.875 : 1.5) : 1);

            if (Items.HasItem(3153))
            {
                var dmgBotrk = Math.Max(0.08 * target.Health, 10);
                result += target is Obj_AI_Minion ? Math.Min(dmgBotrk, 60) : dmgBotrk;
            }

            var targetHero = target as AIHeroClient;

            if (targetHero != null)
            {
                if (Items.HasItem(3047, targetHero))
                {
                    damageModifier *= 0.9d;
                }

                if (targetHero.ChampionName == "Fizz")
                {
                    reduction += 4 + (targetHero.Level - 1 / 3) * 2;
                }

                var mastery = targetHero.Masteries.FirstOrDefault(i => i.Page == MasteryPage.Defense && i.Id == 68);

                if (mastery != null && mastery.Points >= 1)
                {
                    reduction += 1 * mastery.Points;
                }
            }

            return Me.CalcDamage(
                       target,
                       Damage.DamageType.Physical,
                       20*Q.Level + (result - reduction)*damageModifier)
                   + (Me.GetBuffCount("ItemStatikShankCharge") == 100
                       ? Me.CalcDamage(
                           target,
                           Damage.DamageType.Magical,
                           100*(Me.Crit >= 0.85f ? (Items.HasItem(3031) ? 2.25 : 1.8) : 1))
                       : 0);
        }


        internal static double GetEDmg(Obj_AI_Base target)
        {
            if (target == null)
            {
                return 0d;
            }

            var stacksPassive = Me.Buffs.Find(b => b.DisplayName.Equals("YasuoDashScalar"));
            var Estacks = stacksPassive?.Count ?? 0;
            var damage = (E.Level * 20 + 50) * (1 + 0.25 * Estacks) + Me.FlatMagicDamageMod * 0.6;

            return Me.CalcDamage(target, Damage.DamageType.Magical, damage);
        }

        internal static void UseItems(Obj_AI_Base target, bool IsCombo = false)
        {
            if (IsCombo)
            {
                if (Items.HasItem(3153, Me) && Items.CanUseItem(3153) && Me.HealthPercent <= 80)
                {
                    Items.UseItem(3153, target);
                }

                if (Items.HasItem(3143, Me) && Items.CanUseItem(3143) && Me.Distance(target.Position) <= 400)
                {
                    Items.UseItem(3143);
                }

                if (Items.HasItem(3144, Me) && Items.CanUseItem(3144) && target.IsValidTarget(Q.Range))
                {
                    Items.UseItem(3144, target);
                }

                if (Items.HasItem(3142, Me) && Items.CanUseItem(3142) && Me.Distance(target.Position) <= Q.Range)
                {
                    Items.UseItem(3142);
                }
            }

            if (Items.HasItem(3074, Me) && Items.CanUseItem(3074) && Me.Distance(target.Position) <= 400)
            {
                Items.UseItem(3074);
            }

            if (Items.HasItem(3077, Me) && Items.CanUseItem(3077) && Me.Distance(target.Position) <= 400)
            {
                Items.UseItem(3077);
            }
        }
    }
}
