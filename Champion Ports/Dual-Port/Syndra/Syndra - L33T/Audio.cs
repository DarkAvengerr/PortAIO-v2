using System;
using System.Media;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SyndraL33T
{
    public class Audio
    {
        private static int _lastPlayerSoundTick = (int) (Game.Time * 0x3E8);

        public static void PlaySound(SoundPlayer sound = null)
        {
            if (sound != null)
            {
                try
                {
                    sound.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else if ((int) (Game.Time * 0x3E8) - _lastPlayerSoundTick > 45000 &&
                     EntryPoint.Menu.Item("l33t.stds.misc.sounds").GetValue<bool>())
            {
                var rnd = new Random();
                switch (rnd.Next(1, 11))
                {
                    case 1:
                        PlaySound(Sounds.ImKillingTheBitch);
                        break;
                    case 2:
                        PlaySound(Sounds.BallsToTheFace);
                        break;
                    case 3:
                        PlaySound(Sounds.DieFucker);
                        break;
                    case 4:
                        PlaySound(Sounds.GoingSomewhereAsshole);
                        break;
                    case 5:
                        PlaySound(Sounds.LoveThisGame);
                        break;
                    case 6:
                        PlaySound(Sounds.ImKillingTheBitch);
                        break;
                    case 7:
                        PlaySound(Sounds.OhDontYouDare);
                        break;
                    case 8:
                        PlaySound(Sounds.OhIdiot);
                        break;
                    case 9:
                        PlaySound(Sounds.WhosTheBitchNow);
                        break;
                    case 10:
                        PlaySound(Sounds.YourDeadMeatAssHole);
                        break;
                }
                _lastPlayerSoundTick = (int) (Game.Time * 0x3E8);
            }
        }
    }
}