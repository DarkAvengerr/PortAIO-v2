using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Kindred___YinYang.Spell_Database;
using LeagueSharp.Common;
using LeagueSharp;
using Color = System.Drawing.Color;
using EloBuddy;

namespace Kindred___YinYang
{
    class Language
    {
        public static void MenuInit()
        {
            switch (Program.Config.Item("language.supx").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    EnglishMenu();
                    break;
                case 1:
                    KoreanMenu();
                    break;
                case 2:
                    TurkishMenu();
                    break;
                case 3:
                    PortugueseMenu();
                    break;
                case 4:
                    FrenchMenu();
                    break;
                default:
                    EnglishMenu();
                    break;
            }
        }

        public static void EnglishMenu()
        {
            var settings = new Menu(":: Settings", ":: Settings");
            {
                var comboMenu = new Menu("Combo Settings", "Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("q.combo.style", "(Q) Combo Style").SetValue(new StringList(new[] { "Kite", "100% Hit", "Safe Position" })));
                    comboMenu.AddItem(new MenuItem("q.combo", "Use (Q)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("w.combo", "Use (W)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e.combo", "Use (E)").SetValue(true));
                    settings.AddSubMenu(comboMenu);
                }
                var eMenu = new Menu("(E) Settings", "(E) Settings");
                {
                    eMenu.AddItem(new MenuItem("e.whte", "                     (E) Whitelist"));
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                    {
                        eMenu.AddItem(new MenuItem("enemy." + enemy.CharData.BaseSkinName, string.Format("E: {0}", enemy.CharData.BaseSkinName)).SetValue(Program.HighChamps.Contains(enemy.CharData.BaseSkinName)));

                    }
                    settings.AddSubMenu(eMenu);
                }
                var harassMenu = new Menu("Harass Settings", "Harass Settings");
                {
                    harassMenu.AddItem(new MenuItem("q.harass", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("w.harass", "Use W").SetValue(true));
                    harassMenu.AddItem(new MenuItem("e.harass", "Use E").SetValue(true));
                    harassMenu.AddItem(new MenuItem("harass.mana", "Mana Manager").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(harassMenu);
                }
                var laneClear = new Menu("Clear Settings", "Clear Settings");
                {
                    laneClear.AddItem(new MenuItem("q.clear", "Use Q").SetValue(true));
                    laneClear.AddItem(new MenuItem("q.minion.count", "Q Minion Count").SetValue(new Slider(4, 1, 5)));
                    laneClear.AddItem(new MenuItem("clear.mana", "Mana Manager").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(laneClear);
                }
                var jungleClear = new Menu("Jungle Settings", "Jungle Settings");
                {
                    jungleClear.AddItem(new MenuItem("q.jungle", "Use Q").SetValue(true));
                    jungleClear.AddItem(new MenuItem("w.jungle", "Use W").SetValue(true));
                    jungleClear.AddItem(new MenuItem("e.jungle", "Use E").SetValue(true));
                    jungleClear.AddItem(new MenuItem("jungle.mana", "Mana Manager").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(jungleClear);
                }
                var ksMenu = new Menu("KillSteal Settings", "KillSteal Settings");
                {
                    ksMenu.AddItem(new MenuItem("q.ks", "Use Q").SetValue(true));
                    ksMenu.AddItem(new MenuItem("q.ks.count", "Basic Attack Count").SetValue(new Slider(2, 1, 5)));
                    settings.AddSubMenu(ksMenu);
                }
                var miscMenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscMenu.AddItem(new MenuItem("q.antigapcloser", "Anti-Gapcloser Q!").SetValue(true));
                    var antiRengar = new Menu("Anti Rengar", "Anti Rengar");
                    {
                        antiRengar.AddItem(new MenuItem("anti.rengar", "Anti Rengar!").SetValue(true));
                        antiRengar.AddItem(new MenuItem("hp.percent.for.rengar", "Min. HP Percent").SetValue(new Slider(30, 1, 99)));
                        miscMenu.AddSubMenu(antiRengar);
                    }
                    var spellMenu = new Menu("Spell Breaker", "Spell Breaker");
                    {
                        spellMenu.AddItem(new MenuItem("spell.broker", "Spell Breaker!").SetValue(true));
                        spellMenu.AddItem(new MenuItem("katarina.r", "Katarina (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("missfortune.r", "Miss Fortune (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("lucian.r", "Lucian (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("hp.percent.for.broke", "Min. HP Percent").SetValue(new Slider(20, 1, 99)));
                        miscMenu.AddSubMenu(spellMenu);
                    }
                    var rProtector = new Menu("(R) Protector", "(R) Protector");
                    {
                        rProtector.AddItem(new MenuItem("protector", "Disable Protector?").SetValue(true));
                        foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                        {
                            foreach (var skillshot in SpellDatabase.Spells.Where(x => x.charName == enemy.ChampionName)) // 2.5F Protector
                            {
                                rProtector.AddItem(new MenuItem("hero." + skillshot.spellName, "" + skillshot.charName + "(" + skillshot.spellKey + ")").SetValue(true));
                            }
                        }
                        miscMenu.AddSubMenu(rProtector);
                    }
                    settings.AddSubMenu(miscMenu);
                }
            }
            
            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                var damageDraw = new Menu("Damage Draw", "Damage Draw");
                {
                    damageDraw.AddItem(new MenuItem("aa.indicator", "AA Indicator").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    drawMenu.AddSubMenu(damageDraw);
                }
                drawMenu.AddItem(new MenuItem("q.drawx", "Q Range").SetValue(new Circle(true, System.Drawing.Color.White)));
                drawMenu.AddItem(new MenuItem("w.draw", "W Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                drawMenu.AddItem(new MenuItem("e.draw", "E Range").SetValue(new Circle(true, System.Drawing.Color.DodgerBlue)));
                drawMenu.AddItem(new MenuItem("r.draw", "R Range").SetValue(new Circle(true, System.Drawing.Color.GreenYellow)));
                settings.AddSubMenu(drawMenu);
            }
            settings.AddItem(new MenuItem("e.method", "E Method").SetValue(new StringList(new[] { "Cursor Position" })));
            settings.AddItem(new MenuItem("use.r", "Use R").SetValue(true));
            settings.AddItem(new MenuItem("r.whte", "                          R Whitelist"));
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly))
            {
                settings.AddItem(new MenuItem("respite." + ally.CharData.BaseSkinName, string.Format("Respite: {0}", ally.CharData.BaseSkinName)).SetValue(Program.HighChamps.Contains(ally.CharData.BaseSkinName)));

            }
            settings.AddItem(new MenuItem("min.hp.for.r", "Min. HP Percent for R").SetValue(new Slider(20, 1, 99)));
            Program.Config.AddSubMenu(settings);
        }

        public static void KoreanMenu()
        {
            var settings = new Menu(":: Settings", ":: Settings");
            {
                var comboMenu = new Menu("콤보 설정", "콤보 설정");
                {
                    comboMenu.AddItem(new MenuItem("q.combo.style", "(Q) 콤보 스타일").SetValue(new StringList(new[] { "카이팅 위주", "명중률 위주", "안전 위주" })));
                    comboMenu.AddItem(new MenuItem("q.combo", "(Q) 사용").SetValue(true));
                    comboMenu.AddItem(new MenuItem("w.combo", "(W) 사용").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e.combo", "(E) 사용").SetValue(true));
                    settings.AddSubMenu(comboMenu);
                }
                var eMenu = new Menu("(E) 설정", "(E) 설정");
                {
                    eMenu.AddItem(new MenuItem("e.whte", "                     (E) 사용 목록"));
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                    {
                        eMenu.AddItem(new MenuItem("enemy." + enemy.CharData.BaseSkinName, string.Format("E: {0}", enemy.CharData.BaseSkinName)).SetValue(Program.HighChamps.Contains(enemy.CharData.BaseSkinName)));

                    }
                    settings.AddSubMenu(eMenu);
                }
                var harassMenu = new Menu("견제 설정", "견제 설정");
                {
                    harassMenu.AddItem(new MenuItem("q.harass", "Q 사용").SetValue(true));
                    harassMenu.AddItem(new MenuItem("w.harass", "W 사용").SetValue(true));
                    harassMenu.AddItem(new MenuItem("e.harass", "E 사용").SetValue(true));
                    harassMenu.AddItem(new MenuItem("harass.mana", "마나 관리").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(harassMenu);
                }
                var laneClear = new Menu("라인클리어 설정", "라인클리어 설정");
                {
                    laneClear.AddItem(new MenuItem("q.clear", "Q 사용").SetValue(true));
                    laneClear.AddItem(new MenuItem("q.minion.count", "Q 미니언 세기").SetValue(new Slider(4, 1, 5)));
                    laneClear.AddItem(new MenuItem("clear.mana", "마나 관리").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(laneClear);
                }
                var jungleClear = new Menu("정글 설정", "정글 설정");
                {
                    jungleClear.AddItem(new MenuItem("q.jungle", "Q 사용").SetValue(true));
                    jungleClear.AddItem(new MenuItem("w.jungle", "W 사용").SetValue(true));
                    jungleClear.AddItem(new MenuItem("e.jungle", "E 사용").SetValue(true));
                    jungleClear.AddItem(new MenuItem("jungle.mana", "마나 관리").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(jungleClear);
                }
                var ksMenu = new Menu("킬스틸 설정", "킬스틸 설정");
                {
                    ksMenu.AddItem(new MenuItem("q.ks", "Q 사용").SetValue(true));
                    ksMenu.AddItem(new MenuItem("q.ks.count", "기본 공격 횟수").SetValue(new Slider(2, 1, 5)));
                    settings.AddSubMenu(ksMenu);
                }
                var miscMenu = new Menu("다양한 설정", "다양한 설정");
                {
                    miscMenu.AddItem(new MenuItem("q.antigapcloser", "(Q) 돌진스킬 회피").SetValue(true));
                    var antiRengar = new Menu("Anti Rengar", "(Q) 렝가 접근차단");
                    {
                        antiRengar.AddItem(new MenuItem("anti.rengar", "(Q) 렝가 접근 차단").SetValue(true));
                        antiRengar.AddItem(new MenuItem("hp.percent.for.rengar", "최소 체력 퍼센트").SetValue(new Slider(30, 1, 99)));
                        miscMenu.AddSubMenu(antiRengar);
                    }
                    var spellMenu = new Menu("스킬 끊기", "스킬 끊기");
                    {
                        spellMenu.AddItem(new MenuItem("spell.broker", "스킬 끊기!").SetValue(true));
                        spellMenu.AddItem(new MenuItem("katarina.r", "카타리나 (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("missfortune.r", "미스포츈 (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("lucian.r", "루시안 (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("hp.percent.for.broke", "최소 체력 퍼센트").SetValue(new Slider(20, 1, 99)));
                        miscMenu.AddSubMenu(spellMenu);
                    }
                    var rProtector = new Menu("(R) 보호 기능", "(R) 보호 기능");
                    {
                        rProtector.AddItem(new MenuItem("protector", "보호 기능 해제?").SetValue(true));
                        foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                        {
                            foreach (var skillshot in SpellDatabase.Spells.Where(x => x.charName == enemy.ChampionName)) // 2.5F Protector
                            {
                                rProtector.AddItem(new MenuItem("hero." + skillshot.spellName, "" + skillshot.charName + "(" + skillshot.spellKey + ")").SetValue(true));
                            }
                        }
                        miscMenu.AddSubMenu(rProtector);
                    }
                    settings.AddSubMenu(miscMenu);
                }
                var drawMenu = new Menu("사거리 설정", "사거리 설정");
                {
                    var damageDraw = new Menu("데미지 표시", "데미지 표시");
                    {
                        damageDraw.AddItem(new MenuItem("aa.indicator", "AA 표시기").SetValue(new Circle(true, Color.Gold)));
                        drawMenu.AddSubMenu(damageDraw);
                    }
                    drawMenu.AddItem(new MenuItem("q.drawx", "Q 사거리 표시").SetValue(new Circle(true, Color.White)));
                    drawMenu.AddItem(new MenuItem("w.draw", "W 사거리 표시").SetValue(new Circle(true, Color.Gold)));
                    drawMenu.AddItem(new MenuItem("e.draw", "E 사거리 표시").SetValue(new Circle(true, Color.DodgerBlue)));
                    drawMenu.AddItem(new MenuItem("r.draw", "R 사거리 표시").SetValue(new Circle(true, Color.GreenYellow)));
                    settings.AddSubMenu(drawMenu);
                }
                settings.AddItem(new MenuItem("e.method", "E 로직").SetValue(new StringList(new[] { "마우스 위치로" })));
                settings.AddItem(new MenuItem("use.r", "R 사용").SetValue(true));
                settings.AddItem(new MenuItem("r.whte", "                          R 사용 목록"));
                foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly))
                {
                    settings.AddItem(new MenuItem("respite." + ally.CharData.BaseSkinName, string.Format("Respite: {0}", ally.CharData.BaseSkinName)).SetValue(Program.HighChamps.Contains(ally.CharData.BaseSkinName)));

                }
                settings.AddItem(new MenuItem("min.hp.for.r", "최소 체력퍼센트일때 R 사용").SetValue(new Slider(20, 1, 99)));
                Program.Config.AddSubMenu(settings);
            }
        }
        public static void TurkishMenu()
        {
            var settings = new Menu(":: Settings", ":: Ayarlar");
            {
                var comboMenu = new Menu("Kombo Ayarları", "Kombo Ayarları");
                {
                    comboMenu.AddItem(new MenuItem("q.combo.style", "(Q) Kombo Stili").SetValue(new StringList(new[] { "Kite", "100% Hit", "Safe Position" })));
                    comboMenu.AddItem(new MenuItem("q.combo", "(Q) Kullan").SetValue(true));
                    comboMenu.AddItem(new MenuItem("w.combo", "(W) Kullan").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e.combo", "(E) Kullan").SetValue(true));
                    settings.AddSubMenu(comboMenu);
                }
                var eMenu = new Menu("(E) Ayarları", "(E) Ayarları");
                {
                    eMenu.AddItem(new MenuItem("e.whte", "                     (E) Beyaz Liste"));
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                    {
                        eMenu.AddItem(new MenuItem("enemy." + enemy.CharData.BaseSkinName, string.Format("E: {0}", enemy.CharData.BaseSkinName)).SetValue(Program.HighChamps.Contains(enemy.CharData.BaseSkinName)));

                    }
                    settings.AddSubMenu(eMenu);
                }
                var harassMenu = new Menu("Dürtme Ayarları", "Dürtme Ayarları");
                {
                    harassMenu.AddItem(new MenuItem("q.harass", "(Q) Kullan").SetValue(true));
                    harassMenu.AddItem(new MenuItem("w.harass", "(W) Kullan").SetValue(true));
                    harassMenu.AddItem(new MenuItem("e.harass", "(E) Kullan").SetValue(true));
                    harassMenu.AddItem(new MenuItem("harass.mana", "Dürtme Mana Miktarı").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(harassMenu);
                }
                var laneClear = new Menu("Koridor Temizleme Ayarları", "Koridor Temizleme Ayarları");
                {
                    laneClear.AddItem(new MenuItem("q.clear", "(Q) Kullan").SetValue(true));
                    laneClear.AddItem(new MenuItem("q.minion.count", "Q Minyon Sayısı").SetValue(new Slider(4, 1, 5)));
                    laneClear.AddItem(new MenuItem("clear.mana", "Koridor Temizleme Mana Miktarı").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(laneClear);
                }
                var jungleClear = new Menu("Orman Temizleme Ayarları", "Orman Temizleme Ayarları");
                {
                    jungleClear.AddItem(new MenuItem("q.jungle", "(Q) Kullan").SetValue(true));
                    jungleClear.AddItem(new MenuItem("w.jungle", "(W) Kullan").SetValue(true));
                    jungleClear.AddItem(new MenuItem("e.jungle", "(E) Kullan").SetValue(true));
                    jungleClear.AddItem(new MenuItem("jungle.mana", "Orman Temizleme Mana Miktarı").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(jungleClear);
                }
                var ksMenu = new Menu("KS Ayarları", "KS Ayarları");
                {
                    ksMenu.AddItem(new MenuItem("q.ks", "(Q) Kullan").SetValue(true));
                    ksMenu.AddItem(new MenuItem("q.ks.count", "Düz Vuruş Miktarı").SetValue(new Slider(2, 1, 5)));
                    settings.AddSubMenu(ksMenu);
                }
                var miscMenu = new Menu("Diğer Ayarlar", "Diğer Ayarlar");
                {
                    miscMenu.AddItem(new MenuItem("q.antigapcloser", "Mesafe Koruyucu (Q)!").SetValue(true));
                    var antiRengar = new Menu("Anti Rengar", "Anti Rengar");
                    {
                        antiRengar.AddItem(new MenuItem("anti.rengar", "Anti Rengar!").SetValue(true));
                        antiRengar.AddItem(new MenuItem("hp.percent.for.rengar", "Anti-Rengar için Minimum Can Yüzdesi").SetValue(new Slider(30, 1, 99)));
                        miscMenu.AddSubMenu(antiRengar);
                    }
                    var spellMenu = new Menu("Rakibin Yeteneğini Bozma", "Rakibin Yeteneğini Bozma");
                    {
                        spellMenu.AddItem(new MenuItem("spell.broker", "Yetenek Bozma!").SetValue(true));
                        spellMenu.AddItem(new MenuItem("katarina.r", "Katarina (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("missfortune.r", "Miss Fortune (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("lucian.r", "Lucian (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("hp.percent.for.broke", "Spell Bozma için Minimum Can Yüzdesi").SetValue(new Slider(20, 1, 99)));
                        miscMenu.AddSubMenu(spellMenu);
                    }
                    var rProtector = new Menu("(R) Koruyucu", "(R) Koruyucu");
                    {
                        rProtector.AddItem(new MenuItem("protector", "Koruyucuyu Deaktif Et?").SetValue(true));
                        foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                        {
                            foreach (var skillshot in SpellDatabase.Spells.Where(x => x.charName == enemy.ChampionName)) // 2.5F Protector
                            {
                                rProtector.AddItem(new MenuItem("hero." + skillshot.spellName, "" + skillshot.charName + "(" + skillshot.spellKey + ")").SetValue(true));
                            }
                        }
                        miscMenu.AddSubMenu(rProtector);
                    }
                    settings.AddSubMenu(miscMenu);
                }
            }

            var drawMenu = new Menu("Çizim Ayarlar", "Çizim Ayarları");
            {
                var damageDraw = new Menu("Hasar Çizimleri", "Hasar Çizimleri");
                {
                    damageDraw.AddItem(new MenuItem("aa.indicator", "AA Göstergesi").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    drawMenu.AddSubMenu(damageDraw);
                }
                drawMenu.AddItem(new MenuItem("q.drawx", "Q Menzili").SetValue(new Circle(true, System.Drawing.Color.White)));
                drawMenu.AddItem(new MenuItem("w.draw", "W Menzili").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                drawMenu.AddItem(new MenuItem("e.draw", "E Menzili").SetValue(new Circle(true, System.Drawing.Color.DodgerBlue)));
                drawMenu.AddItem(new MenuItem("r.draw", "R Menzili").SetValue(new Circle(true, System.Drawing.Color.GreenYellow)));
                settings.AddSubMenu(drawMenu);
            }
            settings.AddItem(new MenuItem("e.method", "E Türü").SetValue(new StringList(new[] { "Mouse Pozisyonu" })));
            settings.AddItem(new MenuItem("use.r", "(R) Kullan").SetValue(true));
            settings.AddItem(new MenuItem("r.whte", "                          R Beyaz Liste"));
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly))
            {
                settings.AddItem(new MenuItem("respite." + ally.CharData.BaseSkinName, string.Format("(R): {0}", ally.CharData.BaseSkinName)).SetValue(Program.HighChamps.Contains(ally.CharData.BaseSkinName)));

            }
            settings.AddItem(new MenuItem("min.hp.for.r", "(R) İçin Minimum Can Yüzdesi").SetValue(new Slider(20, 1, 99)));
            Program.Config.AddSubMenu(settings);
        }
        public static void PortugueseMenu()
        {
            var settings = new Menu(":: Settings", ":: Settings");
            {
                var comboMenu = new Menu("Definições de combo", "Definições de combo");
                {
                    comboMenu.AddItem(new MenuItem("q.combo.style", "(Q) Combo Style").SetValue(new StringList(new[] { "Kite", "100% de Precisão", "Posição Segura" })));
                    comboMenu.AddItem(new MenuItem("q.combo", "Usar (Q)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("w.combo", "Usar (W)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e.combo", "Usar (E)").SetValue(true));
                    settings.AddSubMenu(comboMenu);
                }
                var eMenu = new Menu("(E) Definições", "(E) Definições");
                {
                    eMenu.AddItem(new MenuItem("e.whte", "                     (E) Beyaz Liste"));
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                    {
                        eMenu.AddItem(new MenuItem("enemy." + enemy.CharData.BaseSkinName, string.Format("E: {0}", enemy.CharData.BaseSkinName)).SetValue(Program.HighChamps.Contains(enemy.CharData.BaseSkinName)));

                    }
                    settings.AddSubMenu(eMenu);
                }
                var harassMenu = new Menu("(E) Definições", "(E) Definições");
                {
                    harassMenu.AddItem(new MenuItem("q.harass", "Usar (Q)").SetValue(true));
                    harassMenu.AddItem(new MenuItem("w.harass", "Usar (W)").SetValue(true));
                    harassMenu.AddItem(new MenuItem("e.harass", "Usar (E)").SetValue(true));
                    harassMenu.AddItem(new MenuItem("harass.mana", "Gestor de mana").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(harassMenu);
                }
                var laneClear = new Menu("Definições de limpeza de lane", "Definições de limpeza de lane");
                {
                    laneClear.AddItem(new MenuItem("q.clear", "Usar (Q)").SetValue(true));
                    laneClear.AddItem(new MenuItem("q.minion.count", "Q Minion Count").SetValue(new Slider(4, 1, 5)));
                    laneClear.AddItem(new MenuItem("clear.mana", "Gestor de mana").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(laneClear);
                }
                var jungleClear = new Menu("Definições da jungle", "Definições da jungle");
                {
                    jungleClear.AddItem(new MenuItem("q.jungle", "Usar (Q)").SetValue(true));
                    jungleClear.AddItem(new MenuItem("w.jungle", "Usar (W)").SetValue(true));
                    jungleClear.AddItem(new MenuItem("e.jungle", "Usar (E)").SetValue(true));
                    jungleClear.AddItem(new MenuItem("jungle.mana", "Gestor de mana").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(jungleClear);
                }
                var ksMenu = new Menu("Definições de killsteal", "Definições de killsteal");
                {
                    ksMenu.AddItem(new MenuItem("q.ks", "Usar (Q)").SetValue(true));
                    ksMenu.AddItem(new MenuItem("q.ks.count", "Contagem de ataques básicos").SetValue(new Slider(2, 1, 5)));
                    settings.AddSubMenu(ksMenu);
                }
                var miscMenu = new Menu("Variado", "Variado");
                {
                    miscMenu.AddItem(new MenuItem("q.antigapcloser", "Anti-Gapcloser Q").SetValue(true));
                    var antiRengar = new Menu("Anti Rengar", "Anti Rengar");
                    {
                        antiRengar.AddItem(new MenuItem("anti.rengar", "Anti Rengar!").SetValue(true));
                        antiRengar.AddItem(new MenuItem("hp.percent.for.rengar", "Percentagem minima de HP").SetValue(new Slider(30, 1, 99)));
                        miscMenu.AddSubMenu(antiRengar);
                    }
                    var spellMenu = new Menu("Quebrador de habilidades", "Quebrador de habilidades");
                    {
                        spellMenu.AddItem(new MenuItem("spell.broker", "Quebrador de habilidades!").SetValue(true));
                        spellMenu.AddItem(new MenuItem("katarina.r", "Katarina (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("missfortune.r", "Miss Fortune (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("lucian.r", "Lucian (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("hp.percent.for.broke", "Spell Bozma için Minimum Can Yüzdesi").SetValue(new Slider(20, 1, 99)));
                        miscMenu.AddSubMenu(spellMenu);
                    }
                    var rProtector = new Menu("(R) Proteção", "(R) Proteção");
                    {
                        rProtector.AddItem(new MenuItem("protector", "Desativar proteção?").SetValue(true));
                        foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                        {
                            foreach (var skillshot in SpellDatabase.Spells.Where(x => x.charName == enemy.ChampionName)) // 2.5F Protector
                            {
                                rProtector.AddItem(new MenuItem("hero." + skillshot.spellName, "" + skillshot.charName + "(" + skillshot.spellKey + ")").SetValue(true));
                            }
                        }
                        miscMenu.AddSubMenu(rProtector);
                    }
                    settings.AddSubMenu(miscMenu);
                }
            }

            var drawMenu = new Menu("Definições de desenhos", "Definições de desenhos");
            {
                var damageDraw = new Menu("Desenho do dan", "Desenho do dano");
                {
                    damageDraw.AddItem(new MenuItem("aa.indicator", "AA Indicator").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    drawMenu.AddSubMenu(damageDraw);
                }
                drawMenu.AddItem(new MenuItem("q.drawx", "Q Range").SetValue(new Circle(true, System.Drawing.Color.White)));
                drawMenu.AddItem(new MenuItem("w.draw", "W Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                drawMenu.AddItem(new MenuItem("e.draw", "E Range").SetValue(new Circle(true, System.Drawing.Color.DodgerBlue)));
                drawMenu.AddItem(new MenuItem("r.draw", "R Range").SetValue(new Circle(true, System.Drawing.Color.GreenYellow)));
                settings.AddSubMenu(drawMenu);
            }
            settings.AddItem(new MenuItem("e.method", "E Method").SetValue(new StringList(new[] { "Mouse Pozisyonu" })));
            settings.AddItem(new MenuItem("use.r", "Usar R").SetValue(true));
            settings.AddItem(new MenuItem("r.whte", "                          R Excepções"));
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly))
            {
                settings.AddItem(new MenuItem("respite." + ally.CharData.BaseSkinName, string.Format("(R): {0}", ally.CharData.BaseSkinName)).SetValue(Program.HighChamps.Contains(ally.CharData.BaseSkinName)));

            }
            settings.AddItem(new MenuItem("min.hp.for.r", "(R) Percentagem minima de HP").SetValue(new Slider(20, 1, 99)));
            Program.Config.AddSubMenu(settings);
        }
        public static void FrenchMenu()
        {
            var settings = new Menu(":: Settings", ":: Settings");
            {
                var comboMenu = new Menu("Option Combo", "Option Combo");
                {
                    comboMenu.AddItem(new MenuItem("q.combo.style", "(Q) Combo Style").SetValue(new StringList(new[] { "Kite l'ennmie", "100% hit l'ennemie", "Position Safe " })));
                    comboMenu.AddItem(new MenuItem("q.combo", "Utiliser (Q)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("w.combo", "Utiliser (W)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e.combo", "Utiliser (E)").SetValue(true));
                    settings.AddSubMenu(comboMenu);
                }
                var eMenu = new Menu("(E) Option", "(E) Settings");
                {
                    eMenu.AddItem(new MenuItem("e.whte", "                     (E) Whitelist du E"));
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                    {
                        eMenu.AddItem(new MenuItem("enemy." + enemy.CharData.BaseSkinName, string.Format("E: {0}", enemy.CharData.BaseSkinName)).SetValue(Program.HighChamps.Contains(enemy.CharData.BaseSkinName)));

                    }
                    settings.AddSubMenu(eMenu);
                }
                var harassMenu = new Menu("Option Harass", "Option Harass");
                {
                    harassMenu.AddItem(new MenuItem("q.harass", "Utiliser (Q)").SetValue(true));
                    harassMenu.AddItem(new MenuItem("w.harass", "Utiliser (W)").SetValue(true));
                    harassMenu.AddItem(new MenuItem("e.harass", "Utiliser (E)").SetValue(true));
                    harassMenu.AddItem(new MenuItem("harass.mana", "Mana Manager").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(harassMenu);
                }
                var laneClear = new Menu("Option du lane-clear", "Option du lane-clear");
                {
                    laneClear.AddItem(new MenuItem("q.clear", "Utiliser (Q)").SetValue(true));
                    laneClear.AddItem(new MenuItem("q.minion.count", "Combien de sbire pour que le A s'active aux minimum").SetValue(new Slider(4, 1, 5)));
                    laneClear.AddItem(new MenuItem("clear.mana", "Mana Manager").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(laneClear);
                }
                var jungleClear = new Menu("Option du jungle-clear", "Option du jungle-clear");
                {
                    jungleClear.AddItem(new MenuItem("q.jungle", "Utiliser (Q)").SetValue(true));
                    jungleClear.AddItem(new MenuItem("w.jungle", "Utiliser (W)").SetValue(true));
                    jungleClear.AddItem(new MenuItem("e.jungle", "Utiliser (E)").SetValue(true));
                    jungleClear.AddItem(new MenuItem("jungle.mana", "Mana Manager").SetValue(new Slider(20, 1, 99)));
                    settings.AddSubMenu(jungleClear);
                }
                var ksMenu = new Menu("Parametre du killsteal", "Parametre du killsteal");
                {
                    ksMenu.AddItem(new MenuItem("q.ks", "Utiliser Q pour KS").SetValue(true));
                    ksMenu.AddItem(new MenuItem("q.ks.count", "Compteur d'auto attaque").SetValue(new Slider(2, 1, 5)));
                    settings.AddSubMenu(ksMenu);
                }
                var miscMenu = new Menu("Divers option", "Divers option");
                {
                    miscMenu.AddItem(new MenuItem("q.antigapcloser", "Anti-Gapcloser Q!").SetValue(true));
                    var antiRengar = new Menu("Anti Rengar", "Anti Rengar");
                    {
                        antiRengar.AddItem(new MenuItem("anti.rengar", "Anti Rengar!").SetValue(true));
                        antiRengar.AddItem(new MenuItem("hp.percent.for.rengar", "Min.HP Pourcentage").SetValue(new Slider(30, 1, 99)));
                        miscMenu.AddSubMenu(antiRengar);
                    }
                    var spellMenu = new Menu("Casse les gros ultime", "Casse les gros ultime");
                    {
                        spellMenu.AddItem(new MenuItem("spell.broker", "Spell Breaker!").SetValue(true));
                        spellMenu.AddItem(new MenuItem("katarina.r", "Katarina (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("missfortune.r", "Miss Fortune (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("lucian.r", "Lucian (R)").SetValue(true));
                        spellMenu.AddItem(new MenuItem("hp.percent.for.broke", "Min. HP Pourcentage").SetValue(new Slider(20, 1, 99)));
                        miscMenu.AddSubMenu(spellMenu);
                    }
                    var rProtector = new Menu("(R) Protection", "(R) Protection");
                    {
                        rProtector.AddItem(new MenuItem("protector", "Supprimer la protection?").SetValue(true));
                        foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                        {
                            foreach (var skillshot in SpellDatabase.Spells.Where(x => x.charName == enemy.ChampionName)) // 2.5F Protector
                            {
                                rProtector.AddItem(new MenuItem("hero." + skillshot.spellName, "" + skillshot.charName + "(" + skillshot.spellKey + ")").SetValue(true));
                            }
                        }
                        miscMenu.AddSubMenu(rProtector);
                    }
                    settings.AddSubMenu(miscMenu);
                }
            }

            var drawMenu = new Menu("Option visuel", "Option visuel");
            {
                var damageDraw = new Menu("Visuel des degats", "Visuel des degats");
                {
                    damageDraw.AddItem(new MenuItem("aa.indicator", "AA Indicator").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    drawMenu.AddSubMenu(damageDraw);
                }
                drawMenu.AddItem(new MenuItem("q.drawx","Range du (Q)").SetValue(new Circle(true, System.Drawing.Color.White)));
                drawMenu.AddItem(new MenuItem("w.draw", "Range du (W)").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                drawMenu.AddItem(new MenuItem("e.draw", "Range du (E)").SetValue(new Circle(true, System.Drawing.Color.DodgerBlue)));
                drawMenu.AddItem(new MenuItem("r.draw", "Range du (R)").SetValue(new Circle(true, System.Drawing.Color.GreenYellow)));
                settings.AddSubMenu(drawMenu);
            }
            settings.AddItem(new MenuItem("e.method", "Methode pour utiliser le E").SetValue(new StringList(new[] { "Position du curseur" })));
            settings.AddItem(new MenuItem("use.r", "Utiliser (R)").SetValue(true));
            settings.AddItem(new MenuItem("r.whte", "                          R Whitelist"));
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly))
            {
                settings.AddItem(new MenuItem("respite." + ally.CharData.BaseSkinName, string.Format("Respite: {0}", ally.CharData.BaseSkinName)).SetValue(Program.HighChamps.Contains(ally.CharData.BaseSkinName)));

            }
            settings.AddItem(new MenuItem("min.hp.for.r", "Mini % hp pour utiliser le R").SetValue(new Slider(20, 1, 99)));
            Program.Config.AddSubMenu(settings);
        }

        public static bool IsEnglish()
        {
            return Program.Config.Item("language.supx").GetValue<StringList>().SelectedIndex == 0;
        }
        public static bool IsKorean()
        {
            return Program.Config.Item("language.supx").GetValue<StringList>().SelectedIndex == 1;
        }
    }
}
