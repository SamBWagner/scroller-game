class Program
{
    public static ConsoleKeyInfo key = new();
    public static int REFRESH_RATE = 16;
    public static bool gameEnded;

    private static void Main(string[] args)
    {
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
                Console.WriteLine($"action: {key.Key}");
                Thread.Sleep(REFRESH_RATE);
                Console.Clear();
            }
            Program.gameEnded = true;
        }
    }
}
