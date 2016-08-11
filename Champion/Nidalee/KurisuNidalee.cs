using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using CM = KurisuNidalee.CastManager;
using Color = System.Drawing.Color;
using KL = KurisuNidalee.KurisuLib;
using EloBuddy;

namespace KurisuNidalee
{
    internal class KurisuNidalee
    {
        internal static Menu Root;
        internal static AIHeroClient Target;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static AIHeroClient Player => ObjectManager.Player;

        internal KurisuNidalee()
        {                                                             
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        internal static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Nidalee")
            {
                return;
            }

            #region Root Menu
            Root = new Menu("Kurisu's Nidalee", "nidalee", true);

            var orbm = new Menu(":: Orbwalker", "orbm");
            Orbwalker = new Orbwalking.Orbwalker(orbm);
            Root.AddSubMenu(orbm);

            var ccmenu = new Menu(":: Nidalee Settings", "ccmenu");

            var humenu = new Menu(":: Human Settings", "humenu");

            var ndhq = new Menu("(Q)  Javelin", "ndhq");
            ndhq.AddItem(new MenuItem("ndhqcheck", "Check Hitchance")).SetValue(true);
            ndhq.AddItem(new MenuItem("qsmcol", "-> Smite Collision"))
                .SetValue(false).SetTooltip("Optimized but still may decrease performance.");
            ndhq.AddItem(new MenuItem("ndhqco", "Enable in Combo")).SetValue(true);
            ndhq.AddItem(new MenuItem("ndhqha", "Enable in Harass")).SetValue(true);
            ndhq.AddItem(new MenuItem("ndhqjg", "Enable in Jungle")).SetValue(true);
            ndhq.AddItem(new MenuItem("ndhqwc", "Enable in WaveClear")).SetValue(false);
            humenu.AddSubMenu(ndhq);

            var ndhw = new Menu("(W) Bushwhack", "ndhw");
            ndhw.AddItem(new MenuItem("ndhwco", "Enable in Combo")).SetValue(false);
            ndhw.AddItem(new MenuItem("ndhwsp", "-> Reduce (W) Usage"))
                .SetValue(false);
            ndhw.AddItem(new MenuItem("ndhwjg", "Enable in Jungle")).SetValue(true);
            ndhw.AddItem(new MenuItem("ndhwwc", "Enable in WaveClear")).SetValue(false);
            ndhw.AddItem(new MenuItem("ndhwforce", "Location"))
                .SetValue(new StringList(new[] {"Prediction", "Behind Target"}));
            humenu.AddSubMenu(ndhw);

            var ndhe = new Menu("(E)  Primal Surge", "ndhe");
            ndhe.AddItem(new MenuItem("ndheon", "Enable Healing")).SetValue(true);
            ndhe.AddItem(new MenuItem("ndhemana", "-> Minumum Mana")).SetValue(new Slider(55, 1));
            ndhe.AddItem(new MenuItem("ndhesw", "Switch Forms"))
                .SetValue(false).SetTooltip("Auto Switch From if Can Heal");

            foreach (var hero in HeroManager.Allies)
            {
                ndhe.AddItem(new MenuItem("xx" + hero.ChampionName, "Heal on " + hero.ChampionName))
                    .SetValue(true);
                ndhe.AddItem(new MenuItem("zz" + hero.ChampionName, hero.ChampionName + " below Pct% "))
                    .SetValue(new Slider(88, 1, 99));
            }


            ndhe.AddItem(new MenuItem("ndheord", "Ally Priority:")).SetValue(new StringList(new[] { "Low HP", "Most AD/AP", "Max HP" }, 1));            
            humenu.AddSubMenu(ndhe);

            var ndhr = new Menu("(R) Aspect of the Cougar", "ndhr");
            ndhr.AddItem(new MenuItem("ndhrco", "Enable in Combo")).SetValue(true);
            ndhr.AddItem(new MenuItem("ndhrcreq", "-> Require Swipe/Takedown")).SetValue(true);
            ndhr.AddItem(new MenuItem("ndhrha", "Enable in Harass")).SetValue(true);
            ndhr.AddItem(new MenuItem("ndhrjg", "Enable in Jungle")).SetValue(true);
            ndhr.AddItem(new MenuItem("ndhrjreq", "-> Require Swipe/Takedown")).SetValue(true);
            ndhr.AddItem(new MenuItem("ndhrwc", "Enable in WaveClear")).SetValue(false);
            humenu.AddSubMenu(ndhr);

            var comenu = new Menu(":: Cougar Settings", "comenu");

            var ndcq = new Menu("(Q) Takedown", "ndcq");
            ndcq.AddItem(new MenuItem("ndcqco", "Enable in Combo")).SetValue(true);
            ndcq.AddItem(new MenuItem("ndcqha", "Enable in Harass")).SetValue(true);
            ndcq.AddItem(new MenuItem("ndcqjg", "Enable in Jungle")).SetValue(true);
            ndcq.AddItem(new MenuItem("ndcqwc", "Enable in WaveClear")).SetValue(true);
            comenu.AddSubMenu(ndcq);

            var ndcw = new Menu("(W) Pounce", "ndcw");
            ndcw.AddItem(new MenuItem("ndcwcheck", "Check Hitchance")).SetValue(false);
            ndcw.AddItem(new MenuItem("ndcwch", "-> Min Hitchance"))
                .SetValue(new StringList(new[] {"Low", "Medium", "High", "Very High"}, 2));
            ndcw.AddItem(new MenuItem("ndcwco", "Enable in Combo")).SetValue(true);
            ndcw.AddItem(new MenuItem("ndcwhunt", "-> Ignore Checks if Hunted")).SetValue(false);
            ndcw.AddItem(new MenuItem("ndcwdistco", "-> Pounce Only if > AARange")).SetValue(true);
            ndcw.AddItem(new MenuItem("ndcwjg", "Enable in Jungle")).SetValue(true);
            ndcw.AddItem(new MenuItem("ndcwwc", "Enable in WaveClear")).SetValue(true);
            ndcw.AddItem(new MenuItem("ndcwdistwc", "-> Pounce Only if > AARange")).SetValue(false);
            ndcw.AddItem(new MenuItem("ndcwene", "-> Dont Pounce into Enemies")).SetValue(true);
            ndcw.AddItem(new MenuItem("ndcwtow", "-> Dont Pounce into Turret")).SetValue(true);
            comenu.AddSubMenu(ndcw);

            var ndce = new Menu("(E) Swipe", "ndce");

            ndce.AddItem(new MenuItem("ndcecheck", "Check Hitchance")).SetValue(false);
            ndce.AddItem(new MenuItem("ndcech", "-> Min Hitchance"))
                .SetValue(new StringList(new[] {"Low", "Medium", "High", "Very High"}, 2));
            ndce.AddItem(new MenuItem("ndceco", "Enable in Combo")).SetValue(true);
            ndce.AddItem(new MenuItem("ndceha", "Enable in Harass")).SetValue(true);
            ndce.AddItem(new MenuItem("ndcejg", "Enable in Jungle")).SetValue(true);
            ndce.AddItem(new MenuItem("ndcewc", "Enable in WaveClear")).SetValue(true);
            ndce.AddItem(new MenuItem("ndcenum", "-> Minimum Minions Hit")).SetValue(new Slider(3, 1, 5));           
            comenu.AddSubMenu(ndce);

            var ndcr = new Menu("(R) Aspect of the Cougar", "ndcr");
            ndcr.AddItem(new MenuItem("ndcrco", "Enable in Combo")).SetValue(true);
            ndcr.AddItem(new MenuItem("ndcrha", "Enable in Harass")).SetValue(true);
            ndcr.AddItem(new MenuItem("ndcrjg", "Enable in Jungle")).SetValue(true);
            ndcr.AddItem(new MenuItem("ndcrwc", "Enable in WaveClear")).SetValue(false);

            comenu.AddSubMenu(ndcr);


            var dmenu = new Menu(":: Draw Settings", "dmenu");
            dmenu.AddItem(new MenuItem("dp", ":: Draw Javelin Range")).SetValue(false);
            dmenu.AddItem(new MenuItem("dti", ":: Draw Javelin Timer")).SetValue(false);
            dmenu.AddItem(new MenuItem("dz", ":: Draw Pounce Range (Hunted)")).SetValue(false);
            dmenu.AddItem(new MenuItem("dt", ":: Draw Target")).SetValue(false);
            ccmenu.AddSubMenu(dmenu);

            var xmenu = new Menu(":: Jungle Settings", "xmenu");
            xmenu.AddItem(new MenuItem("spcol", ":: Force (R) if (Q) Collision [jungle]")).SetValue(false);
            xmenu.AddItem(new MenuItem("jgaacount", ":: AA Weaving jungle] [beta]"))
                .SetValue(new KeyBind('H', KeyBindType.Toggle))
                .SetTooltip("Require auto attacks before switching to Cougar").Permashow();
            xmenu.AddItem(new MenuItem("aareq", "-> Required auto attack Count [jungle]"))
                .SetValue(new Slider(2, 1, 5));
            xmenu.AddItem(new MenuItem("kitejg", ":: Pounce Away [jungle]")).SetTooltip("Try kiting with pounce.")
                .SetValue(false);
            ccmenu.AddSubMenu(xmenu);

            var aamenu = new Menu(":: Automatic Settings", "aamenu");
            aamenu.AddItem(new MenuItem("alvl6", ":: Auto (R) Level Up")).SetValue(false);
            aamenu.AddItem(new MenuItem("ndhqimm", ":: Auto (Q) Javelin Immobile")).SetValue(false);
            aamenu.AddItem(new MenuItem("ndhwimm", ":: Auto (W) Bushwhack Immobile")).SetValue(false);
            aamenu.AddItem(new MenuItem("ndhrgap", ":: Auto (R) Enemy Gapclosers")).SetValue(true);
            aamenu.AddItem(new MenuItem("ndcegap", ":: Auto (E) Swipe Gapclosers")).SetValue(true);
            aamenu.AddItem(new MenuItem("ndhqgap", ":: Auto (Q) Javelin Gapclosers")).SetValue(true);
            aamenu.AddItem(new MenuItem("ndcqgap", ":: Auto (Q) Takedown Gapclosers")).SetValue(true);

            ccmenu.AddItem(new MenuItem("pstyle", ":: Play Style"))
                .SetValue(new StringList(new[] {"Single Target", "Multi-Target"}, 1));


            ccmenu.AddSubMenu(comenu);
            ccmenu.AddSubMenu(humenu);
            ccmenu.AddSubMenu(aamenu);

            Root.AddSubMenu(ccmenu);

            var sset = new Menu(":: Smite Settings", "sset");
            sset.AddItem(new MenuItem("jgsmite", ":: Enable Smite")).SetValue(false);
            sset.AddItem(new MenuItem("jgsmitetd", ":: Takedown + Smite")).SetValue(true);
            sset.AddItem(new MenuItem("jgsmiteep", "-> Smite Epic")).SetValue(true);
            sset.AddItem(new MenuItem("jgsmitebg", "-> Smite Large")).SetValue(true);
            sset.AddItem(new MenuItem("jgsmitesm", "-> Smite Small")).SetValue(true);
            sset.AddItem(new MenuItem("jgsmitehe", "-> Smite On Hero")).SetValue(true);
            Root.AddSubMenu(sset);

            Root.AddItem(new MenuItem("usecombo", ":: Combo [active]")).SetValue(new KeyBind(32, KeyBindType.Press));
            Root.AddItem(new MenuItem("useharass", ":: Harass [active]"))
                .SetValue(new KeyBind('C', KeyBindType.Press));
            Root.AddItem(new MenuItem("usefarm", ":: Wave/Junge Clear [active]"))
                .SetValue(new KeyBind('V', KeyBindType.Press));
            Root.AddItem(new MenuItem("usecombo2", ":: Tripple AA [beta]"))
                .SetValue(new KeyBind('Z', KeyBindType.Press));
            Root.AddItem(new MenuItem("flee", ":: Flee/Walljumper [active]"))
                .SetValue(new KeyBind('A', KeyBindType.Press));


            var zzz = new MenuItem("ppred", ":: Prediction");

            Root.AddItem(zzz).SetValue(new StringList(new[] {"Common", "OKTW", "SPrediction"}));
            Root.AddItem(new MenuItem("ndhqch", "-> Min Hitchance"))
                .SetValue(new StringList(new[] {"Low", "Medium", "High", "Very High"}, 3));

            Root.AddItem(new MenuItem("bbb", ":: SPrediction not Loaded Please F5!"))
                .Show(false).SetFontStyle(FontStyle.Bold, SharpDX.Color.DeepPink);

            zzz.ValueChanged += (sender, eventArgs) =>
            {
                Root.Item("bbb")
                    .Show(eventArgs.GetNewValue<StringList>().SelectedIndex == 2 &&
                          Root.Children.All(x => x.Name != "SPRED"));

                if (eventArgs.GetNewValue<StringList>().SelectedIndex == 2)
                {
                    Root.Item("ndhqch").SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" }, 2));
                }

                if (eventArgs.GetNewValue<StringList>().SelectedIndex == 0 || eventArgs.GetNewValue<StringList>().SelectedIndex == 1)
                {
                    Root.Item("ndhqch").SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" }, 2));
                }
            };


            Root.AddToMainMenu();

            LeagueSharp.Common.Utility.DelayAction.Add(100, () =>
            {
                if (Root.Item("ppred").GetValue<StringList>().SelectedValue == "SPrediction")
                {
                    SPrediction.Prediction.Initialize(Root);

                    // Change menu name
                    if (Root.SubMenu("SPRED") != null)
                    {
                        Root.SubMenu("SPRED").DisplayName = ":: SPrediction";
                    }
                }
            });

            #endregion

            Game.OnUpdate += Game_OnUpdate;
            Chat.Print("<b><font color=\"#FF33D6\">Kurisu's Nidalee</font></b> - Loaded!");

            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffAdd;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnDoCast;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy || sender.Type != Player.Type || !args.SData.IsAutoAttack())
            {
                return;
            }

