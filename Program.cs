using System.Text;

class Program
{
    private const char PLAYER_SYMBOL = '\u2588';
    private const int PLAYER_STARTING_POSTITION = 3;
    private const char OBSTACLE_SYMBOL = '\u2580';
    private const int MAXIMUM_OBSTACLE_TAPE_LENGTH = 240;
    private const int HEIGHT = 15;
    private const int WIDTH = 7;
    private const int REFRESH_RATE = 16; // ~60fps
    
    private static bool m_gameEnded;
    private static char[,] m_gameWorld = new char[HEIGHT,WIDTH]
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

    private static char[] m_playerLine = new char[WIDTH] {' ', ' ', ' ', ' ', ' ', ' ', ' '};
    private static int m_playerPosition = 3;
    private static ConsoleKeyInfo m_playersKeyPressedInfo;
    
    private static List<bool[]> m_obstacleInputTape = new();
    private static int m_obstacleInputTapeReadHead;
    private static List<Obstacle> m_obstacles = new();
    
    private static void Main(string[] args)
    {
        int currentGameTick = 0;
        m_obstacleInputTape = GenerateObstacleTape();

        Thread watchKeyThread = new(WatchKeys);
        Thread gameThread = new(GameLoop);

        Console.Clear();
        watchKeyThread.Start();
        gameThread.Start();

        void WatchKeys()
        {
            while (m_playersKeyPressedInfo.Key != ConsoleKey.Q && !m_gameEnded)
            {
                m_playersKeyPressedInfo = Console.ReadKey(true);
            }
        }

        void GameLoop()
        {
            while (m_playersKeyPressedInfo.Key != ConsoleKey.Q && !m_gameEnded)
            {
                if (m_playersKeyPressedInfo.Key == ConsoleKey.H && m_playerPosition != 0) {
                    m_playerLine[m_playerPosition] = ' ';
                    m_playerPosition--;
                }
                else if (m_playersKeyPressedInfo.Key == ConsoleKey.L && m_playerPosition != 6)
                {
                    m_playerLine[m_playerPosition] = ' ';
                    m_playerPosition++;
                }
                m_playersKeyPressedInfo = new ConsoleKeyInfo();

                if (ShouldUpdateGameWorld(currentGameTick))
                {
                    // Obstacle Insertion
                    for (int i = 0; i < m_obstacleInputTape[m_obstacleInputTapeReadHead].Length; i++)
                    {
                        if (m_obstacleInputTape[m_obstacleInputTapeReadHead][i])
                        {
                            m_obstacles.Add(new Obstacle(i));
                        }
                    }

                    if (m_obstacleInputTape.Count > 0)
                    {
                        m_obstacleInputTapeReadHead++;
                    }

                    // Obstacle management
                    for (int i = 0; i < m_obstacles.Count; i++) 
                    {
                        if (m_obstacles[i].m_firstSpawn)
                        {
                            m_obstacles[i].m_firstSpawn = false;
                            m_obstacles[i].m_yPosition++;
                            m_gameWorld[m_obstacles[i].m_yPosition, m_obstacles[i].m_xPosition] = OBSTACLE_SYMBOL;
                        }
                        else if (m_obstacles[i].m_yPosition == HEIGHT - 1)
                        {
                            m_gameWorld[m_obstacles[i].m_yPosition, m_obstacles[i].m_xPosition] = ' ';
                            m_obstacles.Remove(m_obstacles[i]);
                        }
                        else
                        {
                            m_gameWorld[m_obstacles[i].m_yPosition, m_obstacles[i].m_xPosition] = ' ';
                            m_obstacles[i].m_yPosition++;
                            m_gameWorld[m_obstacles[i].m_yPosition, m_obstacles[i].m_xPosition] = OBSTACLE_SYMBOL;
                        }
                    }
                }

                // Gameworld Drawing Logic
                StringBuilder builder = new();
                for (int i = 0; i < HEIGHT; i++)
                {
                    builder.Append('|');
                    for (int j = 0; j < WIDTH; j++)
                    {
                        builder.Append(m_gameWorld[i,j]);
                    }
                    builder.Append('|');
                    Console.WriteLine(builder);
                    builder.Clear();
                }

                m_playerLine[m_playerPosition] = PLAYER_SYMBOL;
                Console.WriteLine("|-------|");
                Console.WriteLine("|" + string.Concat(m_playerLine)+ "|");

                // Game End State
                for (int i = 0; i < m_obstacles.Count; i++) 
                {
                    if (m_obstacles[i].m_yPosition == HEIGHT - 1
                            && m_playerPosition == m_obstacles.First().m_xPosition) 
                    {
                        Console.WriteLine("Collision Occurred!");
                        m_gameEnded = true;
                    }
                }

                if (m_obstacleInputTapeReadHead == MAXIMUM_OBSTACLE_TAPE_LENGTH) // kinda bung logic but eh... 
                {
                    Console.WriteLine("Game Over! You Win!");
                }

                currentGameTick++;
                Thread.Sleep(REFRESH_RATE);
                Console.Clear();
            }
            m_gameEnded = true;
        }

        bool ShouldUpdateGameWorld(int currentTick)
        {
            return currentTick % 5 == 0;
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

    static List<bool[]> GenerateObstacleTape()
    {
        Random rng = new();
        List<bool[]> gameWorld = new();

        int obstacleCooldown = 0;

        for (int i = 0; i < MAXIMUM_OBSTACLE_TAPE_LENGTH; i++)
        {
            bool[] row = new bool[WIDTH];
            if (obstacleCooldown <= 0)
            {
                int obstacleCount = rng.Next(1, 4); // Up to 3 obstacles per line
                for (int j = 0; j < obstacleCount; j++)
                {
                    int obstaclePos;
                    do
                    {
                        obstaclePos = rng.Next(WIDTH);
                    } while (row[obstaclePos]); // Ensure we don't overwrite an existing obstacle

                    row[obstaclePos] = true;
                }
                obstacleCooldown = 3;
            }
            else
            {
                obstacleCooldown--;
            }

            gameWorld.Add(row);
        }

        return gameWorld;
    }
}
