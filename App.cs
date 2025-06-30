using Khoostic.Player;

namespace Khoostic
{
    public class App
    {   
        public App()
        {

        }

        public void Run()
        {
            KhoosticPlayer player = new KhoosticPlayer();
            player.InitPlayer();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Khoostic");
                Console.WriteLine("");
                Console.WriteLine("r - Plays random song.");
                Console.WriteLine("exit - Exits the application.");
                Console.WriteLine("");
                Console.WriteLine($"Now playing: {player.CurrentSongName}");
                Console.WriteLine("");
                Console.Write("Enter command: ");

                string? input = Console.ReadLine()?.ToLower();

                if (input == "r")
                {
                    player.PlayRandomSong();
                }

                if (input == "exit")
                {
                    break;
                }
            }
        }
    }
}