            foreach (var ally in Allies().Where(hero => !hero.IsMelee))
            {
                if (ally.NetworkId != sender.NetworkId || !Root.Item("xx" + ally.ChampionName).GetValue<bool>())
                {
                    return;
                }

                if (args.Target.Type == GameObjectType.AIHeroClient || args.Target.Type == GameObjectType.obj_AI_Turret)
                {
                    // auto heal on ally hero attacking
                    if (KL.CanUse(KL.Spells["Primalsurge"], true, "on"))
                    {
                        if (ally.IsValidTarget(KL.Spells["Primalsurge"].Range, false) &&
                            ally.Health / ally.MaxHealth * 100 <= 90)
                        {
                            if (!Player.Spellbook.IsChanneling && !Player.IsRecalling())
                            {
                                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None ||
                                    ally.Health / ally.MaxHealth * 100 <= 20 || !KL.CatForm())
                                {
                                    if (Player.Mana / Player.MaxMana * 100 <
                                        Root.Item("ndhemana").GetValue<Slider>().Value &&
                                        !(ally.Health / ally.MaxHealth * 100 <= 20))
                                        return;

                                    if (KL.CatForm() == false)
                                        KL.Spells["Primalsurge"].CastOnUnit(ally);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.IsAutoAttack())
            {
                if (Root.Item("usecombo2").GetValue<KeyBind>().Active)
                {
                    if (KL.CatForm() && KL.Spells["Aspect"].IsReady() && KL.SpellTimer["Javelin"].IsReady())
                    {
                        KL.Spells["Takedown"].Cast();

                        if (Player.HasBuff("Takedown"))
                        {
                            KL.Spells["Aspect"].Cast();
                        }
                    }

                    if (!KL.CatForm() && KL.SpellTimer["Javelin"].IsReady())
                    {
                        if (Utils.GameTimeTickCount - KL.LastBite <= 1200 || KL.SpellTimer["Javelin"].IsReady())
                        {
                            var targ = args.Target as Obj_AI_Base;
                            if (targ == null)
                            {
                                return;
                            }

                            if (targ.Path.Length < 1)
                                KL.Spells["Javelin"].Cast(targ.ServerPosition);

                            if (targ.Path.Length > 0)
                                KL.Spells["Javelin"].Cast(targ);                           
                        }
                    }
                }
            }
        }

        #region OnBuffAdd
        internal static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if (hero != null && hero.IsEnemy && KL.SpellTimer["Javelin"].IsReady() && Root.Item("ndhqimm").GetValue<bool>())
            {
                if (hero.IsValidTarget(KL.Spells["Javelin"].Range))
                {
                    if (args.Buff.Type == BuffType.Stun || args.Buff.Type == BuffType.Snare ||
                        args.Buff.Type == BuffType.Taunt || args.Buff.Type == BuffType.Knockback)
                    {
                        if (!KL.CatForm())
                        {
                            KL.Spells["Javelin"].Cast(hero);
                            KL.Spells["Javelin"].CastIfHitchanceEquals(hero, HitChance.Immobile);
                        }
                        else
                        {
                            if (KL.Spells["Aspect"].IsReady() &&
                                KL.Spells["Javelin"].Cast(hero) == Spell.CastStates.Collision)
                                KL.Spells["Aspect"].Cast();
                        }
                    }
                }
            }

            if (hero != null && hero.IsEnemy && KL.SpellTimer["Bushwhack"].IsReady() && Root.Item("ndhwimm").GetValue<bool>())
            {
                if (hero.IsValidTarget(KL.Spells["Bushwhack"].Range))
                {
                    if (args.Buff.Type == BuffType.Stun || args.Buff.Type == BuffType.Snare ||
                        args.Buff.Type == BuffType.Taunt || args.Buff.Type == BuffType.Knockback)
                    {
                        KL.Spells["Bushwhack"].Cast(hero);
                        KL.Spells["Bushwhack"].CastIfHitchanceEquals(hero, HitChance.Immobile);
                    }
                }
            }
        }

        #endregion
        
        #region OnDraw
        static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead || !Player.IsValid)
            {
                return;
            }

            foreach (
                var unit in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget(900) && x.PassiveRooted()))
            {
                var b = unit.GetBuff("NidaleePassiveMonsterRoot");
                if (b.Caster.IsMe && b.EndTime - Game.Time > 0)
                {
                    var tpos = Drawing.WorldToScreen(unit.Position);
                    Drawing.DrawText(tpos[0], tpos[1], Color.DeepPink,
                        "ROOTED " + (b.EndTime - Game.Time).ToString("F"));
                }
            }

