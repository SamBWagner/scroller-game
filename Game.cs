namespace scroller_game;

public class Game
{
    private const int HEIGHT = 15;
    private const int WIDTH = 7;
    private const int REFRESH_RATE = 16; // ~60fps
    private const int LEVEL_LENGTH = 240;
    private const int PLAYER_STARTING_POSITION = 3; 
    
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
    char[] m_playersArea = new char[WIDTH] {' ', ' ', ' ', ' ', ' ', ' ', ' '};
    bool m_gameEnded;
    ConsoleKeyInfo m_playersKeyPressedInfo;

    public void Run()
    {
        int currentGameTick = 0;
        EntityManager entityManager = new(
            levelLength: LEVEL_LENGTH, 
            levelWidth: WIDTH, 
            playerStartingPosition: PLAYER_STARTING_POSITION);
        Drawer drawer = new(HEIGHT, WIDTH);

        Thread watchKeyThread = new(WatchKeys);
        Thread gameThread = new(GameLoop);

        Console.Clear();
        watchKeyThread.Start();
        gameThread.Start();

        void WatchKeys()
        {
            while (m_playersKeyPressedInfo.Key != Config.QuitKey && !m_gameEnded)
            {
                m_playersKeyPressedInfo = Console.ReadKey(true);
            }
        }

        void GameLoop()
        {
            while (m_playersKeyPressedInfo.Key != Config.QuitKey && !m_gameEnded)
            {
                entityManager.UpdatePlayerPosition(m_playersArea, m_playersKeyPressedInfo.Key);
                m_playersKeyPressedInfo = new ConsoleKeyInfo();
                if (ShouldUpdateGameWorld(currentGameTick))
                {
                    entityManager.AddNextLineOfObstacles();
                    entityManager.UpdateObstacles(m_gameWorld, HEIGHT);
                }
                drawer.Draw(m_gameWorld, m_playersArea);

                if (m_gameEnded) 
                { 
                    break; 
                }

                if (entityManager.LevelLost())
                {
                    Console.WriteLine("Game Over! You Lose!");
                    break;
                }

                if (entityManager.LevelWon()) 
                {
                    Console.WriteLine("Game Over! You Win!");
                    break;
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