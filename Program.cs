// TODO: Build obstacle inserting mechanism
// TODO: Build game end state

using System.Text;

class Program
{
    private static char m_playerCharacter = '\u2588';
    private static char m_obstacleCharacter = '\u2580';
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
        {' ', ' ', ' ', ' ', ' ', ' ', ' '}
    };
    private static List<bool[]> m_obstacleCharacterInputTape = new() 
    {
        new bool[7] { true, false, false, false, false, false, false},
        new bool[7] { false, true, false, false, false, false, false},
        new bool[7] { false, false, true, false, false, false, false},
        new bool[7] { false, false, false, true, false, false, false},
        new bool[7] { false, false, false, false, true, false, false},
        new bool[7] { false, false, false, false, false, true, false},
        new bool[7] { false, false, false, false, false, false, true},
        new bool[7] { false, false, false, false, false, true, false},
        new bool[7] { false, false, false, false, true, false, false},
        new bool[7] { false, false, false, true, false, false, false},
        new bool[7] { false, false, true, false, false, false, false},
        new bool[7] { false, true, false, false, false, false, false},
        new bool[7] { true, false, false, false, false, false, false},
        new bool[7] { false, false, false, false, false, false, false}
    };
    private static int m_obstacleCharacterInputTapeReadHead = 0;
    private static bool[] m_obstacleCharacterReadLine = new bool[7];

    private static List<Obstacle> obstacles = new();

    private static void Main(string[] args)
    {
        int currentGameTick = 0;

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
                Console.WriteLine($"Key: {m_Key.Key} | ticks: {currentGameTick} | ReadHeadValue: {m_obstacleCharacterInputTapeReadHead}");

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
                if (ShouldUpdateGameWorld(currentGameTick) && m_obstacleCharacterInputTapeReadHead != m_obstacleCharacterInputTape.Count)
                {
                    m_obstacleCharacterReadLine = m_obstacleCharacterInputTape[m_obstacleCharacterInputTapeReadHead];
                    for (int i = 0; i < m_obstacleCharacterReadLine.Length; i++)
                    {
                        if (m_obstacleCharacterReadLine[i])
                        {
                            obstacles.Add(new Obstacle(i));
                        }
                    }
                    if (m_obstacleCharacterInputTapeReadHead == m_width)
                    {
                        m_obstacleCharacterInputTapeReadHead = 0;
                    }
                    m_obstacleCharacterInputTapeReadHead++;
                }

                // Obstacle management
                if (ShouldUpdateGameWorld(currentGameTick))
                {
                    for (int i = 0; i < obstacles.Count; i++) 
                    {
                        if (obstacles[i].m_firstSpawn)
                        {
                            obstacles[i].m_firstSpawn = false;
                            obstacles[i].m_yPosition++;
                            m_gameWorld[obstacles[i].m_xPosition, obstacles[i].m_yPosition] = m_obstacleCharacter;
                        }
                        else if (!obstacles[i].m_firstSpawn)
                        {
                            obstacles[i].m_yPosition++;
                            m_gameWorld[obstacles[i].m_xPosition, obstacles[i].m_yPosition] = m_obstacleCharacter;
                            m_gameWorld[obstacles[i].m_xPosition, obstacles[i].m_yPosition - 1] = ' ';
                        }
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
        public int m_yPosition { get; set; }
        public int m_xPosition { get; set; }
        public bool m_firstSpawn { get; set; }

        public Obstacle(int xPosition)
        {
            m_xPosition = xPosition;
            m_yPosition = -1;
            m_firstSpawn = true;
        }
    }
}
