namespace console_snake_core
{
    class Entity
    {
        public Position Position { get; set; }
        public char Art { get; private set; }

        public Entity(Position position, char art)
        {
            Position = position;
            Art = art;
        }
    }
}
