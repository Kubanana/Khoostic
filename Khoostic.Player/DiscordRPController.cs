using DiscordRPC;

namespace Khoostic.Player
{
    public static class DiscordRPController
    {
        private static DiscordRpcClient _client;

        public static void Init()
        {
            _client = new DiscordRpcClient("1378431160027451534");
            _client.Initialize();
        }

        public static void UpdateSongPresence(string title, string artist)
        {
            _client.SetPresence(new RichPresence()
            {
                Details = $"Listening to {title}",
                State = $"By {artist}",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Khoostic"
                },

                Timestamps = Timestamps.Now
            });
        }

        public static void ClearPresence()
        {
            _client.ClearPresence();
        }

        public static void Shutdown()
        {
            _client.Dispose();
        }
    }
}