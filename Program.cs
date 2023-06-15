using System.Text;

class Program
{
    private const char PLAYER_CHARACTER = '\u2588';
    private const char OBSTACLE_CHARACTER = '\u2580';
    private const int HEIGHT = 15;
    private const int WIDTH = 7;
    private const int REFRESH_RATE = 16; // ~60fps
    
    private static bool m_gameEnded;
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

    private static char[] m_playerLine = new char[7] {' ', ' ', ' ', ' ', ' ', ' ', ' '};
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

                // Obstacle Insertion
                if (ShouldUpdateGameWorld(currentGameTick) && m_obstacleInputTapeReadHead != m_obstacleInputTape.Count)
                {
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
                }

                // Obstacle management
                if (ShouldUpdateGameWorld(currentGameTick))
                {
                    for (int i = 0; i < m_obstacles.Count; i++) 
                    {
                        if (m_obstacles[i].m_firstSpawn)
                        {
                            m_obstacles[i].m_firstSpawn = false;
                            m_obstacles[i].m_yPosition++;
                            m_gameWorld[m_obstacles[i].m_yPosition, m_obstacles[i].m_xPosition] = OBSTACLE_CHARACTER;
                        }
                        else if (m_obstacles[i].m_yPosition == HEIGHT - 1)
                        {
                            m_gameWorld[m_obstacles[i].m_yPosition, m_obstacles[i].m_xPosition] = ' ';
                            m_obstacles.Remove(m_obstacles[i]);
                        }
                        else
                        {
                            m_obstacles[i].m_yPosition++;
                            m_gameWorld[m_obstacles[i].m_yPosition, m_obstacles[i].m_xPosition] = OBSTACLE_CHARACTER;
                            m_gameWorld[m_obstacles[i].m_yPosition - 1, m_obstacles[i].m_xPosition] = ' ';
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

                m_playerLine[m_playerPosition] = PLAYER_CHARACTER;
                Console.WriteLine("|-------|");
                Console.WriteLine("|" + string.Concat(m_playerLine)+ "|");

                currentGameTick++;

                // Game End State
                if (ShouldUpdateGameWorld(currentGameTick) 
                        && m_obstacles.Count > 0
                        && m_obstacles.First().m_yPosition == HEIGHT - 1
                        && m_playerPosition == m_obstacles.First().m_xPosition) 
                {
                    Console.WriteLine("Collision Occurred!");
                    m_gameEnded = true;
                }

                if (m_obstacleInputTapeReadHead == 240) // kinda bung logic but eh... 
                {
                    Console.WriteLine("Game Over! You Win!");
                }
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

        // 240 to set the max game time to 1 minute
        for (int i = 0; i < 240; i++)
        {
            bool[] row = new bool[7];
            if (obstacleCooldown <= 0)
            {
                int obstacleCount = rng.Next(1, 4); // Up to 3 obstacles per line
                for (int j = 0; j < obstacleCount; j++)
                {
                    int obstaclePos;
                    do
                    {
                        obstaclePos = rng.Next(7);
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
