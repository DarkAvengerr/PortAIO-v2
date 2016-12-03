using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
using LeagueSharp.Common; 
namespace DarkMage
{
    public class Spells
    {
        public Spell GetQ { get; }
        public Spell GetW { get; }
        public Spell GetE { get; }
        public Spell GetR { get; }
        public Spell EQ { get; }
        public Spell Eany { get; }
        public OrbManager GetOrbs { get; }

        public Spells()
        {
            GetOrbs = new OrbManager();
            GetQ = new Spell(EloBuddy.SpellSlot.Q, 800);
            GetW = new Spell(EloBuddy.SpellSlot.W, 925);
            GetE = new Spell(SpellSlot.E, 700);
            GetR = new Spell(SpellSlot.R, 675);
            EQ = new Spell(SpellSlot.Q, GetQ.Range + 450);
            Eany = new Spell(SpellSlot.Q, GetQ.Range + 450);
            GetQ.SetSkillshot(0.6f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            GetW.SetSkillshot(0.25f, 140f, 1600f, false, SkillshotType.SkillshotCircle);
            GetE.SetSkillshot(0.25f, (float) (45*0.5), 2500f, false, SkillshotType.SkillshotCone);
            EQ.SetSkillshot(0.6f, 100f, 2500f, false, SkillshotType.SkillshotLine);
            Eany.SetSkillshot(0.30f, 50f, 2500f, false, SkillshotType.SkillshotLine);
        }

        public bool CastQ()
        {
            if (!GetQ.IsReady()) return false;
            var qTarget = TargetSelector.GetTarget(GetQ.Range, TargetSelector.DamageType.Magical);
            if (qTarget != null)
            {
                var predictQ = GetQ.GetPrediction(qTarget, true);
                if (predictQ.Hitchance >= HitChance.VeryHigh)
                    return GetQ.Cast(predictQ.CastPosition);
            }
            return false;
        }

        private bool EQcastNow;

        public void CastQE(Obj_AI_Base target)
        {

            if (GetE.IsReady())
            {
            //    var prediction = Prediction.GetPrediction(target, 500);
                var predictionInput = new PredictionInput
                {
                    Aoe = false,
                    Collision = EQ.Collision,
                    Speed = EQ.Speed,
                    Delay = EQ.Delay,
                    Range = EQ.Range,
                    From = HeroManager.Player.ServerPosition,
                    Radius = EQ.Width,
                    Unit = target,
                    Type = SkillshotType.SkillshotLine
                };
          var prediction = Prediction.GetPrediction(predictionInput);


                Vector3 castQpos = prediction.CastPosition;

                if (HeroManager.Player.Distance(castQpos) > GetQ.Range)
                    castQpos = HeroManager.Player.Position.Extend(castQpos, GetE.Range);


                if (prediction.Hitchance >= HitChance.VeryHigh)
                {
                    EQcastNow = true;
                    GetQ.Cast(castQpos);
                }
            }
        }

        public void TryBallE(AIHeroClient t)
        {
            if(!castE(t))
            if (GetQ.IsReady())
            {
                CastQE(t);
            }

            
        }

        public bool castE(AIHeroClient t)
        {
            if (!GetE.IsReady()) return false;
            var ePred = Eany.GetPrediction(t);
            if (ePred.Hitchance >= HitChance.VeryHigh)
            {
                var playerToCP = HeroManager.Player.Distance(ePred.CastPosition);
                foreach (var pos in GetOrbs.GetOrbs())
                {
                    if (HeroManager.Player.Distance(pos) < GetE.Range)
                    {
                        var ballFinalPos = HeroManager.Player.ServerPosition.Extend(pos, playerToCP);
                        if (ballFinalPos.Distance(ePred.CastPosition) < 50)
                        {
                            GetE.Cast(pos);

                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool CastW()
        {
            if (!GetW.IsReady()) return false;
            var wTarget = TargetSelector.GetTarget(GetW.Range, TargetSelector.DamageType.Magical);
            if (wTarget == null) return false;
            if(GetW.IsInRange(wTarget))
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState ==1&& GetW.IsReady())
            {
                var orb = GetOrbs.GetOrbToGrab((int) GetW.Range);
                GetW.Cast(orb);
            }
            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 1 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 1 && GetW.IsReady())
            {
                if (GetW.IsInRange(wTarget))
                {
                        if (GetOrbs.WObject(false) == null) return false;
                    GetW.From = GetOrbs.WObject(false).ServerPosition;

                        var predictW = GetQ.GetPrediction(wTarget, true);
                        if (predictW.Hitchance >= HitChance.VeryHigh)
                            GetW.Cast(predictW.CastPosition, true);
                    return true;
                }
            }
            return false;

        }
        public bool CastWToPos(Vector2 pos)
        {
            if (GetW.IsReady())
            {
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1 && GetW.IsReady())
                {
                    var orb = GetOrbs.GetOrbToGrab((int) GetW.Range);
                    GetW.Cast(orb);
                }
                else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 1 && GetW.IsReady())
                {
                    if (GetOrbs.WObject(false) != null)
                    {
                        GetW.From = GetOrbs.WObject(false).ServerPosition;
                        GetW.Cast(pos);
                        return true;
                    }
                }
            }
            return false;
        }
        public bool CalcE(Vector3 initialPoint , Vector3 finalPoint,AIHeroClient hero)
        {

            /*  for (var i = 25; i <= 500; i += 10)
              {
                  var result = initialPoint.Extend(finalPoint, i);

                  if (hero.Distance(result)<=50) return true;
              }*/
            var ePred = GetE.GetPrediction(hero);
            if (ePred.Hitchance < HitChance.High) return false;
            var pt = HeroManager.Player.Distance(ePred.CastPosition);
               
            var ballFinalPos = HeroManager.Player.ServerPosition.Extend(initialPoint, pt);
            if (ballFinalPos.Distance(ePred.CastPosition) < 50)
                return true;
            return false;
        }
        public bool CastE(SyndraCore core)
        {
            if (!GetE.IsReady()) return false;
            if (GetW.IsReady()) return false;
            if (GetOrbs.WObject(false) != null) return false;
            for (var index = 0; index < core.GetOrbs.Count; index++)
            {
                foreach (AIHeroClient tar in HeroManager.Enemies)
                {
                    if (!(tar.Distance(core.Hero) <= GetE.Range)) continue;
                    var orb = core.GetOrbs[index];
                    if (orb.IsValid())
                        if (!GetE.IsInRange(orb)) continue;
                    //500 extended range. 
                    var finalBallPos = HeroManager.Player.Position.Extend(orb, 500);

                    if (CalcE(orb, finalBallPos, tar))
                    {
                        GetE.Cast(orb);
                    }
                }

            }
            return false;

        }

        public bool CastR(SyndraCore core)
        {
            if (!GetR.IsReady()) return false;

            var rTarget = TargetSelector.GetTarget(GetR.Range, TargetSelector.DamageType.Magical);

            if (rTarget == null) return false;
            if (!CastRCheck(rTarget, core)) return false;
            if (!NotKilleableWithOtherSpells(rTarget,core)) return false;

            var totalDamageR = RDamage(rTarget,core);
            if (rTarget.Health <= totalDamageR)
            {
                GetR.Cast(rTarget);
            }
            return false;
        }

        public float RDamage(AIHeroClient target,SyndraCore core)
        {
            var damagePerBall = (GetR.GetDamage(target)/3);
            var totalDamageR = GetR.GetDamage(target) + damagePerBall*core.GetOrbs.Count;
            return totalDamageR;
        }
        public float RDamage(AIHeroClient target,int NSpeheres)
        {
            float damagePerBall = (GetR.GetDamage(target) / 3);
            float totalDamageR = GetR.GetDamage(target) + damagePerBall * NSpeheres;
            return totalDamageR;
        }
        public bool CastRCheck(AIHeroClient target, SyndraCore core)
        {
            var checkZhoniaMenu = core.GetMenu.GetMenu.Item("DONTRZHONYA").GetValue<bool>();
            if (checkZhoniaMenu)
            {
                //Zhonias lol
                const string zhonyaName = "ZhonyasHourglass";
                for (var i = 1; i <= 6; i++)
                {
                    var slot = core.Events.intToSpellSlot(i);
                    if (target.GetSpell(slot).Name != zhonyaName) continue;
                    if (target.GetSpell(slot).IsReady()) return false;
                }
            }
            if (target.IsInvulnerable)
            {
                return false;
            }
            foreach (var tar in core.championsWithDodgeSpells)
            {
                var tarslo = tar.SpellSlot;
                var result = tar.Name + "-" + SpellSlotToString(tarslo);
                var checkFirst = core.GetMenu.GetMenu.Item(result).GetValue<bool>();
                if (!checkFirst) continue;
                if (target.ChampionName != tar.Name) continue;
                if (core.GetMenu.GetMenu.Item(target.ChampionName).GetValue<bool>())
                    return tar.CastRToDat();
            }
            return core.GetMenu.GetMenu.Item(target.ChampionName).GetValue<bool>();

        }

        private bool NotKilleableWithOtherSpells(AIHeroClient target,SyndraCore core)
        {
            if (GetQ.IsReady() && GetQ.IsKillable(target))
            {
                CastQ();
                return false;
            }
            if (GetW.IsReady() && GetW.IsKillable(target))
            {
                CastW();
                return false;
            }
            if (GetE.IsReady() && GetE.IsKillable(target))
            {
                CastE(core);
                return false;
            }
            return true;
        }

        public string SpellSlotToString(SpellSlot s)
        {
            switch (s)
            {
                case SpellSlot.Q:
                    return "Q";
                case SpellSlot.W:
                    return "W";
                case SpellSlot.E:
                    return "E";
                case SpellSlot.R:
                    return "R";
                case SpellSlot.Unknown:
                    break;
                case SpellSlot.Summoner1:
                    break;
                case SpellSlot.Summoner2:
                    break;
                case SpellSlot.Item1:
                    break;
                case SpellSlot.Item2:
                    break;
                case SpellSlot.Item3:
                    break;
                case SpellSlot.Item4:
                    break;
                case SpellSlot.Item5:
                    break;
                case SpellSlot.Item6:
                    break;
                case SpellSlot.Trinket:
                    break;
                case SpellSlot.Recall:
                    break;
                case SpellSlot.OathSworn:
                    break;
                case SpellSlot.CapturePoint:
                    break;
                default:
                    break;
            }
            return "None";
        }
    }
}
