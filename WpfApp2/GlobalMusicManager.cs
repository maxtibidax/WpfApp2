using System;
using System.IO;
using NAudio.Wave;

namespace WpfApp2
{
    public class LoopStream : WaveStream
    {
        private readonly WaveStream sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            this.EnableLooping = true;
        }

        public bool EnableLooping { get; set; }

        public override WaveFormat WaveFormat => sourceStream.WaveFormat;

        public override long Length => sourceStream.Length;

        public override long Position
        {
            get => sourceStream.Position;
            set => sourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (!EnableLooping)
                        break;

                    sourceStream.Position = 0;
                    continue;
                }
                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                sourceStream?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public static class GlobalMusicManager
    {
        private static WaveOutEvent waveOut;
        private static LoopStream loopStream;
        private static float currentVolume = (float)SettingsControl.MusicVolume;

        public static void PlayMusic(string filePath, bool loop, float volume)
        {
            try
            {
                Stop();

                if (!File.Exists(filePath))
                {
                    System.Windows.MessageBox.Show($"Музыкальный файл не найден: {filePath}");
                    return;
                }

                var reader = new Mp3FileReader(filePath);
                loopStream = new LoopStream(reader)
                {
                    EnableLooping = loop
                };

                waveOut = new WaveOutEvent();
                waveOut.Init(loopStream);
                waveOut.Volume = volume;
                currentVolume = volume;
                waveOut.Play();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка воспроизведения музыки: {ex.Message}");
            }
        }

        public static void Stop()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
            if (loopStream != null)
            {
                loopStream.Dispose();
                loopStream = null;
            }
        }

        public static void SetVolume(float volume)
        {
            currentVolume = volume;
            if (waveOut != null)
            {
                waveOut.Volume = volume;
            }
        }
    }
}