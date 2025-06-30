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
        }
    }
}