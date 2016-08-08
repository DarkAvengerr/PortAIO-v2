using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Activator.Data;
    
using EloBuddy; namespace Activator.Summoners
{
    internal class dot : CoreSum
    {
        internal override string Name => "summonerdot";
        internal override string DisplayName => "Ignite";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => 600f;
        internal override int Duration => 100;

        internal Spell Q => new Spell(SpellSlot.Q);
        internal Spell W => new Spell(SpellSlot.W);
        internal Spell E => new Spell(SpellSlot.E);
        internal Spell R => new Spell(SpellSlot.R);

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var tar in Activator.Heroes)
            {
                if (!tar.Player.LSIsValidTarget(1500))
                {
                    continue;
                }

                if (tar.Player.LSHasBuff("kindredrnodeathbuff") || tar.Player.IsZombie ||
                    tar.Player.LSHasBuff("summonerdot"))
                {
                    continue;
                }

                if (!Parent.Item(Parent.Name + "useon" + tar.Player.NetworkId).GetValue<bool>())
                {
                    continue;
                }

                // ignite damagerino
                var ignotedmg = (float) Player.GetSummonerSpellDamage(tar.Player, Damage.SummonerSpell.Ignite);

                // killsteal ignite
                if (Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 0)
                {
                    if (tar.Player.Health <= ignotedmg)
                    {
                        if (tar.Player.LSDistance(Player.ServerPosition) <= 600)
                            UseSpellOn(tar.Player);
                    }
                }

                // combo ignite
                if (Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1)
                {
                    var totaldmg = 0d;

                    switch (Player.ChampionName)
                    {
                        case "Akali":
                            totaldmg += R.GetDamage(tar.Player) * R.Instance.Ammo;
                            break;
                        case "Ahri":
                            if (!tar.Player.HasBuffOfType(BuffType.Charm) &&
                                Menu.Item("ii" + Player.ChampionName).GetValue<bool>() &&
                                E.LSIsReady())
                                continue;

                            totaldmg += R.LSIsReady() ? R.GetDamage(tar.Player) * 2 : 0;
                            break;
                        case "Cassiopeia":
                            if (!tar.Player.HasBuffOfType(BuffType.Poison) &&
                                Menu.Item("ii" + Player.ChampionName).GetValue<bool>() &&
                                (Q.LSIsReady() || W.LSIsReady()))
                                continue;

                            var dmg = Math.Min(6, Player.Mana / E.ManaCost) * E.GetDamage(tar.Player);
                            totaldmg += tar.Player.HasBuffOfType(BuffType.Poison) ? dmg * 2 : dmg;
                            break;
                        case "Diana":
                            if (!tar.Player.LSHasBuff("dianamoonlight") &&
                                Menu.Item("ii" + Player.ChampionName).GetValue<bool>() &&
                                Q.LSIsReady())
                                continue;

                            totaldmg += tar.Player.LSHasBuff("dianamoonlight")
                                ? R.GetDamage(tar.Player) * 2 : 0;
                            break;
                        case "Evelynn":
                        case "Orianna":
                        case "Lissandra":
                        case "Karthus":
                            totaldmg += Math.Min(6, Player.Mana / Q.ManaCost) * Q.GetDamage(tar.Player);
                            break;
                        case "Vayne":
                            totaldmg += Orbwalking.InAutoAttackRange(tar.Player)
                                ? Math.Min(6, Player.Mana / Q.ManaCost) * Q.GetDamage(tar.Player)
                                : 0;
                            break;
                    }

                    // aa dmg
                    totaldmg += Orbwalking.InAutoAttackRange(tar.Player)
                        ? Player.LSGetAutoAttackDamage(tar.Player, true) * 3
                        : 0;

                    // combo damge
                    totaldmg +=
                        Gamedata.DamageLib.Sum(
                            entry =>
                                Player.GetSpell(entry.Value).LSIsReady(2)
                                    ? entry.Key(Player, tar.Player, Player.GetSpell(entry.Value).Level - 1)
                                    : 0);

                    var finaldmg = totaldmg * Menu.Item("idmgcheck", true).GetValue<Slider>().Value / 100;

                    if (Menu.Item("idraw").GetValue<bool>())
                    {
                        var pdmg = finaldmg > tar.Player.Health ? 100 : finaldmg * 100 / tar.Player.Health;
                        var drawdmg = Math.Round(pdmg);
                        var pos = Drawing.WorldToScreen(tar.Player.Position);

                        Drawing.DrawText(pos[0], pos[1], System.Drawing.Color.Yellow, drawdmg + " %");
                    }

                    if (finaldmg + ignotedmg >= tar.Player.Health)
                    {
                        var nt = ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(
                                x => !x.IsDead && x.IsValid && x.Team == tar.Player.Team && tar.Player.LSDistance(x.Position) <= 1250);
                        
                        if (nt != null && Menu.Item("itu").GetValue<bool>() && Player.Level <= Menu.Item("igtu").GetValue<Slider>().Value)
                        {
                            if (Player.LSCountAlliesInRange(750) == 0 && (totaldmg + ignotedmg / 1.85) < tar.Player.Health)
                                continue;
                        }

                        if (Orbwalking.InAutoAttackRange(tar.Player) && tar.Player.LSCountAlliesInRange(450) > 1)
                        {
                            if (totaldmg + ignotedmg / 2.5 >= tar.Player.Health)
                            {
                                continue;
                            }

                            if (nt != null && tar.Player.LSDistance(nt) <= 600)
                            {
                                continue;
                            }
                        }

                        if (tar.Player.Level <= 4 &&
                            tar.Player.InventoryItems.Any(item => item.Id == (ItemId) 2003 || item.Id == (ItemId) 2010))
                        {
                            continue;
                        }

                        if (tar.Player.LSDistance(Player.ServerPosition) <= 600)
                        {
                            UseSpellOn(tar.Player, true);
                        }
                    }
                }
            }
        }
    }
}
