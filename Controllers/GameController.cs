using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HardwareKiller.Models;


namespace HardwareKiller.Controllers
{
    public enum GameState
    {
        Menu,
        Playing,
        GameOver,
        Won
    }

    public class GameController
    {
        private Game _game;
        public GameState State { get; private set; } = GameState.Menu;
        private int _initialItemCount;
        public int Score => _initialItemCount - _game.Items.Count;
        
        private readonly Graph _graph;
               private int _gameOverRow = -1;

        private readonly Image _bgImg;
        private readonly Image _cursorImg;
        private readonly Image _catImg;
        private readonly Image _catImgLeft;
        private readonly Image _catImgUp;
        private readonly Image _catImgDown;
        private readonly Dictionary<ItemType, Image> _itemImgs;
        private readonly Dictionary<HazardType, Image> _hazardImgs;

        private int _startCellX, _startCellY;
        private int _targetCellX, _targetCellY;
        private float _animOffsetX, _animOffsetY;
        private bool _isMoving;
        private const float StepPerTick = 32f / 1.5f; // клетка за 2 тика
        private bool _holdUp, _holdDown, _holdLeft, _holdRight;

        public GameController()
        {
            _game = new Game();
            _graph = new Graph(_game.GridWidth, _game.GridHeight);
            _initialItemCount = _game.Items.Count;

            _bgImg = Image.FromFile("Images\\windows-bg.jpg");
            _cursorImg = Image.FromFile("Images\\cursor.png");
            _catImg = Image.FromFile("Images\\cat.png");
            _catImgLeft = (Image)_catImg.Clone();
            _catImgLeft.RotateFlip(RotateFlipType.RotateNoneFlipX);
            _catImgUp = (Image)_catImg.Clone();
            _catImgUp.RotateFlip(RotateFlipType.Rotate270FlipNone);
            _catImgDown = (Image)_catImg.Clone();
            _catImgDown.RotateFlip(RotateFlipType.Rotate90FlipNone);

            _itemImgs = new Dictionary<ItemType, Image>
            {
                [ItemType.File] = Image.FromFile("Images\\file.png"),
                [ItemType.Folder] = Image.FromFile("Images\\folder.png"),
                // [ItemType.Custom] = Image.FromFile("Images\\custom.png")
            };
            _hazardImgs = new Dictionary<HazardType, Image>
            {
                [HazardType.Shootdown] = Image.FromFile("Images\\shootdown.png"),
                [HazardType.TrashBox] = Image.FromFile("Images\\trash-box.png"),
                [HazardType.BlueWindows] = Image.FromFile("Images\\blue-windows.png"),
                [HazardType.BurnedPixel] = null
            };

            _startCellX = _targetCellX = _game.Player.X;
            _startCellY = _targetCellY = _game.Player.Y;
            _animOffsetX = _animOffsetY = 0;
            _isMoving = false;
        }

        public void StartGame()
        {
            State = GameState.Playing;
        }
        
        

        public void RestartGame()
        {
            _game = new Game();
            _initialItemCount = _game.Items.Count;
            _startCellX = _targetCellX = _game.Player.X;
            _startCellY = _targetCellY = _game.Player.Y;
            _animOffsetX = _animOffsetY = 0;
            _isMoving = false;
            State = GameState.Playing;
        }

        public void OnKeyDown(Keys key)
        {
            if (State != GameState.Playing) return;
            switch (key)
            {
                case Keys.W:
                case Keys.Up: _holdUp = true; break;
                case Keys.S:
                case Keys.Down: _holdDown = true; break;
                case Keys.A:
                case Keys.Left: _holdLeft = true; break;
                case Keys.D:
                case Keys.Right: _holdRight = true; break;
            }
        }


        public void OnKeyUp(Keys key)
        {
            switch (key)
            {
                case Keys.W:
                case Keys.Up: _holdUp = false; break;
                case Keys.S:
                case Keys.Down: _holdDown = false; break;
                case Keys.A:
                case Keys.Left: _holdLeft = false; break;
                case Keys.D:
                case Keys.Right: _holdRight = false; break;
            }
        }


