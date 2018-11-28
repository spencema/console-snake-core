namespace console_snake_core
{
    class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position()
        {
            X = 0;
            Y = 0;
        }
 
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Position lhs, Position rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y;
        }

        public static bool operator !=(Position lhs, Position rhs)
        {
            return lhs.X != rhs.X && lhs.Y != rhs.Y;
        }
    }
}
