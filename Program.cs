using System.Text;

class Program
{
    private static char m_playerChar = '\u2588';
    private static char m_obstacleChar = '\u2580';

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

    private static char[] m_playerLine = new[] {' ', ' ', ' ', ' ', ' ', ' ', ' '};
    private static int m_playerPosition = 3;
    private static ConsoleKeyInfo m_playersKeyPressedInfo = new();
    private static List<bool[]> m_obstacleInputTape = new();
    private static int m_obstacleInputTapeReadHead = 0;

    private static List<Obstacle> m_obstacles = new();
    private static bool m_collision;

    private static void Main(string[] args)
    {
        int currentGameTick = 0;
        m_obstacleInputTape = GenerateGameWorld();

        Thread watchKeyThread = new(WatchKeys);
        Thread gameThread = new(GameLoop);

        Console.Clear();
        watchKeyThread.Start();
        gameThread.Start();

        void WatchKeys()
        {
            while (!m_GameEnded)
            {
                m_playersKeyPressedInfo = Console.ReadKey(true);
            }
        }

        void GameLoop()
        {
            while (true)
            {
                Console.WriteLine($"Key: {m_playersKeyPressedInfo.Key} | ticks: {currentGameTick} | ReadHeadValue: {m_obstacleInputTapeReadHead}");

                if (m_playersKeyPressedInfo.Key == ConsoleKey.H && m_playerPosition != 0) {
                    m_playerLine[m_playerPosition] = ' ';
                    m_playerPosition--;
                }
                else if (m_playersKeyPressedInfo.Key == ConsoleKey.L && m_playerPosition != 6)
                {
                    m_playerLine[m_playerPosition] = ' ';
                    m_playerPosition++;
                }
                m_playersKeyPressedInfo = new();

                // Obstacle Insertion
                if (ShouldUpdateGameWorld(currentGameTick) 
                        && m_obstacleInputTapeReadHead != m_obstacleInputTape.Count)
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
                if (currentGameTick % 5 == 0)
                {
                    for (int i = 0; i < m_obstacles.Count; i++) 
                    {
                        Obstacle obstacle = m_obstacles[i];
                        if (obstacle.firstSpawn)
                        {
                            obstacle.firstSpawn = false;
                            obstacle.yPosition++;
                            m_gameWorld[obstacle.yPosition, obstacle.xPosition] = m_obstacleChar;
                        }
                        else if (obstacle.yPosition == m_height - 1)
                        {
                            m_gameWorld[obstacle.yPosition, obstacle.xPosition] = ' ';
                            m_obstacles.Remove(obstacle);
                        }
                        else
                        {
                            obstacle.yPosition++;
                            m_gameWorld[obstacle.yPosition, obstacle.xPosition] = m_obstacleChar;
                            m_gameWorld[obstacle.yPosition - 1, obstacle.xPosition] = ' ';
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
                    builder.Append("|\n");
                }
                Console.WriteLine(builder);
                builder.Clear();

                m_playerLine[m_playerPosition] = m_playerChar;
                Console.WriteLine("|-------|");
                Console.WriteLine("|" + string.Concat(m_playerLine)+ "|");

                currentGameTick++;

                // Game End State
                if (currentGameTick % 5 == 0 
                        && m_obstacles.Count > 0
                        && m_obstacles.First().yPosition == m_height - 1
                        && m_playerPosition == m_obstacles.First().xPosition) 
                {
                    Console.WriteLine("Collision Occurred!");
                    break;
                }

                    if (m_obstacleInputTapeReadHead == 240) // kinda bung logic but eh... 
                {
                    Console.WriteLine("Game Over! You Win!");
                }
                Thread.Sleep(m_RefreshRate);
                Console.Clear();
            }
            
            m_GameEnded = true;
        }

        bool ShouldUpdateGameWorld(int currentTick)
        {
            return currentTick % 5 == 0;
        }

    }

    class Obstacle
    {
        public int yPosition { get; set; }
        public int xPosition { get; set; }
        public bool firstSpawn { get; set; }

        public Obstacle(int xPosition)
        {
            this.xPosition = xPosition;
            yPosition = -1;
            firstSpawn = true;
        }
    }

    static List<bool[]> GenerateGameWorld()
    {
        Random rng = new Random();
        List<bool[]> gameWorld = new List<bool[]>();

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
