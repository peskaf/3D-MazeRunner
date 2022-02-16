
namespace MazeRunner
{
    class Map
    {
        public int GridSize { get; init; } // number of tiles on one side of the grid (width=height), must be odd and > 3
        public Tile[,] Grid { get; private set; }
        public int TileSize { get; init; } = 64; // size of one tile in the grid (width=height), good to be even and power of 2

        public Map(int gridSize)
        {
            GridSize = gridSize;
            Grid = new Tile[GridSize, GridSize];
            BuildMaze();
        }
        private void CreateFoundation() // creation of template to build maze in
        {
            for (int i = 0; i < GridSize; i++)
            {
                Grid[i, 0] = Tile.Wall; // left-most wall
                Grid[i, GridSize - 1] = Tile.Wall; // right-most wall
            }

            for (int j = 1; j < GridSize - 1; j++)
            {
                // top wall
                Grid[0, j] = Tile.Wall;

                // bottom wall
                Grid[GridSize - 1, j] = Tile.Wall;

                // inside
                for (int i = 1; i < GridSize - 1; i++)
                {
                    if (j % 2 == 0 && i % 2 == 0) // place foundation on positions with even indices
                    {
                        Grid[i, j] = Tile.Foundation;
                    }
                    else // on odd positions, just empty tiles
                    {
                        Grid[i, j] = Tile.Empty;
                    }
                }
            }
        }
        private int FoundationsLeft() // how many foundations are left in grid?
        {
            int numOfFoundations = 0;
            for (int i = 1; i < GridSize - 1; i++)
            {
                for (int j = 1; j < GridSize - 1; j++)
                {
                    if (Grid[i, j] == Tile.Foundation)
                    {
                        numOfFoundations++;
                    }
                }
            }
            return numOfFoundations;
        }
        private Position GetRandomFoundationPosition(Random random) // choose random foundation to build wall from
        {
            int indexOfFoundation = random.Next(1, FoundationsLeft() + 1); // random index of foundation -> will build from this one
            int numOfFoundationsMet = 0; // how many foundations did forcycle see already

            for (int i = 1; i < GridSize - 1; i++)
            {
                for (int j = 1; j < GridSize - 1; j++)
                {
                    if (Grid[i, j] == Tile.Foundation)
                    {
                        numOfFoundationsMet++;
                        if (numOfFoundationsMet == indexOfFoundation) // it is the random one
                        {
                            return new Position(i,j);
                        }
                    }
                }
            }
            return new Position(-1, -1); // this won't ever happen (as random index is computed from what there really is in the grid)
        }
        private void BuildWall(Random random) //build wall from that random foundation
        {
            Position randomFoundationPosition = GetRandomFoundationPosition(random);
            Array directionValues = Enum.GetValues(typeof(Direction));
#pragma warning disable CS8605 // Unboxing a possibly null value. -> will not be null, maxVal is array length
            Direction randomDirection = (Direction)directionValues.GetValue(random.Next(directionValues.Length));
#pragma warning restore CS8605 // Unboxing a possibly null value.

            int randX = randomFoundationPosition.X;
            int randY = randomFoundationPosition.Y;

            switch (randomDirection)
            {
                case Direction.Up:
                    while (Grid[randX, randY] != Tile.Wall)
                    {
                        Grid[randX--, randY] = Tile.Wall;
                    }
                    break;
                case Direction.Down:
                    while (Grid[randX, randY] != Tile.Wall)
                    {
                        Grid[randX++, randY] = Tile.Wall;
                    }
                    break;
                case Direction.Left:
                    while (Grid[randX, randY] != Tile.Wall)
                    {
                        Grid[randX, randY--] = Tile.Wall;
                    }
                    break;
                case Direction.Right:
                    while (Grid[randX, randY] != Tile.Wall)
                    {
                        Grid[randX, randY++] = Tile.Wall;
                    }
                    break;
                default:
                    break;
            }
        }
        private void BuildMaze() // handle all building functions
        {
            Random random = new();
            CreateFoundation();
            while (FoundationsLeft() != 0)
            {
                BuildWall(random);
            }
        }
    }
}
