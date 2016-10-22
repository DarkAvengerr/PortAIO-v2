using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace MistakenTalon
{
    static class Program
    {
        private static Orbwalking.Orbwalker orbwalker;
        private static Spell[] spells;
        private static Items.Item[] items;
        private static Menu scriptConfig;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Talon") return;
            items = new[]
            {
                new Items.Item(3142, float.PositiveInfinity),
                new Items.Item(3074, 400.0f),
                new Items.Item(3077, 400.0f), 
            };
            spells = new[]
            {
                new Spell(SpellSlot.Q),
                new Spell(SpellSlot.W, 625.0f),
                new Spell(SpellSlot.E, 700.0f),
                new Spell(SpellSlot.R, 650.0f)
            };
            SetupScriptConfig();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!scriptConfig.Item("t_resetaa").GetValue<bool>()) return;
            if (orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo) return;
            if (!unit.IsMe || !spells[0].IsReady() || !(target is AIHeroClient)) return;
            spells[0].Cast();
            Orbwalking.ResetAutoAttackTimer();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (scriptConfig.Item("t_draww").GetValue<bool>() && spells[1].IsReady())
                Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[1].Range, Color.RoyalBlue);
            if (scriptConfig.Item("t_drawe").GetValue<bool>() && spells[2].IsReady())
                Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[2].Range, Color.Yellow);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    DoCombo();
                    break;
            }

            if (scriptConfig.Item("t_autoharass").GetValue<bool>() && !ObjectManager.Player.IsRecalling())
                AutoW();
            if (scriptConfig.Item("t_killsteal").GetValue<bool>())
                KillSteal();
        }

        private static void KillSteal()
        {
            var isPacketCasting = scriptConfig.Item("t_castpackets").GetValue<bool>();
            switch (scriptConfig.Item("t_ksmode").GetValue<StringList>().SelectedIndex)
            {
                case 0: // only W
                {
                    var currTarget = GetTarget(spells[1].Range);
                    if (currTarget == null) return;
                    if (spells[1].IsReady() && ObjectManager.Player.GetSpellDamage(currTarget, SpellSlot.W)*2 >= currTarget.Health)
                        spells[1].Cast(currTarget, isPacketCasting);
                    break;
                }
                case 1: // only R
                {
                    var currTarget = GetTarget(spells[3].Range);
                    if (currTarget == null) return;
                    if (spells[3].IsReady() && ObjectManager.Player.GetSpellDamage(currTarget, SpellSlot.R)*2 >= currTarget.Health)
                        CastR(isPacketCasting);
                    break;
                }
                case 2: // W + R
                {
                    var currTarget = GetTarget(spells[3].IsReady() ? spells[3].Range : spells[1].Range);
                    if (currTarget == null) return;
                    var RDmg = ObjectManager.Player.GetSpellDamage(currTarget, SpellSlot.R)*2;
                    var WDmg = ObjectManager.Player.GetSpellDamage(currTarget, SpellSlot.W)*2;
                    var finaldmg = RDmg + WDmg;
                    if (!spells[1].IsReady()) finaldmg -= WDmg;
                    if (!spells[3].IsReady()) finaldmg -= RDmg;
                    if ((finaldmg) >= currTarget.Health)
                    {
                        if (spells[3].IsReady()) CastR(isPacketCasting);
                        if (spells[1].IsReady()) spells[1].Cast(currTarget, isPacketCasting);
                    }
                    break;
                }
                default:
                {
                    throw new IndexOutOfRangeException("The index for killsteal mode was outside the list's bounds.");
                }
            }
        }

        private static void CastR(bool isPacketCasting)
        {
            spells[3].Cast(isPacketCasting);
            LeagueSharp.Common.Utility.DelayAction.Add(50, () => { spells[3].Cast(isPacketCasting); });
        }

        private static AIHeroClient GetTarget(float range, TargetSelector.DamageType damageType = TargetSelector.DamageType.Physical)
        {
            var currTarget = TargetSelector.GetTarget(range, damageType);
            return currTarget != null && currTarget.IsValid ? currTarget : null;
        }
        private static void AutoW()
        {
            var currTarget = GetTarget(spells[1].Range);
            if (currTarget == null) return;
            if (scriptConfig.Item("t_usew").GetValue<bool>() && spells[1].IsReady() && ObjectManager.Player.Distance(currTarget) <= spells[1].Range)
                spells[1].Cast(currTarget, scriptConfig.Item("t_castpackets").GetValue<bool>());
        }

        private static void DoCombo()
        {
            var currTarget = GetTarget(spells[2].IsReady() ? spells[2].Range : spells[1].Range);
            if (currTarget == null) return;
            //if (scriptConfig.Item("t_onlye").GetValue<bool>() && !currTarget.HasBuff("talonnoxiandiplomacybuff")) return;
            var isPacketCasting = scriptConfig.Item("t_castpackets").GetValue<bool>();
            CastSpells(currTarget, isPacketCasting);
        }

        private static void CastItems(AIHeroClient currTarget)
        {
            foreach (var item in items)
            {
                if (Items.CanUseItem(item.Id) && currTarget.IsValidTarget(item.Range)) Items.UseItem(item.Id);
            }
        }

        private static void CastSpells(AIHeroClient currTarget, bool isPacketCasting)
        {
            if (scriptConfig.Item("t_usee").GetValue<bool>() && spells[2].IsReady() && ObjectManager.Player.Distance(currTarget) <= spells[2].Range)
                spells[2].Cast(currTarget, isPacketCasting);

            if (scriptConfig.Item("t_items").GetValue<bool>())
                CastItems(currTarget);

            if (ObjectManager.Player.Distance(currTarget) <= ObjectManager.Player.AttackRange)
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, currTarget);

            if (scriptConfig.Item("t_usew").GetValue<bool>() && spells[1].IsReady() && ObjectManager.Player.Distance(currTarget) <= spells[1].Range)
                spells[1].Cast(currTarget, isPacketCasting);

            if (scriptConfig.Item("t_user").GetValue<bool>() && spells[3].IsReady() && ObjectManager.Player.Distance(currTarget) <= spells[3].Range)
            {
                if (scriptConfig.Item("t_useifkillabler").GetValue<bool>())
                {
                    if (ObjectManager.Player.GetDamageSpell(currTarget, SpellSlot.R).CalculatedDamage*2 >=
                        currTarget.Health)
                    {
                        CastR(isPacketCasting);
                    }
                }
                else
                {
                    CastR(isPacketCasting);
                }
            }
        }

        private static void SetupScriptConfig()
        {
            scriptConfig = new Menu("MistakenTalon", "MistakenTalon", true);
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            scriptConfig.AddSubMenu(targetSelectorMenu);

            scriptConfig.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            orbwalker = new Orbwalking.Orbwalker(scriptConfig.SubMenu("Orbwalking"));

            scriptConfig.AddSubMenu(new Menu("Combo", "Combo"));
            scriptConfig.SubMenu("Combo").AddItem(new MenuItem("t_usew", "Use W in combo").SetValue(true));
            scriptConfig.SubMenu("Combo").AddItem(new MenuItem("t_usee", "Use E in combo").SetValue(true));
            scriptConfig.SubMenu("Combo").AddItem(new MenuItem("t_user", "Use R in combo").SetValue(true));
            scriptConfig.SubMenu("Combo").AddItem(new MenuItem("t_useifkillabler", "Use R only if killable").SetValue(true));
            //scriptConfig.SubMenu("Combo").AddItem(new MenuItem("t_onlye", "Do combo only if enemy has E debuff").SetValue(false));
            scriptConfig.SubMenu("Combo").AddItem(new MenuItem("t_resetaa", "Reset AA using Q").SetValue(true));

            scriptConfig.AddSubMenu(new Menu("Killsteal", "Killsteal"));
            scriptConfig.SubMenu("Killsteal").AddItem(new MenuItem("t_killsteal", "Killsteal").SetValue(true));
            scriptConfig.SubMenu("Killsteal").AddItem(new MenuItem("t_ksmode", "Killsteal spells").SetValue(new StringList(new[] { "Only W", "Only R", "W + R" })));

            scriptConfig.AddSubMenu(new Menu("Drawings", "Drawings"));
            scriptConfig.SubMenu("Drawings").AddItem(new MenuItem("t_draww", "Draw W range").SetValue(false));
            scriptConfig.SubMenu("Drawings").AddItem(new MenuItem("t_drawe", "Draw E range").SetValue(false));

            scriptConfig.AddSubMenu(new Menu("Items", "Items"));
            scriptConfig.SubMenu("Items").AddItem(new MenuItem("t_items", "Use items").SetValue(true));

            scriptConfig.AddSubMenu(new Menu("Miscellaneous", "Miscellaneous"));
            scriptConfig.SubMenu("Miscellaneous").AddItem(new MenuItem("t_castpackets", "Cast spells using packets").SetValue(false));
            scriptConfig.SubMenu("Miscellaneous").AddItem(new MenuItem("t_autoharass", "Auto cast W when enemy is in range").SetValue(false));

            scriptConfig.AddToMainMenu();
        }
    }
}
