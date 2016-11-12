using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Linq;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CheerleaderLux.Extensions
{
    class Drawmethods : Statics
    {
        public static void DrawEvent()
        {

            Drawing.OnDraw += DamageIndicator;
            Drawing.OnDraw += Rdamage;
            Drawing.OnDraw += Indicator;
            Drawing.OnEndScene += MiniMapDraw;
            Drawing.OnDraw += PredictionDraw;
        }

        private static void Rdamage(EventArgs args)
        {
            var enemy = HeroManager.Enemies.Where(e => e.IsValidTarget(R1.Range));
            if (Config.Item("optimize").GetValue<bool>()) return;

            foreach (var e in enemy)
            {
                if (e == null) return;
                else
                {
                    var debuff = e.HasBuff("luxilluminatingfraulein");
                    float dmgr = R1.GetDamage(e);
                    if (debuff)
                        dmgr += Lux.PassiveDMG(e);
                    var pos1 = Drawing.WorldToScreen(e.Position);

                    if (Config.Item("draw.R.dmg").GetValue<bool>() && R1.IsReady())
                    {
                        Drawing.DrawText(pos1.X - 50, pos1.Y + 30, System.Drawing.Color.Tomato, "[R] Damage = " + dmgr.ToString("#,#"));
                    }

                }
            }
            var pos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            if (Config.Item("draw.harass.indicator").GetValue<bool>())
                Drawing.DrawText(pos.X - 50, pos.Y + 35, System.Drawing.Color.AliceBlue, "AutoHarass:");
            if (Config.Item("autoharass").GetValue<KeyBind>().Active && Config.Item("draw.harass.indicator").GetValue<bool>())           
                Drawing.DrawText(pos.X + 43, pos.Y + 35, System.Drawing.Color.LawnGreen, "On");
            
            else if (Config.Item("draw.harass.indicator").GetValue<bool>())            
                Drawing.DrawText(pos.X + 43, pos.Y + 35, System.Drawing.Color.Tomato, "Off");
            
            
            }

        private static void PredictionDraw(EventArgs args)
        {
            if (Config.Item("optimize").GetValue<bool>()) return;

            var dTarget = TargetSelector.GetTarget(Q1.Range + 125, TargetSelector.DamageType.Magical);
            if (dTarget == null) return;

            if (Config.Item("prediction.draw").GetValue<bool>())
            {
                var PredictionPos = Prediction.GetPrediction(dTarget, 0.25f);

                var pos1 = Drawing.WorldToScreen(player.Position);
                var pos2 = Drawing.WorldToScreen(PredictionPos.CastPosition);


                Drawing.DrawLine(pos1, pos2, 1, System.Drawing.Color.LightBlue);
            }
        }

        static void MiniMapDraw(EventArgs args)
        {
            if (Config.Item("optimize").GetValue<bool>()) return;

            bool drawMinimapR = Config.Item("drawing.minimap").GetValue<bool>();

            if (drawMinimapR && player != null)
                LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R1.Range, System.Drawing.Color.DeepSkyBlue, 2, 30, true);
        }

        public static void Indicator(EventArgs args)
        {
            if (Config.Item("optimize").GetValue<bool>())
            {
                return;
            }

            var enemies1 = HeroManager.Enemies.Where(e => !e.IsDead).ToList();
            var enemies2 = HeroManager.Enemies.Where(e => !e.IsDead && player.Distance(e.Position) < 3000).ToList();

            foreach (var enemy in enemies1.Where(enemy => enemy.Team != player.Team))
            {
                if (enemy == null) return;

                if (enemy.IsHPBarRendered && !enemy.IsDead)
                {
                    if (Config.Item("drawing.indicator").GetValue<bool>())
                    {
                        var pos = player.Position +
                                  Vector3.Normalize(enemy.Position - player.Position) * 200;
                        var myPos = Drawing.WorldToScreen(pos);
                        pos = player.Position + Vector3.Normalize(enemy.Position - player.Position) * 450;
                        var ePos = Drawing.WorldToScreen(pos);

                        var linecolor = System.Drawing.Color.DodgerBlue;
                        var linecolor2 = System.Drawing.Color.DodgerBlue;
                        if (enemy.Position.Distance(player.Position) > 3000)
                        {
                            linecolor = System.Drawing.Color.DodgerBlue;
                        }
                        else if (enemy.Position.Distance(player.Position) < 3000)
                            linecolor = System.Drawing.Color.Red;
                        if (enemies2.Count > 1)
                            linecolor2 = System.Drawing.Color.Red;

                        if (Config.Item("drawing.indicator").GetValue<bool>())
                            Drawing.DrawLine(myPos.X, myPos.Y, ePos.X, ePos.Y, 2, linecolor);
                            Render.Circle.DrawCircle(player.Position, 200, linecolor2);
                    }
                }
            }
        }

        public static void DamageIndicator(EventArgs args)
        {
            if (Config.Item("optimize").GetValue<bool>())
            {
                LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = false;
                return;
            }
            if (Config.Item("disable.dmg").GetValue<bool>())
            {
                LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = false;
                return;
            }

            var mode = Config.Item("drawing.dmg").GetValue<StringList>().SelectedIndex;
            if (mode == 1)
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != player.Team))
                {
                    if (enemy == null) return;

                    if (enemy.IsHPBarRendered && !enemy.IsDead)
                    {

                        var combodamage = (CalcDamage(enemy));

                        var PercentHPleftAfterCombo = (enemy.Health - combodamage) / enemy.MaxHealth;
                        var PercentHPleft = enemy.Health / enemy.MaxHealth;
                        if (PercentHPleftAfterCombo < 0)
                            PercentHPleftAfterCombo = 0;

                        var hpBarPos = enemy.HPBarPosition;
                        hpBarPos.X += 45;
                        hpBarPos.Y += 18;
                        double comboXPos = hpBarPos.X - 36 + (107 * PercentHPleftAfterCombo);
                        double currentHpxPos = hpBarPos.X - 36 + (107 * PercentHPleft);
                        var diff = currentHpxPos - comboXPos;
                        for (var i = 0; i < diff; i++)
                        {
                            Drawing.DrawLine(
                                (float)comboXPos + i, hpBarPos.Y + 2, (float)comboXPos + i,
                                hpBarPos.Y + 10, 1, Config.Item("drawing.dmg.color").GetValue<Circle>().Color);
                            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = false;
                        }

                    }
                }
            }
            if (mode == 0)
            {
                LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = CalcDamage;
                LeagueSharp.Common.Utility.HpBarDamageIndicator.Color = Config.Item("drawing.dmg.color").GetValue<Circle>().Color;
                LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;
            }
        }

        public static float CalcDamage(Obj_AI_Base target)
        {
            //Calculate Combo Damage
            var aa = player.GetAutoAttackDamage(target);
            var damage = aa;
            Ignite = player.GetSpellSlot("summonerdot");

            if (Ignite.IsReady())
                damage += player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            if (Config.Item("combo.E").GetValue<bool>()) // edamage
            {
                if (E1.IsReady())
                {
                    damage += E1.GetDamage(target);
                }
            }
            if (target.HasBuff("luxilluminatingfraulein"))
            {
                damage += aa + player.CalcDamage(target, Damage.DamageType.Magical,
                    10 + (8 * player.Level) + 0.2 * player.FlatMagicDamageMod);
            }
            if (player.HasBuff("lichbane"))
            {
                damage += player.CalcDamage(target, Damage.DamageType.Magical,
                    (player.BaseAttackDamage * 0.75) + ((player.BaseAbilityDamage + player.FlatMagicDamageMod) * 0.5));
            }
            if (R1.IsReady()) // rdamage
            {
                damage += R1.GetDamage(target);
            }

            if (Q1.IsReady())
            {
                damage += Q1.GetDamage(target);
            }
            return (float)damage;
        }
    }
}
    