        public void Update()
        {
            if (State != GameState.Playing) return;

            // 1) Обработка хода игрока
            if (!_isMoving)
            {
                if (_holdUp)    StartMove(0, -1);
                else if (_holdDown)  StartMove(0, 1);
                else if (_holdLeft)  StartMove(-1, 0);
                else if (_holdRight) StartMove(1, 0);
            }

            // 2) Анимация хода
            if (_isMoving)
            {
                float dirX = Math.Sign(_targetCellX - _startCellX);
                float dirY = Math.Sign(_targetCellY - _startCellY);

                _animOffsetX += dirX * StepPerTick;
                _animOffsetY += dirY * StepPerTick;

                if (Math.Abs(_animOffsetX) >= _game.CellSize ||
                    Math.Abs(_animOffsetY) >= _game.CellSize)
                {
                    // Завершаем ход
                    _startCellX = _targetCellX;
                    _startCellY = _targetCellY;
                    _game.MovePlayer((int)dirX, (int)dirY);

                    _animOffsetX = _animOffsetY = 0;
                    _isMoving = false;

                    // Проверяем столкновения и сборы
                    CheckCollisions();

                    // Двигаем котов по кратчайшему пути
                    
                }
            }
            MoveCats();
        }
        
        private void MoveCats()
        {
            foreach (var cat in _game.Cats)
            {
                var path = _graph.ShortestPath(cat.X, cat.Y, _game.Player.X, _game.Player.Y);
                if (path.Count > 1)
                {
                    var next = path[1];
                    // не ходим через «выгоревшие пиксели»
                    if (!_game.Obstacles.Any(o => o.X == next.X && o.Y == next.Y))
                        cat.MoveTo(next.X, next.Y);
                }
                if (cat.X == _game.Player.X && cat.Y == _game.Player.Y)
                {
                    State = GameState.GameOver;
                    _gameOverRow = cat.Y;
                }
            }
        }



        private void CheckCollisions()
        {
            var hitItem = _game.Items.FirstOrDefault(i => i.X == _game.Player.X && i.Y == _game.Player.Y);
            if (hitItem != null) _game.Items.Remove(hitItem);

            if (_game.Hazards.Any(h => h.X == _game.Player.X && h.Y == _game.Player.Y))
                State = GameState.GameOver;

            if (!_game.Items.Any())
                State = GameState.Won;
        }

        public void Draw(Graphics g)
        {
            int cs = _game.CellSize;


            var clip = g.VisibleClipBounds;
            g.DrawImage(_bgImg, new Rectangle(0, 0, (int)clip.Width, (int)clip.Height));
            
            if (State == GameState.GameOver && _gameOverRow >= 0)
            {
                using var brush = new SolidBrush(Color.LightGreen);
                g.FillRectangle(brush, 0, _gameOverRow * cs, _game.GridWidth * cs, cs);
            }

            foreach (var o in _game.Obstacles)
                g.FillRectangle(Brushes.Black, o.X * cs, o.Y * cs, cs, cs);

            foreach (var it in _game.Items)
                g.DrawImage(_itemImgs[it.Type], it.X * cs, it.Y * cs, cs, cs);

            foreach (var hz in _game.Hazards)
            {
                var img = _hazardImgs[hz.Type];
                if (img != null)
                    g.DrawImage(img, hz.X * cs, hz.Y * cs, cs, cs);
                else
                    g.FillRectangle(Brushes.Red, hz.X * cs, hz.Y * cs, cs, cs);
            }

            int catSize = (int)(cs * 1.4f), catOff = (catSize - cs) / 2;
            foreach (var cat in _game.Cats)
            {
                int dx = cat.X - cat.PrevX, dy = cat.Y - cat.PrevY;
                Image cimg = _catImg;
                if (dx < 0)      cimg = _catImgLeft;
                else if (dy < 0) cimg = _catImgUp;
                else if (dy > 0) cimg = _catImgDown;

                g.DrawImage(cimg,
                    cat.X * cs - catOff,
                    cat.Y * cs - catOff,
                    catSize, catSize);
            }


            float px = _startCellX * cs + _animOffsetX;
            float py = _startCellY * cs + _animOffsetY;
            g.DrawImage(_cursorImg, px, py, cs, cs);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        }

        private void StartMove(int dx, int dy)
        {
            int tx = _startCellX + dx;
            int ty = _startCellY + dy;
            if (tx < 0 || tx >= _game.GridWidth || ty < 0 || ty >= _game.GridHeight)
                return;
            if (_game.Obstacles.Any(o => o.X == tx && o.Y == ty))
                return;

            _targetCellX = tx;
            _targetCellY = ty;
            _animOffsetX = _animOffsetY = 0;
            _isMoving = true;
        }
    }
}