            if (Root.Item("dti").GetValue<bool>())
            {
                var pos = Drawing.WorldToScreen(Player.Position);

                Drawing.DrawText(pos[0] + 100, pos[1] - 135, Color.White,
                    "Q: " + KL.SpellTimer["Javelin"].ToString("F"));             
            }

            if (Root.Item("dt").GetValue<bool>() && Target != null)
            {
                if (Root.Item("pstyle").GetValue<StringList>().SelectedIndex == 0)
                {
                    Render.Circle.DrawCircle(Target.Position, Target.BoundingRadius, Color.DeepPink, 6);
                }
            }

            if (Root.Item("dp").GetValue<bool>() && !KL.CatForm())
            {
                Render.Circle.DrawCircle(KL.Player.Position, KL.Spells["Javelin"].Range, Color.FromArgb(155, Color.DeepPink), 4);
            }

            if (Root.Item("dz").GetValue<bool>() && KL.CatForm())
            {
                Render.Circle.DrawCircle(KL.Player.Position, KL.Spells["ExPounce"].Range, Color.FromArgb(155, Color.DeepPink), 4);
            }
        }

        #endregion

        #region Ally Heroes
        internal static IEnumerable<AIHeroClient> Allies()
        {
            switch (Root.Item("ndheord").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HeroManager.Allies.OrderBy(h => h.Health / h.MaxHealth * 100);
                case 1:
                    return
                        HeroManager.Allies.OrderByDescending(h => h.BaseAttackDamage + h.FlatPhysicalDamageMod)
                            .ThenByDescending(h => h.FlatMagicDamageMod);
                case 2:
                    return HeroManager.Allies.OrderByDescending(h => h.MaxHealth);
            }

            return null;
        }

