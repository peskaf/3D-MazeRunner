
namespace MazeRunner
{
    struct Vector
    {
        public double X { get; init; }
        public double Y { get; init; }

        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public Vector RotateCounterclockwise(double angle) // returns ccw rotated vector by given angle
        {
            return new Vector(X * Math.Cos(angle) + Y * (-Math.Sin(angle)), X * Math.Sin(angle) + Y * Math.Cos(angle));
        }

        public Vector RotateClockwise(double angle) // returns cw rotated vector by given angle
        {
            return new Vector(X * Math.Cos(-angle) + Y * (-Math.Sin(-angle)), X * Math.Sin(-angle) + Y * Math.Cos(-angle));
        }

        public double GetAngle() // get angle in radians; angle that vector forms with (1,0)^T vector
        {
            return Math.Atan2(Y, X);
        }
    }
}
