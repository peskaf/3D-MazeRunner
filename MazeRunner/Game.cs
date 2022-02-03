
namespace MazeRunner
{
    public partial class Game : Form
    {
        const int GRID_SIZE = 21; // must be odd and > 3 !!!
        private GameMap gameMap = new(GRID_SIZE);
        bool keyW = false;
        bool keyS = false;
        bool keyA = false;
        bool keyD = false;
        int newX, newY;
        long timeElapsed;

        public Game()
        {
            InitializeComponent();
            GameOver.Visible = false;
            Timer.Enabled = false;
            Victory.Visible = false;
        }
        private void Render(GameMap gameMap)
        {
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            BufferedGraphics myBuffer = currentContext.Allocate(CreateGraphics(), DisplayRectangle);

            // sky and floor
            Rectangle sky = new(0, 0, DisplayRectangle.Width, DisplayRectangle.Height / 2);
            Rectangle floor = new(0, DisplayRectangle.Height / 2, DisplayRectangle.Width, DisplayRectangle.Height / 2);
            myBuffer.Graphics.FillRectangle(Brushes.Gray, sky);
            myBuffer.Graphics.FillRectangle(Brushes.Green, floor);

            // walls
            List<Ray> rays = gameMap.CastRays(DisplayRectangle.Width);
            int x = 0;

            foreach (Ray ray in rays)
            {
                // convert each ray into line that is to be displayed on screen
                float currentAngle = Convert.ToSingle(gameMap.Player.Direction.GetAngle() - ray.Angle);
                if (currentAngle < 0)
                {
                    currentAngle += Convert.ToSingle(2 * Math.PI);
                }
                else if (currentAngle > 0)
                {
                    currentAngle -= Convert.ToSingle(2 * Math.PI);
                }

                float disT = Convert.ToSingle(ray.Size * Math.Cos(currentAngle));
                float wallHeight = Convert.ToSingle((gameMap.Map.TileSize * DisplayRectangle.Height) / disT);

                if (wallHeight > DisplayRectangle.Height)
                {
                    wallHeight = DisplayRectangle.Height;
                }

                float startY = (DisplayRectangle.Height - Convert.ToSingle(wallHeight)) / 2;
                var color = ray.WallHit switch
                {
                    Direction.Up => Pens.Blue,
                    Direction.Down => Pens.LightBlue,
                    Direction.Left => Pens.DodgerBlue,
                    Direction.Right => Pens.DarkBlue,
                    _ => Pens.Red, // default -> won't happen
                };
                myBuffer.Graphics.DrawLine(color, x, startY, x, Convert.ToSingle(startY + wallHeight));

                x++;
            }

            // Guardian - billboarding
            if (gameMap.Guardian.IsSpawned == true)
            {
                Vector normalVectorToPlayersDirection = new(gameMap.Player.Direction.Y * Math.Tan(gameMap.Player.FOV / 2), -gameMap.Player.Direction.X * Math.Tan(gameMap.Player.FOV / 2));

                Position guardianRelativePosition = gameMap.Guardian.CurrPosition - gameMap.Player.CurrPosition;

                double invDet = 1.0 / (normalVectorToPlayersDirection.X * gameMap.Player.Direction.Y - gameMap.Player.Direction.X * normalVectorToPlayersDirection.Y); // just for matrix multiplication (to transform X)

                double transformX = invDet * (gameMap.Player.Direction.Y * guardianRelativePosition.X - gameMap.Player.Direction.X * -guardianRelativePosition.Y); // Y with - (y cordinate rises while going down the screen)
                double transformY = guardianRelativePosition.X * gameMap.Player.Direction.X + -guardianRelativePosition.Y * gameMap.Player.Direction.Y;

                int spriteScreenX = (int)((DisplayRectangle.Width / 2) * (1 + transformX / transformY)); // sprite screen coordinates

                // calculate size of the sprite on screen
                float spriteSize = Convert.ToSingle((gameMap.Map.TileSize * DisplayRectangle.Height) / transformY); // sprite screen size (width=height)

                // calculate highest pixel (even if not on screen)
                int drawStartY = (int)(-spriteSize / 2 + DisplayRectangle.Height / 2);

                // calculate leftmost pixel (even if not on screen)
                int drawStartX = (int)(-spriteSize / 2 + spriteScreenX);

                if (transformY > 30) // if sprite more than 30 units away -> draw it
                {
                    for (int stripe = drawStartX; stripe < drawStartX + spriteSize + 1; stripe++)
                    {
                        if (stripe > 0 && stripe < DisplayRectangle.Width && rays[stripe].Size * Math.Cos(gameMap.Player.Direction.GetAngle() - rays[stripe].Angle) > transformY)
                        {
                            int texX = (int)((stripe - (-spriteSize / 2 + spriteScreenX)) * gameMap.Guardian.texture.Width / spriteSize); // x coordinate in sprite texture
                            Rectangle destRect = new(stripe, drawStartY, 1, (int)spriteSize); // where to display current stripe of sprite texture
                            myBuffer.Graphics.DrawImage(gameMap.Guardian.texture, destRect, texX, 0, 1, gameMap.Guardian.texture.Height, GraphicsUnit.Pixel);
                        }
                    }
                }
            }

            // minimap
            int squareSize = 7; // size of one tile on minimap, odd and > 1
            int offset = 10; // offset from the edge of the screen
            x = DisplayRectangle.Width - gameMap.Map.GridSize * squareSize - offset; 
            int y = offset;
            for (int i = 0; i < gameMap.Map.GridSize; i++)
            {
                for (int j = 0; j < gameMap.Map.GridSize; j++)
                {
                    Rectangle r = new(x, y, squareSize, squareSize);
                    if (gameMap.Map.Grid[j, i] == Tile.Wall)
                    {
                        myBuffer.Graphics.FillRectangle(Brushes.Black, r);
                    }
                    else
                    {
                        myBuffer.Graphics.FillRectangle(Brushes.Yellow, r);
                    }
                    x += squareSize;
                }
                y += squareSize;
                x = DisplayRectangle.Width - gameMap.Map.GridSize * squareSize - offset;
            }

            // player on minimap
            Rectangle player = new(DisplayRectangle.Width - gameMap.Map.GridSize * squareSize - offset + (gameMap.Player.CurrPosition.X / gameMap.Map.TileSize) * squareSize + 1, offset + (gameMap.Player.CurrPosition.Y / gameMap.Map.TileSize) * squareSize + 1, squareSize - 2, squareSize - 2); // demands squareSize is odd and > 1
            myBuffer.Graphics.FillRectangle(Brushes.Blue, player);

            // guardian on minimap
            if (gameMap.Guardian.IsSpawned == true)
            {
                Rectangle enemy = new(DisplayRectangle.Width - gameMap.Map.GridSize * squareSize - offset + (gameMap.Guardian.CurrPosition.X / gameMap.Map.TileSize) * squareSize + 1, offset + (gameMap.Guardian.CurrPosition.Y / gameMap.Map.TileSize) * squareSize + 1, squareSize - 2, squareSize - 2); // demands squareSize is odd and > 1
                myBuffer.Graphics.FillRectangle(Brushes.Red, enemy);
            }

            // finish on minimap
            Rectangle finish = new(DisplayRectangle.Width - gameMap.Map.GridSize * squareSize - offset + (gameMap.Map.GridSize - 2) * squareSize + 1, offset + (gameMap.Map.GridSize - 2) * squareSize + 1, squareSize - 2, squareSize - 2); // demands squareSize is odd and > 1
            myBuffer.Graphics.FillRectangle(Brushes.LimeGreen, finish);

            // render everything
            myBuffer.Render();
            myBuffer.Dispose();
        }
        private void Timer_Tick(object sender, EventArgs e) // game loop
        {
            // render
            Render(gameMap);

            // keyboard input handling
            newX = gameMap.Player.CurrPosition.X;
            newY = gameMap.Player.CurrPosition.Y;

            if (keyW)
            {
                newX = Convert.ToInt32(gameMap.Player.CurrPosition.X + gameMap.Player.Direction.X * gameMap.Player.Step);
                newY = Convert.ToInt32(gameMap.Player.CurrPosition.Y - gameMap.Player.Direction.Y * gameMap.Player.Step);
            }

            if (keyS)
            {
                newX = Convert.ToInt32(gameMap.Player.CurrPosition.X - gameMap.Player.Direction.X * gameMap.Player.Step);
                newY = Convert.ToInt32(gameMap.Player.CurrPosition.Y + gameMap.Player.Direction.Y * gameMap.Player.Step);
            }

            // check if the player would skip the corner
            if (Math.Abs((newX / gameMap.Map.TileSize) - gameMap.Player.CurrPosition.X) != 1 || Math.Abs((newY / gameMap.Map.TileSize) - gameMap.Player.CurrPosition.Y) != 1)
            {
                if (gameMap.Map.Grid[(newX / gameMap.Map.TileSize), (newY / gameMap.Map.TileSize)] == Tile.Empty)
                {
                    gameMap.Player.CurrPosition = new Position(newX, newY);
                }
                else if (gameMap.Map.Grid[(newX / gameMap.Map.TileSize), (gameMap.Player.CurrPosition.Y / gameMap.Map.TileSize)] == 0) // slide to x side
                {
                    gameMap.Player.CurrPosition = new Position(newX, gameMap.Player.CurrPosition.Y);
                }
                else if (gameMap.Map.Grid[(gameMap.Player.CurrPosition.X / gameMap.Map.TileSize), (newY / gameMap.Map.TileSize)] == 0) // slide to y side
                {
                    gameMap.Player.CurrPosition = new Position(gameMap.Player.CurrPosition.X, newY);
                }
            }

            if (keyA)
            {
                gameMap.Player.Direction = gameMap.Player.Direction.RotateCounterclockwise(gameMap.Player.RotationSpeed * Math.PI / 180);
            }
            if (keyD)
            {
                gameMap.Player.Direction = gameMap.Player.Direction.RotateClockwise(gameMap.Player.RotationSpeed * Math.PI / 180);
            }

            // game over/victory conditions check
            if ((gameMap.Player.CurrPosition.X > ((gameMap.Map.GridSize - 2) * gameMap.Map.TileSize) && gameMap.Player.CurrPosition.Y > ((gameMap.Map.GridSize - 2) * gameMap.Map.TileSize)) || gameMap.Player.IsKilled) // end of the game
            {
                Timer.Stop();
                Menu.Visible = true;
                NewGame.Visible = true;
                Exit.Visible = true;

                if (gameMap.Player.IsKilled)
                {
                    GameOver.Visible = true;
                }
                else
                {
                    Victory.Visible = true;
                }
            }

            // killed condition check
            if ((gameMap.Guardian.CurrPosition.X / gameMap.Map.TileSize) == (gameMap.Player.CurrPosition.X / gameMap.Map.TileSize) && (gameMap.Guardian.CurrPosition.Y / gameMap.Map.TileSize) == (gameMap.Player.CurrPosition.Y / gameMap.Map.TileSize))
            {
                gameMap.Player.IsKilled = true;
            }

            // enemy spawntime check and spawn handle

            
            if (!gameMap.Guardian.IsSpawned && timeElapsed >= gameMap.Guardian.SpawnTime)
            {
                gameMap.Guardian.Spawn();
            }
            else if (timeElapsed < gameMap.Guardian.SpawnTime)
            {
                timeElapsed += Timer.Interval;
            }
            

            // enemy movement handle
            if (gameMap.Guardian.IsSpawned)
            {
                gameMap.Guardian.Move(gameMap);
            }
        }
        private void MazeRunner_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                keyW = false;
            else if (e.KeyCode == Keys.S)
                keyS = false;
            else if (e.KeyCode == Keys.A)
                keyA = false;
            else if (e.KeyCode == Keys.D)
                keyD = false;
        }
        private void NewGame_Click(object sender, EventArgs e)
        {
            gameMap = new GameMap(GRID_SIZE); // new game map
            Menu.Visible = false;
            NewGame.Visible = false;
            GameOver.Visible = false;
            Victory.Visible = false;
            Exit.Visible = false;
            timeElapsed = 0;
            Timer.Start();
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void MazeRunner_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                keyW = true;
            else if (e.KeyCode == Keys.S)
                keyS = true;
            else if (e.KeyCode == Keys.A)
                keyA = true;
            else if (e.KeyCode == Keys.D)
                keyD = true;
        }
    }
}