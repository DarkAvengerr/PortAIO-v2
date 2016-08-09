using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KarmaXD.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KarmaXD
{
    class Program
    {
        private static Spell Q, W, E, R;
        private static Menu Config;
        private static Orbwalking.Orbwalker Orbwalker;

        public static string[] HighChamps =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
                "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
                "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
                "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
                "Zed", "Ziggs","Kindred","Jhin"
            };
        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        public static HitChance[] HitchanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };

        public static void Game_OnGameLoad()
        {
            SpellDatabase.InitalizeSpellDatabase();

            Q = new Spell(SpellSlot.Q, 950f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 800f);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 60f, 1700f, true, SkillshotType.SkillshotLine);
            W.SetTargetted(0.25f, 2200f);
            E.SetTargetted(0.25f, float.MaxValue);


            Config = new Menu("KarmaXD","KarmaXD",true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu(":: Orbwalker Settings"));

                var comboMenu = new Menu(":: Combo Settings", ":: Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("karma.q.combo", "Use Q (Combo)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("karma.w.combo", "Use W (Combo)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("karma.w.hp", "Min. HP for (W)").SetValue(new Slider(25, 1, 99)));
                    comboMenu.AddItem(new MenuItem("karma.r.combo", "Use R (Combo)").SetValue(true));
                    Config.AddSubMenu(comboMenu);
                }

                var harassMenu = new Menu(":: Harass Settings", ":: Harass Settings");
                {
                    harassMenu.AddItem(new MenuItem("karma.q.harass", "Use Q (Harass)").SetValue(true));
                    harassMenu.AddItem(new MenuItem("karma.r.harass", "Use R (Harass)").SetValue(true));
                    harassMenu.AddItem(new MenuItem("harass.mana", "Min. Mana Percent").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(harassMenu);
                }

                var clearMenu = new Menu(":: Clear Settings", ":: Clear Settings");
                {
                    clearMenu.AddItem(new MenuItem("karma.q.clear", "Use Q (Clear)").SetValue(true));
                    clearMenu.AddItem(new MenuItem("min.count", "Min. Minion Count").SetValue(new Slider(3, 1, 5)));
                    clearMenu.AddItem(new MenuItem("clear.mana", "Min. Mana Percent").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(clearMenu);
                }

                var esettings = new Menu(":: E Settings", ":: E Settings").SetFontStyle(FontStyle.Bold, SharpDX.Color.HotPink);
                {

                    var evademenu = new Menu(":: Protectable Skillshots", ":: Protectable Skillshots");
                    {
                        foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.EvadeableSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsSkillshot)))
                        {
                            evademenu.AddItem(new MenuItem(string.Format("e.protect.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                        }
                        esettings.AddSubMenu(evademenu);
                    }
                    var targettedmenu = new Menu(":: Protectable Targetted Spells", ":: Protectable Targetted Spells");
                    {
                        foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsTargeted)))
                        {
                            targettedmenu.AddItem(new MenuItem(string.Format("e.protect.targetted.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                        }
                        esettings.AddSubMenu(targettedmenu);
                    }

                    var engagemenu = new Menu(":: Engage Spells", ":: Engage Spells");
                    {
                        foreach (var spell in HeroManager.Allies.SelectMany(ally => SpellDatabase.EscapeSpells.Where(p => p.ChampionName == ally.ChampionName)))
                        {
                            engagemenu.AddItem(new MenuItem(string.Format("e.engage.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                        }
                        esettings.AddSubMenu(engagemenu);
                    }

                    var ewhitelist = new Menu(":: Whitelist", ":: Whitelist");
                    {
                        foreach (var ally in HeroManager.Allies.Where(x => x.IsValid))
                        {
                            ewhitelist.AddItem(new MenuItem("e." + ally.ChampionName, "(E): " + ally.ChampionName).SetValue(HighChamps.Contains(ally.ChampionName)));
                        }
                        esettings.AddSubMenu(ewhitelist);
                    }
                    
                    esettings.AddItem(new MenuItem("evade.protector.E", "If enemy spell damage bigger than carry health cast (E) for protect").SetValue(true));
                    esettings.AddItem(new MenuItem("protect.carry.from.turret", "Protect Carry From Turrets").SetValue(true));
                    esettings.AddItem(new MenuItem("min.mana.for.e", "Min. Mana").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(esettings);
                }

                var drawMenu = new Menu(":: Draw Settings", ":: Draw Settings");
                {
                    var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo Damage").SetValue(true);
                    var drawFill = new MenuItem("RushDrawEDamageFill", "Combo Damage Fill").SetValue(new Circle(true, Color.Gold));

                    drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
                    drawMenu.SubMenu("Damage Draws").AddItem(drawFill);

                    DamageIndicator.DamageToUnit = TotalDamage;
                    DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
                    DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                    DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

                    drawDamageMenu.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };

                    drawFill.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };
                    Config.AddSubMenu(drawMenu);
                }
               
                Config.AddItem(new MenuItem("keysinfo", "                 Hit Chance Settings").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Gold));
                Config.AddItem(new MenuItem("hitchance", "Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));


                Config.AddToMainMenu();
            }

            Obj_AI_Base.OnProcessSpellCast += OnProcess;
            Game.OnUpdate += OnUpdate;
        }

        private static void OnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (E.IsReady())
            {
                if (sender.IsAlly && sender is AIHeroClient && Config.Item("e.engage." + args.SData.Name).GetValue<bool>()
                && Config.Item("e." + sender.CharData.BaseSkinName).GetValue<bool>() && sender.Distance(ObjectManager.Player.Position) <= E.Range
                    && !sender.IsDead && !sender.IsZombie && sender.IsValid)
                {
                    E.CastOnUnit(sender);
                }

                if (sender is AIHeroClient && sender.IsEnemy && args.Target.IsAlly && args.Target.Type == GameObjectType.AIHeroClient
                && args.SData.IsAutoAttack() && ObjectManager.Player.ManaPercent >= Config.Item("min.mana.for.e").GetValue<Slider>().Value
                && Config.Item("e." + ((AIHeroClient)args.Target).ChampionName).GetValue<bool>() && ((AIHeroClient)args.Target).Distance(ObjectManager.Player.Position) < E.Range)
                {
                    E.Cast((AIHeroClient)args.Target);
                }

                if (sender is AIHeroClient && args.Target.IsAlly && args.Target.Type == GameObjectType.AIHeroClient
                    && !args.SData.IsAutoAttack() && (Config.Item("e.protect." + args.SData.Name).GetValue<bool>() || Config.Item("e.protect.targetted." + args.SData.Name).GetValue<bool>())
                    && sender.IsEnemy && sender.GetSpellDamage(((AIHeroClient)args.Target), args.SData.Name) > ((AIHeroClient)args.Target).Health)
                {
                    E.Cast((AIHeroClient)args.Target);
                }

                if (sender is AIHeroClient && sender.IsEnemy && args.Target.IsAlly && args.Target.Type == GameObjectType.obj_AI_Turret
                    && args.SData.IsAutoAttack() && ObjectManager.Player.ManaPercent >= Config.Item("min.mana.for.e").GetValue<Slider>().Value
                    && ((AIHeroClient)args.Target).Distance(ObjectManager.Player.Position) < E.Range
                    && ((AIHeroClient)args.Target).HealthPercent < Config.Item("turret.hp.percent").GetValue<Slider>().Value)
                {
                    E.Cast((AIHeroClient)args.Target);
                }
            }
        }
        private static float TotalDamage(AIHeroClient hero)
        {
            var damage = 0d;
            if (Q.IsReady())
            {
                damage += Q.GetDamage(hero);
            }
            if (W.IsReady())
            {
                damage += W.GetDamage(hero);
            }
            if (E.IsReady())
            {
                damage += W.GetDamage(hero);
            }
            if (R.IsReady())
            {
                damage += R.GetDamage(hero);
            }
            return (float)damage;
        }
        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
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
            }
        }

        public static HitChance HikiChance(string menuName)
        {
            return HitchanceArray[Config.Item(menuName).GetValue<StringList>().SelectedIndex];
        }

        private static void Combo()
        {
            if (Q.IsReady() && Config.Item("karma.q.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Q.Range)))
                {
                    var hit = Q.GetPrediction(enemy);
                    if (hit.Hitchance >= HikiChance("hitchance"))
                    {
                        Q.Cast(hit.CastPosition);
                    }
                }
            }

            if (W.IsReady() && Config.Item("karma.w.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(W.Range - 50)))
                {
                    W.CastOnUnit(enemy);
                }
            }

            if (R.IsReady() && Config.Item("karma.r.combo").GetValue<bool>())
            {
                if (Q.IsReady() && Config.Item("karma.q.combo").GetValue<bool>())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
                    {
                        var hit = Q.GetPrediction(enemy);
                        if (hit.Hitchance >= HikiChance("hitchance"))
                        {
                            R.Cast();
                        }
                    }
                }
                if (W.IsReady() && Config.Item("karma.w.combo").GetValue<bool>()
                    && ObjectManager.Player.HealthPercent <= Config.Item("karma.w.hp").GetValue<Slider>().Value)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(W.Range+10)))
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent <= Config.Item("harass.mana").GetValue<Slider>().Value)
            {
                return;
            }

            if (Q.IsReady() && Config.Item("karma.q.harass").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
                {
                    var hit = Q.GetPrediction(enemy);
                    if (hit.Hitchance >= HitChance.VeryHigh)
                    {
                        R.Cast();
                    }
                    if (hit.Hitchance >= HikiChance("hitchance"))
                    {
                        Q.Cast(hit.CastPosition);
                    }
                    
                }
            }
        }

        private static void Clear()
        {
            if (ObjectManager.Player.ManaPercent <= Config.Item("clear.mana").GetValue<Slider>().Value)
            {
                return;
            }

            if (Q.IsReady() && Config.Item("karma.q.clear").GetValue<bool>())
            {
                var wminion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.NotAlly).ToList();
                var xx = Q.GetCircularFarmLocation(wminion);
                if (xx.MinionsHit >= Config.Item("min.count").GetValue<Slider>().Value)
                {
                    Q.Cast(xx.Position);
                }
            }

        }
        
    }
}