        #endregion

        internal static void Game_OnUpdate(EventArgs args)
        {
            Target = TargetSelector.GetTarget(KL.Spells["Javelin"].Range, TargetSelector.DamageType.Magical);

            #region Active Modes

            if (Root.Item("usecombo2").GetValue<KeyBind>().Active)
            {
                Combo2();
            }

            if (Root.Item("usecombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }

            if (Root.Item("useharass").GetValue<KeyBind>().Active)
            {
                Harass();
            }

            if (Root.Item("usefarm").GetValue<KeyBind>().Active)
            {
                Clear();
            }

            if (Root.Item("flee").GetValue<KeyBind>().Active)
            {
                Flee();
            }

            #endregion

            #region Auto Heal

            // auto heal on ally hero
            if (KL.CanUse(KL.Spells["Primalsurge"], true, "on"))
            {
                if (!Player.Spellbook.IsChanneling && !Player.IsRecalling())
                {
                    if (Root.Item("flee").GetValue<KeyBind>().Active && KL.CatForm())
                        return;

                    foreach (
                        var hero in
                            Allies().Where(
                                h => Root.Item("xx" + h.ChampionName).GetValue<bool>() &&
                                        h.IsValidTarget(KL.Spells["Primalsurge"].Range, false) &&
                                        h.Health / h.MaxHealth * 100 <
                                        Root.Item("zz" + h.ChampionName).GetValue<Slider>().Value))
                    {
                        if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None ||
                            hero.Health / hero.MaxHealth * 100 <= 20 || !KL.CatForm())
                        {
                            if (Player.Mana / Player.MaxMana * 100 < Root.Item("ndhemana").GetValue<Slider>().Value &&
                                !(hero.Health / hero.MaxHealth * 100 <= 20))
                                return;

                            if (KL.CatForm() == false)
                                KL.Spells["Primalsurge"].CastOnUnit(hero);

                            if (KL.CatForm() && Root.Item("ndhesw").GetValue<bool>() &&
                                KL.SpellTimer["Primalsurge"].IsReady() &&
                                KL.Spells["Aspect"].IsReady())
                                KL.Spells["Aspect"].Cast();
                        }
                    }             
                }            
            }

            #endregion
        }

        internal static void Orb(Obj_AI_Base target)
        {
            if (target != null && target.IsHPBarRendered && target.IsEnemy)
            {
                Orbwalking.Orbwalk(target, Game.CursorPos);
            }
        }

        internal static void Combo()
        {
            var solo = Root.Item("pstyle").GetValue<StringList>().SelectedIndex == 0;

            if (!Player.Spellbook.IsAutoAttacking)
            {
                CM.CastJavelin(solo ? Target : TargetSelector.GetTarget(KL.Spells["Javelin"].Range, TargetSelector.DamageType.Magical), "co");
                CM.SwitchForm(solo ? Target : TargetSelector.GetTarget(KL.Spells["Javelin"].Range, TargetSelector.DamageType.Magical), "co");
            }

            if (!Root.Item("ndhwsp").GetValue<bool>())
            {
                CM.CastBushwhack(solo ? Target : TargetSelector.GetTarget(KL.Spells["Bushwhack"].Range, TargetSelector.DamageType.Magical), "co");
            }

            CM.CastTakedown(solo ? Target : TargetSelector.GetTarget(KL.Spells["Takedown"].Range, TargetSelector.DamageType.Magical), "co");
            CM.CastPounce(solo ? Target : TargetSelector.GetTarget(KL.Spells["ExPounce"].Range, TargetSelector.DamageType.Magical), "co");
            CM.CastSwipe(solo ? Target : TargetSelector.GetTarget(KL.Spells["Swipe"].Range, TargetSelector.DamageType.Magical), "co");
        }

        internal static void Combo2()
        {
            var target = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Distance(Player.ServerPosition) <= 600 
                        && x.IsEnemy && x.IsHPBarRendered
                        && !MinionManager.IsWard(x)).OrderByDescending(x => x.MaxHealth).FirstOrDefault();

            Orb(target);

            if (target == null)
            {
                return;
            }

            if (Utils.GameTimeTickCount - KL.LastR >= 500 - Game.Ping)
            {
                if (!KL.CanUse(KL.Spells["Javelin"], true, "jg") && KL.CanUse(KL.Spells["Swipe"], false, "jg"))
                {
                    if (KL.CatForm() && target.IsValidTarget(KL.Spells["Swipe"].Range))
                    {
                        KL.Spells["Swipe"].Cast(target.ServerPosition);
                    }
                }

                if (!KL.CanUse(KL.Spells["Javelin"], true, "jg") &&
                    KL.CanUse(KL.Spells["Bushwhack"], false, "jg"))
                {
                    if (!KL.CatForm() && target.IsValidTarget(KL.Spells["Bushwhack"].Range) && KL.Player.ManaPercent > 40)
                    {
                        KL.Spells["Bushwhack"].Cast(target.ServerPosition);
                    }
                }

                if (!KL.CanUse(KL.Spells["Javelin"], true, "jg") && KL.CanUse(KL.Spells["Pounce"], false, "jg"))
                {
                    var r = target.IsHunted() ? KL.Spells["ExPounce"].Range : KL.Spells["Pounce"].Range;
                    if (KL.CatForm() && target.IsValidTarget(r))
                    {
                        KL.Spells["Pounce"].Cast(target.ServerPosition);
                    }
                }
            }

            if (KL.Spells["Takedown"].Level > 0 && KL.SpellTimer["Takedown"].IsReady() && !KL.CatForm())
            {
                if (KL.Spells["Aspect"].IsReady())
                {
                    KL.Spells["Aspect"].Cast();
                }
            }

            if (KL.Spells["Javelin"].Level > 0 && !KL.SpellTimer["Javelin"].IsReady() && !KL.CatForm())
            {
                if (KL.Spells["Aspect"].IsReady())
                {
                    KL.Spells["Aspect"].Cast();
                }
            }
        }

