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
        public OrbManager GetOrbs { get; }

        public Spells()
        {
            GetOrbs = new OrbManager();
            GetQ = new Spell(EloBuddy.SpellSlot.Q, 800);
            GetW = new Spell(EloBuddy.SpellSlot.W, 925);
            GetE = new Spell(SpellSlot.E, 700);
            GetR = new Spell(SpellSlot.R, 675);
            GetQ.SetSkillshot(0.6f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            GetW.SetSkillshot(0.25f, 140f, 1600f, false, SkillshotType.SkillshotCircle);
            GetE.SetSkillshot(0.25f, (float) (45*0.5), 2500f, false, SkillshotType.SkillshotCone);
        }

        public bool CastQ()
        {
            if (!GetQ.IsReady()) return false;
            var qTarget = TargetSelector.GetTarget(GetQ.Range, TargetSelector.DamageType.Magical);
            if (qTarget != null)
            {
                var predictQ=GetQ.GetPrediction(qTarget, true);
                if (predictQ.Hitchance >= HitChance.High)
                    return GetQ.Cast(predictQ.CastPosition);
            }
            return false;
        }
        public bool CastW()
        {
            if (!GetW.IsReady()) return false;
            var wTarget = TargetSelector.GetTarget(GetW.Range, TargetSelector.DamageType.Magical);
            if (wTarget == null) return false;
            if (HeroManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1 && GetW.IsReady())
            {
                var orb = GetOrbs.GetOrbToGrab((int) GetW.Range);
                if (orb == null) return false;
                GetW.Cast(orb);
            }
            else if (HeroManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 1 && GetW.IsReady())
            {
                if (GetOrbs.WObject(false) == null) return false;
                GetW.From = GetOrbs.WObject(false).ServerPosition;
                GetW.Cast(wTarget, true);
                return true;
            }
            return false;

        }

        public bool CastWToPos(Vector2 pos)
        {
            if (GetW.IsReady())
            {
                if (HeroManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1 && GetW.IsReady())
                {
                    var orb = GetOrbs.GetOrbToGrab((int) GetW.Range);
                    if (orb != null)
                        GetW.Cast(orb);
                }
                else if (HeroManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 1 && GetW.IsReady())
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

        public bool CalcE(Vector3 initialPoint , Vector3 finalPoint)
        {

            for (var i = 0; i <= 500; i += 10)
            {
                var result = initialPoint.Extend(finalPoint, i);
                if (result.GetEnemiesInRange(10)!=null) return true;

            }
            return false;
        }
        public bool CastE()
        {
            if (!GetE.IsReady()) return false;
            foreach (var orb in GetOrbs.GetOrbs())
            {
                foreach (var tar in HeroManager.Enemies)
                {
                    //500 extended range.
                    if (GetE.IsInRange(orb))
                    {
                        var finalBallPos = HeroManager.Player.Position.Extend(orb, 500);

                        if (CalcE(orb, finalBallPos))
                        {
                            GetE.Cast(orb);
                        }
                    }
                }
            }
            return false;

        }

        public bool CastR(SyndraCore core)
        {
            if (GetR.IsReady())
            {
                var rTarget = TargetSelector.GetTarget(GetR.Range, TargetSelector.DamageType.Magical);
                if (rTarget != null)
                {

                    if (CastRCheck(rTarget, core))
                        if (NotKilleableWithOtherSpells(rTarget))
                        {
                            var totalDamageR = RDamage(rTarget);
                            if (rTarget.Health <= totalDamageR)
                            {
                                GetR.Cast(rTarget);
                            }
                        }
                }
            }
            return false;
        }

        public float RDamage(AIHeroClient target)
        {
            float damagePerBall = (GetR.GetDamage(target)/3);
            float totalDamageR = GetR.GetDamage(target) + damagePerBall*GetOrbs.GetOrbs().Count;
            return totalDamageR;
        }

        public bool CastRCheck(AIHeroClient target, SyndraCore core)
        {
            var checkZhoniaMenu = core.GetMenu.GetMenu.Item("DONTRZHONYA").GetValue<bool>();
            if (checkZhoniaMenu)
            {
                //Zhonias lol
                const string zhonyaName = "ZhonyasHourglass";
                SpellSlot slot;
                for (var i = 1; i <= 6; i++)
                {
                    slot = core.Events.intToSpellSlot(i);
                    if (target.GetSpell(slot).Name == zhonyaName)
                    {
                        if (target.GetSpell(slot).IsReady()) return false;
                    }
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
                if (checkFirst)
                    if (target.ChampionName == tar.Name)
                    {
                        if (core.GetMenu.GetMenu.Item(target.ChampionName).GetValue<bool>())
                            return tar.CastRToDat();
                    }
            }
            return core.GetMenu.GetMenu.Item(target.ChampionName).GetValue<bool>();

        }

        private bool NotKilleableWithOtherSpells(AIHeroClient target)
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
                CastE();
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
