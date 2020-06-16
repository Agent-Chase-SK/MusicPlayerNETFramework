using MusicPlayerAPI;
using MusicPlayerAPI.Players;
using MusicPlayerAPI.SongList;
using System;
using System.Collections.Generic;

namespace ConsoleForTesting
{
    internal class Program
    {
        private static void Main()
        {
            Loop();
        }

        private static void Loop()
        {
            MusicPlayerHandler musicPlayer = new MusicPlayerHandler(new NAudioPlayer(), new SimpleRecursiveSongList());
            Console.WriteLine("Folder path:");
            string path = Console.ReadLine();
            musicPlayer.LoadSongs(path);

            HandleSongSelection(musicPlayer);

            while (true)
            {
                Console.WriteLine("play stop pause load volume exit");
                string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "play":
                        musicPlayer.Play();
                        break;
                    case "stop":
                        musicPlayer.Stop();
                        break;
                    case "pause":
                        musicPlayer.Pause();
                        break;
                    case "exit":
                        musicPlayer.Dispose();
                        return;
                    case "load":
                        HandleSongSelection(musicPlayer);
                        break;
                    case "volume":
                        HandleVolume(musicPlayer);
                        break;
                    default:
                        break;
                }
            }
        }

        private static void HandleVolume(MusicPlayerHandler musicPlayer)
        {
            Console.WriteLine($"Volume is: {musicPlayer.GetVolume()}, new volume (0-100): ");
            int volume = int.Parse(Console.ReadLine());
            musicPlayer.SetVoume(volume);
        }

        private static void PrintSongList(IList<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine($"{i}: {list[i]}");
            }
        }

        private static void HandleSongSelection(MusicPlayerHandler musicPlayer)
        {
            PrintSongList(musicPlayer.GetSongList());
            Console.WriteLine("Select song (number):");
            int songNum = int.Parse(Console.ReadLine());
            musicPlayer.SelectSong(musicPlayer.GetSongList()[songNum]);
        }
    }
}