using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Drawing;
using Color = SharpDX.Color;
//using SebbyLib;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SNTwitch
{
    
    class Program
    {
        private static AIHeroClient p { get { return ObjectManager.Player; } }
        private static Spell Q, W, E, R, useYumu, Recall;
        private static LeagueSharp.Common.Orbwalking.Orbwalker orb;
       // private static Orbwalking.Orbwalker sebbyOrb;
        private static Menu m;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if(p.ChampionName != "Twitch")
                return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 1200, TargetSelector.DamageType.Physical);
            R = new Spell(SpellSlot.R);
            useYumu = new Spell(SpellSlot.Q, 1500);
            Recall = new Spell(SpellSlot.Recall);

            W.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);
         

            m = new Menu("SNTwitch by SnRolls", "Twitch", true);

           // if (m.Item("useSebbyOrb").IsActive())
//      sebbyOrb = new Orbwalking.Orbwalker(m.SubMenu("Orbwalker"));
           // else
                orb = new LeagueSharp.Common.Orbwalking.Orbwalker(m.SubMenu("Orbwalker"));

            TargetSelector.AddToMenu(m.SubMenu("Target Selector"));

            m.AddSubMenu(new Menu("Drawings", "drawings", false));
            m.SubMenu("drawings").AddItem(new MenuItem("drawW", "Draw W").SetValue(true));
            m.SubMenu("drawings").AddItem(new MenuItem("drawE", "Draw E").SetValue(true));

            m.AddSubMenu(new Menu("Harass", "harass", false));
            m.SubMenu("harass").AddItem(new MenuItem("hUseE", "Use E on X stacks").SetValue<Slider>(new Slider(6, 0, 6)));
            m.SubMenu("harass").AddItem(new MenuItem("hActive", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));

            m.AddSubMenu(new Menu("Combo", "combo", false));
            m.SubMenu("combo").AddItem(new MenuItem("cUseQ", "Use Q").SetValue(true));
            m.SubMenu("combo").AddItem(new MenuItem("cUseW", "Use W").SetValue(true));
            m.SubMenu("combo").AddItem(new MenuItem("cAutoE", "Auto use E on max stacks").SetValue(true));
            m.SubMenu("combo").AddItem(new MenuItem("au", "Auto Ult").SetValue(true));
            m.SubMenu("combo").AddItem(new MenuItem("cActive", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            m.SubMenu("combo").AddItem(new MenuItem("cMode", "Combo Mode").SetValue(new StringList(new [] {"Normal", "Assassination"})));

            m.AddSubMenu(new Menu("Lane Clear", "laneclear", false));
            m.SubMenu("laneclear").AddItem(new MenuItem("lcActive", "Lane Clear").SetValue(new KeyBind('V', KeyBindType.Press)));

            m.AddSubMenu(new Menu("Last Hit", "lasthit", false));
            m.SubMenu("lasthit").AddItem(new MenuItem("lhActive", "Last Hit").SetValue(new KeyBind('X', KeyBindType.Press)));

            m.AddSubMenu(new Menu("Misc", "misc", false));
                m.SubMenu("misc").AddItem(new MenuItem("Emobs", "Try to KS Epic Monsters with E").SetValue(new StringList(new [] {"Baron + Dragon", "None" })));
            
            m.SubMenu("misc").AddItem(new MenuItem("eKS", "E Auto-KillSteal").SetValue(true));
            m.SubMenu("misc").AddItem(new MenuItem("resetPassive", "Use W to reset passive on enemy").SetValue(true));
            m.SubMenu("misc").AddItem(new MenuItem("deathE", "Auto E below health %").SetValue<Slider>(new Slider(5, 1, 100)));
            m.SubMenu("misc").AddItem(new MenuItem("stealthRecall", "Stealth Recall").SetValue(new KeyBind('T', KeyBindType.Press)));
      //      m.SubMenu("misc").AddItem(new MenuItem("useSebbyOrb", "Use Sebby Orbwalker(MUST RELOAD L# for it to change)").SetValue(false));
           
            m.AddSubMenu(new Menu("Credits", "credits", false));
            m.SubMenu("credits").AddItem(new MenuItem("c1", "iMeh for KS Epic monsters"));
            m.SubMenu("credits").AddItem(new MenuItem("c2", "SnRolls for coding the rest"));
            m.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;

            CustomDamageIndicator.Initialize(GetDamage);

            
        }

        private static float GetDamage(AIHeroClient target)
        {
            return E.GetDamage(target);
        }

        public static void OnUpdate(EventArgs args)
        {
            if (p.IsDead) return;

            if (m.Item("deathE").IsActive() && p.HealthPercent < 5)
                E.Cast();

            if(m.Item("eKS").IsActive())
            CastEKS();

            if(m.Item("stealthRecall").IsActive())
            {
                CastQ();
                Recall.Cast();
            }
            if (m.Item("resetPassive").IsActive())
            {
                foreach (
                   var enemy in
                      ObjectManager.Get<AIHeroClient>()
                      .Where(enemy => enemy.IsValidTarget(W.Range) && enemy.GetBuffCount("twitchdeadlyvenom") > 5)
                          )
                {
                    if ((enemy.GetBuff("twitchdeadlyvenom").EndTime - 1 < Game.Time))
                        W.Cast(enemy);

                }
            }

          
                if (orb.ActiveMode == LeagueSharp.Common.Orbwalking.OrbwalkingMode.Combo)
                {
                    switch (m.Item("cMode").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            var Ntarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                            if (Ntarget.IsValidTarget(W.Range) && m.Item("cUseQ").IsActive())
                            {


                                Q.Cast();
                            }

                            Items.UseItem(3142);
                            if (m.Item("au").IsActive())
                                R.Cast();

                            if (Ntarget != null && Ntarget.Type == p.Type &&
                        Ntarget.ServerPosition.Distance(p.ServerPosition) < 450)
                            {
                                var hasCutGlass = Items.HasItem(3144);
                                var hasBotrk = Items.HasItem(3153);

                                if (hasBotrk || hasCutGlass)
                                {
                                    var itemId = hasCutGlass ? 3144 : 3153;
                                    var damage = p.GetItemDamage(Ntarget, Damage.DamageItems.Botrk);
                                    if (hasCutGlass || p.Health + damage < p.MaxHealth)
                                        Items.UseItem(itemId, Ntarget);
                                }


                            }

                            if (W.IsReady() && m.Item("cUseW").IsActive())
                            {
                                if (W.IsInRange(Ntarget))
                                    CastW();
                            }

                            if (E.IsReady() && m.Item("cAutoE").IsActive())
                            {
                                if (Ntarget.GetBuffCount("twitchdeadlyvenom") == 6 && E.IsInRange(Ntarget))
                                    E.Cast();
                            }
                            break;
                        case 1:
                            var Atarget = TargetSelector.GetTarget(2500, TargetSelector.DamageType.Physical);
                            if (Atarget.IsValidTarget(2500))
                            {
                                Q.Cast();
                            }

                            if (Atarget.IsValidTarget(800))
                            {
                                Items.UseItem(3142);
                            }

                            if (Atarget.IsValidTarget(700))
                            {
                                if (m.Item("au").IsActive())
                                    R.Cast();

                            }
                            if (Atarget != null && Atarget.Type == p.Type &&
                        Atarget.ServerPosition.Distance(p.ServerPosition) < 450)
                            {
                                var hasCutGlass = Items.HasItem(3144);
                                var hasBotrk = Items.HasItem(3153);

                                if (hasBotrk || hasCutGlass)
                                {
                                    var itemId = hasCutGlass ? 3144 : 3153;
                                    var damage = p.GetItemDamage(Atarget, Damage.DamageItems.Botrk);
                                    if (hasCutGlass || p.Health + damage < p.MaxHealth)
                                        Items.UseItem(itemId, Atarget);
                                }
                            }

                            if (W.IsReady())
                            {
                                if (W.IsInRange(Atarget))
                                {
                                    CastW();
                                }
                            }
                            break;
                    }

                }

            

          
                if (orb.ActiveMode == LeagueSharp.Common.Orbwalking.OrbwalkingMode.Mixed)
                {

                    if (m.Item("hUseE").GetValue<Slider>().Value > 0)
                    {
                        foreach (
                           var enemy in
                              ObjectManager.Get<AIHeroClient>()
                              .Where(enemy => enemy.IsValidTarget(E.Range) && enemy.GetBuffCount("twitchdeadlyvenom") == m.Item("hUseE").GetValue<Slider>().Value)
                                  )

                        {
                            if (!HasUndyingBuff(enemy))
                                E.Cast();

                        }
                    }
                }

        
                if (orb.ActiveMode != LeagueSharp.Common.Orbwalking.OrbwalkingMode.Combo)
                {
                    var minions = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly);
                    foreach (var mi in minions)
                    {
                        switch (m.Item("Emobs").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                if ((mi.BaseSkinName.Contains("Dragon") || mi.BaseSkinName.Contains("Baron")) && E.IsKillable(mi))
                                {
                                    E.Cast();
                                }
                                break;

                            case 1:
                                return;
                                break;
                        }
                    }
                }
            
            
            

        }

        public static void OnDraw(EventArgs args)
        {
            CustomDamageIndicator.DrawingColor = System.Drawing.Color.DarkSeaGreen;
            CustomDamageIndicator.Enabled = true;
            if (!p.IsDead && W.Level > 0 && W.IsReady() && m.Item("drawW").GetValue<bool>())
            {
                Render.Circle.DrawCircle(p.Position, W.Range, System.Drawing.Color.DarkGreen);
            }

            if (!p.IsDead && E.Level > 0 && E.IsReady() && m.Item("drawE").GetValue<bool>())
            {
                Render.Circle.DrawCircle(p.Position, E.Range, System.Drawing.Color.DarkGreen);
            }

            foreach (
                      var enemy in
                         ObjectManager.Get<AIHeroClient>()
                         .Where(enemy => enemy.GetBuffCount("twitchdeadlyvenom") > 0)
                             )
            {

                Drawing.DrawText(enemy.HPBarPosition.X+20, enemy.HPBarPosition.Y+50, System.Drawing.Color.Red, "Stacks: " + enemy.GetBuffCount("twitchdeadlyvenom"));
               
            }

        }

        public static void CastQ()
        {
            Q.Cast();
        }

        public static void CastEKS()
        {
            foreach (
                       var enemy in
                           ObjectManager.Get<AIHeroClient>()
                               .Where(enemy => enemy.IsValidTarget(E.Range) && E.IsKillable(enemy))
                       )
            {
                if(!HasUndyingBuff(enemy))
                E.Cast();
            }

        }
        public static void CastW()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.True);

            if(target.IsStunned || target.IsRooted || target.IsCharmed)
            {
                W.Cast(target);
            }
            else
            {
                W.CastIfHitchanceEquals(target, HitChance.High);
            }
        }


        public static bool HasUndyingBuff(AIHeroClient target)
        {
            // Tryndamere R
            if (target.ChampionName == "Tryndamere"
                && target.Buffs.Any(
                    b => b.Caster.NetworkId == target.NetworkId && b.IsValidBuff() && b.DisplayName == "Undying Rage"))
            {
                return true;
            }

            // Zilean R
            if (target.Buffs.Any(b => b.IsValidBuff() && b.DisplayName == "Chrono Shift"))
            {
                return true;
            }

            // Kayle R
            if (target.Buffs.Any(b => b.IsValidBuff() && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }

            // Poppy R
            if (target.ChampionName == "Poppy")
            {
                if (
                    HeroManager.Allies.Any(
                        o =>
                        !o.IsMe
                        && o.Buffs.Any(
                            b =>
                            b.Caster.NetworkId == target.NetworkId && b.IsValidBuff()
                            && b.DisplayName == "PoppyDITarget")))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
