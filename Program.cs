namespace Game {
    class Program
    {
        public static ConsoleKeyInfo key = new();
        public static int REFRESH_RATE = 16;
        public static bool gameEnded;

        private static void Main(string[] args)
        {
            int counter = 0;
            GameDrawer drawer = new();
            Console.Clear();
            var keyThread = new Thread(WatchKeys);
            keyThread.Start();

            var gameThread = new Thread(GameLoop);
            gameThread.Start();

            void WatchKeys()
            {
                while (!gameEnded)
                {
                    key = Console.ReadKey(true);
                    Thread.Sleep(REFRESH_RATE);
                }
            }

            void GameLoop()
            {
                while (key.Key != ConsoleKey.Q && !gameEnded)
                {
                    Console.WriteLine($"action: {key.Key}\nticks: {counter}");
                    drawer.Draw(3);
                    Thread.Sleep(REFRESH_RATE);
                    Console.Clear();
                }
                Program.gameEnded = true;
            }
        }
    }
}
