using System;
using System.Windows.Media;

namespace Transliterator.Services;

public static class SoundPlayerService
{
    private static MediaPlayer mediaPlayer;

    private static byte volume;

    /// <summary>
    /// Volume level from 1 to 100
    /// </summary>
    public static byte Volume
    {
        get => volume;
        set
        {
            if (value > 100)
                volume = 100;
            else
                volume = value;

            // MediaPlayer volume is a double value between 0 and 1.
            mediaPlayer.Volume = volume / 100.0f;
        }
    }

    public static bool IsMuted { get; set; }

    static SoundPlayerService()
    {
        mediaPlayer = new MediaPlayer();
        Volume = 50;
    }

    public static void Play(string filePath)
    {
        if (IsMuted)
            return;

        mediaPlayer.Open(new Uri(filePath));
        mediaPlayer.Play();
    }
}