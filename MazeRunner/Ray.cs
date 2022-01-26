
namespace MazeRunner
{
    struct Ray
    {
        public double Size { get; set; }
        public double Angle { get; set; } // angle it was casted from
        public Direction WallHit { get; set; } // which wall the ray hit
    }
}
