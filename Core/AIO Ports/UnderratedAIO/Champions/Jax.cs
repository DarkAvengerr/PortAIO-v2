using System;
using System.Linq;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;


using EloBuddy; 
using LeagueSharp.Common; 
 namespace UnderratedAIO.Champions
{
    internal class Jax
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public bool justE, justWJ;

        public Jax()
        {
            InitJax();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Jax</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Helpers.Jungle.setSmiteSlot();
            
            CustomEvents.Unit.OnDash += Unit_OnDash;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        private void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            AIHeroClient t = DrawHelper.GetBetterTarget(1100, TargetSelector.DamageType.Physical, true);
            if (!unit.IsMe || !W.IsReady() || !target.IsValidTarget() || !target.IsEnemy)
            {
                return;
            }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && target is AIHeroClient &&
                config.Item("usew", true).GetValue<bool>() && t != null && target.NetworkId == t.NetworkId)
            {
                W.Cast();
                Orbwalking.ResetAutoAttackTimer();
            }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && !(target is AIHeroClient) &&
                config.Item("usewLC", true).GetValue<bool>() &&
                MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(target), MinionTypes.All, MinionTeam.NotAlly)
                    .Count(m => m.Health > player.GetAutoAttackDamage((Obj_AI_Base) target, true)) > 0)
            {
                W.Cast();
                Orbwalking.ResetAutoAttackTimer();
            }
        }

        private void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (sender.Distance(player.Position) > Q.Range || !Q.IsReady())
            {
                return;
            }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && config.Item("useq", true).GetValue<bool>() &&
                args.EndPos.Distance(player.Position) > Q.Range &&
                args.EndPos.Distance(player) > args.StartPos.Distance(player))
            {
                Q.CastOnUnit(sender);
            }
        }

        private void InitJax()
        {
            Q = new Spell(SpellSlot.Q, 700);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if(false)
            {
                return;
            }
            orbwalker.SetMovement(true);
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                default:
                    break;
            }
            if (E.IsReady() && config.Item("autoE", true).GetValue<bool>() && !Eactive)
            {
                var data = Program.IncDamages.GetAllyData(player.NetworkId);
                if (config.Item("EAggro", true).GetValue<Slider>().Value <= data.AADamageCount)
                {
                    E.Cast();
                }
                if (data.AADamageTaken >= player.Health * config.Item("Emindam", true).GetValue<Slider>().Value / 100f)
                {
                    E.Cast();
                }
            }
            if (config.Item("wardJump", true).GetValue<KeyBind>().Active)
            {
                WardJump();
            }
        }

        private void WardJump()
        {
            Orbwalking.MoveTo(Game.CursorPos);
            if (!Q.IsReady())
            {
                return;
            }
            var wardSlot = Items.GetWardSlot();
            var pos = Game.CursorPos;
            if (pos.Distance(player.Position) > 600)
            {
                pos = player.Position.Extend(pos, 600);
            }

            var jumpObj = GetJumpObj(pos);
            if (jumpObj != null)
            {
                Q.CastOnUnit(jumpObj);
            }
            else
            {
                if (wardSlot != null && wardSlot.IsValidSlot() &&
                    (player.Spellbook.CanUseSpell(wardSlot.SpellSlot) == SpellState.Ready || wardSlot.Stacks != 0) &&
                    !justWJ)
                {
                    justWJ = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(new Random().Next(1000, 1500), () => { justWJ = false; });
                    player.Spellbook.CastSpell(wardSlot.SpellSlot, pos);
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        150, () =>
                        {
                            var predWard = GetJumpObj(pos);
                            if (predWard != null && Q.IsReady())
                            {
                                Q.CastOnUnit(predWard);
                            }
                        });
                }
            }
        }

        public Obj_AI_Base GetJumpObj(Vector3 pos)
        {
            return
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        obj =>
                            obj.IsValidTarget(600, false) && pos.Distance(obj.ServerPosition) <= 100 &&
                            (obj is Obj_AI_Minion || obj is AIHeroClient))
                    .OrderBy(obj => obj.Distance(pos))
                    .FirstOrDefault();
        }

        private void Harass()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(1100, TargetSelector.DamageType.Physical, true);
            float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc || target == null)
            {
                return;
            }
            if (config.Item("useqH", true).GetValue<bool>() && Orbwalking.CanMove(100) && !player.Spellbook.IsAutoAttacking &&
                Q.CanCast(target))
            {
                Q.CastOnUnit(target);
            }
        }

        private static bool Eactive
        {
            get { return player.HasBuff("JaxCounterStrike"); }
        }

        private void Clear()
        {
            float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            if (Q.IsReady() && config.Item("useqLC", true).GetValue<bool>())
            {
                var minions =
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(
                            m =>
                                Q.CanCast(m) &&
                                (Q.GetDamage(m) > m.Health || m.Health > player.GetAutoAttackDamage(m) * 5))
                        .OrderByDescending(m => Q.GetDamage(m) > m.Health)
                        .ThenBy(m => m.Distance(player));
                foreach (var mini in minions)
                {
                    if (!Orbwalking.CanAttack() && mini.Distance(player) <= Orbwalking.GetRealAutoAttackRange(mini))
                    {
                        Q.CastOnUnit(mini);
                        return;
                    }
                    if (Orbwalking.CanMove(100) && !player.Spellbook.IsAutoAttacking &&
                        mini.Distance(player) > Orbwalking.GetRealAutoAttackRange(mini))
                    {
                        Q.CastOnUnit(mini);
                        return;
                    }
                }
            }
        }

        private void Combo()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(1100, TargetSelector.DamageType.Physical, true);
            if (target == null || target.IsInvulnerable || target.MagicImmune)
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() && ignitedmg > target.Health && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) &&
                ((target.Distance(player) > Orbwalking.GetRealAutoAttackRange(target) &&
                  (!Q.IsReady() || Q.ManaCost < player.Mana)) || player.HealthPercent < 35))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (Q.CanCast(target))
            {
                if (config.Item("useqLimit", true).GetValue<bool>())
                {
                    if (player.CountEnemiesInRange(Q.Range) == 1 && config.Item("useq", true).GetValue<bool>() &&
                        (target.Distance(player) > Orbwalking.GetRealAutoAttackRange(target) ||
                         (Q.GetDamage(target) > target.Health) &&
                         (player.HealthPercent < 50 || player.CountAlliesInRange(900) > 0)))
                    {
                        if (Q.CastOnUnit(target))
                        {
                            HandleECombo();
                        }
                    }
                    if ((player.CountEnemiesInRange(Q.Range) > 1 && config.Item("useqSec", true).GetValue<bool>() &&
                         Q.GetDamage(target) > target.Health) || player.HealthPercent < 35f ||
                        target.Distance(player) > Orbwalking.GetRealAutoAttackRange(target))
                    {
                        if (Q.CastOnUnit(target))
                        {
                            HandleECombo();
                        }
                    }
                }
                else
                {
                    if (Q.CastOnUnit(target))
                    {
                        HandleECombo();
                    }
                }
            }
            if (R.IsReady() && config.Item("user", true).GetValue<bool>())
            {
                if (player.CountEnemiesInRange(Q.Range) >= config.Item("userMin", true).GetValue<Slider>().Value &&
                    Program.IncDamages.GetAllyData(player.NetworkId).DamageTaken > 40)
                {
                    R.Cast();
                }
                if (config.Item("userDmg", true).GetValue<bool>() &&
                    Program.IncDamages.GetAllyData(player.NetworkId).DamageTaken >= player.Health * 0.3f &&
                    player.Distance(target) < 450f)
                {
                    R.Cast();
                }
            }
            if (config.Item("useeAA", true).GetValue<bool>() && !Eactive &&
                Program.IncDamages.GetAllyData(player.NetworkId).AADamageTaken > target.GetAutoAttackDamage(player) - 10)
            {
                E.Cast();
            }
            if (Eactive)
            {
                if (E.IsReady() && target.IsValidTarget() && !target.MagicImmune &&
                    ((Prediction.GetPrediction(target, 0.1f).UnitPosition.Distance(player.Position) >
                      Orbwalking.GetRealAutoAttackRange(target) && target.Distance(player.Position) <= E.Range) ||
                     config.Item("useeStun", true).GetValue<bool>()))
                {
                    E.Cast();
                }
            }
            else
            {
                if (config.Item("useeStun", true).GetValue<bool>() &&
                    Prediction.GetPrediction(target, 0.1f).UnitPosition.Distance(player.Position) <
                    Orbwalking.GetRealAutoAttackRange(target) && target.Distance(player.Position) <= E.Range)
                {
                    E.Cast();
                }
            }
        }

        private void HandleECombo()
        {
            if (!Eactive)
            {
                if (config.Item("useeStun", true).GetValue<bool>() && E.IsReady() && !justE)
                {
                    justE = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        new Random().Next(10, 60), () =>
                        {
                            E.Cast();
                            justE = false;
                        });
                }
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }
            //damage += ItemHandler.GetItemsDamage(target);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }


        private void InitMenu()
        {
            config = new Menu("Jax ", "Jax", true);
            // Target Selector
            Menu menuTS = new Menu("Selector", "tselect");
            TargetSelector.AddToMenu(menuTS);
            config.AddSubMenu(menuTS);
            // Orbwalker
            Menu menuOrb = new Menu("Orbwalker", "orbwalker");
            orbwalker = new Orbwalking.Orbwalker(menuOrb);
            config.AddSubMenu(menuOrb);
            // Draw settings
            Menu menuD = new Menu("Drawings ", "dsettings");
            menuD.AddItem(new MenuItem("drawqq", "Draw Q range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 72, 46, 81)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 72, 46, 81)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings 
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useqLimit", "   Limit usage", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useqSec", "Use Q to secure kills", true)).SetValue(false);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useeStun", "Use E to stun", true)).SetValue(false);
            menuC.AddItem(new MenuItem("useeAA", "Block AA from target", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("userMin", "   Min enemies around", true)).SetValue(new Slider(2, 1, 5));
            menuC.AddItem(new MenuItem("userDmg", "   Use R before high damage", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q", true)).SetValue(true);
            menuH.AddItem(new MenuItem("usewH", "Use W on target", true)).SetValue(true);
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("usewLC", "Use w", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);

            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("autoE", "Auto E", true)).SetValue(true);
            menuM.AddItem(new MenuItem("EAggro", "   Aggro count", true)).SetValue(new Slider(3, 1, 10));
            menuM.AddItem(new MenuItem("Emindam", "   Damage % in health", true)).SetValue(new Slider(15, 1, 100));
            menuM.AddItem(new MenuItem("wardJump", "Ward jump", true))
                .SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            menuM = DrawHelper.AddMisc(menuM);

            config.AddSubMenu(menuM);


            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}