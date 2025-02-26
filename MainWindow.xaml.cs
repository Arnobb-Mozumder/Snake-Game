using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snakee
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }
        };

        private readonly Dictionary<Direction, int> directionToRotation = new()
        {
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Left, 270 },
            { Direction.Down, 180 }
        };

        private readonly int rows = 30, cols = 30;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        private int highScore;
        private bool waitingForKeyPress;
        private int initialSpeed = 150; 
        private int minimumSpeed = 60;  


        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
            LoadHighScore();
            this.KeyDown += Window_KeyDown;
            this.Focusable = true;
            this.Focus();
            Keyboard.Focus(this);
            ShowStartPrompt();  
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image { Source = Images.Empty, RenderTransformOrigin = new Point(0.5, 0.5) };
                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                int currentSpeed = CalculateSpeed(gameState.Score);
                await Task.Delay(currentSpeed);
                gameState.Move();
                Draw();
            }
            await DrawDeadSnake();
            UpdateHighScore();
            gameRunning = false;
            ShowStartPrompt();  
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"SCORE : {gameState.Score}  HIGH SCORE : {highScore}";
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;
            image.RenderTransform = new RotateTransform(directionToRotation[gameState.Dir]);
        }

        private async Task ShowCountDown()
        {
            Overlay.Visibility = Visibility.Visible;
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
            OverlayText.Text = "Go!";
            await Task.Delay(500);
            Overlay.Visibility = Visibility.Hidden;
        }

        private async Task DrawDeadSnake()
        {
            var positions = new List<Position>(gameState.SnakePositions());
            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                gridImages[pos.Row, pos.Col].Source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                await Task.Delay(50);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (waitingForKeyPress)
            {
                waitingForKeyPress = false; 
                StartGame();
                return;
            }

            if (!gameRunning) return;
            if (gameState.GameOver) return;

            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
            }
        }

        private async void StartGame()
        {
            if (gameRunning) return;

            gameRunning = true;

            if (gameState.GameOver)
            {
                ResetGame();
            }

            await ShowCountDown();
            await GameLoop();
        }

        private void ResetGame()
        {
            gameState = new GameState(rows, cols);
            DrawGrid();
        }

        private void ShowStartPrompt()
        {
            waitingForKeyPress = true; 
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Press any key to continue";
        }

        private void LoadHighScore()
        {
            try
            {
                if (System.IO.File.Exists("highscore.txt"))
                {
                    string scoreString = System.IO.File.ReadAllText("highscore.txt");
                    if (int.TryParse(scoreString, out int score))
                    {
                        highScore = score;
                    }
                }
            }
            catch
            {
                highScore = 0;
            }
        }

        private void SaveHighScore()
        {
            try
            {
                System.IO.File.WriteAllText("highscore.txt", highScore.ToString());
            }
            catch { }
        }

        private void UpdateHighScore()
        {
            if (gameState.Score > highScore)
            {
                highScore = gameState.Score;
                SaveHighScore();
            }
        }

        private int CalculateSpeed(int score)
        {
            
            int speed=initialSpeed-(score*5); 
            if (speed<minimumSpeed)
            {
                speed+=1; 
            }
            return speed;
        }


        public static class Images
        {
            public static BitmapImage Empty = new BitmapImage(new Uri(@"C:\Users\amozu\source\repos\Snakee\Assets\Empty.png", UriKind.Absolute));
            public static BitmapImage Body = new BitmapImage(new Uri(@"C:\Users\amozu\source\repos\Snakee\Assets\Body.png", UriKind.Absolute));
            public static BitmapImage Food = new BitmapImage(new Uri(@"C:\Users\amozu\source\repos\Snakee\Assets\Food.png", UriKind.Absolute));
            public static BitmapImage Head = new BitmapImage(new Uri(@"C:\Users\amozu\source\repos\Snakee\Assets\Head3.png", UriKind.Absolute));
            public static BitmapImage DeadHead = new BitmapImage(new Uri(@"C:\Users\amozu\source\repos\Snakee\Assets\DeadHead.png", UriKind.Absolute));
            public static BitmapImage DeadBody = new BitmapImage(new Uri(@"C:\Users\amozu\source\repos\Snakee\Assets\DeadBody.png", UriKind.Absolute));
        }
    }
}
