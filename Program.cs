class Program
{
    private static char m_playerCharacter = '\u2588';
    private static char[] m_playerLine = new[] {' ', ' ', ' ', ' ', ' ', ' ', ' '};
    private static int m_playerPosition = 3;

    private static ConsoleKeyInfo Key = new();
    private static int REFRESH_RATE = 16;
    private static bool GameEnded;

    private static void Main(string[] args)
    {
        int counter = 0;
        Console.Clear();
        var LastKeyThread = new Thread(WatchKeys);
        LastKeyThread.Start();

        var gameThread = new Thread(GameLoop);
        gameThread.Start();

        void WatchKeys()
        {
            while (!GameEnded)
            {
                Key = Console.ReadKey(true);
                Thread.Sleep(REFRESH_RATE);
            }
        }

        void GameLoop()
        {
            while (Key.Key != ConsoleKey.Q && !GameEnded)
            {
                Console.WriteLine($"Key: {Key.Key} | ticks: {counter}");

                if (Key.Key == ConsoleKey.H && m_playerPosition != 0) {
                    m_playerLine[m_playerPosition] = ' ';
                    m_playerPosition--;
                    Key = new();
                } 
                else if (Key.Key == ConsoleKey.L && m_playerPosition != 6)
                {
                    m_playerLine[m_playerPosition] = ' ';
                    m_playerPosition++;
                    Key = new();
                }

                Draw(playerPosition: m_playerPosition, width: 7, height: 15);
                Thread.Sleep(REFRESH_RATE);
                Console.Clear();
            }
            GameEnded = true;
        }

        void Draw(int playerPosition, int width, int height)
        {
            for(int i = 0; i < height - 1; i++)
            {
                Console.WriteLine();
            }
            m_playerLine[playerPosition] = m_playerCharacter;
            Console.WriteLine(string.Concat(m_playerLine));
        }
    }
}
