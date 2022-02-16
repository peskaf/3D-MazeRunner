
namespace MazeRunner
{
    internal class Guardian
    {
        public Position CurrPosition { get; set; }
        public Position StartPosition { get; init; }
        public int Step { get; init; } = 2; // how much points in direction to go
        public Bitmap Texture = new(Properties.Resources.protector); // texture uploaded to Resources.resx
        public long SpawnTime { get; init; } = 2500; // millis to elapse before enemy spawns
        public bool IsSpawned { get; private set; } = false;
        public int StepsToMake { get; set; } = 0; // how much steps to make before computing next steps (made to reach the middle of each square it needs to visit)
        private Tuple<int, int> nextStepDirection = new(0,0); // where it goes next

        public Guardian(Position startPos)
        {
            StartPosition = startPos;
        }

        public void Spawn()
        {
            CurrPosition = StartPosition;
            IsSpawned = true;
        }

        // performs BFS to find shortest path to the player so that enemy can move in given direction
        private static Dictionary<Position, Position?> BFS(Position from, Position to, Map map)
        {
            List<Position> visited = new();
            Dictionary<Position, Position?> nodePredecessor = new();

            Position fromTile = new(from.X / map.TileSize, from.Y / map.TileSize);
            Position toTile = new(to.X / map.TileSize, to.Y / map.TileSize);

            Queue<Position> queue = new();
            visited.Add(fromTile);
            nodePredecessor[fromTile] = null;
            queue.Enqueue(fromTile);

            while (queue.Count != 0)
            {
                Position currPos = queue.Dequeue();
                
                if (map.Grid[currPos.X, currPos.Y - 1] == Tile.Empty && !visited.Contains(new Position(currPos.X, currPos.Y - 1))) // up, empty, not found yet
                {
                    Position nextPos = new(currPos.X, currPos.Y - 1);
                    nodePredecessor[nextPos] = currPos;
                    if (nextPos == toTile) // it found player
                    {
                        return nodePredecessor;
                    }
                    visited.Add(nextPos);
                    queue.Enqueue(nextPos);
                }

                if (map.Grid[currPos.X, currPos.Y + 1] == Tile.Empty && !visited.Contains(new Position(currPos.X, currPos.Y + 1))) // down, empty, not found yet
                {
                    Position nextPos = new(currPos.X, currPos.Y + 1);
                    nodePredecessor[nextPos] = currPos;
                    if (nextPos == toTile) // it found player
                    {
                        return nodePredecessor;
                    }
                    visited.Add(nextPos);
                    queue.Enqueue(nextPos);
                }

                if (map.Grid[currPos.X - 1, currPos.Y] == Tile.Empty && !visited.Contains(new Position(currPos.X - 1, currPos.Y))) // left, empty, not found yet
                {
                    Position nextPos = new(currPos.X - 1, currPos.Y);
                    nodePredecessor[nextPos] = currPos;
                    if (nextPos == toTile) // it found player
                    {
                        return nodePredecessor;
                    }
                    visited.Add(nextPos);
                    queue.Enqueue(nextPos);
                }

                if (map.Grid[currPos.X + 1, currPos.Y] == Tile.Empty && !visited.Contains(new Position(currPos.X + 1, currPos.Y))) // right, empty, not found yet
                {
                    Position nextPos = new(currPos.X + 1, currPos.Y);
                    nodePredecessor[nextPos] = currPos;
                    if (nextPos == toTile) // it found player
                    {
                        return nodePredecessor;
                    }
                    visited.Add(nextPos);
                    queue.Enqueue(nextPos);
                }

            }
            return nodePredecessor; // return whole predecessor dict if "to" position not found
        }

        // makes path out of predecessors obtained from BFS
        private static Position GetNextPosition(Position from, Position to, Map map)
        {
            Dictionary<Position, Position?> nodePredecessor = BFS(from, to, map);

            Position fromTile = new(from.X / map.TileSize, from.Y / map.TileSize);
            Position toTile = new(to.X / map.TileSize, to.Y / map.TileSize);

            Position? nextPos = toTile;

            if (nextPos == null)
            {
                return fromTile; // don't move
            }

            while (nodePredecessor[nextPos.Value] != fromTile) // find what tile is next to move to
            {

                nextPos = (Position?)nodePredecessor[nextPos.Value];

                if (nextPos == null)
                {
                    return fromTile; // don't move
                }

            }
            return nextPos.Value;
        }

        public void Move(GameMap gameMap) // handle moving -> move it if it has steps to make, if not, compute it's way and move that way
        {
            if (StepsToMake > 0)
            {
                CurrPosition = new Position(CurrPosition.X + nextStepDirection.Item1 * Step, CurrPosition.Y + nextStepDirection.Item2 * Step);
                StepsToMake--;
            }
            else
            {
                // find where to go next
                Position nextTilePos = GetNextPosition(CurrPosition, gameMap.Player.CurrPosition, gameMap.Map);

                nextStepDirection = new Tuple<int, int>(nextTilePos.X - CurrPosition.X / gameMap.Map.TileSize, nextTilePos.Y - CurrPosition.Y / gameMap.Map.TileSize);
                StepsToMake = gameMap.Map.TileSize / Step;

                // and actually go there
                Move(gameMap);
            }
        }
    }
}
