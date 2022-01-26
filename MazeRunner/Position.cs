
namespace MazeRunner
{
    struct Position
    {
        public int X { get; init; }
        public int Y { get; init; }

        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            Position other = (Position)obj;
            if (this.X == other.X && this.Y == other.Y)
            {
                return true;
            }
            return false;
        }

        public static Position operator -(Position pos1, Position pos2)
        {
            return new Position(pos1.X - pos2.X, pos1.Y - pos2.Y);
        }

        public static Position operator +(Position pos1, Position pos2)
        {
            return new Position(pos1.X + pos2.X, pos1.Y + pos2.Y);
        }

        public static bool operator ==(Position? pos1, Position? pos2)
        {
            if (!pos1.HasValue && !pos2.HasValue)
            {
                return true;
            }
            return pos1.Equals(pos2);
        }

        public static bool operator !=(Position? pos1, Position? pos2)
        {
            if (!pos1.HasValue && !pos2.HasValue)
            {
                return false;
            }
            return !pos1.Equals(pos2);
        }

        public override int GetHashCode()
        {
            const int prime1 = 17;
            const int prime2 = 31;

            int hash = prime1;
            hash = hash * prime2 + this.X.GetHashCode();
            hash = hash * prime2 + this.Y.GetHashCode();
            return hash;
        }
    }
}
