using EloBuddy; 
using LeagueSharp.Common; 
namespace ElLeeSin.Components.SpellManagers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElLeeSin.Utilities;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class InsecManager
    {
        /// <summary>
        ///     
        /// </summary>
        public static InsecComboStepSelect insecComboStep;

        /// <summary>
        ///     Click Insec.
        /// </summary>
        public static bool ClicksecEnabled;

        /// <summary>
        ///     Insec click pos.
        /// </summary>
        public static Vector3 InsecClickPos;

        /// <summary>
        ///     Insec line pos.
        /// </summary>
        public static Vector2 InsecLinePos;

        /// <summary>
        ///     The ward range.
        /// </summary>
        public const int WardRange = 600;

        /// <summary>
        ///     The flash range.
        /// </summary>
        private const int FlashRange = 425;

        public static bool isNullInsecPos = true;

        private static Vector3 insecPos;

        public enum InsecComboStepSelect
        {
            None,

            Qgapclose,

            Wgapclose,

            Pressr
        }

        private static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return from.To2D() + distance * Vector3.Normalize(direction - from).To2D();
        }

        private static List<AIHeroClient> GetAllyHeroes(AIHeroClient position, int range)
        {
            return
                HeroManager.Allies.Where(hero => !hero.IsMe && !hero.IsDead && (hero.Distance(position) < range))
                    .ToList();
        }

        private static Vector3 InterceptionPoint(List<AIHeroClient> heroes)
        {
            var result = new Vector3();
            foreach (var hero in heroes)
            {
                result += hero.Position;
            }
            result.X /= heroes.Count;
            result.Y /= heroes.Count;
            return result;
        }


        private static List<AIHeroClient> GetAllyInsec(List<AIHeroClient> heroes)
        {
            byte alliesAround = 0;
            var tempObject = new AIHeroClient();
            foreach (var hero in heroes)
            {
                var localTemp =
                    GetAllyHeroes(hero, 750 + MyMenu.Menu.Item("bonusRangeA").GetValue<Slider>().Value).Count;
                if (localTemp > alliesAround)
                {
                    tempObject = hero;
                    alliesAround = (byte)localTemp;
                }
            }
            return GetAllyHeroes(tempObject, 750 + MyMenu.Menu.Item("bonusRangeA").GetValue<Slider>().Value);
        }

        public static Vector3 GetInsecPos(AIHeroClient target)
        {
            try
            {
                if (ClicksecEnabled && Misc.GetMenuItem("clickInsec"))
                {
                    InsecLinePos = Drawing.WorldToScreen(InsecClickPos);
                    return V2E(InsecClickPos, target.Position, target.Distance(InsecClickPos) + 230).To3D();
                }

                if (isNullInsecPos)
                {
                    isNullInsecPos = false;
                    insecPos = ObjectManager.Player.Position;
                }

                if ((GetAllyHeroes(target, 1500).Count > 0) && Misc.GetMenuItem("ElLeeSin.Insec.Ally"))
                {
                    var insecPosition = InterceptionPoint(GetAllyInsec(GetAllyHeroes(target, 2000)));

                    InsecLinePos = Drawing.WorldToScreen(insecPosition);
                    return V2E(insecPosition, target.Position, target.Distance(insecPosition) + 230).To3D();
                }

                if (Misc.GetMenuItem("ElLeeSin.Insec.Tower"))
                {
                    var tower =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .Where(
                                t =>
                                    t.IsVisible && t.Health > 0 && t.Distance(target) - 725 < 1100 && t.Distance(ObjectManager.Player) < 3000
                                    && t.IsAlly && !t.IsDead)
                            .MinOrDefault(t => t.Distance(target));

                    if (tower != null)
                    {
                        InsecLinePos = Drawing.WorldToScreen(tower.Position);
                        return V2E(tower.Position, target.Position, target.Distance(tower.Position) + 230).To3D();
                    }
                }

                if (Misc.GetMenuItem("ElLeeSin.Insec.Original.Pos"))
                {
                    InsecLinePos = Drawing.WorldToScreen(insecPos);
                    return V2E(insecPos, target.Position, target.Distance(insecPos) + 230).To3D();
                }
                return new Vector3();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
            return new Vector3();
        }

        public static void InsecCombo(AIHeroClient target)
        {
            if (target == null)
            {
                return;
            }

            if (ObjectManager.Player.Distance(GetInsecPos(target)) < 200)
            {
                insecComboStep = InsecComboStepSelect.Pressr;
            }
            else if ((insecComboStep == InsecComboStepSelect.None)
                     && (GetInsecPos(target).Distance(ObjectManager.Player.Position) < 600))
            {
                insecComboStep = InsecComboStepSelect.Wgapclose;
            }
            else if ((insecComboStep == InsecComboStepSelect.None)
                     && (target.Distance(ObjectManager.Player) < LeeSin.spells[LeeSin.Spells.Q].Range))
            {
                insecComboStep = InsecComboStepSelect.Qgapclose;
            }

            switch (insecComboStep)
            {
                case InsecComboStepSelect.Qgapclose:
                    if (Misc.IsQOne)
                    {
                        var pred1 = LeeSin.spells[LeeSin.Spells.Q].GetPrediction(target);
                        if (pred1.Hitchance >= HitChance.High)
                        {
                            LeeSin.CastQ(target, Misc.GetMenuItem("ElLeeSin.Smite.Q"));
                        }

                        if (!Misc.GetMenuItem("checkOthers2"))
                        {
                            return;
                        }

                        var insectObjects =
                            HeroManager.Enemies.Where(
                                    x =>
                                        x.IsValidTarget(LeeSin.spells[LeeSin.Spells.Q].Range) && !x.Compare(target)
                                        && (LeeSin.spells[LeeSin.Spells.Q].GetHealthPrediction(x)
                                            > LeeSin.spells[LeeSin.Spells.Q].GetDamage(x))
                                        && (x.Distance(target) < target.DistanceToPlayer()) && (x.Distance(target) < 750))
                                .Concat(
                                    MinionManager.GetMinions(
                                        ObjectManager.Player.ServerPosition,
                                        LeeSin.spells[LeeSin.Spells.Q].Range,
                                        MinionTypes.All,
                                        MinionTeam.NotAlly))
                                .Where(
                                    m =>
                                        m.IsValidTarget(LeeSin.spells[LeeSin.Spells.Q].Range)
                                        && LeeSin.spells[LeeSin.Spells.Q].GetHealthPrediction(m)
                                        > LeeSin.spells[LeeSin.Spells.Q].GetDamage(m) && m.Distance(target) < 400f)
                                .OrderBy(i => i.Distance(target))
                                .FirstOrDefault();

                        if (insectObjects == null)
                        {
                            return;
                        }

                        LeeSin.spells[LeeSin.Spells.Q].Cast(insectObjects);
                    }

                    if (!target.HasQBuff() && Misc.IsQOne)
                    {
                        LeeSin.CastQ(target, Misc.GetMenuItem("ElLeeSin.Smite.Q"));
                    }
                    else if (target.HasQBuff())
                    {
                        LeeSin.spells[LeeSin.Spells.Q].Cast();
                        insecComboStep = InsecComboStepSelect.Wgapclose;
                    }
                    else
                    {
                        if (LeeSin.spells[LeeSin.Spells.Q].Instance.Name.Equals(
                                "blindmonkqtwo",
                                StringComparison.InvariantCultureIgnoreCase)
                            && (Misc.ReturnQBuff()?.Distance(target) <= 600))
                        {
                            LeeSin.spells[LeeSin.Spells.Q].Cast();
                        }
                    }
                    break;

                case InsecComboStepSelect.Wgapclose:

                    if (ObjectManager.Player.Distance(target) < WardRange)
                    {
                        Wardmanager.WardJump(GetInsecPos(target), false, true, true);

                        if ((Wardmanager.FindBestWardItem() == null) && LeeSin.spells[LeeSin.Spells.R].IsReady()
                            && Misc.GetMenuItem("ElLeeSin.Flash.Insec")
                            && (ObjectManager.Player.Spellbook.CanUseSpell(LeeSin.flashSlot) == SpellState.Ready))
                        {
                            if (((GetInsecPos(target).Distance(ObjectManager.Player.Position) < FlashRange)
                                 && (Wardmanager.LastWard + 1000 < Environment.TickCount)) || !LeeSin.spells[LeeSin.Spells.W].IsReady())
                            {
                                ObjectManager.Player.Spellbook.CastSpell(LeeSin.flashSlot, GetInsecPos(target));
                            }
                        }
                    }
                    else if (ObjectManager.Player.Distance(target) < Misc.WardFlashRange)
                    {
                        Wardmanager.WardJump(target.Position);

                        if (LeeSin.spells[LeeSin.Spells.R].IsReady() && Misc.GetMenuItem("ElLeeSin.Flash.Insec")
                            && (ObjectManager.Player.Spellbook.CanUseSpell(LeeSin.flashSlot) == SpellState.Ready))
                        {
                            if (ObjectManager.Player.Distance(target) < FlashRange - 25)
                            {
                                if ((Wardmanager.FindBestWardItem() == null) || (Wardmanager.LastWard + 1000 < Environment.TickCount))
                                {
                                    ObjectManager.Player.Spellbook.CastSpell(LeeSin.flashSlot, GetInsecPos(target));
                                }
                            }
                        }
                    }
                    break;

                case InsecComboStepSelect.Pressr:
                    LeeSin.spells[LeeSin.Spells.R].CastOnUnit(target);
                    break;
            }
        }

    }
}
