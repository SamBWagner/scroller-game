namespace scroller_game;

public class EntityManager
{
    private List<bool[]> m_level { get; }
    private int m_currentLevelRow { get; set; }
    private List<Obstacle> m_obstacles { get; }
    private int m_playerPosition;
    
    public EntityManager(
        int levelLength, 
        int levelWidth, 
        int playerStartingPosition)
    {
        m_level = GenerateLevel(levelLength, levelWidth);
        m_currentLevelRow = 0;
        m_obstacles = new List<Obstacle>();
        m_playerPosition = playerStartingPosition;
    }
    
    private List<bool[]> GenerateLevel(int levelLength, int levelWidth)
    {
        Random rng = new();
        List<bool[]> gameWorld = new();
        int obstacleCooldown = 0;

        for (int i = 0; i < levelLength; i++)
        {
            bool[] row = new bool[levelWidth];
            if (obstacleCooldown <= 0)
            {
                int obstacleCount = rng.Next(1, 4);
                for (int j = 0; j < obstacleCount; j++)
                {
                    int obstaclePos;
                    do
                    {
                        obstaclePos = rng.Next(levelWidth);
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

    public void UpdatePlayerPosition(char[] playerArea, ConsoleKey key)
    {
        if (key == Config.LeftKey && m_playerPosition != 0) {
            playerArea[m_playerPosition] = ' ';
            m_playerPosition--;
        }
        else if (key == Config.RightKey && m_playerPosition != 6)
        {
            playerArea[m_playerPosition] = ' ';
            m_playerPosition++;
        }

        playerArea[m_playerPosition] = Config.PlayerSymbol;
    }

    public void AddNextLineOfObstacles()
    {
        if (m_currentLevelRow < m_level.Count) 
        {
            for (int i = 0; i < m_level[m_currentLevelRow].Length; i++)
            {
                
                if (m_level[m_currentLevelRow][i])
                {
                    m_obstacles.Add(new Obstacle(i));
                }
            }
        }

        if (m_currentLevelRow < m_level.Count)
        {
            m_currentLevelRow++;
        }        
    }

    public void UpdateObstacles(char[,] gameWorld, int gameWorldHeight)
    {
        for (int i = m_obstacles.Count - 1; i >= 0; i--) 
        {
            if (m_obstacles[i].m_firstSpawn)
            {
                m_obstacles[i].m_firstSpawn = false;
                m_obstacles[i].m_yPosition++;
                gameWorld[m_obstacles[i].m_yPosition, m_obstacles[i].m_xPosition] = Config.ObstacleSymbol;
            }
            else if (m_obstacles[i].m_yPosition == gameWorldHeight - 1)
            {
                gameWorld[m_obstacles[i].m_yPosition, m_obstacles[i].m_xPosition] = ' ';
                m_obstacles.RemoveAt(i);
            }
            else
            {
                gameWorld[m_obstacles[i].m_yPosition, m_obstacles[i].m_xPosition] = ' ';
                m_obstacles[i].m_yPosition++;
                gameWorld[m_obstacles[i].m_yPosition, m_obstacles[i].m_xPosition] = Config.ObstacleSymbol;
            }
        }
    }

    public bool LevelWon()
    {
        return m_currentLevelRow == m_level.Count && m_obstacles.Count == 0;
    }
    
    public bool LevelLost()
    {
        for (int i = 0; i < m_obstacles.Count; i++) 
        {
            if (m_obstacles[i].m_yPosition == 15 - 1
                && m_playerPosition == m_obstacles[i].m_xPosition) 
            {
                return true;
            }
        }

        return false;
    }
}

public class Obstacle
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