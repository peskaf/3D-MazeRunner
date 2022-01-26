
namespace MazeRunner
{
    struct Line
    {
        public Point FirstPoint { get; init; }
        public Point SecondPoint { get; init; }

        public Line(Point firstPoint, Point secondPoint)
        {
            FirstPoint = firstPoint;
            SecondPoint = secondPoint;
        }

        public Point? GetIntersection(Line otherLine)
        {
            // see https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection#Formulas for more info (implemented with similar names)
            double t = ((FirstPoint.X - otherLine.FirstPoint.X) * (otherLine.FirstPoint.Y - otherLine.SecondPoint.Y) - (FirstPoint.Y - otherLine.FirstPoint.Y) * (otherLine.FirstPoint.X - otherLine.SecondPoint.X)) /
                ((FirstPoint.X - SecondPoint.X) * (otherLine.FirstPoint.Y - otherLine.SecondPoint.Y) - (FirstPoint.Y - SecondPoint.Y) * (otherLine.FirstPoint.X - otherLine.SecondPoint.X));

            double u = -((FirstPoint.X - SecondPoint.X) * (FirstPoint.Y - otherLine.FirstPoint.Y) - (FirstPoint.Y - SecondPoint.Y) * (FirstPoint.X - otherLine.FirstPoint.X)) /
                ((FirstPoint.X - SecondPoint.X) * (otherLine.FirstPoint.Y - otherLine.SecondPoint.Y) - (FirstPoint.Y - SecondPoint.Y) * (otherLine.FirstPoint.X - otherLine.SecondPoint.X));

            if (t >= 0 && t <= 1 && u >= 0 && u <= 1) // intersection falls within the first and the second line segment
            {
                return new Point(FirstPoint.X + t * (SecondPoint.X - FirstPoint.X), FirstPoint.Y + t * (SecondPoint.Y - FirstPoint.Y));
            }
            return null; // no intersection
        }
    }
}
