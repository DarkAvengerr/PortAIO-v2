using System;
using System.Collections.Generic;
using System.Linq;

using LeagueSharp;
using LeagueSharp.SDK;

using SharpDX;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcKalista
{
    internal static class DamageIndicator
    {
        //hero offsets
        private static readonly int XOffset = 10;
        private static readonly int YOffset = 20;
        private static readonly int Width = 103;
        private static readonly int Height = 8;

        internal static readonly List<JungleHpBarOffset> JungleHpBarOffsetList = new List<JungleHpBarOffset>
        {
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_Dragon_Air", Width = 140, Height = 4, XOffset = 12, YOffset = 24
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_Dragon_Water", Width = 140, Height = 4, XOffset = 12, YOffset = 24
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_Dragon_Fire", Width = 140, Height = 4, XOffset = 12, YOffset = 24
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_Dragon_Earth", Width = 140, Height = 4, XOffset = 12, YOffset = 24
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_Dragon_Elder", Width = 140, Height = 4, XOffset = 12, YOffset = 24
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_Baron", Width = 190, Height = 10, XOffset = 16, YOffset = 24
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_RiftHerald", Width = 139, Height = 6, XOffset = 12, YOffset = 22
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_Red", Width = 139, Height = 4, XOffset = 12, YOffset = 24
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_RedMini", Width = 49, Height = 2, XOffset = 1, YOffset = 5
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_Blue", Width = 139, Height = 4, XOffset = 12, YOffset = 24
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_BlueMini", Width = 49, Height = 2, XOffset = 1, YOffset = 5
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_BlueMini2", Width = 49, Height = 2, XOffset = 1, YOffset = 5
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_Gromp", Width = 86, Height = 2, XOffset = 1, YOffset = 7
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "Sru_Crab", Width = 61, Height = 2, XOffset = 1, YOffset = 5
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_Krug", Width = 79, Height = 2, XOffset = 1, YOffset = 7
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_KrugMini", Width = 55, Height = 2, XOffset = 1, YOffset = 5
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_Razorbeak", Width = 74, Height = 2, XOffset = 1, YOffset = 7
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_RazorbeakMini", Width = 49, Height = 2, XOffset = 1, YOffset = 5
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_Murkwolf", Width = 74, Height = 2, XOffset = 1, YOffset = 7
            },
            new JungleHpBarOffset
            {
                BaseSkinName = "SRU_MurkwolfMini", Width = 55, Height = 2, XOffset = 1, YOffset = 5
            }

            //new JungleMobOffsets { BaseSkinName = "SRU_ChaosMinionMelee", Width = 62, Height = 2, XOffset = 44, YOffset= 21 },
            //new JungleMobOffsets { BaseSkinName = "SRU_ChaosMinionSiege", Width = 60, Height = 2, XOffset = 44, YOffset= 21 },
            //new JungleMobOffsets { BaseSkinName = "SRU_ChaosMinionSuper", Width = 55, Height = 2, XOffset = 44, YOffset= 21 },
            //new JungleMobOffsets { BaseSkinName = "SRU_ChaosMinionRanged", Width = 62, Height = 2, XOffset = 44, YOffset= 21 },
            //new JungleMobOffsets { BaseSkinName = "SRU_OrderMinionMelee", Width = 62, Height = 2, XOffset = 44, YOffset= 21 },
            //new JungleMobOffsets { BaseSkinName = "SRU_OrderMinionSiege", Width = 60, Height = 2, XOffset = 44, YOffset= 21 },
            //new JungleMobOffsets { BaseSkinName = "SRU_OrderMinionSuper", Width = 55, Height = 2, XOffset = 44, YOffset= 21 },
            //new JungleMobOffsets { BaseSkinName = "SRU_OrderMinionRanged", Width = 62, Height = 2, XOffset = 44, YOffset= 21 }
        };

        private static Func<bool> _enabled;

        private static Func<bool> _herosEnabled;
        private static Func<bool> _junglesEnabled;

        private static List<DamageInfo> _damageInfoList = new List<DamageInfo>();

        static DamageIndicator()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }

        internal static void Initialize(List<DamageInfo> damageInfoList, Func<bool> enabled, Func<bool> herosEnabled, Func<bool> junglesEnabled)
        {
            _damageInfoList = damageInfoList;
            _enabled = enabled;
            _herosEnabled = herosEnabled;
            _junglesEnabled = junglesEnabled;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!_enabled())
                return;

            var targets = new List<Obj_AI_Base>();

            if (_herosEnabled())
            {
                targets.AddRange(GameObjects.EnemyHeroes);
            }

            if (_junglesEnabled())
            {
                targets.AddRange(GameObjects.Jungle);
            }

            targets.Where(h => h.IsValid() && h.IsHPBarRendered).ForEach(unit =>
            {
                int width, height, xOffset, yOffset;

                if (unit is AIHeroClient)
                {
                    width = Width;
                    height = Height;
                    xOffset = XOffset;
                    yOffset = YOffset;
                }
                else
                {
                    var mobOffset = JungleHpBarOffsetList.FirstOrDefault(x => x.BaseSkinName == unit.CharData.BaseSkinName);
                    if (mobOffset != null)
                    {
                        width = mobOffset.Width;
                        height = mobOffset.Height;
                        xOffset = mobOffset.XOffset;
                        yOffset = mobOffset.YOffset;
                    }
                    else
                    {
                        return;
                    }
                }

                _damageInfoList.Where(x => x.Enabled()).OrderByDescending(x => x.DamageCalcFunc(unit)).ForEach(damageinfo =>
                {
                    var damage = damageinfo.DamageCalcFunc(unit);

                    if (damage < 1)
                        return;

                    var barPos = unit.HPBarPosition;
                    barPos.X += xOffset;
                    barPos.Y += yOffset;

                    var hpPercent = unit.HealthPercent;
                    var hpPrecentAfterDamage = (unit.Health - damage) / unit.MaxHealth * 100;
                    var drawStartXPos = barPos.X + width * (hpPrecentAfterDamage / 100);
                    var drawEndXPos = barPos.X + width * (hpPercent / 100);

                    if (unit.Health < damage)
                        drawStartXPos = barPos.X;

                    Drawing.DrawLine(drawStartXPos, barPos.Y, drawEndXPos, barPos.Y, height, System.Drawing.Color.FromArgb(170, damageinfo.Color().R, damageinfo.Color().G, damageinfo.Color().B));
                    Drawing.DrawLine(drawStartXPos, barPos.Y, drawStartXPos, barPos.Y + height + 1, 1, System.Drawing.Color.Lime);
                });
            });
        }

        internal class DamageInfo
        {
            internal readonly Func<Color> Color;
            internal readonly Func<Obj_AI_Base, float> DamageCalcFunc;
            internal readonly Func<bool> Enabled;
            internal readonly string Tag;

            internal DamageInfo(string tag, Func<Color> color, Func<Obj_AI_Base, float> damageCalcFunc, Func<bool> enabled)
            {
                Tag = tag;
                Color = color;
                DamageCalcFunc = damageCalcFunc;
                Enabled = enabled;
            }
        }

        internal class JungleHpBarOffset
        {
            internal string BaseSkinName;
            internal int Height;
            internal int Width;
            internal int XOffset;
            internal int YOffset;
        }
    }
}
