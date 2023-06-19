using System.Text;

namespace scroller_game;

public class Game
{
    // Parts of the Game
    private const int HEIGHT = 15;
    private const int WIDTH = 7;
    private const int REFRESH_RATE = 16; // ~60fps
    private const int LEVEL_LENGTH = 240;
    private const ConsoleKey QUIT_KEY = ConsoleKey.Q;
    private const ConsoleKey LEFT_KEY = ConsoleKey.H;
    private const ConsoleKey RIGHT_KEY = ConsoleKey.L;
    
    char[,] m_gameWorld = new char[HEIGHT,WIDTH]
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
    bool m_gameEnded;
    ConsoleKeyInfo m_playersKeyPressedInfo;
    
    //Players stuff?
    const char PLAYER_SYMBOL = '\u2588';
    const int PLAYER_STARTING_POSITION = 3; 
    char[] m_playerLine = new char[WIDTH] {' ', ' ', ' ', ' ', ' ', ' ', ' '};
    int m_playerPosition;
   
    public Game()
    {
        m_playerPosition = PLAYER_STARTING_POSITION;
    }

    public void Run()
    {
        int currentGameTick = 0;
        ObstacleManager obstacleManager = new(LEVEL_LENGTH);

        Thread watchKeyThread = new(WatchKeys);
        Thread gameThread = new(GameLoop);

        Console.Clear();
        watchKeyThread.Start();
        gameThread.Start();

        void WatchKeys()
        {
            while (m_playersKeyPressedInfo.Key != QUIT_KEY && !m_gameEnded)
            {
                m_playersKeyPressedInfo = Console.ReadKey(true);
            }
        }

        void GameLoop()
        {
            while (m_playersKeyPressedInfo.Key != QUIT_KEY && !m_gameEnded)
            {
                if (m_playersKeyPressedInfo.Key == LEFT_KEY && m_playerPosition != 0) {
                    m_playerLine[m_playerPosition] = ' ';
                    m_playerPosition--;
                }
                else if (m_playersKeyPressedInfo.Key == RIGHT_KEY && m_playerPosition != 6)
                {
                    m_playerLine[m_playerPosition] = ' ';
                    m_playerPosition++;
                }
                m_playersKeyPressedInfo = new ConsoleKeyInfo();

                if (ShouldUpdateGameWorld(currentGameTick))
                {
                    obstacleManager.AddNextLineOfObstacles();
                    obstacleManager.UpdateObstacles(ref m_gameWorld, HEIGHT);
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
                for (int i = 0; i < obstacleManager.m_obstacles.Count; i++) 
                {
                    if (obstacleManager.m_obstacles[i].m_yPosition == HEIGHT - 1
                            && m_playerPosition == obstacleManager.m_obstacles[i].m_xPosition) 
                    {
                        Console.WriteLine("Collision Occurred!");
                        m_gameEnded = true;
                        break;
                    }
                }

                if (m_gameEnded) 
                { 
                    break; 
                }

                if (obstacleManager.CompletedLevel()) 
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

}