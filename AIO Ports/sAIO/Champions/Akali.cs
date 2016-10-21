using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Color = System.Drawing.Color;
using sAIO.Core;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace sAIO.Champions
{
    class AkaliA : Helper
    {
        private static int lastR = 0, rState = 0;
        private static Items.Item Hex;
        private static Items.Item Cutlass;
        public AkaliA()
        {
            Akali_OnLoad();
        }
        private static void Akali_OnLoad()
        {
            if (player.ChampionName != "Akali")
                return;

            Ignite = player.GetSpellSlot("summonerdot");
            Hex = new Items.Item(3146, 700);
            Cutlass = new Items.Item(3144, 450);
            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 290f);
            R = new Spell(SpellSlot.R, 800f);

            menu.AddSubMenu(new Menu("Combo", "Combo"));

            CreateMenuBool("Combo", "Combo.Q", "Use Q", true);
            CreateMenuBool("Combo", "Combo.W", "Use W", true);
            CreateMenuBool("Combo", "Combo.E", "Use E", true);
            CreateMenuBool("Combo", "Combo.R", "Use R", true);
            CreateMenuBool("Combo", "Combo.Ignite", "Use Ignite", true);
            menu.SubMenu("Combo").AddItem(new MenuItem("Combo.RDelay", "R delay").SetValue(new Slider(0, 0, 2000)));


            menu.AddSubMenu(new Menu("Harass", "Harass"));

            CreateMenuBool("Harass", "Harass.Q", "Use Q", true);
            CreateMenuBool("Harass", "Harass.E", "Use E", true);
            


            menu.AddSubMenu(new Menu("Gap closer", "GC"));
            CreateMenuBool("GC", "GC.W", "Use W", true);

            menu.AddSubMenu(new Menu("Kill Steal", "KS"));
            CreateMenuBool("KS", "KS.Q", "Use Q", true);            
            CreateMenuBool("KS", "KS.E", "Use E", false);
            CreateMenuBool("KS", "KS.R", "Use R", false);

            menu.AddSubMenu(new Menu("Farm", "Farm"));
            CreateMenuBool("Farm", "Farm.Q", "Use Q", true);
            CreateMenuBool("Farm", "Farm.E", "Use E", true);

            menu.AddSubMenu(new Menu("Lane Clean", "LC"));
            CreateMenuBool("LC", "LC.Q", "Use Q", true);
            CreateMenuBool("LC", "LC.E", "Use E", true);

            menu.AddSubMenu(new Menu("Jungle Clean", "JC"));
            CreateMenuBool("JC", "JC.Q", "Use Q", true);
            CreateMenuBool("JC", "JC.E", "Use E", true);
            CreateMenuBool("JC", "JC.R", "Use R", true);
            CreateMenuKeyBind("JC", "JC.Key", "Key", 'G', KeyBindType.Press);

            menu.AddSubMenu(new Menu("Drawing", "Draw"));
            CreateMenuBool("Draw", "Draw.Q", "Draw Q", true);
            CreateMenuBool("Draw", "Draw.E", "Draw E", true);
            CreateMenuBool("Draw", "Draw.R", "Draw R", true);
            CreateMenuBool("Draw", "Draw.CBDamage", "Draw Combo Damage", true);
            menu.SubMenu("Draw").AddItem(new MenuItem("Draw.DrawColor", "Fill color").SetValue(new Circle(true, Color.FromArgb(204, 255, 0, 1))));

            DrawDamage.DamageToUnit = GetComboDamage;
            DrawDamage.Enabled = GetValueMenuBool("Draw.CBDamage");
            DrawDamage.Fill = menu.Item("Draw.DrawColor").GetValue<Circle>().Active;
            DrawDamage.FillColor = menu.Item("Draw.DrawColor").GetValue<Circle>().Color;

            menu.Item("Draw.CBDamage").ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
            };

            menu.Item("Draw.DrawColor").ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
            menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            //Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            new AssassinManager();
            Chat.Print("sAIO: " + player.ChampionName + " loaded");

        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (W.IsReady() && GetValueMenuBool("GC.W"))
                W.Cast(player.Position);
        }

        private static float GetComboDamage(Obj_AI_Base vTarget)
        {
            var damage = 0d;

            if (Q.IsReady())
                damage += player.GetSpellDamage(vTarget, SpellSlot.Q) +
                                player.GetSpellDamage(vTarget, SpellSlot.Q, 1);
            if (E.IsReady())
                damage += player.GetSpellDamage(vTarget, SpellSlot.E);

            if (R.IsReady())
               damage += player.GetSpellDamage(vTarget, SpellSlot.R) * R.Instance.Ammo;

            if (Items.CanUseItem(3146))
                damage += player.GetItemDamage(vTarget, Damage.DamageItems.Hexgun);

            if (Ignite != SpellSlot.Unknown &&
                player.Spellbook.CanUseSpell(Ignite) == SpellState.Ready)
                damage += player.GetSummonerSpellDamage(vTarget, Damage.SummonerSpell.Ignite);

            return (float)damage;
        }
        static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if (args.SData.Name == R.Instance.SData.Name)
            {
                lastR = Utils.GameTimeTickCount;
                rState++;
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (GetValueMenuBool("Draw.Q"))
                Drawing.DrawCircle(player.Position, Q.Range, Color.Green);

            if (GetValueMenuBool("Draw.E"))
                Drawing.DrawCircle(player.Position, E.Range, Color.Blue);

            if (GetValueMenuBool("Draw.R"))
                Drawing.DrawCircle(player.Position, Q.Range, Color.Red);
        }

        static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            throw new NotImplementedException();
        }

        static void Game_OnUpdate(EventArgs args)
        {
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo: Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed: Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit: Farm();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear: Clean();
                    break;
            }

            if (GetValueMenuKeyBind("JC.Key"))
                JungleClean();
        }


        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if (Q.IsReady() && GetValueMenuBool("Combo.Q") && target.IsValidTarget(Q.Range))
                Q.CastOnUnit(target);

            if (R.IsReady() && GetValueMenuBool("Combo.R") && target.IsValidTarget(R.Range)
                && HasBuff(target, "AkaliMota") && Utils.GameTimeTickCount - lastR > GetValueMenuSlider("Combo.RDelay"))
                R.CastOnUnit(target);

            if (W.IsReady() && GetValueMenuBool("Combo.W") && !player.IsDashing())
                W.Cast(player.Position);

            if (E.IsReady() && GetValueMenuBool("Combo.E") && target.IsValidTarget(E.Range))
                E.Cast();

            if (Hex.IsReady() && Hex.IsOwned() && target.IsValidTarget(Hex.Range))
                Hex.Cast(target);

            if (Cutlass.IsReady() && Cutlass.IsOwned() && target.IsValidTarget(Cutlass.Range))
                Cutlass.Cast(target);

            if (Ignite.IsReady() && GetValueMenuBool("Combo.Ignite") && rState == 3)
                player.Spellbook.CastSpell(Ignite, target);
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if (Q.IsReady() && GetValueMenuBool("Harass.Q") && target.IsValidTarget(Q.Range))
                Q.CastOnUnit(target);           

            if (E.IsReady() && GetValueMenuBool("Harass.E") && target.IsValidTarget(E.Range))
                E.Cast();

        }

        private static void Farm()
        {
            var minions = MinionManager.GetMinions(player.Position, Q.Range);
            
            foreach(var minion in minions)
            {
                if (minion.Health < Q.GetDamage(minion) * 0.9 && Q.IsReady() && GetValueMenuBool("Farm.Q") && minion.IsValidTarget(Q.Range))
                    Q.CastOnUnit(minion);
            }

            foreach (var minion in minions)
            {
                if (minion.Health < E.GetDamage(minion) * 0.9 && E.IsReady() && GetValueMenuBool("Farm.E") && minion.Distance(player.Position) < E.Range)
                    E.Cast();
            }

        }

        private static void Clean()
        {
            var minions = MinionManager.GetMinions(player.Position, Q.Range);

            foreach (var minion in minions)
            {
                if (Q.IsReady() && GetValueMenuBool("LC.Q") && minion.IsValidTarget(Q.Range))
                    Q.CastOnUnit(minion);

                if (E.IsReady() && GetValueMenuBool("LC.E") && minion.Distance(player.Position) < E.Range)
                    E.Cast();
            }           
        }

        private static void JungleClean()
        {
            var mons = MinionManager.GetMinions(player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mons.Count < 0)
                return;

            var kappa = mons[0];

            if (Q.IsReady() && GetValueMenuBool("JC.Q") && kappa.IsValidTarget(Q.Range))
                Q.CastOnUnit(kappa);

            if (E.IsReady() && GetValueMenuBool("JC.E") && kappa.Distance(player.Position) < E.Range)
                E.Cast();
        }

        private static void KillSteal()
        {
            foreach(var hero in HeroManager.Enemies)
            {
                if (Q.IsReady() && GetValueMenuBool("KS.Q") && hero.Health < Q.GetDamage(hero) * 0.9 && hero.IsValidTarget(Q.Range))
                    Q.CastOnUnit(hero);

                if (E.IsReady() && GetValueMenuBool("KS.E") && hero.IsValidTarget(E.Range) && hero.Health < E.GetDamage(hero) * 0.9)
                    E.Cast();

                if (R.IsReady() && GetValueMenuBool("KS.R") && hero.Health < R.GetDamage(hero) * 0.9 && hero.IsValidTarget(R.Range))
                    R.CastOnUnit(hero);
            }
        }
    }
}
