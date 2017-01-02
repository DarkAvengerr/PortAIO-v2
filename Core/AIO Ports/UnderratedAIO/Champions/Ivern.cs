using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;
using SPrediction;

using Prediction = LeagueSharp.Common.Prediction;

using EloBuddy; 
using LeagueSharp.Common; 
namespace UnderratedAIO.Champions
{
    internal class Ivern
    {
        public static Menu config;
        private static Orbwalking.Orbwalker orbwalker;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static bool justW;
        public static int wWidth = 300;
        
        public AIHeroClient IgniteTarget;
        public List<Bush> Bushes = new List<Bush>();

        public Ivern()
        {
            InitIvern();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Ivern</font>");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Game_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            
            Jungle.setSmiteSlot();
        }


        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            if (hero.BaseSkinName == "Ivern" && args.Slot == SpellSlot.W)
            {
                Bushes.Add(new Bush(System.Environment.TickCount, args.End));
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (false)
            {
                return;
            }
            Bushes.RemoveAll(b => !b.isValid());
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
            if (IvernPet && R.IsReady())
            {
                PetHandler.MovePet(config, orbwalker.ActiveMode);
            }
        }

        private void Combo()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(W.Range, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                if (config.Item("useItems").GetValue<bool>())
                {
                    ItemHandler.UseItems(target, config, ComboDamage(target));
                }
                bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
                var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
                if (config.Item("useq", true).GetValue<bool>() && Q.IsReady() && Q.CanCast(target))
                {
                    if (Program.IsSPrediction)
                    {
                        Q.SPredictionCast(target, HitChance.High);
                    }
                    else
                    {
                        Q.CastIfHitchanceEquals(target, HitChance.High);
                    }
                }
                if (config.Item("usee", true).GetValue<bool>() && E.IsReady())
                {
                    var ally =
                        HeroManager.Allies.Where(
                            a =>
                                a.Distance(player) < E.Range &&
                                Program.IncDamages.GetAllyData(a.NetworkId).DamageTaken >
                                getEShield() / 100f * config.Item("useeDmg", true).GetValue<Slider>().Value ||
                                Program.IncDamages.GetAllyData(a.NetworkId).IsAboutToDie)
                            .OrderByDescending(a => a.ChampionsKilled)
                            .FirstOrDefault();
                    if (ally != null)
                    {
                        E.CastOnUnit(ally);
                    }
                    if (config.Item("useeNearEnemy", true).GetValue<bool>())
                    {
                        var allyNearTarget =
                            HeroManager.Allies.Where(a => a.Distance(player) < E.Range && a.Distance(target) < 400)
                                .OrderBy(a => a.Health)
                                .FirstOrDefault();
                        if (allyNearTarget != null)
                        {
                            E.CastOnUnit(allyNearTarget);
                        }
                        if (player.Pet != null && player.Pet.Position.Distance(target.Position) < 450)
                        {
                            E.CastOnUnit((Obj_AI_Base) player.Pet);
                        }
                    }
                }
                if (config.Item("usew", true).GetValue<bool>() && W.IsReady())
                {
                    if (config.Item("usewRengar", true).GetValue<bool>())
                    {
                        var rengar =
                            HeroManager.Allies.FirstOrDefault(
                                a =>
                                    a.Distance(player) < W.Range && a.Distance(target) < 600 && a.Distance(target) > 150 &&
                                    a.ChampionName == "Rengar");
                        if (rengar != null && !NavMesh.GetCollisionFlags(rengar.Position).HasFlag(CollisionFlags.Grass) &&
                            !Bushes.Any(b => b.Position.Distance(rengar.Position) < 300))
                        {
                            castW(rengar);
                        }
                    }
                    if (config.Item("usewAlly", true).GetValue<bool>())
                    {
                        var ally =
                            HeroManager.Allies.FirstOrDefault(
                                a =>
                                    a.Distance(player) < W.Range &&
                                    Program.IncDamages.GetAllyData(a.NetworkId).DamageTaken > a.Health * 0.4f &&
                                    Program.IncDamages.GetAllyData(a.NetworkId).AnyCC);
                        if (ally != null && !NavMesh.GetCollisionFlags(ally.Position).HasFlag(CollisionFlags.Grass) &&
                            !Bushes.Any(b => b.Position.Distance(ally.Position) < 300))
                        {
                            castW(ally);
                        }
                    }
                    if (config.Item("usewSelf", true).GetValue<bool>() && !player.Spellbook.IsAutoAttacking &&
                        !player.HasBuff("ivernwpassive") && player.AttackRange < 200 &&
                        ((!target.IsInAttackRange() && target.IsInAttackRange(325)) || !player.IsMoving) &&
                        !NavMesh.GetCollisionFlags(player.Position).HasFlag(CollisionFlags.Grass) &&
                        !Bushes.Any(b => b.Position.Distance(player.Position) < 300))
                    {
                        castW(player);
                    }
                }
                if (config.Item("user", true).GetValue<bool>() && R.IsReady() &&
                    (player.Distance(target) < R.Range + 50 ||
                     (target.HasBuffOfType(BuffType.Snare) && target.Distance(player) < 750)))
                {
                    R.Cast(target.Position);
                }
                if (config.Item("useIgnite").GetValue<bool>() && ignitedmg > target.Health && hasIgnite)
                {
                    if (IgniteTarget != null)
                    {
                        player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), IgniteTarget);
                        return;
                    }
                    player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
                }
            }
        }

        private double getEShield()
        {
            return new double[] { 80, 120, 160, 200, 240 }[E.Level - 1] + 0.8f * player.FlatMagicDamageMod;
        }


        private void castW(AIHeroClient t)
        {
            var pred = Prediction.GetPrediction(t, 0.1f);
            W.Cast(pred.CastPosition);
        }

        private static bool IvernPet
        {
            get { return player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "ivernrrecast"; }
        }

        private void Harass()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(E.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }
            float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            if (config.Item("useqH", true).GetValue<bool>() && Q.CanCast(target))
            {
                Q.Cast(target);
            }
        }

        private void Clear()
        {
            float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            if (config.Item("useeLC", true).GetValue<bool>() && E.IsReady())
            {
                if (MinionManager.GetMinions(player.Position, 350, MinionTypes.All, MinionTeam.NotAlly).Count >
                    config.Item("ehitLC", true).GetValue<Slider>().Value)
                {
                    E.CastOnUnit(player);
                }
                var data = Program.IncDamages.GetAllyData(player.NetworkId);
                if (data.DamageTaken > getEShield() / 100f * config.Item("useeDmgLC", true).GetValue<Slider>().Value ||
                    data.IsAboutToDie)
                {
                    E.CastOnUnit(player);
                }
            }
            if (config.Item("usewLC", true).GetValue<bool>() && W.IsReady() &&
                MinionManager.GetMinions(player.Position, 350, MinionTypes.All, MinionTeam.NotAlly).Count >
                config.Item("whitLC", true).GetValue<Slider>().Value && !player.HasBuff("ivernwpassive") &&
                !NavMesh.GetCollisionFlags(player.Position).HasFlag(CollisionFlags.Grass) &&
                !Bushes.Any(b => b.Position.Distance(player.Position) < 300))
            {
                castW(player);
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);
            
        }

        private float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (E.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }

            damage += ItemHandler.GetItemsDamage(hero);

            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private void InitIvern()
        {
            Q = new Spell(SpellSlot.Q, 1075);
            Q.SetSkillshot(0.5f, 65f, 1300f, true, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 1650);
            W.SetSkillshot(W.Delay, W.Width, W.Speed, false, SkillshotType.SkillshotCircle);
            E = new Spell(SpellSlot.E, 750);
            R = new Spell(SpellSlot.R, 350);
        }

        private void InitMenu()
        {
            config = new Menu("Ivern", "Ivern", true);
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
                .SetValue(new Circle(false, Color.FromArgb(180, 96, 153, 28)));
            menuD.AddItem(new MenuItem("drawww", "Draw W range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 96, 153, 28)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 96, 153, 28)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 96, 153, 28)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage")).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usewRengar", "   On Rengar to dash", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usewAlly", "   On Ally", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usewSelf", "   For AA range", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useeDmg", "   Min dmg in shield %", true)).SetValue(new Slider(50, 1, 100));
            menuC.AddItem(new MenuItem("useeNearEnemy", "   Near target", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite")).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q", true)).SetValue(true);
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("usewLC", "Use W", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("whitLC", "   Min", true).SetValue(new Slider(2, 1, 5)));
            menuLC.AddItem(new MenuItem("useeLC", "Use E", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("ehitLC", "   Min hit", true).SetValue(new Slider(2, 1, 5)));
            menuLC.AddItem(new MenuItem("useeDmgLC", "   Min dmg in shield %", true)).SetValue(new Slider(50, 1, 100));
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);
            // Misc Settings
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }

    struct Bush
    {
        public int Time;
        public Vector3 Position;

        public Bush(int time, Vector3 position)
        {
            Time = time;
            Position = position;
        }

        public bool isValid()
        {
            return System.Environment.TickCount - Time < 30000;
        }
    }
}