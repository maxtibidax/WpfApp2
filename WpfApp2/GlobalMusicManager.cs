using System;
using NAudio.Wave;

namespace WpfApp2
{
    public static class GlobalMusicManager
    {
        private static WaveOutEvent outputDevice;
        private static AudioFileReader audioFileReader;
        private static string currentTrack;

        public static void PlayMusic(string filePath, bool loop = true, double volume = 0.5)
        {
            // Если играет тот же трек, не перезапускаем
            if (currentTrack == filePath)
            {
                SetVolume((float)volume);
                return;
            }

            // Останавливаем текущую музыку
            Stop();

            try
            {
                outputDevice = new WaveOutEvent();
                audioFileReader = new AudioFileReader(filePath);

                // Устанавливаем громкость
                outputDevice.Volume = (float)volume;

                if (loop)
                {
                    // Создаем зацикленный поток
                    var loopStream = new LoopStream(audioFileReader);
                    outputDevice.Init(loopStream);
                }
                else
                {
                    outputDevice.Init(audioFileReader);
                }

                outputDevice.Play();
                currentTrack = filePath;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка воспроизведения музыки: {ex.Message}");
            }
        }

        public static void SetVolume(float volume)
        {
            if (outputDevice != null)
            {
                outputDevice.Volume = volume;
            }
        }

        public static void Stop()
        {
            if (outputDevice != null)
            {
                outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;
            }

            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }

            currentTrack = null;
        }
    }

    // Класс LoopStream остается прежним из предыдущей реализации
    public class LoopStream : WaveStream
    {
        private readonly WaveStream sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
        }

        public override WaveFormat WaveFormat => sourceStream.WaveFormat;
        public override long Length => long.MaxValue;
        public override long Position { get; set; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);

                if (bytesRead == 0)
                {
                    sourceStream.Position = 0; // Перезапуск потока
                    continue;
                }

                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }
    }
}