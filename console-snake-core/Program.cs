using System;
using System.Collections.Generic;
using System.Linq;

namespace console_snake_core
{
    enum Direction
    {
        North,
        East,
        South,
        West
    }

    class Program
    {
        private static readonly string _title = "Snake Core | Score: {0} | X: {1}, Y: {2} | Freq: {3}";
        private static readonly char _snakeArt = '*';
        private static readonly char _foodArt = 'x';
        private static readonly char _wallArtHorizontal = '-';
        private static readonly char _wallArtVertical = '|';
        private static readonly int _pointsPerFood = 10;

        private static DateTime _startTime;
        private static int _score;
        private static bool _running;
        private static bool _gameOver;
        private static int _updateFrequency;
        private static int _maxUpdateFrequency;
        private static Direction _direction;
        private static List<Entity> _snake;
        private static Entity _food;

        static void Main(string[] args)
        {
            Init();

            GameLoop();
        }

        private static void Init()
        {
            _startTime = DateTime.Now;

            var windowWidth = 50;
            var windowHeight = 25;
            Console.SetWindowSize(windowWidth, windowHeight);

            _score = 0;
            var x = Console.WindowWidth / 2 - 1;
            var y = Console.WindowHeight / 2 - 1;

            Console.Title = string.Format(_title, _score, x, y, _updateFrequency);
            Console.CursorVisible = false;

            _running = true;
            _gameOver = false;
            _updateFrequency = 110;
            _maxUpdateFrequency = _updateFrequency;
            _direction = Direction.North;

            var startingPos = new Position(x, y);
            _snake = new List<Entity>
            {
                new Entity(new Position(startingPos.X, startingPos.Y), _snakeArt),
                new Entity(new Position(startingPos.X, startingPos.Y + 1), _snakeArt),
                new Entity(new Position(startingPos.X, startingPos.Y + 2), _snakeArt),
                new Entity(new Position(startingPos.X, startingPos.Y + 3), _snakeArt)
            };

            CreateFood();

            Console.Clear();

            DrawBackground();

            DrawSnake();

            DrawFood();
        }

        private static void GameLoop()
        {
            while (_running)
            {
                if (Console.KeyAvailable)
                {
                    Input();
                }

                if (!_gameOver)
                {
                    var timeDifference = DateTime.Now - _startTime;
                    var timeDifferenceMilliseconds = (int)timeDifference.TotalMilliseconds;
                    int updateFrequency = CalculateUpdateFrequency();
                    if (timeDifferenceMilliseconds >= updateFrequency)
                    {
                        if (HitWall())
                        {
                            GameOver();
                            continue;
                        }

                        var head = GetHead();
                        var collidedSelf = _snake.GetRange(1, _snake.Count - 1).Where(segment => segment.Position == head.Position).Any();

                        if (collidedSelf)
                        {
                            GameOver();
                            continue;
                        }

                        if (head.Position == _food.Position)
                        {
                            Ate();
                        }
                        else
                        {
                            Move(_snake, NextPos());
                        }

                        UpdateTitle();
                        DrawSnake();
                        _startTime = DateTime.Now;
                    }
                }
            }
        }

        private static bool HitWall()
        {
            int minBoundX = 0;
            int maxBoundX = Console.WindowWidth - 1;
            int minBoundY = 0;
            int maxBoundY = Console.WindowHeight - 1;
            var head = GetHead();
            return head.Position.X <= minBoundX || head.Position.X >= maxBoundX ||
                head.Position.Y <= minBoundY || head.Position.Y >= maxBoundY;
        }

        private static int CalculateUpdateFrequency()
        {
            // The default font on Windows 10 is Consolas and is is 8px x 16px.
            // When we move vertically we cover twice as much distance which makes the snake
            // appear faster vertically so we slow down the update frequency for vertical movement.
            // Todo: Find a way to slow it down based on font used in the console so this works across different fonts.
            if (_direction == Direction.North || _direction == Direction.South)
            {
                return _updateFrequency * 2;
            }

            return _updateFrequency;
        }

