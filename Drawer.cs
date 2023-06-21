using System.Text;

namespace scroller_game;

public class Drawer
{
    private int m_height { get; set; } 
    private int m_width { get; set; } 
    
    public Drawer(int height, int width)
    {
        m_height = height;
        m_width = width;
    }
    public void Draw(char[,] gameWorld, char[] playersArea)
    {
        StringBuilder builder = new();
        for (int i = 0; i < m_height; i++)
        {
            builder.Append('|');
            for (int j = 0; j < m_width; j++)
            {
                builder.Append(gameWorld[i,j]);
            }
            builder.Append('|');
            Console.WriteLine(builder);
            builder.Clear();
        }
        Console.WriteLine("|-------|");
        Console.WriteLine("|" + string.Concat(playersArea)+ "|");
    }
}