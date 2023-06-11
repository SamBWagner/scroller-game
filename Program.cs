// TODO: Build obstacle inserting mechanism
// TODO: Build game end state

using System.Text;

class Program
{
    private static char m_playerCharacter = '\u2588';
    private static char m_obstacle = '\u2580';
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
    private static int[] obstacleInputTape = new int[] {0, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 0};
    private static int obstacleInputTapeReadHead = 0;

    private static void Main(string[] args)
    {
        int currentGameTick = 0;
        List<Obstacle> obstacles = new();

        Thread watchKeyThread = new(WatchKeys);
        Thread gameThread = new(GameLoop);

        Console.Clear();
        watchKeyThread.Start();
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
                Console.WriteLine($"Key: {m_Key.Key} | ticks: {currentGameTick}");

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

                // Obstacle Insertion
                if (ShouldUpdateGameWorld(currentGameTick) && obstacleInputTape.Length != 0 && !(obstacleInputTapeReadHead > obstacleInputTape.Length - 1))
                {
                    obstacles.Add(new Obstacle 
                            {
                                m_xPosition = obstacleInputTape[obstacleInputTapeReadHead],
                                m_yPosition = 0,
                                m_previousYPosition = -1 
                            });
                    obstacleInputTapeReadHead++;
                }

                // Obstacle placement
                if (ShouldUpdateGameWorld(currentGameTick) && obstacles.Count != 0)
                {
                    for (int i = 0; i < obstacles.Count; i++)
                    {
                        if (obstacles[i].m_yPosition == m_height)
                        {
                            m_gameWorld[obstacles[i].m_yPosition - 1, obstacles[i].m_xPosition] = ' ';
                            obstacles.RemoveAt(i);
                        }
                        m_gameWorld[obstacles[i].m_yPosition, obstacles[i].m_xPosition] = m_obstacle;
                        obstacles[i].m_yPosition++;

                        if (obstacles[i].m_previousYPosition != -1)
                        {
                            m_gameWorld[obstacles[i].m_previousYPosition, obstacles[i].m_xPosition] = ' ';
                        }
                        obstacles[i].m_previousYPosition = obstacles[i].m_yPosition - 1;
                    }
                }

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


                currentGameTick++;
                Thread.Sleep(m_RefreshRate);
                Console.Clear();
            }
            m_GameEnded = true;
        }

        bool ShouldUpdateGameWorld(int currentTick)
        {
            return currentTick % 15 == 0; 
        }

    }

    class Obstacle
    {
        public int m_xPosition { get; set; }
        public int m_previousYPosition { get; set; }
        public int m_yPosition { get; set; }
    }
}