        private static void Input()
        {
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    if (_direction != Direction.South)
                        _direction = Direction.North;
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    if (_direction != Direction.East)
                        _direction = Direction.West;
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    if (_direction != Direction.West)
                        _direction = Direction.East;
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    if (_direction != Direction.North)
                        _direction = Direction.South;
                    break;
                case ConsoleKey.R:
                    Init();
                    break;
                case ConsoleKey.Escape:
                    _running = false;
                    break;
            }
        }

        private static void Ate()
        {
            CreateFood();
            DrawFood();
            var currentHead = _snake[0];
            InsertNewHead(new Entity(NextPos(), _snakeArt));
            IncreaseSnakeSpeed();
            _score += _pointsPerFood;
        }

        private static void InsertNewHead(Entity newHead)
        {
            _snake.Insert(0, newHead);
            Console.SetCursorPosition(newHead.Position.X, newHead.Position.Y);
        }

        private static Position NextPos()
        {
            var head = _snake[0];
            Position pos = new Position();
            switch (_direction)
            {
                case Direction.North:
                    pos = new Position(head.Position.X, head.Position.Y - 1);
                    break;
                case Direction.East:
                    pos = new Position(head.Position.X + 1, head.Position.Y);
                    break;
                case Direction.South:
                    pos = new Position(head.Position.X, head.Position.Y + 1);
                    break;
                case Direction.West:
                    pos = new Position(head.Position.X - 1, head.Position.Y);
                    break;
                default:
                    break;
            }

            return pos;
        }

        private static void CreateFood()
        {
            // Choose a random position for the food within the bounds of the level.
            var random = new Random();
            int randX;
            int randY;
            Position randPos;

            do
            {
                randX = random.Next(2, Console.WindowWidth - 2);
                randY = random.Next(2, Console.WindowHeight - 2);

                randPos = new Position(randX, randY);
            } while (_snake.Exists(x => x.Position == randPos));

            _food = new Entity(randPos, _foodArt);
        }

        private static void Move(List<Entity> snake, Position newPosition)
        {
            // To move the snake we make make the current tail segment the new head.
            var tail = GetTail();
            snake.RemoveAt(snake.Count - 1);

            int currentX = tail.Position.X;
            int currentY = tail.Position.Y;

            var newHead = tail;
            newHead.Position = newPosition;
            snake.Insert(0, newHead);

            Console.MoveBufferArea(currentX, currentY, 1, 1, newHead.Position.X, newHead.Position.Y);
        }

        private static void IncreaseSnakeSpeed()
        {
            // To change the snake's speed we change how often the console is updated.
            int min = 40;
            int max = _maxUpdateFrequency;
            int change = 10;
            _updateFrequency = Math.Clamp(_updateFrequency - change, min, max);
        }

        private static void UpdateTitle()
        {
            var head = _snake[0];
            Console.Title = string.Format(_title, _score, head.Position.X, head.Position.Y, _updateFrequency);
        }

        private static void GameOver()
        {
            _gameOver = true;
            Console.Clear();
            string gameOver = string.Format("Game Over! You Scored: {0}", _score);
            int x = Console.WindowWidth / 2;
            int y = Console.WindowHeight / 2;
            Console.SetCursorPosition(x - gameOver.Length / 2, y);
            Console.Write(gameOver);
            const string restart = "Press [R] to restart or [Esc] to quit.";
            Console.SetCursorPosition(x - restart.Length / 2, y + 1);
            Console.Write(restart);
        }

        private static void DrawBackground()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write(_wallArtHorizontal);

            for (int i = 1; i < Console.WindowHeight - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(_wallArtVertical);
            }

            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write(_wallArtHorizontal);

            for (int i = 1; i < Console.WindowHeight - 1; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth - 1, i);
                Console.Write(_wallArtVertical);
            }
        }

        private static void DrawFood()
        {
            Console.SetCursorPosition(_food.Position.X, _food.Position.Y);
            Console.Write(_food.Art);
        }

        private static void DrawSnake()
        {
            foreach (var segment in _snake)
            {
                Console.SetCursorPosition(segment.Position.X, segment.Position.Y);
                Console.Write(segment.Art);
            }
        }

        private static Entity GetHead()
        {
            return _snake[0];
        }

        private static Entity GetTail()
        {
            return _snake[_snake.Count - 1];
        }
    }
}