        internal static void Harass()
        {
            CM.CastJavelin(TargetSelector.GetTarget(KL.Spells["Javelin"].Range, TargetSelector.DamageType.Magical), "ha");
            CM.CastTakedown(TargetSelector.GetTarget(KL.Spells["Takedown"].Range, TargetSelector.DamageType.Magical), "ha");
            CM.CastSwipe(TargetSelector.GetTarget(KL.Spells["Swipe"].Range, TargetSelector.DamageType.Magical), "ha");
            CM.SwitchForm(TargetSelector.GetTarget(KL.Spells["Javelin"].Range, TargetSelector.DamageType.Magical), "ha");
        }

        internal static bool m;
        internal static void Clear()
        {
            var minions = MinionManager.GetMinions(Player.ServerPosition, 
                750f, MinionTypes.All, MinionTeam.All, MinionOrderTypes.MaxHealth);

            m = minions.Any(KL.IsJungleMinion);

            foreach (var unit in minions.OrderByDescending(KL.IsJungleMinion))
            {
                switch (unit.Team)
                {
                    case GameObjectTeam.Neutral:
                        if (!unit.Name.Contains("Mini"))
                        {
                            CM.CastJavelin(unit, "jg");
                            CM.CastBushwhack(unit, "jg");
                        }

                        CM.CastPounce(unit, "jg");
                        CM.CastTakedown(unit, "jg");
                        CM.CastSwipe(unit, "jg");

                        if (unit.PassiveRooted() && Root.Item("jgaacount").GetValue<KeyBind>().Active &&
                            Player.Distance(unit.ServerPosition) > 450)
                        {
                            return;
                        }

                        CM.SwitchForm(unit, "jg");
                        break;
                    default:
                        if (unit.Team != Player.Team && unit.Team != GameObjectTeam.Neutral)
                        {
                            CM.CastJavelin(unit, "wc");
                            CM.CastPounce(unit, "wc");
                            CM.CastBushwhack(unit, "wc");
                            CM.CastTakedown(unit, "wc");
                            CM.CastSwipe(unit, "wc");
                            CM.SwitchForm(unit, "wc");
                        }
                        break;
                }
            }

        }

