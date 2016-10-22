using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace yol0Garen
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player;  } }
        private static Spell _Q = new Spell(SpellSlot.Q);
        private static Spell _W = new Spell(SpellSlot.W);
        private static Spell _E = new Spell(SpellSlot.E, 330);
        private static Spell _R = new Spell(SpellSlot.R, 400);

        private static Menu _menu;
        private static Orbwalking.Orbwalker _orbwalker;

        private static Items.Item _Tiamat = new Items.Item(3077, 185);
        private static Items.Item _Hydra = new Items.Item(3074, 185);
        private static Items.Item _Ghostblade = new Items.Item(3142);
        private static Items.Item _Bilgewater = new Items.Item(3144, 450);
        private static Items.Item _Botrk = new Items.Item(3153, 450);
        private static Items.Item _Hextech = new Items.Item(3146, 700);
        private static Items.Item _Randuins = new Items.Item(3143, 500);

        private static SpellSlot _Ignite;

        private static AIHeroClient _target;
        public static void Main()
        {
            OnLoad();
        }

        private static void OnLoad()
        {
            if (Player.ChampionName != "Garen")
                return;

            _menu = new Menu("yol0 Garen", "yol0Garen", true);
            _orbwalker = new Orbwalking.Orbwalker(_menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker")));
            _menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(_menu.SubMenu("Target Selector"));

            _menu.AddSubMenu(new Menu("Combo Settings", "Combo"));
            _menu.AddSubMenu(new Menu("Lane Clear Settings", "LaneClear"));
            _menu.AddSubMenu(new Menu("Misc Settings", "Misc"));
            _menu.AddSubMenu(new Menu("Item Settings", "Item"));
            _menu.AddSubMenu(new Menu("KS Settings", "KS"));
            _menu.AddSubMenu(new Menu("Draw Settings", "Drawing"));

            _menu.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            _menu.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            _menu.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            _menu.SubMenu("Combo").AddItem(new MenuItem("UseIgnite", "Use Ignite if Killable").SetValue(true));
            _menu.SubMenu("Combo").AddItem(new MenuItem("UseItems", "Use Items").SetValue(true));
            _menu.SubMenu("Combo").AddItem(new MenuItem("yol0Mode", "Stick to Target").SetValue(true));

            _menu.SubMenu("LaneClear").AddItem(new MenuItem("UseE", "Use Smart E Positioning").SetValue(true));

            _menu.SubMenu("Misc").AddItem(new MenuItem("Interrupt", "Interrupt Spells with Q").SetValue(true));

            _menu.SubMenu("Item").AddItem(new MenuItem("UseTiamat", "Tiamat").SetValue(true));
            _menu.SubMenu("Item").AddItem(new MenuItem("UseHydra", "Ravenous Hydra").SetValue(true));
            _menu.SubMenu("Item").AddItem(new MenuItem("UseGhostblade", "Ghostblade").SetValue(true));
            _menu.SubMenu("Item").AddItem(new MenuItem("UseBilgewater", "Bilgewater Cutlass").SetValue(true));
            _menu.SubMenu("Item").AddItem(new MenuItem("UseBlade", "Blade of the Ruined King").SetValue(true));
            _menu.SubMenu("Item").AddItem(new MenuItem("UseHextech", "Hextech Gunblade").SetValue(true));
            _menu.SubMenu("Item").AddItem(new MenuItem("UseRanduins", "Randuin's Omen").SetValue(true));

            _menu.SubMenu("KS").AddItem(new MenuItem("KsQ", "KS with Q").SetValue(false));
            _menu.SubMenu("KS").AddItem(new MenuItem("KsE", "KS with E").SetValue(false));
            _menu.SubMenu("KS").AddItem(new MenuItem("KsR", "KS with R").SetValue(true));
            _menu.SubMenu("KS").AddItem(new MenuItem("KsItems", "KS with Items").SetValue(true));
            _menu.SubMenu("KS").AddItem(new MenuItem("KsI", "KS with Ignite").SetValue(true));

            _menu.SubMenu("Drawing").AddItem(new MenuItem("drawTarget", "Draw Current Target").SetValue(new Circle(true, Color.Red)));
            _menu.SubMenu("Drawing").AddItem(new MenuItem("drawERange", "Draw E Range").SetValue(new Circle(true, Color.Green)));
            _menu.SubMenu("Drawing").AddItem(new MenuItem("drawRRange", "Draw R Range").SetValue(new Circle(true, Color.Cyan)));
            _menu.SubMenu("Drawing").AddItem(new MenuItem("drawKillability", "Draw Killable").SetValue(true));
            _menu.SubMenu("Drawing").AddItem(new MenuItem("drawDamage", "Draw Damage to Target").SetValue(true));

            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit += GetComboDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Color = Color.GreenYellow;
            
            _menu.AddToMainMenu();

            _Ignite = Player.GetSpellSlot("summonerdot");

            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Interrupter2.OnInterruptableTarget += OnInterruptibleTarget;
            Orbwalking.AfterAttack += AfterAttack;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = _menu.SubMenu("Drawing").Item("drawDamage").GetValue<bool>();
            _target = TargetSelector.GetTarget(500, TargetSelector.DamageType.Physical);

            if (_orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || _orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                _orbwalker.SetOrbwalkingPoint(new Vector3());

            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo(TargetSelector.GetTarget(500, TargetSelector.DamageType.Physical));

            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                LaneClear();

            KillSecure();
        }

        private static void OnDraw(EventArgs args)
        {
            if (_menu.SubMenu("Drawing").Item("drawTarget").GetValue<Circle>().Active && _target.IsValidTarget())
            {
                Render.Circle.DrawCircle(_target.Position, _target.BoundingRadius + 10, _menu.SubMenu("Drawing").Item("drawTarget").GetValue<Circle>().Color);
                Render.Circle.DrawCircle(_target.Position, _target.BoundingRadius + 25, _menu.SubMenu("Drawing").Item("drawTarget").GetValue<Circle>().Color);
                Render.Circle.DrawCircle(_target.Position, _target.BoundingRadius + 40, _menu.SubMenu("Drawing").Item("drawTarget").GetValue<Circle>().Color);
            }

            if (_menu.SubMenu("Drawing").Item("drawERange").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(Player.Position, _E.Range, _menu.SubMenu("Drawing").Item("drawERange").GetValue<Circle>().Color);
            }

            if (_menu.SubMenu("Drawing").Item("drawRRange").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(Player.Position, _E.Range, _menu.SubMenu("Drawing").Item("drawRRange").GetValue<Circle>().Color);
            }

            if (_menu.SubMenu("Drawing").Item("drawKillability").GetValue<bool>())
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(unit => unit.IsEnemy && unit.IsVisible && !unit.IsDead))
                {
                    var wts = Drawing.WorldToScreen(enemy.Position);
                    if (GetComboDamage(enemy) > enemy.Health)
                    {
                        Drawing.DrawText(wts[0] - 20, wts[1] + 20, Color.Red, "Killable!");
                    }
                    else
                    {
                        Drawing.DrawText(wts[0] - 20, wts[1] + 20, Color.White, "Not Killable");
                    }
                }
            }
        }

        private static void Combo(AIHeroClient target)
        {
            if (!target.IsValidTarget())
                return;

            var QActive = false;
            var EActive = false;
            foreach (var buff in Player.Buffs)
            {
                if (buff.Name == "GarenQ")
                    QActive = true;
                if (buff.Name == "GarenE")
                    EActive = true;
            }
            if (!QActive && _menu.SubMenu("Combo").Item("UseQ").GetValue<bool>() && _Q.IsReady())
            {
                _Q.Cast();
                return;
            }

            if (!QActive && !EActive && _menu.SubMenu("Combo").Item("UseE").GetValue<bool>() && _E.IsReady() && target.IsValidTarget(_E.Range))
            {
                _E.Cast();
            }

            if (_menu.SubMenu("Combo").Item("yol0Mode").GetValue<bool>() && QActive)
            {
                _orbwalker.SetMovement(false);
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }

            if (_menu.SubMenu("Combo").Item("yol0Mode").GetValue<bool>() && EActive)
            {
                _orbwalker.SetMovement(false);
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target.Path[0]);
            }

            if (!QActive && !EActive && !Orbwalking.Move)
            {
                _orbwalker.SetMovement(true);
            }

            if (_menu.SubMenu("Combo").Item("UseItems").GetValue<bool>())
            {
                if (_Tiamat.IsReady() && target.IsValidTarget(_Tiamat.Range))
                {
                    _Tiamat.Cast();
                }
                if (_Hydra.IsReady() && target.IsValidTarget(_Hydra.Range))
                {
                    _Hydra.Cast();
                }
                if (_Bilgewater.IsReady() && target.IsValidTarget(_Bilgewater.Range))
                {
                    _Bilgewater.Cast(target);
                }
                if (_Botrk.IsReady() && target.IsValidTarget(_Botrk.Range))
                {
                    _Botrk.Cast(target);
                }
                if (_Hextech.IsReady() && target.IsValidTarget(_Hextech.Range))
                {
                    _Hextech.Cast(target);
                }
                if (_Ghostblade.IsReady() && target.IsValidTarget(600))
                {
                    _Ghostblade.Cast();
                }
                if (_Randuins.IsReady() && target.IsValidTarget(_Randuins.Range))
                {
                    _Randuins.Cast();
                }
            }

            if (_menu.SubMenu("Combo").Item("UseIgnite").GetValue<bool>() && _Ignite != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(_Ignite) == SpellState.Ready)
            {
                var comboDmg = GetComboDamage(target);
                if (comboDmg >= target.Health && comboDmg - Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) < target.Health)
                {
                    Player.Spellbook.CastSpell(_Ignite, target);
                }
            }
        }

        private static void LaneClear()
        {
            var loc = GetFarmLocation();
            var EActive = false;
            foreach (var buff in Player.Buffs)
            {
                if (buff.Name == "GarenE")
                    EActive = true;
            }
            if (_E.IsReady() && !EActive && _menu.SubMenu("LaneClear").Item("UseE").GetValue<bool>() && loc.IsValid())
            {
                _E.Cast();
                _orbwalker.SetOrbwalkingPoint(loc.To3D());
            }
            if (!loc.IsValid())
                _orbwalker.SetOrbwalkingPoint(new Vector3());
        }

        private static void OnInterruptibleTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Player.Distance(sender) < 500 && _menu.SubMenu("Misc").Item("Interrupt").GetValue<bool>())
            {
                if (_Q.IsReady())
                {
                    _Q.Cast();
                    _orbwalker.SetMovement(false);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
                }
            }
        }
        
        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!Orbwalking.Move)
                _orbwalker.SetMovement(true);

            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && _menu.SubMenu("Combo").Item("UseQ").GetValue<bool>() && _Q.IsReady())
            {
                _Q.Cast();
            }

            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && _menu.SubMenu("Harass").Item("UseQ").GetValue<bool>() && _Q.IsReady())
            {
                _Q.Cast();
            }
        }

        private static void KillSecure()
        {
            var QActive = false;
            var EActive = false;
            foreach (var buff in Player.Buffs)
            {
                if (buff.Name == "GarenQ")
                    QActive = true;

                if (buff.Name == "GarenE")
                    EActive = true;
            }

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(unit => unit.IsEnemy))
            {
                if (_menu.SubMenu("KS").Item("KsR").GetValue<bool>() && _R.IsReady() && _R.IsInRange(enemy) && Player.GetSpellDamage(enemy, SpellSlot.R) > enemy.Health)
                {
                    _R.Cast(enemy);
                    return;
                }

                if (_menu.SubMenu("KS").Item("KsQ").GetValue<bool>() && !EActive &&_Q.IsReady() && Player.Distance(enemy) <= Orbwalking.GetRealAutoAttackRange(enemy) && Player.MoveSpeed >= enemy.MoveSpeed && Player.GetSpellDamage(enemy, SpellSlot.Q) > enemy.Health)
                {
                    _Q.Cast();
                    _orbwalker.SetMovement(false);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, enemy);
                    return;
                }

                if (_menu.SubMenu("KS").Item("KsE").GetValue<bool>() && !QActive && !EActive && _E.IsReady() && Player.Distance(enemy) <= _E.Range && Player.GetSpellDamage(enemy, SpellSlot.E) > enemy.Health)
                {
                    _E.Cast();
                    _orbwalker.SetOrbwalkingPoint(enemy.Path[0]);
                    return;
                }


                if (_menu.SubMenu("KS").Item("KsItems").GetValue<bool>())
                {
                    if (_Tiamat.IsReady() && enemy.IsValidTarget(_Tiamat.Range) && Player.GetItemDamage(enemy, Damage.DamageItems.Tiamat) > enemy.Health)
                    {
                        _Tiamat.Cast();
                    }
                    if (_Hydra.IsReady() && enemy.IsValidTarget(_Hydra.Range) && Player.GetItemDamage(enemy, Damage.DamageItems.Hydra) > enemy.Health)
                    {
                        _Hydra.Cast();
                    }
                    if (_Bilgewater.IsReady() && enemy.IsValidTarget(_Bilgewater.Range) && Player.GetItemDamage(enemy, Damage.DamageItems.Bilgewater) > enemy.Health)
                    {
                        _Bilgewater.Cast(enemy);
                    }
                    if (_Botrk.IsReady() && enemy.IsValidTarget(_Botrk.Range) && Player.GetItemDamage(enemy, Damage.DamageItems.Botrk) > enemy.Health)
                    {
                        _Botrk.Cast(enemy);
                    }
                    if (_Hextech.IsReady() && enemy.IsValidTarget(_Hextech.Range) && Player.GetItemDamage(enemy, Damage.DamageItems.Hexgun) > enemy.Health)
                    {
                        _Hextech.Cast(enemy);
                    }
                    return;
                }

                if (_menu.SubMenu("KS").Item("KsIgnite").GetValue<bool>())
                {
                    if (Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite) > enemy.Health && enemy.IsValidTarget(600))
                    {
                        Player.Spellbook.CastSpell(_Ignite, enemy);
                    }
                }
            }
        }

        private static float GetComboDamage(Obj_AI_Base target)
        {
            float QDmg = 0.0f;
            float EDmg = 0.0f;
            float RDmg = 0.0f;
            float TiamatDmg = 0.0f;
            float HydraDmg = 0.0f;
            float BotrkDmg = 0.0f;
            float BilgeDmg = 0.0f;
            float HxgDmg = 0.0f;
            float IgniteDmg = 0.0f;

            if (_Q.IsReady())
                QDmg = (float)Player.GetSpellDamage(target, SpellSlot.Q);
            if (_E.IsReady())
                EDmg = (float)Player.GetSpellDamage(target, SpellSlot.E) * 3;
            if (_R.IsReady())
                RDmg = (float)Player.GetSpellDamage(target, SpellSlot.R);

            if (_menu.SubMenu("Item").Item("UseTiamat").GetValue<bool>() && _Tiamat.IsReady())
                TiamatDmg = (float)Player.GetItemDamage(target, Damage.DamageItems.Tiamat);
            if (_menu.SubMenu("Item").Item("UseHydra").GetValue<bool>() && _Hydra.IsReady())
                HydraDmg = (float)Player.GetItemDamage(target, Damage.DamageItems.Hydra);
            if (_menu.SubMenu("Item").Item("UseBlade").GetValue<bool>() && _Botrk.IsReady())
                BotrkDmg = (float)Player.GetItemDamage(target, Damage.DamageItems.Botrk);
            if (_menu.SubMenu("Item").Item("UseBilgewater").GetValue<bool>() && _Bilgewater.IsReady())
                BilgeDmg = (float)Player.GetItemDamage(target, Damage.DamageItems.Bilgewater);
            if (_menu.SubMenu("Item").Item("UseHextech").GetValue<bool>() && _Hextech.IsReady())
                HxgDmg = (float)Player.GetItemDamage(target, Damage.DamageItems.Hexgun);

            if (_Ignite != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(_Ignite) == SpellState.Ready)
            {
                IgniteDmg = (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            }

            return QDmg + EDmg + RDmg + TiamatDmg + HydraDmg + BotrkDmg + BilgeDmg + HxgDmg + IgniteDmg;
        }

        private static Vector2 GetFarmLocation()
        {
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(minion => Player.Distance(minion) < 600 && !minion.IsDead && minion.IsVisible);
            var minionLocations = new List<Vector2>();
            foreach (var minion in minions)
            {
                minionLocations.Add(minion.ServerPosition.To2D());
            }
            var location = MinionManager.GetBestCircularFarmLocation(minionLocations, 330, 165);
            if (location.MinionsHit < 3)
                return new Vector2();

            return location.Position;
        }
    }
}
