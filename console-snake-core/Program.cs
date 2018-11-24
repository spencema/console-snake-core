using System;
using System.Collections.Generic;

namespace console_snake_core
{
    class Program
    {
        enum Direction
        {
            North,
            East,
            South,
            West
        }

        struct Position
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Position(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        struct Segment
        {
            public Position Position { get; set; }

            public Segment(Position position)
            {
                Position = position;
            }
        }

        static void Main(string[] args)
        {
            var startTime = DateTime.Now;
            Console.Title = "Snake Core";

            Console.CursorVisible = false;

            var startingPos = new Position(Console.WindowWidth / 2 - 1, Console.WindowHeight / 2 - 1);

            var direction = Direction.North;

            DrawBackground();

            var snake = new List<Segment>
            {
                new Segment(new Position(startingPos.X, startingPos.Y)),
                new Segment(new Position(startingPos.X, startingPos.Y + 1)),
                new Segment(new Position(startingPos.X, startingPos.Y + 2)),
                new Segment(new Position(startingPos.X, startingPos.Y + 3))
            };

            foreach (var segment in snake)
            {
                Console.SetCursorPosition(segment.Position.X, segment.Position.Y);
                Console.Write("*");
            }

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    direction = Input(direction);
                    continue;
                }

                var timeDifference = DateTime.Now - startTime;
                var timeDifferenceMilliseconds = (int)timeDifference.TotalMilliseconds;
                const int updateFrequency = 200;
                if (timeDifferenceMilliseconds >= updateFrequency)
                {
                    var head = snake[0];
                    var tail = snake[snake.Count - 1];
                    switch (direction)
                    {
                        case Direction.North:
                            if (head.Position.Y > 1)
                            {
                                Move(snake, head, tail, new Position(head.Position.X, head.Position.Y - 1));
                            }
                            break;
                        case Direction.East:
                            if (head.Position.X < Console.WindowWidth - 1)
                            {
                                Move(snake, head, tail, new Position(head.Position.X + 1, head.Position.Y));
                            }
                            break;
                        case Direction.South:
                            if (head.Position.Y < Console.WindowHeight - 1)
                            {
                                Move(snake, head, tail, new Position(head.Position.X, head.Position.Y + 1));
                            }
                            break;
                        case Direction.West:
                            if (head.Position.X > 0)
                            {
                                Move(snake, head, tail, new Position(head.Position.X - 1, head.Position.Y));
                            }
                            break;
                        default:
                            break;
                    }

                    startTime = DateTime.Now;
                }
            }
        }

        private static Direction Input(Direction dir)
        {
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.W:
                    if (dir != Direction.South)
                        dir = Direction.North;
                    break;
                case ConsoleKey.A:
                    if (dir != Direction.East)
                        dir = Direction.West;
                    break;
                case ConsoleKey.D:
                    if (dir != Direction.West)
                        dir = Direction.East;
                    break;
                case ConsoleKey.S:
                    if (dir != Direction.North)
                        dir = Direction.South;
                    break;
            }

            return dir;
        }

        private static void DrawBackground()
        {
            Console.SetCursorPosition(1, 0);
            for (int i = 1; i < Console.WindowWidth - 1; i++)
                Console.Write("*");

            for (int i = 1; i < Console.WindowHeight - 1; i++)
            {
                Console.SetCursorPosition(1, i);
                Console.Write("*");
            }

            Console.SetCursorPosition(1, Console.WindowHeight - 1);
            for (int i = 1; i < Console.WindowWidth - 1; i++)
                Console.Write("*");

            for (int i = 1; i < Console.WindowHeight - 1; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth - 2, i);
                Console.Write("*");
            }
        }

        private static void Move(List<Segment> snake, Segment head, Segment tail, Position newPosition)
        {
            // To move the snake we make make the current tail segment the new head.

            snake.RemoveAt(snake.Count - 1);

            int currentX = tail.Position.X;
            int currentY = tail.Position.Y;

            var newHead = tail;
            newHead.Position = newPosition;
            snake.Insert(0, newHead);

            Console.MoveBufferArea(currentX, currentY, 1, 1, newHead.Position.X, newHead.Position.Y);
        }
    }
}