        #region Walljumper @Hellsing
        internal static void Flee()
        {
            if (!KL.CatForm() && KL.Spells["Aspect"].IsReady())
            {
                if (KL.SpellTimer["Pounce"].IsReady())
                    KL.Spells["Aspect"].Cast();
            }

            var wallCheck = KL.GetFirstWallPoint(KL.Player.Position, Game.CursorPos);

            if (wallCheck != null)
                wallCheck = KL.GetFirstWallPoint((Vector3) wallCheck, Game.CursorPos, 5);

            var movePosition = wallCheck != null ? (Vector3) wallCheck : Game.CursorPos;

            var tempGrid = NavMesh.WorldToGrid(movePosition.X, movePosition.Y);
            var fleeTargetPosition = NavMesh.GridToWorld((short) tempGrid.X, (short)tempGrid.Y);

            Obj_AI_Base target = null;

            var wallJumpPossible = false;

            if (KL.CatForm() && KL.SpellTimer["Pounce"].IsReady() && wallCheck != null)
            {
                var wallPosition = movePosition;

                var direction = (Game.CursorPos.To2D() - wallPosition.To2D()).Normalized();
                float maxAngle = 80f;
                float step = maxAngle / 20;
                float currentAngle = 0;
                float currentStep = 0;
                bool jumpTriggered = false;

                while (true)
                {
                    if (currentStep > maxAngle && currentAngle < 0)
                        break;

                    if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                    {
                        currentAngle = (currentStep) * (float)Math.PI / 180;
                        currentStep += step;
                    }

                    else if (currentAngle > 0)
                        currentAngle = -currentAngle;

                    Vector3 checkPoint;

                    if (currentStep == 0)
                    {
                        currentStep = step;
                        checkPoint = wallPosition + KL.Spells["Pounce"].Range * direction.To3D();
                    }

                    else
                        checkPoint = wallPosition + KL.Spells["Pounce"].Range * direction.Rotated(currentAngle).To3D();

                    if (checkPoint.IsWall()) 
                        continue;

                    wallCheck = KL.GetFirstWallPoint(checkPoint, wallPosition);

                    if (wallCheck == null) 
                        continue;

                    var wallPositionOpposite =  (Vector3) KL.GetFirstWallPoint((Vector3)wallCheck, wallPosition, 5);

                    if (KL.Player.GetPath(wallPositionOpposite).ToList().To2D().PathLength() -
                        KL.Player.Distance(wallPositionOpposite) > 200)
                    {
                        if (KL.Player.Distance(wallPositionOpposite) < KL.Spells["Pounce"].Range - KL.Player.BoundingRadius / 2)
                        {
                            KL.Spells["Pounce"].Cast(wallPositionOpposite);
                            jumpTriggered = true;
                            break;
                        }

                        else
                            wallJumpPossible = true;
                    }

                    else
                    {
                        Render.Circle.DrawCircle(Game.CursorPos, 35, Color.Red, 2);
                    }
                }

                if (!jumpTriggered)
                {
                    Orbwalking.Orbwalk(target, Game.CursorPos, 90f, 35f);
                }
            }

            else
            {
                Orbwalking.Orbwalk(target, Game.CursorPos, 90f, 35f);
                if (KL.CatForm() && KL.SpellTimer["Pounce"].IsReady())
                    KL.Spells["Pounce"].Cast(Game.CursorPos);
            }
        }

        #endregion
    }
}
