#region copyrights

//  Copyright 2016 Marvin Piekarek
//  CardShot.cs is part of RARETwistedFate.
//  RARETwistedFate is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//  RARETwistedFate is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//  You should have received a copy of the GNU General Public License
//  along with RARETwistedFate. If not, see <http://www.gnu.org/licenses/>.

#endregion

#region usages

using System;
using System.Linq;
using System.Runtime.InteropServices;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.MoreLinq;
using HitChance = LeagueSharp.SDK.Enumerations.HitChance;
using SkillshotType = LeagueSharp.SDK.Enumerations.SkillshotType;
using Spell = LeagueSharp.SDK.Spell;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace RARETwistedFate.TwistedFate
{
    internal class CardShot
    {
        public Spell SpellQ;

        public CardShot()
        {
            SpellQ = new Spell(SpellSlot.Q, 1450);
            SpellQ.SetSkillshot(250, 50, 1000, true, SkillshotType.SkillshotLine);
            SpellQ.MinHitChance = HitChance.Medium;
            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (MenuTwisted.MainMenu["Q"]["ImmoQ"])
            {
                CheckForImmobiles();
            }
        }

        public void CheckForImmobiles()
        {
            var heros = ObjectManager.Get<AIHeroClient>().Where(h => h.IsEnemy && h.Distance(GameObjects.Player) <= SpellQ.Range);

            foreach (var objAiHero in heros)
            {
                if ((objAiHero.HasBuffOfType(BuffType.Charm) || objAiHero.HasBuffOfType(BuffType.Stun) || objAiHero.HasBuffOfType(BuffType.Knockup) ||
                    objAiHero.HasBuffOfType(BuffType.Snare) ) && Extensions.IsValidTarget(objAiHero, SpellQ.Range))
                {
                    Console.WriteLine("Cast on immo");
                    var pred = SpellQ.GetPrediction(objAiHero).CastPosition;
                    SpellQ.Cast(pred);
                }
            }

        }

        public void HandleQ(OrbwalkingMode orbMode)
        {
            if (orbMode == OrbwalkingMode.Combo && IsOn(orbMode))
            {
                var heroes = Variables.TargetSelector.GetTargets(SpellQ.Range, DamageType.Magical, false);
                foreach (var hero in heroes)
                {
                    if (SpellQ.IsReady() && SpellQ.IsInRange(hero))
                    {
                        var pred = SpellQ.GetPrediction(hero).CastPosition;
                        SpellQ.Cast(pred);
                    }
                }
                    
            }
            else if ((orbMode == OrbwalkingMode.Hybrid || orbMode == OrbwalkingMode.LaneClear) && IsOn(orbMode))
            {
                var minions = GameObjects.EnemyMinions.Where(m => SpellQ.IsInRange(m)).ToList();
                var farmloc = SpellQ.GetLineFarmLocation(minions);

                var minionsN = GameObjects.Jungle.Where(m => SpellQ.IsInRange(m)).ToList();
                var farmlocN = SpellQ.GetLineFarmLocation(minionsN);

                if (farmloc.MinionsHit >= 3)
                {
                    SpellQ.Cast(farmloc.Position);
                }

                if (farmlocN.MinionsHit >= 1)
                {
                    SpellQ.Cast(farmlocN.Position);
                }

            }
            else if (orbMode == OrbwalkingMode.LastHit && IsOn(orbMode))
            {
                var minions = GameObjects.EnemyMinions.Where(m => SpellQ.IsInRange(m)).ToList();
                var farmloc = SpellQ.GetLineFarmLocation(minions);

                if (farmloc.MinionsHit >= 3)
                {
                    SpellQ.Cast(farmloc.Position);
                }
            }
        }

        public bool IsOn(OrbwalkingMode orbMode)
        {
            switch (orbMode)
            {
                case OrbwalkingMode.Combo:
                    return MenuTwisted.MainMenu["Q"]["ComboQ"];
                case OrbwalkingMode.LastHit:
                    return MenuTwisted.MainMenu["Q"]["LastQ"];
                case OrbwalkingMode.Hybrid:
                    return MenuTwisted.MainMenu["Q"]["HybridQ"];
                case OrbwalkingMode.LaneClear:
                    return MenuTwisted.MainMenu["Q"]["FarmQ"];
            }

            return false;
        }
    }
}