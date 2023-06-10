namespace Game {
    class GameDrawer
    {
        private char m_playerCharacter = '\u2588';
        private char[] m_playerLine = new[] {' ', ' ', ' ', ' ', ' ', ' ', ' '};

        public int Width { get; private set; } = 7;
        public int Height { get; private set; } = 16;

        public void Draw(int playerPosition)
        {
            for(int i = 0; i < Height - 1; i++)
            {
                Console.WriteLine();
            }
            m_playerLine[playerPosition] = m_playerCharacter;
            Console.WriteLine(string.Concat(m_playerLine));
        }
    }
}
