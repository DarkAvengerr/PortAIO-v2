// This file is part of LeagueSharp.Common.
// 
// LeagueSharp.Common is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Common is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Common.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

// ReSharper disable ObjectCreationAsStatement

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NidaleeTheBestialHuntress
{
    public static class Program
    {
        private const string ChampionName = "Nidalee";
        private static readonly List<Spell> HumanSpellList = new List<Spell>();
        private static readonly List<Spell> CougarSpellList = new List<Spell>();
        public static Menu MainMenu;
        private static Orbwalking.Orbwalker _orbwalker;
        private static HealManager _healManager;
        private static ManaManager _manaManager;
        private static AIHeroClient _player;
        private static Spell _javelinToss, _takedown, _bushwhack, _pounce, _primalSurge, _swipe, _aspectOfTheCougar;
        private static Vector3? _fleeTargetPosition;

        private static readonly string[] JungleMinions =
        {
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "SRU_Baron",
            "SRU_Dragon", "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp"
        };

        #region hitchance

        private static HitChance CustomHitChance
        {
            get { return GetHitchance(); }
        }

        #endregion

        #region OnDraw

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in HumanSpellList)
            {
                var circleEntry = MainMenu.Item("drawRange" + spell.Slot).GetValue<Circle>();
                if (circleEntry.Active && !_player.IsCougar() && !_player.IsDead)
                {
                    Render.Circle.DrawCircle(_player.Position, spell.Range, circleEntry.Color);
                }
            }

            foreach (var spell in CougarSpellList)
            {
                var circleEntry = MainMenu.Item("drawRange" + spell.Slot).GetValue<Circle>();
                if (circleEntry.Active && _player.IsCougar() && !_player.IsDead)
                {
                    Render.Circle.DrawCircle(_player.Position, spell.Range, circleEntry.Color);
                }
            }

            Circle damageCircle = MainMenu.Item("drawDamage").GetValue<Circle>();

            DamageIndicator.DrawingColor = damageCircle.Color;
            DamageIndicator.Enabled = damageCircle.Active;
        }

        #endregion

        #region OnGameUpdate

        private static void Game_OnGameUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(_javelinToss.Range, TargetSelector.DamageType.Magical);

            Killsteal();
            OnImmobile();
            ProcessCooldowns();

            if (MainMenu.Item("useCombo").GetValue<KeyBind>().Active)
            {
                OnCombo(target);
            }

            if (MainMenu.Item("useHarass").GetValue<KeyBind>().Active)
            {
                OnHarass(target);
            }

            if (MainMenu.Item("useWC").GetValue<KeyBind>().Active)
            {
                WaveClear();
            }

            if (MainMenu.Item("useJC").GetValue<KeyBind>().Active)
            {
                JungleClear();
            }

            if (MainMenu.Item("useFlee").GetValue<KeyBind>().Active)
            {
                Flee();
            }
        }

        #endregion

        #region Combo

        private static void OnCombo(AIHeroClient target)
        {
            var pounceDistance = target.IsHunted() ? 730 : _pounce.Range;
            if (_player.IsCougar())
            {
                /*if (_menu.Item("useTakedown").GetValue<bool>() && _takedown.IsReady() &&
                    _player.Distance(target.Position) <= _takedown.Range)
                {
                    _takedown.Cast(true);
                }*/

                if (_pounce.IsReady() && MainMenu.Item("usePounce").GetValue<bool>())
                {
                    if (MainMenu.Item("turretSafety").GetValue<bool>() && target.UnderTurret(true))
                    {
                        return;
                    }

                    if (MainMenu.Item("pounceHunted").GetValue<bool>())
                    {
                        if (target.IsHunted() && _player.Distance(target.ServerPosition) <= pounceDistance &&
                            _player.Distance(target) > _swipe.Range)
                        {
                            _pounce.Cast(target.ServerPosition);
                        }
                        if (!target.IsHunted() && _player.GetSpellDamage(target, SpellSlot.W) > target.Health + 20 &&
                            _player.Distance(target.ServerPosition) <= pounceDistance)
                        {
                            _pounce.Cast(target.ServerPosition);
                        }
                    }
                    else
                    {
                        if (_player.Distance(target) <= pounceDistance && _player.Distance(target) > _swipe.Range)
                        {
                            _pounce.Cast(target.ServerPosition);
                        }
                    }
                }

                if (MainMenu.Item("useSwipe").GetValue<bool>() && _swipe.IsReady() &&
                    _player.Distance(target.Position, true) <= _swipe.RangeSqr)
                {
                    _swipe.Cast(target);
                }

                //I'd call it 0.5% ?

                if (MainMenu.Item("useHuman").GetValue<bool>())
                {
                    if (_player.Distance(target.ServerPosition) > pounceDistance && HQ < 0.5 &&
                        _player.Distance(target.ServerPosition) < _javelinToss.Range && CW < 0.5)
                    {
                        var prediction = _javelinToss.GetPrediction(target);
                        if (_aspectOfTheCougar.IsReady() && prediction.Hitchance >= HitChance.Medium)
                        {
                            _aspectOfTheCougar.Cast();
                            //LeagueSharp.Common.Utility.DelayAction.Add(200, () => _javelinToss.Cast(prediction.CastPosition));
                        }
                    }
                }
            }
            else
            {
                if (MainMenu.Item("useJavelin").GetValue<bool>() && _javelinToss.IsReady() &&
                    target.IsValidTarget(_javelinToss.Range) &&
                    _player.Distance(target.Position) <= MainMenu.Item("javelinRange").GetValue<Slider>().Value)
                {
                    _javelinToss.CastIfHitchanceEquals(target, CustomHitChance);
                }

                if (MainMenu.Item("useBushwhack").GetValue<bool>() && _bushwhack.IsReady() &&
                    target.IsValidTarget(_bushwhack.Range) && _player.Distance(target.Position) <= _bushwhack.Range)
                {
                    _bushwhack.CastIfHitchanceEquals(target, CustomHitChance);
                }

                if (MainMenu.Item("useCougar").GetValue<bool>() && (CW < 0.2) && (CQ < 0.2) && (CE < 0.2) &&
                    !_javelinToss.IsReady() && _player.Distance(target) <= pounceDistance)
                {
                    if (MainMenu.Item("pounceHunted").GetValue<bool>()) {}
                    if (_aspectOfTheCougar.IsReady())
                    {
                        _aspectOfTheCougar.Cast();
                    }
                }
            }
        }

        #endregion

        #region Harass

        private static void OnHarass(AIHeroClient target)
        {
            if (!target.IsValidTarget(_javelinToss.Range) || !_manaManager.CanHarass())
            {
                return;
            }

            var pred = _javelinToss.GetPrediction(target);
            if (!_player.IsCougar() && MainMenu.Item("useJavelinHarass").GetValue<bool>() && _javelinToss.IsReady() &&
                target.IsValidTarget(_javelinToss.Range) && _player.Distance(target.Position) <= _javelinToss.Range &&
                pred.Hitchance >= CustomHitChance)
            {
                _javelinToss.Cast(pred.CastPosition);
            }
        }

        #endregion

        #region WaveClear

        private static void WaveClear()
        {
            //redo
            List<Obj_AI_Base> allMinionsQ = MinionManager.GetMinions(
                _player.ServerPosition, _takedown.Range, MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> allMinionsW = MinionManager.GetMinions(
                _player.ServerPosition, _pounce.Range, MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> allMinionsE = MinionManager.GetMinions(
                _player.ServerPosition, _swipe.Range, MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> allMinionsQ2 = MinionManager.GetMinions(
                _player.ServerPosition, _javelinToss.Range, MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> allMinionsW2 = MinionManager.GetMinions(
                _player.ServerPosition, _bushwhack.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (!allMinionsQ[0].IsValidTarget(_takedown.Range) || !allMinionsW[0].IsValidTarget(_pounce.Range) ||
                !allMinionsE[0].IsValidTarget(_swipe.Range) || !allMinionsQ2[0].IsValidTarget(_javelinToss.Range) ||
                !allMinionsW2[0].IsValidTarget(_bushwhack.Range) || !_manaManager.CanLaneclear() && !_player.IsCougar())
            {
                return;
            }

            if (_player.IsCougar())
            {
                if (MainMenu.Item("wcUseCougarQ").GetValue<bool>() && allMinionsQ.Count > 0 &&
                    allMinionsQ[0].IsValidTarget(_takedown.Range) && _takedown.IsReady())
                {
                    _takedown.Cast();
                }

                if (MainMenu.Item("wcUseCougarW").GetValue<bool>() && allMinionsW.Count > 0 &&
                    allMinionsW[0].IsValidTarget(_pounce.Range) && _pounce.IsReady())
                {
                    _pounce.Cast(allMinionsW[0].Position);
                }

                if (MainMenu.Item("wcUseCougarE").GetValue<bool>() && allMinionsE.Count > 0 &&
                    allMinionsE[0].IsValidTarget(_swipe.Range) && _swipe.IsReady())
                {
                    _swipe.Cast(allMinionsE[0]);
                }
            } // TODO remake ofc
            else
            {
                if (MainMenu.Item("wcUseHumanQ").GetValue<bool>() && allMinionsQ2.Count > 0 &&
                    allMinionsQ2[0].IsValidTarget(_javelinToss.Range) && _javelinToss.IsReady())
                {
                    _javelinToss.Cast(allMinionsQ2[0]);
                }

                if (MainMenu.Item("wcUseHumanW").GetValue<bool>() && allMinionsW2.Count > 0 &&
                    allMinionsW2[0].IsValidTarget(_bushwhack.Range) && _bushwhack.IsReady())
                {
                    _bushwhack.Cast(allMinionsW2[0]);
                }
            }
        }

        #endregion

        #region JungleClear credits to kurisu :S

        private static void JungleClear()
        {
            Obj_AI_Minion smallMobs =
                ObjectManager.Get<Obj_AI_Minion>()
                    .FirstOrDefault(
                        x => x.Name.Contains("Mini") && !x.Name.StartsWith("Minion") && x.IsValidTarget(700));

            Obj_AI_Minion bigMobs =
                ObjectManager.Get<Obj_AI_Minion>()
                    .FirstOrDefault(
                        x =>
                            !x.Name.Contains("Mini") && !x.Name.StartsWith("Minion") &&
                            JungleMinions.Any(name => x.Name.StartsWith(name)) && x.IsValidTarget(900));

            var selectedMinion = bigMobs ?? smallMobs;
            var pounceDistance = selectedMinion.IsHunted() ? 730 : _pounce.Range;
            if (selectedMinion != null)
            {
                if (_player.IsCougar())
                {
                    if (_player.Distance(selectedMinion.ServerPosition) <= _swipe.Range)
                    {
                        if (MainMenu.Item("jcUseCougarE").GetValue<bool>() && !_pounce.IsReady())
                        {
                            _swipe.Cast(selectedMinion.ServerPosition);
                        }

                        if (_player.Distance(selectedMinion.ServerPosition) <= pounceDistance &&
                            (CW < 0 || _pounce.IsReady()))
                        {
                            if (MainMenu.Item("jcUseCougarW").GetValue<bool>())
                            {
                                _pounce.Cast(selectedMinion.ServerPosition);
                            }
                        }

                        if (_player.Distance(selectedMinion.ServerPosition) <= _takedown.Range && CQ < 1)
                        {
                            if (MainMenu.Item("jcUseCougarQ").GetValue<bool>())
                            {
                                _takedown.Cast();
                            }
                        }

                        if ((!_takedown.IsReady() || CQ > 1) && (!_pounce.IsReady() || CW > 1) &&
                            (_swipe.IsReady() || CE > 1))
                        {
                            if (_aspectOfTheCougar.IsReady())
                            {
                                _aspectOfTheCougar.Cast();
                            }
                        }
                    }
                }
                else
                {
                    if (MainMenu.Item("jcUseHumanQ").GetValue<bool>())
                    {
                        _javelinToss.Cast(selectedMinion.ServerPosition);
                    }

                    if (HQ > 1 || !_javelinToss.IsReady() && _aspectOfTheCougar.IsReady())
                    {
                        _aspectOfTheCougar.Cast();

                        //Chat.Print("Cast Delay for Spear = "+castDelay);
                    }
                }
            }
        }

        #endregion

        #region Flee Credits to Hellsing.

        private static void Flee()
        {
            if (!_player.IsCougar() && _aspectOfTheCougar.IsReady() && CW < 0.2)
            {
                _aspectOfTheCougar.Cast();
            }
            // We need to define a new move position since jumping over walls
            // requires you to be close to the specified wall. Therefore we set the move
            // point to be that specific piont. People will need to get used to it,
            // but this is how it works.
            var wallCheck = VectorHelper.GetFirstWallPoint(_player.Position, Game.CursorPos);
            // Be more precise
            if (wallCheck != null)
            {
                wallCheck = VectorHelper.GetFirstWallPoint((Vector3) wallCheck, Game.CursorPos, 5);
            }
            // Define more position point
            var movePosition = wallCheck != null ? (Vector3) wallCheck : Game.CursorPos;
            // Update fleeTargetPosition
            var tempGrid = NavMesh.WorldToGrid(movePosition.X, movePosition.Y);
            _fleeTargetPosition = NavMesh.GridToWorld((short) tempGrid.X, (short) tempGrid.Y);
            // Also check if we want to AA aswell
            // Reset walljump indicators
            // Only calculate stuff when our Q is up and there is a wall inbetween
            if (_player.IsCougar() && _pounce.IsReady() && wallCheck != null)
            {
                // Get our wall position to calculate from
                var wallPosition = movePosition;
                // Check 300 units to the cursor position in a 160 degree cone for a valid non-wall spot
                Vector2 direction = (Game.CursorPos.To2D() - wallPosition.To2D()).Normalized();
                const float maxAngle = 80;
                const float step = maxAngle / 20;
                float currentAngle = 0;
                float currentStep = 0;
                bool jumpTriggered = false;
                while (true)
                {
                    // Validate the counter, break if no valid spot was found in previous loops
                    if (currentStep > maxAngle && currentAngle < 0)
                    {
                        break;
                    }
                    // Check next angle
                    if ((Math.Abs(currentAngle) < 0.001 || currentAngle < 0) && Math.Abs(currentStep) > 0.000)
                    {
                        currentAngle = (currentStep) * (float) Math.PI / 180;
                        currentStep += step;
                    }
                    else if (currentAngle > 0)
                    {
                        currentAngle = -currentAngle;
                    }
                    Vector3 checkPoint;
                    // One time only check for direct line of sight without rotating
                    if (Math.Abs(currentStep) < 0.001)
                    {
                        currentStep = step;
                        checkPoint = wallPosition + _pounce.Range * direction.To3D();
                    }
                    // Rotated check
                    else
                    {
                        checkPoint = wallPosition + _pounce.Range * direction.Rotated(currentAngle).To3D();
                    }
                    // Check if the point is not a wall
                    if (!checkPoint.IsWall())
                    {
                        // Check if there is a wall between the checkPoint and wallPosition
                        wallCheck = VectorHelper.GetFirstWallPoint(checkPoint, wallPosition);
                        if (wallCheck != null)
                        {
                            // There is a wall inbetween, get the closes point to the wall, as precise as possible
                            Vector2? firstWallPoint = VectorHelper.GetFirstWallPoint(
                                (Vector3) wallCheck, wallPosition, 5);
                            if (firstWallPoint != null)
                            {
                                Vector3 wallPositionOpposite = (Vector3) firstWallPoint;
                                // Check if it'spell worth to jump considering the path length
                                if (_player.GetPath(wallPositionOpposite).ToList().To2D().PathLength() -
                                    _player.Distance(wallPositionOpposite) > 200)
                                {
                                    // Check the distance to the opposite side of the wall
                                    if (_player.Distance(wallPositionOpposite, true) <
                                        Math.Pow(375 - _player.BoundingRadius / 2, 2))
                                    {
                                        _pounce.Cast(wallPositionOpposite);
                                        jumpTriggered = true;
                                    }
                                }
                                else
                                {
                                    Render.Circle.DrawCircle(Game.CursorPos, 35, Color.Red, 2);
                                }
                            }
                        }
                    }
                }
                // Check if the loop triggered the jump, if not just orbwalk
                if (!jumpTriggered)
                {
                    Orbwalking.Orbwalk(null, Game.CursorPos, 90f, 0f, false, false);
                }
            }
            // Either no wall or W on cooldown, just move towards to wall then
            else
            {
                Orbwalking.Orbwalk(null, Game.CursorPos, 90f, 0f, false, false);
                if (_player.IsCougar() && _pounce.IsReady())
                {
                    _pounce.Cast(Game.CursorPos);
                }
            }
        }

        #endregion

        #region KillSteal

        private static void Killsteal()
        {
            if (MainMenu.Item("killstealUseQ").GetValue<bool>())
            {
                foreach (PredictionOutput pred in
                    from target in HeroManager.Enemies.Where(hero => hero.IsValidTarget(_javelinToss.Range))
                    let prediction = _javelinToss.GetPrediction(target)
                    let javelinDamage = GetActualSpearDamage(target)
                    where target.Health <= javelinDamage && prediction.Hitchance >= HitChance.Medium
                    select prediction)
                {
                    _javelinToss.Cast(pred.CastPosition);
                }
            }

            if (MainMenu.Item("smiteQ").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(_javelinToss.Range, TargetSelector.DamageType.Magical);
                if (GetActualSpearDamage(target) > target.Health + 5 && _javelinToss.IsReady())
                {
                    SmiteQ(target);
                }
            }

            if (MainMenu.Item("killstealDashing").GetValue<bool>())
            {
                foreach (AIHeroClient target in
                    HeroManager.Enemies.Where(hero => hero.IsValidTarget(_javelinToss.Range))
                        .Where(target => GetActualSpearDamage(target) > target.Health + 20 && _javelinToss.IsReady()))
                {
                    _javelinToss.CastIfHitchanceEquals(target, HitChance.Dashing);
                }
            }
        }

        #endregion

        #region CreateMenu

        private static void CreateMenu()
        {
            MainMenu = new Menu(ChampionName + " the Bestial Huntress", ChampionName + " the bestial huntress", true);

            var targetSelectorMenu = new Menu("Target Selector", "ts");
            MainMenu.AddSubMenu(targetSelectorMenu);
            TargetSelector.AddToMenu(targetSelectorMenu);
            new AssassinManager();

            var orbwalkingMenu = new Menu("Orbwalking", "orbwalk");
            MainMenu.AddSubMenu(orbwalkingMenu);
            _orbwalker = new Orbwalking.Orbwalker(orbwalkingMenu);

            var keybindings = new Menu("Key Bindings", "keybindings");
            {
                keybindings.AddItem(new MenuItem("useCombo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
                keybindings.AddItem(new MenuItem("useHarass", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
                keybindings.AddItem(new MenuItem("useWC", "Waveclear").SetValue(new KeyBind('X', KeyBindType.Press)));
                keybindings.AddItem(new MenuItem("useJC", "Jungleclear").SetValue(new KeyBind('X', KeyBindType.Press)));
                keybindings.AddItem(new MenuItem("useFlee", "Flee").SetValue(new KeyBind('V', KeyBindType.Press)));
                MainMenu.AddSubMenu(keybindings);
            }

            var combo = new Menu("Combo Options", "combo");
            {
                var humanMenu = new Menu("Human Spells", "human");
                {
                    humanMenu.AddItem(new MenuItem("useJavelin", "Use Javelin (Q)").SetValue(true));
                    humanMenu.AddItem(
                        new MenuItem("javelinRange", "Javelin Range").SetValue(new Slider(1300, 500, 1500)));
                    humanMenu.AddItem(new MenuItem("useBushwhack", "Use Bushwhack (W)").SetValue(false));
                    humanMenu.AddItem(new MenuItem("useCougar", "Auto Transform to Cougar").SetValue(true));
                    combo.AddSubMenu(humanMenu);
                }
                var cougarMenu = new Menu("Cougar Spells", "cougar");
                {
                    cougarMenu.AddItem(new MenuItem("useTakedown", "Use Takedown (Q)").SetValue(true));
                    cougarMenu.AddItem(new MenuItem("usePounce", "Use Pounce (W)").SetValue(true));
                    cougarMenu.AddItem(new MenuItem("pounceHunted", " --> Only pounce hunted targets").SetValue(true));
                    cougarMenu.AddItem(new MenuItem("useSwipe", "Use Swipe (E)").SetValue(true));
                    cougarMenu.AddItem(new MenuItem("useHuman", "Auto Transform to Human").SetValue(true));
                    combo.AddSubMenu(cougarMenu);
                }
                MainMenu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass Options", "harass");
            {
                harass.AddItem(new MenuItem("useJavelinHarass", "Use Javelin (Q)").SetValue(true));
                MainMenu.AddSubMenu(harass);
            }

            var waveclear = new Menu("Waveclear Options", "waveclear");
            {
                waveclear.AddItem(new MenuItem("wcUseHumanQ", "Use Javelin Toss").SetValue(false));
                waveclear.AddItem(new MenuItem("wcUseHumanW", "Use Bushwhack").SetValue(false));
                waveclear.AddItem(new MenuItem("wcUseCougarQ", "Use Takedown").SetValue(true));
                waveclear.AddItem(new MenuItem("wcUseCougarW", "Use Pounce").SetValue(true));
                waveclear.AddItem(new MenuItem("wcUseCougarE", "Use Swipe").SetValue(true));
                MainMenu.AddSubMenu(waveclear);
            }

            var jungleclear = new Menu("Jungleclear Options", "jungleclear");
            {
                jungleclear.AddItem(new MenuItem("jcUseHumanQ", "Use Javelin Toss").SetValue(false));
                jungleclear.AddItem(new MenuItem("jcUseHumanW", "Use Bushwhack").SetValue(false));
                jungleclear.AddItem(new MenuItem("jcUseCougarQ", "Use Takedown").SetValue(true));
                jungleclear.AddItem(new MenuItem("jcUseCougarW", "Use Pounce").SetValue(true));
                jungleclear.AddItem(new MenuItem("jcUseCougarE", "Use Swipe").SetValue(true));
                jungleclear.AddItem(new MenuItem("jcMana", "Mana to Jungleclear").SetValue(new Slider(40, 100, 0)));
                MainMenu.AddSubMenu(jungleclear);
            }

            var killsteal = new Menu("Killsteal Options", "killsteal");
            {
                killsteal.AddItem(new MenuItem("killstealUseQ", "Use Javelin (Q)").SetValue(true));
                killsteal.AddItem(new MenuItem("killstealDashing", "Use Javelin on dashing").SetValue(true));
                killsteal.AddItem(new MenuItem("smiteQ", "Smite + Q killsteal").SetValue(true));
                MainMenu.AddSubMenu(killsteal);
            }

            _manaManager.AddToMenu(ref MainMenu);
            _healManager.AddToMenu(ref MainMenu);

            var misc = new Menu("Misc Options", "misc");
            {
                misc.AddItem(new MenuItem("miscIgnite", "Use Ignite").SetValue(true));
                misc.AddItem(new MenuItem("miscImmobile", "Use Javelin / Bushwhack on immobile").SetValue(true));
                misc.AddItem(
                    new MenuItem("hitChanceSetting", "Hitchance").SetValue(
                        new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3)));
                misc.AddItem(new MenuItem("turretSafety", "Don't use pounce under turret").SetValue(true));
                MainMenu.AddSubMenu(misc);
            }

            var drawings = new Menu("Drawing Options", "drawings");
            {
                drawings.AddItem(new MenuItem("drawRangeQ", "Q range").SetValue(new Circle(false, Color.Aquamarine)));
                drawings.AddItem(new MenuItem("drawRangeW", "W range").SetValue(new Circle(false, Color.Aquamarine)));
                drawings.AddItem(new MenuItem("drawRangeE", "E range").SetValue(new Circle(false, Color.Aquamarine)));
                drawings.AddItem(
                    new MenuItem("drawDamage", "Draw Spell Damage").SetValue(new Circle(false, Color.GreenYellow)));
                MainMenu.AddSubMenu(drawings);
            }

            var donationMenu = new Menu("Donating", "donations");
            {
                donationMenu.AddItem(new MenuItem("kindword", "If you feel like supporting my work"));
                donationMenu.AddItem(new MenuItem("kindword2", "Feel free to send a donation to: "));
                donationMenu.AddItem(new MenuItem("kindword3", "iJava.i@hotmail.com"));
                MainMenu.AddSubMenu(donationMenu);
                //https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=F9A3B9JPQYTDJ
            }

            MainMenu.AddToMainMenu();
        }

        #endregion

        #region Notifications Credits to Beaving.

        public static void ShowNotification(string message, Color color, int duration = -1, bool dispose = true)
        {
            var notify = new Notification(message).SetTextColor(color);
            Notifications.AddNotification(notify);
            if (dispose)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(duration, () => notify.Dispose());
            }
        }

        #endregion

        #region spear calculation

        private static float GetActualSpearDamage(AIHeroClient target)
        {
            double baseDamage = new double[] { 50, 75, 100, 125, 150 }[_javelinToss.Level - 1] +
                                0.4 * _player.FlatMagicDamageMod;
            var increasedDamageFactor = 1f;
            var distance = _player.Distance(target);
            if (distance > 525)
            {
                if (distance > 1300)
                {
                    distance = 1300;
                }
                float delta = distance - 525;
                increasedDamageFactor = delta / 7.75f * 0.02f;
            }
            return (float) (baseDamage * increasedDamageFactor);
        }

        #endregion

        #region OnImmobile

        private static void OnImmobile()
        {
            if (MainMenu.Item("miscImmobile").GetValue<bool>() && !_player.IsCougar())
            {
                foreach (var pred in
                    HeroManager.Enemies.Where(
                        hero => hero.IsValidTarget() && hero.Distance(_player.Position) <= _javelinToss.Range)
                        .Select(target => _javelinToss.GetPrediction(target))
                        .Where(pred => pred.Hitchance == HitChance.Immobile))
                {
                    _javelinToss.Cast(pred.CastPosition);
                }

                foreach (var pred in
                    HeroManager.Enemies.Where(
                        hero => hero.IsValidTarget() && hero.Distance(_player.Position) <= _bushwhack.Range)
                        .Select(target => _bushwhack.GetPrediction(target))
                        .Where(pred => pred.Hitchance == HitChance.Immobile))
                {
                    _bushwhack.Cast(pred.CastPosition);
                }
            }
        }

        #endregion

        #region IsUnderEnemyTurret

        private static bool IsUnderEnemyTurret(Obj_AI_Base unit)
        {
            IEnumerable<Obj_AI_Turret> turrets;
            if (unit.IsEnemy)
            {
                turrets =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .Where(
                            x =>
                                x.IsAlly && x.IsValid && !x.IsDead &&
                                unit.ServerPosition.Distance(x.ServerPosition) < x.AttackRange);
            }
            else
            {
                turrets =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .Where(
                            x =>
                                x.IsEnemy && x.IsValid && !x.IsDead &&
                                unit.ServerPosition.Distance(x.ServerPosition) < x.AttackRange);
            }
            return (turrets.Any());
        }

        #endregion

        #region smite q

        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        private static string GetSmiteName()
        {
            if (SmiteBlue.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmiteplayerganker";
            }
            if (SmiteRed.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmiteduel";
            }
            if (SmiteGrey.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmitequick";
            }
            if (SmitePurple.Any(a => Items.HasItem(a)))
            {
                return "itemsmiteaoe";
            }
            return "summonersmite";
        }

        private static void SmiteQ(AIHeroClient target)
        {
            if (target.IsValidTarget(_javelinToss.Range) && _javelinToss.IsReady() && !_player.IsCougar())
            {
                //var projection = _player.ServerPosition.To2D()
                //   .ProjectOn(_player.ServerPosition.To2D(), target.ServerPosition.To2D());
                foreach (Obj_AI_Base minion in
                    from minion in
                        MinionManager.GetMinions(
                            _player.ServerPosition, _javelinToss.Range, MinionTypes.All, MinionTeam.NotAlly)
                    let projection =
                        minion.Position.To2D().ProjectOn(_player.ServerPosition.To2D(), target.ServerPosition.To2D())
                    where
                        projection.IsOnSegment &&
                        projection.SegmentPoint.Distance(minion) <= minion.BoundingRadius + _javelinToss.Width &&
                        _player.GetSpellSlot(GetSmiteName()).IsReady() &&
                        _player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite) > minion.Health
                    select minion)
                {
                    _player.Spellbook.CastSpell(_player.GetSpellSlot(GetSmiteName()), minion);
                    _javelinToss.Cast(target);
                }
            }
        }

        #endregion

        #region OnGameLoad

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            _player = ObjectManager.Player;
            if (_player.ChampionName != ChampionName)
            {
                return;
            }

            DamageIndicator.Initialize(GetComboDamage);
            DamageIndicator.Enabled = true;
            DamageIndicator.DrawingColor = Color.Green;

            _javelinToss = new Spell(SpellSlot.Q, 1500f);
            _takedown = new Spell(SpellSlot.Q, 200f);
            _bushwhack = new Spell(SpellSlot.W, 900f);
            _pounce = new Spell(SpellSlot.W, 375f);
            _primalSurge = new Spell(SpellSlot.E, 600f);
            _swipe = new Spell(SpellSlot.E, 300f);
            _aspectOfTheCougar = new Spell(SpellSlot.R);

            HumanSpellList.AddRange(new[] { _javelinToss, _bushwhack, _primalSurge });
            CougarSpellList.AddRange(new[] { _takedown, _pounce, _swipe });

            _javelinToss.SetSkillshot(0.125f, 40f, 1300f, true, SkillshotType.SkillshotLine);
            _bushwhack.SetSkillshot(0.50f, 100f, 1500f, false, SkillshotType.SkillshotCircle);
            _swipe.SetSkillshot(0.50f, 375f, 1500f, false, SkillshotType.SkillshotCone);
            _pounce.SetSkillshot(0.50f, 400f, 1500f, false, SkillshotType.SkillshotCone);

            _healManager = new HealManager();
            _manaManager = new ManaManager();

            CreateMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnSpellCast;
            Orbwalking.OnAttack += OnAttack;

            ShowNotification("Nidalee by blacky & iJabba", Color.Crimson, 4000);
            ShowNotification("Heal & ManaManager by iJabba", Color.Crimson, 4000);
        }

        private static void OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            AIHeroClient targetQ = target as AIHeroClient;
            if (unit.IsMe && _takedown.IsReady() && MainMenu.Item("useTakedown").GetValue<bool>() && _player.IsCougar())
            {
                if (targetQ.IsValidTarget())
                {
                    _takedown.Cast();
                }
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                GetCooldowns(args);
            }
        }

        private static readonly float[] HumanQcd = { 6, 6, 6, 6, 6 };
        private static readonly float[] HumanWcd = { 13, 12, 11, 10, 9 };
        private static readonly float[] HumanEcd = { 12, 12, 12, 12, 12 };
        private static float CQRem, CWRem, CERem;
        private static float HQRem, HWRem, HERem;
        private static float CQ, CW, CE;
        private static float HQ, HW, HE;

        private static void ProcessCooldowns()
        {
            if (_player.IsDead)
            {
                return;
            }
            CQ = ((CQRem - Game.Time) > 0) ? (CQRem - Game.Time) : 0;
            CW = ((CWRem - Game.Time) > 0) ? (CWRem - Game.Time) : 0;
            CE = ((CERem - Game.Time) > 0) ? (CERem - Game.Time) : 0;
            HQ = ((HQRem - Game.Time) > 0) ? (HQRem - Game.Time) : 0;
            HW = ((HWRem - Game.Time) > 0) ? (HWRem - Game.Time) : 0;
            HE = ((HERem - Game.Time) > 0) ? (HERem - Game.Time) : 0;
        }

        private static float CalculateCd(float time)
        {
            return time + (time * _player.PercentCooldownMod);
        }

        private static void GetCooldowns(GameObjectProcessSpellCastEventArgs spell)
        {
            if (_player.IsCougar())
            {
                if (spell.SData.Name == "Takedown")
                {
                    CQRem = Game.Time + CalculateCd(5);
                }
                if (spell.SData.Name == "Pounce")
                {
                    CWRem = Game.Time + CalculateCd(5);
                }
                if (spell.SData.Name == "Swipe")
                {
                    CERem = Game.Time + CalculateCd(5);
                }
            }
            else
            {
                if (spell.SData.Name == "JavelinToss")
                {
                    HQRem = Game.Time + CalculateCd(HumanQcd[_javelinToss.Level - 1]);
                }
                if (spell.SData.Name == "Bushwhack")
                {
                    HWRem = Game.Time + CalculateCd(HumanWcd[_bushwhack.Level - 1]);
                }
                if (spell.SData.Name == "PrimalSurge")
                {
                    HERem = Game.Time + CalculateCd(HumanEcd[_primalSurge.Level - 1]);
                }
            }
        }

        #endregion

        #region calculations

        private static bool IsCougar(this AIHeroClient player)
        {
            return player.Spellbook.GetSpell(SpellSlot.Q).Name == "Takedown";
        }

        private static bool IsHunted(this Obj_AI_Base target)
        {
            return target.HasBuff("nidaleepassivehunted");
        }

        private static HitChance GetHitchance()
        {
            switch (MainMenu.Item("hitChanceSetting").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        private static float GetComboDamage(AIHeroClient target)
        {
            double damage = 0d;

            if (_player.IsCougar())
            {
                if (_takedown.IsReady())
                {
                    damage += target.IsHunted()
                        ? _player.GetSpellDamage(target, SpellSlot.Q) * 0.33f
                        : _player.GetSpellDamage(target, SpellSlot.Q);
                }
                if (_pounce.IsReady())
                {
                    damage += _player.GetSpellDamage(target, SpellSlot.W);
                }
                if (_swipe.IsReady())
                {
                    damage += _player.GetSpellDamage(target, SpellSlot.E);
                }
            }
            else
            {
                if (_javelinToss.IsReady())
                {
                    damage += GetActualSpearDamage(target);
                }
            }

            return (float) damage;
        }

        private static List<Vector2> GetPointsInACircle(Vector2 center, int points, double radius)
        {
            var list = new List<Vector2>();
            var slice = 2 * Math.PI / points;
            for (var i = 0; i < points; i++)
            {
                var angle = slice * i;
                var newX = (int) (center.X + radius * Math.Cos(angle));
                var newY = (int) (center.Y + radius * Math.Sin(angle));
                list.Add(new Vector2(newX, newY));
            }
            return list;
        }

        private static Vector3 GetPosPrediction(Spell spell, AIHeroClient target)
        {
            foreach (Vector2 position in GetPointsInACircle(_player.ServerPosition.To2D(), 36, spell.Range))
            {
                SetSpellPosition(spell, position.To3D());
                if (spell.GetPrediction(target).Hitchance >= HitChance.High)
                {
                    return position.To3D();
                }
            }
            SetSpellPosition(spell, new Vector3());
            return new Vector3();
        }

        public static void ExecutePositionPred(AIHeroClient target) //TODO fix / work
        {
            bool hasSwitched = false;
            foreach (Vector2 point in GetPointsInACircle(_player.ServerPosition.To2D(), 36, _pounce.Range))
            {
                SetSpellPosition(_javelinToss, point.To3D());

                if (target != null && target.IsValidTarget()) // TODO safety checks
                {
                    if (GetActualSpearDamage(target) > target.Health + 15)
                    {
                        if (_player.IsCougar() && _aspectOfTheCougar.IsReady() && _pounce.IsReady())
                        {
                            _pounce.Cast(point);
                            if (_aspectOfTheCougar.Cast())
                            {
                                hasSwitched = true;
                            }
                        }
                        if (!_player.IsCougar() && hasSwitched && _javelinToss.IsReady() &&
                            _javelinToss.GetPrediction(target).Hitchance >= CustomHitChance)
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(250, () => _javelinToss.Cast(target));
                            hasSwitched = false;
                        }
                    }
                }
            }

            SetSpellPosition(_pounce, new Vector3());
        }

        private static void SetSpellPosition(this Spell spell, Vector3 position)
        {
            if (position.IsValid())
            {
                spell.From = position;
                spell.RangeCheckFrom = position;
                return;
            }
            spell.From = new Vector3();
            spell.RangeCheckFrom = new Vector3();
        }

        #endregion
    }
}
