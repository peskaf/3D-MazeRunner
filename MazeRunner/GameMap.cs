﻿
namespace MazeRunner
{
    class GameMap
    {
        public Map Map; // maze generated by maze class
        public Player Player;
        public Guardian Guardian;
        public GameMap(int size)
        {
            Map = new Map(size);

            // the middle of the first empty square
            Position startPos = new(Convert.ToInt32(1.5 * Map.TileSize), Convert.ToInt32(1.5 * Map.TileSize));

            // make the player face empty space; right direction not guaranteed
            Vector baseDirection = Map.Grid[(startPos.X / Map.TileSize) + 1, startPos.Y / Map.TileSize] == Tile.Wall ? new Vector(0, -1) : new Vector(1, 0);

            Guardian = new Guardian(startPos);
            Player = new Player(startPos, baseDirection);
        }
        Ray Raycast(Position pos, Vector direction, int max) // cast one ray
        {
            int maxExtent = Map.GridSize * Map.TileSize - 1; // - 1 to work as range -> [0,maxExtent]
            Point furthestObservablePoint = new(pos.X + (direction.X * max), pos.Y - (direction.Y * max)); // coordinates are furthest observable points in given directions
            Line playerLine = new(new Point(pos.X, pos.Y), furthestObservablePoint);

            Ray horizonatalLineRay = new();
            Ray verticalLineRay = new();

            if (direction.Y != 0) // looking down or up
            {
                // choose line to check for intersection with player's look vector (direction)
                int startY = direction.Y < 0 ? Convert.ToInt32(Math.Ceiling((double)pos.Y / Map.TileSize) * Map.TileSize) : ((pos.Y / Map.TileSize) * Map.TileSize);
                Line possibleWallLine = new(new Point(0, startY), new Point(maxExtent, startY));

                int gridPx; // grid in which intersection is (X)

                while (true)
                {
                    Point? intersection = playerLine.GetIntersection(possibleWallLine);

                    if (intersection != null)
                    {
                        gridPx = Convert.ToInt32(Math.Floor(intersection.Value.X / Map.TileSize));

                        // check for wall hit
                        if (Map.Grid[gridPx, ((int)possibleWallLine.FirstPoint.Y / Map.TileSize) - (direction.Y > 0 ? 1 : 0)] == Tile.Wall) // we hit the wall
                        {
                            horizonatalLineRay.Size = Math.Sqrt(Math.Pow(intersection.Value.X - pos.X, 2) + Math.Pow(possibleWallLine.FirstPoint.Y - pos.Y, 2)); // size of the (x, y), (Px, y3) line -> Pythagorean theorem
                            horizonatalLineRay.WallHit = direction.Y < 0 ? Direction.Down : Direction.Up; // check if the ray hit bottom or top of the wall
                            break;
                        }

                        // no hit -> check on the next/previous horizontal line based on the direction
                        if (direction.Y < 0)
                        {
                            possibleWallLine = new Line(new Point(0, possibleWallLine.FirstPoint.Y + Map.TileSize), new Point(maxExtent, possibleWallLine.SecondPoint.Y + Map.TileSize));
                        }
                        else
                        {
                            possibleWallLine = new Line(new Point(0, possibleWallLine.FirstPoint.Y - Map.TileSize), new Point(maxExtent, possibleWallLine.SecondPoint.Y - Map.TileSize));
                        }
                    }
                    else // no intersection (max ray size didn't hit the wall)
                    {
                        horizonatalLineRay.Size = Double.PositiveInfinity;
                        break;
                    }
                }
            }

            // vertical lines intersection
            if (direction.X != 0) // looking left or right
            {
                // choose line to check for intersection with player's look vector (direction)
                int startX = direction.X > 0 ? Convert.ToInt32(Math.Ceiling((double)pos.X / Map.TileSize) * Map.TileSize) : ((pos.X / Map.TileSize) * Map.TileSize);
                Line possibleWallLine = new(new Point(startX, 0), new Point(startX, maxExtent));

                int gridPy; // grid in which intersection is (Y)

                while (true)
                {
                    Point? intersection = playerLine.GetIntersection(possibleWallLine);

                    if (intersection != null)
                    {
                        gridPy = Convert.ToInt32(Math.Floor(intersection.Value.Y / Map.TileSize));

                        // check for wall hit
                        if (Map.Grid[((int)possibleWallLine.FirstPoint.X / Map.TileSize) - (direction.X < 0 ? 1 : 0), gridPy] == Tile.Wall) // we hit the wall
                        {
                            verticalLineRay.Size = Math.Sqrt(Math.Pow(intersection.Value.Y - pos.Y, 2) + Math.Pow(possibleWallLine.FirstPoint.X - pos.X, 2)); // size of the (x, y), (Px, y3) line -> Pythagorean theorem
                            verticalLineRay.WallHit = direction.X > 0 ? Direction.Right : Direction.Left; // check if the ray hit left or right side of the wall
                            break;
                        }

                        // no hit -> check on the next/previous horizontal line based on the direction
                        if (direction.X > 0)
                        {
                            possibleWallLine = new Line(new Point(possibleWallLine.FirstPoint.X + Map.TileSize, 0), new Point(possibleWallLine.FirstPoint.X + Map.TileSize, maxExtent));
                        }
                        else
                        {
                            possibleWallLine = new Line(new Point(possibleWallLine.FirstPoint.X - Map.TileSize, 0), new Point(possibleWallLine.FirstPoint.X - Map.TileSize, maxExtent));
                        }
                    }
                    else // no intersection (max ray size didn't hit the wall)
                    {
                        verticalLineRay.Size = Double.PositiveInfinity;
                        break;
                    }
                }

            }

            // return shorter ray (shorter ray hit the wall first)

            if (horizonatalLineRay.Size < verticalLineRay.Size)
            {
                horizonatalLineRay.Angle = direction.GetAngle();
                return horizonatalLineRay;
            }
            else
            {
                verticalLineRay.Angle = direction.GetAngle();
                return verticalLineRay;
            }
        }

        public List<Ray> CastRays(int screenWidth) // cast all the rays to fill the screen width
        {
            int maxRaySize = 1500; // how far can player see

            Vector minAngleVector = Player.Direction.RotateCounterclockwise(Player.FOV / 2); // from where to start casting

            double step = Player.FOV / screenWidth; // how much to rotate each step


            List<Ray> rays = new();

            Vector currAngleVector = minAngleVector;

            for (int i = 0; i < screenWidth; i++)
            {
                Ray ray = Raycast(Player.CurrPosition, currAngleVector, maxRaySize);
                rays.Add(ray);
                currAngleVector = currAngleVector.RotateClockwise(step); // rotate a bit
            }

            return rays;
        }
    }
}
