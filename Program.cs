// TODO: Build obstacle inserting mechanism
// TODO: Build game end state

using System.Text;

class Program
{
    private static char m_playerCharacter = '\u2588';
    private static char[] m_playerLine = new[] {' ', ' ', ' ', ' ', ' ', ' ', ' '};
    private static int m_playerPosition = 3;

    private static ConsoleKeyInfo m_Key = new();
    private static int m_RefreshRate = 16; // ~60fps
    private static bool m_GameEnded;
    private static int m_height = 15;
    private static int m_width = 7;
    private static char[,] m_gameWorld = new char[15,7] 
    {
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' '},
    };

    private static void Main(string[] args)
    {
        int counter = 0;
        Thread WatchKeyThread = new(WatchKeys);
        Thread gameThread = new(GameLoop);

        Console.Clear();
        WatchKeyThread.Start();
        gameThread.Start();

        void WatchKeys()
        {
            while (m_Key.Key != ConsoleKey.Q && !m_GameEnded)
            {
                m_Key = Console.ReadKey(true);
            }
        }

        void GameLoop()
        {
            while (m_Key.Key != ConsoleKey.Q && !m_GameEnded)
            {
                Console.WriteLine($"Key: {m_Key.Key} | ticks: {counter}");

                if (m_Key.Key == ConsoleKey.H && m_playerPosition != 0) {
                    m_playerLine[m_playerPosition] = ' ';
                    m_playerPosition--;
                } 
                else if (m_Key.Key == ConsoleKey.L && m_playerPosition != 6)
                {
                    m_playerLine[m_playerPosition] = ' ';
                    m_playerPosition++;
                }
                m_Key = new();
                
                // Gameworld Drawing Logic
                StringBuilder builder = new();
                for (int i = 0; i < m_height; i++)
                {
                    builder.Append('|');
                    for (int j = 0; j < m_width; j++)
                    {
                        builder.Append(m_gameWorld[i,j]);
                    }
                    builder.Append("|");
                    Console.WriteLine(builder);
                    builder.Clear();
                }

                m_playerLine[m_playerPosition] = m_playerCharacter;
                Console.WriteLine("|" + string.Concat(m_playerLine)+ "|");

                counter++;
                Thread.Sleep(m_RefreshRate);
                Console.Clear();
            }
            m_GameEnded = true;
        }
    }
}
