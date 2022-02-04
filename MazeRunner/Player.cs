
namespace MazeRunner
{
    class Player
    {
        public Vector Direction { get; set; } // current looking direction of the player
        public Position CurrPosition { get; set; }
        public Position StartPosition { get; init; } // where player starts
        public int Step { get; init; } = 7; // size of player's step
        public double FOV { get; init; } // Field of view (radians)
        public double RotationSpeed { get; init; } = 2.9; // in degrees/10 ms
        public bool IsKilled { get; set; } = false;

        public Player(Position position, Vector direction)
        {
            this.Direction = direction;
            this.CurrPosition = position;
            this.StartPosition = position;
            FOV = 50 * Math.PI / 180; // radians = degrees * PI / 180
        }
    }
}
