namespace scroller_game;

public class ObstacleManager
{
    private const char OBSTACLE_SYMBOL = '\u2580';
     
    private int m_levelLength { get; }
    private List<bool[]> m_level { get; }
    private int m_currentLevelRow { get; set; }
    
    public List<Obstacle> m_obstacles { get; }

    public ObstacleManager(int levelLength)
    {
        m_levelLength = levelLength;
        m_level = GenerateLevel(m_levelLength);
        m_currentLevelRow = 0;
        m_obstacles = new List<Obstacle>();
    }
    
    private List<bool[]> GenerateLevel(int levelLength)
    {
        Random rng = new();
        List<bool[]> gameWorld = new();
        int obstacleCooldown = 0;

        for (int i = 0; i < levelLength; i++)
        {
            bool[] row = new bool[levelLength];
            if (obstacleCooldown <= 0)
            {
                int obstacleCount = rng.Next(1, 4);
                for (int j = 0; j < obstacleCount; j++)
                {
                    int obstaclePos;
                    do
                    {
                        obstaclePos = rng.Next(levelLength);
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

    public void AddNextLineOfObstacles()
    {
        if (m_currentLevelRow < m_level.Count) 
        {
            //TODO: This is not working. WTF. why? This should only iterate for the length of the current thingo. Bruh.
            for (int xPosition = 0; xPosition < m_level[m_currentLevelRow].Length; xPosition++)
            {
                if (m_level[m_currentLevelRow][xPosition])
                {
                    m_obstacles.Add(new Obstacle(xPosition));
                }
            }
        }
        
        if (m_currentLevelRow < m_level.Count)
        {
            m_currentLevelRow++;
        }
    }

    public void UpdateObstacles(ref char[,] gameWorld, int gameWorldHeight)
    {
        for (int i = m_obstacles.Count - 1; i >= 0; i--) 
        {
            if (m_obstacles[i].m_firstSpawn)
            {
                m_obstacles[i].m_firstSpawn = false;
                m_obstacles[i].m_yPosition++;
                gameWorld[m_obstacles[i].m_yPosition, m_obstacles[i].m_xPosition] = OBSTACLE_SYMBOL;
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
                gameWorld[m_obstacles[i].m_yPosition, m_obstacles[i].m_xPosition] = OBSTACLE_SYMBOL;
            }
        }
    }

    public bool CompletedLevel()
    {
        return m_currentLevelRow == m_level.Count && m_obstacles.Count == 0;
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