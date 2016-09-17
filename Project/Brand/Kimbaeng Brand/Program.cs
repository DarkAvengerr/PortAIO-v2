using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kimbaeng_Brand
{

     class Program
    {
        public static Menu _Menu;

        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Orbwalking.Orbwalker _Orbwalker;

        public static Spell Q, W, E, R;

        public static SpellSlot IgniteSlot;

        static readonly Render.Text Text = new Render.Text(
                                               0, 0, "", 11, new ColorBGRA(255, 0, 0, 255), "monospace");
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_onGameLoad;
        }

        static void Game_onGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Brand")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 1050);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R, 750);

            Q.SetSkillshot(0.25f,60, 1600, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1, 240, float.MaxValue, false, SkillshotType.SkillshotCircle);
            
            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");

            (_Menu = new Menu("Kimbaeng Brand", "kimbaengbrand", true)).AddToMainMenu();

            var targetSelectorMenu = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            _Menu.AddSubMenu(targetSelectorMenu);

            _Orbwalker = new Orbwalking.Orbwalker(_Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking")));
            _Orbwalker.SetAttack(true);

            var HitchanceMenu = _Menu.AddSubMenu(new Menu("Hitchance", "Hitchance"));
            HitchanceMenu.AddItem(
                new MenuItem("Hitchance", "Hitchance").SetValue(
                    new StringList(new[] { "Low", "Medium", "High", "VeryHigh", "Impossible" }, 2)));

            var comboMenu = _Menu.AddSubMenu(new Menu("Combo", "Combo"));
            comboMenu.AddItem(new MenuItem("useCQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("useCW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("useCE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("useCR", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("useCI", "Use Ignite").SetValue(true));

            var harassMenu = _Menu.AddSubMenu(new Menu("Harass", "Harass"));
            harassMenu.AddItem(new MenuItem("useHQ", "UseQ").SetValue(true));
            harassMenu.AddItem(new MenuItem("useHW", "UseW").SetValue(true));
            harassMenu.AddItem(new MenuItem("useHE", "UseE").SetValue(true));
            harassMenu.AddItem(new MenuItem("ManaManagerH", "Mana %").SetValue(new Slider(60,1,100)));

            var laneMenu = _Menu.AddSubMenu(new Menu("Lane Clear", "laneclear"));
            laneMenu.AddItem(new MenuItem("useLW", "Use W").SetValue(true));
            laneMenu.AddItem(new MenuItem("useLE", "Use E").SetValue(true));
            laneMenu.AddItem(new MenuItem("ManaManagerL", "Mana %").SetValue(new Slider(30, 1, 100)));

            var LastHitMenu = _Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            LastHitMenu.AddItem(new MenuItem("useLHQ", "Use Q").SetValue(true));

            var MiscMenu = _Menu.AddSubMenu(new Menu("MISC", "MISC"));
            MiscMenu.AddItem(new MenuItem("gapclose", "Auto AntiGapcloser(E → Q)").SetValue(true));

            var DrawMenu = _Menu.AddSubMenu(new Menu("Drawing", "drawing"));
            DrawMenu.AddItem(new MenuItem("noDraw", "Disable Drawing").SetValue(true));
            DrawMenu.AddItem(new MenuItem("drawQ", "DrawQ").SetValue(new Circle(true, System.Drawing.Color.Goldenrod)));
            DrawMenu.AddItem(new MenuItem("drawW", "DrawW").SetValue(new Circle(false, System.Drawing.Color.Goldenrod)));
            DrawMenu.AddItem(new MenuItem("drawE", "DrawE").SetValue(new Circle(false, System.Drawing.Color.Goldenrod)));
            DrawMenu.AddItem(new MenuItem("drawR", "DrawR").SetValue(new Circle(false, System.Drawing.Color.Goldenrod)));
            //DrawMenu.AddItem(new MenuItem("drawdmg", "Draw Damage").SetValue(true));

            Game.OnUpdate += Game_onUpdate;
            Drawing.OnDraw += Drawing_Ondraw;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Chat.Print("<font color=\"#ED4C00\">Kimbaeng Brand</font> Loaded ");
            Chat.Print("If You like this Assembly plz <font color=\"#1DDB16\">Upvote</font> XD ");
        }

        static void Game_onUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (_Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (_Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed
                && (Player.ManaPercent > _Menu.Item("ManaManagerH").GetValue<Slider>().Value))
            {
                Harass();
                LastHit();
            }

            if (_Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                && (Player.ManaPercent > _Menu.Item("ManaManagerL").GetValue<Slider>().Value))
            {
                LaneClear();
            }

            if (_Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                LastHit();
            }
        }

        static void Combo()
        {
            var Target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (Target == null) return;

            var useQ = _Menu.Item("useCQ").GetValue<bool>();
            var useW = _Menu.Item("useCW").GetValue<bool>();
            var useE = _Menu.Item("useCE").GetValue<bool>();
            var useR = _Menu.Item("useCR").GetValue<bool>();
            var UseI = _Menu.Item("useCI").GetValue<bool>();
            var HC = HitChance.High; 
            switch (_Menu.Item("Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0: //Low
                    HC = HitChance.Low;
                    break;
                case 1: //Medium
                    HC = HitChance.Medium;
                    break;
                case 2: //High
                    HC = HitChance.High;
                    break;
                case 3: //Very High
                    HC = HitChance.VeryHigh;
                    break;
                case 4: //impossable
                    HC = HitChance.Impossible;
                    break;
            }
                if (Player.Distance(Target.Position) < E.Range)
                {
                    if (E.IsReady() && useE && Target.IsValidTarget(E.Range))
                        E.Cast(Target);
                if (Q.IsReady() && useQ && Target.IsValidTarget(Q.Range))
                {
                    if (Target.HasBuff("brandablaze"))
                    {
                        Q.CastIfHitchanceEquals(Target, HC);
                    }
                    else
                    {
                        Q.CastIfHitchanceEquals(Target, HC);
                    }
                }
                if (W.IsReady() && useW)
                        W.CastIfHitchanceEquals(Target, HC);
                    if (R.IsReady() && useR)
                        R.Cast(Target);
                }
                else
                {
                    if (W.IsReady() && useW) W.CastIfHitchanceEquals(Target, HC);
                if (Q.IsReady() && useQ && Target.IsValidTarget(Q.Range))
                {
                    if (Target.HasBuff("brandablaze"))
                    {
                        Q.CastIfHitchanceEquals(Target, HC);
                    }
                    else
                    {
                        Q.CastIfHitchanceEquals(Target, HC);
                    }
                }

                if (E.IsReady() && useE)
                    E.Cast(Target);
                if (R.IsReady() && useR)
                    R.Cast(Target);
                }

                if (IgniteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready &&
                ObjectManager.Player.Distance(Target.ServerPosition) < 600 &&
                Player.GetSummonerSpellDamage(Target, Damage.SummonerSpell.Ignite) > Target.Health && UseI)
                {
                    ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, Target);
                }
            
        }

        static void Harass()
        {
            var useQ = _Menu.Item("useHQ").GetValue<bool>();
            var useW = _Menu.Item("useHW").GetValue<bool>();
            var useE = _Menu.Item("useHE").GetValue<bool>();
            var Target = TargetSelector.GetTarget(W.Range - 50, TargetSelector.DamageType.Magical);
            var HC = HitChance.VeryHigh;
            switch (_Menu.Item("Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0: //Low
                    HC = HitChance.Low;
                    break;
                case 1: //Medium
                    HC = HitChance.Medium;
                    break;
                case 2: //High
                    HC = HitChance.High;
                    break;
                case 3: //Very High
                    HC = HitChance.VeryHigh;
                    break;
                case 4: //impossable
                    HC = HitChance.Impossible;
                    break;
            }

            if (Target != null)
            {
                if (Player.Distance(Target.Position) < E.Range)
                {
                    if (E.IsReady() && useE)
                        E.Cast(Target);
                    if (Q.IsReady() && useQ && Target.IsValidTarget(Q.Range))
                    {
                        if (Target.HasBuff("brandablaze"))
                        {
                            Q.CastIfHitchanceEquals(Target, HC);
                        }
                        else
                        {
                            Q.CastIfHitchanceEquals(Target, HC);
                        }
                    }

                    if (W.IsReady() && useW)
                        W.CastIfHitchanceEquals(Target, HC);
                }
                else
                {
                    if (W.IsReady() && useW)
                        W.CastIfHitchanceEquals(Target, HC);
                    if (Q.IsReady() && useQ && Target.IsValidTarget(Q.Range))
                    {
                        if (Target.HasBuff("brandablaze"))
                        {
                            Q.CastIfHitchanceEquals(Target, HC);
                        }
                        else
                        {
                            Q.CastIfHitchanceEquals(Target, HC);
                        }
                    }
                    if (E.IsReady() && useE)
                        E.Cast(Target);
                }
            }
        }

        static void LaneClear()
        {
            var useW = _Menu.Item("useLW").GetValue<bool>();
            var useE = _Menu.Item("useLE").GetValue<bool>();
            List<Obj_AI_Base> minions;
            bool jungleMobs;

            minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                W.Range,
                MinionTypes.All,
                MinionTeam.NotAlly);
            minions.RemoveAll(x => x.MaxHealth <= 5);
            jungleMobs = minions.Any(x => x.Team == GameObjectTeam.Neutral);

            var farminfo = MinionManager.GetBestCircularFarmLocation(
                minions.Select(x => x.Position.To2D()).ToList(),
                W.Width,
                W.Range);

            if (W.IsReady() && farminfo.MinionsHit > 2 && useW)
            {
                W.Cast(farminfo.Position, jungleMobs);
            }

            if (E.IsReady() && useE)
            {
                foreach (Obj_AI_Base minion in minions)
                {
                    if (minion.HasBuff("brandablaze"))
                    E.Cast(minion);
                    break;
                }
            }
        }

        static void LastHit()
        {
            var useQ = _Menu.Item("useLHQ").GetValue<bool>();

            if (!Q.IsReady() || !Orbwalking.CanMove(40))
            {
                return;
            }

            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);

            if (minionCount.Count > 0 && useQ && Q.IsReady())
            {
                foreach (var minion in minionCount)
                {
                    if (Player.Distance(minion) < Player.AttackRange)
                    {
                        return;
                    }
                    if (HealthPrediction.GetHealthPrediction(
                            minion, (int)(Q.Delay + (minion.Distance(Player.Position) / Q.Speed))) <
                        Player.GetSpellDamage(minion, SpellSlot.Q))
                    {
                        Q.Cast(minion);
                    }
                }
            }
        }
        static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly) return;

            if (!_Menu.Item("gapclose").GetValue<bool>())
            return;

            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.CastOnUnit(gapcloser.Sender);
            if (Q.IsReady() && gapcloser.Sender.IsValidTarget(Q.Range) && gapcloser.Sender.HasBuff("brandablaze"))
                Q.CastIfHitchanceEquals(gapcloser.Sender, HitChance.High);

        }

        static void DrawHPBarDamage()
        {
            const int XOffset = 10;
            const int YOffset = 20;
            const int Width = 103;
            const int Height = 8;
            foreach (var unit in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValid && h.IsHPBarRendered && h.IsEnemy))
            {
                var barPos = unit.HPBarPosition;
                var damage = getComboDamage(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int)barPos.X + XOffset;
                    Text.Y = (int)barPos.Y + YOffset - 13;
                    Text.text = ((int)(unit.Health - damage)).ToString();
                    Text.OnEndScene();
                }

                Drawing.DrawLine((float)xPosDamage, yPos, (float)xPosDamage, yPos + Height, 2, System.Drawing.Color.DarkGray);
            }
        }

        static double getComboDamage(Obj_AI_Base target)
        {
            double damage = Player.GetAutoAttackDamage(target);
            if (Q.IsReady() && _Menu.Item("comboUseQ").GetValue<bool>())
                damage += Player.GetSpellDamage(target, SpellSlot.Q);
            if (W.IsReady() && _Menu.Item("comboUseW").GetValue<bool>())
                damage += Player.GetSpellDamage(target, SpellSlot.W);
            if (E.IsReady() && _Menu.Item("comboUseE").GetValue<bool>())
                damage += Player.GetSpellDamage(target, SpellSlot.E);
            if (R.IsReady() && _Menu.Item("comboUseR").GetValue<bool>())
                damage += Player.GetSpellDamage(target, SpellSlot.R);

            return damage;
        }

        static void Drawing_Ondraw(EventArgs args)
        {
            var qValue = _Menu.Item("drawQ").GetValue<Circle>();
            var wValue = _Menu.Item("drawW").GetValue<Circle>();
            var eValue = _Menu.Item("drawE").GetValue<Circle>();
            var rValue = _Menu.Item("drawR").GetValue<Circle>();

            if (_Menu.Item("noDraw").GetValue<bool>())
                return;    

            if (qValue.Active)
            {
                if (Q.Instance.Level != 0)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, qValue.Color);
            }

            if (wValue.Active)
            {
                if (W.Instance.Level != 0)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, wValue.Color);
            }

            if (eValue.Active)
            {
                if (E.Instance.Level != 0)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, eValue.Color);
            }

            if (rValue.Active)
            {
                if (R.Instance.Level != 0)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, eValue.Color);
            }
            //if (_Menu.Item("drawdmg").GetValue<bool>())
            //    DrawHPBarDamage();
        }
  
    }
}
