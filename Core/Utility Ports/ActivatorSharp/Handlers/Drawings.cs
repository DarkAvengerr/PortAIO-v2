using System;
using System.Collections.Generic;
using System.Linq;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Handlers
{
    struct Offset
    {
        public float X;
        public float Y;
        public int Width;
        public int Height;

        internal Offset(Vector2 vec, int width, int height)
        {
            X = vec.X;
            Y = vec.Y;
            Width = width;
            Height = height;
        }
    }

    class Drawings
    {
        private static readonly Dictionary<string, Offset> Offsets = new Dictionary<string, Offset>
        {
            { "SRU_Blue1.1.1" , new Offset(new Vector2(-2, 23), 150, 6) },
            { "SRU_Red4.1.1" , new Offset(new Vector2(-2, 23), 150, 6) },
            { "SRU_Blue7.1.1" , new Offset(new Vector2(-2, 23), 150, 6) },
            { "SRU_Red10.1.1" , new Offset(new Vector2(-2, 23), 150, 6) },
            { "SRU_Baron12.1.1" , new Offset(new Vector2(57, 24), 165, 11) },
            { "SRU_RiftHerald17.1.1" , new Offset(new Vector2(-2, 23), 155, 6) },
            { "SRU_Dragon_Air6.1.1" , new Offset(new Vector2(1, 23), 150, 6) },
            { "SRU_Dragon_Fire6.2.1" , new Offset(new Vector2(1, 23), 150, 6) },
            { "SRU_Dragon_Water6.3.1" , new Offset(new Vector2(1, 23), 150, 6) },
            { "SRU_Dragon_Earth6.4.1" , new Offset(new Vector2(1, 23), 150, 6) },
            { "SRU_Dragon_Elder6.5.1" , new Offset(new Vector2(1, 23), 150, 7) },
        };

        public static void Init()
        {
            Drawing.OnDraw += args =>
            {
                if (Activator.Origin.Item("acdebug").GetValue<bool>())
                {
                    foreach (var hero in Activator.Allies())
                    {
                        var mpos = Drawing.WorldToScreen(hero.Player.Position);

                        if (!hero.Player.IsDead)
                        {
                            Drawing.DrawText(mpos[0] - 40, mpos[1] - 15, Color.White, "Ability Damage: " + hero.AbilityDamage);
                            Drawing.DrawText(mpos[0] - 40, mpos[1] + 0, Color.White, "Tower Damage: " + hero.TowerDamage);
                            Drawing.DrawText(mpos[0] - 40, mpos[1] + 15, Color.White, "Buff Damage: " + hero.BuffDamage);
                            Drawing.DrawText(mpos[0] - 40, mpos[1] + 30, Color.White, "Troy Damage: " + hero.TroyDamage);
                            Drawing.DrawText(mpos[0] - 40, mpos[1] + 45, Color.White, "Minion Damage: " + hero.MinionDamage);
                        }
                    }
                }

                if (Activator.Origin.Item("acdebug2").GetValue<bool>())
                {
                    Drawing.DrawText(200f, 250f, Color.Wheat, "Item Priority (Debug)");

                    var prior = Lists.Priorities.Values.Where(ii => ii.Needed())
                                .OrderByDescending(ii => ii.Menu().Item("prior" + ii.Name()).GetValue<Slider>().Value);

                    foreach (var item in prior)
                    {
                        for (int i = 0; i < prior.Count(); i++)
                        {
                            Drawing.DrawText(200, 265 + 5 * (i * 3), Color.White, item.Name() + " / Needed: " 
                                + item.Needed() + " / Ready: " + item.Ready() + " :: " + item.Position);
                        }
                    }
                }

                if (!Activator.SmiteInGame)
                {
                    return;
                }

                if (Activator.Origin.Item("drawsmitet").GetValue<bool>())
                {
                    var wts = Drawing.WorldToScreen(Activator.Player.Position);

                    if (Activator.Origin.Item("usesmite").GetValue<KeyBind>().Active)
                        Drawing.DrawText(wts[0] - 35, wts[1] + 55, Color.White, "Smite: ON");

                    if (!Activator.Origin.Item("usesmite").GetValue<KeyBind>().Active)
                        Drawing.DrawText(wts[0]- 35, wts[1] + 55, Color.Gray, "Smite: OFF");
                }

                if (Activator.Origin.Item("drawsmite").GetValue<bool>())
                {
                    if (Activator.Origin.Item("usesmite").GetValue<KeyBind>().Active)
                        Render.Circle.DrawCircle(Activator.Player.Position, 500f, Color.White, 2);

                    if (!Activator.Origin.Item("usesmite").GetValue<KeyBind>().Active)
                        Render.Circle.DrawCircle(Activator.Player.Position, 500f, Color.Gray, 2);
                }

                if (!Activator.Player.IsDead && Activator.Origin.Item("drawfill").GetValue<bool>())
                {
                    if (Activator.MapId != (int) MapType.SummonersRift)
                    {
                        return;
                    }

                    var spell = Data.Smitedata.CachedSpellList.FirstOrDefault();
                    var minionlist = MinionManager.GetMinions(Activator.Player.Position, 1200f, MinionTypes.All, MinionTeam.Neutral);

                    foreach (var minion in minionlist.Where(th => Helpers.IsEpicMinion(th) || Helpers.IsLargeMinion(th)))
                    {
                        var yoffset = Offsets[minion.Name].Y;
                        var xoffset = Offsets[minion.Name].X;
                        var width = Offsets[minion.Name].Width;
                        var height = Offsets[minion.Name].Height;

                        if (!minion.IsHPBarRendered)
                        {
                            continue;
                        }

                        var barPos = minion.HPBarPosition;

                        var sdamage = spell != null && Activator.Player.GetSpell(spell.Slot).State == SpellState.Ready
                            ? Activator.Player.GetSpellDamage(minion, spell.Slot, spell.Stage)
                            : 0;

                        var smite = Activator.Player.GetSpell(Activator.Smite).State == SpellState.Ready
                            ? Activator.Player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite)
                            : 0;

                        var damage = smite + sdamage;
                        var pctafter = Math.Max(0, minion.Health - damage) / minion.MaxHealth;

                        var yaxis = barPos.Y + yoffset;
                        var xaxisdmg = (float) (barPos.X + xoffset + width * pctafter);
                        var xaxisnow = barPos.X + xoffset + width * minion.Health / minion.MaxHealth;

                        var ana = xaxisnow - xaxisdmg;
                        var pos = barPos.X + xoffset + 12 + (139 * pctafter);

                        for (var i = 0; i < ana; i++)
                        {
                            if (Activator.Origin.Item("usesmite").GetValue<KeyBind>().Active)
                                Drawing.DrawLine((float) pos + i, yaxis, (float) pos + i, yaxis + height, 1,
                                    Color.White);

                            if (!Activator.Origin.Item("usesmite").GetValue<KeyBind>().Active)
                                Drawing.DrawLine((float) pos + i, yaxis, (float) pos + i, yaxis + height, 1,
                                    Color.Gray);
                        }
                    }
                }
            };
        }
    }
}
